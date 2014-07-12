/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
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

namespace GreenshotPlugin.Controls
{
	partial class BackgroundForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
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
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label_pleasewait = new System.Windows.Forms.Label();
			this.timer_checkforclose = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// label_pleasewait
			// 
			this.label_pleasewait.AutoSize = true;
			this.label_pleasewait.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_pleasewait.Location = new System.Drawing.Point(0, 0);
			this.label_pleasewait.Name = "label_pleasewait";
			this.label_pleasewait.Padding = new System.Windows.Forms.Padding(10);
			this.label_pleasewait.Size = new System.Drawing.Size(90, 33);
			this.label_pleasewait.TabIndex = 0;
			this.label_pleasewait.Text = "Please wait...";
			this.label_pleasewait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label_pleasewait.UseWaitCursor = true;
			// 
			// timer_checkforclose
			// 
			this.timer_checkforclose.Interval = 200;
			this.timer_checkforclose.Tick += new System.EventHandler(this.Timer_checkforcloseTick);
			// 
			// BackgroundForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(169, 52);
			this.ControlBox = true;
			this.Controls.Add(this.label_pleasewait);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BackgroundForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Greenshot";
			this.TopMost = true;
			this.UseWaitCursor = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BackgroundFormFormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Timer timer_checkforclose;
		private System.Windows.Forms.Label label_pleasewait;
	}
}
