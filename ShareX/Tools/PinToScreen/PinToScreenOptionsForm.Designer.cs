namespace ShareX
{
    partial class PinToScreenOptionsForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPlacement = new System.Windows.Forms.Label();
            this.cbPlacement = new System.Windows.Forms.ComboBox();
            this.lblPlacementOffset = new System.Windows.Forms.Label();
            this.nudPlacementOffset = new System.Windows.Forms.NumericUpDown();
            this.cbTopMost = new System.Windows.Forms.CheckBox();
            this.cbKeepCenterLocation = new System.Windows.Forms.CheckBox();
            this.cbShadow = new System.Windows.Forms.CheckBox();
            this.cbBorder = new System.Windows.Forms.CheckBox();
            this.lblBorderSize = new System.Windows.Forms.Label();
            this.nudBorderSize = new System.Windows.Forms.NumericUpDown();
            this.btnBorderColor = new ShareX.HelpersLib.ColorButton();
            this.lblMinimizeSize = new System.Windows.Forms.Label();
            this.nudMinimizeSizeWidth = new System.Windows.Forms.NumericUpDown();
            this.nudMinimizeSizeHeight = new System.Windows.Forms.NumericUpDown();
            this.lblMinimizeSizeX = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlacementOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBorderSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinimizeSizeWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinimizeSizeHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(192, 336);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(104, 32);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(304, 336);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 32);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblPlacement
            // 
            this.lblPlacement.AutoSize = true;
            this.lblPlacement.Location = new System.Drawing.Point(13, 16);
            this.lblPlacement.Name = "lblPlacement";
            this.lblPlacement.Size = new System.Drawing.Size(74, 16);
            this.lblPlacement.TabIndex = 0;
            this.lblPlacement.Text = "Placement:";
            // 
            // cbPlacement
            // 
            this.cbPlacement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPlacement.FormattingEnabled = true;
            this.cbPlacement.Location = new System.Drawing.Point(16, 40);
            this.cbPlacement.Name = "cbPlacement";
            this.cbPlacement.Size = new System.Drawing.Size(144, 24);
            this.cbPlacement.TabIndex = 1;
            // 
            // lblPlacementOffset
            // 
            this.lblPlacementOffset.AutoSize = true;
            this.lblPlacementOffset.Location = new System.Drawing.Point(173, 16);
            this.lblPlacementOffset.Name = "lblPlacementOffset";
            this.lblPlacementOffset.Size = new System.Drawing.Size(109, 16);
            this.lblPlacementOffset.TabIndex = 2;
            this.lblPlacementOffset.Text = "Placement offset:";
            // 
            // nudPlacementOffset
            // 
            this.nudPlacementOffset.Location = new System.Drawing.Point(176, 41);
            this.nudPlacementOffset.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudPlacementOffset.Name = "nudPlacementOffset";
            this.nudPlacementOffset.Size = new System.Drawing.Size(72, 22);
            this.nudPlacementOffset.TabIndex = 3;
            this.nudPlacementOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbTopMost
            // 
            this.cbTopMost.AutoSize = true;
            this.cbTopMost.Location = new System.Drawing.Point(16, 80);
            this.cbTopMost.Name = "cbTopMost";
            this.cbTopMost.Size = new System.Drawing.Size(83, 20);
            this.cbTopMost.TabIndex = 4;
            this.cbTopMost.Text = "Top most";
            this.cbTopMost.UseVisualStyleBackColor = true;
            // 
            // cbKeepCenterLocation
            // 
            this.cbKeepCenterLocation.AutoSize = true;
            this.cbKeepCenterLocation.Location = new System.Drawing.Point(16, 112);
            this.cbKeepCenterLocation.Name = "cbKeepCenterLocation";
            this.cbKeepCenterLocation.Size = new System.Drawing.Size(148, 20);
            this.cbKeepCenterLocation.TabIndex = 5;
            this.cbKeepCenterLocation.Text = "Keep center location";
            this.cbKeepCenterLocation.UseVisualStyleBackColor = true;
            // 
            // cbShadow
            // 
            this.cbShadow.AutoSize = true;
            this.cbShadow.Location = new System.Drawing.Point(16, 144);
            this.cbShadow.Name = "cbShadow";
            this.cbShadow.Size = new System.Drawing.Size(75, 20);
            this.cbShadow.TabIndex = 6;
            this.cbShadow.Text = "Shadow";
            this.cbShadow.UseVisualStyleBackColor = true;
            // 
            // cbBorder
            // 
            this.cbBorder.AutoSize = true;
            this.cbBorder.Location = new System.Drawing.Point(16, 176);
            this.cbBorder.Name = "cbBorder";
            this.cbBorder.Size = new System.Drawing.Size(67, 20);
            this.cbBorder.TabIndex = 7;
            this.cbBorder.Text = "Border";
            this.cbBorder.UseVisualStyleBackColor = true;
            // 
            // lblBorderSize
            // 
            this.lblBorderSize.AutoSize = true;
            this.lblBorderSize.Location = new System.Drawing.Point(13, 208);
            this.lblBorderSize.Name = "lblBorderSize";
            this.lblBorderSize.Size = new System.Drawing.Size(78, 16);
            this.lblBorderSize.TabIndex = 8;
            this.lblBorderSize.Text = "Border size:";
            // 
            // nudBorderSize
            // 
            this.nudBorderSize.Location = new System.Drawing.Point(16, 232);
            this.nudBorderSize.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudBorderSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBorderSize.Name = "nudBorderSize";
            this.nudBorderSize.Size = new System.Drawing.Size(72, 22);
            this.nudBorderSize.TabIndex = 9;
            this.nudBorderSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudBorderSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnBorderColor
            // 
            this.btnBorderColor.Color = System.Drawing.Color.Empty;
            this.btnBorderColor.ColorPickerOptions = null;
            this.btnBorderColor.Location = new System.Drawing.Point(104, 230);
            this.btnBorderColor.Name = "btnBorderColor";
            this.btnBorderColor.Size = new System.Drawing.Size(160, 26);
            this.btnBorderColor.TabIndex = 10;
            this.btnBorderColor.Text = "Border color";
            this.btnBorderColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBorderColor.UseVisualStyleBackColor = true;
            // 
            // lblMinimizeSize
            // 
            this.lblMinimizeSize.AutoSize = true;
            this.lblMinimizeSize.Location = new System.Drawing.Point(13, 272);
            this.lblMinimizeSize.Name = "lblMinimizeSize";
            this.lblMinimizeSize.Size = new System.Drawing.Size(89, 16);
            this.lblMinimizeSize.TabIndex = 11;
            this.lblMinimizeSize.Text = "Minimize size:";
            // 
            // nudMinimizeSizeWidth
            // 
            this.nudMinimizeSizeWidth.Location = new System.Drawing.Point(16, 296);
            this.nudMinimizeSizeWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMinimizeSizeWidth.Name = "nudMinimizeSizeWidth";
            this.nudMinimizeSizeWidth.Size = new System.Drawing.Size(72, 22);
            this.nudMinimizeSizeWidth.TabIndex = 12;
            this.nudMinimizeSizeWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nudMinimizeSizeHeight
            // 
            this.nudMinimizeSizeHeight.Location = new System.Drawing.Point(120, 296);
            this.nudMinimizeSizeHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMinimizeSizeHeight.Name = "nudMinimizeSizeHeight";
            this.nudMinimizeSizeHeight.Size = new System.Drawing.Size(72, 22);
            this.nudMinimizeSizeHeight.TabIndex = 14;
            this.nudMinimizeSizeHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblMinimizeSizeX
            // 
            this.lblMinimizeSizeX.AutoSize = true;
            this.lblMinimizeSizeX.Location = new System.Drawing.Point(98, 299);
            this.lblMinimizeSizeX.Name = "lblMinimizeSizeX";
            this.lblMinimizeSizeX.Size = new System.Drawing.Size(13, 16);
            this.lblMinimizeSizeX.TabIndex = 13;
            this.lblMinimizeSizeX.Text = "x";
            // 
            // PinToScreenOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 380);
            this.Controls.Add(this.lblMinimizeSizeX);
            this.Controls.Add(this.nudMinimizeSizeHeight);
            this.Controls.Add(this.nudMinimizeSizeWidth);
            this.Controls.Add(this.lblMinimizeSize);
            this.Controls.Add(this.btnBorderColor);
            this.Controls.Add(this.nudBorderSize);
            this.Controls.Add(this.lblBorderSize);
            this.Controls.Add(this.cbBorder);
            this.Controls.Add(this.cbShadow);
            this.Controls.Add(this.cbKeepCenterLocation);
            this.Controls.Add(this.cbTopMost);
            this.Controls.Add(this.nudPlacementOffset);
            this.Controls.Add(this.lblPlacementOffset);
            this.Controls.Add(this.cbPlacement);
            this.Controls.Add(this.lblPlacement);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "PinToScreenOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Pin to screen options";
            ((System.ComponentModel.ISupportInitialize)(this.nudPlacementOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBorderSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinimizeSizeWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinimizeSizeHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblPlacement;
        private System.Windows.Forms.ComboBox cbPlacement;
        private System.Windows.Forms.Label lblPlacementOffset;
        private System.Windows.Forms.NumericUpDown nudPlacementOffset;
        private System.Windows.Forms.CheckBox cbTopMost;
        private System.Windows.Forms.CheckBox cbKeepCenterLocation;
        private System.Windows.Forms.CheckBox cbShadow;
        private System.Windows.Forms.CheckBox cbBorder;
        private System.Windows.Forms.Label lblBorderSize;
        private System.Windows.Forms.NumericUpDown nudBorderSize;
        private HelpersLib.ColorButton btnBorderColor;
        private System.Windows.Forms.Label lblMinimizeSize;
        private System.Windows.Forms.NumericUpDown nudMinimizeSizeWidth;
        private System.Windows.Forms.NumericUpDown nudMinimizeSizeHeight;
        private System.Windows.Forms.Label lblMinimizeSizeX;
    }
}