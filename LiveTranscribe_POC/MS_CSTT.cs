using System;
using System.Windows.Forms;
using Microsoft.CognitiveServices.Speech;
using System.Threading;
using System.Threading.Tasks;

namespace LiveTranscribe_POC
{
    public partial class MS_CSTT : Form
    {
        private string azureSpeechKey;
        private string azureSpeechRegion;
        private bool isRecording = false;
        private SpeechRecognizer recognizer;
        private CancellationTokenSource cancellationTokenSource;

        public MS_CSTT()
        {
            InitializeComponent();

            // Retrieve the API keys from environment variables
            azureSpeechKey = Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY");
            azureSpeechRegion = Environment.GetEnvironmentVariable("AZURE_SPEECH_REGION");

            if (string.IsNullOrEmpty(azureSpeechKey) || string.IsNullOrEmpty(azureSpeechRegion))
            {
                MessageBox.Show("API keys or credentials are not set in environment variables.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private async void btnStartStop_Click(object sender, EventArgs e)
        {
            if (isRecording)
            {
                cancellationTokenSource.Cancel();
                await recognizer.StopContinuousRecognitionAsync();
                isRecording = false;
                btnStartStop.Text = "Start Recording";
                lblStatus.Text = "Stopped";
            }
            else
            {
                cancellationTokenSource = new CancellationTokenSource();
                RecognizeSpeech(cancellationTokenSource.Token);
                isRecording = true;
                btnStartStop.Text = "Stop Recording";
                lblStatus.Text = "Listening...";
            }
        }

        private async void RecognizeSpeech(CancellationToken cancellationToken)
        {
            var speechConfig = SpeechConfig.FromSubscription(azureSpeechKey, azureSpeechRegion);
            recognizer = new SpeechRecognizer(speechConfig);

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Invoke((Action)(() =>
                    {
                        txtTranscribed.Text += e.Result.Text + Environment.NewLine;
                    }));
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                if (e.Reason == CancellationReason.Error)
                {
                    Invoke((Action)(() =>
                    {
                        MessageBox.Show($"Recognition canceled: {e.ErrorDetails}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
            };

            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            // Handle cancellation
            cancellationToken.Register(async () =>
            {
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            });
        }
    }
}
