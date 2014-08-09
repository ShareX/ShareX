namespace ShareX
{
    partial class VideoEncodersForm
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
            this.btnEncoderDuplicate = new System.Windows.Forms.Button();
            this.btnEncodersAdd = new System.Windows.Forms.Button();
            this.btnEncodersEdit = new System.Windows.Forms.Button();
            this.btnEncodersRemove = new System.Windows.Forms.Button();
            this.lvEncoders = new HelpersLib.MyListView();
            this.chEncoderDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chEncoderPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chEncoderArgs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chEncoderOutputExtension = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // btnEncoderDuplicate
            // 
            this.btnEncoderDuplicate.Location = new System.Drawing.Point(168, 8);
            this.btnEncoderDuplicate.Name = "btnEncoderDuplicate";
            this.btnEncoderDuplicate.Size = new System.Drawing.Size(75, 23);
            this.btnEncoderDuplicate.TabIndex = 2;
            this.btnEncoderDuplicate.Text = "Duplicate";
            this.btnEncoderDuplicate.UseVisualStyleBackColor = true;
            this.btnEncoderDuplicate.Click += new System.EventHandler(this.btnEncoderDuplicate_Click);
            // 
            // btnEncodersAdd
            // 
            this.btnEncodersAdd.Location = new System.Drawing.Point(8, 8);
            this.btnEncodersAdd.Name = "btnEncodersAdd";
            this.btnEncodersAdd.Size = new System.Drawing.Size(75, 23);
            this.btnEncodersAdd.TabIndex = 0;
            this.btnEncodersAdd.Text = "Add";
            this.btnEncodersAdd.UseVisualStyleBackColor = true;
            this.btnEncodersAdd.Click += new System.EventHandler(this.btnEncodersAdd_Click);
            // 
            // btnEncodersEdit
            // 
            this.btnEncodersEdit.Location = new System.Drawing.Point(88, 8);
            this.btnEncodersEdit.Name = "btnEncodersEdit";
            this.btnEncodersEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEncodersEdit.TabIndex = 1;
            this.btnEncodersEdit.Text = "Edit";
            this.btnEncodersEdit.UseVisualStyleBackColor = true;
            this.btnEncodersEdit.Click += new System.EventHandler(this.btnEncodersEdit_Click);
            // 
            // btnEncodersRemove
            // 
            this.btnEncodersRemove.Location = new System.Drawing.Point(248, 8);
            this.btnEncodersRemove.Name = "btnEncodersRemove";
            this.btnEncodersRemove.Size = new System.Drawing.Size(75, 23);
            this.btnEncodersRemove.TabIndex = 3;
            this.btnEncodersRemove.Text = "Remove";
            this.btnEncodersRemove.UseVisualStyleBackColor = true;
            this.btnEncodersRemove.Click += new System.EventHandler(this.btnEncodersRemove_Click);
            // 
            // lvEncoders
            // 
            this.lvEncoders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvEncoders.AutoFillColumn = true;
            this.lvEncoders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chEncoderDescription,
            this.chEncoderPath,
            this.chEncoderArgs,
            this.chEncoderOutputExtension});
            this.lvEncoders.FullRowSelect = true;
            this.lvEncoders.Location = new System.Drawing.Point(8, 40);
            this.lvEncoders.MultiSelect = false;
            this.lvEncoders.Name = "lvEncoders";
            this.lvEncoders.Size = new System.Drawing.Size(646, 330);
            this.lvEncoders.TabIndex = 4;
            this.lvEncoders.UseCompatibleStateImageBehavior = false;
            this.lvEncoders.View = System.Windows.Forms.View.Details;
            this.lvEncoders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvEncoders_MouseDoubleClick);
            // 
            // chEncoderDescription
            // 
            this.chEncoderDescription.Text = "Description";
            this.chEncoderDescription.Width = 130;
            // 
            // chEncoderPath
            // 
            this.chEncoderPath.Text = "Path";
            this.chEncoderPath.Width = 80;
            // 
            // chEncoderArgs
            // 
            this.chEncoderArgs.Text = "Args";
            this.chEncoderArgs.Width = 230;
            // 
            // chEncoderOutputExtension
            // 
            this.chEncoderOutputExtension.Text = "Output extension";
            this.chEncoderOutputExtension.Width = 100;
            // 
            // VideoEncodersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 381);
            this.Controls.Add(this.btnEncoderDuplicate);
            this.Controls.Add(this.lvEncoders);
            this.Controls.Add(this.btnEncodersAdd);
            this.Controls.Add(this.btnEncodersRemove);
            this.Controls.Add(this.btnEncodersEdit);
            this.MinimumSize = new System.Drawing.Size(680, 420);
            this.Name = "VideoEncodersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ShareX - Encoder Profiles";
            this.Load += new System.EventHandler(this.VideoEncodersForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEncoderDuplicate;
        private HelpersLib.MyListView lvEncoders;
        private System.Windows.Forms.ColumnHeader chEncoderDescription;
        private System.Windows.Forms.ColumnHeader chEncoderPath;
        private System.Windows.Forms.ColumnHeader chEncoderArgs;
        private System.Windows.Forms.ColumnHeader chEncoderOutputExtension;
        private System.Windows.Forms.Button btnEncodersAdd;
        private System.Windows.Forms.Button btnEncodersEdit;
        private System.Windows.Forms.Button btnEncodersRemove;
    }
}