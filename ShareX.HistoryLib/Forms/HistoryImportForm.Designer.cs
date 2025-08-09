namespace ShareX.HistoryLib.Forms
{
    partial class HistoryImportForm
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
            cbSkipDuplicates = new System.Windows.Forms.CheckBox();
            txtFolderPath = new System.Windows.Forms.TextBox();
            btnBrowse = new System.Windows.Forms.Button();
            lblFolderPath = new System.Windows.Forms.Label();
            btnImport = new System.Windows.Forms.Button();
            cbOnlyImportImageFiles = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // cbSkipDuplicates
            // 
            cbSkipDuplicates.AutoSize = true;
            cbSkipDuplicates.Checked = true;
            cbSkipDuplicates.CheckState = System.Windows.Forms.CheckState.Checked;
            cbSkipDuplicates.Location = new System.Drawing.Point(16, 112);
            cbSkipDuplicates.Name = "cbSkipDuplicates";
            cbSkipDuplicates.Size = new System.Drawing.Size(135, 21);
            cbSkipDuplicates.TabIndex = 4;
            cbSkipDuplicates.Text = "Skip duplicate files";
            cbSkipDuplicates.UseVisualStyleBackColor = true;
            // 
            // txtFolderPath
            // 
            txtFolderPath.Location = new System.Drawing.Point(16, 40);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.Size = new System.Drawing.Size(344, 25);
            txtFolderPath.TabIndex = 1;
            txtFolderPath.TextChanged += txtFolderPath_TextChanged;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new System.Drawing.Point(368, 36);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(112, 32);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // lblFolderPath
            // 
            lblFolderPath.AutoSize = true;
            lblFolderPath.Location = new System.Drawing.Point(13, 16);
            lblFolderPath.Name = "lblFolderPath";
            lblFolderPath.Size = new System.Drawing.Size(78, 17);
            lblFolderPath.TabIndex = 0;
            lblFolderPath.Text = "Folder path:";
            // 
            // btnImport
            // 
            btnImport.Enabled = false;
            btnImport.Location = new System.Drawing.Point(16, 144);
            btnImport.Name = "btnImport";
            btnImport.Size = new System.Drawing.Size(112, 32);
            btnImport.TabIndex = 5;
            btnImport.Text = "Import";
            btnImport.UseVisualStyleBackColor = true;
            btnImport.Click += btnImport_Click;
            // 
            // cbOnlyImportImageFiles
            // 
            cbOnlyImportImageFiles.AutoSize = true;
            cbOnlyImportImageFiles.Location = new System.Drawing.Point(16, 80);
            cbOnlyImportImageFiles.Name = "cbOnlyImportImageFiles";
            cbOnlyImportImageFiles.Size = new System.Drawing.Size(163, 21);
            cbOnlyImportImageFiles.TabIndex = 3;
            cbOnlyImportImageFiles.Text = "Only import image files";
            cbOnlyImportImageFiles.UseVisualStyleBackColor = true;
            // 
            // HistoryImportForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(497, 192);
            Controls.Add(cbOnlyImportImageFiles);
            Controls.Add(btnImport);
            Controls.Add(lblFolderPath);
            Controls.Add(btnBrowse);
            Controls.Add(txtFolderPath);
            Controls.Add(cbSkipDuplicates);
            Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Name = "HistoryImportForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ShareX - Import folder";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox cbSkipDuplicates;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblFolderPath;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.CheckBox cbOnlyImportImageFiles;
    }
}