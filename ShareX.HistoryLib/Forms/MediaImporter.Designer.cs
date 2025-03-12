namespace ShareX.HistoryLib.Forms
{
    partial class MediaImporter
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
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowseFolderPath = new System.Windows.Forms.Button();
            this.lblFolderPath = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Location = new System.Drawing.Point(12, 51);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.ReadOnly = true;
            this.txtFolderPath.Size = new System.Drawing.Size(408, 20);
            this.txtFolderPath.TabIndex = 2;
            // 
            // btnBrowseFolderPath
            // 
            this.btnBrowseFolderPath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnBrowseFolderPath.Location = new System.Drawing.Point(437, 49);
            this.btnBrowseFolderPath.Name = "btnBrowseFolderPath";
            this.btnBrowseFolderPath.Size = new System.Drawing.Size(96, 23);
            this.btnBrowseFolderPath.TabIndex = 3;
            this.btnBrowseFolderPath.Text = "Browse...";
            this.btnBrowseFolderPath.UseVisualStyleBackColor = true;
            this.btnBrowseFolderPath.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblFolderPath
            // 
            this.lblFolderPath.AutoSize = true;
            this.lblFolderPath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFolderPath.Location = new System.Drawing.Point(12, 26);
            this.lblFolderPath.Name = "lblFolderPath";
            this.lblFolderPath.Size = new System.Drawing.Size(63, 13);
            this.lblFolderPath.TabIndex = 4;
            this.lblFolderPath.Text = "Folder path:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 109);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(405, 23);
            this.progressBar1.TabIndex = 5;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCount.Location = new System.Drawing.Point(434, 109);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(36, 13);
            this.lblCount.TabIndex = 6;
            this.lblCount.Text = "34/34";
            // 
            // btnImport
            // 
            this.btnImport.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnImport.Location = new System.Drawing.Point(309, 186);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(96, 23);
            this.btnImport.TabIndex = 7;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnClose
            // 
            this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClose.Location = new System.Drawing.Point(437, 186);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(96, 23);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // MediaImporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 221);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblFolderPath);
            this.Controls.Add(this.btnBrowseFolderPath);
            this.Controls.Add(this.txtFolderPath);
            this.Name = "MediaImporter";
            this.Text = "ShareX - Media Importer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MediaImporter_FormClosing);
            this.Load += new System.EventHandler(this.ImageImporter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowseFolderPath;
        private System.Windows.Forms.Label lblFolderPath;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnClose;
    }
}