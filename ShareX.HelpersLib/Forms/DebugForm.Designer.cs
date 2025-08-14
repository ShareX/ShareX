namespace ShareX.HelpersLib
{
    partial class DebugForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugForm));
            btnLoadedAssemblies = new System.Windows.Forms.Button();
            btnCopyAll = new System.Windows.Forms.Button();
            rtbDebug = new System.Windows.Forms.RichTextBox();
            btnOpenLogFile = new System.Windows.Forms.Button();
            llRunningFrom = new System.Windows.Forms.LinkLabel();
            flpRunningFrom = new System.Windows.Forms.FlowLayoutPanel();
            lblRunningFrom = new System.Windows.Forms.Label();
            btnUploadLog = new System.Windows.Forms.Button();
            flpRunningFrom.SuspendLayout();
            SuspendLayout();
            // 
            // btnLoadedAssemblies
            // 
            resources.ApplyResources(btnLoadedAssemblies, "btnLoadedAssemblies");
            btnLoadedAssemblies.Name = "btnLoadedAssemblies";
            btnLoadedAssemblies.UseVisualStyleBackColor = true;
            btnLoadedAssemblies.Click += btnLoadedAssemblies_Click;
            // 
            // btnCopyAll
            // 
            resources.ApplyResources(btnCopyAll, "btnCopyAll");
            btnCopyAll.Name = "btnCopyAll";
            btnCopyAll.UseVisualStyleBackColor = true;
            btnCopyAll.Click += btnCopyAll_Click;
            // 
            // rtbDebug
            // 
            resources.ApplyResources(rtbDebug, "rtbDebug");
            rtbDebug.BackColor = System.Drawing.SystemColors.Window;
            rtbDebug.BorderStyle = System.Windows.Forms.BorderStyle.None;
            rtbDebug.Name = "rtbDebug";
            rtbDebug.ReadOnly = true;
            rtbDebug.LinkClicked += rtbDebug_LinkClicked;
            // 
            // btnOpenLogFile
            // 
            resources.ApplyResources(btnOpenLogFile, "btnOpenLogFile");
            btnOpenLogFile.Name = "btnOpenLogFile";
            btnOpenLogFile.UseVisualStyleBackColor = true;
            btnOpenLogFile.Click += btnOpenLogFile_Click;
            // 
            // llRunningFrom
            // 
            resources.ApplyResources(llRunningFrom, "llRunningFrom");
            llRunningFrom.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            llRunningFrom.Name = "llRunningFrom";
            // 
            // flpRunningFrom
            // 
            resources.ApplyResources(flpRunningFrom, "flpRunningFrom");
            flpRunningFrom.Controls.Add(lblRunningFrom);
            flpRunningFrom.Controls.Add(llRunningFrom);
            flpRunningFrom.Name = "flpRunningFrom";
            // 
            // lblRunningFrom
            // 
            resources.ApplyResources(lblRunningFrom, "lblRunningFrom");
            lblRunningFrom.Name = "lblRunningFrom";
            // 
            // btnUploadLog
            // 
            resources.ApplyResources(btnUploadLog, "btnUploadLog");
            btnUploadLog.Name = "btnUploadLog";
            btnUploadLog.UseVisualStyleBackColor = true;
            btnUploadLog.Click += btnUploadLog_Click;
            // 
            // DebugForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(btnUploadLog);
            Controls.Add(flpRunningFrom);
            Controls.Add(btnOpenLogFile);
            Controls.Add(rtbDebug);
            Controls.Add(btnCopyAll);
            Controls.Add(btnLoadedAssemblies);
            Name = "DebugForm";
            flpRunningFrom.ResumeLayout(false);
            flpRunningFrom.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadedAssemblies;
        private System.Windows.Forms.Button btnCopyAll;
        private System.Windows.Forms.RichTextBox rtbDebug;
        private System.Windows.Forms.Button btnOpenLogFile;
        private System.Windows.Forms.LinkLabel llRunningFrom;
        private System.Windows.Forms.FlowLayoutPanel flpRunningFrom;
        private System.Windows.Forms.Label lblRunningFrom;
        private System.Windows.Forms.Button btnUploadLog;
    }
}