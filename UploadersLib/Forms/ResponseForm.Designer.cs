namespace UploadersLib
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
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Location = new System.Drawing.Point(3, 3);
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSource.Size = new System.Drawing.Size(746, 481);
            this.txtSource.TabIndex = 0;
            // 
            // tcResponse
            // 
            this.tcResponse.Controls.Add(this.tpString);
            this.tcResponse.Controls.Add(this.tpWebBrowser);
            this.tcResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcResponse.Location = new System.Drawing.Point(5, 5);
            this.tcResponse.Name = "tcResponse";
            this.tcResponse.SelectedIndex = 0;
            this.tcResponse.Size = new System.Drawing.Size(760, 513);
            this.tcResponse.TabIndex = 0;
            this.tcResponse.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tcResponse_Selecting);
            // 
            // tpString
            // 
            this.tpString.Controls.Add(this.txtSource);
            this.tpString.Location = new System.Drawing.Point(4, 22);
            this.tpString.Name = "tpString";
            this.tpString.Padding = new System.Windows.Forms.Padding(3);
            this.tpString.Size = new System.Drawing.Size(752, 487);
            this.tpString.TabIndex = 0;
            this.tpString.Text = "Text";
            this.tpString.UseVisualStyleBackColor = true;
            // 
            // tpWebBrowser
            // 
            this.tpWebBrowser.Controls.Add(this.wbResponse);
            this.tpWebBrowser.Location = new System.Drawing.Point(4, 22);
            this.tpWebBrowser.Name = "tpWebBrowser";
            this.tpWebBrowser.Padding = new System.Windows.Forms.Padding(3);
            this.tpWebBrowser.Size = new System.Drawing.Size(752, 487);
            this.tpWebBrowser.TabIndex = 1;
            this.tpWebBrowser.Text = "Web browser";
            this.tpWebBrowser.UseVisualStyleBackColor = true;
            // 
            // wbResponse
            // 
            this.wbResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbResponse.Location = new System.Drawing.Point(3, 3);
            this.wbResponse.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbResponse.Name = "wbResponse";
            this.wbResponse.ScriptErrorsSuppressed = true;
            this.wbResponse.Size = new System.Drawing.Size(746, 481);
            this.wbResponse.TabIndex = 0;
            // 
            // ResponseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 523);
            this.Controls.Add(this.tcResponse);
            this.Name = "ResponseForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Response";
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