namespace HelpersLib
{
    partial class MonitorTestForm
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
            this.pSettings = new System.Windows.Forms.Panel();
            this.cbGradient = new System.Windows.Forms.ComboBox();
            this.rbGradient = new System.Windows.Forms.RadioButton();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblShapeSize = new System.Windows.Forms.Label();
            this.lblShapeSizeValue = new System.Windows.Forms.Label();
            this.tbShapeSize = new System.Windows.Forms.TrackBar();
            this.btnColorDialog = new System.Windows.Forms.Button();
            this.cbShapes = new System.Windows.Forms.ComboBox();
            this.rbShapes = new System.Windows.Forms.RadioButton();
            this.lblBlue = new System.Windows.Forms.Label();
            this.lblBlueValue = new System.Windows.Forms.Label();
            this.tbBlue = new System.Windows.Forms.TrackBar();
            this.lblGreen = new System.Windows.Forms.Label();
            this.lblGreenValue = new System.Windows.Forms.Label();
            this.tbGreen = new System.Windows.Forms.TrackBar();
            this.lblRed = new System.Windows.Forms.Label();
            this.lblRedValue = new System.Windows.Forms.Label();
            this.tbRed = new System.Windows.Forms.TrackBar();
            this.rbRedGreenBlue = new System.Windows.Forms.RadioButton();
            this.lblBlackWhiteValue = new System.Windows.Forms.Label();
            this.tbBlackWhite = new System.Windows.Forms.TrackBar();
            this.rbBlackWhite = new System.Windows.Forms.RadioButton();
            this.lblTip = new System.Windows.Forms.Label();
            this.pSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbShapeSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBlackWhite)).BeginInit();
            this.SuspendLayout();
            // 
            // pSettings
            // 
            this.pSettings.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pSettings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pSettings.Controls.Add(this.lblTip);
            this.pSettings.Controls.Add(this.cbGradient);
            this.pSettings.Controls.Add(this.rbGradient);
            this.pSettings.Controls.Add(this.btnClose);
            this.pSettings.Controls.Add(this.lblShapeSize);
            this.pSettings.Controls.Add(this.lblShapeSizeValue);
            this.pSettings.Controls.Add(this.tbShapeSize);
            this.pSettings.Controls.Add(this.btnColorDialog);
            this.pSettings.Controls.Add(this.cbShapes);
            this.pSettings.Controls.Add(this.rbShapes);
            this.pSettings.Controls.Add(this.lblBlue);
            this.pSettings.Controls.Add(this.lblBlueValue);
            this.pSettings.Controls.Add(this.tbBlue);
            this.pSettings.Controls.Add(this.lblGreen);
            this.pSettings.Controls.Add(this.lblGreenValue);
            this.pSettings.Controls.Add(this.tbGreen);
            this.pSettings.Controls.Add(this.lblRed);
            this.pSettings.Controls.Add(this.lblRedValue);
            this.pSettings.Controls.Add(this.tbRed);
            this.pSettings.Controls.Add(this.rbRedGreenBlue);
            this.pSettings.Controls.Add(this.lblBlackWhiteValue);
            this.pSettings.Controls.Add(this.tbBlackWhite);
            this.pSettings.Controls.Add(this.rbBlackWhite);
            this.pSettings.Location = new System.Drawing.Point(24, 24);
            this.pSettings.Name = "pSettings";
            this.pSettings.Size = new System.Drawing.Size(320, 368);
            this.pSettings.TabIndex = 0;
            // 
            // cbGradient
            // 
            this.cbGradient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGradient.FormattingEnabled = true;
            this.cbGradient.Location = new System.Drawing.Point(88, 222);
            this.cbGradient.Name = "cbGradient";
            this.cbGradient.Size = new System.Drawing.Size(216, 21);
            this.cbGradient.TabIndex = 22;
            this.cbGradient.SelectedIndexChanged += new System.EventHandler(this.cbGradient_SelectedIndexChanged);
            // 
            // rbGradient
            // 
            this.rbGradient.AutoSize = true;
            this.rbGradient.Location = new System.Drawing.Point(16, 224);
            this.rbGradient.Name = "rbGradient";
            this.rbGradient.Size = new System.Drawing.Size(68, 17);
            this.rbGradient.TabIndex = 21;
            this.rbGradient.Text = "Gradient:";
            this.rbGradient.UseVisualStyleBackColor = true;
            this.rbGradient.CheckedChanged += new System.EventHandler(this.rbGradient_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(208, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(96, 23);
            this.btnClose.TabIndex = 20;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblShapeSize
            // 
            this.lblShapeSize.AutoSize = true;
            this.lblShapeSize.Location = new System.Drawing.Point(16, 302);
            this.lblShapeSize.Name = "lblShapeSize";
            this.lblShapeSize.Size = new System.Drawing.Size(30, 13);
            this.lblShapeSize.TabIndex = 19;
            this.lblShapeSize.Text = "Size:";
            // 
            // lblShapeSizeValue
            // 
            this.lblShapeSizeValue.AutoSize = true;
            this.lblShapeSizeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblShapeSizeValue.Location = new System.Drawing.Point(280, 300);
            this.lblShapeSizeValue.Name = "lblShapeSizeValue";
            this.lblShapeSizeValue.Size = new System.Drawing.Size(15, 16);
            this.lblShapeSizeValue.TabIndex = 18;
            this.lblShapeSizeValue.Text = "1";
            // 
            // tbShapeSize
            // 
            this.tbShapeSize.AutoSize = false;
            this.tbShapeSize.Location = new System.Drawing.Point(40, 296);
            this.tbShapeSize.Maximum = 100;
            this.tbShapeSize.Minimum = 1;
            this.tbShapeSize.Name = "tbShapeSize";
            this.tbShapeSize.Size = new System.Drawing.Size(232, 24);
            this.tbShapeSize.TabIndex = 17;
            this.tbShapeSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbShapeSize.Value = 1;
            this.tbShapeSize.ValueChanged += new System.EventHandler(this.tbShapeSize_ValueChanged);
            // 
            // btnColorDialog
            // 
            this.btnColorDialog.Location = new System.Drawing.Point(208, 88);
            this.btnColorDialog.Name = "btnColorDialog";
            this.btnColorDialog.Size = new System.Drawing.Size(96, 23);
            this.btnColorDialog.TabIndex = 16;
            this.btnColorDialog.Text = "Color dialog...";
            this.btnColorDialog.UseVisualStyleBackColor = true;
            this.btnColorDialog.Click += new System.EventHandler(this.btnColorDialog_Click);
            // 
            // cbShapes
            // 
            this.cbShapes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbShapes.FormattingEnabled = true;
            this.cbShapes.Items.AddRange(new object[] {
            "Checker",
            "Horizontal lines",
            "Vertical lines"});
            this.cbShapes.Location = new System.Drawing.Point(88, 262);
            this.cbShapes.Name = "cbShapes";
            this.cbShapes.Size = new System.Drawing.Size(216, 21);
            this.cbShapes.TabIndex = 15;
            this.cbShapes.SelectedIndexChanged += new System.EventHandler(this.cbShapes_SelectedIndexChanged);
            // 
            // rbShapes
            // 
            this.rbShapes.AutoSize = true;
            this.rbShapes.Location = new System.Drawing.Point(16, 264);
            this.rbShapes.Name = "rbShapes";
            this.rbShapes.Size = new System.Drawing.Size(59, 17);
            this.rbShapes.TabIndex = 14;
            this.rbShapes.Text = "Shape:";
            this.rbShapes.UseVisualStyleBackColor = true;
            this.rbShapes.CheckedChanged += new System.EventHandler(this.rbShapes_CheckedChanged);
            // 
            // lblBlue
            // 
            this.lblBlue.AutoSize = true;
            this.lblBlue.Location = new System.Drawing.Point(16, 190);
            this.lblBlue.Name = "lblBlue";
            this.lblBlue.Size = new System.Drawing.Size(17, 13);
            this.lblBlue.TabIndex = 13;
            this.lblBlue.Text = "B:";
            // 
            // lblBlueValue
            // 
            this.lblBlueValue.AutoSize = true;
            this.lblBlueValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBlueValue.Location = new System.Drawing.Point(280, 188);
            this.lblBlueValue.Name = "lblBlueValue";
            this.lblBlueValue.Size = new System.Drawing.Size(15, 16);
            this.lblBlueValue.TabIndex = 12;
            this.lblBlueValue.Text = "0";
            // 
            // tbBlue
            // 
            this.tbBlue.AutoSize = false;
            this.tbBlue.Location = new System.Drawing.Point(32, 184);
            this.tbBlue.Maximum = 255;
            this.tbBlue.Name = "tbBlue";
            this.tbBlue.Size = new System.Drawing.Size(240, 24);
            this.tbBlue.TabIndex = 11;
            this.tbBlue.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbBlue.ValueChanged += new System.EventHandler(this.tbRedGreenBlue_ValueChanged);
            // 
            // lblGreen
            // 
            this.lblGreen.AutoSize = true;
            this.lblGreen.Location = new System.Drawing.Point(16, 158);
            this.lblGreen.Name = "lblGreen";
            this.lblGreen.Size = new System.Drawing.Size(18, 13);
            this.lblGreen.TabIndex = 10;
            this.lblGreen.Text = "G:";
            // 
            // lblGreenValue
            // 
            this.lblGreenValue.AutoSize = true;
            this.lblGreenValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblGreenValue.Location = new System.Drawing.Point(280, 156);
            this.lblGreenValue.Name = "lblGreenValue";
            this.lblGreenValue.Size = new System.Drawing.Size(15, 16);
            this.lblGreenValue.TabIndex = 9;
            this.lblGreenValue.Text = "0";
            // 
            // tbGreen
            // 
            this.tbGreen.AutoSize = false;
            this.tbGreen.Location = new System.Drawing.Point(32, 152);
            this.tbGreen.Maximum = 255;
            this.tbGreen.Name = "tbGreen";
            this.tbGreen.Size = new System.Drawing.Size(240, 24);
            this.tbGreen.TabIndex = 8;
            this.tbGreen.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbGreen.ValueChanged += new System.EventHandler(this.tbRedGreenBlue_ValueChanged);
            // 
            // lblRed
            // 
            this.lblRed.AutoSize = true;
            this.lblRed.Location = new System.Drawing.Point(16, 126);
            this.lblRed.Name = "lblRed";
            this.lblRed.Size = new System.Drawing.Size(18, 13);
            this.lblRed.TabIndex = 7;
            this.lblRed.Text = "R:";
            // 
            // lblRedValue
            // 
            this.lblRedValue.AutoSize = true;
            this.lblRedValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblRedValue.Location = new System.Drawing.Point(280, 124);
            this.lblRedValue.Name = "lblRedValue";
            this.lblRedValue.Size = new System.Drawing.Size(15, 16);
            this.lblRedValue.TabIndex = 6;
            this.lblRedValue.Text = "0";
            // 
            // tbRed
            // 
            this.tbRed.AutoSize = false;
            this.tbRed.Location = new System.Drawing.Point(32, 120);
            this.tbRed.Maximum = 255;
            this.tbRed.Name = "tbRed";
            this.tbRed.Size = new System.Drawing.Size(240, 24);
            this.tbRed.TabIndex = 5;
            this.tbRed.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRed.ValueChanged += new System.EventHandler(this.tbRedGreenBlue_ValueChanged);
            // 
            // rbRedGreenBlue
            // 
            this.rbRedGreenBlue.AutoSize = true;
            this.rbRedGreenBlue.Location = new System.Drawing.Point(16, 88);
            this.rbRedGreenBlue.Name = "rbRedGreenBlue";
            this.rbRedGreenBlue.Size = new System.Drawing.Size(110, 17);
            this.rbRedGreenBlue.TabIndex = 4;
            this.rbRedGreenBlue.Text = "Red, Green, Blue:";
            this.rbRedGreenBlue.UseVisualStyleBackColor = true;
            this.rbRedGreenBlue.CheckedChanged += new System.EventHandler(this.rbRedGreenBlue_CheckedChanged);
            // 
            // lblBlackWhiteValue
            // 
            this.lblBlackWhiteValue.AutoSize = true;
            this.lblBlackWhiteValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBlackWhiteValue.Location = new System.Drawing.Point(280, 52);
            this.lblBlackWhiteValue.Name = "lblBlackWhiteValue";
            this.lblBlackWhiteValue.Size = new System.Drawing.Size(15, 16);
            this.lblBlackWhiteValue.TabIndex = 3;
            this.lblBlackWhiteValue.Text = "0";
            // 
            // tbBlackWhite
            // 
            this.tbBlackWhite.AutoSize = false;
            this.tbBlackWhite.Location = new System.Drawing.Point(8, 48);
            this.tbBlackWhite.Maximum = 255;
            this.tbBlackWhite.Name = "tbBlackWhite";
            this.tbBlackWhite.Size = new System.Drawing.Size(264, 24);
            this.tbBlackWhite.TabIndex = 2;
            this.tbBlackWhite.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbBlackWhite.ValueChanged += new System.EventHandler(this.tbBlackWhite_ValueChanged);
            // 
            // rbBlackWhite
            // 
            this.rbBlackWhite.AutoSize = true;
            this.rbBlackWhite.Location = new System.Drawing.Point(16, 16);
            this.rbBlackWhite.Name = "rbBlackWhite";
            this.rbBlackWhite.Size = new System.Drawing.Size(89, 17);
            this.rbBlackWhite.TabIndex = 1;
            this.rbBlackWhite.Text = "Black, White:";
            this.rbBlackWhite.UseVisualStyleBackColor = true;
            this.rbBlackWhite.CheckedChanged += new System.EventHandler(this.rbBlackWhite_CheckedChanged);
            // 
            // lblTip
            // 
            this.lblTip.AutoSize = true;
            this.lblTip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTip.Location = new System.Drawing.Point(16, 336);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(279, 16);
            this.lblTip.TabIndex = 23;
            this.lblTip.Text = "You can click outside for hide/show this panel.";
            // 
            // MonitorTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.Controls.Add(this.pSettings);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "MonitorTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MonitorTest";
            this.TopMost = true;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.pSettings.ResumeLayout(false);
            this.pSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbShapeSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBlackWhite)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pSettings;
        private System.Windows.Forms.Label lblBlackWhiteValue;
        private System.Windows.Forms.TrackBar tbBlackWhite;
        private System.Windows.Forms.RadioButton rbBlackWhite;
        private System.Windows.Forms.Label lblBlue;
        private System.Windows.Forms.Label lblBlueValue;
        private System.Windows.Forms.TrackBar tbBlue;
        private System.Windows.Forms.Label lblGreen;
        private System.Windows.Forms.Label lblGreenValue;
        private System.Windows.Forms.TrackBar tbGreen;
        private System.Windows.Forms.Label lblRed;
        private System.Windows.Forms.Label lblRedValue;
        private System.Windows.Forms.TrackBar tbRed;
        private System.Windows.Forms.RadioButton rbRedGreenBlue;
        private System.Windows.Forms.ComboBox cbShapes;
        private System.Windows.Forms.RadioButton rbShapes;
        private System.Windows.Forms.Button btnColorDialog;
        private System.Windows.Forms.Label lblShapeSize;
        private System.Windows.Forms.Label lblShapeSizeValue;
        private System.Windows.Forms.TrackBar tbShapeSize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cbGradient;
        private System.Windows.Forms.RadioButton rbGradient;
        private System.Windows.Forms.Label lblTip;

    }
}