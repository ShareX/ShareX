namespace ShareX.UploadersLib.Forms
{
    partial class MegaFolderSelectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MegaFolderSelectForm));
            this.listViewNodes = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSelectFolder = new System.Windows.Forms.Button();
            this.labelFolderBreadcrumb = new System.Windows.Forms.Label();
            this.buttonNavigateUp = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewNodes
            // 
            this.listViewNodes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewNodes.Location = new System.Drawing.Point(0, 40);
            this.listViewNodes.Margin = new System.Windows.Forms.Padding(0);
            this.listViewNodes.Name = "listViewNodes";
            this.listViewNodes.Size = new System.Drawing.Size(462, 177);
            this.listViewNodes.TabIndex = 0;
            this.listViewNodes.UseCompatibleStateImageBehavior = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonSelectFolder);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 220);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(12);
            this.panel1.Size = new System.Drawing.Size(462, 49);
            this.panel1.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Left;
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(177, 25);
            this.progressBar1.TabIndex = 2;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonCancel.Location = new System.Drawing.Point(260, 12);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Padding = new System.Windows.Forms.Padding(3);
            this.buttonCancel.Size = new System.Drawing.Size(75, 25);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSelectFolder
            // 
            this.buttonSelectFolder.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSelectFolder.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonSelectFolder.Location = new System.Drawing.Point(335, 12);
            this.buttonSelectFolder.Name = "buttonSelectFolder";
            this.buttonSelectFolder.Padding = new System.Windows.Forms.Padding(3);
            this.buttonSelectFolder.Size = new System.Drawing.Size(115, 25);
            this.buttonSelectFolder.TabIndex = 0;
            this.buttonSelectFolder.Text = "Select Folder";
            this.buttonSelectFolder.UseVisualStyleBackColor = true;
            // 
            // labelFolderBreadcrumb
            // 
            this.labelFolderBreadcrumb.AutoSize = true;
            this.labelFolderBreadcrumb.Location = new System.Drawing.Point(93, 12);
            this.labelFolderBreadcrumb.Name = "labelFolderBreadcrumb";
            this.labelFolderBreadcrumb.Padding = new System.Windows.Forms.Padding(6);
            this.labelFolderBreadcrumb.Size = new System.Drawing.Size(119, 25);
            this.labelFolderBreadcrumb.TabIndex = 2;
            this.labelFolderBreadcrumb.Text = "Path > To > Location";
            // 
            // buttonNavigateUp
            // 
            this.buttonNavigateUp.Location = new System.Drawing.Point(12, 12);
            this.buttonNavigateUp.Name = "buttonNavigateUp";
            this.buttonNavigateUp.Size = new System.Drawing.Size(75, 25);
            this.buttonNavigateUp.TabIndex = 3;
            this.buttonNavigateUp.Text = "Up";
            this.buttonNavigateUp.UseVisualStyleBackColor = true;
            this.buttonNavigateUp.Click += new System.EventHandler(this.buttonNavigateUp_Click);
            // 
            // MegaFolderSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(462, 269);
            this.Controls.Add(this.buttonNavigateUp);
            this.Controls.Add(this.labelFolderBreadcrumb);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listViewNodes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MegaFolderSelectForm";
            this.Text = "Select Folder";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewNodes;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSelectFolder;
        private System.Windows.Forms.Label labelFolderBreadcrumb;
        private System.Windows.Forms.Button buttonNavigateUp;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}