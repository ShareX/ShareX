namespace ShareX.HelpersLib
{
    partial class GradientPickerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GradientPickerForm));
            this.ilColors = new System.Windows.Forms.ImageList(this.components);
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.nudLocation = new System.Windows.Forms.NumericUpDown();
            this.lblLocation = new System.Windows.Forms.Label();
            this.cbGradientType = new System.Windows.Forms.ComboBox();
            this.lblGradientType = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.lblPreview = new System.Windows.Forms.Label();
            this.btnReverse = new System.Windows.Forms.Button();
            this.lblPresets = new System.Windows.Forms.Label();
            this.ilPresets = new System.Windows.Forms.ImageList(this.components);
            this.lvPresets = new System.Windows.Forms.ListView();
            this.chGradient = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClear = new System.Windows.Forms.Button();
            this.cbtnCurrentColor = new ShareX.HelpersLib.ColorButton();
            this.lvGradientPoints = new ShareX.HelpersLib.MyListView();
            this.chLocation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.nudLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // ilColors
            // 
            this.ilColors.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.ilColors, "ilColors");
            this.ilColors.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // nudLocation
            // 
            this.nudLocation.DecimalPlaces = 2;
            resources.ApplyResources(this.nudLocation, "nudLocation");
            this.nudLocation.Name = "nudLocation";
            this.nudLocation.ValueChanged += new System.EventHandler(this.nudLocation_ValueChanged);
            // 
            // lblLocation
            // 
            resources.ApplyResources(this.lblLocation, "lblLocation");
            this.lblLocation.Name = "lblLocation";
            // 
            // cbGradientType
            // 
            this.cbGradientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGradientType.FormattingEnabled = true;
            resources.ApplyResources(this.cbGradientType, "cbGradientType");
            this.cbGradientType.Name = "cbGradientType";
            this.cbGradientType.SelectedIndexChanged += new System.EventHandler(this.cbGradientType_SelectedIndexChanged);
            // 
            // lblGradientType
            // 
            resources.ApplyResources(this.lblGradientType, "lblGradientType");
            this.lblGradientType.Name = "lblGradientType";
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
            // pbPreview
            // 
            resources.ApplyResources(this.pbPreview, "pbPreview");
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.TabStop = false;
            // 
            // lblPreview
            // 
            resources.ApplyResources(this.lblPreview, "lblPreview");
            this.lblPreview.Name = "lblPreview";
            // 
            // btnReverse
            // 
            resources.ApplyResources(this.btnReverse, "btnReverse");
            this.btnReverse.Name = "btnReverse";
            this.btnReverse.UseVisualStyleBackColor = true;
            this.btnReverse.Click += new System.EventHandler(this.btnReverse_Click);
            // 
            // lblPresets
            // 
            resources.ApplyResources(this.lblPresets, "lblPresets");
            this.lblPresets.Name = "lblPresets";
            // 
            // ilPresets
            // 
            this.ilPresets.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.ilPresets, "ilPresets");
            this.ilPresets.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lvPresets
            // 
            this.lvPresets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chGradient});
            this.lvPresets.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvPresets.HideSelection = false;
            this.lvPresets.LargeImageList = this.ilPresets;
            resources.ApplyResources(this.lvPresets, "lvPresets");
            this.lvPresets.MultiSelect = false;
            this.lvPresets.Name = "lvPresets";
            this.lvPresets.TileSize = new System.Drawing.Size(70, 70);
            this.lvPresets.UseCompatibleStateImageBehavior = false;
            this.lvPresets.View = System.Windows.Forms.View.Tile;
            this.lvPresets.SelectedIndexChanged += new System.EventHandler(this.lvPresets_SelectedIndexChanged);
            // 
            // chGradient
            // 
            resources.ApplyResources(this.chGradient, "chGradient");
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // cbtnCurrentColor
            // 
            this.cbtnCurrentColor.Color = System.Drawing.Color.White;
            this.cbtnCurrentColor.ColorPickerOptions = null;
            resources.ApplyResources(this.cbtnCurrentColor, "cbtnCurrentColor");
            this.cbtnCurrentColor.Name = "cbtnCurrentColor";
            this.cbtnCurrentColor.UseVisualStyleBackColor = true;
            this.cbtnCurrentColor.ColorChanged += new ShareX.HelpersLib.ColorButton.ColorChangedEventHandler(this.cbtnCurrentColor_ColorChanged);
            // 
            // lvGradientPoints
            // 
            this.lvGradientPoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chLocation});
            this.lvGradientPoints.DisableDeselect = true;
            this.lvGradientPoints.FullRowSelect = true;
            this.lvGradientPoints.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvGradientPoints.HideSelection = false;
            resources.ApplyResources(this.lvGradientPoints, "lvGradientPoints");
            this.lvGradientPoints.MultiSelect = false;
            this.lvGradientPoints.Name = "lvGradientPoints";
            this.lvGradientPoints.SmallImageList = this.ilColors;
            this.lvGradientPoints.UseCompatibleStateImageBehavior = false;
            this.lvGradientPoints.View = System.Windows.Forms.View.Details;
            this.lvGradientPoints.SelectedIndexChanged += new System.EventHandler(this.lvGradientPoints_SelectedIndexChanged);
            this.lvGradientPoints.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvGradientPoints_MouseDoubleClick);
            // 
            // chLocation
            // 
            resources.ApplyResources(this.chLocation, "chLocation");
            // 
            // GradientPickerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lvPresets);
            this.Controls.Add(this.lblPresets);
            this.Controls.Add(this.btnReverse);
            this.Controls.Add(this.pbPreview);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblGradientType);
            this.Controls.Add(this.cbGradientType);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.nudLocation);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cbtnCurrentColor);
            this.Controls.Add(this.lvGradientPoints);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GradientPickerForm";
            this.Shown += new System.EventHandler(this.GradientPickerForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyListView lvGradientPoints;
        private System.Windows.Forms.ColumnHeader chLocation;
        private ColorButton cbtnCurrentColor;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.NumericUpDown nudLocation;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.ComboBox cbGradientType;
        private System.Windows.Forms.Label lblGradientType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.Button btnReverse;
        private System.Windows.Forms.ImageList ilColors;
        private System.Windows.Forms.Label lblPresets;
        private System.Windows.Forms.ImageList ilPresets;
        private System.Windows.Forms.ListView lvPresets;
        private System.Windows.Forms.ColumnHeader chGradient;
        private System.Windows.Forms.Button btnClear;
    }
}