namespace ShareX.MediaLib
{
    partial class ImageBeautifierForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblMargin = new System.Windows.Forms.Label();
            this.tbMargin = new System.Windows.Forms.TrackBar();
            this.lblPadding = new System.Windows.Forms.Label();
            this.tbPadding = new System.Windows.Forms.TrackBar();
            this.cbSmartPadding = new System.Windows.Forms.CheckBox();
            this.lblRoundedCorner = new System.Windows.Forms.Label();
            this.tbRoundedCorner = new System.Windows.Forms.TrackBar();
            this.lblShadowRadius = new System.Windows.Forms.Label();
            this.tbShadowRadius = new System.Windows.Forms.TrackBar();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblMarginValue = new System.Windows.Forms.Label();
            this.lblPaddingValue = new System.Windows.Forms.Label();
            this.lblRoundedCornerValue = new System.Windows.Forms.Label();
            this.lblShadowRadiusValue = new System.Windows.Forms.Label();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pbPreview = new ShareX.HelpersLib.MyPictureBox();
            this.pOptions = new System.Windows.Forms.Panel();
            this.btnShadowExpand = new System.Windows.Forms.Button();
            this.gbShadow = new System.Windows.Forms.GroupBox();
            this.lblShadowAngleValue = new System.Windows.Forms.Label();
            this.lblShadowDistanceValue = new System.Windows.Forms.Label();
            this.lblShadowOpacityValue = new System.Windows.Forms.Label();
            this.btnShadowColor = new ShareX.HelpersLib.ColorButton();
            this.tbShadowAngle = new System.Windows.Forms.TrackBar();
            this.lblShadowAngle = new System.Windows.Forms.Label();
            this.tbShadowDistance = new System.Windows.Forms.TrackBar();
            this.lblShadowOpacity = new System.Windows.Forms.Label();
            this.lblShadowDistance = new System.Windows.Forms.Label();
            this.tbShadowOpacity = new System.Windows.Forms.TrackBar();
            this.btnResetOptions = new System.Windows.Forms.Button();
            this.lblBackgroundImageFilePath = new System.Windows.Forms.Label();
            this.btnBackgroundImageFilePathBrowse = new System.Windows.Forms.Button();
            this.cbBackgroundType = new System.Windows.Forms.ComboBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.pbBackground = new System.Windows.Forms.PictureBox();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tbMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPadding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRoundedCorner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowRadius)).BeginInit();
            this.tlpMain.SuspendLayout();
            this.pOptions.SuspendLayout();
            this.gbShadow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowOpacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMargin
            // 
            this.lblMargin.AutoSize = true;
            this.lblMargin.Location = new System.Drawing.Point(13, 16);
            this.lblMargin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMargin.Name = "lblMargin";
            this.lblMargin.Size = new System.Drawing.Size(53, 17);
            this.lblMargin.TabIndex = 0;
            this.lblMargin.Text = "Margin:";
            // 
            // tbMargin
            // 
            this.tbMargin.Location = new System.Drawing.Point(16, 40);
            this.tbMargin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbMargin.Maximum = 300;
            this.tbMargin.Name = "tbMargin";
            this.tbMargin.Size = new System.Drawing.Size(296, 45);
            this.tbMargin.TabIndex = 1;
            this.tbMargin.TickFrequency = 10;
            this.tbMargin.Scroll += new System.EventHandler(this.tbMargin_Scroll);
            this.tbMargin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbMargin_MouseUp);
            // 
            // lblPadding
            // 
            this.lblPadding.AutoSize = true;
            this.lblPadding.Location = new System.Drawing.Point(13, 88);
            this.lblPadding.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPadding.Name = "lblPadding";
            this.lblPadding.Size = new System.Drawing.Size(59, 17);
            this.lblPadding.TabIndex = 3;
            this.lblPadding.Text = "Padding:";
            // 
            // tbPadding
            // 
            this.tbPadding.Location = new System.Drawing.Point(16, 112);
            this.tbPadding.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbPadding.Maximum = 200;
            this.tbPadding.Name = "tbPadding";
            this.tbPadding.Size = new System.Drawing.Size(296, 45);
            this.tbPadding.TabIndex = 4;
            this.tbPadding.TickFrequency = 10;
            this.tbPadding.Scroll += new System.EventHandler(this.tbPadding_Scroll);
            this.tbPadding.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbPadding_MouseUp);
            // 
            // cbSmartPadding
            // 
            this.cbSmartPadding.AutoSize = true;
            this.cbSmartPadding.Location = new System.Drawing.Point(16, 160);
            this.cbSmartPadding.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbSmartPadding.Name = "cbSmartPadding";
            this.cbSmartPadding.Size = new System.Drawing.Size(114, 21);
            this.cbSmartPadding.TabIndex = 6;
            this.cbSmartPadding.Text = "Smart padding";
            this.cbSmartPadding.UseVisualStyleBackColor = true;
            this.cbSmartPadding.CheckedChanged += new System.EventHandler(this.cbSmartPadding_CheckedChanged);
            // 
            // lblRoundedCorner
            // 
            this.lblRoundedCorner.AutoSize = true;
            this.lblRoundedCorner.Location = new System.Drawing.Point(13, 192);
            this.lblRoundedCorner.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRoundedCorner.Name = "lblRoundedCorner";
            this.lblRoundedCorner.Size = new System.Drawing.Size(106, 17);
            this.lblRoundedCorner.TabIndex = 7;
            this.lblRoundedCorner.Text = "Rounded corner:";
            // 
            // tbRoundedCorner
            // 
            this.tbRoundedCorner.Location = new System.Drawing.Point(16, 216);
            this.tbRoundedCorner.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbRoundedCorner.Maximum = 50;
            this.tbRoundedCorner.Name = "tbRoundedCorner";
            this.tbRoundedCorner.Size = new System.Drawing.Size(296, 45);
            this.tbRoundedCorner.TabIndex = 8;
            this.tbRoundedCorner.TickFrequency = 5;
            this.tbRoundedCorner.Scroll += new System.EventHandler(this.tbRoundedCorner_Scroll);
            this.tbRoundedCorner.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbRoundedCorner_MouseUp);
            // 
            // lblShadowRadius
            // 
            this.lblShadowRadius.AutoSize = true;
            this.lblShadowRadius.Location = new System.Drawing.Point(16, 32);
            this.lblShadowRadius.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShadowRadius.Name = "lblShadowRadius";
            this.lblShadowRadius.Size = new System.Drawing.Size(50, 17);
            this.lblShadowRadius.TabIndex = 10;
            this.lblShadowRadius.Text = "Radius:";
            // 
            // tbShadowRadius
            // 
            this.tbShadowRadius.AutoSize = false;
            this.tbShadowRadius.Location = new System.Drawing.Point(16, 56);
            this.tbShadowRadius.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbShadowRadius.Maximum = 100;
            this.tbShadowRadius.Name = "tbShadowRadius";
            this.tbShadowRadius.Size = new System.Drawing.Size(280, 32);
            this.tbShadowRadius.TabIndex = 11;
            this.tbShadowRadius.TickFrequency = 5;
            this.tbShadowRadius.Scroll += new System.EventHandler(this.tbShadowRadius_Scroll);
            // 
            // lblBackground
            // 
            this.lblBackground.AutoSize = true;
            this.lblBackground.Location = new System.Drawing.Point(13, 384);
            this.lblBackground.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(80, 17);
            this.lblBackground.TabIndex = 13;
            this.lblBackground.Text = "Background:";
            // 
            // lblMarginValue
            // 
            this.lblMarginValue.Location = new System.Drawing.Point(256, 16);
            this.lblMarginValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMarginValue.Name = "lblMarginValue";
            this.lblMarginValue.Size = new System.Drawing.Size(56, 24);
            this.lblMarginValue.TabIndex = 2;
            this.lblMarginValue.Text = "0";
            this.lblMarginValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPaddingValue
            // 
            this.lblPaddingValue.Location = new System.Drawing.Point(256, 88);
            this.lblPaddingValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPaddingValue.Name = "lblPaddingValue";
            this.lblPaddingValue.Size = new System.Drawing.Size(56, 24);
            this.lblPaddingValue.TabIndex = 5;
            this.lblPaddingValue.Text = "0";
            this.lblPaddingValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRoundedCornerValue
            // 
            this.lblRoundedCornerValue.Location = new System.Drawing.Point(256, 192);
            this.lblRoundedCornerValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRoundedCornerValue.Name = "lblRoundedCornerValue";
            this.lblRoundedCornerValue.Size = new System.Drawing.Size(56, 24);
            this.lblRoundedCornerValue.TabIndex = 9;
            this.lblRoundedCornerValue.Text = "0";
            this.lblRoundedCornerValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblShadowRadiusValue
            // 
            this.lblShadowRadiusValue.Location = new System.Drawing.Point(240, 32);
            this.lblShadowRadiusValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShadowRadiusValue.Name = "lblShadowRadiusValue";
            this.lblShadowRadiusValue.Size = new System.Drawing.Size(56, 24);
            this.lblShadowRadiusValue.TabIndex = 12;
            this.lblShadowRadiusValue.Text = "0";
            this.lblShadowRadiusValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 335F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.pbPreview, 1, 0);
            this.tlpMain.Controls.Add(this.pOptions, 0, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(1384, 761);
            this.tlpMain.TabIndex = 0;
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.SystemColors.Window;
            this.pbPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPreview.DrawCheckeredBackground = true;
            this.pbPreview.EnableRightClickMenu = true;
            this.pbPreview.FullscreenOnClick = true;
            this.pbPreview.Location = new System.Drawing.Point(335, 0);
            this.pbPreview.Margin = new System.Windows.Forms.Padding(0);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            this.pbPreview.ShowImageSizeLabel = true;
            this.pbPreview.Size = new System.Drawing.Size(1049, 761);
            this.pbPreview.TabIndex = 1;
            // 
            // pOptions
            // 
            this.pOptions.Controls.Add(this.btnShadowExpand);
            this.pOptions.Controls.Add(this.gbShadow);
            this.pOptions.Controls.Add(this.btnResetOptions);
            this.pOptions.Controls.Add(this.lblBackgroundImageFilePath);
            this.pOptions.Controls.Add(this.btnBackgroundImageFilePathBrowse);
            this.pOptions.Controls.Add(this.cbBackgroundType);
            this.pOptions.Controls.Add(this.btnPrint);
            this.pOptions.Controls.Add(this.btnSave);
            this.pOptions.Controls.Add(this.btnUpload);
            this.pOptions.Controls.Add(this.btnSaveAs);
            this.pOptions.Controls.Add(this.btnCopy);
            this.pOptions.Controls.Add(this.pbBackground);
            this.pOptions.Controls.Add(this.lblMargin);
            this.pOptions.Controls.Add(this.tbMargin);
            this.pOptions.Controls.Add(this.lblRoundedCornerValue);
            this.pOptions.Controls.Add(this.lblPadding);
            this.pOptions.Controls.Add(this.lblPaddingValue);
            this.pOptions.Controls.Add(this.tbPadding);
            this.pOptions.Controls.Add(this.lblMarginValue);
            this.pOptions.Controls.Add(this.cbSmartPadding);
            this.pOptions.Controls.Add(this.lblRoundedCorner);
            this.pOptions.Controls.Add(this.lblBackground);
            this.pOptions.Controls.Add(this.tbRoundedCorner);
            this.pOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pOptions.Location = new System.Drawing.Point(3, 3);
            this.pOptions.Name = "pOptions";
            this.pOptions.Size = new System.Drawing.Size(329, 755);
            this.pOptions.TabIndex = 0;
            // 
            // btnShadowExpand
            // 
            this.btnShadowExpand.Image = global::ShareX.MediaLib.Properties.Resources.plus_white;
            this.btnShadowExpand.Location = new System.Drawing.Point(280, 260);
            this.btnShadowExpand.Name = "btnShadowExpand";
            this.btnShadowExpand.Size = new System.Drawing.Size(30, 30);
            this.btnShadowExpand.TabIndex = 16;
            this.btnShadowExpand.Tag = "+";
            this.btnShadowExpand.UseVisualStyleBackColor = true;
            this.btnShadowExpand.Click += new System.EventHandler(this.btnShadowExpand_Click);
            // 
            // gbShadow
            // 
            this.gbShadow.Controls.Add(this.lblShadowAngleValue);
            this.gbShadow.Controls.Add(this.lblShadowDistanceValue);
            this.gbShadow.Controls.Add(this.lblShadowOpacityValue);
            this.gbShadow.Controls.Add(this.btnShadowColor);
            this.gbShadow.Controls.Add(this.tbShadowRadius);
            this.gbShadow.Controls.Add(this.tbShadowAngle);
            this.gbShadow.Controls.Add(this.lblShadowRadiusValue);
            this.gbShadow.Controls.Add(this.lblShadowAngle);
            this.gbShadow.Controls.Add(this.lblShadowRadius);
            this.gbShadow.Controls.Add(this.tbShadowDistance);
            this.gbShadow.Controls.Add(this.lblShadowOpacity);
            this.gbShadow.Controls.Add(this.lblShadowDistance);
            this.gbShadow.Controls.Add(this.tbShadowOpacity);
            this.gbShadow.Location = new System.Drawing.Point(8, 264);
            this.gbShadow.Name = "gbShadow";
            this.gbShadow.Size = new System.Drawing.Size(312, 104);
            this.gbShadow.TabIndex = 25;
            this.gbShadow.TabStop = false;
            this.gbShadow.Text = "Shadow";
            // 
            // lblShadowAngleValue
            // 
            this.lblShadowAngleValue.Location = new System.Drawing.Point(240, 248);
            this.lblShadowAngleValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShadowAngleValue.Name = "lblShadowAngleValue";
            this.lblShadowAngleValue.Size = new System.Drawing.Size(56, 24);
            this.lblShadowAngleValue.TabIndex = 15;
            this.lblShadowAngleValue.Text = "0";
            this.lblShadowAngleValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblShadowDistanceValue
            // 
            this.lblShadowDistanceValue.Location = new System.Drawing.Point(240, 176);
            this.lblShadowDistanceValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShadowDistanceValue.Name = "lblShadowDistanceValue";
            this.lblShadowDistanceValue.Size = new System.Drawing.Size(56, 24);
            this.lblShadowDistanceValue.TabIndex = 14;
            this.lblShadowDistanceValue.Text = "0";
            this.lblShadowDistanceValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblShadowOpacityValue
            // 
            this.lblShadowOpacityValue.Location = new System.Drawing.Point(240, 104);
            this.lblShadowOpacityValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShadowOpacityValue.Name = "lblShadowOpacityValue";
            this.lblShadowOpacityValue.Size = new System.Drawing.Size(56, 24);
            this.lblShadowOpacityValue.TabIndex = 13;
            this.lblShadowOpacityValue.Text = "0";
            this.lblShadowOpacityValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnShadowColor
            // 
            this.btnShadowColor.Color = System.Drawing.Color.Empty;
            this.btnShadowColor.ColorPickerOptions = null;
            this.btnShadowColor.Location = new System.Drawing.Point(16, 320);
            this.btnShadowColor.Name = "btnShadowColor";
            this.btnShadowColor.Size = new System.Drawing.Size(280, 32);
            this.btnShadowColor.TabIndex = 6;
            this.btnShadowColor.Text = "Color...";
            this.btnShadowColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnShadowColor.UseVisualStyleBackColor = true;
            this.btnShadowColor.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.btnShadowColor_ColorChanged);
            // 
            // tbShadowAngle
            // 
            this.tbShadowAngle.Location = new System.Drawing.Point(16, 272);
            this.tbShadowAngle.Maximum = 360;
            this.tbShadowAngle.Name = "tbShadowAngle";
            this.tbShadowAngle.Size = new System.Drawing.Size(280, 45);
            this.tbShadowAngle.TabIndex = 5;
            this.tbShadowAngle.TickFrequency = 45;
            this.tbShadowAngle.Scroll += new System.EventHandler(this.tbShadowAngle_Scroll);
            // 
            // lblShadowAngle
            // 
            this.lblShadowAngle.AutoSize = true;
            this.lblShadowAngle.Location = new System.Drawing.Point(16, 248);
            this.lblShadowAngle.Name = "lblShadowAngle";
            this.lblShadowAngle.Size = new System.Drawing.Size(44, 17);
            this.lblShadowAngle.TabIndex = 4;
            this.lblShadowAngle.Text = "Angle:";
            // 
            // tbShadowDistance
            // 
            this.tbShadowDistance.Location = new System.Drawing.Point(16, 200);
            this.tbShadowDistance.Maximum = 100;
            this.tbShadowDistance.Name = "tbShadowDistance";
            this.tbShadowDistance.Size = new System.Drawing.Size(280, 45);
            this.tbShadowDistance.TabIndex = 3;
            this.tbShadowDistance.TickFrequency = 10;
            this.tbShadowDistance.Scroll += new System.EventHandler(this.tbShadowDistance_Scroll);
            // 
            // lblShadowOpacity
            // 
            this.lblShadowOpacity.AutoSize = true;
            this.lblShadowOpacity.Location = new System.Drawing.Point(16, 104);
            this.lblShadowOpacity.Name = "lblShadowOpacity";
            this.lblShadowOpacity.Size = new System.Drawing.Size(55, 17);
            this.lblShadowOpacity.TabIndex = 0;
            this.lblShadowOpacity.Text = "Opacity:";
            // 
            // lblShadowDistance
            // 
            this.lblShadowDistance.AutoSize = true;
            this.lblShadowDistance.Location = new System.Drawing.Point(16, 176);
            this.lblShadowDistance.Name = "lblShadowDistance";
            this.lblShadowDistance.Size = new System.Drawing.Size(60, 17);
            this.lblShadowDistance.TabIndex = 2;
            this.lblShadowDistance.Text = "Distance:";
            // 
            // tbShadowOpacity
            // 
            this.tbShadowOpacity.Location = new System.Drawing.Point(16, 128);
            this.tbShadowOpacity.Maximum = 100;
            this.tbShadowOpacity.Name = "tbShadowOpacity";
            this.tbShadowOpacity.Size = new System.Drawing.Size(280, 45);
            this.tbShadowOpacity.SmallChange = 10;
            this.tbShadowOpacity.TabIndex = 1;
            this.tbShadowOpacity.TickFrequency = 10;
            this.tbShadowOpacity.Scroll += new System.EventHandler(this.tbShadowOpacity_Scroll);
            // 
            // btnResetOptions
            // 
            this.btnResetOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnResetOptions.Location = new System.Drawing.Point(8, 656);
            this.btnResetOptions.Name = "btnResetOptions";
            this.btnResetOptions.Size = new System.Drawing.Size(312, 32);
            this.btnResetOptions.TabIndex = 23;
            this.btnResetOptions.Text = "Reset options...";
            this.btnResetOptions.UseVisualStyleBackColor = true;
            this.btnResetOptions.Click += new System.EventHandler(this.btnResetOptions_Click);
            // 
            // lblBackgroundImageFilePath
            // 
            this.lblBackgroundImageFilePath.Location = new System.Drawing.Point(13, 480);
            this.lblBackgroundImageFilePath.Name = "lblBackgroundImageFilePath";
            this.lblBackgroundImageFilePath.Size = new System.Drawing.Size(296, 120);
            this.lblBackgroundImageFilePath.TabIndex = 22;
            // 
            // btnBackgroundImageFilePathBrowse
            // 
            this.btnBackgroundImageFilePathBrowse.Location = new System.Drawing.Point(16, 440);
            this.btnBackgroundImageFilePathBrowse.Name = "btnBackgroundImageFilePathBrowse";
            this.btnBackgroundImageFilePathBrowse.Size = new System.Drawing.Size(296, 32);
            this.btnBackgroundImageFilePathBrowse.TabIndex = 21;
            this.btnBackgroundImageFilePathBrowse.Text = "Browse image file...";
            this.btnBackgroundImageFilePathBrowse.UseVisualStyleBackColor = true;
            this.btnBackgroundImageFilePathBrowse.Click += new System.EventHandler(this.btnBackgroundImageFilePathBrowse_Click);
            // 
            // cbBackgroundType
            // 
            this.cbBackgroundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBackgroundType.FormattingEnabled = true;
            this.cbBackgroundType.Location = new System.Drawing.Point(16, 408);
            this.cbBackgroundType.Name = "cbBackgroundType";
            this.cbBackgroundType.Size = new System.Drawing.Size(296, 25);
            this.cbBackgroundType.TabIndex = 19;
            this.cbBackgroundType.SelectedIndexChanged += new System.EventHandler(this.cbBackgroundType_SelectedIndexChanged);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.Image = global::ShareX.MediaLib.Properties.Resources.printer;
            this.btnPrint.Location = new System.Drawing.Point(264, 696);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(56, 48);
            this.btnPrint.TabIndex = 18;
            this.ttMain.SetToolTip(this.btnPrint, "Print...");
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Image = global::ShareX.MediaLib.Properties.Resources.disk_black;
            this.btnSave.Location = new System.Drawing.Point(72, 696);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(56, 48);
            this.btnSave.TabIndex = 15;
            this.ttMain.SetToolTip(this.btnSave, "Save");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpload.Image = global::ShareX.MediaLib.Properties.Resources.upload_cloud;
            this.btnUpload.Location = new System.Drawing.Point(200, 696);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(56, 48);
            this.btnUpload.TabIndex = 17;
            this.ttMain.SetToolTip(this.btnUpload, "Upload");
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveAs.Image = global::ShareX.MediaLib.Properties.Resources.disks_black;
            this.btnSaveAs.Location = new System.Drawing.Point(136, 696);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(56, 48);
            this.btnSaveAs.TabIndex = 16;
            this.ttMain.SetToolTip(this.btnSaveAs, "Save as...");
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Image = global::ShareX.MediaLib.Properties.Resources.document_copy;
            this.btnCopy.Location = new System.Drawing.Point(8, 696);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(56, 48);
            this.btnCopy.TabIndex = 14;
            this.ttMain.SetToolTip(this.btnCopy, "Copy");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // pbBackground
            // 
            this.pbBackground.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbBackground.Location = new System.Drawing.Point(16, 440);
            this.pbBackground.Name = "pbBackground";
            this.pbBackground.Size = new System.Drawing.Size(296, 40);
            this.pbBackground.TabIndex = 14;
            this.pbBackground.TabStop = false;
            this.pbBackground.Click += new System.EventHandler(this.pbBackground_Click);
            // 
            // ImageBeautifierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 761);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Name = "ImageBeautifierForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Image beautifier";
            this.Shown += new System.EventHandler(this.ImageBeautifierForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.tbMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPadding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRoundedCorner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowRadius)).EndInit();
            this.tlpMain.ResumeLayout(false);
            this.pOptions.ResumeLayout(false);
            this.pOptions.PerformLayout();
            this.gbShadow.ResumeLayout(false);
            this.gbShadow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbShadowOpacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackground)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblMargin;
        private System.Windows.Forms.TrackBar tbMargin;
        private System.Windows.Forms.Label lblPadding;
        private System.Windows.Forms.TrackBar tbPadding;
        private System.Windows.Forms.CheckBox cbSmartPadding;
        private System.Windows.Forms.Label lblRoundedCorner;
        private System.Windows.Forms.TrackBar tbRoundedCorner;
        private System.Windows.Forms.Label lblShadowRadius;
        private System.Windows.Forms.TrackBar tbShadowRadius;
        private System.Windows.Forms.Label lblBackground;
        private System.Windows.Forms.Label lblMarginValue;
        private System.Windows.Forms.Label lblPaddingValue;
        private System.Windows.Forms.Label lblRoundedCornerValue;
        private System.Windows.Forms.Label lblShadowRadiusValue;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pOptions;
        private HelpersLib.MyPictureBox pbPreview;
        private System.Windows.Forms.PictureBox pbBackground;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.ToolTip ttMain;
        private System.Windows.Forms.ComboBox cbBackgroundType;
        private System.Windows.Forms.Button btnBackgroundImageFilePathBrowse;
        private System.Windows.Forms.Label lblBackgroundImageFilePath;
        private System.Windows.Forms.Button btnResetOptions;
        private System.Windows.Forms.TrackBar tbShadowOpacity;
        private System.Windows.Forms.Label lblShadowOpacity;
        private HelpersLib.ColorButton btnShadowColor;
        private System.Windows.Forms.TrackBar tbShadowAngle;
        private System.Windows.Forms.Label lblShadowAngle;
        private System.Windows.Forms.TrackBar tbShadowDistance;
        private System.Windows.Forms.Label lblShadowDistance;
        private System.Windows.Forms.GroupBox gbShadow;
        private System.Windows.Forms.Label lblShadowAngleValue;
        private System.Windows.Forms.Label lblShadowDistanceValue;
        private System.Windows.Forms.Label lblShadowOpacityValue;
        private System.Windows.Forms.Button btnShadowExpand;
    }
}