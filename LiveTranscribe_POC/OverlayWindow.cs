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

        public void UpdateInterimFinalText(string new_text, bool IsInterim = true)
        {

            lblInterimText.Text = new_text;

            if (!IsInterim)
            {
                if (txtCorrected.Text == "" || transcribedIndexOverlay == 0)
                    txtCorrected.Text = new_text;
                else
                    txtCorrected.Text = txtCorrected.Text.Substring(0, transcribedIndexOverlay) + new_text;


                lblInterimText.Text = "";
                transcribedIndexOverlay = txtCorrected.Text.Length; // update after changes
            }
            txtCorrected.Select(txtCorrected.Text.Length, 0);
        }

        public void UpdateCorrectedText(string correctionKeyText)
        {
            IsProgrammaticChange = true;
            //int index = txtCorrected.Text.IndexOf(correctionKeyText);
            //Console.WriteLine(index);
            //if (index < 0) throw new Exception("text not found");
            txtCorrected.Text = txtCorrected.Text.Replace(correctionKeyText, parentWindow.correctionQueue[correctionKeyText]);
            transcribedIndexOverlay = txtCorrected.Text.Length; // update after changes
            txtCorrected.Select(txtCorrected.Text.Length, 0);

            parentWindow.LogCorrection(correctionKeyText);

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
            this.Hide();
            ClearAll();
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
            transcribedIndexOverlay = 0;
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
