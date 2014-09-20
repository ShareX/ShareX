namespace ShareX
{
    partial class FilePathTooLongForm
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnNewName = new System.Windows.Forms.Button();
			this.txtNewName = new System.Windows.Forms.TextBox();
			this.btnShortenedName = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.lblTitle.Location = new System.Drawing.Point(8, 8);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(187, 32);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "The file name is too long.\r\nSelect new file name or action:";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(8, 212);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(352, 32);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Do not save";
			this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnNewName
			// 
			this.btnNewName.AutoSize = true;
			this.btnNewName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnNewName.Enabled = false;
			this.btnNewName.Location = new System.Drawing.Point(8, 80);
			this.btnNewName.MaximumSize = new System.Drawing.Size(500, 0);
			this.btnNewName.MinimumSize = new System.Drawing.Size(352, 60);
			this.btnNewName.Name = "btnNewName";
			this.btnNewName.Size = new System.Drawing.Size(352, 60);
			this.btnNewName.TabIndex = 2;
			this.btnNewName.Text = "Use new name:";
			this.btnNewName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnNewName.UseVisualStyleBackColor = true;
			this.btnNewName.Click += new System.EventHandler(this.btnNewName_Click);
			// 
			// txtNewName
			// 
			this.txtNewName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtNewName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.txtNewName.Location = new System.Drawing.Point(8, 48);
			this.txtNewName.Name = "txtNewName";
			this.txtNewName.Size = new System.Drawing.Size(352, 22);
			this.txtNewName.TabIndex = 1;
			this.txtNewName.TextChanged += new System.EventHandler(this.txtNewName_TextChanged);
			// 
			// btnShortenedName
			// 
			this.btnShortenedName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnShortenedName.AutoSize = true;
			this.btnShortenedName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnShortenedName.Location = new System.Drawing.Point(8, 146);
			this.btnShortenedName.MaximumSize = new System.Drawing.Size(500, 0);
			this.btnShortenedName.MinimumSize = new System.Drawing.Size(352, 60);
			this.btnShortenedName.Name = "btnShortenedName";
			this.btnShortenedName.Size = new System.Drawing.Size(352, 60);
			this.btnShortenedName.TabIndex = 3;
			this.btnShortenedName.Text = "Use shortened name: ";
			this.btnShortenedName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnShortenedName.UseVisualStyleBackColor = true;
			this.btnShortenedName.Click += new System.EventHandler(this.btnShortenedName_Click);
			// 
			// FilePathTooLongForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(369, 251);
			this.Controls.Add(this.btnShortenedName);
			this.Controls.Add(this.txtNewName);
			this.Controls.Add(this.btnNewName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblTitle);
			this.Name = "FilePathTooLongForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "`";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

	   private System.Windows.Forms.Label lblTitle;
	   private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnNewName;
        private System.Windows.Forms.TextBox txtNewName;
	   private System.Windows.Forms.Button btnShortenedName;
    }
}