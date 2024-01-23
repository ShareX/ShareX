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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageBeautifierForm));
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
            resources.ApplyResources(this.lblMargin, "lblMargin");
            this.lblMargin.Name = "lblMargin";
            // 
            // tbMargin
            // 
            resources.ApplyResources(this.tbMargin, "tbMargin");
            this.tbMargin.Maximum = 300;
            this.tbMargin.Name = "tbMargin";
            this.tbMargin.TickFrequency = 10;
            this.tbMargin.Scroll += new System.EventHandler(this.tbMargin_Scroll);
            this.tbMargin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbMargin_MouseUp);
            // 
            // lblPadding
            // 
            resources.ApplyResources(this.lblPadding, "lblPadding");
            this.lblPadding.Name = "lblPadding";
            // 
            // tbPadding
            // 
            resources.ApplyResources(this.tbPadding, "tbPadding");
            this.tbPadding.Maximum = 200;
            this.tbPadding.Name = "tbPadding";
            this.tbPadding.TickFrequency = 10;
            this.tbPadding.Scroll += new System.EventHandler(this.tbPadding_Scroll);
            this.tbPadding.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbPadding_MouseUp);
            // 
            // cbSmartPadding
            // 
            resources.ApplyResources(this.cbSmartPadding, "cbSmartPadding");
            this.cbSmartPadding.Name = "cbSmartPadding";
            this.cbSmartPadding.UseVisualStyleBackColor = true;
            this.cbSmartPadding.CheckedChanged += new System.EventHandler(this.cbSmartPadding_CheckedChanged);
            // 
            // lblRoundedCorner
            // 
            resources.ApplyResources(this.lblRoundedCorner, "lblRoundedCorner");
            this.lblRoundedCorner.Name = "lblRoundedCorner";
            // 
            // tbRoundedCorner
            // 
            resources.ApplyResources(this.tbRoundedCorner, "tbRoundedCorner");
            this.tbRoundedCorner.Maximum = 50;
            this.tbRoundedCorner.Name = "tbRoundedCorner";
            this.tbRoundedCorner.TickFrequency = 5;
            this.tbRoundedCorner.Scroll += new System.EventHandler(this.tbRoundedCorner_Scroll);
            this.tbRoundedCorner.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbRoundedCorner_MouseUp);
            // 
            // lblShadowRadius
            // 
            resources.ApplyResources(this.lblShadowRadius, "lblShadowRadius");
            this.lblShadowRadius.Name = "lblShadowRadius";
            // 
            // tbShadowRadius
            // 
            resources.ApplyResources(this.tbShadowRadius, "tbShadowRadius");
            this.tbShadowRadius.Maximum = 100;
            this.tbShadowRadius.Name = "tbShadowRadius";
            this.tbShadowRadius.TickFrequency = 5;
            this.tbShadowRadius.Scroll += new System.EventHandler(this.tbShadowRadius_Scroll);
            // 
            // lblBackground
            // 
            resources.ApplyResources(this.lblBackground, "lblBackground");
            this.lblBackground.Name = "lblBackground";
            // 
            // lblMarginValue
            // 
            resources.ApplyResources(this.lblMarginValue, "lblMarginValue");
            this.lblMarginValue.Name = "lblMarginValue";
            // 
            // lblPaddingValue
            // 
            resources.ApplyResources(this.lblPaddingValue, "lblPaddingValue");
            this.lblPaddingValue.Name = "lblPaddingValue";
            // 
            // lblRoundedCornerValue
            // 
            resources.ApplyResources(this.lblRoundedCornerValue, "lblRoundedCornerValue");
            this.lblRoundedCornerValue.Name = "lblRoundedCornerValue";
            // 
            // lblShadowRadiusValue
            // 
            resources.ApplyResources(this.lblShadowRadiusValue, "lblShadowRadiusValue");
            this.lblShadowRadiusValue.Name = "lblShadowRadiusValue";
            // 
            // tlpMain
            // 
            resources.ApplyResources(this.tlpMain, "tlpMain");
            this.tlpMain.Controls.Add(this.pbPreview, 1, 0);
            this.tlpMain.Controls.Add(this.pOptions, 0, 0);
            this.tlpMain.Name = "tlpMain";
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.pbPreview, "pbPreview");
            this.pbPreview.DrawCheckeredBackground = true;
            this.pbPreview.EnableRightClickMenu = true;
            this.pbPreview.FullscreenOnClick = true;
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            this.pbPreview.ShowImageSizeLabel = true;
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
            resources.ApplyResources(this.pOptions, "pOptions");
            this.pOptions.Name = "pOptions";
            // 
            // btnShadowExpand
            // 
            this.btnShadowExpand.Image = global::ShareX.MediaLib.Properties.Resources.plus_white;
            resources.ApplyResources(this.btnShadowExpand, "btnShadowExpand");
            this.btnShadowExpand.Name = "btnShadowExpand";
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
            resources.ApplyResources(this.gbShadow, "gbShadow");
            this.gbShadow.Name = "gbShadow";
            this.gbShadow.TabStop = false;
            // 
            // lblShadowAngleValue
            // 
            resources.ApplyResources(this.lblShadowAngleValue, "lblShadowAngleValue");
            this.lblShadowAngleValue.Name = "lblShadowAngleValue";
            // 
            // lblShadowDistanceValue
            // 
            resources.ApplyResources(this.lblShadowDistanceValue, "lblShadowDistanceValue");
            this.lblShadowDistanceValue.Name = "lblShadowDistanceValue";
            // 
            // lblShadowOpacityValue
            // 
            resources.ApplyResources(this.lblShadowOpacityValue, "lblShadowOpacityValue");
            this.lblShadowOpacityValue.Name = "lblShadowOpacityValue";
            // 
            // btnShadowColor
            // 
            this.btnShadowColor.Color = System.Drawing.Color.Empty;
            this.btnShadowColor.ColorPickerOptions = null;
            resources.ApplyResources(this.btnShadowColor, "btnShadowColor");
            this.btnShadowColor.Name = "btnShadowColor";
            this.btnShadowColor.UseVisualStyleBackColor = true;
            this.btnShadowColor.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.btnShadowColor_ColorChanged);
            // 
            // tbShadowAngle
            // 
            resources.ApplyResources(this.tbShadowAngle, "tbShadowAngle");
            this.tbShadowAngle.Maximum = 360;
            this.tbShadowAngle.Name = "tbShadowAngle";
            this.tbShadowAngle.TickFrequency = 45;
            this.tbShadowAngle.Scroll += new System.EventHandler(this.tbShadowAngle_Scroll);
            // 
            // lblShadowAngle
            // 
            resources.ApplyResources(this.lblShadowAngle, "lblShadowAngle");
            this.lblShadowAngle.Name = "lblShadowAngle";
            // 
            // tbShadowDistance
            // 
            resources.ApplyResources(this.tbShadowDistance, "tbShadowDistance");
            this.tbShadowDistance.Maximum = 100;
            this.tbShadowDistance.Name = "tbShadowDistance";
            this.tbShadowDistance.TickFrequency = 10;
            this.tbShadowDistance.Scroll += new System.EventHandler(this.tbShadowDistance_Scroll);
            // 
            // lblShadowOpacity
            // 
            resources.ApplyResources(this.lblShadowOpacity, "lblShadowOpacity");
            this.lblShadowOpacity.Name = "lblShadowOpacity";
            // 
            // lblShadowDistance
            // 
            resources.ApplyResources(this.lblShadowDistance, "lblShadowDistance");
            this.lblShadowDistance.Name = "lblShadowDistance";
            // 
            // tbShadowOpacity
            // 
            resources.ApplyResources(this.tbShadowOpacity, "tbShadowOpacity");
            this.tbShadowOpacity.Maximum = 100;
            this.tbShadowOpacity.Name = "tbShadowOpacity";
            this.tbShadowOpacity.SmallChange = 10;
            this.tbShadowOpacity.TickFrequency = 10;
            this.tbShadowOpacity.Scroll += new System.EventHandler(this.tbShadowOpacity_Scroll);
            // 
            // btnResetOptions
            // 
            resources.ApplyResources(this.btnResetOptions, "btnResetOptions");
            this.btnResetOptions.Name = "btnResetOptions";
            this.btnResetOptions.UseVisualStyleBackColor = true;
            this.btnResetOptions.Click += new System.EventHandler(this.btnResetOptions_Click);
            // 
            // lblBackgroundImageFilePath
            // 
            resources.ApplyResources(this.lblBackgroundImageFilePath, "lblBackgroundImageFilePath");
            this.lblBackgroundImageFilePath.Name = "lblBackgroundImageFilePath";
            // 
            // btnBackgroundImageFilePathBrowse
            // 
            resources.ApplyResources(this.btnBackgroundImageFilePathBrowse, "btnBackgroundImageFilePathBrowse");
            this.btnBackgroundImageFilePathBrowse.Name = "btnBackgroundImageFilePathBrowse";
            this.btnBackgroundImageFilePathBrowse.UseVisualStyleBackColor = true;
            this.btnBackgroundImageFilePathBrowse.Click += new System.EventHandler(this.btnBackgroundImageFilePathBrowse_Click);
            // 
            // cbBackgroundType
            // 
            this.cbBackgroundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBackgroundType.FormattingEnabled = true;
            resources.ApplyResources(this.cbBackgroundType, "cbBackgroundType");
            this.cbBackgroundType.Name = "cbBackgroundType";
            this.cbBackgroundType.SelectedIndexChanged += new System.EventHandler(this.cbBackgroundType_SelectedIndexChanged);
            // 
            // btnPrint
            // 
            resources.ApplyResources(this.btnPrint, "btnPrint");
            this.btnPrint.Image = global::ShareX.MediaLib.Properties.Resources.printer;
            this.btnPrint.Name = "btnPrint";
            this.ttMain.SetToolTip(this.btnPrint, resources.GetString("btnPrint.ToolTip"));
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Image = global::ShareX.MediaLib.Properties.Resources.disk_black;
            this.btnSave.Name = "btnSave";
            this.ttMain.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUpload
            // 
            resources.ApplyResources(this.btnUpload, "btnUpload");
            this.btnUpload.Image = global::ShareX.MediaLib.Properties.Resources.upload_cloud;
            this.btnUpload.Name = "btnUpload";
            this.ttMain.SetToolTip(this.btnUpload, resources.GetString("btnUpload.ToolTip"));
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnSaveAs
            // 
            resources.ApplyResources(this.btnSaveAs, "btnSaveAs");
            this.btnSaveAs.Image = global::ShareX.MediaLib.Properties.Resources.disks_black;
            this.btnSaveAs.Name = "btnSaveAs";
            this.ttMain.SetToolTip(this.btnSaveAs, resources.GetString("btnSaveAs.ToolTip"));
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnCopy
            // 
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.Image = global::ShareX.MediaLib.Properties.Resources.document_copy;
            this.btnCopy.Name = "btnCopy";
            this.ttMain.SetToolTip(this.btnCopy, resources.GetString("btnCopy.ToolTip"));
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // pbBackground
            // 
            this.pbBackground.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pbBackground, "pbBackground");
            this.pbBackground.Name = "pbBackground";
            this.pbBackground.TabStop = false;
            this.pbBackground.Click += new System.EventHandler(this.pbBackground_Click);
            // 
            // ImageBeautifierForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "ImageBeautifierForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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