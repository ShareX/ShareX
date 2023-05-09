namespace ShareX.UploadersLib
{
    partial class OAuthLoopbackControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OAuthLoopbackControl));
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.flpStatus = new System.Windows.Forms.FlowLayoutPanel();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.flpStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            resources.ApplyResources(this.btnConnect, "btnConnect");
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Name = "lblStatus";
            // 
            // flpStatus
            // 
            resources.ApplyResources(this.flpStatus, "flpStatus");
            this.flpStatus.Controls.Add(this.lblStatus);
            this.flpStatus.Controls.Add(this.lblStatusValue);
            this.flpStatus.Name = "flpStatus";
            // 
            // lblStatusValue
            // 
            resources.ApplyResources(this.lblStatusValue, "lblStatusValue");
            this.lblStatusValue.Name = "lblStatusValue";
            // 
            // OAuthLoopbackControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flpStatus);
            this.Controls.Add(this.btnConnect);
            this.Name = "OAuthLoopbackControl";
            this.flpStatus.ResumeLayout(false);
            this.flpStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.FlowLayoutPanel flpStatus;
        private System.Windows.Forms.Label lblStatusValue;
    }
}
