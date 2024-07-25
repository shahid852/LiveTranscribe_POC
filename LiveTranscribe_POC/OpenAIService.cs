using System;
using System.Net.Http;
using System.Threading.Tasks;
using GenerativeAI.Models;
using OpenAI;
using OpenAI.Chat;
using Polly;
using Polly.Retry;

public class OpenAIService
{
    private readonly OpenAIClient _client;
    private readonly AsyncRetryPolicy _retryPolicy;

    ChatClient client;

    public OpenAIService(string openApi_key, string model = "gpt-4o-mini")
    {
        client = new ChatClient(model: model, credential: openApi_key);
    }

    public async Task<string> CorrectGrammarWithOpenAI(string text)
    {

        try
        {
            
            ChatCompletion completion = await client.CompleteChatAsync(
            $"The text is the transcript from a speech recognition software that does not always work " +
            $"flawlessly. Do not change the wording, only correct obvious recognition errors. " +
            $"Give me the corrected text without anything before or after so that I can copy-paste it " +
            $"one-to-one. Do not even add quotation marks.:\n\n{text}"
            );

            //if (completion.choices != null && response.Choices.Count > 0)
            //{
            return completion.ToString();
            //}
            //else
            //{
            //    LogError("Error: No candidates returned.");
            //    return text;
            //}
        }
        catch (HttpRequestException httpEx)
        {
            LogError($"HttpRequestException: {httpEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            LogError($"Exception: {ex.Message}");
            throw;
        }
    }

    private void LogError(string message)
    {
        // Implement your logging logic here
        Console.WriteLine(message);
    }
}
