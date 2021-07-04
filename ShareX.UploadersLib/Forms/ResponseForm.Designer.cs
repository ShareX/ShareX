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
            this.tscResponseText = new System.Windows.Forms.ToolStripContainer();
            this.pResponseText = new System.Windows.Forms.Panel();
            this.rtbResponseText = new System.Windows.Forms.RichTextBox();
            this.tsResponseText = new System.Windows.Forms.ToolStrip();
            this.tsbResponseTextJSONFormat = new System.Windows.Forms.ToolStripButton();
            this.tsbResponseTextXMLFormat = new System.Windows.Forms.ToolStripButton();
            this.tsbResponseTextCopy = new System.Windows.Forms.ToolStripButton();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpResult = new System.Windows.Forms.TabPage();
            this.pResult = new System.Windows.Forms.Panel();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.tpResponseInfo = new System.Windows.Forms.TabPage();
            this.pResponseInfo = new System.Windows.Forms.Panel();
            this.rtbResponseInfo = new System.Windows.Forms.RichTextBox();
            this.tpResponseText = new System.Windows.Forms.TabPage();
            this.tpWebBrowser = new System.Windows.Forms.TabPage();
            this.wbResponse = new System.Windows.Forms.WebBrowser();
            this.tscResponseText.ContentPanel.SuspendLayout();
            this.tscResponseText.TopToolStripPanel.SuspendLayout();
            this.tscResponseText.SuspendLayout();
            this.pResponseText.SuspendLayout();
            this.tsResponseText.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpResult.SuspendLayout();
            this.pResult.SuspendLayout();
            this.tpResponseInfo.SuspendLayout();
            this.pResponseInfo.SuspendLayout();
            this.tpResponseText.SuspendLayout();
            this.tpWebBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // tscResponseText
            // 
            // 
            // tscResponseText.ContentPanel
            // 
            this.tscResponseText.ContentPanel.Controls.Add(this.pResponseText);
            resources.ApplyResources(this.tscResponseText.ContentPanel, "tscResponseText.ContentPanel");
            resources.ApplyResources(this.tscResponseText, "tscResponseText");
            this.tscResponseText.Name = "tscResponseText";
            // 
            // tscResponseText.TopToolStripPanel
            // 
            this.tscResponseText.TopToolStripPanel.Controls.Add(this.tsResponseText);
            this.tscResponseText.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // pResponseText
            // 
            this.pResponseText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResponseText.Controls.Add(this.rtbResponseText);
            resources.ApplyResources(this.pResponseText, "pResponseText");
            this.pResponseText.Name = "pResponseText";
            // 
            // rtbResponseText
            // 
            this.rtbResponseText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtbResponseText, "rtbResponseText");
            this.rtbResponseText.Name = "rtbResponseText";
            this.rtbResponseText.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbResult_LinkClicked);
            // 
            // tsResponseText
            // 
            resources.ApplyResources(this.tsResponseText, "tsResponseText");
            this.tsResponseText.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsResponseText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbResponseTextJSONFormat,
            this.tsbResponseTextXMLFormat,
            this.tsbResponseTextCopy});
            this.tsResponseText.Name = "tsResponseText";
            this.tsResponseText.ShowItemToolTips = false;
            // 
            // tsbResponseTextJSONFormat
            // 
            this.tsbResponseTextJSONFormat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.tsbResponseTextJSONFormat, "tsbResponseTextJSONFormat");
            this.tsbResponseTextJSONFormat.Name = "tsbResponseTextJSONFormat";
            this.tsbResponseTextJSONFormat.Click += new System.EventHandler(this.tsbResponseTextJSONFormat_Click);
            // 
            // tsbResponseTextXMLFormat
            // 
            this.tsbResponseTextXMLFormat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.tsbResponseTextXMLFormat, "tsbResponseTextXMLFormat");
            this.tsbResponseTextXMLFormat.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.tsbResponseTextXMLFormat.Name = "tsbResponseTextXMLFormat";
            this.tsbResponseTextXMLFormat.Click += new System.EventHandler(this.tsbResponseTextXMLFormat_Click);
            // 
            // tsbResponseTextCopy
            // 
            this.tsbResponseTextCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.tsbResponseTextCopy, "tsbResponseTextCopy");
            this.tsbResponseTextCopy.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.tsbResponseTextCopy.Name = "tsbResponseTextCopy";
            this.tsbResponseTextCopy.Click += new System.EventHandler(this.tsbResponseTextCopy_Click);
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpResult);
            this.tcMain.Controls.Add(this.tpResponseInfo);
            this.tcMain.Controls.Add(this.tpResponseText);
            this.tcMain.Controls.Add(this.tpWebBrowser);
            resources.ApplyResources(this.tcMain, "tcMain");
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tcMain_Selecting);
            // 
            // tpResult
            // 
            this.tpResult.Controls.Add(this.pResult);
            resources.ApplyResources(this.tpResult, "tpResult");
            this.tpResult.Name = "tpResult";
            this.tpResult.UseVisualStyleBackColor = true;
            // 
            // pResult
            // 
            this.pResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResult.Controls.Add(this.rtbResult);
            resources.ApplyResources(this.pResult, "pResult");
            this.pResult.Name = "pResult";
            // 
            // rtbResult
            // 
            this.rtbResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtbResult, "rtbResult");
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbResult_LinkClicked);
            // 
            // tpResponseInfo
            // 
            this.tpResponseInfo.Controls.Add(this.pResponseInfo);
            resources.ApplyResources(this.tpResponseInfo, "tpResponseInfo");
            this.tpResponseInfo.Name = "tpResponseInfo";
            this.tpResponseInfo.UseVisualStyleBackColor = true;
            // 
            // pResponseInfo
            // 
            this.pResponseInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResponseInfo.Controls.Add(this.rtbResponseInfo);
            resources.ApplyResources(this.pResponseInfo, "pResponseInfo");
            this.pResponseInfo.Name = "pResponseInfo";
            // 
            // rtbResponseInfo
            // 
            this.rtbResponseInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtbResponseInfo, "rtbResponseInfo");
            this.rtbResponseInfo.Name = "rtbResponseInfo";
            this.rtbResponseInfo.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbResult_LinkClicked);
            // 
            // tpResponseText
            // 
            this.tpResponseText.Controls.Add(this.tscResponseText);
            resources.ApplyResources(this.tpResponseText, "tpResponseText");
            this.tpResponseText.Name = "tpResponseText";
            this.tpResponseText.UseVisualStyleBackColor = true;
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
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tcMain);
            this.Name = "ResponseForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.tscResponseText.ContentPanel.ResumeLayout(false);
            this.tscResponseText.TopToolStripPanel.ResumeLayout(false);
            this.tscResponseText.TopToolStripPanel.PerformLayout();
            this.tscResponseText.ResumeLayout(false);
            this.tscResponseText.PerformLayout();
            this.pResponseText.ResumeLayout(false);
            this.tsResponseText.ResumeLayout(false);
            this.tsResponseText.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.tpResult.ResumeLayout(false);
            this.pResult.ResumeLayout(false);
            this.tpResponseInfo.ResumeLayout(false);
            this.pResponseInfo.ResumeLayout(false);
            this.tpResponseText.ResumeLayout(false);
            this.tpWebBrowser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpResult;
        private System.Windows.Forms.Panel pResult;
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.TabPage tpResponseInfo;
        private System.Windows.Forms.Panel pResponseInfo;
        private System.Windows.Forms.RichTextBox rtbResponseInfo;
        private System.Windows.Forms.TabPage tpResponseText;
        private System.Windows.Forms.ToolStripContainer tscResponseText;
        private System.Windows.Forms.Panel pResponseText;
        private System.Windows.Forms.RichTextBox rtbResponseText;
        private System.Windows.Forms.ToolStrip tsResponseText;
        private System.Windows.Forms.ToolStripButton tsbResponseTextJSONFormat;
        private System.Windows.Forms.ToolStripButton tsbResponseTextXMLFormat;
        private System.Windows.Forms.ToolStripButton tsbResponseTextCopy;
        private System.Windows.Forms.TabPage tpWebBrowser;
        private System.Windows.Forms.WebBrowser wbResponse;
    }
}