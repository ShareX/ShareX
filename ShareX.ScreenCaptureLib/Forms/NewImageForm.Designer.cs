namespace ShareX.ScreenCaptureLib
{
    partial class NewImageForm
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
            this.lblHeightPixels = new System.Windows.Forms.Label();
            this.lblWidthPixels = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.cbTransparent = new System.Windows.Forms.CheckBox();
            this.btnChangeColor = new ShareX.HelpersLib.ColorButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbBackgroundColor = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            this.gbBackgroundColor.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeightPixels
            // 
            this.lblHeightPixels.AutoSize = true;
            this.lblHeightPixels.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHeightPixels.Location = new System.Drawing.Point(168, 48);
            this.lblHeightPixels.Name = "lblHeightPixels";
            this.lblHeightPixels.Size = new System.Drawing.Size(33, 13);
            this.lblHeightPixels.TabIndex = 5;
            this.lblHeightPixels.Text = "pixels";
            // 
            // lblWidthPixels
            // 
            this.lblWidthPixels.AutoSize = true;
            this.lblWidthPixels.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblWidthPixels.Location = new System.Drawing.Point(168, 16);
            this.lblWidthPixels.Name = "lblWidthPixels";
            this.lblWidthPixels.Size = new System.Drawing.Size(33, 13);
            this.lblWidthPixels.TabIndex = 2;
            this.lblWidthPixels.Text = "pixels";
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHeight.Location = new System.Drawing.Point(13, 48);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(41, 13);
            this.lblHeight.TabIndex = 3;
            this.lblHeight.Text = "Height:";
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblWidth.Location = new System.Drawing.Point(13, 16);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(38, 13);
            this.lblWidth.TabIndex = 0;
            this.lblWidth.Text = "Width:";
            // 
            // nudHeight
            // 
            this.nudHeight.Location = new System.Drawing.Point(96, 44);
            this.nudHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(64, 20);
            this.nudHeight.TabIndex = 4;
            this.nudHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudHeight.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // nudWidth
            // 
            this.nudWidth.Location = new System.Drawing.Point(96, 12);
            this.nudWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(64, 20);
            this.nudWidth.TabIndex = 1;
            this.nudWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudWidth.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // cbTransparent
            // 
            this.cbTransparent.AutoSize = true;
            this.cbTransparent.Location = new System.Drawing.Point(16, 24);
            this.cbTransparent.Name = "cbTransparent";
            this.cbTransparent.Size = new System.Drawing.Size(83, 17);
            this.cbTransparent.TabIndex = 0;
            this.cbTransparent.Text = "Transparent";
            this.cbTransparent.UseVisualStyleBackColor = true;
            this.cbTransparent.CheckedChanged += new System.EventHandler(this.cbTransparent_CheckedChanged);
            // 
            // btnChangeColor
            // 
            this.btnChangeColor.Color = System.Drawing.Color.Empty;
            this.btnChangeColor.Location = new System.Drawing.Point(16, 48);
            this.btnChangeColor.Name = "btnChangeColor";
            this.btnChangeColor.Size = new System.Drawing.Size(200, 24);
            this.btnChangeColor.TabIndex = 1;
            this.btnChangeColor.Text = "Change color...";
            this.btnChangeColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnChangeColor.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(16, 176);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(136, 176);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbBackgroundColor
            // 
            this.gbBackgroundColor.Controls.Add(this.cbTransparent);
            this.gbBackgroundColor.Controls.Add(this.btnChangeColor);
            this.gbBackgroundColor.Location = new System.Drawing.Point(16, 80);
            this.gbBackgroundColor.Name = "gbBackgroundColor";
            this.gbBackgroundColor.Size = new System.Drawing.Size(232, 88);
            this.gbBackgroundColor.TabIndex = 6;
            this.gbBackgroundColor.TabStop = false;
            this.gbBackgroundColor.Text = "Background color";
            // 
            // ImageEditorNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(262, 209);
            this.Controls.Add(this.gbBackgroundColor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblHeightPixels);
            this.Controls.Add(this.lblWidthPixels);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.nudHeight);
            this.Controls.Add(this.nudWidth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImageEditorNew";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - New";
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            this.gbBackgroundColor.ResumeLayout(false);
            this.gbBackgroundColor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeightPixels;
        private System.Windows.Forms.Label lblWidthPixels;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.CheckBox cbTransparent;
        private HelpersLib.ColorButton btnChangeColor;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbBackgroundColor;
    }
}