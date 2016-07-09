namespace Greenshot
{
    partial class EditorSettingsForm
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
            this.lblIconSize = new System.Windows.Forms.Label();
            this.nudIconSize = new System.Windows.Forms.NumericUpDown();
            this.cbMatchSizeToCapture = new System.Windows.Forms.CheckBox();
            this.cbSuppressSaveDialogAtClose = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbRememberLastDrawingMode = new System.Windows.Forms.CheckBox();
            this.cbMaximizeWhenLargeImage = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudIconSize)).BeginInit();
            this.SuspendLayout();
            // 
            // lblIconSize
            // 
            this.lblIconSize.AutoSize = true;
            this.lblIconSize.Location = new System.Drawing.Point(13, 16);
            this.lblIconSize.Name = "lblIconSize";
            this.lblIconSize.Size = new System.Drawing.Size(52, 13);
            this.lblIconSize.TabIndex = 0;
            this.lblIconSize.Text = "Icon size:";
            // 
            // nudIconSize
            // 
            this.nudIconSize.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudIconSize.Location = new System.Drawing.Point(72, 12);
            this.nudIconSize.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudIconSize.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudIconSize.Name = "nudIconSize";
            this.nudIconSize.ReadOnly = true;
            this.nudIconSize.Size = new System.Drawing.Size(48, 20);
            this.nudIconSize.TabIndex = 1;
            this.nudIconSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudIconSize.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // cbMatchSizeToCapture
            // 
            this.cbMatchSizeToCapture.AutoSize = true;
            this.cbMatchSizeToCapture.Location = new System.Drawing.Point(16, 40);
            this.cbMatchSizeToCapture.Name = "cbMatchSizeToCapture";
            this.cbMatchSizeToCapture.Size = new System.Drawing.Size(221, 17);
            this.cbMatchSizeToCapture.TabIndex = 2;
            this.cbMatchSizeToCapture.Text = "Automatically resize window to image size";
            this.cbMatchSizeToCapture.UseVisualStyleBackColor = true;
            // 
            // cbSuppressSaveDialogAtClose
            // 
            this.cbSuppressSaveDialogAtClose.AutoSize = true;
            this.cbSuppressSaveDialogAtClose.Location = new System.Drawing.Point(16, 88);
            this.cbSuppressSaveDialogAtClose.Name = "cbSuppressSaveDialogAtClose";
            this.cbSuppressSaveDialogAtClose.Size = new System.Drawing.Size(257, 17);
            this.cbSuppressSaveDialogAtClose.TabIndex = 3;
            this.cbSuppressSaveDialogAtClose.Text = "Suppress the save dialog when closing the editor";
            this.cbSuppressSaveDialogAtClose.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(136, 152);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 24);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(232, 152);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cbRememberLastDrawingMode
            // 
            this.cbRememberLastDrawingMode.AutoSize = true;
            this.cbRememberLastDrawingMode.Location = new System.Drawing.Point(16, 111);
            this.cbRememberLastDrawingMode.Name = "cbRememberLastDrawingMode";
            this.cbRememberLastDrawingMode.Size = new System.Drawing.Size(226, 17);
            this.cbRememberLastDrawingMode.TabIndex = 4;
            this.cbRememberLastDrawingMode.Text = "Remember the last drawing mode selected";
            this.cbRememberLastDrawingMode.UseVisualStyleBackColor = true;
            // 
            // cbMaximizeWhenLargeImage
            // 
            this.cbMaximizeWhenLargeImage.AutoSize = true;
            this.cbMaximizeWhenLargeImage.Location = new System.Drawing.Point(36, 63);
            this.cbMaximizeWhenLargeImage.Name = "cbMaximizeWhenLargeImage";
            this.cbMaximizeWhenLargeImage.Size = new System.Drawing.Size(280, 17);
            this.cbMaximizeWhenLargeImage.TabIndex = 6;
            this.cbMaximizeWhenLargeImage.Text = "Maximize window when image larger than display area";
            this.cbMaximizeWhenLargeImage.UseVisualStyleBackColor = true;
            // 
            // EditorSettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(330, 186);
            this.Controls.Add(this.cbMaximizeWhenLargeImage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbRememberLastDrawingMode);
            this.Controls.Add(this.cbSuppressSaveDialogAtClose);
            this.Controls.Add(this.cbMatchSizeToCapture);
            this.Controls.Add(this.nudIconSize);
            this.Controls.Add(this.lblIconSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "EditorSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.nudIconSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIconSize;
        private System.Windows.Forms.NumericUpDown nudIconSize;
        private System.Windows.Forms.CheckBox cbMatchSizeToCapture;
        private System.Windows.Forms.CheckBox cbSuppressSaveDialogAtClose;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbRememberLastDrawingMode;
        private System.Windows.Forms.CheckBox cbMaximizeWhenLargeImage;
    }
}