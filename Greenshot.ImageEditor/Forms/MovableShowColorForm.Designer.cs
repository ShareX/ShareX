/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015  Thomas Braun, Jens Klingen, Robin Krom
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

namespace Greenshot.Forms
{
    partial class MovableShowColorForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.html = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.preview = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.red = new System.Windows.Forms.Label();
            this.green = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.blue = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.alpha = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // html
            // 
            this.html.Location = new System.Drawing.Point(40, 18);
            this.html.Name = "html";
            this.html.Size = new System.Drawing.Size(59, 19);
            this.html.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "HTML";
            // 
            // preview
            // 
            this.preview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.preview.Location = new System.Drawing.Point(5, 5);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(32, 32);
            this.preview.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Red: ";
            // 
            // red
            // 
            this.red.Location = new System.Drawing.Point(43, 37);
            this.red.Name = "red";
            this.red.Size = new System.Drawing.Size(55, 13);
            this.red.TabIndex = 4;
            // 
            // green
            // 
            this.green.Location = new System.Drawing.Point(43, 50);
            this.green.Name = "green";
            this.green.Size = new System.Drawing.Size(55, 13);
            this.green.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Green: ";
            // 
            // blue
            // 
            this.blue.Location = new System.Drawing.Point(43, 63);
            this.blue.Name = "blue";
            this.blue.Size = new System.Drawing.Size(55, 13);
            this.blue.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Blue: ";
            // 
            // alpha
            // 
            this.alpha.Location = new System.Drawing.Point(43, 76);
            this.alpha.Name = "alpha";
            this.alpha.Size = new System.Drawing.Size(55, 13);
            this.alpha.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Alpha: ";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Info;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.alpha);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.blue);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.green);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.red);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.html);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.preview);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(100, 100);
			this.panel1.TabIndex = 0;
			// 
            // Zoomer
            //
			this.Visible = false;
			this.Location = new System.Drawing.Point(-10000,-10000);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(100, 100);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Zoomer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Zoomer";
            this.TopMost = true;
            this.panel1.ResumeLayout(true);
            this.ResumeLayout(true);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label html;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label preview;
        private System.Windows.Forms.Label alpha;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label blue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label green;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label red;
        private System.Windows.Forms.Label label2;
    }
}
