namespace ShareX
{
    partial class FirstTimeConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstTimeConfigForm));
            this.cbRunStartup = new ShareX.HelpersLib.BlackStyleCheckBox();
            this.cbShellContextMenuButton = new ShareX.HelpersLib.BlackStyleCheckBox();
            this.cbSendToMenu = new ShareX.HelpersLib.BlackStyleCheckBox();
            this.cbSteamInApp = new ShareX.HelpersLib.BlackStyleCheckBox();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.btnOK = new ShareX.HelpersLib.BlackStyleButton();
            this.lblNote = new ShareX.HelpersLib.BlackStyleLabel();
            this.lblTitle = new ShareX.HelpersLib.BlackStyleLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // cbRunStartup
            // 
            this.cbRunStartup.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.cbRunStartup, "cbRunStartup");
            this.cbRunStartup.ForeColor = System.Drawing.Color.White;
            this.cbRunStartup.Name = "cbRunStartup";
            this.cbRunStartup.SpaceAfterCheckBox = 3;
            this.cbRunStartup.CheckedChanged += new System.EventHandler(this.cbRunStartup_CheckedChanged);
            // 
            // cbShellContextMenuButton
            // 
            this.cbShellContextMenuButton.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.cbShellContextMenuButton, "cbShellContextMenuButton");
            this.cbShellContextMenuButton.ForeColor = System.Drawing.Color.White;
            this.cbShellContextMenuButton.Name = "cbShellContextMenuButton";
            this.cbShellContextMenuButton.SpaceAfterCheckBox = 3;
            this.cbShellContextMenuButton.CheckedChanged += new System.EventHandler(this.cbShellContextMenuButton_CheckedChanged);
            // 
            // cbSendToMenu
            // 
            this.cbSendToMenu.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.cbSendToMenu, "cbSendToMenu");
            this.cbSendToMenu.ForeColor = System.Drawing.Color.White;
            this.cbSendToMenu.Name = "cbSendToMenu";
            this.cbSendToMenu.SpaceAfterCheckBox = 3;
            this.cbSendToMenu.CheckedChanged += new System.EventHandler(this.cbSendToMenu_CheckedChanged);
            // 
            // cbSteamInApp
            // 
            this.cbSteamInApp.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.cbSteamInApp, "cbSteamInApp");
            this.cbSteamInApp.ForeColor = System.Drawing.Color.White;
            this.cbSteamInApp.Name = "cbSteamInApp";
            this.cbSteamInApp.SpaceAfterCheckBox = 3;
            this.cbSteamInApp.CheckedChanged += new System.EventHandler(this.cbSteamInApp_CheckedChanged);
            // 
            // pbLogo
            // 
            this.pbLogo.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pbLogo, "pbLogo");
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.TabStop = false;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Name = "btnOK";
            this.btnOK.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnOK_MouseClick);
            // 
            // lblNote
            // 
            this.lblNote.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblNote, "lblNote");
            this.lblNote.ForeColor = System.Drawing.Color.White;
            this.lblNote.Name = "lblNote";
            this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Name = "lblTitle";
            // 
            // SteamConfigForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.cbSteamInApp);
            this.Controls.Add(this.cbSendToMenu);
            this.Controls.Add(this.cbShellContextMenuButton);
            this.Controls.Add(this.cbRunStartup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FirstTimeConfigForm";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private HelpersLib.BlackStyleCheckBox cbRunStartup;
        private HelpersLib.BlackStyleCheckBox cbShellContextMenuButton;
        private HelpersLib.BlackStyleCheckBox cbSendToMenu;
        private HelpersLib.BlackStyleCheckBox cbSteamInApp;
        private System.Windows.Forms.PictureBox pbLogo;
        private HelpersLib.BlackStyleButton btnOK;
        private HelpersLib.BlackStyleLabel lblNote;
        private HelpersLib.BlackStyleLabel lblTitle;
    }
}