using System;
using System.Windows.Forms;

namespace LiveTranscribe_POC
{
    public partial class OverlayWindow : Form
    {
        public event Action<string> TransferClicked;

        public OverlayWindow()
        {
            InitializeComponent();
        }

        public void UpdateInterimText(string text)
        {
            lblInterimText.Text = text;
        }

        public void UpdateCorrectedText(string text)
        {
            lblCorrectedText.Text = text;
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            TransferClicked?.Invoke(lblCorrectedText.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
