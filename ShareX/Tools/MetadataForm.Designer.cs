namespace ShareX
{
    partial class MetadataForm
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
            this.btnOpen = new System.Windows.Forms.Button();
            this.rtbMetadata = new System.Windows.Forms.RichTextBox();
            this.btnCopyAll = new System.Windows.Forms.Button();
            this.btnStripMetadata = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpen.Location = new System.Drawing.Point(8, 520);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(120, 32);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Open...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // rtbMetadata
            // 
            this.rtbMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbMetadata.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbMetadata.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbMetadata.Location = new System.Drawing.Point(8, 8);
            this.rtbMetadata.Name = "rtbMetadata";
            this.rtbMetadata.ReadOnly = true;
            this.rtbMetadata.Size = new System.Drawing.Size(568, 504);
            this.rtbMetadata.TabIndex = 0;
            this.rtbMetadata.Text = "";
            this.rtbMetadata.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbMetadata_LinkClicked);
            // 
            // btnCopyAll
            // 
            this.btnCopyAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopyAll.Location = new System.Drawing.Point(136, 520);
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.Size = new System.Drawing.Size(120, 32);
            this.btnCopyAll.TabIndex = 2;
            this.btnCopyAll.Text = "Copy all";
            this.btnCopyAll.UseVisualStyleBackColor = true;
            this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);
            // 
            // btnStripMetadata
            // 
            this.btnStripMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStripMetadata.Location = new System.Drawing.Point(264, 520);
            this.btnStripMetadata.Name = "btnStripMetadata";
            this.btnStripMetadata.Size = new System.Drawing.Size(160, 32);
            this.btnStripMetadata.TabIndex = 3;
            this.btnStripMetadata.Text = "Strip metadata...";
            this.btnStripMetadata.UseVisualStyleBackColor = true;
            this.btnStripMetadata.Click += new System.EventHandler(this.btnStripMetadata_Click);
            // 
            // MetadataForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.btnStripMetadata);
            this.Controls.Add(this.btnCopyAll);
            this.Controls.Add(this.rtbMetadata);
            this.Controls.Add(this.btnOpen);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.Name = "MetadataForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Metadata";
            this.Shown += new System.EventHandler(this.MetadataForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MetadataForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MetadataForm_DragEnter);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.RichTextBox rtbMetadata;
        private System.Windows.Forms.Button btnCopyAll;
        private System.Windows.Forms.Button btnStripMetadata;
    }
}