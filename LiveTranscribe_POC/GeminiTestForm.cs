using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GenerativeAI.Models;
using GenerativeAI.Types;
using GenerativeAI;

namespace LiveTranscribe_POC
{
    public partial class GeminiTestForm : Form
    {
        private string googleGeminiApiKey;

        public GeminiTestForm()
        {
            InitializeComponent();
            googleGeminiApiKey = Environment.GetEnvironmentVariable("GOOGLE_GENAI_API_KEY");
            if (string.IsNullOrEmpty(googleGeminiApiKey))
            {
                MessageBox.Show("Google Gemini API Key is not set in environment variables.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            string inputText = txtInput.Text;
            if (string.IsNullOrEmpty(inputText))
            {
                MessageBox.Show("Please enter some text to correct.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string correctedText = await CorrectGrammarWithGemini(inputText);
            txtOutput.Text = correctedText;
        }

        private async Task<string> CorrectGrammarWithGemini(string text)
        {
            try
            {
                var client = new GenerativeModel(googleGeminiApiKey);
                //{
                //    Model = "models/text-bison-001",
                //    Config = new GenerationConfig
                //    {
                //        Temperature = 0.4,
                //        CandidateCount = 1,
                //        TopK = 40,
                //        TopP = 0.95,
                //        MaxOutputTokens = 256
                //    }
                //};

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

        private void LogError(string error)
        {
            string logFilePath = "error_log.txt";
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {error}");
            }
        }
    }
}
