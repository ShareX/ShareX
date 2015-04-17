namespace ShareX.UploadersLib
{
    partial class ResponseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResponseForm));
            this.txtSource = new System.Windows.Forms.TextBox();
            this.tcResponse = new System.Windows.Forms.TabControl();
            this.tpString = new System.Windows.Forms.TabPage();
            this.tpWebBrowser = new System.Windows.Forms.TabPage();
            this.wbResponse = new System.Windows.Forms.WebBrowser();
            this.tcResponse.SuspendLayout();
            this.tpString.SuspendLayout();
            this.tpWebBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSource
            // 
            resources.ApplyResources(this.txtSource, "txtSource");
            this.txtSource.Name = "txtSource";
            // 
            // tcResponse
            // 
            this.tcResponse.Controls.Add(this.tpString);
            this.tcResponse.Controls.Add(this.tpWebBrowser);
            resources.ApplyResources(this.tcResponse, "tcResponse");
            this.tcResponse.Name = "tcResponse";
            this.tcResponse.SelectedIndex = 0;
            this.tcResponse.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tcResponse_Selecting);
            // 
            // tpString
            // 
            this.tpString.Controls.Add(this.txtSource);
            resources.ApplyResources(this.tpString, "tpString");
            this.tpString.Name = "tpString";
            this.tpString.UseVisualStyleBackColor = true;
            // 
            // tpWebBrowser
            // 
            this.tpWebBrowser.Controls.Add(this.wbResponse);
            resources.ApplyResources(this.tpWebBrowser, "tpWebBrowser");
            this.tpWebBrowser.Name = "tpWebBrowser";
            this.tpWebBrowser.UseVisualStyleBackColor = true;
            // 
            // wbResponse
            // 
            resources.ApplyResources(this.wbResponse, "wbResponse");
            this.wbResponse.Name = "wbResponse";
            this.wbResponse.ScriptErrorsSuppressed = true;
            // 
            // ResponseForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcResponse);
            this.Name = "ResponseForm";
            this.Resize += new System.EventHandler(this.ResponseForm_Resize);
            this.tcResponse.ResumeLayout(false);
            this.tpString.ResumeLayout(false);
            this.tpString.PerformLayout();
            this.tpWebBrowser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.TabControl tcResponse;
        private System.Windows.Forms.TabPage tpString;
        private System.Windows.Forms.TabPage tpWebBrowser;
        private System.Windows.Forms.WebBrowser wbResponse;
    }
}