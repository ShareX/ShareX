namespace ShareX.HelpersLib
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorTestForm));
            this.pSettings = new System.Windows.Forms.Panel();
            this.btnScreenTearingTest = new System.Windows.Forms.Button();
            this.btnGradientColor2 = new ShareX.HelpersLib.ColorButton();
            this.btnGradientColor1 = new ShareX.HelpersLib.ColorButton();
            this.lblTip = new System.Windows.Forms.Label();
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
            this.pSettings.BackColor = System.Drawing.SystemColors.Window;
            this.pSettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSettings.Controls.Add(this.btnScreenTearingTest);
            this.pSettings.Controls.Add(this.btnGradientColor2);
            this.pSettings.Controls.Add(this.btnGradientColor1);
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
            resources.ApplyResources(this.pSettings, "pSettings");
            this.pSettings.Name = "pSettings";
            // 
            // btnScreenTearingTest
            // 
            resources.ApplyResources(this.btnScreenTearingTest, "btnScreenTearingTest");
            this.btnScreenTearingTest.Name = "btnScreenTearingTest";
            this.btnScreenTearingTest.UseVisualStyleBackColor = true;
            this.btnScreenTearingTest.Click += new System.EventHandler(this.btnScreenTearingTest_Click);
            // 
            // btnGradientColor2
            // 
            this.btnGradientColor2.Color = System.Drawing.Color.Empty;
            this.btnGradientColor2.ColorPickerOptions = null;
            resources.ApplyResources(this.btnGradientColor2, "btnGradientColor2");
            this.btnGradientColor2.Name = "btnGradientColor2";
            this.btnGradientColor2.UseVisualStyleBackColor = true;
            this.btnGradientColor2.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.btnGradientColor2_ColorChanged);
            // 
            // btnGradientColor1
            // 
            this.btnGradientColor1.Color = System.Drawing.Color.Empty;
            this.btnGradientColor1.ColorPickerOptions = null;
            resources.ApplyResources(this.btnGradientColor1, "btnGradientColor1");
            this.btnGradientColor1.Name = "btnGradientColor1";
            this.btnGradientColor1.UseVisualStyleBackColor = true;
            this.btnGradientColor1.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.btnGradientColor1_ColorChanged);
            // 
            // lblTip
            // 
            this.lblTip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblTip, "lblTip");
            this.lblTip.Name = "lblTip";
            // 
            // cbGradient
            // 
            this.cbGradient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGradient.FormattingEnabled = true;
            resources.ApplyResources(this.cbGradient, "cbGradient");
            this.cbGradient.Name = "cbGradient";
            this.cbGradient.SelectedIndexChanged += new System.EventHandler(this.cbGradient_SelectedIndexChanged);
            // 
            // rbGradient
            // 
            resources.ApplyResources(this.rbGradient, "rbGradient");
            this.rbGradient.Name = "rbGradient";
            this.rbGradient.UseVisualStyleBackColor = true;
            this.rbGradient.CheckedChanged += new System.EventHandler(this.rbGradient_CheckedChanged);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblShapeSize
            // 
            resources.ApplyResources(this.lblShapeSize, "lblShapeSize");
            this.lblShapeSize.Name = "lblShapeSize";
            // 
            // lblShapeSizeValue
            // 
            resources.ApplyResources(this.lblShapeSizeValue, "lblShapeSizeValue");
            this.lblShapeSizeValue.Name = "lblShapeSizeValue";
            // 
            // tbShapeSize
            // 
            resources.ApplyResources(this.tbShapeSize, "tbShapeSize");
            this.tbShapeSize.Maximum = 100;
            this.tbShapeSize.Minimum = 1;
            this.tbShapeSize.Name = "tbShapeSize";
            this.tbShapeSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbShapeSize.Value = 1;
            this.tbShapeSize.ValueChanged += new System.EventHandler(this.tbShapeSize_ValueChanged);
            // 
            // btnColorDialog
            // 
            resources.ApplyResources(this.btnColorDialog, "btnColorDialog");
            this.btnColorDialog.Name = "btnColorDialog";
            this.btnColorDialog.UseVisualStyleBackColor = true;
            this.btnColorDialog.Click += new System.EventHandler(this.btnColorDialog_Click);
            // 
            // cbShapes
            // 
            this.cbShapes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbShapes.FormattingEnabled = true;
            this.cbShapes.Items.AddRange(new object[] {
            resources.GetString("cbShapes.Items"),
            resources.GetString("cbShapes.Items1"),
            resources.GetString("cbShapes.Items2")});
            resources.ApplyResources(this.cbShapes, "cbShapes");
            this.cbShapes.Name = "cbShapes";
            this.cbShapes.SelectedIndexChanged += new System.EventHandler(this.cbShapes_SelectedIndexChanged);
            // 
            // rbShapes
            // 
            resources.ApplyResources(this.rbShapes, "rbShapes");
            this.rbShapes.Name = "rbShapes";
            this.rbShapes.UseVisualStyleBackColor = true;
            this.rbShapes.CheckedChanged += new System.EventHandler(this.rbShapes_CheckedChanged);
            // 
            // lblBlue
            // 
            resources.ApplyResources(this.lblBlue, "lblBlue");
            this.lblBlue.Name = "lblBlue";
            // 
            // lblBlueValue
            // 
            resources.ApplyResources(this.lblBlueValue, "lblBlueValue");
            this.lblBlueValue.Name = "lblBlueValue";
            // 
            // tbBlue
            // 
            resources.ApplyResources(this.tbBlue, "tbBlue");
            this.tbBlue.Maximum = 255;
            this.tbBlue.Name = "tbBlue";
            this.tbBlue.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbBlue.ValueChanged += new System.EventHandler(this.tbRedGreenBlue_ValueChanged);
            // 
            // lblGreen
            // 
            resources.ApplyResources(this.lblGreen, "lblGreen");
            this.lblGreen.Name = "lblGreen";
            // 
            // lblGreenValue
            // 
            resources.ApplyResources(this.lblGreenValue, "lblGreenValue");
            this.lblGreenValue.Name = "lblGreenValue";
            // 
            // tbGreen
            // 
            resources.ApplyResources(this.tbGreen, "tbGreen");
            this.tbGreen.Maximum = 255;
            this.tbGreen.Name = "tbGreen";
            this.tbGreen.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbGreen.ValueChanged += new System.EventHandler(this.tbRedGreenBlue_ValueChanged);
            // 
            // lblRed
            // 
            resources.ApplyResources(this.lblRed, "lblRed");
            this.lblRed.Name = "lblRed";
            // 
            // lblRedValue
            // 
            resources.ApplyResources(this.lblRedValue, "lblRedValue");
            this.lblRedValue.Name = "lblRedValue";
            // 
            // tbRed
            // 
            resources.ApplyResources(this.tbRed, "tbRed");
            this.tbRed.Maximum = 255;
            this.tbRed.Name = "tbRed";
            this.tbRed.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRed.ValueChanged += new System.EventHandler(this.tbRedGreenBlue_ValueChanged);
            // 
            // rbRedGreenBlue
            // 
            resources.ApplyResources(this.rbRedGreenBlue, "rbRedGreenBlue");
            this.rbRedGreenBlue.Name = "rbRedGreenBlue";
            this.rbRedGreenBlue.UseVisualStyleBackColor = true;
            this.rbRedGreenBlue.CheckedChanged += new System.EventHandler(this.rbRedGreenBlue_CheckedChanged);
            // 
            // lblBlackWhiteValue
            // 
            resources.ApplyResources(this.lblBlackWhiteValue, "lblBlackWhiteValue");
            this.lblBlackWhiteValue.Name = "lblBlackWhiteValue";
            // 
            // tbBlackWhite
            // 
            resources.ApplyResources(this.tbBlackWhite, "tbBlackWhite");
            this.tbBlackWhite.Maximum = 255;
            this.tbBlackWhite.Name = "tbBlackWhite";
            this.tbBlackWhite.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbBlackWhite.ValueChanged += new System.EventHandler(this.tbBlackWhite_ValueChanged);
            // 
            // rbBlackWhite
            // 
            resources.ApplyResources(this.rbBlackWhite, "rbBlackWhite");
            this.rbBlackWhite.Name = "rbBlackWhite";
            this.rbBlackWhite.UseVisualStyleBackColor = true;
            this.rbBlackWhite.CheckedChanged += new System.EventHandler(this.rbBlackWhite_CheckedChanged);
            // 
            // MonitorTestForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pSettings);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MonitorTestForm";
            this.TopMost = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MonitorTestForm_MouseDown);
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
        private ColorButton btnGradientColor2;
        private ColorButton btnGradientColor1;
        private System.Windows.Forms.Button btnScreenTearingTest;
    }
}