namespace HelpersLib
{
    partial class DialogColor
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblOld = new System.Windows.Forms.Label();
            this.lblNew = new System.Windows.Forms.Label();
            this.txtHex = new System.Windows.Forms.TextBox();
            this.lblHex = new System.Windows.Forms.Label();
            this.nudKey = new System.Windows.Forms.NumericUpDown();
            this.nudYellow = new System.Windows.Forms.NumericUpDown();
            this.nudMagenta = new System.Windows.Forms.NumericUpDown();
            this.nudCyan = new System.Windows.Forms.NumericUpDown();
            this.lblKey = new System.Windows.Forms.Label();
            this.lblYellow = new System.Windows.Forms.Label();
            this.lblMagenta = new System.Windows.Forms.Label();
            this.lblCyan = new System.Windows.Forms.Label();
            this.lblHue = new System.Windows.Forms.Label();
            this.lblBrightnessPerc = new System.Windows.Forms.Label();
            this.lblSaturationPerc = new System.Windows.Forms.Label();
            this.nudBlue = new System.Windows.Forms.NumericUpDown();
            this.nudGreen = new System.Windows.Forms.NumericUpDown();
            this.nudRed = new System.Windows.Forms.NumericUpDown();
            this.nudBrightness = new System.Windows.Forms.NumericUpDown();
            this.nudSaturation = new System.Windows.Forms.NumericUpDown();
            this.nudHue = new System.Windows.Forms.NumericUpDown();
            this.rbBlue = new System.Windows.Forms.RadioButton();
            this.rbGreen = new System.Windows.Forms.RadioButton();
            this.rbRed = new System.Windows.Forms.RadioButton();
            this.rbBrightness = new System.Windows.Forms.RadioButton();
            this.rbSaturation = new System.Windows.Forms.RadioButton();
            this.rbHue = new System.Windows.Forms.RadioButton();
            this.lblDecimal = new System.Windows.Forms.Label();
            this.txtDecimal = new System.Windows.Forms.TextBox();
            this.lblCyanPerc = new System.Windows.Forms.Label();
            this.lblMagentaPerc = new System.Windows.Forms.Label();
            this.lblYellowPerc = new System.Windows.Forms.Label();
            this.lblKeyPerc = new System.Windows.Forms.Label();
            this.nudAlpha = new System.Windows.Forms.NumericUpDown();
            this.lblAlpha = new System.Windows.Forms.Label();
            this.pbColorPreview = new HelpersLib.MyPictureBox();
            this.colorPicker = new HelpersLib.ColorPicker();
            ((System.ComponentModel.ISupportInitialize)(this.nudKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudYellow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMagenta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCyan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(536, 240);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 24);
            this.btnCancel.TabIndex = 41;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(464, 240);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(64, 24);
            this.btnOK.TabIndex = 40;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblOld
            // 
            this.lblOld.AutoSize = true;
            this.lblOld.Location = new System.Drawing.Point(312, 240);
            this.lblOld.Name = "lblOld";
            this.lblOld.Size = new System.Drawing.Size(26, 13);
            this.lblOld.TabIndex = 33;
            this.lblOld.Text = "Old:";
            // 
            // lblNew
            // 
            this.lblNew.AutoSize = true;
            this.lblNew.Location = new System.Drawing.Point(312, 216);
            this.lblNew.Name = "lblNew";
            this.lblNew.Size = new System.Drawing.Size(32, 13);
            this.lblNew.TabIndex = 32;
            this.lblNew.Text = "New:";
            // 
            // txtHex
            // 
            this.txtHex.Location = new System.Drawing.Point(528, 173);
            this.txtHex.MaxLength = 9;
            this.txtHex.Name = "txtHex";
            this.txtHex.Size = new System.Drawing.Size(72, 20);
            this.txtHex.TabIndex = 27;
            this.txtHex.Text = "FF00FF00";
            this.txtHex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtHex.TextChanged += new System.EventHandler(this.txtHex_TextChanged);
            // 
            // lblHex
            // 
            this.lblHex.AutoSize = true;
            this.lblHex.Location = new System.Drawing.Point(472, 177);
            this.lblHex.Name = "lblHex";
            this.lblHex.Size = new System.Drawing.Size(54, 13);
            this.lblHex.TabIndex = 26;
            this.lblHex.Text = "Hex:      #";
            // 
            // nudKey
            // 
            this.nudKey.DecimalPlaces = 1;
            this.nudKey.Location = new System.Drawing.Point(528, 141);
            this.nudKey.Name = "nudKey";
            this.nudKey.Size = new System.Drawing.Size(56, 20);
            this.nudKey.TabIndex = 23;
            this.nudKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudKey.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudKey.ValueChanged += new System.EventHandler(this.CMYK_ValueChanged);
            // 
            // nudYellow
            // 
            this.nudYellow.DecimalPlaces = 1;
            this.nudYellow.Location = new System.Drawing.Point(528, 109);
            this.nudYellow.Name = "nudYellow";
            this.nudYellow.Size = new System.Drawing.Size(56, 20);
            this.nudYellow.TabIndex = 18;
            this.nudYellow.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudYellow.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudYellow.ValueChanged += new System.EventHandler(this.CMYK_ValueChanged);
            // 
            // nudMagenta
            // 
            this.nudMagenta.DecimalPlaces = 1;
            this.nudMagenta.Location = new System.Drawing.Point(528, 77);
            this.nudMagenta.Name = "nudMagenta";
            this.nudMagenta.Size = new System.Drawing.Size(56, 20);
            this.nudMagenta.TabIndex = 11;
            this.nudMagenta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudMagenta.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudMagenta.ValueChanged += new System.EventHandler(this.CMYK_ValueChanged);
            // 
            // nudCyan
            // 
            this.nudCyan.DecimalPlaces = 1;
            this.nudCyan.Location = new System.Drawing.Point(528, 45);
            this.nudCyan.Name = "nudCyan";
            this.nudCyan.Size = new System.Drawing.Size(56, 20);
            this.nudCyan.TabIndex = 6;
            this.nudCyan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudCyan.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudCyan.ValueChanged += new System.EventHandler(this.CMYK_ValueChanged);
            // 
            // lblKey
            // 
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(472, 145);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(28, 13);
            this.lblKey.TabIndex = 22;
            this.lblKey.Text = "Key:";
            // 
            // lblYellow
            // 
            this.lblYellow.AutoSize = true;
            this.lblYellow.Location = new System.Drawing.Point(472, 113);
            this.lblYellow.Name = "lblYellow";
            this.lblYellow.Size = new System.Drawing.Size(41, 13);
            this.lblYellow.TabIndex = 17;
            this.lblYellow.Text = "Yellow:";
            // 
            // lblMagenta
            // 
            this.lblMagenta.AutoSize = true;
            this.lblMagenta.Location = new System.Drawing.Point(472, 81);
            this.lblMagenta.Name = "lblMagenta";
            this.lblMagenta.Size = new System.Drawing.Size(52, 13);
            this.lblMagenta.TabIndex = 10;
            this.lblMagenta.Text = "Magenta:";
            // 
            // lblCyan
            // 
            this.lblCyan.AutoSize = true;
            this.lblCyan.Location = new System.Drawing.Point(472, 49);
            this.lblCyan.Name = "lblCyan";
            this.lblCyan.Size = new System.Drawing.Size(34, 13);
            this.lblCyan.TabIndex = 5;
            this.lblCyan.Text = "Cyan:";
            // 
            // lblHue
            // 
            this.lblHue.AutoSize = true;
            this.lblHue.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblHue.Location = new System.Drawing.Point(444, 17);
            this.lblHue.Name = "lblHue";
            this.lblHue.Size = new System.Drawing.Size(13, 13);
            this.lblHue.TabIndex = 3;
            this.lblHue.Text = "°";
            // 
            // lblBrightnessPerc
            // 
            this.lblBrightnessPerc.AutoSize = true;
            this.lblBrightnessPerc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBrightnessPerc.Location = new System.Drawing.Point(444, 81);
            this.lblBrightnessPerc.Name = "lblBrightnessPerc";
            this.lblBrightnessPerc.Size = new System.Drawing.Size(19, 13);
            this.lblBrightnessPerc.TabIndex = 15;
            this.lblBrightnessPerc.Text = "%";
            // 
            // lblSaturationPerc
            // 
            this.lblSaturationPerc.AutoSize = true;
            this.lblSaturationPerc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSaturationPerc.Location = new System.Drawing.Point(444, 49);
            this.lblSaturationPerc.Name = "lblSaturationPerc";
            this.lblSaturationPerc.Size = new System.Drawing.Size(19, 13);
            this.lblSaturationPerc.TabIndex = 9;
            this.lblSaturationPerc.Text = "%";
            // 
            // nudBlue
            // 
            this.nudBlue.Location = new System.Drawing.Point(392, 173);
            this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.Name = "nudBlue";
            this.nudBlue.Size = new System.Drawing.Size(48, 20);
            this.nudBlue.TabIndex = 29;
            this.nudBlue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudBlue.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // nudGreen
            // 
            this.nudGreen.Location = new System.Drawing.Point(392, 141);
            this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.Name = "nudGreen";
            this.nudGreen.Size = new System.Drawing.Size(48, 20);
            this.nudGreen.TabIndex = 25;
            this.nudGreen.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudGreen.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // nudRed
            // 
            this.nudRed.Location = new System.Drawing.Point(392, 109);
            this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.Name = "nudRed";
            this.nudRed.Size = new System.Drawing.Size(48, 20);
            this.nudRed.TabIndex = 20;
            this.nudRed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudRed.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // nudBrightness
            // 
            this.nudBrightness.Location = new System.Drawing.Point(392, 77);
            this.nudBrightness.Name = "nudBrightness";
            this.nudBrightness.Size = new System.Drawing.Size(48, 20);
            this.nudBrightness.TabIndex = 14;
            this.nudBrightness.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudBrightness.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudBrightness.ValueChanged += new System.EventHandler(this.HSB_ValueChanged);
            // 
            // nudSaturation
            // 
            this.nudSaturation.Location = new System.Drawing.Point(392, 45);
            this.nudSaturation.Name = "nudSaturation";
            this.nudSaturation.Size = new System.Drawing.Size(48, 20);
            this.nudSaturation.TabIndex = 8;
            this.nudSaturation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudSaturation.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudSaturation.ValueChanged += new System.EventHandler(this.HSB_ValueChanged);
            // 
            // nudHue
            // 
            this.nudHue.Location = new System.Drawing.Point(392, 13);
            this.nudHue.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudHue.Name = "nudHue";
            this.nudHue.Size = new System.Drawing.Size(48, 20);
            this.nudHue.TabIndex = 2;
            this.nudHue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudHue.Value = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudHue.ValueChanged += new System.EventHandler(this.HSB_ValueChanged);
            // 
            // rbBlue
            // 
            this.rbBlue.Location = new System.Drawing.Point(312, 173);
            this.rbBlue.Name = "rbBlue";
            this.rbBlue.Size = new System.Drawing.Size(77, 20);
            this.rbBlue.TabIndex = 28;
            this.rbBlue.Text = "Blue:";
            this.rbBlue.UseVisualStyleBackColor = true;
            this.rbBlue.CheckedChanged += new System.EventHandler(this.rbBlue_CheckedChanged);
            // 
            // rbGreen
            // 
            this.rbGreen.Location = new System.Drawing.Point(312, 141);
            this.rbGreen.Name = "rbGreen";
            this.rbGreen.Size = new System.Drawing.Size(77, 20);
            this.rbGreen.TabIndex = 24;
            this.rbGreen.Text = "Green:";
            this.rbGreen.UseVisualStyleBackColor = true;
            this.rbGreen.CheckedChanged += new System.EventHandler(this.rbGreen_CheckedChanged);
            // 
            // rbRed
            // 
            this.rbRed.Location = new System.Drawing.Point(312, 109);
            this.rbRed.Name = "rbRed";
            this.rbRed.Size = new System.Drawing.Size(77, 20);
            this.rbRed.TabIndex = 19;
            this.rbRed.Text = "Red:";
            this.rbRed.UseVisualStyleBackColor = true;
            this.rbRed.CheckedChanged += new System.EventHandler(this.rbRed_CheckedChanged);
            // 
            // rbBrightness
            // 
            this.rbBrightness.Location = new System.Drawing.Point(312, 77);
            this.rbBrightness.Name = "rbBrightness";
            this.rbBrightness.Size = new System.Drawing.Size(77, 20);
            this.rbBrightness.TabIndex = 13;
            this.rbBrightness.Text = "Brightness:";
            this.rbBrightness.UseVisualStyleBackColor = true;
            this.rbBrightness.CheckedChanged += new System.EventHandler(this.rbBrightness_CheckedChanged);
            // 
            // rbSaturation
            // 
            this.rbSaturation.Location = new System.Drawing.Point(312, 45);
            this.rbSaturation.Name = "rbSaturation";
            this.rbSaturation.Size = new System.Drawing.Size(77, 20);
            this.rbSaturation.TabIndex = 7;
            this.rbSaturation.Text = "Saturation:";
            this.rbSaturation.UseVisualStyleBackColor = true;
            this.rbSaturation.CheckedChanged += new System.EventHandler(this.rbSaturation_CheckedChanged);
            // 
            // rbHue
            // 
            this.rbHue.Checked = true;
            this.rbHue.Location = new System.Drawing.Point(312, 13);
            this.rbHue.Name = "rbHue";
            this.rbHue.Size = new System.Drawing.Size(77, 20);
            this.rbHue.TabIndex = 1;
            this.rbHue.TabStop = true;
            this.rbHue.Text = "Hue:";
            this.rbHue.UseVisualStyleBackColor = true;
            this.rbHue.CheckedChanged += new System.EventHandler(this.rbHue_CheckedChanged);
            // 
            // lblDecimal
            // 
            this.lblDecimal.AutoSize = true;
            this.lblDecimal.Location = new System.Drawing.Point(472, 208);
            this.lblDecimal.Name = "lblDecimal";
            this.lblDecimal.Size = new System.Drawing.Size(48, 13);
            this.lblDecimal.TabIndex = 30;
            this.lblDecimal.Text = "Decimal:";
            // 
            // txtDecimal
            // 
            this.txtDecimal.Location = new System.Drawing.Point(528, 204);
            this.txtDecimal.Name = "txtDecimal";
            this.txtDecimal.Size = new System.Drawing.Size(72, 20);
            this.txtDecimal.TabIndex = 31;
            this.txtDecimal.Text = "12345678";
            this.txtDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDecimal.TextChanged += new System.EventHandler(this.txtDecimal_TextChanged);
            // 
            // lblCyanPerc
            // 
            this.lblCyanPerc.AutoSize = true;
            this.lblCyanPerc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCyanPerc.Location = new System.Drawing.Point(586, 49);
            this.lblCyanPerc.Name = "lblCyanPerc";
            this.lblCyanPerc.Size = new System.Drawing.Size(19, 13);
            this.lblCyanPerc.TabIndex = 4;
            this.lblCyanPerc.Text = "%";
            // 
            // lblMagentaPerc
            // 
            this.lblMagentaPerc.AutoSize = true;
            this.lblMagentaPerc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblMagentaPerc.Location = new System.Drawing.Point(586, 81);
            this.lblMagentaPerc.Name = "lblMagentaPerc";
            this.lblMagentaPerc.Size = new System.Drawing.Size(19, 13);
            this.lblMagentaPerc.TabIndex = 12;
            this.lblMagentaPerc.Text = "%";
            // 
            // lblYellowPerc
            // 
            this.lblYellowPerc.AutoSize = true;
            this.lblYellowPerc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblYellowPerc.Location = new System.Drawing.Point(586, 113);
            this.lblYellowPerc.Name = "lblYellowPerc";
            this.lblYellowPerc.Size = new System.Drawing.Size(19, 13);
            this.lblYellowPerc.TabIndex = 16;
            this.lblYellowPerc.Text = "%";
            // 
            // lblKeyPerc
            // 
            this.lblKeyPerc.AutoSize = true;
            this.lblKeyPerc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblKeyPerc.Location = new System.Drawing.Point(586, 145);
            this.lblKeyPerc.Name = "lblKeyPerc";
            this.lblKeyPerc.Size = new System.Drawing.Size(19, 13);
            this.lblKeyPerc.TabIndex = 21;
            this.lblKeyPerc.Text = "%";
            // 
            // nudAlpha
            // 
            this.nudAlpha.Location = new System.Drawing.Point(528, 13);
            this.nudAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudAlpha.Name = "nudAlpha";
            this.nudAlpha.Size = new System.Drawing.Size(48, 20);
            this.nudAlpha.TabIndex = 43;
            this.nudAlpha.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudAlpha.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // lblAlpha
            // 
            this.lblAlpha.AutoSize = true;
            this.lblAlpha.Location = new System.Drawing.Point(472, 17);
            this.lblAlpha.Name = "lblAlpha";
            this.lblAlpha.Size = new System.Drawing.Size(37, 13);
            this.lblAlpha.TabIndex = 44;
            this.lblAlpha.Text = "Alpha:";
            // 
            // pbColorPreview
            // 
            this.pbColorPreview.BackColor = System.Drawing.Color.White;
            this.pbColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbColorPreview.DrawCheckeredBackground = true;
            this.pbColorPreview.Location = new System.Drawing.Point(360, 208);
            this.pbColorPreview.Name = "pbColorPreview";
            this.pbColorPreview.Size = new System.Drawing.Size(80, 56);
            this.pbColorPreview.TabIndex = 49;
            this.pbColorPreview.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbColorPreview_MouseClick);
            // 
            // colorPicker
            // 
            this.colorPicker.AutoSize = true;
            this.colorPicker.DrawStyle = HelpersLib.DrawStyle.Hue;
            this.colorPicker.Location = new System.Drawing.Point(8, 8);
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.Size = new System.Drawing.Size(293, 263);
            this.colorPicker.TabIndex = 0;
            this.colorPicker.ColorChanged += new HelpersLib.ColorEventHandler(this.colorPicker_ColorChanged);
            // 
            // DialogColor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(609, 273);
            this.Controls.Add(this.pbColorPreview);
            this.Controls.Add(this.lblAlpha);
            this.Controls.Add(this.nudAlpha);
            this.Controls.Add(this.lblKeyPerc);
            this.Controls.Add(this.lblYellowPerc);
            this.Controls.Add(this.lblMagentaPerc);
            this.Controls.Add(this.lblCyanPerc);
            this.Controls.Add(this.txtDecimal);
            this.Controls.Add(this.lblDecimal);
            this.Controls.Add(this.colorPicker);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblOld);
            this.Controls.Add(this.lblNew);
            this.Controls.Add(this.txtHex);
            this.Controls.Add(this.lblHex);
            this.Controls.Add(this.nudKey);
            this.Controls.Add(this.nudYellow);
            this.Controls.Add(this.nudMagenta);
            this.Controls.Add(this.nudCyan);
            this.Controls.Add(this.lblKey);
            this.Controls.Add(this.lblYellow);
            this.Controls.Add(this.lblMagenta);
            this.Controls.Add(this.lblCyan);
            this.Controls.Add(this.lblHue);
            this.Controls.Add(this.lblBrightnessPerc);
            this.Controls.Add(this.lblSaturationPerc);
            this.Controls.Add(this.nudBlue);
            this.Controls.Add(this.nudGreen);
            this.Controls.Add(this.nudRed);
            this.Controls.Add(this.nudBrightness);
            this.Controls.Add(this.nudSaturation);
            this.Controls.Add(this.nudHue);
            this.Controls.Add(this.rbBlue);
            this.Controls.Add(this.rbGreen);
            this.Controls.Add(this.rbRed);
            this.Controls.Add(this.rbBrightness);
            this.Controls.Add(this.rbSaturation);
            this.Controls.Add(this.rbHue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DialogColor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Color picker";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudYellow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMagenta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCyan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblOld;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.Label lblHex;
        private System.Windows.Forms.NumericUpDown nudKey;
        private System.Windows.Forms.NumericUpDown nudYellow;
        private System.Windows.Forms.NumericUpDown nudMagenta;
        private System.Windows.Forms.NumericUpDown nudCyan;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.Label lblYellow;
        private System.Windows.Forms.Label lblMagenta;
        private System.Windows.Forms.Label lblCyan;
        private System.Windows.Forms.Label lblHue;
        private System.Windows.Forms.Label lblBrightnessPerc;
        private System.Windows.Forms.Label lblSaturationPerc;
        private System.Windows.Forms.NumericUpDown nudBlue;
        private System.Windows.Forms.NumericUpDown nudGreen;
        private System.Windows.Forms.NumericUpDown nudRed;
        private System.Windows.Forms.NumericUpDown nudBrightness;
        private System.Windows.Forms.NumericUpDown nudSaturation;
        private System.Windows.Forms.NumericUpDown nudHue;
        private System.Windows.Forms.RadioButton rbBlue;
        private System.Windows.Forms.RadioButton rbGreen;
        private System.Windows.Forms.RadioButton rbRed;
        private System.Windows.Forms.RadioButton rbBrightness;
        private System.Windows.Forms.RadioButton rbSaturation;
        private System.Windows.Forms.RadioButton rbHue;
        private System.Windows.Forms.Label lblDecimal;
        private System.Windows.Forms.TextBox txtDecimal;
        private System.Windows.Forms.Label lblCyanPerc;
        private System.Windows.Forms.Label lblMagentaPerc;
        private System.Windows.Forms.Label lblYellowPerc;
        private System.Windows.Forms.Label lblKeyPerc;
        private System.Windows.Forms.NumericUpDown nudAlpha;
        private System.Windows.Forms.Label lblAlpha;
        private MyPictureBox pbColorPreview;
        protected ColorPicker colorPicker;
        protected System.Windows.Forms.TextBox txtHex;
    }
}