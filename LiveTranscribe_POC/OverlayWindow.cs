using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveTranscribe_POC
{
    public partial class OverlayWindow : Form
    {
        public bool IsProgrammaticChange { get; set; }

        public event Action<string> TransferClicked;
        public GSTT parentWindow { get; set; }
        public int transcribedIndexOverlay { get; set; }

        public OverlayWindow()
        {
            InitializeComponent();
        }

        public void UpdateInterimFinalText(string text, bool IsInterim = true)
        {

            lblInterimText.Text = text;

            if (IsInterim)
            {
                if (txtCorrected.Text == "" || transcribedIndexOverlay == 0)
                    txtCorrected.Text = text;
                else
                    txtCorrected.Text = txtCorrected.Text.Substring(0, transcribedIndexOverlay) + text;
            }
            else
            {

                if (txtCorrected.Text == "" || transcribedIndexOverlay == 0)
                    txtCorrected.Text = text;
                else
                    txtCorrected.Text = txtCorrected.Text.Substring(0, transcribedIndexOverlay) + text;

                transcribedIndexOverlay = txtCorrected.Text.Length; // update after changes
            }
        }

        public void UpdateCorrectedText(string text)
        {
            IsProgrammaticChange = true;
            int index = txtCorrected.Text.IndexOf(text);
            Console.WriteLine(index);
            if (index < 0) throw new Exception("text not found");
            txtCorrected.Text = txtCorrected.Text.Replace(text, parentWindow.correctionQueue[text]);
            transcribedIndexOverlay = txtCorrected.Text.Length; // update after changes

            parentWindow.LogCorrection(text);

            //parentWindow.correctionQueue.Remove(text);

            //if (txtCorrected.Text == "")
            //txtCorrected.Text = text;
            //else
            //txtCorrected.Text = txtCorrected.Text.Remove(parentWindow.correctedIndex) + text;

            IsProgrammaticChange = false;
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            //TransferClicked?.Invoke(txtCorrected.Text);
            this.DialogResult = DialogResult.OK;
            parentWindow.TransferTextToFocusedWindow(txtCorrected.Text);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            ClearAll();

        }

        void ClearAll()
        {
            txtCorrected.Text = "";
            lblInterimText.Text = "";
            parentWindow.ClearAll();
        }

        private void txtCorrected_TextChanged(object sender, EventArgs e)
        {
            //if (txtCorrected.Text.EndsWith(" ") && !IsProgrammaticChange) // detect word completion to avoid processing after every keystroke
            //{
            ////txtCorrected.SelectAll();
            //txtCorrected.Font = new System.Drawing.Font(txtCorrected.Font, FontStyle.Regular);
            ////txtCorrected.Select(parentWindow.correctedIndex, txtCorrected.Text.Length);
            //txtCorrected.SelectionFont = new System.Drawing.Font(txtCorrected.Font, FontStyle.Bold);
            //txtCorrected.DeselectAll();
            //parentWindow.ProcessFinalResult(txtCorrected.Text, true);
            //}
        }

        private void OverlayWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            ClearAll();
        }
    }
}
