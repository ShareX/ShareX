namespace ShareX
{
    partial class ActionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionsForm));
            this.lblName = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.lblArgs = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.txtArguments = new System.Windows.Forms.TextBox();
            this.btnPathBrowse = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblExtensions = new System.Windows.Forms.Label();
            this.txtExtensions = new System.Windows.Forms.TextBox();
            this.txtOutputExtension = new System.Windows.Forms.TextBox();
            this.lblOutputExtension = new System.Windows.Forms.Label();
            this.cbHiddenWindow = new System.Windows.Forms.CheckBox();
            this.cbDeleteInputFile = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // lblPath
            // 
            resources.ApplyResources(this.lblPath, "lblPath");
            this.lblPath.Name = "lblPath";
            // 
            // lblArgs
            // 
            resources.ApplyResources(this.lblArgs, "lblArgs");
            this.lblArgs.Name = "lblArgs";
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            // 
            // txtPath
            // 
            resources.ApplyResources(this.txtPath, "txtPath");
            this.txtPath.Name = "txtPath";
            // 
            // txtArguments
            // 
            resources.ApplyResources(this.txtArguments, "txtArguments");
            this.txtArguments.Name = "txtArguments";
            // 
            // btnPathBrowse
            // 
            resources.ApplyResources(this.btnPathBrowse, "btnPathBrowse");
            this.btnPathBrowse.Name = "btnPathBrowse";
            this.btnPathBrowse.UseVisualStyleBackColor = true;
            this.btnPathBrowse.Click += new System.EventHandler(this.btnPathBrowse_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblExtensions
            // 
            resources.ApplyResources(this.lblExtensions, "lblExtensions");
            this.lblExtensions.Name = "lblExtensions";
            // 
            // txtExtensions
            // 
            resources.ApplyResources(this.txtExtensions, "txtExtensions");
            this.txtExtensions.Name = "txtExtensions";
            // 
            // txtOutputExtension
            // 
            resources.ApplyResources(this.txtOutputExtension, "txtOutputExtension");
            this.txtOutputExtension.Name = "txtOutputExtension";
            this.txtOutputExtension.TextChanged += new System.EventHandler(this.txtOutputExtension_TextChanged);
            // 
            // lblOutputExtension
            // 
            resources.ApplyResources(this.lblOutputExtension, "lblOutputExtension");
            this.lblOutputExtension.Name = "lblOutputExtension";
            // 
            // cbHiddenWindow
            // 
            resources.ApplyResources(this.cbHiddenWindow, "cbHiddenWindow");
            this.cbHiddenWindow.Name = "cbHiddenWindow";
            this.cbHiddenWindow.UseVisualStyleBackColor = true;
            // 
            // cbDeleteInputFile
            // 
            resources.ApplyResources(this.cbDeleteInputFile, "cbDeleteInputFile");
            this.cbDeleteInputFile.Name = "cbDeleteInputFile";
            this.cbDeleteInputFile.UseVisualStyleBackColor = true;
            // 
            // ActionsForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.cbDeleteInputFile);
            this.Controls.Add(this.cbHiddenWindow);
            this.Controls.Add(this.lblOutputExtension);
            this.Controls.Add(this.txtOutputExtension);
            this.Controls.Add(this.txtExtensions);
            this.Controls.Add(this.lblExtensions);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnPathBrowse);
            this.Controls.Add(this.txtArguments);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblArgs);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.lblName);
            this.Name = "ActionsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Label lblArgs;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TextBox txtArguments;
        private System.Windows.Forms.Button btnPathBrowse;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblExtensions;
        private System.Windows.Forms.TextBox txtExtensions;
        private System.Windows.Forms.TextBox txtOutputExtension;
        private System.Windows.Forms.Label lblOutputExtension;
        private System.Windows.Forms.CheckBox cbHiddenWindow;
        private System.Windows.Forms.CheckBox cbDeleteInputFile;
    }
}