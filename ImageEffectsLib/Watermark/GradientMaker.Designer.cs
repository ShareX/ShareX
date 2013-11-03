namespace ImageEffectsLib
{
    partial class GradientMaker
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
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.btnAddColor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtColor = new System.Windows.Forms.TextBox();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.btnBrowseColor = new System.Windows.Forms.Button();
            this.rtbCodes = new System.Windows.Forms.RichTextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cboGradientDirection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // pbPreview
            // 
            this.pbPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPreview.Location = new System.Drawing.Point(8, 6);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(384, 50);
            this.pbPreview.TabIndex = 2;
            this.pbPreview.TabStop = false;
            // 
            // btnAddColor
            // 
            this.btnAddColor.Location = new System.Drawing.Point(296, 300);
            this.btnAddColor.Name = "btnAddColor";
            this.btnAddColor.Size = new System.Drawing.Size(96, 23);
            this.btnAddColor.TabIndex = 12;
            this.btnAddColor.Text = "Add / Update";
            this.btnAddColor.UseVisualStyleBackColor = true;
            this.btnAddColor.Click += new System.EventHandler(this.btnAddColor_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 303);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Color:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(200, 303);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Offset:";
            // 
            // txtColor
            // 
            this.txtColor.Location = new System.Drawing.Point(48, 300);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(112, 20);
            this.txtColor.TabIndex = 8;
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(248, 300);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(40, 20);
            this.txtOffset.TabIndex = 11;
            // 
            // btnBrowseColor
            // 
            this.btnBrowseColor.Location = new System.Drawing.Point(168, 300);
            this.btnBrowseColor.Name = "btnBrowseColor";
            this.btnBrowseColor.Size = new System.Drawing.Size(24, 23);
            this.btnBrowseColor.TabIndex = 9;
            this.btnBrowseColor.Text = "...";
            this.btnBrowseColor.UseVisualStyleBackColor = true;
            this.btnBrowseColor.Click += new System.EventHandler(this.btnBrowseColor_Click);
            // 
            // rtbCodes
            // 
            this.rtbCodes.AcceptsTab = true;
            this.rtbCodes.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbCodes.Location = new System.Drawing.Point(8, 64);
            this.rtbCodes.Name = "rtbCodes";
            this.rtbCodes.Size = new System.Drawing.Size(384, 232);
            this.rtbCodes.TabIndex = 5;
            this.rtbCodes.Text = "";
            this.rtbCodes.SelectionChanged += new System.EventHandler(this.rtbCodes_SelectionChanged);
            this.rtbCodes.TextChanged += new System.EventHandler(this.rtbCodes_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(240, 332);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(320, 332);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 23);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cboGradientDirection
            // 
            this.cboGradientDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGradientDirection.FormattingEnabled = true;
            this.cboGradientDirection.Location = new System.Drawing.Point(104, 332);
            this.cboGradientDirection.Name = "cboGradientDirection";
            this.cboGradientDirection.Size = new System.Drawing.Size(128, 21);
            this.cboGradientDirection.TabIndex = 3;
            this.cboGradientDirection.SelectedIndexChanged += new System.EventHandler(this.cboGradientDirection_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 336);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gradient direction:";
            // 
            // GradientMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 361);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboGradientDirection);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rtbCodes);
            this.Controls.Add(this.btnBrowseColor);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.txtColor);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnAddColor);
            this.Controls.Add(this.pbPreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "GradientMaker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Gradient maker";
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.Button btnAddColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtColor;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Button btnBrowseColor;
        private System.Windows.Forms.RichTextBox rtbCodes;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboGradientDirection;
        private System.Windows.Forms.Label label1;
    }
}