namespace ShareX
{
    partial class AfterCaptureForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AfterCaptureForm));
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbImage = new ShareX.HelpersLib.MyPictureBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tcTasks = new System.Windows.Forms.TabControl();
            this.tpAfterCapture = new System.Windows.Forms.TabPage();
            this.lvAfterCaptureTasks = new ShareX.HelpersLib.MyListView();
            this.chAfterCapture = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpBeforeUpload = new System.Windows.Forms.TabPage();
            this.ucBeforeUpload = new ShareX.BeforeUploadControl();
            this.tpAfterUpload = new System.Windows.Forms.TabPage();
            this.lvAfterUploadTasks = new ShareX.HelpersLib.MyListView();
            this.chAfterUpload = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblFileName = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.lblImageSize = new System.Windows.Forms.Label();
            this.tcTasks.SuspendLayout();
            this.tpAfterCapture.SuspendLayout();
            this.tpBeforeUpload.SuspendLayout();
            this.tpAfterUpload.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnContinue
            // 
            resources.ApplyResources(this.btnContinue, "btnContinue");
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pbImage
            // 
            resources.ApplyResources(this.pbImage, "pbImage");
            this.pbImage.BackColor = System.Drawing.Color.White;
            this.pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbImage.Cursor = System.Windows.Forms.Cursors.Default;
            this.pbImage.DrawCheckeredBackground = true;
            this.pbImage.EnableRightClickMenu = true;
            this.pbImage.FullscreenOnClick = true;
            this.pbImage.Name = "pbImage";
            // 
            // btnCopy
            // 
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // tcTasks
            // 
            resources.ApplyResources(this.tcTasks, "tcTasks");
            this.tcTasks.Controls.Add(this.tpAfterCapture);
            this.tcTasks.Controls.Add(this.tpBeforeUpload);
            this.tcTasks.Controls.Add(this.tpAfterUpload);
            this.tcTasks.Name = "tcTasks";
            this.tcTasks.SelectedIndex = 0;
            // 
            // tpAfterCapture
            // 
            this.tpAfterCapture.Controls.Add(this.lvAfterCaptureTasks);
            resources.ApplyResources(this.tpAfterCapture, "tpAfterCapture");
            this.tpAfterCapture.Name = "tpAfterCapture";
            this.tpAfterCapture.UseVisualStyleBackColor = true;
            // 
            // lvAfterCaptureTasks
            // 
            this.lvAfterCaptureTasks.AutoFillColumn = true;
            this.lvAfterCaptureTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvAfterCaptureTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chAfterCapture});
            resources.ApplyResources(this.lvAfterCaptureTasks, "lvAfterCaptureTasks");
            this.lvAfterCaptureTasks.FullRowSelect = true;
            this.lvAfterCaptureTasks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvAfterCaptureTasks.MultiSelect = false;
            this.lvAfterCaptureTasks.Name = "lvAfterCaptureTasks";
            this.lvAfterCaptureTasks.Scrollable = false;
            this.lvAfterCaptureTasks.UseCompatibleStateImageBehavior = false;
            this.lvAfterCaptureTasks.View = System.Windows.Forms.View.Details;
            this.lvAfterCaptureTasks.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvAfterCaptureTasks_ItemSelectionChanged);
            this.lvAfterCaptureTasks.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvAfterCaptureTasks_MouseDown);
            // 
            // tpBeforeUpload
            // 
            this.tpBeforeUpload.Controls.Add(this.ucBeforeUpload);
            resources.ApplyResources(this.tpBeforeUpload, "tpBeforeUpload");
            this.tpBeforeUpload.Name = "tpBeforeUpload";
            this.tpBeforeUpload.UseVisualStyleBackColor = true;
            // 
            // ucBeforeUpload
            // 
            resources.ApplyResources(this.ucBeforeUpload, "ucBeforeUpload");
            this.ucBeforeUpload.Name = "ucBeforeUpload";
            // 
            // tpAfterUpload
            // 
            this.tpAfterUpload.Controls.Add(this.lvAfterUploadTasks);
            resources.ApplyResources(this.tpAfterUpload, "tpAfterUpload");
            this.tpAfterUpload.Name = "tpAfterUpload";
            this.tpAfterUpload.UseVisualStyleBackColor = true;
            // 
            // lvAfterUploadTasks
            // 
            this.lvAfterUploadTasks.AutoFillColumn = true;
            this.lvAfterUploadTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvAfterUploadTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chAfterUpload});
            resources.ApplyResources(this.lvAfterUploadTasks, "lvAfterUploadTasks");
            this.lvAfterUploadTasks.FullRowSelect = true;
            this.lvAfterUploadTasks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvAfterUploadTasks.MultiSelect = false;
            this.lvAfterUploadTasks.Name = "lvAfterUploadTasks";
            this.lvAfterUploadTasks.Scrollable = false;
            this.lvAfterUploadTasks.UseCompatibleStateImageBehavior = false;
            this.lvAfterUploadTasks.View = System.Windows.Forms.View.Details;
            this.lvAfterUploadTasks.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvAfterUploadTasks_ItemSelectionChanged);
            this.lvAfterUploadTasks.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvAfterUploadTasks_MouseDown);
            // 
            // lblFileName
            // 
            resources.ApplyResources(this.lblFileName, "lblFileName");
            this.lblFileName.Name = "lblFileName";
            // 
            // txtFileName
            // 
            resources.ApplyResources(this.txtFileName, "txtFileName");
            this.txtFileName.Name = "txtFileName";
            // 
            // lblImageSize
            // 
            resources.ApplyResources(this.lblImageSize, "lblImageSize");
            this.lblImageSize.BackColor = System.Drawing.Color.White;
            this.lblImageSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblImageSize.Name = "lblImageSize";
            // 
            // AfterCaptureForm
            // 
            this.AcceptButton = this.btnContinue;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.lblImageSize);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.tcTasks);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.pbImage);
            this.Name = "AfterCaptureForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.tcTasks.ResumeLayout(false);
            this.tpAfterCapture.ResumeLayout(false);
            this.tpBeforeUpload.ResumeLayout(false);
            this.tpAfterUpload.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelpersLib.MyPictureBox pbImage;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.TabControl tcTasks;
        private System.Windows.Forms.TabPage tpAfterCapture;
        private System.Windows.Forms.TabPage tpBeforeUpload;
        private BeforeUploadControl ucBeforeUpload;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.TabPage tpAfterUpload;
        private HelpersLib.MyListView lvAfterCaptureTasks;
        private System.Windows.Forms.ColumnHeader chAfterCapture;
        private HelpersLib.MyListView lvAfterUploadTasks;
        private System.Windows.Forms.ColumnHeader chAfterUpload;
        private System.Windows.Forms.Label lblImageSize;
    }
}