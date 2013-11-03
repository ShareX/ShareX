namespace UploadersLib
{
    partial class ApiKeysUI
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
            this.pgAppConfig = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // pgAppConfig
            // 
            this.pgAppConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgAppConfig.Location = new System.Drawing.Point(0, 0);
            this.pgAppConfig.Name = "pgAppConfig";
            this.pgAppConfig.Size = new System.Drawing.Size(568, 366);
            this.pgAppConfig.TabIndex = 1;
            // 
            // ApiKeysUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 366);
            this.Controls.Add(this.pgAppConfig);
            this.MinimumSize = new System.Drawing.Size(576, 400);
            this.Name = "ApiKeysUI";
            this.Text = "ApiKeysUI";
            this.Load += new System.EventHandler(this.ApiKeysUI_Load);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.PropertyGrid pgAppConfig;
    }
}