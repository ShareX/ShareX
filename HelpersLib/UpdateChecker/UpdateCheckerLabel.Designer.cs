namespace UpdateCheckerLib
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
            this.pbLoading.Image = ((System.Drawing.Image)(resources.GetObject("pbLoading.Image")));
            this.pbLoading.Location = new System.Drawing.Point(0, 0);
            this.pbLoading.Name = "pbLoading";
            this.pbLoading.Size = new System.Drawing.Size(24, 24);
            this.pbLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbLoading.TabIndex = 0;
            this.pbLoading.TabStop = false;
            // 
            // lblCheckingUpdates
            // 
            this.lblCheckingUpdates.AutoSize = true;
            this.lblCheckingUpdates.ForeColor = System.Drawing.Color.DimGray;
            this.lblCheckingUpdates.Location = new System.Drawing.Point(24, 6);
            this.lblCheckingUpdates.Name = "lblCheckingUpdates";
            this.lblCheckingUpdates.Size = new System.Drawing.Size(117, 13);
            this.lblCheckingUpdates.TabIndex = 1;
            this.lblCheckingUpdates.Text = "Checking for updates...";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Location = new System.Drawing.Point(0, 6);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(77, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "... is up to date";
            this.lblStatus.Visible = false;
            // 
            // llblUpdateAvailable
            // 
            this.llblUpdateAvailable.AutoSize = true;
            this.llblUpdateAvailable.Location = new System.Drawing.Point(0, 6);
            this.llblUpdateAvailable.Name = "llblUpdateAvailable";
            this.llblUpdateAvailable.Size = new System.Drawing.Size(149, 13);
            this.llblUpdateAvailable.TabIndex = 3;
            this.llblUpdateAvailable.TabStop = true;
            this.llblUpdateAvailable.Text = "Newest version of ... available";
            this.llblUpdateAvailable.Visible = false;
            this.llblUpdateAvailable.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblUpdateAvailable_LinkClicked);
            // 
            // UpdateCheckerLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.llblUpdateAvailable);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblCheckingUpdates);
            this.Controls.Add(this.pbLoading);
            this.Name = "UpdateCheckerLabel";
            this.Size = new System.Drawing.Size(250, 24);
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
