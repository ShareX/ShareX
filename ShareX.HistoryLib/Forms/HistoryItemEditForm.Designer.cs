namespace ShareX.HistoryLib.Forms
{
    partial class HistoryItemEditForm
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
            lblFileName = new System.Windows.Forms.Label();
            lblFilePath = new System.Windows.Forms.Label();
            lblType = new System.Windows.Forms.Label();
            lblHost = new System.Windows.Forms.Label();
            lblURL = new System.Windows.Forms.Label();
            lblThumbnailURL = new System.Windows.Forms.Label();
            lblDeletionURL = new System.Windows.Forms.Label();
            lblShortenedURL = new System.Windows.Forms.Label();
            txtFileName = new System.Windows.Forms.TextBox();
            txtFilePath = new System.Windows.Forms.TextBox();
            txtType = new System.Windows.Forms.TextBox();
            txtHost = new System.Windows.Forms.TextBox();
            txtURL = new System.Windows.Forms.TextBox();
            txtThumbnailURL = new System.Windows.Forms.TextBox();
            txtDeletionURL = new System.Windows.Forms.TextBox();
            txtShortenedURL = new System.Windows.Forms.TextBox();
            lblTags = new System.Windows.Forms.Label();
            dgvTags = new System.Windows.Forms.DataGridView();
            cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            cValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            lblDateTime = new System.Windows.Forms.Label();
            txtDateTime = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvTags).BeginInit();
            SuspendLayout();
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Location = new System.Drawing.Point(15, 72);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new System.Drawing.Size(66, 17);
            lblFileName.TabIndex = 4;
            lblFileName.Text = "File name:";
            // 
            // lblFilePath
            // 
            lblFilePath.AutoSize = true;
            lblFilePath.Location = new System.Drawing.Point(15, 128);
            lblFilePath.Name = "lblFilePath";
            lblFilePath.Size = new System.Drawing.Size(60, 17);
            lblFilePath.TabIndex = 6;
            lblFilePath.Text = "File path:";
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new System.Drawing.Point(15, 184);
            lblType.Name = "lblType";
            lblType.Size = new System.Drawing.Size(38, 17);
            lblType.TabIndex = 8;
            lblType.Text = "Type:";
            // 
            // lblHost
            // 
            lblHost.AutoSize = true;
            lblHost.Location = new System.Drawing.Point(15, 240);
            lblHost.Name = "lblHost";
            lblHost.Size = new System.Drawing.Size(38, 17);
            lblHost.TabIndex = 10;
            lblHost.Text = "Host:";
            // 
            // lblURL
            // 
            lblURL.AutoSize = true;
            lblURL.Location = new System.Drawing.Point(383, 16);
            lblURL.Name = "lblURL";
            lblURL.Size = new System.Drawing.Size(34, 17);
            lblURL.TabIndex = 12;
            lblURL.Text = "URL:";
            // 
            // lblThumbnailURL
            // 
            lblThumbnailURL.AutoSize = true;
            lblThumbnailURL.Location = new System.Drawing.Point(383, 72);
            lblThumbnailURL.Name = "lblThumbnailURL";
            lblThumbnailURL.Size = new System.Drawing.Size(98, 17);
            lblThumbnailURL.TabIndex = 14;
            lblThumbnailURL.Text = "Thumbnail URL:";
            // 
            // lblDeletionURL
            // 
            lblDeletionURL.AutoSize = true;
            lblDeletionURL.Location = new System.Drawing.Point(383, 128);
            lblDeletionURL.Name = "lblDeletionURL";
            lblDeletionURL.Size = new System.Drawing.Size(86, 17);
            lblDeletionURL.TabIndex = 16;
            lblDeletionURL.Text = "Deletion URL:";
            // 
            // lblShortenedURL
            // 
            lblShortenedURL.AutoSize = true;
            lblShortenedURL.Location = new System.Drawing.Point(383, 184);
            lblShortenedURL.Name = "lblShortenedURL";
            lblShortenedURL.Size = new System.Drawing.Size(98, 17);
            lblShortenedURL.TabIndex = 18;
            lblShortenedURL.Text = "Shortened URL:";
            // 
            // txtFileName
            // 
            txtFileName.Location = new System.Drawing.Point(16, 96);
            txtFileName.Name = "txtFileName";
            txtFileName.Size = new System.Drawing.Size(352, 25);
            txtFileName.TabIndex = 5;
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new System.Drawing.Point(16, 152);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new System.Drawing.Size(352, 25);
            txtFilePath.TabIndex = 7;
            // 
            // txtType
            // 
            txtType.Location = new System.Drawing.Point(16, 208);
            txtType.Name = "txtType";
            txtType.Size = new System.Drawing.Size(352, 25);
            txtType.TabIndex = 9;
            // 
            // txtHost
            // 
            txtHost.Location = new System.Drawing.Point(16, 264);
            txtHost.Name = "txtHost";
            txtHost.Size = new System.Drawing.Size(352, 25);
            txtHost.TabIndex = 11;
            // 
            // txtURL
            // 
            txtURL.Location = new System.Drawing.Point(384, 40);
            txtURL.Name = "txtURL";
            txtURL.Size = new System.Drawing.Size(352, 25);
            txtURL.TabIndex = 13;
            // 
            // txtThumbnailURL
            // 
            txtThumbnailURL.Location = new System.Drawing.Point(384, 96);
            txtThumbnailURL.Name = "txtThumbnailURL";
            txtThumbnailURL.Size = new System.Drawing.Size(352, 25);
            txtThumbnailURL.TabIndex = 15;
            // 
            // txtDeletionURL
            // 
            txtDeletionURL.Location = new System.Drawing.Point(384, 152);
            txtDeletionURL.Name = "txtDeletionURL";
            txtDeletionURL.Size = new System.Drawing.Size(352, 25);
            txtDeletionURL.TabIndex = 17;
            // 
            // txtShortenedURL
            // 
            txtShortenedURL.Location = new System.Drawing.Point(384, 208);
            txtShortenedURL.Name = "txtShortenedURL";
            txtShortenedURL.Size = new System.Drawing.Size(352, 25);
            txtShortenedURL.TabIndex = 19;
            // 
            // lblTags
            // 
            lblTags.AutoSize = true;
            lblTags.Location = new System.Drawing.Point(15, 296);
            lblTags.Name = "lblTags";
            lblTags.Size = new System.Drawing.Size(38, 17);
            lblTags.TabIndex = 20;
            lblTags.Text = "Tags:";
            // 
            // dgvTags
            // 
            dgvTags.AllowUserToResizeRows = false;
            dgvTags.BackgroundColor = System.Drawing.SystemColors.Window;
            dgvTags.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvTags.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dgvTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTags.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { cName, cValue });
            dgvTags.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgvTags.Location = new System.Drawing.Point(16, 320);
            dgvTags.MultiSelect = false;
            dgvTags.Name = "dgvTags";
            dgvTags.RowHeadersVisible = false;
            dgvTags.Size = new System.Drawing.Size(720, 280);
            dgvTags.TabIndex = 21;
            // 
            // cName
            // 
            cName.HeaderText = "Name";
            cName.Name = "cName";
            cName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            cName.Width = 200;
            // 
            // cValue
            // 
            cValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            cValue.HeaderText = "Value";
            cValue.Name = "cValue";
            cValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnOK
            // 
            btnOK.Location = new System.Drawing.Point(504, 616);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(112, 32);
            btnOK.TabIndex = 0;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(624, 616);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(112, 32);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // lblDateTime
            // 
            lblDateTime.AutoSize = true;
            lblDateTime.Location = new System.Drawing.Point(15, 16);
            lblDateTime.Name = "lblDateTime";
            lblDateTime.Size = new System.Drawing.Size(67, 17);
            lblDateTime.TabIndex = 2;
            lblDateTime.Text = "Date time:";
            // 
            // txtDateTime
            // 
            txtDateTime.Location = new System.Drawing.Point(16, 40);
            txtDateTime.Name = "txtDateTime";
            txtDateTime.ReadOnly = true;
            txtDateTime.Size = new System.Drawing.Size(352, 25);
            txtDateTime.TabIndex = 3;
            // 
            // HistoryItemEdit
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(753, 663);
            Controls.Add(txtDateTime);
            Controls.Add(lblDateTime);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(dgvTags);
            Controls.Add(lblTags);
            Controls.Add(txtShortenedURL);
            Controls.Add(txtDeletionURL);
            Controls.Add(txtThumbnailURL);
            Controls.Add(txtURL);
            Controls.Add(txtHost);
            Controls.Add(txtType);
            Controls.Add(txtFilePath);
            Controls.Add(txtFileName);
            Controls.Add(lblShortenedURL);
            Controls.Add(lblDeletionURL);
            Controls.Add(lblThumbnailURL);
            Controls.Add(lblURL);
            Controls.Add(lblHost);
            Controls.Add(lblType);
            Controls.Add(lblFilePath);
            Controls.Add(lblFileName);
            Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "HistoryItemEdit";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ShareX - Edit item";
            ((System.ComponentModel.ISupportInitialize)dgvTags).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.Label lblThumbnailURL;
        private System.Windows.Forms.Label lblDeletionURL;
        private System.Windows.Forms.Label lblShortenedURL;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.TextBox txtThumbnailURL;
        private System.Windows.Forms.TextBox txtDeletionURL;
        private System.Windows.Forms.TextBox txtShortenedURL;
        private System.Windows.Forms.Label lblTags;
        private System.Windows.Forms.DataGridView dgvTags;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cValue;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.TextBox txtDateTime;
    }
}