namespace ShareX.HelpersLib
{
    partial class ColorPickerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPickerForm));
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
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblOld
            // 
            resources.ApplyResources(this.lblOld, "lblOld");
            this.lblOld.Name = "lblOld";
            // 
            // lblNew
            // 
            resources.ApplyResources(this.lblNew, "lblNew");
            this.lblNew.Name = "lblNew";
            // 
            // txtHex
            // 
            resources.ApplyResources(this.txtHex, "txtHex");
            this.txtHex.Name = "txtHex";
            this.txtHex.TextChanged += new System.EventHandler(this.txtHex_TextChanged);
            // 
            // lblHex
            // 
            resources.ApplyResources(this.lblHex, "lblHex");
            this.lblHex.Name = "lblHex";
            // 
            // nudKey
            // 
            this.nudKey.DecimalPlaces = 1;
            resources.ApplyResources(this.nudKey, "nudKey");
            this.nudKey.Name = "nudKey";
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
            resources.ApplyResources(this.nudYellow, "nudYellow");
            this.nudYellow.Name = "nudYellow";
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
            resources.ApplyResources(this.nudMagenta, "nudMagenta");
            this.nudMagenta.Name = "nudMagenta";
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
            resources.ApplyResources(this.nudCyan, "nudCyan");
            this.nudCyan.Name = "nudCyan";
            this.nudCyan.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudCyan.ValueChanged += new System.EventHandler(this.CMYK_ValueChanged);
            // 
            // lblKey
            // 
            resources.ApplyResources(this.lblKey, "lblKey");
            this.lblKey.Name = "lblKey";
            // 
            // lblYellow
            // 
            resources.ApplyResources(this.lblYellow, "lblYellow");
            this.lblYellow.Name = "lblYellow";
            // 
            // lblMagenta
            // 
            resources.ApplyResources(this.lblMagenta, "lblMagenta");
            this.lblMagenta.Name = "lblMagenta";
            // 
            // lblCyan
            // 
            resources.ApplyResources(this.lblCyan, "lblCyan");
            this.lblCyan.Name = "lblCyan";
            // 
            // lblHue
            // 
            resources.ApplyResources(this.lblHue, "lblHue");
            this.lblHue.Name = "lblHue";
            // 
            // lblBrightnessPerc
            // 
            resources.ApplyResources(this.lblBrightnessPerc, "lblBrightnessPerc");
            this.lblBrightnessPerc.Name = "lblBrightnessPerc";
            // 
            // lblSaturationPerc
            // 
            resources.ApplyResources(this.lblSaturationPerc, "lblSaturationPerc");
            this.lblSaturationPerc.Name = "lblSaturationPerc";
            // 
            // nudBlue
            // 
            resources.ApplyResources(this.nudBlue, "nudBlue");
            this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.Name = "nudBlue";
            this.nudBlue.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // nudGreen
            // 
            resources.ApplyResources(this.nudGreen, "nudGreen");
            this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.Name = "nudGreen";
            this.nudGreen.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // nudRed
            // 
            resources.ApplyResources(this.nudRed, "nudRed");
            this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.Name = "nudRed";
            this.nudRed.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // nudBrightness
            // 
            resources.ApplyResources(this.nudBrightness, "nudBrightness");
            this.nudBrightness.Name = "nudBrightness";
            this.nudBrightness.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudBrightness.ValueChanged += new System.EventHandler(this.HSB_ValueChanged);
            // 
            // nudSaturation
            // 
            resources.ApplyResources(this.nudSaturation, "nudSaturation");
            this.nudSaturation.Name = "nudSaturation";
            this.nudSaturation.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudSaturation.ValueChanged += new System.EventHandler(this.HSB_ValueChanged);
            // 
            // nudHue
            // 
            resources.ApplyResources(this.nudHue, "nudHue");
            this.nudHue.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudHue.Name = "nudHue";
            this.nudHue.Value = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudHue.ValueChanged += new System.EventHandler(this.HSB_ValueChanged);
            // 
            // rbBlue
            // 
            resources.ApplyResources(this.rbBlue, "rbBlue");
            this.rbBlue.Name = "rbBlue";
            this.rbBlue.UseVisualStyleBackColor = true;
            this.rbBlue.CheckedChanged += new System.EventHandler(this.rbBlue_CheckedChanged);
            // 
            // rbGreen
            // 
            resources.ApplyResources(this.rbGreen, "rbGreen");
            this.rbGreen.Name = "rbGreen";
            this.rbGreen.UseVisualStyleBackColor = true;
            this.rbGreen.CheckedChanged += new System.EventHandler(this.rbGreen_CheckedChanged);
            // 
            // rbRed
            // 
            resources.ApplyResources(this.rbRed, "rbRed");
            this.rbRed.Name = "rbRed";
            this.rbRed.UseVisualStyleBackColor = true;
            this.rbRed.CheckedChanged += new System.EventHandler(this.rbRed_CheckedChanged);
            // 
            // rbBrightness
            // 
            resources.ApplyResources(this.rbBrightness, "rbBrightness");
            this.rbBrightness.Name = "rbBrightness";
            this.rbBrightness.UseVisualStyleBackColor = true;
            this.rbBrightness.CheckedChanged += new System.EventHandler(this.rbBrightness_CheckedChanged);
            // 
            // rbSaturation
            // 
            resources.ApplyResources(this.rbSaturation, "rbSaturation");
            this.rbSaturation.Name = "rbSaturation";
            this.rbSaturation.UseVisualStyleBackColor = true;
            this.rbSaturation.CheckedChanged += new System.EventHandler(this.rbSaturation_CheckedChanged);
            // 
            // rbHue
            // 
            this.rbHue.Checked = true;
            resources.ApplyResources(this.rbHue, "rbHue");
            this.rbHue.Name = "rbHue";
            this.rbHue.TabStop = true;
            this.rbHue.UseVisualStyleBackColor = true;
            this.rbHue.CheckedChanged += new System.EventHandler(this.rbHue_CheckedChanged);
            // 
            // lblDecimal
            // 
            resources.ApplyResources(this.lblDecimal, "lblDecimal");
            this.lblDecimal.Name = "lblDecimal";
            // 
            // txtDecimal
            // 
            resources.ApplyResources(this.txtDecimal, "txtDecimal");
            this.txtDecimal.Name = "txtDecimal";
            this.txtDecimal.TextChanged += new System.EventHandler(this.txtDecimal_TextChanged);
            // 
            // lblCyanPerc
            // 
            resources.ApplyResources(this.lblCyanPerc, "lblCyanPerc");
            this.lblCyanPerc.Name = "lblCyanPerc";
            // 
            // lblMagentaPerc
            // 
            resources.ApplyResources(this.lblMagentaPerc, "lblMagentaPerc");
            this.lblMagentaPerc.Name = "lblMagentaPerc";
            // 
            // lblYellowPerc
            // 
            resources.ApplyResources(this.lblYellowPerc, "lblYellowPerc");
            this.lblYellowPerc.Name = "lblYellowPerc";
            // 
            // lblKeyPerc
            // 
            resources.ApplyResources(this.lblKeyPerc, "lblKeyPerc");
            this.lblKeyPerc.Name = "lblKeyPerc";
            // 
            // nudAlpha
            // 
            resources.ApplyResources(this.nudAlpha, "nudAlpha");
            this.nudAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudAlpha.Name = "nudAlpha";
            this.nudAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudAlpha.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // lblAlpha
            // 
            resources.ApplyResources(this.lblAlpha, "lblAlpha");
            this.lblAlpha.Name = "lblAlpha";
            // 
            // pbColorPreview
            // 
            this.pbColorPreview.BackColor = System.Drawing.Color.White;
            this.pbColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbColorPreview.DrawCheckeredBackground = true;
            resources.ApplyResources(this.pbColorPreview, "pbColorPreview");
            this.pbColorPreview.Name = "pbColorPreview";
            this.pbColorPreview.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbColorPreview_MouseClick);
            // 
            // colorPicker
            // 
            resources.ApplyResources(this.colorPicker, "colorPicker");
            this.colorPicker.DrawStyle = HelpersLib.DrawStyle.Hue;
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.ColorChanged += new HelpersLib.ColorEventHandler(this.colorPicker_ColorChanged);
            // 
            // ColorPickerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            resources.ApplyResources(this, "$this");
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
            this.Name = "ColorPickerForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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
        protected System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.Button btnOK;
    }
}