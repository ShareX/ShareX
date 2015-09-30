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
            this.btnAdd.Location = new System.Drawing.Point(8, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(120, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add...";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(136, 8);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(120, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(264, 8);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(120, 23);
            this.btnMoveUp.TabIndex = 2;
            this.btnMoveUp.Text = "Move up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(392, 8);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(120, 23);
            this.btnMoveDown.TabIndex = 3;
            this.btnMoveDown.Text = "Move down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // lvImages
            // 
            this.lvImages.AllowDrop = true;
            this.lvImages.AllowItemDrag = true;
            this.lvImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvImages.AutoFillColumn = true;
            this.lvImages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilepath});
            this.lvImages.FullRowSelect = true;
            this.lvImages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvImages.Location = new System.Drawing.Point(8, 40);
            this.lvImages.Name = "lvImages";
            this.lvImages.Size = new System.Drawing.Size(504, 368);
            this.lvImages.TabIndex = 4;
            this.lvImages.UseCompatibleStateImageBehavior = false;
            this.lvImages.View = System.Windows.Forms.View.Details;
            // 
            // chFilepath
            // 
            this.chFilepath.Text = "Image file path";
            this.chFilepath.Width = 487;
            // 
            // btnCombine
            // 
            this.btnCombine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCombine.Location = new System.Drawing.Point(8, 472);
            this.btnCombine.Name = "btnCombine";
            this.btnCombine.Size = new System.Drawing.Size(504, 31);
            this.btnCombine.TabIndex = 5;
            this.btnCombine.Text = "Combine images and save/upload depending on after capture settings";
            this.btnCombine.UseVisualStyleBackColor = true;
            this.btnCombine.Click += new System.EventHandler(this.btnCombine_Click);
            // 
            // lblSpace
            // 
            this.lblSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSpace.AutoSize = true;
            this.lblSpace.Location = new System.Drawing.Point(8, 448);
            this.lblSpace.Name = "lblSpace";
            this.lblSpace.Size = new System.Drawing.Size(121, 13);
            this.lblSpace.TabIndex = 6;
            this.lblSpace.Text = "Space between images:";
            // 
            // nudSpace
            // 
            this.nudSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudSpace.Location = new System.Drawing.Point(200, 444);
            this.nudSpace.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudSpace.Name = "nudSpace";
            this.nudSpace.Size = new System.Drawing.Size(64, 20);
            this.nudSpace.TabIndex = 7;
            this.nudSpace.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudSpace.ValueChanged += new System.EventHandler(this.nudSpace_ValueChanged);
            // 
            // lblOrientation
            // 
            this.lblOrientation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOrientation.AutoSize = true;
            this.lblOrientation.Location = new System.Drawing.Point(8, 424);
            this.lblOrientation.Name = "lblOrientation";
            this.lblOrientation.Size = new System.Drawing.Size(103, 13);
            this.lblOrientation.TabIndex = 8;
            this.lblOrientation.Text = "Combine orientation:";
            // 
            // cbOrientation
            // 
            this.cbOrientation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOrientation.FormattingEnabled = true;
            this.cbOrientation.Location = new System.Drawing.Point(200, 420);
            this.cbOrientation.Name = "cbOrientation";
            this.cbOrientation.Size = new System.Drawing.Size(121, 21);
            this.cbOrientation.TabIndex = 9;
            this.cbOrientation.SelectedIndexChanged += new System.EventHandler(this.cbOrientation_SelectedIndexChanged);
            // 
            // lblSpacePixel
            // 
            this.lblSpacePixel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSpacePixel.AutoSize = true;
            this.lblSpacePixel.Location = new System.Drawing.Point(272, 448);
            this.lblSpacePixel.Name = "lblSpacePixel";
            this.lblSpacePixel.Size = new System.Drawing.Size(18, 13);
            this.lblSpacePixel.TabIndex = 10;
            this.lblSpacePixel.Text = "px";
            // 
            // ImageCombinerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 514);
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Image combiner";
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