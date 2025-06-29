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
            lblProductName = new System.Windows.Forms.Label();
            rtbInfo = new ShareX.HelpersLib.ReadOnlyRichTextBox();
            pbLogo = new System.Windows.Forms.PictureBox();
            lblBuild = new System.Windows.Forms.Label();
            cLogo = new ShareX.HelpersLib.Canvas();
            uclUpdate = new ShareX.HelpersLib.UpdateCheckerLabel();
            pLogo = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)pbLogo).BeginInit();
            pLogo.SuspendLayout();
            SuspendLayout();
            // 
            // lblProductName
            // 
            resources.ApplyResources(lblProductName, "lblProductName");
            lblProductName.BackColor = System.Drawing.Color.Transparent;
            lblProductName.Name = "lblProductName";
            // 
            // rtbInfo
            // 
            rtbInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(rtbInfo, "rtbInfo");
            rtbInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            rtbInfo.Name = "rtbInfo";
            rtbInfo.ReadOnly = true;
            rtbInfo.LinkClicked += rtb_LinkClicked;
            // 
            // pbLogo
            // 
            pbLogo.Image = Properties.Resources.About_Logo;
            resources.ApplyResources(pbLogo, "pbLogo");
            pbLogo.Name = "pbLogo";
            pbLogo.TabStop = false;
            pbLogo.MouseDown += pbLogo_MouseDown;
            // 
            // lblBuild
            // 
            resources.ApplyResources(lblBuild, "lblBuild");
            lblBuild.Name = "lblBuild";
            // 
            // cLogo
            // 
            resources.ApplyResources(cLogo, "cLogo");
            cLogo.Interval = 100;
            cLogo.Name = "cLogo";
            // 
            // uclUpdate
            // 
            resources.ApplyResources(uclUpdate, "uclUpdate");
            uclUpdate.Name = "uclUpdate";
            // 
            // pLogo
            // 
            pLogo.BackColor = System.Drawing.Color.FromArgb(35, 35, 35);
            pLogo.Controls.Add(pbLogo);
            pLogo.Controls.Add(cLogo);
            resources.ApplyResources(pLogo, "pLogo");
            pLogo.Name = "pLogo";
            // 
            // AboutForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(rtbInfo);
            Controls.Add(uclUpdate);
            Controls.Add(lblBuild);
            Controls.Add(lblProductName);
            Controls.Add(pLogo);
            ForeColor = System.Drawing.Color.FromArgb(231, 233, 234);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "AboutForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Shown += AboutForm_Shown;
            ((System.ComponentModel.ISupportInitialize)pbLogo).EndInit();
            pLogo.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.Label lblProductName;
        private HelpersLib.Canvas cLogo;
        private HelpersLib.ReadOnlyRichTextBox rtbInfo;
        private HelpersLib.UpdateCheckerLabel uclUpdate;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Label lblBuild;
        private System.Windows.Forms.Panel pLogo;
    }
}