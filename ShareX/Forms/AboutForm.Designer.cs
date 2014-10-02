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
            this.lblOwners = new System.Windows.Forms.Label();
            this.cLogo = new HelpersLib.Canvas();
            this.uclUpdate = new HelpersLib.UpdateCheckerLabel();
            this.pbMikeGooglePlus = new System.Windows.Forms.PictureBox();
            this.pbBerkSteamURL = new System.Windows.Forms.PictureBox();
            this.pbMikeURL = new System.Windows.Forms.PictureBox();
            this.pbAU = new System.Windows.Forms.PictureBox();
            this.pbBerkURL = new System.Windows.Forms.PictureBox();
            this.pbTR = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbMikeGooglePlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBerkSteamURL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMikeURL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBerkURL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTR)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProductName
            // 
            resources.ApplyResources(this.lblProductName, "lblProductName");
            this.lblProductName.BackColor = System.Drawing.Color.Transparent;
            this.lblProductName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblProductName.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Click += new System.EventHandler(this.lblProductName_Click);
            // 
            // lblBerk
            // 
            resources.ApplyResources(this.lblBerk, "lblBerk");
            this.lblBerk.BackColor = System.Drawing.Color.Transparent;
            this.lblBerk.ForeColor = System.Drawing.Color.Black;
            this.lblBerk.Name = "lblBerk";
            // 
            // lblMike
            // 
            resources.ApplyResources(this.lblMike, "lblMike");
            this.lblMike.BackColor = System.Drawing.Color.Transparent;
            this.lblMike.ForeColor = System.Drawing.Color.Black;
            this.lblMike.Name = "lblMike";
            // 
            // rtbCredits
            // 
            resources.ApplyResources(this.rtbCredits, "rtbCredits");
            this.rtbCredits.BackColor = System.Drawing.Color.WhiteSmoke;
            this.rtbCredits.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbCredits.Name = "rtbCredits";
            this.rtbCredits.ReadOnly = true;
            this.rtbCredits.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtb_LinkClicked);
            // 
            // rtbShareXInfo
            // 
            resources.ApplyResources(this.rtbShareXInfo, "rtbShareXInfo");
            this.rtbShareXInfo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.rtbShareXInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbShareXInfo.Name = "rtbShareXInfo";
            this.rtbShareXInfo.ReadOnly = true;
            this.rtbShareXInfo.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtb_LinkClicked);
            // 
            // lblOwners
            // 
            resources.ApplyResources(this.lblOwners, "lblOwners");
            this.lblOwners.Name = "lblOwners";
            // 
            // cLogo
            // 
            resources.ApplyResources(this.cLogo, "cLogo");
            this.cLogo.Interval = 100;
            this.cLogo.Name = "cLogo";
            this.cLogo.Draw += new HelpersLib.Canvas.DrawEventHandler(this.cLogo_Draw);
            this.cLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cLogo_MouseDown);
            // 
            // uclUpdate
            // 
            resources.ApplyResources(this.uclUpdate, "uclUpdate");
            this.uclUpdate.Name = "uclUpdate";
            // 
            // pbMikeGooglePlus
            // 
            resources.ApplyResources(this.pbMikeGooglePlus, "pbMikeGooglePlus");
            this.pbMikeGooglePlus.BackColor = System.Drawing.Color.Transparent;
            this.pbMikeGooglePlus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbMikeGooglePlus.Image = global::ShareX.Properties.Resources.google_plus;
            this.pbMikeGooglePlus.Name = "pbMikeGooglePlus";
            this.pbMikeGooglePlus.TabStop = false;
            this.pbMikeGooglePlus.Click += new System.EventHandler(this.pbMikeGooglePlus_Click);
            // 
            // pbBerkSteamURL
            // 
            resources.ApplyResources(this.pbBerkSteamURL, "pbBerkSteamURL");
            this.pbBerkSteamURL.BackColor = System.Drawing.Color.Transparent;
            this.pbBerkSteamURL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbBerkSteamURL.Image = global::ShareX.Properties.Resources.steam;
            this.pbBerkSteamURL.Name = "pbBerkSteamURL";
            this.pbBerkSteamURL.TabStop = false;
            this.pbBerkSteamURL.Click += new System.EventHandler(this.pbBerkSteamURL_Click);
            // 
            // pbMikeURL
            // 
            resources.ApplyResources(this.pbMikeURL, "pbMikeURL");
            this.pbMikeURL.BackColor = System.Drawing.Color.Transparent;
            this.pbMikeURL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbMikeURL.Image = global::ShareX.Properties.Resources.GitHub;
            this.pbMikeURL.Name = "pbMikeURL";
            this.pbMikeURL.TabStop = false;
            this.pbMikeURL.Click += new System.EventHandler(this.pbMikeURL_Click);
            // 
            // pbAU
            // 
            resources.ApplyResources(this.pbAU, "pbAU");
            this.pbAU.BackColor = System.Drawing.Color.Transparent;
            this.pbAU.Image = global::ShareX.Properties.Resources.au;
            this.pbAU.Name = "pbAU";
            this.pbAU.TabStop = false;
            // 
            // pbBerkURL
            // 
            resources.ApplyResources(this.pbBerkURL, "pbBerkURL");
            this.pbBerkURL.BackColor = System.Drawing.Color.Transparent;
            this.pbBerkURL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbBerkURL.Image = global::ShareX.Properties.Resources.GitHub;
            this.pbBerkURL.Name = "pbBerkURL";
            this.pbBerkURL.TabStop = false;
            this.pbBerkURL.Click += new System.EventHandler(this.pbBerkURL_Click);
            // 
            // pbTR
            // 
            resources.ApplyResources(this.pbTR, "pbTR");
            this.pbTR.BackColor = System.Drawing.Color.Transparent;
            this.pbTR.Image = global::ShareX.Properties.Resources.tr;
            this.pbTR.Name = "pbTR";
            this.pbTR.TabStop = false;
            // 
            // AboutForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.pbMikeGooglePlus);
            this.Controls.Add(this.lblOwners);
            this.Controls.Add(this.rtbShareXInfo);
            this.Controls.Add(this.rtbCredits);
            this.Controls.Add(this.pbBerkSteamURL);
            this.Controls.Add(this.lblBerk);
            this.Controls.Add(this.lblMike);
            this.Controls.Add(this.uclUpdate);
            this.Controls.Add(this.pbMikeURL);
            this.Controls.Add(this.pbAU);
            this.Controls.Add(this.pbBerkURL);
            this.Controls.Add(this.pbTR);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.cLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AboutForm";
            this.Shown += new System.EventHandler(this.AboutForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pbMikeGooglePlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBerkSteamURL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMikeURL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBerkURL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTR)).EndInit();
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
        private System.Windows.Forms.PictureBox pbBerkSteamURL;
        private System.Windows.Forms.RichTextBox rtbCredits;
        private System.Windows.Forms.RichTextBox rtbShareXInfo;
        private System.Windows.Forms.Label lblOwners;
        private HelpersLib.UpdateCheckerLabel uclUpdate;
        private System.Windows.Forms.PictureBox pbMikeGooglePlus;
    }
}