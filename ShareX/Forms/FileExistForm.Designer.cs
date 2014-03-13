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
            this.btnNewName = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTitle.Location = new System.Drawing.Point(8, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(346, 16);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "There is already a file with the same name in this location.";
            // 
            // btnOverwrite
            // 
            this.btnOverwrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOverwrite.Location = new System.Drawing.Point(8, 32);
            this.btnOverwrite.Name = "btnOverwrite";
            this.btnOverwrite.Size = new System.Drawing.Size(352, 32);
            this.btnOverwrite.TabIndex = 1;
            this.btnOverwrite.Text = "Overwrite: ";
            this.btnOverwrite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOverwrite.UseVisualStyleBackColor = true;
            this.btnOverwrite.Click += new System.EventHandler(this.btnOverwrite_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(8, 112);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(352, 32);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Don\'t save";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNewName
            // 
            this.btnNewName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewName.Location = new System.Drawing.Point(8, 72);
            this.btnNewName.Name = "btnNewName";
            this.btnNewName.Size = new System.Drawing.Size(352, 32);
            this.btnNewName.TabIndex = 3;
            this.btnNewName.Text = "Save with new name: ";
            this.btnNewName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNewName.UseVisualStyleBackColor = true;
            this.btnNewName.Click += new System.EventHandler(this.btnNewName_Click);
            // 
            // FileExistForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 154);
            this.Controls.Add(this.btnNewName);
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
        private System.Windows.Forms.Button btnNewName;
    }
}