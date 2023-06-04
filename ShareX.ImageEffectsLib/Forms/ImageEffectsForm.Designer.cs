namespace ShareX.ImageEffectsLib
{
    partial class ImageEffectsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEffectsForm));
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.cmsEffects = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsLoadImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiLoadImageFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLoadImageFromClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.lblPresetName = new System.Windows.Forms.Label();
            this.txtPresetName = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnUploadImage = new System.Windows.Forms.Button();
            this.lblPresets = new System.Windows.Forms.Label();
            this.btnPackager = new System.Windows.Forms.Button();
            this.btnPresetNew = new System.Windows.Forms.Button();
            this.btnPresetRemove = new System.Windows.Forms.Button();
            this.btnPresetDuplicate = new System.Windows.Forms.Button();
            this.lblEffects = new System.Windows.Forms.Label();
            this.btnEffectAdd = new System.Windows.Forms.Button();
            this.btnEffectRemove = new System.Windows.Forms.Button();
            this.btnEffectDuplicate = new System.Windows.Forms.Button();
            this.btnEffectClear = new System.Windows.Forms.Button();
            this.btnEffectRefresh = new System.Windows.Forms.Button();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.pgSettings = new System.Windows.Forms.PropertyGrid();
            this.btnImageEffects = new System.Windows.Forms.Button();
            this.lblEffectName = new System.Windows.Forms.Label();
            this.txtEffectName = new System.Windows.Forms.TextBox();
            this.pbResult = new ShareX.HelpersLib.MyPictureBox();
            this.lvPresets = new ShareX.HelpersLib.MyListView();
            this.chPreset = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mbLoadImage = new ShareX.HelpersLib.MenuButton();
            this.lvEffects = new ShareX.HelpersLib.MyListView();
            this.chEffect = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsLoadImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveImage
            // 
            resources.ApplyResources(this.btnSaveImage, "btnSaveImage");
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
            // 
            // cmsEffects
            // 
            this.cmsEffects.Name = "cmsEffects";
            this.cmsEffects.ShowImageMargin = false;
            resources.ApplyResources(this.cmsEffects, "cmsEffects");
            // 
            // cmsLoadImage
            // 
            this.cmsLoadImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLoadImageFromFile,
            this.tsmiLoadImageFromClipboard});
            this.cmsLoadImage.Name = "cmsLoadImage";
            this.cmsLoadImage.ShowImageMargin = false;
            resources.ApplyResources(this.cmsLoadImage, "cmsLoadImage");
            // 
            // tsmiLoadImageFromFile
            // 
            this.tsmiLoadImageFromFile.Name = "tsmiLoadImageFromFile";
            resources.ApplyResources(this.tsmiLoadImageFromFile, "tsmiLoadImageFromFile");
            this.tsmiLoadImageFromFile.Click += new System.EventHandler(this.tsmiLoadImageFromFile_Click);
            // 
            // tsmiLoadImageFromClipboard
            // 
            this.tsmiLoadImageFromClipboard.Name = "tsmiLoadImageFromClipboard";
            resources.ApplyResources(this.tsmiLoadImageFromClipboard, "tsmiLoadImageFromClipboard");
            this.tsmiLoadImageFromClipboard.Click += new System.EventHandler(this.tsmiLoadImageFromClipboard_Click);
            // 
            // lblPresetName
            // 
            resources.ApplyResources(this.lblPresetName, "lblPresetName");
            this.lblPresetName.Name = "lblPresetName";
            // 
            // txtPresetName
            // 
            resources.ApplyResources(this.txtPresetName, "txtPresetName");
            this.txtPresetName.Name = "txtPresetName";
            this.txtPresetName.TextChanged += new System.EventHandler(this.txtPresetName_TextChanged);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnUploadImage
            // 
            resources.ApplyResources(this.btnUploadImage, "btnUploadImage");
            this.btnUploadImage.Name = "btnUploadImage";
            this.btnUploadImage.UseVisualStyleBackColor = true;
            this.btnUploadImage.Click += new System.EventHandler(this.btnUploadImage_Click);
            // 
            // lblPresets
            // 
            resources.ApplyResources(this.lblPresets, "lblPresets");
            this.lblPresets.Name = "lblPresets";
            // 
            // btnPackager
            // 
            resources.ApplyResources(this.btnPackager, "btnPackager");
            this.btnPackager.Name = "btnPackager";
            this.btnPackager.UseVisualStyleBackColor = true;
            this.btnPackager.Click += new System.EventHandler(this.btnPackager_Click);
            // 
            // btnPresetNew
            // 
            this.btnPresetNew.Image = global::ShareX.ImageEffectsLib.Properties.Resources.plus;
            resources.ApplyResources(this.btnPresetNew, "btnPresetNew");
            this.btnPresetNew.Name = "btnPresetNew";
            this.ttMain.SetToolTip(this.btnPresetNew, resources.GetString("btnPresetNew.ToolTip"));
            this.btnPresetNew.UseVisualStyleBackColor = true;
            this.btnPresetNew.Click += new System.EventHandler(this.btnPresetNew_Click);
            // 
            // btnPresetRemove
            // 
            this.btnPresetRemove.Image = global::ShareX.ImageEffectsLib.Properties.Resources.minus;
            resources.ApplyResources(this.btnPresetRemove, "btnPresetRemove");
            this.btnPresetRemove.Name = "btnPresetRemove";
            this.ttMain.SetToolTip(this.btnPresetRemove, resources.GetString("btnPresetRemove.ToolTip"));
            this.btnPresetRemove.UseVisualStyleBackColor = true;
            this.btnPresetRemove.Click += new System.EventHandler(this.btnPresetRemove_Click);
            // 
            // btnPresetDuplicate
            // 
            this.btnPresetDuplicate.Image = global::ShareX.ImageEffectsLib.Properties.Resources.document_copy;
            resources.ApplyResources(this.btnPresetDuplicate, "btnPresetDuplicate");
            this.btnPresetDuplicate.Name = "btnPresetDuplicate";
            this.ttMain.SetToolTip(this.btnPresetDuplicate, resources.GetString("btnPresetDuplicate.ToolTip"));
            this.btnPresetDuplicate.UseVisualStyleBackColor = true;
            this.btnPresetDuplicate.Click += new System.EventHandler(this.btnPresetDuplicate_Click);
            // 
            // lblEffects
            // 
            resources.ApplyResources(this.lblEffects, "lblEffects");
            this.lblEffects.Name = "lblEffects";
            // 
            // btnEffectAdd
            // 
            this.btnEffectAdd.Image = global::ShareX.ImageEffectsLib.Properties.Resources.plus;
            resources.ApplyResources(this.btnEffectAdd, "btnEffectAdd");
            this.btnEffectAdd.Name = "btnEffectAdd";
            this.ttMain.SetToolTip(this.btnEffectAdd, resources.GetString("btnEffectAdd.ToolTip"));
            this.btnEffectAdd.UseVisualStyleBackColor = true;
            this.btnEffectAdd.Click += new System.EventHandler(this.btnEffectAdd_Click);
            // 
            // btnEffectRemove
            // 
            this.btnEffectRemove.Image = global::ShareX.ImageEffectsLib.Properties.Resources.minus;
            resources.ApplyResources(this.btnEffectRemove, "btnEffectRemove");
            this.btnEffectRemove.Name = "btnEffectRemove";
            this.ttMain.SetToolTip(this.btnEffectRemove, resources.GetString("btnEffectRemove.ToolTip"));
            this.btnEffectRemove.UseVisualStyleBackColor = true;
            this.btnEffectRemove.Click += new System.EventHandler(this.btnEffectRemove_Click);
            // 
            // btnEffectDuplicate
            // 
            this.btnEffectDuplicate.Image = global::ShareX.ImageEffectsLib.Properties.Resources.document_copy;
            resources.ApplyResources(this.btnEffectDuplicate, "btnEffectDuplicate");
            this.btnEffectDuplicate.Name = "btnEffectDuplicate";
            this.ttMain.SetToolTip(this.btnEffectDuplicate, resources.GetString("btnEffectDuplicate.ToolTip"));
            this.btnEffectDuplicate.UseVisualStyleBackColor = true;
            this.btnEffectDuplicate.Click += new System.EventHandler(this.btnEffectDuplicate_Click);
            // 
            // btnEffectClear
            // 
            this.btnEffectClear.Image = global::ShareX.ImageEffectsLib.Properties.Resources.eraser;
            resources.ApplyResources(this.btnEffectClear, "btnEffectClear");
            this.btnEffectClear.Name = "btnEffectClear";
            this.ttMain.SetToolTip(this.btnEffectClear, resources.GetString("btnEffectClear.ToolTip"));
            this.btnEffectClear.UseVisualStyleBackColor = true;
            this.btnEffectClear.Click += new System.EventHandler(this.btnEffectClear_Click);
            // 
            // btnEffectRefresh
            // 
            this.btnEffectRefresh.Image = global::ShareX.ImageEffectsLib.Properties.Resources.arrow_circle_double_135;
            resources.ApplyResources(this.btnEffectRefresh, "btnEffectRefresh");
            this.btnEffectRefresh.Name = "btnEffectRefresh";
            this.ttMain.SetToolTip(this.btnEffectRefresh, resources.GetString("btnEffectRefresh.ToolTip"));
            this.btnEffectRefresh.UseVisualStyleBackColor = true;
            this.btnEffectRefresh.Click += new System.EventHandler(this.btnEffectRefresh_Click);
            // 
            // ttMain
            // 
            this.ttMain.AutoPopDelay = 5000;
            this.ttMain.InitialDelay = 200;
            this.ttMain.ReshowDelay = 100;
            // 
            // pgSettings
            // 
            resources.ApplyResources(this.pgSettings, "pgSettings");
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgSettings.ToolbarVisible = false;
            this.pgSettings.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgSettings_PropertyValueChanged);
            // 
            // btnImageEffects
            // 
            resources.ApplyResources(this.btnImageEffects, "btnImageEffects");
            this.btnImageEffects.Name = "btnImageEffects";
            this.btnImageEffects.UseVisualStyleBackColor = true;
            this.btnImageEffects.Click += new System.EventHandler(this.btnImageEffects_Click);
            // 
            // lblEffectName
            // 
            resources.ApplyResources(this.lblEffectName, "lblEffectName");
            this.lblEffectName.Name = "lblEffectName";
            // 
            // txtEffectName
            // 
            resources.ApplyResources(this.txtEffectName, "txtEffectName");
            this.txtEffectName.Name = "txtEffectName";
            this.txtEffectName.TextChanged += new System.EventHandler(this.txtEffectName_TextChanged);
            // 
            // pbResult
            // 
            resources.ApplyResources(this.pbResult, "pbResult");
            this.pbResult.BackColor = System.Drawing.SystemColors.Window;
            this.pbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbResult.DrawCheckeredBackground = true;
            this.pbResult.EnableRightClickMenu = true;
            this.pbResult.FullscreenOnClick = true;
            this.pbResult.Name = "pbResult";
            this.pbResult.PictureBoxBackColor = System.Drawing.SystemColors.Control;
            this.pbResult.ShowImageSizeLabel = true;
            this.pbResult.DragDrop += new System.Windows.Forms.DragEventHandler(this.pbResult_DragDrop);
            this.pbResult.DragEnter += new System.Windows.Forms.DragEventHandler(this.pbResult_DragEnter);
            // 
            // lvPresets
            // 
            this.lvPresets.AllowDrop = true;
            this.lvPresets.AllowItemDrag = true;
            resources.ApplyResources(this.lvPresets, "lvPresets");
            this.lvPresets.AutoFillColumn = true;
            this.lvPresets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chPreset});
            this.lvPresets.DisableDeselect = true;
            this.lvPresets.FullRowSelect = true;
            this.lvPresets.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvPresets.HideSelection = false;
            this.lvPresets.MultiSelect = false;
            this.lvPresets.Name = "lvPresets";
            this.lvPresets.UseCompatibleStateImageBehavior = false;
            this.lvPresets.View = System.Windows.Forms.View.Details;
            this.lvPresets.ItemMoving += new ShareX.HelpersLib.MyListView.ListViewItemMovedEventHandler(this.lvPresets_ItemMoving);
            this.lvPresets.SelectedIndexChanged += new System.EventHandler(this.lvPresets_SelectedIndexChanged);
            // 
            // chPreset
            // 
            resources.ApplyResources(this.chPreset, "chPreset");
            // 
            // mbLoadImage
            // 
            resources.ApplyResources(this.mbLoadImage, "mbLoadImage");
            this.mbLoadImage.Menu = this.cmsLoadImage;
            this.mbLoadImage.Name = "mbLoadImage";
            this.mbLoadImage.UseVisualStyleBackColor = true;
            // 
            // lvEffects
            // 
            this.lvEffects.AllowDrop = true;
            this.lvEffects.AllowItemDrag = true;
            resources.ApplyResources(this.lvEffects, "lvEffects");
            this.lvEffects.AutoFillColumn = true;
            this.lvEffects.CheckBoxes = true;
            this.lvEffects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chEffect});
            this.lvEffects.DisableDeselect = true;
            this.lvEffects.FullRowSelect = true;
            this.lvEffects.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvEffects.HideSelection = false;
            this.lvEffects.MultiSelect = false;
            this.lvEffects.Name = "lvEffects";
            this.lvEffects.UseCompatibleStateImageBehavior = false;
            this.lvEffects.View = System.Windows.Forms.View.Details;
            this.lvEffects.ItemMoved += new ShareX.HelpersLib.MyListView.ListViewItemMovedEventHandler(this.lvEffects_ItemMoved);
            this.lvEffects.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvEffects_ItemChecked);
            this.lvEffects.SelectedIndexChanged += new System.EventHandler(this.lvEffects_SelectedIndexChanged);
            this.lvEffects.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvEffects_KeyDown);
            // 
            // chEffect
            // 
            resources.ApplyResources(this.chEffect, "chEffect");
            // 
            // ImageEffectsForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.txtEffectName);
            this.Controls.Add(this.lblEffectName);
            this.Controls.Add(this.btnImageEffects);
            this.Controls.Add(this.pbResult);
            this.Controls.Add(this.pgSettings);
            this.Controls.Add(this.btnEffectRefresh);
            this.Controls.Add(this.btnEffectClear);
            this.Controls.Add(this.btnEffectDuplicate);
            this.Controls.Add(this.btnEffectRemove);
            this.Controls.Add(this.btnEffectAdd);
            this.Controls.Add(this.lblEffects);
            this.Controls.Add(this.lvPresets);
            this.Controls.Add(this.btnPresetDuplicate);
            this.Controls.Add(this.btnPresetRemove);
            this.Controls.Add(this.btnPresetNew);
            this.Controls.Add(this.btnPackager);
            this.Controls.Add(this.lblPresets);
            this.Controls.Add(this.btnUploadImage);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtPresetName);
            this.Controls.Add(this.lblPresetName);
            this.Controls.Add(this.mbLoadImage);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.lvEffects);
            this.Name = "ImageEffectsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.ImageEffectsForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImageEffectsForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImageEffectsForm_DragEnter);
            this.cmsLoadImage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgSettings;
        private ShareX.HelpersLib.MyListView lvEffects;
        private System.Windows.Forms.ColumnHeader chEffect;
        private ShareX.HelpersLib.MyPictureBox pbResult;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.ContextMenuStrip cmsEffects;
        private HelpersLib.MenuButton mbLoadImage;
        private System.Windows.Forms.ContextMenuStrip cmsLoadImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiLoadImageFromFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiLoadImageFromClipboard;
        private System.Windows.Forms.Label lblPresetName;
        private System.Windows.Forms.TextBox txtPresetName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnUploadImage;
        private System.Windows.Forms.Label lblPresets;
        private System.Windows.Forms.Button btnPackager;
        private System.Windows.Forms.Button btnPresetNew;
        private System.Windows.Forms.Button btnPresetRemove;
        private System.Windows.Forms.Button btnPresetDuplicate;
        private HelpersLib.MyListView lvPresets;
        private System.Windows.Forms.Label lblEffects;
        private System.Windows.Forms.Button btnEffectAdd;
        private System.Windows.Forms.Button btnEffectRemove;
        private System.Windows.Forms.Button btnEffectDuplicate;
        private System.Windows.Forms.Button btnEffectClear;
        private System.Windows.Forms.Button btnEffectRefresh;
        private System.Windows.Forms.ColumnHeader chPreset;
        private System.Windows.Forms.ToolTip ttMain;
        private System.Windows.Forms.Button btnImageEffects;
        private System.Windows.Forms.Label lblEffectName;
        private System.Windows.Forms.TextBox txtEffectName;
    }
}

