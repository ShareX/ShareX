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
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTitle.Location = new System.Drawing.Point(8, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(346, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "There is already a file with the same name in this location.\r\nSelect new file nam" +
    "e or action:";
            // 
            // btnOverwrite
            // 
            this.btnOverwrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOverwrite.Location = new System.Drawing.Point(8, 120);
            this.btnOverwrite.Name = "btnOverwrite";
            this.btnOverwrite.Size = new System.Drawing.Size(352, 32);
            this.btnOverwrite.TabIndex = 0;
            this.btnOverwrite.Text = "Overwrite: ";
            this.btnOverwrite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOverwrite.UseVisualStyleBackColor = true;
            this.btnOverwrite.Click += new System.EventHandler(this.btnOverwrite_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(8, 200);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(352, 32);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Don\'t save";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUniqueName
            // 
            this.btnUniqueName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUniqueName.Location = new System.Drawing.Point(8, 160);
            this.btnUniqueName.Name = "btnUniqueName";
            this.btnUniqueName.Size = new System.Drawing.Size(352, 32);
            this.btnUniqueName.TabIndex = 1;
            this.btnUniqueName.Text = "Use unique name: ";
            this.btnUniqueName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUniqueName.UseVisualStyleBackColor = true;
            this.btnUniqueName.Click += new System.EventHandler(this.btnUniqueName_Click);
            // 
            // btnNewName
            // 
            this.btnNewName.Enabled = false;
            this.btnNewName.Location = new System.Drawing.Point(8, 80);
            this.btnNewName.Name = "btnNewName";
            this.btnNewName.Size = new System.Drawing.Size(352, 32);
            this.btnNewName.TabIndex = 4;
            this.btnNewName.Text = "Use new name:";
            this.btnNewName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNewName.UseVisualStyleBackColor = true;
            this.btnNewName.Click += new System.EventHandler(this.btnNewName_Click);
            // 
            // txtNewName
            // 
            this.txtNewName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtNewName.Location = new System.Drawing.Point(8, 48);
            this.txtNewName.Name = "txtNewName";
            this.txtNewName.Size = new System.Drawing.Size(352, 22);
            this.txtNewName.TabIndex = 3;
            this.txtNewName.TextChanged += new System.EventHandler(this.txtNewName_TextChanged);
            // 
            // FileExistForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 241);
            this.Controls.Add(this.txtNewName);
            this.Controls.Add(this.btnNewName);
            this.Controls.Add(this.btnUniqueName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOverwrite);
            this.Controls.Add(this.lblTitle);
            this.Name = "FileExistForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - How to save?";
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