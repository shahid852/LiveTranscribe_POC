using System;
using System.Windows.Forms;
using Google.Cloud.Speech.V1;
using Newtonsoft.Json;
using RestSharp;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using NAudio.Wave;

namespace LiveTranscribe_POC
{
    public partial class MainForm : Form
    {
        private string openAiApiKey;
        private string googleApplicationCredentials;
        private string latestCorrection = "";
        private bool isRecording = false;
        private WaveInEvent waveIn;
        private BufferedWaveProvider waveProvider;
        private CancellationTokenSource cancellationTokenSource;

        public MainForm()
        {
            InitializeComponent();
            SetupHotkey();

            // Retrieve the API keys from environment variables
            openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            googleApplicationCredentials = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

            if (string.IsNullOrEmpty(openAiApiKey) || string.IsNullOrEmpty(googleApplicationCredentials))
            {
                MessageBox.Show("API keys or credentials are not set in environment variables.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", googleApplicationCredentials);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 1)
            };
            waveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
            waveIn.DataAvailable += (s, a) => waveProvider.AddSamples(a.Buffer, 0, a.BytesRecorded);
        }

        private async void RecognizeSpeech(CancellationToken cancellationToken)
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
                    foreach (var result in responseStream.Current.Results)
                    {
                        foreach (var alternative in result.Alternatives)
                        {
                            Invoke((Action)(() =>
                            {
                                var text = alternative.Transcript;
                                txtTranscribed.Text += text + Environment.NewLine;
                                if (text.Contains("new paragraph"))
                                {
                                    text = text.Replace("new paragraph", "").Trim();
                                    var correctedText = CorrectGrammar(text);
                                    latestCorrection = correctedText;
                                    //txtCorrected.Text += correctedText + Environment.NewLine;
                                }
                            }));
                        }
                    }
                }
            }, cancellationToken);

            byte[] buffer = new byte[waveProvider.BufferLength];
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead = waveProvider.Read(buffer, 0, buffer.Length);
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

            await streamingCall.WriteCompleteAsync();
            await responseReader;
        }

        private string CorrectGrammar(string text)
        {
            var client = new RestClient("https://api.openai.com/v1/completions");
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Authorization", $"Bearer {openAiApiKey}");
            request.AddHeader("Content-Type", "application/json");
            var body = new
            {
                model = "text-davinci-003",
                prompt = $"Rewrite the following sentence and fix any grammar issues:\n\n{text}",
                max_tokens = 256,
                temperature = 0.4
            };
            request.AddJsonBody(body);

            RestResponse response = client.Execute(request);
            dynamic result = JsonConvert.DeserializeObject(response.Content);
            return result.choices[0].text.Trim();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
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

        private void btnPaste_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(latestCorrection);
            SendKeys.SendWait("^v");
            Clipboard.Clear();
        }

        private void SetupHotkey()
        {
            RegisterHotKey(this.Handle, 1, MOD_CONTROL, (int)Keys.Q);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY && (int)m.WParam == 1)
            {
                //btnPaste.PerformClick();
            }
            base.WndProc(ref m);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const uint MOD_CONTROL = 0x0002;

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 1);
        }
    }
}
