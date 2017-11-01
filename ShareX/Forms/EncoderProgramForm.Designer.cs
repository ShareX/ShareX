namespace ShareX
{
    partial class EncoderProgramForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EncoderProgramForm));
            this.txtExtension = new System.Windows.Forms.TextBox();
            this.lblExt = new System.Windows.Forms.Label();
            this.txtArguments = new System.Windows.Forms.TextBox();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblArgs = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.btnPathBrowse = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtExtension
            // 
            resources.ApplyResources(this.txtExtension, "txtExtension");
            this.txtExtension.Name = "txtExtension";
            // 
            // lblExt
            // 
            resources.ApplyResources(this.lblExt, "lblExt");
            this.lblExt.Name = "lblExt";
            // 
            // txtArguments
            // 
            resources.ApplyResources(this.txtArguments, "txtArguments");
            this.txtArguments.Name = "txtArguments";
            // 
            // txtPath
            // 
            resources.ApplyResources(this.txtPath, "txtPath");
            this.txtPath.Name = "txtPath";
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            // 
            // lblArgs
            // 
            resources.ApplyResources(this.lblArgs, "lblArgs");
            this.lblArgs.Name = "lblArgs";
            // 
            // lblPath
            // 
            resources.ApplyResources(this.lblPath, "lblPath");
            this.lblPath.Name = "lblPath";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // btnPathBrowse
            // 
            resources.ApplyResources(this.btnPathBrowse, "btnPathBrowse");
            this.btnPathBrowse.Name = "btnPathBrowse";
            this.btnPathBrowse.UseVisualStyleBackColor = true;
            this.btnPathBrowse.Click += new System.EventHandler(this.btnPathBrowse_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // EncoderProgramForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnPathBrowse);
            this.Controls.Add(this.txtArguments);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblArgs);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtExtension);
            this.Controls.Add(this.lblExt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "EncoderProgramForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtExtension;
        private System.Windows.Forms.Label lblExt;
        private System.Windows.Forms.TextBox txtArguments;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TextBox txtName;
        protected System.Windows.Forms.Label lblArgs;
        protected System.Windows.Forms.Label lblPath;
        protected System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnPathBrowse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}