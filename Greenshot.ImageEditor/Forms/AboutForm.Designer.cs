/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2014 Thomas Braun, Jens Klingen, Robin Krom
 * 
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

namespace Greenshot {
	partial class AboutForm {
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            this.lblTitle = new System.Windows.Forms.Label();
            this.linkLblLicense = new System.Windows.Forms.LinkLabel();
            this.linkLblHost = new System.Windows.Forms.LinkLabel();
            this.linkLblBugs = new System.Windows.Forms.LinkLabel();
            this.linkLblDonations = new System.Windows.Forms.LinkLabel();
            this.linkLblIcons = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lblTranslation = new GreenshotPlugin.Controls.GreenshotLabel();
            this.lblIcons = new GreenshotPlugin.Controls.GreenshotLabel();
            this.lblDonations = new GreenshotPlugin.Controls.GreenshotLabel();
            this.lblBugs = new GreenshotPlugin.Controls.GreenshotLabel();
            this.lblHost = new GreenshotPlugin.Controls.GreenshotLabel();
            this.lblLicense = new GreenshotPlugin.Controls.GreenshotLabel();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(13, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(145, 19);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Greenshot image editor";
            // 
            // linkLblLicense
            // 
            this.linkLblLicense.Location = new System.Drawing.Point(13, 86);
            this.linkLblLicense.Name = "linkLblLicense";
            this.linkLblLicense.Size = new System.Drawing.Size(369, 23);
            this.linkLblLicense.TabIndex = 5;
            this.linkLblLicense.TabStop = true;
            this.linkLblLicense.Text = "http://www.gnu.org/licenses/gpl.html";
            this.linkLblLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // linkLblHost
            // 
            this.linkLblHost.Location = new System.Drawing.Point(13, 124);
            this.linkLblHost.Name = "linkLblHost";
            this.linkLblHost.Size = new System.Drawing.Size(465, 23);
            this.linkLblHost.TabIndex = 6;
            this.linkLblHost.TabStop = true;
            this.linkLblHost.Text = "http://sourceforge.net/projects/greenshot/";
            this.linkLblHost.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // linkLblBugs
            // 
            this.linkLblBugs.Location = new System.Drawing.Point(13, 162);
            this.linkLblBugs.Name = "linkLblBugs";
            this.linkLblBugs.Size = new System.Drawing.Size(465, 23);
            this.linkLblBugs.TabIndex = 8;
            this.linkLblBugs.TabStop = true;
            this.linkLblBugs.Text = "http://getgreenshot.org/tickets/";
            this.linkLblBugs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // linkLblDonations
            // 
            this.linkLblDonations.Location = new System.Drawing.Point(13, 201);
            this.linkLblDonations.Name = "linkLblDonations";
            this.linkLblDonations.Size = new System.Drawing.Size(465, 23);
            this.linkLblDonations.TabIndex = 10;
            this.linkLblDonations.TabStop = true;
            this.linkLblDonations.Text = "http://getgreenshot.org/support/";
            this.linkLblDonations.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // linkLblIcons
            // 
            this.linkLblIcons.Location = new System.Drawing.Point(13, 239);
            this.linkLblIcons.Name = "linkLblIcons";
            this.linkLblIcons.Size = new System.Drawing.Size(279, 23);
            this.linkLblIcons.TabIndex = 12;
            this.linkLblIcons.TabStop = true;
            this.linkLblIcons.Text = "http://p.yusukekamiyamane.com";
            this.linkLblIcons.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(156, 12);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(128, 19);
            this.linkLabel1.TabIndex = 13;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://getgreenshot.org";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // lblTranslation
            // 
            this.lblTranslation.LanguageKey = "about_translation";
            this.lblTranslation.Location = new System.Drawing.Point(12, 262);
            this.lblTranslation.Name = "lblTranslation";
            this.lblTranslation.Size = new System.Drawing.Size(466, 23);
            this.lblTranslation.TabIndex = 15;
            // 
            // lblIcons
            // 
            this.lblIcons.LanguageKey = "";
            this.lblIcons.Location = new System.Drawing.Point(12, 224);
            this.lblIcons.Name = "lblIcons";
            this.lblIcons.Size = new System.Drawing.Size(467, 23);
            this.lblIcons.TabIndex = 11;
            this.lblIcons.Text = "Icons from Yusuke Kamiyamane\'s Fugue icon set (Creative Commons Attribution 3.0 l" +
    "icense)";
            // 
            // lblDonations
            // 
            this.lblDonations.LanguageKey = "";
            this.lblDonations.Location = new System.Drawing.Point(12, 186);
            this.lblDonations.Name = "lblDonations";
            this.lblDonations.Size = new System.Drawing.Size(466, 23);
            this.lblDonations.TabIndex = 9;
            this.lblDonations.Text = "If you like Greenshot, you are welcome to support us:";
            // 
            // lblBugs
            // 
            this.lblBugs.LanguageKey = "";
            this.lblBugs.Location = new System.Drawing.Point(12, 147);
            this.lblBugs.Name = "lblBugs";
            this.lblBugs.Size = new System.Drawing.Size(466, 23);
            this.lblBugs.TabIndex = 7;
            this.lblBugs.Text = "Please report bugs to";
            // 
            // lblHost
            // 
            this.lblHost.LanguageKey = "";
            this.lblHost.Location = new System.Drawing.Point(12, 109);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(466, 23);
            this.lblHost.TabIndex = 4;
            this.lblHost.Text = "Greenshot is hosted by sourceforge.net at";
            // 
            // lblLicense
            // 
            this.lblLicense.LanguageKey = "";
            this.lblLicense.Location = new System.Drawing.Point(12, 34);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(369, 68);
            this.lblLicense.TabIndex = 3;
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(465, 268);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lblTranslation);
            this.Controls.Add(this.linkLblIcons);
            this.Controls.Add(this.lblIcons);
            this.Controls.Add(this.linkLblDonations);
            this.Controls.Add(this.lblDonations);
            this.Controls.Add(this.linkLblBugs);
            this.Controls.Add(this.lblBugs);
            this.Controls.Add(this.linkLblHost);
            this.Controls.Add(this.linkLblLicense);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Greenshot image editor";
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.LinkLabel linkLblHost;
		private System.Windows.Forms.LinkLabel linkLblDonations;
		private System.Windows.Forms.LinkLabel linkLblBugs;
		private System.Windows.Forms.LinkLabel linkLblLicense;
		private System.Windows.Forms.LinkLabel linkLblIcons;
        private System.Windows.Forms.Label lblTitle;
		private GreenshotPlugin.Controls.GreenshotLabel lblTranslation;
		private GreenshotPlugin.Controls.GreenshotLabel lblHost;
		private GreenshotPlugin.Controls.GreenshotLabel lblDonations;
		private GreenshotPlugin.Controls.GreenshotLabel lblBugs;
		private GreenshotPlugin.Controls.GreenshotLabel lblIcons;
		private GreenshotPlugin.Controls.GreenshotLabel lblLicense;
	}
}
