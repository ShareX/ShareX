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
            btnAdd = new System.Windows.Forms.Button();
            btnRemove = new System.Windows.Forms.Button();
            btnMoveUp = new System.Windows.Forms.Button();
            btnMoveDown = new System.Windows.Forms.Button();
            lvImages = new ShareX.HelpersLib.MyListView();
            chFilepath = new System.Windows.Forms.ColumnHeader();
            btnCombine = new System.Windows.Forms.Button();
            lblSpace = new System.Windows.Forms.Label();
            nudSpace = new System.Windows.Forms.NumericUpDown();
            lblOrientation = new System.Windows.Forms.Label();
            lblSpacePixel = new System.Windows.Forms.Label();
            lblImageAlignment = new System.Windows.Forms.Label();
            cbAlignment = new System.Windows.Forms.ComboBox();
            flpOrientation = new System.Windows.Forms.FlowLayoutPanel();
            rbOrientationHorizontal = new System.Windows.Forms.RadioButton();
            rbOrientationVertical = new System.Windows.Forms.RadioButton();
            cbAutoFillBackground = new System.Windows.Forms.CheckBox();
            lblWrapAfter = new System.Windows.Forms.Label();
            nudWrapAfter = new System.Windows.Forms.NumericUpDown();
            lblWrapAfterImages = new System.Windows.Forms.Label();
            lblImageCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)nudSpace).BeginInit();
            flpOrientation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudWrapAfter).BeginInit();
            SuspendLayout();
            // 
            // btnAdd
            // 
            resources.ApplyResources(btnAdd, "btnAdd");
            btnAdd.Name = "btnAdd";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnRemove
            // 
            resources.ApplyResources(btnRemove, "btnRemove");
            btnRemove.Name = "btnRemove";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // btnMoveUp
            // 
            resources.ApplyResources(btnMoveUp, "btnMoveUp");
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.UseVisualStyleBackColor = true;
            btnMoveUp.Click += btnMoveUp_Click;
            // 
            // btnMoveDown
            // 
            resources.ApplyResources(btnMoveDown, "btnMoveDown");
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.UseVisualStyleBackColor = true;
            btnMoveDown.Click += btnMoveDown_Click;
            // 
            // lvImages
            // 
            lvImages.AllowDrop = true;
            lvImages.AllowItemDrag = true;
            resources.ApplyResources(lvImages, "lvImages");
            lvImages.AutoFillColumn = true;
            lvImages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chFilepath });
            lvImages.FullRowSelect = true;
            lvImages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            lvImages.Name = "lvImages";
            lvImages.UseCompatibleStateImageBehavior = false;
            lvImages.View = System.Windows.Forms.View.Details;
            lvImages.DragDrop += ImageCombinerForm_DragDrop;
            lvImages.DragEnter += ImageCombinerForm_DragEnter;
            // 
            // chFilepath
            // 
            resources.ApplyResources(chFilepath, "chFilepath");
            // 
            // btnCombine
            // 
            resources.ApplyResources(btnCombine, "btnCombine");
            btnCombine.Name = "btnCombine";
            btnCombine.UseVisualStyleBackColor = true;
            btnCombine.Click += btnCombine_Click;
            // 
            // lblSpace
            // 
            resources.ApplyResources(lblSpace, "lblSpace");
            lblSpace.Name = "lblSpace";
            // 
            // nudSpace
            // 
            resources.ApplyResources(nudSpace, "nudSpace");
            nudSpace.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudSpace.Name = "nudSpace";
            nudSpace.ValueChanged += nudSpace_ValueChanged;
            // 
            // lblOrientation
            // 
            resources.ApplyResources(lblOrientation, "lblOrientation");
            lblOrientation.Name = "lblOrientation";
            // 
            // lblSpacePixel
            // 
            resources.ApplyResources(lblSpacePixel, "lblSpacePixel");
            lblSpacePixel.Name = "lblSpacePixel";
            // 
            // lblImageAlignment
            // 
            resources.ApplyResources(lblImageAlignment, "lblImageAlignment");
            lblImageAlignment.Name = "lblImageAlignment";
            // 
            // cbAlignment
            // 
            resources.ApplyResources(cbAlignment, "cbAlignment");
            cbAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAlignment.FormattingEnabled = true;
            cbAlignment.Name = "cbAlignment";
            cbAlignment.SelectedIndexChanged += cbAlignment_SelectedIndexChanged;
            // 
            // flpOrientation
            // 
            resources.ApplyResources(flpOrientation, "flpOrientation");
            flpOrientation.Controls.Add(rbOrientationHorizontal);
            flpOrientation.Controls.Add(rbOrientationVertical);
            flpOrientation.Name = "flpOrientation";
            // 
            // rbOrientationHorizontal
            // 
            resources.ApplyResources(rbOrientationHorizontal, "rbOrientationHorizontal");
            rbOrientationHorizontal.Name = "rbOrientationHorizontal";
            rbOrientationHorizontal.TabStop = true;
            rbOrientationHorizontal.UseVisualStyleBackColor = true;
            rbOrientationHorizontal.CheckedChanged += rbOrientationHorizontal_CheckedChanged;
            // 
            // rbOrientationVertical
            // 
            resources.ApplyResources(rbOrientationVertical, "rbOrientationVertical");
            rbOrientationVertical.Name = "rbOrientationVertical";
            rbOrientationVertical.TabStop = true;
            rbOrientationVertical.UseVisualStyleBackColor = true;
            rbOrientationVertical.CheckedChanged += rbOrientationVertical_CheckedChanged;
            // 
            // cbAutoFillBackground
            // 
            resources.ApplyResources(cbAutoFillBackground, "cbAutoFillBackground");
            cbAutoFillBackground.Name = "cbAutoFillBackground";
            cbAutoFillBackground.UseVisualStyleBackColor = true;
            cbAutoFillBackground.CheckedChanged += cbAutoFillBackground_CheckedChanged;
            // 
            // lblWrapAfter
            // 
            resources.ApplyResources(lblWrapAfter, "lblWrapAfter");
            lblWrapAfter.Name = "lblWrapAfter";
            // 
            // nudWrapAfter
            // 
            resources.ApplyResources(nudWrapAfter, "nudWrapAfter");
            nudWrapAfter.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudWrapAfter.Name = "nudWrapAfter";
            nudWrapAfter.ValueChanged += nudWrapAfter_ValueChanged;
            // 
            // lblWrapAfterImages
            // 
            resources.ApplyResources(lblWrapAfterImages, "lblWrapAfterImages");
            lblWrapAfterImages.Name = "lblWrapAfterImages";
            // 
            // lblImageCount
            // 
            resources.ApplyResources(lblImageCount, "lblImageCount");
            lblImageCount.Name = "lblImageCount";
            // 
            // ImageCombinerForm
            // 
            AcceptButton = btnCombine;
            AllowDrop = true;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(lblImageCount);
            Controls.Add(lblWrapAfterImages);
            Controls.Add(nudWrapAfter);
            Controls.Add(lblWrapAfter);
            Controls.Add(cbAutoFillBackground);
            Controls.Add(flpOrientation);
            Controls.Add(cbAlignment);
            Controls.Add(lblImageAlignment);
            Controls.Add(lblSpacePixel);
            Controls.Add(lblOrientation);
            Controls.Add(nudSpace);
            Controls.Add(lblSpace);
            Controls.Add(btnCombine);
            Controls.Add(lvImages);
            Controls.Add(btnMoveDown);
            Controls.Add(btnMoveUp);
            Controls.Add(btnRemove);
            Controls.Add(btnAdd);
            Name = "ImageCombinerForm";
            DragDrop += ImageCombinerForm_DragDrop;
            DragEnter += ImageCombinerForm_DragEnter;
            ((System.ComponentModel.ISupportInitialize)nudSpace).EndInit();
            flpOrientation.ResumeLayout(false);
            flpOrientation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudWrapAfter).EndInit();
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.RadioButton rbOrientationHorizontal;
        private System.Windows.Forms.RadioButton rbOrientationVertical;
        private System.Windows.Forms.CheckBox cbAutoFillBackground;
        private System.Windows.Forms.Label lblWrapAfter;
        private System.Windows.Forms.NumericUpDown nudWrapAfter;
        private System.Windows.Forms.Label lblWrapAfterImages;
        private System.Windows.Forms.Label lblImageCount;
    }
}