using Google.Cloud.Speech.V1;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using GenerativeAI.Models;
using GenerativeAI.Types;

namespace CoreLibrary
{
    public class SpeechRecognitionService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private string googleApplicationCredentials;
        private string openaiApiKey;
        private string googleGeminiApiKey;
        private string azureApiKey;
        private string azureEndpoint;
        private string lastTranscribedText = "";
        private string interimText = "";
        private string uncorrectedText = "";
        private string correctedText = "";
        private List<string> correctionTriggers = new List<string> { ".", ",", "\n" };

        public event Action<string> OnInterimResult;
        public event Action<string> OnFinalResult;
        public event Action<string> OnCorrectionComplete;

        public SpeechRecognitionService()
        {
            // Retrieve the API keys from environment variables
            googleApplicationCredentials = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            googleGeminiApiKey = Environment.GetEnvironmentVariable("GOOGLE_GENAI_API_KEY");
            azureApiKey = Environment.GetEnvironmentVariable("AZURE_API_KEY");
            azureEndpoint = Environment.GetEnvironmentVariable("AZURE_ENDPOINT");

            if (string.IsNullOrEmpty(googleApplicationCredentials) || string.IsNullOrEmpty(openaiApiKey) || string.IsNullOrEmpty(googleGeminiApiKey) || string.IsNullOrEmpty(azureApiKey) || string.IsNullOrEmpty(azureEndpoint))
            {
                throw new InvalidOperationException("One or more API keys or endpoints are not set in environment variables.");
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", googleApplicationCredentials);
        }

        public async Task RecognizeSpeech(Stream audioStream, CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];
            try
            {
                var speechClient = await SpeechClient.CreateAsync();
                var streamingCall = speechClient.StreamingRecognize();

                // Write the initial request with the config.
                await streamingCall.WriteAsync(new StreamingRecognizeRequest
                {
                    StreamingConfig = new StreamingRecognitionConfig
                    {
                        Config = new RecognitionConfig
                        {
                            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                            SampleRateHertz = 16000,
                            LanguageCode = "en-US"
                        },
                        InterimResults = true
                    }
                });

                var responseReader = Task.Run(async () =>
                {
                    var responseStream = streamingCall.GetResponseStream();
                    while (await responseStream.MoveNextAsync(cancellationToken))
                    {
                        var response = responseStream.Current;
                        foreach (var result in response.Results)
                        {
                            foreach (var alternative in result.Alternatives)
                            {
                                if (result.IsFinal)
                                {
                                    ProcessFinalResult(alternative.Transcript);
                                }
                                else
                                {
                                    ProcessInterimResult(alternative.Transcript);
                                }
                            }
                        }
                    }
                }, cancellationToken);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        int bytesRead;
                        lock (audioStream)
                        {
                            bytesRead = audioStream.Read(buffer, 0, buffer.Length);
                        }

                        if (bytesRead > 0)
                        {
                            await streamingCall.WriteAsync(new StreamingRecognizeRequest
                            {
                                AudioContent = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead)
                            });
                        }
                        else
                        {
                            await Task.Delay(100, cancellationToken);
                        }
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                {
                    LogError("Stream cancelled.");
                }
                catch (IOException ex)
                {
                    LogError($"Stream error: {ex.Message}");
                }
                finally
                {
                    try
                    {
                        await streamingCall.WriteCompleteAsync();
                    }
                    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                    {
                        LogError("Stream WriteCompleteAsync cancelled.");
                    }

                    await responseReader;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in RecognizeSpeech: {ex.Message}");
            }
        }

        private void ProcessInterimResult(string interimText)
        {
            try
            {
                this.interimText = interimText;
                OnInterimResult?.Invoke(lastTranscribedText + " " + interimText);
            }
            catch (Exception ex)
            {
                LogError($"Error in ProcessInterimResult: {ex.Message}");
            }
        }

