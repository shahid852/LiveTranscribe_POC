using System.Windows.Forms;
using System;

namespace LiveTranscribe_POC
{
    public partial class ErrorWindow : Form
    {
        public ErrorWindow()
        {
            InitializeComponent();
        }

        public void LogError(string error)
        {
            txtErrors.AppendText(error + Environment.NewLine);
        }
    }
}
