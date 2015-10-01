namespace ShareX
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
            this.lvImages = new ShareX.HelpersLib.MyListView();
            this.chFilepath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCombine = new System.Windows.Forms.Button();
            this.lblSpace = new System.Windows.Forms.Label();
            this.nudSpace = new System.Windows.Forms.NumericUpDown();
            this.lblOrientation = new System.Windows.Forms.Label();
            this.cbOrientation = new System.Windows.Forms.ComboBox();
            this.lblSpacePixel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpace)).BeginInit();
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
            this.lvImages.Name = "lvImages";
            this.lvImages.UseCompatibleStateImageBehavior = false;
            this.lvImages.View = System.Windows.Forms.View.Details;
            // 
            // chFilepath
            // 
            resources.ApplyResources(this.chFilepath, "chFilepath");
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
            // cbOrientation
            // 
            resources.ApplyResources(this.cbOrientation, "cbOrientation");
            this.cbOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOrientation.FormattingEnabled = true;
            this.cbOrientation.Name = "cbOrientation";
            this.cbOrientation.SelectedIndexChanged += new System.EventHandler(this.cbOrientation_SelectedIndexChanged);
            // 
            // lblSpacePixel
            // 
            resources.ApplyResources(this.lblSpacePixel, "lblSpacePixel");
            this.lblSpacePixel.Name = "lblSpacePixel";
            // 
            // ImageCombinerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblSpacePixel);
            this.Controls.Add(this.cbOrientation);
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
            ((System.ComponentModel.ISupportInitialize)(this.nudSpace)).EndInit();
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
        private System.Windows.Forms.ComboBox cbOrientation;
        private System.Windows.Forms.ColumnHeader chFilepath;
        private System.Windows.Forms.Label lblSpacePixel;
    }
}