        private void ProcessFinalResult(string finalText)
        {
            try
            {
                if (!string.IsNullOrEmpty(finalText.Trim()))
                {
                    var textToAdd = finalText.Replace("new paragraph", "\n");
                    lastTranscribedText += " " + textToAdd;
                    uncorrectedText += " " + textToAdd;

                    if (ContainsCorrectionTrigger(textToAdd))
                    {
                        SendForCorrection(uncorrectedText.Trim());
                        correctedText += uncorrectedText;
                        lastTranscribedText = lastTranscribedText.Replace(uncorrectedText, "").Trim();
                        uncorrectedText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in ProcessFinalResult: {ex.Message}");
            }
        }

        private bool ContainsCorrectionTrigger(string text)
        {
            try
            {
                foreach (var trigger in correctionTriggers)
                {
                    if (text.Contains(trigger))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError($"Error in ContainsCorrectionTrigger: {ex.Message}");
                return false;
            }
        }

        private async void SendForCorrection(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    string corrected = await CorrectGrammar(text);
                    correctedText += corrected;
                    OnCorrectionComplete?.Invoke(correctedText);
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in SendForCorrection: {ex.Message}");
            }
        }

        private async Task<string> CorrectGrammar(string text)
        {
            try
            {
                string selectedService = "Gemini"; // For simplicity, hardcoded to Gemini. You can change it as needed.

                switch (selectedService)
                {
                    case "OpenAI":
                        return await CorrectGrammarWithOpenAI(text);
                    case "Gemini":
                        return await CorrectGrammarWithGemini(text);
                    case "Microsoft":
                        return await CorrectGrammarWithMicrosoft(text);
                    default:
                        throw new InvalidOperationException("Unknown correction service selected.");
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in CorrectGrammar: {ex.Message}");
                return text;
            }
        }

        private async Task<string> CorrectGrammarWithOpenAI(string text)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                var client = new RestClient("https://api.openai.com/v1/chat/completions");

                var request = new RestRequest { Method = Method.Post };
                request.AddHeader("Authorization", $"Bearer {apiKey}");
                request.AddHeader("Content-Type", "application/json");

                var data = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = "You are a helpful assistant that corrects grammatical errors."
                        },
                        new
                        {
                            role = "user",
                            content = $"Rewrite the following sentence and fix any grammar issues:\n\n{text}"
                        }
                    },
                    max_tokens = 256,
                    temperature = 0.4,
                };

                request.AddJsonBody(data);

                var policy = Policy.Handle<HttpRequestException>()
                                   .Or<Exception>() // Handle any other potential exceptions
                                   .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                       (exception, timeSpan, retryCount, context) =>
                                       {
                                           LogError($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                                       });

                var response = await policy.ExecuteAsync(async () =>
                {
                    var httpResponse = await client.ExecuteAsync(request);
                    if (!httpResponse.IsSuccessful)
                    {
                        LogError($"HTTP Request Failed: {httpResponse.StatusCode}, {httpResponse.Content}");
                        throw new HttpRequestException(httpResponse.ErrorMessage);
                    }
                    return httpResponse.Content;
                });

                dynamic jsonResult = JsonConvert.DeserializeObject(response);

                if (jsonResult.choices != null && jsonResult.choices.Count > 0)
                {
                    return jsonResult.choices[0].message.content.ToString().Trim();
                }
                else
                {
                    LogError("Error: No candidates returned.");
                    return text;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in CorrectGrammarWithOpenAI: {ex.Message}");
                return text;
            }
        }

        private async Task<string> CorrectGrammarWithGemini(string text)
        {
            try
            {
                var client = new GenerativeModel(googleGeminiApiKey);
                var prompt = $"Rewrite the following sentence and fix any grammar issues:\n\n{text}";
                var response = await client.GenerateContentAsync(prompt);

                if (!string.IsNullOrEmpty(response))
                {
                    return response.Trim();
                }
                else
                {
                    LogError("Error: No candidates returned.");
                    return text;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in CorrectGrammarWithGemini: {ex.Message}");
                return text;
            }
        }

        private async Task<string> CorrectGrammarWithMicrosoft(string text)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("AZURE_API_KEY");
                var endpoint = Environment.GetEnvironmentVariable("AZURE_ENDPOINT");

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
                {
                    throw new InvalidOperationException("Azure API Key or Endpoint is not set in environment variables.");
                }

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                var uri = $"{endpoint}/text/analytics/v3.0/spellcheck";

                var requestContent = new
                {
                    text = text
                };

                var jsonContent = JsonConvert.SerializeObject(requestContent);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var policy = Policy.Handle<HttpRequestException>()
                                   .Or<Exception>() // Handle any other potential exceptions
                                   .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                       (exception, timeSpan, retryCount, context) =>
                                       {
                                           LogError($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                                       });

                var response = await policy.ExecuteAsync(async () =>
                {
                    var httpResponse = await client.PostAsync(uri, content);
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        LogError($"HTTP Request Failed: {httpResponse.StatusCode}, {await httpResponse.Content.ReadAsStringAsync()}");
                        throw new HttpRequestException($"HTTP Request Failed: {httpResponse.StatusCode}");
                    }
                    return httpResponse.Content.ReadAsStringAsync();
                });

                dynamic jsonResult = JsonConvert.DeserializeObject(response.Result);

                if (jsonResult.documents != null && jsonResult.documents.Count > 0)
                {
                    return jsonResult.documents[0].detectedCorrections.ToString().Trim();
                }
                else
                {
                    LogError("Error: No corrections returned.");
                    return text;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in CorrectGrammarWithMicrosoft: {ex.Message}");
                return text;
            }
        }

        private void LogError(string error)
        {
            string logFilePath = "error_log.txt";
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {error}");
            }
        }
    }
}
