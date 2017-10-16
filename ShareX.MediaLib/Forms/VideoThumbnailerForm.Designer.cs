namespace ShareX.MediaLib
{
    partial class VideoThumbnailerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoThumbnailerForm));
            this.txtMediaPath = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.pgOptions = new System.Windows.Forms.PropertyGrid();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // txtMediaPath
            // 
            resources.ApplyResources(this.txtMediaPath, "txtMediaPath");
            this.txtMediaPath.Name = "txtMediaPath";
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.Name = "btnStart";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pgOptions
            // 
            this.pgOptions.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            resources.ApplyResources(this.pgOptions, "pgOptions");
            this.pgOptions.Name = "pgOptions";
            this.pgOptions.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgOptions.ToolbarVisible = false;
            // 
            // btnBrowse
            // 
            resources.ApplyResources(this.btnBrowse, "btnBrowse");
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // pbProgress
            // 
            resources.ApplyResources(this.pbProgress, "pbProgress");
            this.pbProgress.Name = "pbProgress";
            // 
            // VideoThumbnailerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.pgOptions);
            this.Controls.Add(this.txtMediaPath);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.pbProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "VideoThumbnailerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMediaPath;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PropertyGrid pgOptions;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ProgressBar pbProgress;
    }
}