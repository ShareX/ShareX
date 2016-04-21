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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
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
            this.cmsLoadImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgSettings
            // 
            resources.ApplyResources(this.pgSettings, "pgSettings");
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
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.pbResult.BackColor = System.Drawing.Color.White;
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
            // ImageEffectsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mbLoadImage);
            this.Controls.Add(this.eiImageEffects);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDuplicate);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.pbResult);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.pgSettings);
            this.Controls.Add(this.lvEffects);
            this.Name = "ImageEffectsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
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
    }
}

