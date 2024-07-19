using Google.Cloud.Speech.V1;
using Newtonsoft.Json;
using Polly;
using NAudio.Wave;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grpc.Core;
using GenerativeAI.Models;
using GenerativeAI.Types;

namespace LiveTranscribe_POC
{
    public partial class GSTT : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private string googleApplicationCredentials;
        private string openaiApiKey;
        private string googleGeminiApiKey;
        private string azureApiKey;
        private string azureEndpoint;
        private bool isRecording = false;
        private WaveInEvent waveIn;
        private MemoryStream memoryStream;
        private CancellationTokenSource cancellationTokenSource;
        private SpeechClient speechClient;
        private string lastTranscribedText = "";
        private string interimText = "";
        private OverlayWindow overlayWindow;
        private ComboBox correctionServiceDropdown;
        private string[] correctionServices = { "Gemini", "OpenAI", "Microsoft" };
        private List<string> correctionTriggers = new List<string> { ".", ",", "\n" };

        public GSTT()
        {
            InitializeComponent();

            // Retrieve the API keys from environment variables
            googleApplicationCredentials = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            googleGeminiApiKey = Environment.GetEnvironmentVariable("GOOGLE_GENAI_API_KEY");
            azureApiKey = Environment.GetEnvironmentVariable("AZURE_API_KEY");
            azureEndpoint = Environment.GetEnvironmentVariable("AZURE_ENDPOINT");

            if (string.IsNullOrEmpty(googleApplicationCredentials) || string.IsNullOrEmpty(openaiApiKey) || string.IsNullOrEmpty(googleGeminiApiKey) || string.IsNullOrEmpty(azureApiKey) || string.IsNullOrEmpty(azureEndpoint))
            {
                MessageBox.Show("Google Application Credentials, OpenAI API Key, Google Gemini API Key, Azure API Key, or Azure Endpoint are not set in environment variables.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", googleApplicationCredentials);

            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 1)
            };
            memoryStream = new MemoryStream();
            waveIn.DataAvailable += OnDataAvailable;

            overlayWindow = new OverlayWindow();
            overlayWindow.TransferClicked += TransferTextToFocusedWindow;

            // Add correction service dropdown
            correctionServiceDropdown = new ComboBox();
            correctionServiceDropdown.Items.AddRange(correctionServices);
            correctionServiceDropdown.SelectedIndex = 0; // Default to Gemini
            correctionServiceDropdown.Location = new Point(350, 60); // Adjust the location as needed
            this.Controls.Add(correctionServiceDropdown);

            // Add event listener for voice command "start dictation"
            VoiceCommandRecognizer.OnVoiceCommandRecognized += VoiceCommandRecognized;
            VoiceCommandRecognizer.StartListening();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private async void btnStartStop_Click(object sender, EventArgs e)
        {
            if (isRecording)
            {
                cancellationTokenSource.Cancel();
                waveIn.StopRecording();
                isRecording = false;
                btnStartStop.Text = "Start Recording";
                lblStatus.Text = "Stopped";
            }
            else
            {
                cancellationTokenSource = new CancellationTokenSource();
                waveIn.StartRecording();
                RecognizeSpeech(cancellationTokenSource.Token);
                isRecording = true;
                btnStartStop.Text = "Stop Recording";
                lblStatus.Text = "Listening...";
            }
        }

        private async void RecognizeSpeech(CancellationToken cancellationToken)
        {
            speechClient = await SpeechClient.CreateAsync();
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
                            Invoke((Action)(() =>
                            {
                                if (result.IsFinal)
                                {
                                    ProcessFinalResult(alternative.Transcript);
                                }
                                else
                                {
                                    ProcessInterimResult(alternative.Transcript);
                                }
                            }));
                        }
                    }
                }
            }, cancellationToken);

            byte[] buffer = new byte[4096]; // Adjust buffer size for more efficient reading
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (memoryStream.Length > 0)
                    {
                        byte[] audioBytes;
                        lock (memoryStream)
                        {
                            audioBytes = memoryStream.ToArray();
                            memoryStream.SetLength(0);
                        }

                        await streamingCall.WriteAsync(new StreamingRecognizeRequest
                        {
                            AudioContent = Google.Protobuf.ByteString.CopyFrom(audioBytes)
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
                // Handle the cancellation gracefully
                LogError("Stream cancelled.");
            }
            catch (IOException ex)
            {
                // Handle I/O exceptions that might occur during streaming
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
                    // Ignore the cancellation exception on WriteCompleteAsync
                    LogError("Stream WriteCompleteAsync cancelled.");
                }

                await responseReader;
            }
        }

        private void ProcessInterimResult(string interimText)
        {
            this.interimText = interimText;
            txtTranscribed.Text = lastTranscribedText + " " + interimText;
            overlayWindow.UpdateInterimText(lastTranscribedText + " " + interimText);
        }

        private void ProcessFinalResult(string finalText)
        {
            if (!string.IsNullOrEmpty(finalText.Trim()))
            {
                lastTranscribedText += " " + finalText;
                txtTranscribed.Text = lastTranscribedText;
                overlayWindow.UpdateInterimText(lastTranscribedText);

                if (ContainsCorrectionTrigger(finalText))
                {
                    SendForCorrection(lastTranscribedText);
                }
            }
        }

        private bool ContainsCorrectionTrigger(string text)
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

        private async void SendForCorrection(string text)
        {
            if (!string.IsNullOrEmpty(text.Trim()))
            {
                string correctedText = await CorrectGrammar(text);
                txtCorrected.Text = correctedText;
                overlayWindow.UpdateCorrectedText(correctedText);
                lastTranscribedText = "";
            }
        }

        private async Task<string> CorrectGrammar(string text)
        {
            string selectedService = correctionServiceDropdown.SelectedItem.ToString();

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

        private async Task<string> CorrectGrammarWithOpenAI(string text)
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

        private void LogError(string error)
        {
            string logFilePath = "error_log.txt";
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {error}");
            }
        }

        private void TransferTextToFocusedWindow(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            Clipboard.SetText(text);
            SendKeys.SendWait("^v");
            Clipboard.Clear();
        }

        private void VoiceCommandRecognized(string command)
        {
            if (command.Equals("start dictation", StringComparison.OrdinalIgnoreCase))
            {
                ShowOverlayWindow();
            }
        }

        private void ShowOverlayWindow()
        {
            if (overlayWindow != null)
            {
                overlayWindow.ShowDialog(this);
            }
        }

        private void GSTT_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000, "LiveTranscribe POC", "Running in background", ToolTipIcon.Info);
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void Show_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
