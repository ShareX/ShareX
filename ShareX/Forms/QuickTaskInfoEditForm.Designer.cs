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
            this.mbAfterCaptureTasks = new ShareX.HelpersLib.MenuButton();
            this.mbAfterUploadTasks = new ShareX.HelpersLib.MenuButton();
            this.cmsAfterCapture = new System.Windows.Forms.ContextMenuStrip(this.components);
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
            this.mbAfterCaptureTasks.Location = new System.Drawing.Point(8, 72);
            this.mbAfterCaptureTasks.Menu = this.cmsAfterCapture;
            this.mbAfterCaptureTasks.Name = "mbAfterCaptureTasks";
            this.mbAfterCaptureTasks.Size = new System.Drawing.Size(424, 23);
            this.mbAfterCaptureTasks.TabIndex = 0;
            this.mbAfterCaptureTasks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbAfterCaptureTasks.UseVisualStyleBackColor = true;
            // 
            // mbAfterUploadTasks
            // 
            this.mbAfterUploadTasks.Location = new System.Drawing.Point(8, 120);
            this.mbAfterUploadTasks.Menu = this.cmsAfterUpload;
            this.mbAfterUploadTasks.Name = "mbAfterUploadTasks";
            this.mbAfterUploadTasks.Size = new System.Drawing.Size(424, 23);
            this.mbAfterUploadTasks.TabIndex = 1;
            this.mbAfterUploadTasks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mbAfterUploadTasks.UseVisualStyleBackColor = true;
            // 
            // cmsAfterCapture
            // 
            this.cmsAfterCapture.Name = "cmsAfterCapture";
            this.cmsAfterCapture.Size = new System.Drawing.Size(61, 4);
            // 
            // cmsAfterUpload
            // 
            this.cmsAfterUpload.Name = "cmsAfterUpload";
            this.cmsAfterUpload.Size = new System.Drawing.Size(61, 4);
            // 
            // lblAfterCaptureTasks
            // 
            this.lblAfterCaptureTasks.AutoSize = true;
            this.lblAfterCaptureTasks.Location = new System.Drawing.Point(5, 56);
            this.lblAfterCaptureTasks.Name = "lblAfterCaptureTasks";
            this.lblAfterCaptureTasks.Size = new System.Drawing.Size(99, 13);
            this.lblAfterCaptureTasks.TabIndex = 2;
            this.lblAfterCaptureTasks.Text = "After capture tasks:";
            // 
            // lblAfterUploadTasks
            // 
            this.lblAfterUploadTasks.AutoSize = true;
            this.lblAfterUploadTasks.Location = new System.Drawing.Point(5, 104);
            this.lblAfterUploadTasks.Name = "lblAfterUploadTasks";
            this.lblAfterUploadTasks.Size = new System.Drawing.Size(95, 13);
            this.lblAfterUploadTasks.TabIndex = 3;
            this.lblAfterUploadTasks.Text = "After upload tasks:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(5, 8);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(57, 13);
            this.lblName.TabIndex = 4;
            this.lblName.Text = "Menu text:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(8, 24);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(424, 20);
            this.txtName.TabIndex = 5;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(312, 160);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(120, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // QuickTaskInfoEditForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 192);
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Edit quick task menu item";
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