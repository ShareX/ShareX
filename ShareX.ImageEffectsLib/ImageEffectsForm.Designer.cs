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
            this.pgSettings = new System.Windows.Forms.PropertyGrid();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lvEffects = new ShareX.HelpersLib.MyListView();
            this.chEffect = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDuplicate = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.eiImageEffects = new ShareX.HelpersLib.ExportImportControl();
            this.pbResult = new ShareX.HelpersLib.MyPictureBox();
            this.cmsEffects = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mbLoadImage = new ShareX.HelpersLib.MenuButton();
            this.cmsLoadImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiLoadImageFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLoadImageFromClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddPreset = new System.Windows.Forms.Button();
            this.btnRemovePreset = new System.Windows.Forms.Button();
            this.cbPresets = new System.Windows.Forms.ComboBox();
            this.lblPresetName = new System.Windows.Forms.Label();
            this.txtPresetName = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.cmsLoadImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgSettings
            // 
            resources.ApplyResources(this.pgSettings, "pgSettings");
            this.pgSettings.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgSettings.ToolbarVisible = false;
            this.pgSettings.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgSettings_PropertyValueChanged);
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDuplicate
            // 
            resources.ApplyResources(this.btnDuplicate, "btnDuplicate");
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.UseVisualStyleBackColor = true;
            this.btnDuplicate.Click += new System.EventHandler(this.btnDuplicate_Click);
            // 
            // btnRefresh
            // 
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSaveImage
            // 
            resources.ApplyResources(this.btnSaveImage, "btnSaveImage");
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
            // 
            // eiImageEffects
            // 
            resources.ApplyResources(this.eiImageEffects, "eiImageEffects");
            this.eiImageEffects.Name = "eiImageEffects";
            this.eiImageEffects.ObjectType = null;
            this.eiImageEffects.ExportRequested += new ShareX.HelpersLib.ExportImportControl.ExportEventHandler(this.eiImageEffects_ExportRequested);
            this.eiImageEffects.ImportRequested += new ShareX.HelpersLib.ExportImportControl.ImportEventHandler(this.eiImageEffects_ImportRequested);
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
            this.pbResult.ShowImageSizeLabel = true;
            this.pbResult.DragDrop += new System.Windows.Forms.DragEventHandler(this.pbResult_DragDrop);
            this.pbResult.DragEnter += new System.Windows.Forms.DragEventHandler(this.pbResult_DragEnter);
            // 
            // cmsEffects
            // 
            this.cmsEffects.Name = "cmsEffects";
            this.cmsEffects.ShowImageMargin = false;
            resources.ApplyResources(this.cmsEffects, "cmsEffects");
            // 
            // mbLoadImage
            // 
            resources.ApplyResources(this.mbLoadImage, "mbLoadImage");
            this.mbLoadImage.Menu = this.cmsLoadImage;
            this.mbLoadImage.Name = "mbLoadImage";
            this.mbLoadImage.UseVisualStyleBackColor = true;
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
            // btnAddPreset
            // 
            resources.ApplyResources(this.btnAddPreset, "btnAddPreset");
            this.btnAddPreset.Name = "btnAddPreset";
            this.btnAddPreset.UseVisualStyleBackColor = true;
            this.btnAddPreset.Click += new System.EventHandler(this.btnAddPreset_Click);
            // 
            // btnRemovePreset
            // 
            resources.ApplyResources(this.btnRemovePreset, "btnRemovePreset");
            this.btnRemovePreset.Name = "btnRemovePreset";
            this.btnRemovePreset.UseVisualStyleBackColor = true;
            this.btnRemovePreset.Click += new System.EventHandler(this.btnRemovePreset_Click);
            // 
            // cbPresets
            // 
            this.cbPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPresets.FormattingEnabled = true;
            resources.ApplyResources(this.cbPresets, "cbPresets");
            this.cbPresets.Name = "cbPresets";
            this.cbPresets.SelectedIndexChanged += new System.EventHandler(this.cbPresets_SelectedIndexChanged);
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
            // ImageEffectsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtPresetName);
            this.Controls.Add(this.lblPresetName);
            this.Controls.Add(this.cbPresets);
            this.Controls.Add(this.btnRemovePreset);
            this.Controls.Add(this.btnAddPreset);
            this.Controls.Add(this.mbLoadImage);
            this.Controls.Add(this.eiImageEffects);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDuplicate);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.pbResult);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.pgSettings);
            this.Controls.Add(this.lvEffects);
            this.Name = "ImageEffectsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.ImageEffectsForm_Shown);
            this.cmsLoadImage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgSettings;
        private System.Windows.Forms.Button btnAdd;
        private ShareX.HelpersLib.MyListView lvEffects;
        private System.Windows.Forms.ColumnHeader chEffect;
        private System.Windows.Forms.Button btnRemove;
        private ShareX.HelpersLib.MyPictureBox pbResult;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnDuplicate;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSaveImage;
        private ShareX.HelpersLib.ExportImportControl eiImageEffects;
        private System.Windows.Forms.ContextMenuStrip cmsEffects;
        private HelpersLib.MenuButton mbLoadImage;
        private System.Windows.Forms.ContextMenuStrip cmsLoadImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiLoadImageFromFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiLoadImageFromClipboard;
        private System.Windows.Forms.Button btnAddPreset;
        private System.Windows.Forms.Button btnRemovePreset;
        private System.Windows.Forms.ComboBox cbPresets;
        private System.Windows.Forms.Label lblPresetName;
        private System.Windows.Forms.TextBox txtPresetName;
        private System.Windows.Forms.Button btnClose;
    }
}

