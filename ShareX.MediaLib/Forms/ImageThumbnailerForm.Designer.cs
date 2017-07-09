namespace ShareX.MediaLib
{
    partial class ImageThumbnailerForm
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
            this.lvImages = new ShareX.HelpersLib.MyListView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lblWidth = new System.Windows.Forms.Label();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.lblHeight = new System.Windows.Forms.Label();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.cbAllowEnlarge = new System.Windows.Forms.CheckBox();
            this.cbCenterImage = new System.Windows.Forms.CheckBox();
            this.lblOutputFilename = new System.Windows.Forms.Label();
            this.txtOutputFilename = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.chImages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // lvImages
            // 
            this.lvImages.AllowDrop = true;
            this.lvImages.AllowItemDrag = true;
            this.lvImages.AutoFillColumn = true;
            this.lvImages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chImages});
            this.lvImages.FullRowSelect = true;
            this.lvImages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvImages.Location = new System.Drawing.Point(8, 40);
            this.lvImages.Name = "lvImages";
            this.lvImages.Size = new System.Drawing.Size(560, 368);
            this.lvImages.TabIndex = 0;
            this.lvImages.UseCompatibleStateImageBehavior = false;
            this.lvImages.View = System.Windows.Forms.View.Details;
            this.lvImages.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvImages_DragDrop);
            this.lvImages.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvImages_DragEnter);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(8, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(168, 24);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add...";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(184, 8);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(168, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(8, 416);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(38, 13);
            this.lblWidth.TabIndex = 3;
            this.lblWidth.Text = "Width:";
            // 
            // nudWidth
            // 
            this.nudWidth.Location = new System.Drawing.Point(8, 432);
            this.nudWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(80, 20);
            this.nudWidth.TabIndex = 4;
            this.nudWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(104, 416);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(41, 13);
            this.lblHeight.TabIndex = 5;
            this.lblHeight.Text = "Height:";
            // 
            // nudHeight
            // 
            this.nudHeight.Location = new System.Drawing.Point(104, 432);
            this.nudHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(80, 20);
            this.nudHeight.TabIndex = 6;
            this.nudHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbAllowEnlarge
            // 
            this.cbAllowEnlarge.AutoSize = true;
            this.cbAllowEnlarge.Location = new System.Drawing.Point(8, 464);
            this.cbAllowEnlarge.Name = "cbAllowEnlarge";
            this.cbAllowEnlarge.Size = new System.Drawing.Size(89, 17);
            this.cbAllowEnlarge.TabIndex = 7;
            this.cbAllowEnlarge.Text = "Allow enlarge";
            this.cbAllowEnlarge.UseVisualStyleBackColor = true;
            // 
            // cbCenterImage
            // 
            this.cbCenterImage.AutoSize = true;
            this.cbCenterImage.Location = new System.Drawing.Point(8, 488);
            this.cbCenterImage.Name = "cbCenterImage";
            this.cbCenterImage.Size = new System.Drawing.Size(88, 17);
            this.cbCenterImage.TabIndex = 8;
            this.cbCenterImage.Text = "Center image";
            this.cbCenterImage.UseVisualStyleBackColor = true;
            // 
            // lblOutputFilename
            // 
            this.lblOutputFilename.AutoSize = true;
            this.lblOutputFilename.Location = new System.Drawing.Point(8, 512);
            this.lblOutputFilename.Name = "lblOutputFilename";
            this.lblOutputFilename.Size = new System.Drawing.Size(87, 13);
            this.lblOutputFilename.TabIndex = 9;
            this.lblOutputFilename.Text = "Output file name:";
            // 
            // txtOutputFilename
            // 
            this.txtOutputFilename.Location = new System.Drawing.Point(8, 528);
            this.txtOutputFilename.Name = "txtOutputFilename";
            this.txtOutputFilename.Size = new System.Drawing.Size(256, 20);
            this.txtOutputFilename.TabIndex = 10;
            this.txtOutputFilename.Text = "Thumbnail_$filename";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(8, 560);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(256, 23);
            this.btnGenerate.TabIndex = 11;
            this.btnGenerate.Text = "Generate thumbnails";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // ImageThumbnailerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 593);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtOutputFilename);
            this.Controls.Add(this.lblOutputFilename);
            this.Controls.Add(this.cbCenterImage);
            this.Controls.Add(this.cbAllowEnlarge);
            this.Controls.Add(this.nudHeight);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.nudWidth);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lvImages);
            this.Name = "ImageThumbnailerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Image thumbnailer";
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelpersLib.MyListView lvImages;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.CheckBox cbAllowEnlarge;
        private System.Windows.Forms.CheckBox cbCenterImage;
        private System.Windows.Forms.Label lblOutputFilename;
        private System.Windows.Forms.TextBox txtOutputFilename;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.ColumnHeader chImages;
    }
}