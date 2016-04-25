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

            if (bounceTimer != null) bounceTimer.Dispose();

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
            this.lblBerk = new System.Windows.Forms.Label();
            this.lblMike = new System.Windows.Forms.Label();
            this.rtbCredits = new System.Windows.Forms.RichTextBox();
            this.rtbShareXInfo = new System.Windows.Forms.RichTextBox();
            this.pbMikeURL = new System.Windows.Forms.PictureBox();
            this.pbAU = new System.Windows.Forms.PictureBox();
            this.pbBerkURL = new System.Windows.Forms.PictureBox();
            this.pbTR = new System.Windows.Forms.PictureBox();
            this.lblTeam = new System.Windows.Forms.Label();
            this.lblSteamBuild = new System.Windows.Forms.Label();
            this.pbSteam = new System.Windows.Forms.PictureBox();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnShareXLicense = new System.Windows.Forms.Button();
            this.btnLicenses = new System.Windows.Forms.Button();
            this.uclUpdate = new ShareX.HelpersLib.UpdateCheckerLabel();
            this.cLogo = new ShareX.HelpersLib.Canvas();
            ((System.ComponentModel.ISupportInitialize)(this.pbMikeURL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBerkURL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSteam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProductName
            // 
            resources.ApplyResources(this.lblProductName, "lblProductName");
            this.lblProductName.BackColor = System.Drawing.Color.Transparent;
            this.lblProductName.Name = "lblProductName";
            // 
            // lblBerk
            // 
            resources.ApplyResources(this.lblBerk, "lblBerk");
            this.lblBerk.Name = "lblBerk";
            // 
            // lblMike
            // 
            resources.ApplyResources(this.lblMike, "lblMike");
            this.lblMike.Name = "lblMike";
            // 
            // rtbCredits
            // 
            resources.ApplyResources(this.rtbCredits, "rtbCredits");
            this.rtbCredits.BackColor = System.Drawing.SystemColors.Window;
            this.rtbCredits.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbCredits.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rtbCredits.Name = "rtbCredits";
            this.rtbCredits.ReadOnly = true;
            this.rtbCredits.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtb_LinkClicked);
            // 
            // rtbShareXInfo
            // 
            this.rtbShareXInfo.BackColor = System.Drawing.SystemColors.Window;
            this.rtbShareXInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbShareXInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.rtbShareXInfo, "rtbShareXInfo");
            this.rtbShareXInfo.Name = "rtbShareXInfo";
            this.rtbShareXInfo.ReadOnly = true;
            this.rtbShareXInfo.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtb_LinkClicked);
            // 
            // pbMikeURL
            // 
            this.pbMikeURL.BackColor = System.Drawing.Color.Transparent;
            this.pbMikeURL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbMikeURL.Image = global::ShareX.Properties.Resources.GitHub;
            resources.ApplyResources(this.pbMikeURL, "pbMikeURL");
            this.pbMikeURL.Name = "pbMikeURL";
            this.pbMikeURL.TabStop = false;
            this.pbMikeURL.Click += new System.EventHandler(this.pbMikeURL_Click);
            // 
            // pbAU
            // 
            this.pbAU.BackColor = System.Drawing.Color.Transparent;
            this.pbAU.Image = global::ShareX.Properties.Resources.au;
            resources.ApplyResources(this.pbAU, "pbAU");
            this.pbAU.Name = "pbAU";
            this.pbAU.TabStop = false;
            // 
            // pbBerkURL
            // 
            this.pbBerkURL.BackColor = System.Drawing.Color.Transparent;
            this.pbBerkURL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbBerkURL.Image = global::ShareX.Properties.Resources.GitHub;
            resources.ApplyResources(this.pbBerkURL, "pbBerkURL");
            this.pbBerkURL.Name = "pbBerkURL";
            this.pbBerkURL.TabStop = false;
            this.pbBerkURL.Click += new System.EventHandler(this.pbBerkURL_Click);
            // 
            // pbTR
            // 
            this.pbTR.BackColor = System.Drawing.Color.Transparent;
            this.pbTR.Image = global::ShareX.Properties.Resources.tr;
            resources.ApplyResources(this.pbTR, "pbTR");
            this.pbTR.Name = "pbTR";
            this.pbTR.TabStop = false;
            // 
            // lblTeam
            // 
            resources.ApplyResources(this.lblTeam, "lblTeam");
            this.lblTeam.Name = "lblTeam";
            // 
            // lblSteamBuild
            // 
            resources.ApplyResources(this.lblSteamBuild, "lblSteamBuild");
            this.lblSteamBuild.BackColor = System.Drawing.Color.Transparent;
            this.lblSteamBuild.Name = "lblSteamBuild";
            // 
            // pbSteam
            // 
            this.pbSteam.BackColor = System.Drawing.Color.Transparent;
            this.pbSteam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbSteam.Image = global::ShareX.Properties.Resources.steam;
            resources.ApplyResources(this.pbSteam, "pbSteam");
            this.pbSteam.Name = "pbSteam";
            this.pbSteam.TabStop = false;
            this.pbSteam.Click += new System.EventHandler(this.pbSteam_Click);
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
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
            // uclUpdate
            // 
            resources.ApplyResources(this.uclUpdate, "uclUpdate");
            this.uclUpdate.Name = "uclUpdate";
            // 
            // cLogo
            // 
            resources.ApplyResources(this.cLogo, "cLogo");
            this.cLogo.Interval = 100;
            this.cLogo.Name = "cLogo";
            this.cLogo.Draw += new ShareX.HelpersLib.Canvas.DrawEventHandler(this.cLogo_Draw);
            this.cLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cLogo_MouseDown);
            // 
            // AboutForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.btnLicenses);
            this.Controls.Add(this.btnShareXLicense);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.pbSteam);
            this.Controls.Add(this.lblSteamBuild);
            this.Controls.Add(this.lblTeam);
            this.Controls.Add(this.rtbShareXInfo);
            this.Controls.Add(this.rtbCredits);
            this.Controls.Add(this.lblBerk);
            this.Controls.Add(this.lblMike);
            this.Controls.Add(this.uclUpdate);
            this.Controls.Add(this.pbMikeURL);
            this.Controls.Add(this.pbAU);
            this.Controls.Add(this.pbBerkURL);
            this.Controls.Add(this.pbTR);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.cLogo);
            this.MaximizeBox = false;
            this.Name = "AboutForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.AboutForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pbMikeURL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBerkURL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSteam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label lblBerk;
        private System.Windows.Forms.PictureBox pbTR;
        private System.Windows.Forms.PictureBox pbBerkURL;
        private System.Windows.Forms.PictureBox pbMikeURL;
        private System.Windows.Forms.PictureBox pbAU;
        private System.Windows.Forms.Label lblMike;
        private HelpersLib.Canvas cLogo;
        private System.Windows.Forms.RichTextBox rtbCredits;
        private System.Windows.Forms.RichTextBox rtbShareXInfo;
        private HelpersLib.UpdateCheckerLabel uclUpdate;
        private System.Windows.Forms.Label lblTeam;
        private System.Windows.Forms.Label lblSteamBuild;
        private System.Windows.Forms.PictureBox pbSteam;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnShareXLicense;
        private System.Windows.Forms.Button btnLicenses;
    }
}