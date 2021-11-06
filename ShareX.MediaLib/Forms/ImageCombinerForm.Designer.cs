namespace ShareX.MediaLib
{
    partial class ImageCombinerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageCombinerForm));
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnCombine = new System.Windows.Forms.Button();
            this.lblSpace = new System.Windows.Forms.Label();
            this.nudSpace = new System.Windows.Forms.NumericUpDown();
            this.lblOrientation = new System.Windows.Forms.Label();
            this.lblSpacePixel = new System.Windows.Forms.Label();
            this.lblImageAlignment = new System.Windows.Forms.Label();
            this.cbAlignment = new System.Windows.Forms.ComboBox();
            this.flpOrientation = new System.Windows.Forms.FlowLayoutPanel();
            this.rbOptionHorizontal = new System.Windows.Forms.RadioButton();
            this.rbOptionVertical = new System.Windows.Forms.RadioButton();
            this.rbOptionMinimum = new System.Windows.Forms.RadioButton();
            this.cbAutoFillBackground = new System.Windows.Forms.CheckBox();
            this.lvImages = new ShareX.HelpersLib.MyListView();
            this.chFilepath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.nudSpace)).BeginInit();
            this.flpOrientation.SuspendLayout();
            this.SuspendLayout();
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
            // btnMoveUp
            // 
            resources.ApplyResources(this.btnMoveUp, "btnMoveUp");
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            resources.ApplyResources(this.btnMoveDown, "btnMoveDown");
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnCombine
            // 
            resources.ApplyResources(this.btnCombine, "btnCombine");
            this.btnCombine.Name = "btnCombine";
            this.btnCombine.UseVisualStyleBackColor = true;
            this.btnCombine.Click += new System.EventHandler(this.btnCombine_Click);
            // 
            // lblSpace
            // 
            resources.ApplyResources(this.lblSpace, "lblSpace");
            this.lblSpace.Name = "lblSpace";
            // 
            // nudSpace
            // 
            resources.ApplyResources(this.nudSpace, "nudSpace");
            this.nudSpace.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudSpace.Name = "nudSpace";
            this.nudSpace.ValueChanged += new System.EventHandler(this.nudSpace_ValueChanged);
            // 
            // lblOrientation
            // 
            resources.ApplyResources(this.lblOrientation, "lblOrientation");
            this.lblOrientation.Name = "lblOrientation";
            // 
            // lblSpacePixel
            // 
            resources.ApplyResources(this.lblSpacePixel, "lblSpacePixel");
            this.lblSpacePixel.Name = "lblSpacePixel";
            // 
            // lblImageAlignment
            // 
            resources.ApplyResources(this.lblImageAlignment, "lblImageAlignment");
            this.lblImageAlignment.Name = "lblImageAlignment";
            // 
            // cbAlignment
            // 
            resources.ApplyResources(this.cbAlignment, "cbAlignment");
            this.cbAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAlignment.FormattingEnabled = true;
            this.cbAlignment.Name = "cbAlignment";
            this.cbAlignment.SelectedIndexChanged += new System.EventHandler(this.cbAlignment_SelectedIndexChanged);
            // 
            // flpOrientation
            // 
            resources.ApplyResources(this.flpOrientation, "flpOrientation");
            this.flpOrientation.Controls.Add(this.rbOptionHorizontal);
            this.flpOrientation.Controls.Add(this.rbOptionVertical);
            this.flpOrientation.Controls.Add(this.rbOptionMinimum);
            this.flpOrientation.Name = "flpOrientation";
            // 
            // rbOptionHorizontal
            // 
            resources.ApplyResources(this.rbOptionHorizontal, "rbOptionHorizontal");
            this.rbOptionHorizontal.Name = "rbOptionHorizontal";
            this.rbOptionHorizontal.TabStop = true;
            this.rbOptionHorizontal.UseVisualStyleBackColor = true;
            this.rbOptionHorizontal.CheckedChanged += new System.EventHandler(this.rbOrientationHorizontal_CheckedChanged);
            // 
            // rbOptionVertical
            // 
            resources.ApplyResources(this.rbOptionVertical, "rbOptionVertical");
            this.rbOptionVertical.Name = "rbOptionVertical";
            this.rbOptionVertical.TabStop = true;
            this.rbOptionVertical.UseVisualStyleBackColor = true;
            this.rbOptionVertical.CheckedChanged += new System.EventHandler(this.rbOrientationVertical_CheckedChanged);
            // 
            // rbOptionMinimum
            // 
            resources.ApplyResources(this.rbOptionMinimum, "rbOptionMinimum");
            this.rbOptionMinimum.Name = "rbOptionMinimum";
            this.rbOptionMinimum.TabStop = true;
            this.rbOptionMinimum.UseVisualStyleBackColor = true;
            this.rbOptionMinimum.CheckedChanged += new System.EventHandler(this.rbOrientationMinimum_CheckedChanged);
            // 
            // cbAutoFillBackground
            // 
            resources.ApplyResources(this.cbAutoFillBackground, "cbAutoFillBackground");
            this.cbAutoFillBackground.Name = "cbAutoFillBackground";
            this.cbAutoFillBackground.UseVisualStyleBackColor = true;
            this.cbAutoFillBackground.CheckedChanged += new System.EventHandler(this.cbAutoFillBackground_CheckedChanged);
            // 
            // lvImages
            // 
            this.lvImages.AllowDrop = true;
            this.lvImages.AllowItemDrag = true;
            resources.ApplyResources(this.lvImages, "lvImages");
            this.lvImages.AutoFillColumn = true;
            this.lvImages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilepath});
            this.lvImages.FullRowSelect = true;
            this.lvImages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvImages.HideSelection = false;
            this.lvImages.Name = "lvImages";
            this.lvImages.UseCompatibleStateImageBehavior = false;
            this.lvImages.View = System.Windows.Forms.View.Details;
            this.lvImages.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImageCombinerForm_DragDrop);
            this.lvImages.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImageCombinerForm_DragEnter);
            // 
            // chFilepath
            // 
            resources.ApplyResources(this.chFilepath, "chFilepath");
            // 
            // ImageCombinerForm
            // 
            this.AcceptButton = this.btnCombine;
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.cbAutoFillBackground);
            this.Controls.Add(this.flpOrientation);
            this.Controls.Add(this.cbAlignment);
            this.Controls.Add(this.lblImageAlignment);
            this.Controls.Add(this.lblSpacePixel);
            this.Controls.Add(this.lblOrientation);
            this.Controls.Add(this.nudSpace);
            this.Controls.Add(this.lblSpace);
            this.Controls.Add(this.btnCombine);
            this.Controls.Add(this.lvImages);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnMoveUp);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Name = "ImageCombinerForm";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImageCombinerForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImageCombinerForm_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.nudSpace)).EndInit();
            this.flpOrientation.ResumeLayout(false);
            this.flpOrientation.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private HelpersLib.MyListView lvImages;
        private System.Windows.Forms.Button btnCombine;
        private System.Windows.Forms.Label lblSpace;
        private System.Windows.Forms.NumericUpDown nudSpace;
        private System.Windows.Forms.Label lblOrientation;
        private System.Windows.Forms.ColumnHeader chFilepath;
        private System.Windows.Forms.Label lblSpacePixel;
        private System.Windows.Forms.Label lblImageAlignment;
        private System.Windows.Forms.ComboBox cbAlignment;
        private System.Windows.Forms.FlowLayoutPanel flpOrientation;
        private System.Windows.Forms.RadioButton rbOptionHorizontal;
        private System.Windows.Forms.RadioButton rbOptionVertical;
        private System.Windows.Forms.CheckBox cbAutoFillBackground;
        private System.Windows.Forms.RadioButton rbOptionMinimum;
    }
}