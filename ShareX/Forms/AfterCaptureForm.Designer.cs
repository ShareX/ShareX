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
            btnContinue = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnCopy = new System.Windows.Forms.Button();
            tcTasks = new System.Windows.Forms.TabControl();
            tpAfterCapture = new System.Windows.Forms.TabPage();
            lvAfterCaptureTasks = new ShareX.HelpersLib.MyListView();
            chAfterCapture = new System.Windows.Forms.ColumnHeader();
            tpBeforeUpload = new System.Windows.Forms.TabPage();
            ucBeforeUpload = new BeforeUploadControl();
            tpAfterUpload = new System.Windows.Forms.TabPage();
            lvAfterUploadTasks = new ShareX.HelpersLib.MyListView();
            chAfterUpload = new System.Windows.Forms.ColumnHeader();
            lblFileName = new System.Windows.Forms.Label();
            txtFileName = new System.Windows.Forms.TextBox();
            pbImage = new ShareX.HelpersLib.MyPictureBox();
            tcTasks.SuspendLayout();
            tpAfterCapture.SuspendLayout();
            tpBeforeUpload.SuspendLayout();
            tpAfterUpload.SuspendLayout();
            SuspendLayout();
            // 
            // btnContinue
            // 
            resources.ApplyResources(btnContinue, "btnContinue");
            btnContinue.Name = "btnContinue";
            btnContinue.UseVisualStyleBackColor = true;
            btnContinue.Click += btnContinue_Click;
            // 
            // btnCancel
            // 
            resources.ApplyResources(btnCancel, "btnCancel");
            btnCancel.Name = "btnCancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnCopy
            // 
            resources.ApplyResources(btnCopy, "btnCopy");
            btnCopy.Name = "btnCopy";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += btnCopy_Click;
            // 
            // tcTasks
            // 
            resources.ApplyResources(tcTasks, "tcTasks");
            tcTasks.Controls.Add(tpAfterCapture);
            tcTasks.Controls.Add(tpBeforeUpload);
            tcTasks.Controls.Add(tpAfterUpload);
            tcTasks.Name = "tcTasks";
            tcTasks.SelectedIndex = 0;
            // 
            // tpAfterCapture
            // 
            tpAfterCapture.BackColor = System.Drawing.SystemColors.Window;
            tpAfterCapture.Controls.Add(lvAfterCaptureTasks);
            resources.ApplyResources(tpAfterCapture, "tpAfterCapture");
            tpAfterCapture.Name = "tpAfterCapture";
            // 
            // lvAfterCaptureTasks
            // 
            lvAfterCaptureTasks.AutoFillColumn = true;
            lvAfterCaptureTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lvAfterCaptureTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chAfterCapture });
            resources.ApplyResources(lvAfterCaptureTasks, "lvAfterCaptureTasks");
            lvAfterCaptureTasks.FullRowSelect = true;
            lvAfterCaptureTasks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            lvAfterCaptureTasks.MultiSelect = false;
            lvAfterCaptureTasks.Name = "lvAfterCaptureTasks";
            lvAfterCaptureTasks.UseCompatibleStateImageBehavior = false;
            lvAfterCaptureTasks.View = System.Windows.Forms.View.Details;
            lvAfterCaptureTasks.ItemSelectionChanged += lvAfterCaptureTasks_ItemSelectionChanged;
            lvAfterCaptureTasks.MouseDown += lvAfterCaptureTasks_MouseDown;
            // 
            // tpBeforeUpload
            // 
            tpBeforeUpload.BackColor = System.Drawing.SystemColors.Window;
            tpBeforeUpload.Controls.Add(ucBeforeUpload);
            resources.ApplyResources(tpBeforeUpload, "tpBeforeUpload");
            tpBeforeUpload.Name = "tpBeforeUpload";
            // 
            // ucBeforeUpload
            // 
            resources.ApplyResources(ucBeforeUpload, "ucBeforeUpload");
            ucBeforeUpload.Name = "ucBeforeUpload";
            // 
            // tpAfterUpload
            // 
            tpAfterUpload.BackColor = System.Drawing.SystemColors.Window;
            tpAfterUpload.Controls.Add(lvAfterUploadTasks);
            resources.ApplyResources(tpAfterUpload, "tpAfterUpload");
            tpAfterUpload.Name = "tpAfterUpload";
            // 
            // lvAfterUploadTasks
            // 
            lvAfterUploadTasks.AutoFillColumn = true;
            lvAfterUploadTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lvAfterUploadTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chAfterUpload });
            resources.ApplyResources(lvAfterUploadTasks, "lvAfterUploadTasks");
            lvAfterUploadTasks.FullRowSelect = true;
            lvAfterUploadTasks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            lvAfterUploadTasks.MultiSelect = false;
            lvAfterUploadTasks.Name = "lvAfterUploadTasks";
            lvAfterUploadTasks.UseCompatibleStateImageBehavior = false;
            lvAfterUploadTasks.View = System.Windows.Forms.View.Details;
            lvAfterUploadTasks.ItemSelectionChanged += lvAfterUploadTasks_ItemSelectionChanged;
            lvAfterUploadTasks.MouseDown += lvAfterUploadTasks_MouseDown;
            // 
            // lblFileName
            // 
            resources.ApplyResources(lblFileName, "lblFileName");
            lblFileName.Name = "lblFileName";
            // 
            // txtFileName
            // 
            resources.ApplyResources(txtFileName, "txtFileName");
            txtFileName.Name = "txtFileName";
            txtFileName.KeyDown += txtFileName_KeyDown;
            txtFileName.KeyUp += txtFileName_KeyUp;
            // 
            // pbImage
            // 
            resources.ApplyResources(pbImage, "pbImage");
            pbImage.BackColor = System.Drawing.SystemColors.Window;
            pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pbImage.DrawCheckeredBackground = true;
            pbImage.EnableRightClickMenu = true;
            pbImage.FullscreenOnClick = true;
            pbImage.Name = "pbImage";
            pbImage.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            pbImage.ShowImageSizeLabel = true;
            // 
            // AfterCaptureForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(txtFileName);
            Controls.Add(lblFileName);
            Controls.Add(tcTasks);
            Controls.Add(btnCopy);
            Controls.Add(btnCancel);
            Controls.Add(btnContinue);
            Controls.Add(pbImage);
            Name = "AfterCaptureForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            TopMost = true;
            Shown += AfterCaptureForm_Shown;
            tcTasks.ResumeLayout(false);
            tpAfterCapture.ResumeLayout(false);
            tpBeforeUpload.ResumeLayout(false);
            tpAfterUpload.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

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
    }
}