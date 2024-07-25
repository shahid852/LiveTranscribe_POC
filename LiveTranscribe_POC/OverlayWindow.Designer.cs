namespace LiveTranscribe_POC
{
    partial class OverlayWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInterimText = new System.Windows.Forms.Label();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtCorrected = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lblInterimText
            // 
            this.lblInterimText.AutoSize = true;
            this.lblInterimText.Location = new System.Drawing.Point(24, 17);
            this.lblInterimText.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblInterimText.Name = "lblInterimText";
            this.lblInterimText.Size = new System.Drawing.Size(124, 25);
            this.lblInterimText.TabIndex = 0;
            this.lblInterimText.Text = "Interim Text";
            this.lblInterimText.Visible = false;
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(934, 556);
            this.btnTransfer.Margin = new System.Windows.Forms.Padding(6);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(150, 44);
            this.btnTransfer.TabIndex = 2;
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(772, 556);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 44);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtCorrected
            // 
            this.txtCorrected.Location = new System.Drawing.Point(13, 45);
            this.txtCorrected.Name = "txtCorrected";
            this.txtCorrected.Size = new System.Drawing.Size(1071, 502);
            this.txtCorrected.TabIndex = 4;
            this.txtCorrected.Text = "";
            this.txtCorrected.TextChanged += new System.EventHandler(this.txtCorrected_TextChanged);
            // 
            // OverlayWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 615);
            this.ControlBox = false;
            this.Controls.Add(this.txtCorrected);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.lblInterimText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OverlayWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "OverlayWindow";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OverlayWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInterimText;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RichTextBox txtCorrected;
    }
}
