using System;

namespace LiveTranscribe_POC
{
    partial class GSTT
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
            this.components = new System.ComponentModel.Container();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtTranscribed = new System.Windows.Forms.TextBox();
            this.txtCorrected = new System.Windows.Forms.TextBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(30, 87);
            this.btnStartStop.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(228, 56);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start Recording";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(30, 171);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(92, 25);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Stopped";
            // 
            // txtTranscribed
            // 
            this.txtTranscribed.Location = new System.Drawing.Point(30, 235);
            this.txtTranscribed.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtTranscribed.Multiline = true;
            this.txtTranscribed.Name = "txtTranscribed";
            this.txtTranscribed.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTranscribed.Size = new System.Drawing.Size(914, 623);
            this.txtTranscribed.TabIndex = 2;
            this.txtTranscribed.TextChanged += new System.EventHandler(this.txtTranscribed_TextChanged);
            // 
            // txtCorrected
            // 
            this.txtCorrected.Location = new System.Drawing.Point(966, 235);
            this.txtCorrected.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtCorrected.Multiline = true;
            this.txtCorrected.Name = "txtCorrected";
            this.txtCorrected.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCorrected.Size = new System.Drawing.Size(964, 623);
            this.txtCorrected.TabIndex = 3;
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "LiveTranscribe POC is running in the background.";
            this.notifyIcon.Text = "LiveTranscribe POC";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // GSTT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1950, 877);
            this.Controls.Add(this.txtCorrected);
            this.Controls.Add(this.txtTranscribed);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnStartStop);
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "GSTT";
            this.Text = "LiveTranscribe POC";
            this.Resize += new System.EventHandler(this.GSTT_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void txtTranscribed_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtTranscribed;
        private System.Windows.Forms.TextBox txtCorrected;
        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}
