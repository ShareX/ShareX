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
            this.SuspendLayout();
            // 
            // txtMediaPath
            // 
            this.txtMediaPath.Location = new System.Drawing.Point(8, 8);
            this.txtMediaPath.Name = "txtMediaPath";
            this.txtMediaPath.Size = new System.Drawing.Size(624, 20);
            this.txtMediaPath.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(640, 8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pgOptions
            // 
            this.pgOptions.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.pgOptions.Location = new System.Drawing.Point(8, 40);
            this.pgOptions.Name = "pgOptions";
            this.pgOptions.Size = new System.Drawing.Size(704, 416);
            this.pgOptions.TabIndex = 2;
            this.pgOptions.ToolbarVisible = false;
            // 
            // VideoThumbnailerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 467);
            this.Controls.Add(this.pgOptions);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtMediaPath);
            this.Name = "VideoThumbnailerForm";
            this.Text = "ShareX - Video thumbnailer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMediaPath;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PropertyGrid pgOptions;
    }
}