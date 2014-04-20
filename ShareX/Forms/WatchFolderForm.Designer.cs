namespace ShareX
{
    partial class WatchFolderForm
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
            this.btnPathBrowse = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.lblFolderPath = new System.Windows.Forms.Label();
            this.lblFilterExample = new System.Windows.Forms.Label();
            this.cbIncludeSubdirectories = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPathBrowse
            // 
            this.btnPathBrowse.Location = new System.Drawing.Point(272, 11);
            this.btnPathBrowse.Name = "btnPathBrowse";
            this.btnPathBrowse.Size = new System.Drawing.Size(40, 23);
            this.btnPathBrowse.TabIndex = 2;
            this.btnPathBrowse.Text = "...";
            this.btnPathBrowse.UseVisualStyleBackColor = true;
            this.btnPathBrowse.Click += new System.EventHandler(this.btnPathBrowse_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(88, 36);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(224, 20);
            this.txtFilter.TabIndex = 4;
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Location = new System.Drawing.Point(88, 12);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(176, 20);
            this.txtFolderPath.TabIndex = 1;
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(16, 40);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(32, 13);
            this.lblFilter.TabIndex = 3;
            this.lblFilter.Text = "Filter:";
            // 
            // lblFolderPath
            // 
            this.lblFolderPath.AutoSize = true;
            this.lblFolderPath.Location = new System.Drawing.Point(16, 16);
            this.lblFolderPath.Name = "lblFolderPath";
            this.lblFolderPath.Size = new System.Drawing.Size(63, 13);
            this.lblFolderPath.TabIndex = 0;
            this.lblFolderPath.Text = "Folder path:";
            // 
            // lblFilterExample
            // 
            this.lblFilterExample.AutoSize = true;
            this.lblFilterExample.Location = new System.Drawing.Point(88, 60);
            this.lblFilterExample.Name = "lblFilterExample";
            this.lblFilterExample.Size = new System.Drawing.Size(78, 13);
            this.lblFilterExample.TabIndex = 5;
            this.lblFilterExample.Text = "Example: *.png";
            // 
            // cbIncludeSubdirectories
            // 
            this.cbIncludeSubdirectories.AutoSize = true;
            this.cbIncludeSubdirectories.Location = new System.Drawing.Point(16, 88);
            this.cbIncludeSubdirectories.Name = "cbIncludeSubdirectories";
            this.cbIncludeSubdirectories.Size = new System.Drawing.Size(129, 17);
            this.cbIncludeSubdirectories.TabIndex = 6;
            this.cbIncludeSubdirectories.Text = "Include subdirectories";
            this.cbIncludeSubdirectories.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(232, 120);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(144, 120);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // WatchFolderForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 153);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbIncludeSubdirectories);
            this.Controls.Add(this.lblFilterExample);
            this.Controls.Add(this.btnPathBrowse);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.txtFolderPath);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.lblFolderPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "WatchFolderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Watch folder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPathBrowse;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.Label lblFolderPath;
        private System.Windows.Forms.Label lblFilterExample;
        private System.Windows.Forms.CheckBox cbIncludeSubdirectories;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}