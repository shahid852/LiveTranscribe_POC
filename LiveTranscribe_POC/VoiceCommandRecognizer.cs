using Microsoft.CognitiveServices.Speech;
using System;
using System.Speech.Recognition;

namespace LiveTranscribe_POC
{
    public static class VoiceCommandRecognizer
    {
        public static event Action<string> OnVoiceCommandRecognized;

        private static SpeechRecognitionEngine recognizer;

        public static void StartListening()
        {
            recognizer = new SpeechRecognitionEngine();

            // Create a grammar for recognizing the command "start dictation", "transfer my text"
            var choices = new Choices();
            choices.Add("start dictation");
            choices.Add("transfer my text");

            var grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(choices);

            var grammar = new System.Speech.Recognition.Grammar(grammarBuilder);

            recognizer.LoadGrammar(grammar);

            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private static void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.8)
            {
                OnVoiceCommandRecognized?.Invoke(e.Result.Text);
            }
        }

        public static void StopListening()
        {
            if (recognizer != null)
            {
                recognizer.RecognizeAsyncStop();
                recognizer.Dispose();
            }
        }
    }
}
