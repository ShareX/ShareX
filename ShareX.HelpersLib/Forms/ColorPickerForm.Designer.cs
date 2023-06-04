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
            this.components = new System.ComponentModel.Container();
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
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.btnScreenColorPicker = new System.Windows.Forms.Button();
            this.btnClipboardColorPicker = new System.Windows.Forms.Button();
            this.cbTransparent = new ShareX.HelpersLib.ColorButton();
            this.cmsCopy = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopyAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyRGB = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyHexadecimal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyCMYK = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyHSB = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyDecimal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.pCursorPosition = new System.Windows.Forms.Panel();
            this.txtY = new System.Windows.Forms.TextBox();
            this.txtX = new System.Windows.Forms.TextBox();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblCursorPosition = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.flpColorPalette = new System.Windows.Forms.FlowLayoutPanel();
            this.rbRecentColors = new System.Windows.Forms.RadioButton();
            this.rbStandardColors = new System.Windows.Forms.RadioButton();
            this.flpColorPaletteSelection = new System.Windows.Forms.FlowLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.lblNameValue = new System.Windows.Forms.Label();
            this.btnClipboardStatus = new System.Windows.Forms.Button();
            this.mbCopy = new ShareX.HelpersLib.MenuButton();
            this.pbColorPreview = new ShareX.HelpersLib.MyPictureBox();
            this.colorPicker = new ShareX.HelpersLib.ColorPicker();
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
            this.cmsCopy.SuspendLayout();
            this.pCursorPosition.SuspendLayout();
            this.flpColorPaletteSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
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
            resources.ApplyResources(this.rbHue, "rbHue");
            this.rbHue.Checked = true;
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
            // ttMain
            // 
            this.ttMain.AutoPopDelay = 5000;
            this.ttMain.InitialDelay = 100;
            this.ttMain.ReshowDelay = 100;
            // 
            // btnScreenColorPicker
            // 
            this.btnScreenColorPicker.Image = global::ShareX.HelpersLib.Properties.Resources.pipette;
            resources.ApplyResources(this.btnScreenColorPicker, "btnScreenColorPicker");
            this.btnScreenColorPicker.Name = "btnScreenColorPicker";
            this.ttMain.SetToolTip(this.btnScreenColorPicker, resources.GetString("btnScreenColorPicker.ToolTip"));
            this.btnScreenColorPicker.UseVisualStyleBackColor = true;
            this.btnScreenColorPicker.Click += new System.EventHandler(this.btnScreenColorPicker_Click);
            // 
            // btnClipboardColorPicker
            // 
            this.btnClipboardColorPicker.Image = global::ShareX.HelpersLib.Properties.Resources.clipboard_block;
            resources.ApplyResources(this.btnClipboardColorPicker, "btnClipboardColorPicker");
            this.btnClipboardColorPicker.Name = "btnClipboardColorPicker";
            this.ttMain.SetToolTip(this.btnClipboardColorPicker, resources.GetString("btnClipboardColorPicker.ToolTip"));
            this.btnClipboardColorPicker.UseVisualStyleBackColor = true;
            this.btnClipboardColorPicker.Click += new System.EventHandler(this.btnClipboardColorPicker_Click);
            // 
            // cbTransparent
            // 
            this.cbTransparent.Color = System.Drawing.Color.Transparent;
            this.cbTransparent.ColorPickerOptions = null;
            resources.ApplyResources(this.cbTransparent, "cbTransparent");
            this.cbTransparent.ManualButtonClick = true;
            this.cbTransparent.Name = "cbTransparent";
            this.ttMain.SetToolTip(this.cbTransparent, resources.GetString("cbTransparent.ToolTip"));
            this.cbTransparent.UseVisualStyleBackColor = true;
            this.cbTransparent.Click += new System.EventHandler(this.cbTransparent_Click);
            // 
            // cmsCopy
            // 
            this.cmsCopy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyAll,
            this.tsmiCopyRGB,
            this.tsmiCopyHexadecimal,
            this.tsmiCopyCMYK,
            this.tsmiCopyHSB,
            this.tsmiCopyDecimal,
            this.tsmiCopyPosition});
            this.cmsCopy.Name = "cmsCopy";
            this.cmsCopy.ShowImageMargin = false;
            resources.ApplyResources(this.cmsCopy, "cmsCopy");
            // 
            // tsmiCopyAll
            // 
            this.tsmiCopyAll.Name = "tsmiCopyAll";
            resources.ApplyResources(this.tsmiCopyAll, "tsmiCopyAll");
            this.tsmiCopyAll.Click += new System.EventHandler(this.tsmiCopyAll_Click);
            // 
            // tsmiCopyRGB
            // 
            this.tsmiCopyRGB.Name = "tsmiCopyRGB";
            resources.ApplyResources(this.tsmiCopyRGB, "tsmiCopyRGB");
            this.tsmiCopyRGB.Click += new System.EventHandler(this.tsmiCopyRGB_Click);
            // 
            // tsmiCopyHexadecimal
            // 
            this.tsmiCopyHexadecimal.Name = "tsmiCopyHexadecimal";
            resources.ApplyResources(this.tsmiCopyHexadecimal, "tsmiCopyHexadecimal");
            this.tsmiCopyHexadecimal.Click += new System.EventHandler(this.tsmiCopyHexadecimal_Click);
            // 
            // tsmiCopyCMYK
            // 
            this.tsmiCopyCMYK.Name = "tsmiCopyCMYK";
            resources.ApplyResources(this.tsmiCopyCMYK, "tsmiCopyCMYK");
            this.tsmiCopyCMYK.Click += new System.EventHandler(this.tsmiCopyCMYK_Click);
            // 
            // tsmiCopyHSB
            // 
            this.tsmiCopyHSB.Name = "tsmiCopyHSB";
            resources.ApplyResources(this.tsmiCopyHSB, "tsmiCopyHSB");
            this.tsmiCopyHSB.Click += new System.EventHandler(this.tsmiCopyHSB_Click);
            // 
            // tsmiCopyDecimal
            // 
            this.tsmiCopyDecimal.Name = "tsmiCopyDecimal";
            resources.ApplyResources(this.tsmiCopyDecimal, "tsmiCopyDecimal");
            this.tsmiCopyDecimal.Click += new System.EventHandler(this.tsmiCopyDecimal_Click);
            // 
            // tsmiCopyPosition
            // 
            this.tsmiCopyPosition.Name = "tsmiCopyPosition";
            resources.ApplyResources(this.tsmiCopyPosition, "tsmiCopyPosition");
            this.tsmiCopyPosition.Click += new System.EventHandler(this.tsmiCopyPosition_Click);
            // 
            // pCursorPosition
            // 
            this.pCursorPosition.Controls.Add(this.txtY);
            this.pCursorPosition.Controls.Add(this.txtX);
            this.pCursorPosition.Controls.Add(this.lblY);
            this.pCursorPosition.Controls.Add(this.lblX);
            this.pCursorPosition.Controls.Add(this.lblCursorPosition);
            resources.ApplyResources(this.pCursorPosition, "pCursorPosition");
            this.pCursorPosition.Name = "pCursorPosition";
            // 
            // txtY
            // 
            resources.ApplyResources(this.txtY, "txtY");
            this.txtY.Name = "txtY";
            this.txtY.ReadOnly = true;
            // 
            // txtX
            // 
            resources.ApplyResources(this.txtX, "txtX");
            this.txtX.Name = "txtX";
            this.txtX.ReadOnly = true;
            // 
            // lblY
            // 
            resources.ApplyResources(this.lblY, "lblY");
            this.lblY.Name = "lblY";
            // 
            // lblX
            // 
            resources.ApplyResources(this.lblX, "lblX");
            this.lblX.Name = "lblX";
            // 
            // lblCursorPosition
            // 
            resources.ApplyResources(this.lblCursorPosition, "lblCursorPosition");
            this.lblCursorPosition.Name = "lblCursorPosition";
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // flpColorPalette
            // 
            resources.ApplyResources(this.flpColorPalette, "flpColorPalette");
            this.flpColorPalette.Name = "flpColorPalette";
            // 
            // rbRecentColors
            // 
            resources.ApplyResources(this.rbRecentColors, "rbRecentColors");
            this.rbRecentColors.Checked = true;
            this.rbRecentColors.Name = "rbRecentColors";
            this.rbRecentColors.TabStop = true;
            this.rbRecentColors.UseVisualStyleBackColor = true;
            this.rbRecentColors.CheckedChanged += new System.EventHandler(this.rbRecentColors_CheckedChanged);
            // 
            // rbStandardColors
            // 
            resources.ApplyResources(this.rbStandardColors, "rbStandardColors");
            this.rbStandardColors.Name = "rbStandardColors";
            this.rbStandardColors.UseVisualStyleBackColor = true;
            // 
            // flpColorPaletteSelection
            // 
            resources.ApplyResources(this.flpColorPaletteSelection, "flpColorPaletteSelection");
            this.flpColorPaletteSelection.Controls.Add(this.rbRecentColors);
            this.flpColorPaletteSelection.Controls.Add(this.rbStandardColors);
            this.flpColorPaletteSelection.Name = "flpColorPaletteSelection";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // lblNameValue
            // 
            resources.ApplyResources(this.lblNameValue, "lblNameValue");
            this.lblNameValue.Name = "lblNameValue";
            // 
            // btnClipboardStatus
            // 
            resources.ApplyResources(this.btnClipboardStatus, "btnClipboardStatus");
            this.btnClipboardStatus.Image = global::ShareX.HelpersLib.Properties.Resources.tick;
            this.btnClipboardStatus.Name = "btnClipboardStatus";
            this.btnClipboardStatus.UseVisualStyleBackColor = true;
            // 
            // mbCopy
            // 
            resources.ApplyResources(this.mbCopy, "mbCopy");
            this.mbCopy.Menu = this.cmsCopy;
            this.mbCopy.Name = "mbCopy";
            this.mbCopy.UseVisualStyleBackColor = true;
            // 
            // pbColorPreview
            // 
            this.pbColorPreview.BackColor = System.Drawing.SystemColors.Window;
            this.pbColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbColorPreview.DrawCheckeredBackground = true;
            resources.ApplyResources(this.pbColorPreview, "pbColorPreview");
            this.pbColorPreview.Name = "pbColorPreview";
            this.pbColorPreview.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            this.pbColorPreview.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbColorPreview_MouseClick);
            // 
            // colorPicker
            // 
            resources.ApplyResources(this.colorPicker, "colorPicker");
            this.colorPicker.DrawStyle = ShareX.HelpersLib.DrawStyle.Hue;
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.ColorChanged += new ShareX.HelpersLib.ColorEventHandler(this.colorPicker_ColorChanged);
            // 
            // ColorPickerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnClipboardStatus);
            this.Controls.Add(this.btnClipboardColorPicker);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblNameValue);
            this.Controls.Add(this.flpColorPalette);
            this.Controls.Add(this.flpColorPaletteSelection);
            this.Controls.Add(this.btnScreenColorPicker);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pCursorPosition);
            this.Controls.Add(this.mbCopy);
            this.Controls.Add(this.cbTransparent);
            this.Controls.Add(this.nudBlue);
            this.Controls.Add(this.nudGreen);
            this.Controls.Add(this.nudRed);
            this.Controls.Add(this.rbBlue);
            this.Controls.Add(this.rbGreen);
            this.Controls.Add(this.rbRed);
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
            this.Controls.Add(this.nudBrightness);
            this.Controls.Add(this.nudSaturation);
            this.Controls.Add(this.nudHue);
            this.Controls.Add(this.rbBrightness);
            this.Controls.Add(this.rbSaturation);
            this.Controls.Add(this.rbHue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ColorPickerForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.ColorPickerForm_Shown);
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
            this.cmsCopy.ResumeLayout(false);
            this.pCursorPosition.ResumeLayout(false);
            this.pCursorPosition.PerformLayout();
            this.flpColorPaletteSelection.ResumeLayout(false);
            this.flpColorPaletteSelection.PerformLayout();
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
        private ColorButton cbTransparent;
        private System.Windows.Forms.ToolTip ttMain;
        private MenuButton mbCopy;
        private System.Windows.Forms.ContextMenuStrip cmsCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyRGB;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyHexadecimal;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyCMYK;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyHSB;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyDecimal;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyPosition;
        private System.Windows.Forms.Panel pCursorPosition;
        private System.Windows.Forms.Label lblCursorPosition;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.FlowLayoutPanel flpColorPalette;
        private System.Windows.Forms.Button btnScreenColorPicker;
        private System.Windows.Forms.RadioButton rbRecentColors;
        private System.Windows.Forms.RadioButton rbStandardColors;
        private System.Windows.Forms.FlowLayoutPanel flpColorPaletteSelection;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblNameValue;
        private System.Windows.Forms.Button btnClipboardColorPicker;
        private System.Windows.Forms.Button btnClipboardStatus;
    }
}