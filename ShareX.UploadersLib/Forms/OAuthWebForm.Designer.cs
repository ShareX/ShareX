namespace ShareX.UploadersLib
{
    partial class OAuthWebForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OAuthWebForm));
            this.tscMain = new System.Windows.Forms.ToolStripContainer();
            this.wbMain = new System.Windows.Forms.WebBrowser();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tslURL = new System.Windows.Forms.ToolStripLabel();
            this.tstbURL = new System.Windows.Forms.ToolStripTextBox();
            this.tscMain.ContentPanel.SuspendLayout();
            this.tscMain.TopToolStripPanel.SuspendLayout();
            this.tscMain.SuspendLayout();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tscMain
            // 
            // 
            // tscMain.ContentPanel
            // 
            this.tscMain.ContentPanel.Controls.Add(this.wbMain);
            resources.ApplyResources(this.tscMain.ContentPanel, "tscMain.ContentPanel");
            resources.ApplyResources(this.tscMain, "tscMain");
            this.tscMain.Name = "tscMain";
            // 
            // tscMain.TopToolStripPanel
            // 
            this.tscMain.TopToolStripPanel.Controls.Add(this.tsMain);
            // 
            // wbMain
            // 
            resources.ApplyResources(this.wbMain, "wbMain");
            this.wbMain.Name = "wbMain";
            this.wbMain.ScriptErrorsSuppressed = true;
            this.wbMain.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.wbMain_Navigated);
            // 
            // tsMain
            // 
            resources.ApplyResources(this.tsMain, "tsMain");
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslURL,
            this.tstbURL});
            this.tsMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsMain.Name = "tsMain";
            this.tsMain.Stretch = true;
            // 
            // tslURL
            // 
            this.tslURL.Name = "tslURL";
            resources.ApplyResources(this.tslURL, "tslURL");
            // 
            // tstbURL
            // 
            this.tstbURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tstbURL.Name = "tstbURL";
            resources.ApplyResources(this.tstbURL, "tstbURL");
            // 
            // OAuthWebForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tscMain);
            this.Name = "OAuthWebForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.tscMain.ContentPanel.ResumeLayout(false);
            this.tscMain.TopToolStripPanel.ResumeLayout(false);
            this.tscMain.TopToolStripPanel.PerformLayout();
            this.tscMain.ResumeLayout(false);
            this.tscMain.PerformLayout();
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbMain;
        private System.Windows.Forms.ToolStripContainer tscMain;
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripLabel tslURL;
        private System.Windows.Forms.ToolStripTextBox tstbURL;
    }
}