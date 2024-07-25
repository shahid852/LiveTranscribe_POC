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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GSTT));
            this.btnStartStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtTranscribed = new System.Windows.Forms.TextBox();
            this.txtCorrected = new System.Windows.Forms.TextBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.correctionServiceDropdown = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbOpenAIModels = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(13, 15);
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
            this.lblStatus.Location = new System.Drawing.Point(13, 77);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Stopped";
            // 
            // txtTranscribed
            // 
            this.txtTranscribed.Location = new System.Drawing.Point(13, 235);
            this.txtTranscribed.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtTranscribed.Multiline = true;
            this.txtTranscribed.Name = "txtTranscribed";
            this.txtTranscribed.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTranscribed.Size = new System.Drawing.Size(440, 358);
            this.txtTranscribed.TabIndex = 2;
            this.txtTranscribed.TextChanged += new System.EventHandler(this.txtTranscribed_TextChanged);
            // 
            // txtCorrected
            // 
            this.txtCorrected.Location = new System.Drawing.Point(461, 235);
            this.txtCorrected.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtCorrected.Multiline = true;
            this.txtCorrected.Name = "txtCorrected";
            this.txtCorrected.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCorrected.Size = new System.Drawing.Size(474, 358);
            this.txtCorrected.TabIndex = 3;
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "LiveTranscribe POC is running in the background.";
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "LiveTranscribe POC";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.showOverlayToolStripMenuItem,
            this.hideOverlayToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(147, 70);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.Exit_Click);
            // 
            // showOverlayToolStripMenuItem
            // 
            this.showOverlayToolStripMenuItem.Name = "showOverlayToolStripMenuItem";
            this.showOverlayToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.showOverlayToolStripMenuItem.Text = "Show Overlay";
            this.showOverlayToolStripMenuItem.Click += new System.EventHandler(this.showOverlayToolStripMenuItem_Click);
            // 
            // hideOverlayToolStripMenuItem
            // 
            this.hideOverlayToolStripMenuItem.Name = "hideOverlayToolStripMenuItem";
            this.hideOverlayToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.hideOverlayToolStripMenuItem.Text = "Hide Overlay";
            this.hideOverlayToolStripMenuItem.Click += new System.EventHandler(this.hideOverlayToolStripMenuItem_Click);
            // 
            // correctionServiceDropdown
            // 
            this.correctionServiceDropdown.FormattingEnabled = true;
            this.correctionServiceDropdown.Items.AddRange(new object[] {
            "OpenAI",
            "Gemini",
            "Microsoft"});
            this.correctionServiceDropdown.Location = new System.Drawing.Point(677, 6);
            this.correctionServiceDropdown.Name = "correctionServiceDropdown";
            this.correctionServiceDropdown.Size = new System.Drawing.Size(261, 21);
            this.correctionServiceDropdown.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(574, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Correction Service:";
            // 
            // cmbOpenAIModels
            // 
            this.cmbOpenAIModels.FormattingEnabled = true;
            this.cmbOpenAIModels.Items.AddRange(new object[] {
            "gpt-4o-mini",
            "gpt-4o",
            "gpt-3.5-turbo"});
            this.cmbOpenAIModels.Location = new System.Drawing.Point(677, 33);
            this.cmbOpenAIModels.Name = "cmbOpenAIModels";
            this.cmbOpenAIModels.Size = new System.Drawing.Size(261, 21);
            this.cmbOpenAIModels.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(593, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "OpenAI Model:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 213);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Transcribed Text:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(465, 216);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Corrected Text:";
            // 
            // GSTT
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(944, 611);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbOpenAIModels);
            this.Controls.Add(this.correctionServiceDropdown);
            this.Controls.Add(this.txtCorrected);
            this.Controls.Add(this.txtTranscribed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnStartStop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "GSTT";
            this.ShowInTaskbar = false;
            this.Text = "LiveTranscribe POC";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Resize += new System.EventHandler(this.GSTT_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
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
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOverlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideOverlayToolStripMenuItem;
        private System.Windows.Forms.ComboBox correctionServiceDropdown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbOpenAIModels;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
