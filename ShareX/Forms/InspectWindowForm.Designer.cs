
namespace ShareX
{
    partial class InspectWindowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InspectWindowForm));
            this.rtbInfo = new System.Windows.Forms.RichTextBox();
            this.pInfo = new System.Windows.Forms.Panel();
            this.btnInspectWindow = new System.Windows.Forms.Button();
            this.btnInspectControl = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnPinToTop = new System.Windows.Forms.Button();
            this.pInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbInfo
            // 
            this.rtbInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbInfo.DetectUrls = false;
            resources.ApplyResources(this.rtbInfo, "rtbInfo");
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.ReadOnly = true;
            // 
            // pInfo
            // 
            resources.ApplyResources(this.pInfo, "pInfo");
            this.pInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pInfo.Controls.Add(this.rtbInfo);
            this.pInfo.Name = "pInfo";
            // 
            // btnInspectWindow
            // 
            resources.ApplyResources(this.btnInspectWindow, "btnInspectWindow");
            this.btnInspectWindow.Name = "btnInspectWindow";
            this.btnInspectWindow.UseVisualStyleBackColor = true;
            this.btnInspectWindow.Click += new System.EventHandler(this.btnInspectWindow_Click);
            // 
            // btnInspectControl
            // 
            resources.ApplyResources(this.btnInspectControl, "btnInspectControl");
            this.btnInspectControl.Name = "btnInspectControl";
            this.btnInspectControl.UseVisualStyleBackColor = true;
            this.btnInspectControl.Click += new System.EventHandler(this.btnInspectControl_Click);
            // 
            // btnRefresh
            // 
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnPinToTop
            // 
            resources.ApplyResources(this.btnPinToTop, "btnPinToTop");
            this.btnPinToTop.Name = "btnPinToTop";
            this.btnPinToTop.UseVisualStyleBackColor = true;
            this.btnPinToTop.Click += new System.EventHandler(this.btnPinToTop_Click);
            // 
            // InspectWindowForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnPinToTop);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnInspectControl);
            this.Controls.Add(this.btnInspectWindow);
            this.Controls.Add(this.pInfo);
            this.Name = "InspectWindowForm";
            this.pInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbInfo;
        private System.Windows.Forms.Panel pInfo;
        private System.Windows.Forms.Button btnInspectWindow;
        private System.Windows.Forms.Button btnInspectControl;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnPinToTop;
    }
}