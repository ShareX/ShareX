namespace ShareX
{
    partial class AboutForm
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

            if (easterEgg != null) easterEgg.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.lblProductName = new System.Windows.Forms.Label();
            this.rtbInfo = new ShareX.HelpersLib.ReadOnlyRichTextBox();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnShareXLicense = new System.Windows.Forms.Button();
            this.btnLicenses = new System.Windows.Forms.Button();
            this.lblBuild = new System.Windows.Forms.Label();
            this.cLogo = new ShareX.HelpersLib.Canvas();
            this.uclUpdate = new ShareX.HelpersLib.UpdateCheckerLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProductName
            // 
            resources.ApplyResources(this.lblProductName, "lblProductName");
            this.lblProductName.BackColor = System.Drawing.Color.Transparent;
            this.lblProductName.Name = "lblProductName";
            // 
            // rtbInfo
            // 
            resources.ApplyResources(this.rtbInfo, "rtbInfo");
            this.rtbInfo.BackColor = System.Drawing.SystemColors.Window;
            this.rtbInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.ReadOnly = true;
            this.rtbInfo.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtb_LinkClicked);
            // 
            // pbLogo
            // 
            resources.ApplyResources(this.pbLogo, "pbLogo");
            this.pbLogo.BackColor = System.Drawing.Color.Transparent;
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.TabStop = false;
            this.pbLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbLogo_MouseDown);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnShareXLicense
            // 
            resources.ApplyResources(this.btnShareXLicense, "btnShareXLicense");
            this.btnShareXLicense.Name = "btnShareXLicense";
            this.btnShareXLicense.UseVisualStyleBackColor = true;
            this.btnShareXLicense.Click += new System.EventHandler(this.btnShareXLicense_Click);
            // 
            // btnLicenses
            // 
            resources.ApplyResources(this.btnLicenses, "btnLicenses");
            this.btnLicenses.Name = "btnLicenses";
            this.btnLicenses.UseVisualStyleBackColor = true;
            this.btnLicenses.Click += new System.EventHandler(this.btnLicenses_Click);
            // 
            // lblBuild
            // 
            resources.ApplyResources(this.lblBuild, "lblBuild");
            this.lblBuild.Name = "lblBuild";
            // 
            // cLogo
            // 
            resources.ApplyResources(this.cLogo, "cLogo");
            this.cLogo.Interval = 100;
            this.cLogo.Name = "cLogo";
            // 
            // uclUpdate
            // 
            resources.ApplyResources(this.uclUpdate, "uclUpdate");
            this.uclUpdate.Name = "uclUpdate";
            // 
            // AboutForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.cLogo);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.lblBuild);
            this.Controls.Add(this.btnLicenses);
            this.Controls.Add(this.btnShareXLicense);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.rtbInfo);
            this.Controls.Add(this.uclUpdate);
            this.Name = "AboutForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.AboutForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.Label lblProductName;
        private HelpersLib.Canvas cLogo;
        private HelpersLib.ReadOnlyRichTextBox rtbInfo;
        private HelpersLib.UpdateCheckerLabel uclUpdate;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnShareXLicense;
        private System.Windows.Forms.Button btnLicenses;
        private System.Windows.Forms.Label lblBuild;
    }
}