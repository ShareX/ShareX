namespace ShareX
{
    partial class FileExistForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileExistForm));
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnOverwrite = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUniqueName = new System.Windows.Forms.Button();
            this.btnNewName = new System.Windows.Forms.Button();
            this.txtNewName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.Name = "lblTitle";
            // 
            // btnOverwrite
            // 
            resources.ApplyResources(this.btnOverwrite, "btnOverwrite");
            this.btnOverwrite.Name = "btnOverwrite";
            this.btnOverwrite.UseVisualStyleBackColor = true;
            this.btnOverwrite.Click += new System.EventHandler(this.btnOverwrite_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUniqueName
            // 
            resources.ApplyResources(this.btnUniqueName, "btnUniqueName");
            this.btnUniqueName.Name = "btnUniqueName";
            this.btnUniqueName.UseVisualStyleBackColor = true;
            this.btnUniqueName.Click += new System.EventHandler(this.btnUniqueName_Click);
            // 
            // btnNewName
            // 
            resources.ApplyResources(this.btnNewName, "btnNewName");
            this.btnNewName.Name = "btnNewName";
            this.btnNewName.UseVisualStyleBackColor = true;
            this.btnNewName.Click += new System.EventHandler(this.btnNewName_Click);
            // 
            // txtNewName
            // 
            resources.ApplyResources(this.txtNewName, "txtNewName");
            this.txtNewName.Name = "txtNewName";
            this.txtNewName.TextChanged += new System.EventHandler(this.txtNewName_TextChanged);
            // 
            // FileExistForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.txtNewName);
            this.Controls.Add(this.btnNewName);
            this.Controls.Add(this.btnUniqueName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOverwrite);
            this.Controls.Add(this.lblTitle);
            this.Name = "FileExistForm";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FileExistForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnOverwrite;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUniqueName;
        private System.Windows.Forms.Button btnNewName;
        private System.Windows.Forms.TextBox txtNewName;
    }
}