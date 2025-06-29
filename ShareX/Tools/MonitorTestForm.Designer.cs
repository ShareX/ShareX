namespace ShareX
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
            pSettings = new System.Windows.Forms.Panel();
            btnScreenTearingTest = new System.Windows.Forms.Button();
            btnGradientColor2 = new HelpersLib.ColorButton();
            btnGradientColor1 = new HelpersLib.ColorButton();
            lblTip = new System.Windows.Forms.Label();
            cbGradient = new System.Windows.Forms.ComboBox();
            rbGradient = new System.Windows.Forms.RadioButton();
            btnClose = new System.Windows.Forms.Button();
            lblShapeSize = new System.Windows.Forms.Label();
            lblShapeSizeValue = new System.Windows.Forms.Label();
            tbShapeSize = new System.Windows.Forms.TrackBar();
            btnColorDialog = new System.Windows.Forms.Button();
            cbShapes = new System.Windows.Forms.ComboBox();
            rbShapes = new System.Windows.Forms.RadioButton();
            lblBlue = new System.Windows.Forms.Label();
            lblBlueValue = new System.Windows.Forms.Label();
            tbBlue = new System.Windows.Forms.TrackBar();
            lblGreen = new System.Windows.Forms.Label();
            lblGreenValue = new System.Windows.Forms.Label();
            tbGreen = new System.Windows.Forms.TrackBar();
            lblRed = new System.Windows.Forms.Label();
            lblRedValue = new System.Windows.Forms.Label();
            tbRed = new System.Windows.Forms.TrackBar();
            rbRedGreenBlue = new System.Windows.Forms.RadioButton();
            lblBlackWhiteValue = new System.Windows.Forms.Label();
            tbBlackWhite = new System.Windows.Forms.TrackBar();
            rbBlackWhite = new System.Windows.Forms.RadioButton();
            pSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbShapeSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbBlue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbGreen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbRed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbBlackWhite).BeginInit();
            SuspendLayout();
            // 
            // pSettings
            // 
            pSettings.BackColor = System.Drawing.SystemColors.Window;
            pSettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pSettings.Controls.Add(btnScreenTearingTest);
            pSettings.Controls.Add(btnGradientColor2);
            pSettings.Controls.Add(btnGradientColor1);
            pSettings.Controls.Add(lblTip);
            pSettings.Controls.Add(cbGradient);
            pSettings.Controls.Add(rbGradient);
            pSettings.Controls.Add(btnClose);
            pSettings.Controls.Add(lblShapeSize);
            pSettings.Controls.Add(lblShapeSizeValue);
            pSettings.Controls.Add(tbShapeSize);
            pSettings.Controls.Add(btnColorDialog);
            pSettings.Controls.Add(cbShapes);
            pSettings.Controls.Add(rbShapes);
            pSettings.Controls.Add(lblBlue);
            pSettings.Controls.Add(lblBlueValue);
            pSettings.Controls.Add(tbBlue);
            pSettings.Controls.Add(lblGreen);
            pSettings.Controls.Add(lblGreenValue);
            pSettings.Controls.Add(tbGreen);
            pSettings.Controls.Add(lblRed);
            pSettings.Controls.Add(lblRedValue);
            pSettings.Controls.Add(tbRed);
            pSettings.Controls.Add(rbRedGreenBlue);
            pSettings.Controls.Add(lblBlackWhiteValue);
            pSettings.Controls.Add(tbBlackWhite);
            pSettings.Controls.Add(rbBlackWhite);
            resources.ApplyResources(pSettings, "pSettings");
            pSettings.Name = "pSettings";
            // 
            // btnScreenTearingTest
            // 
            resources.ApplyResources(btnScreenTearingTest, "btnScreenTearingTest");
            btnScreenTearingTest.Name = "btnScreenTearingTest";
            btnScreenTearingTest.UseVisualStyleBackColor = true;
            btnScreenTearingTest.Click += btnScreenTearingTest_Click;
            // 
            // btnGradientColor2
            // 
            btnGradientColor2.Color = System.Drawing.Color.Empty;
            btnGradientColor2.ColorPickerOptions = null;
            resources.ApplyResources(btnGradientColor2, "btnGradientColor2");
            btnGradientColor2.Name = "btnGradientColor2";
            btnGradientColor2.UseVisualStyleBackColor = true;
            btnGradientColor2.ColorChanged += btnGradientColor2_ColorChanged;
            // 
            // btnGradientColor1
            // 
            btnGradientColor1.Color = System.Drawing.Color.Empty;
            btnGradientColor1.ColorPickerOptions = null;
            resources.ApplyResources(btnGradientColor1, "btnGradientColor1");
            btnGradientColor1.Name = "btnGradientColor1";
            btnGradientColor1.UseVisualStyleBackColor = true;
            btnGradientColor1.ColorChanged += btnGradientColor1_ColorChanged;
            // 
            // lblTip
            // 
            lblTip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(lblTip, "lblTip");
            lblTip.Name = "lblTip";
            // 
            // cbGradient
            // 
            cbGradient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbGradient.FormattingEnabled = true;
            resources.ApplyResources(cbGradient, "cbGradient");
            cbGradient.Name = "cbGradient";
            cbGradient.SelectedIndexChanged += cbGradient_SelectedIndexChanged;
            // 
            // rbGradient
            // 
            resources.ApplyResources(rbGradient, "rbGradient");
            rbGradient.Name = "rbGradient";
            rbGradient.UseVisualStyleBackColor = true;
            rbGradient.CheckedChanged += rbGradient_CheckedChanged;
            // 
            // btnClose
            // 
            resources.ApplyResources(btnClose, "btnClose");
            btnClose.Name = "btnClose";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblShapeSize
            // 
            resources.ApplyResources(lblShapeSize, "lblShapeSize");
            lblShapeSize.Name = "lblShapeSize";
            // 
            // lblShapeSizeValue
            // 
            resources.ApplyResources(lblShapeSizeValue, "lblShapeSizeValue");
            lblShapeSizeValue.Name = "lblShapeSizeValue";
            // 
            // tbShapeSize
            // 
            resources.ApplyResources(tbShapeSize, "tbShapeSize");
            tbShapeSize.Maximum = 100;
            tbShapeSize.Minimum = 1;
            tbShapeSize.Name = "tbShapeSize";
            tbShapeSize.TickStyle = System.Windows.Forms.TickStyle.None;
            tbShapeSize.Value = 1;
            tbShapeSize.ValueChanged += tbShapeSize_ValueChanged;
            // 
            // btnColorDialog
            // 
            resources.ApplyResources(btnColorDialog, "btnColorDialog");
            btnColorDialog.Name = "btnColorDialog";
            btnColorDialog.UseVisualStyleBackColor = true;
            btnColorDialog.Click += btnColorDialog_Click;
            // 
            // cbShapes
            // 
            cbShapes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbShapes.FormattingEnabled = true;
            cbShapes.Items.AddRange(new object[] { resources.GetString("cbShapes.Items"), resources.GetString("cbShapes.Items1"), resources.GetString("cbShapes.Items2") });
            resources.ApplyResources(cbShapes, "cbShapes");
            cbShapes.Name = "cbShapes";
            cbShapes.SelectedIndexChanged += cbShapes_SelectedIndexChanged;
            // 
            // rbShapes
            // 
            resources.ApplyResources(rbShapes, "rbShapes");
            rbShapes.Name = "rbShapes";
            rbShapes.UseVisualStyleBackColor = true;
            rbShapes.CheckedChanged += rbShapes_CheckedChanged;
            // 
            // lblBlue
            // 
            resources.ApplyResources(lblBlue, "lblBlue");
            lblBlue.Name = "lblBlue";
            // 
            // lblBlueValue
            // 
            resources.ApplyResources(lblBlueValue, "lblBlueValue");
            lblBlueValue.Name = "lblBlueValue";
            // 
            // tbBlue
            // 
            resources.ApplyResources(tbBlue, "tbBlue");
            tbBlue.Maximum = 255;
            tbBlue.Name = "tbBlue";
            tbBlue.TickStyle = System.Windows.Forms.TickStyle.None;
            tbBlue.ValueChanged += tbRedGreenBlue_ValueChanged;
            // 
            // lblGreen
            // 
            resources.ApplyResources(lblGreen, "lblGreen");
            lblGreen.Name = "lblGreen";
            // 
            // lblGreenValue
            // 
            resources.ApplyResources(lblGreenValue, "lblGreenValue");
            lblGreenValue.Name = "lblGreenValue";
            // 
            // tbGreen
            // 
            resources.ApplyResources(tbGreen, "tbGreen");
            tbGreen.Maximum = 255;
            tbGreen.Name = "tbGreen";
            tbGreen.TickStyle = System.Windows.Forms.TickStyle.None;
            tbGreen.ValueChanged += tbRedGreenBlue_ValueChanged;
            // 
            // lblRed
            // 
            resources.ApplyResources(lblRed, "lblRed");
            lblRed.Name = "lblRed";
            // 
            // lblRedValue
            // 
            resources.ApplyResources(lblRedValue, "lblRedValue");
            lblRedValue.Name = "lblRedValue";
            // 
            // tbRed
            // 
            resources.ApplyResources(tbRed, "tbRed");
            tbRed.Maximum = 255;
            tbRed.Name = "tbRed";
            tbRed.TickStyle = System.Windows.Forms.TickStyle.None;
            tbRed.ValueChanged += tbRedGreenBlue_ValueChanged;
            // 
            // rbRedGreenBlue
            // 
            resources.ApplyResources(rbRedGreenBlue, "rbRedGreenBlue");
            rbRedGreenBlue.Name = "rbRedGreenBlue";
            rbRedGreenBlue.UseVisualStyleBackColor = true;
            rbRedGreenBlue.CheckedChanged += rbRedGreenBlue_CheckedChanged;
            // 
            // lblBlackWhiteValue
            // 
            resources.ApplyResources(lblBlackWhiteValue, "lblBlackWhiteValue");
            lblBlackWhiteValue.Name = "lblBlackWhiteValue";
            // 
            // tbBlackWhite
            // 
            resources.ApplyResources(tbBlackWhite, "tbBlackWhite");
            tbBlackWhite.Maximum = 255;
            tbBlackWhite.Name = "tbBlackWhite";
            tbBlackWhite.TickStyle = System.Windows.Forms.TickStyle.None;
            tbBlackWhite.ValueChanged += tbBlackWhite_ValueChanged;
            // 
            // rbBlackWhite
            // 
            resources.ApplyResources(rbBlackWhite, "rbBlackWhite");
            rbBlackWhite.Name = "rbBlackWhite";
            rbBlackWhite.UseVisualStyleBackColor = true;
            rbBlackWhite.CheckedChanged += rbBlackWhite_CheckedChanged;
            // 
            // MonitorTestForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.White;
            Controls.Add(pSettings);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "MonitorTestForm";
            TopMost = true;
            MouseDown += MonitorTestForm_MouseDown;
            pSettings.ResumeLayout(false);
            pSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbShapeSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbBlue).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbGreen).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbRed).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbBlackWhite).EndInit();
            ResumeLayout(false);

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
        private HelpersLib.ColorButton btnGradientColor2;
        private HelpersLib.ColorButton btnGradientColor1;
        private System.Windows.Forms.Button btnScreenTearingTest;
    }
}