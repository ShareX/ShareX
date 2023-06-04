namespace ShareX
{
    partial class ClipboardUploadForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClipboardUploadForm));
            this.lblQuestion = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtClipboard = new System.Windows.Forms.TextBox();
            this.lbClipboard = new System.Windows.Forms.ListBox();
            this.cbDontShowThisWindow = new System.Windows.Forms.CheckBox();
            this.pbClipboard = new ShareX.HelpersLib.MyPictureBox();
            this.btnUpload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblQuestion
            // 
            this.lblQuestion.BackColor = System.Drawing.Color.RoyalBlue;
            resources.ApplyResources(this.lblQuestion, "lblQuestion");
            this.lblQuestion.ForeColor = System.Drawing.Color.White;
            this.lblQuestion.Name = "lblQuestion";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtClipboard
            // 
            resources.ApplyResources(this.txtClipboard, "txtClipboard");
            this.txtClipboard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtClipboard.Name = "txtClipboard";
            this.txtClipboard.ReadOnly = true;
            // 
            // lbClipboard
            // 
            resources.ApplyResources(this.lbClipboard, "lbClipboard");
            this.lbClipboard.FormattingEnabled = true;
            this.lbClipboard.Name = "lbClipboard";
            // 
            // cbDontShowThisWindow
            // 
            resources.ApplyResources(this.cbDontShowThisWindow, "cbDontShowThisWindow");
            this.cbDontShowThisWindow.Name = "cbDontShowThisWindow";
            this.cbDontShowThisWindow.UseVisualStyleBackColor = true;
            this.cbDontShowThisWindow.CheckedChanged += new System.EventHandler(this.cbDontShowThisWindow_CheckedChanged);
            // 
            // pbClipboard
            // 
            resources.ApplyResources(this.pbClipboard, "pbClipboard");
            this.pbClipboard.BackColor = System.Drawing.SystemColors.Window;
            this.pbClipboard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbClipboard.DrawCheckeredBackground = true;
            this.pbClipboard.FullscreenOnClick = true;
            this.pbClipboard.Name = "pbClipboard";
            this.pbClipboard.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            this.pbClipboard.ShowImageSizeLabel = true;
            // 
            // btnUpload
            // 
            resources.ApplyResources(this.btnUpload, "btnUpload");
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // ClipboardUploadForm
            // 
            this.AcceptButton = this.btnUpload;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.pbClipboard);
            this.Controls.Add(this.cbDontShowThisWindow);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblQuestion);
            this.Controls.Add(this.txtClipboard);
            this.Controls.Add(this.lbClipboard);
            this.Name = "ClipboardUploadForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ClipboardUploadForm_FormClosed);
            this.Shown += new System.EventHandler(this.ClipboardContentViewer_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.TextBox txtClipboard;
        private System.Windows.Forms.ListBox lbClipboard;
        private System.Windows.Forms.CheckBox cbDontShowThisWindow;
        private HelpersLib.MyPictureBox pbClipboard;
        private System.Windows.Forms.Button btnUpload;
    }
}