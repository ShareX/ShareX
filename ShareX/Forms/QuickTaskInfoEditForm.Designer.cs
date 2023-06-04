namespace ShareX
{
    partial class QuickTaskInfoEditForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuickTaskInfoEditForm));
            this.mbAfterCaptureTasks = new ShareX.HelpersLib.MenuButton();
            this.cmsAfterCapture = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mbAfterUploadTasks = new ShareX.HelpersLib.MenuButton();
            this.cmsAfterUpload = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lblAfterCaptureTasks = new System.Windows.Forms.Label();
            this.lblAfterUploadTasks = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mbAfterCaptureTasks
            // 
            resources.ApplyResources(this.mbAfterCaptureTasks, "mbAfterCaptureTasks");
            this.mbAfterCaptureTasks.Menu = this.cmsAfterCapture;
            this.mbAfterCaptureTasks.Name = "mbAfterCaptureTasks";
            this.mbAfterCaptureTasks.UseVisualStyleBackColor = true;
            // 
            // cmsAfterCapture
            // 
            this.cmsAfterCapture.Name = "cmsAfterCapture";
            resources.ApplyResources(this.cmsAfterCapture, "cmsAfterCapture");
            // 
            // mbAfterUploadTasks
            // 
            resources.ApplyResources(this.mbAfterUploadTasks, "mbAfterUploadTasks");
            this.mbAfterUploadTasks.Menu = this.cmsAfterUpload;
            this.mbAfterUploadTasks.Name = "mbAfterUploadTasks";
            this.mbAfterUploadTasks.UseVisualStyleBackColor = true;
            // 
            // cmsAfterUpload
            // 
            this.cmsAfterUpload.Name = "cmsAfterUpload";
            resources.ApplyResources(this.cmsAfterUpload, "cmsAfterUpload");
            // 
            // lblAfterCaptureTasks
            // 
            resources.ApplyResources(this.lblAfterCaptureTasks, "lblAfterCaptureTasks");
            this.lblAfterCaptureTasks.Name = "lblAfterCaptureTasks";
            // 
            // lblAfterUploadTasks
            // 
            resources.ApplyResources(this.lblAfterUploadTasks, "lblAfterUploadTasks");
            this.lblAfterUploadTasks.Name = "lblAfterUploadTasks";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // QuickTaskInfoEditForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblAfterUploadTasks);
            this.Controls.Add(this.lblAfterCaptureTasks);
            this.Controls.Add(this.mbAfterUploadTasks);
            this.Controls.Add(this.mbAfterCaptureTasks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "QuickTaskInfoEditForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelpersLib.MenuButton mbAfterCaptureTasks;
        private HelpersLib.MenuButton mbAfterUploadTasks;
        private System.Windows.Forms.ContextMenuStrip cmsAfterCapture;
        private System.Windows.Forms.ContextMenuStrip cmsAfterUpload;
        private System.Windows.Forms.Label lblAfterCaptureTasks;
        private System.Windows.Forms.Label lblAfterUploadTasks;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnOK;
    }
}