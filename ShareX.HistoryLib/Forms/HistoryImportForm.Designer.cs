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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryImportForm));
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
            resources.ApplyResources(cbSkipDuplicates, "cbSkipDuplicates");
            cbSkipDuplicates.Checked = true;
            cbSkipDuplicates.CheckState = System.Windows.Forms.CheckState.Checked;
            cbSkipDuplicates.Name = "cbSkipDuplicates";
            cbSkipDuplicates.UseVisualStyleBackColor = true;
            // 
            // txtFolderPath
            // 
            resources.ApplyResources(txtFolderPath, "txtFolderPath");
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.TextChanged += txtFolderPath_TextChanged;
            // 
            // btnBrowse
            // 
            resources.ApplyResources(btnBrowse, "btnBrowse");
            btnBrowse.Name = "btnBrowse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // lblFolderPath
            // 
            resources.ApplyResources(lblFolderPath, "lblFolderPath");
            lblFolderPath.Name = "lblFolderPath";
            // 
            // btnImport
            // 
            resources.ApplyResources(btnImport, "btnImport");
            btnImport.Name = "btnImport";
            btnImport.UseVisualStyleBackColor = true;
            btnImport.Click += btnImport_Click;
            // 
            // cbOnlyImportImageFiles
            // 
            resources.ApplyResources(cbOnlyImportImageFiles, "cbOnlyImportImageFiles");
            cbOnlyImportImageFiles.Name = "cbOnlyImportImageFiles";
            cbOnlyImportImageFiles.UseVisualStyleBackColor = true;
            // 
            // HistoryImportForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(cbOnlyImportImageFiles);
            Controls.Add(btnImport);
            Controls.Add(lblFolderPath);
            Controls.Add(btnBrowse);
            Controls.Add(txtFolderPath);
            Controls.Add(cbSkipDuplicates);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "HistoryImportForm";
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