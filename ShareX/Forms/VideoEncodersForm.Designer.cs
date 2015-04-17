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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoEncodersForm));
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
            resources.ApplyResources(this.btnEncoderDuplicate, "btnEncoderDuplicate");
            this.btnEncoderDuplicate.Name = "btnEncoderDuplicate";
            this.btnEncoderDuplicate.UseVisualStyleBackColor = true;
            this.btnEncoderDuplicate.Click += new System.EventHandler(this.btnEncoderDuplicate_Click);
            // 
            // btnEncodersAdd
            // 
            resources.ApplyResources(this.btnEncodersAdd, "btnEncodersAdd");
            this.btnEncodersAdd.Name = "btnEncodersAdd";
            this.btnEncodersAdd.UseVisualStyleBackColor = true;
            this.btnEncodersAdd.Click += new System.EventHandler(this.btnEncodersAdd_Click);
            // 
            // btnEncodersEdit
            // 
            resources.ApplyResources(this.btnEncodersEdit, "btnEncodersEdit");
            this.btnEncodersEdit.Name = "btnEncodersEdit";
            this.btnEncodersEdit.UseVisualStyleBackColor = true;
            this.btnEncodersEdit.Click += new System.EventHandler(this.btnEncodersEdit_Click);
            // 
            // btnEncodersRemove
            // 
            resources.ApplyResources(this.btnEncodersRemove, "btnEncodersRemove");
            this.btnEncodersRemove.Name = "btnEncodersRemove";
            this.btnEncodersRemove.UseVisualStyleBackColor = true;
            this.btnEncodersRemove.Click += new System.EventHandler(this.btnEncodersRemove_Click);
            // 
            // lvEncoders
            // 
            resources.ApplyResources(this.lvEncoders, "lvEncoders");
            this.lvEncoders.AutoFillColumn = true;
            this.lvEncoders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chEncoderDescription,
            this.chEncoderPath,
            this.chEncoderArgs,
            this.chEncoderOutputExtension});
            this.lvEncoders.FullRowSelect = true;
            this.lvEncoders.MultiSelect = false;
            this.lvEncoders.Name = "lvEncoders";
            this.lvEncoders.UseCompatibleStateImageBehavior = false;
            this.lvEncoders.View = System.Windows.Forms.View.Details;
            this.lvEncoders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvEncoders_MouseDoubleClick);
            // 
            // chEncoderDescription
            // 
            resources.ApplyResources(this.chEncoderDescription, "chEncoderDescription");
            // 
            // chEncoderPath
            // 
            resources.ApplyResources(this.chEncoderPath, "chEncoderPath");
            // 
            // chEncoderArgs
            // 
            resources.ApplyResources(this.chEncoderArgs, "chEncoderArgs");
            // 
            // chEncoderOutputExtension
            // 
            resources.ApplyResources(this.chEncoderOutputExtension, "chEncoderOutputExtension");
            // 
            // VideoEncodersForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnEncoderDuplicate);
            this.Controls.Add(this.lvEncoders);
            this.Controls.Add(this.btnEncodersAdd);
            this.Controls.Add(this.btnEncodersRemove);
            this.Controls.Add(this.btnEncodersEdit);
            this.Name = "VideoEncodersForm";
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