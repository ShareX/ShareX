namespace HelpersLib
{
    partial class UpdateCheckerLabel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateCheckerLabel));
            this.pbLoading = new System.Windows.Forms.PictureBox();
            this.lblCheckingUpdates = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.llblUpdateAvailable = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLoading
            // 
            resources.ApplyResources(this.pbLoading, "pbLoading");
            this.pbLoading.Name = "pbLoading";
            this.pbLoading.TabStop = false;
            // 
            // lblCheckingUpdates
            // 
            resources.ApplyResources(this.lblCheckingUpdates, "lblCheckingUpdates");
            this.lblCheckingUpdates.ForeColor = System.Drawing.Color.DimGray;
            this.lblCheckingUpdates.Name = "lblCheckingUpdates";
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Name = "lblStatus";
            // 
            // llblUpdateAvailable
            // 
            resources.ApplyResources(this.llblUpdateAvailable, "llblUpdateAvailable");
            this.llblUpdateAvailable.Name = "llblUpdateAvailable";
            this.llblUpdateAvailable.TabStop = true;
            this.llblUpdateAvailable.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblUpdateAvailable_LinkClicked);
            // 
            // UpdateCheckerLabel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.llblUpdateAvailable);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblCheckingUpdates);
            this.Controls.Add(this.pbLoading);
            this.Name = "UpdateCheckerLabel";
            ((System.ComponentModel.ISupportInitialize)(this.pbLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbLoading;
        private System.Windows.Forms.Label lblCheckingUpdates;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.LinkLabel llblUpdateAvailable;
    }
}
