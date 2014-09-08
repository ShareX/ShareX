namespace HelpersLib
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
            this.lvGradientPoints = new System.Windows.Forms.ListView();
            this.chLocation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.cbtnCurrentColor = new HelpersLib.ColorButton();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // lvGradientPoints
            // 
            this.lvGradientPoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chLocation});
            this.lvGradientPoints.FullRowSelect = true;
            this.lvGradientPoints.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvGradientPoints.HideSelection = false;
            this.lvGradientPoints.Location = new System.Drawing.Point(128, 112);
            this.lvGradientPoints.Name = "lvGradientPoints";
            this.lvGradientPoints.Size = new System.Drawing.Size(80, 160);
            this.lvGradientPoints.TabIndex = 1;
            this.lvGradientPoints.UseCompatibleStateImageBehavior = false;
            this.lvGradientPoints.View = System.Windows.Forms.View.Details;
            this.lvGradientPoints.SelectedIndexChanged += new System.EventHandler(this.lvGradientPoints_SelectedIndexChanged);
            this.lvGradientPoints.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvGradientPoints_MouseDoubleClick);
            // 
            // chLocation
            // 
            this.chLocation.Text = "Location";
            this.chLocation.Width = 76;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(8, 112);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(112, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(8, 136);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(112, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // nudLocation
            // 
            this.nudLocation.DecimalPlaces = 2;
            this.nudLocation.Location = new System.Drawing.Point(64, 188);
            this.nudLocation.Name = "nudLocation";
            this.nudLocation.Size = new System.Drawing.Size(56, 20);
            this.nudLocation.TabIndex = 5;
            this.nudLocation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudLocation.ValueChanged += new System.EventHandler(this.nudLocation_ValueChanged);
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(8, 192);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(51, 13);
            this.lblLocation.TabIndex = 6;
            this.lblLocation.Text = "Location:";
            // 
            // cbGradientType
            // 
            this.cbGradientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGradientType.FormattingEnabled = true;
            this.cbGradientType.Location = new System.Drawing.Point(88, 84);
            this.cbGradientType.Name = "cbGradientType";
            this.cbGradientType.Size = new System.Drawing.Size(120, 21);
            this.cbGradientType.TabIndex = 7;
            this.cbGradientType.SelectedIndexChanged += new System.EventHandler(this.cbGradientType_SelectedIndexChanged);
            // 
            // lblGradientType
            // 
            this.lblGradientType.AutoSize = true;
            this.lblGradientType.Location = new System.Drawing.Point(8, 88);
            this.lblGradientType.Name = "lblGradientType";
            this.lblGradientType.Size = new System.Drawing.Size(73, 13);
            this.lblGradientType.TabIndex = 8;
            this.lblGradientType.Text = "Gradient type:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(8, 280);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(96, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(112, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pbPreview
            // 
            this.pbPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPreview.Location = new System.Drawing.Point(8, 26);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(200, 50);
            this.pbPreview.TabIndex = 11;
            this.pbPreview.TabStop = false;
            // 
            // lblPreview
            // 
            this.lblPreview.AutoSize = true;
            this.lblPreview.Location = new System.Drawing.Point(8, 8);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(48, 13);
            this.lblPreview.TabIndex = 12;
            this.lblPreview.Text = "Preview:";
            // 
            // cbtnCurrentColor
            // 
            this.cbtnCurrentColor.Color = System.Drawing.Color.White;
            this.cbtnCurrentColor.Location = new System.Drawing.Point(8, 160);
            this.cbtnCurrentColor.Name = "cbtnCurrentColor";
            this.cbtnCurrentColor.Size = new System.Drawing.Size(112, 24);
            this.cbtnCurrentColor.TabIndex = 2;
            this.cbtnCurrentColor.Text = "Color";
            this.cbtnCurrentColor.UseVisualStyleBackColor = true;
            this.cbtnCurrentColor.ColorChanged += new HelpersLib.ColorButton.ColorChangedEventHandler(this.cbtnCurrentColor_ColorChanged);
            // 
            // GradientPickerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 311);
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
            this.Name = "GradientPickerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gradient";
            ((System.ComponentModel.ISupportInitialize)(this.nudLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvGradientPoints;
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
    }
}