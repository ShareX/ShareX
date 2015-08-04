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
            this.txtMediaPath = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.pgOptions = new System.Windows.Forms.PropertyGrid();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // txtMediaPath
            // 
            this.txtMediaPath.Location = new System.Drawing.Point(8, 8);
            this.txtMediaPath.Name = "txtMediaPath";
            this.txtMediaPath.Size = new System.Drawing.Size(496, 20);
            this.txtMediaPath.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(544, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(168, 24);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Take screenshots";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pgOptions
            // 
            this.pgOptions.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.pgOptions.Location = new System.Drawing.Point(8, 40);
            this.pgOptions.Name = "pgOptions";
            this.pgOptions.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgOptions.Size = new System.Drawing.Size(704, 416);
            this.pgOptions.TabIndex = 2;
            this.pgOptions.ToolbarVisible = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(512, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(27, 24);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(544, 6);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(168, 24);
            this.pbProgress.TabIndex = 4;
            this.pbProgress.Visible = false;
            // 
            // VideoThumbnailerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 464);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.pgOptions);
            this.Controls.Add(this.txtMediaPath);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.pbProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "VideoThumbnailerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Video thumbnailer";
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