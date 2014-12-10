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
namespace Greenshot.Forms {
	partial class DropShadowSettingsForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.thickness = new System.Windows.Forms.NumericUpDown();
            this.offsetX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.offsetY = new System.Windows.Forms.NumericUpDown();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.buttonOK = new GreenshotPlugin.Controls.GreenshotButton();
            this.buttonCancel = new GreenshotPlugin.Controls.GreenshotButton();
            this.labelDarkness = new GreenshotPlugin.Controls.GreenshotLabel();
            this.labelOffset = new GreenshotPlugin.Controls.GreenshotLabel();
            this.labelThickness = new GreenshotPlugin.Controls.GreenshotLabel();
            ((System.ComponentModel.ISupportInitialize)(this.thickness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // thickness
            // 
            this.thickness.Location = new System.Drawing.Point(173, 7);
            this.thickness.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.thickness.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.thickness.Name = "thickness";
            this.thickness.Size = new System.Drawing.Size(45, 20);
            this.thickness.TabIndex = 1;
            this.thickness.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // offsetX
            // 
            this.offsetX.Location = new System.Drawing.Point(102, 33);
            this.offsetX.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.offsetX.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.offsetX.Name = "offsetX";
            this.offsetX.Size = new System.Drawing.Size(45, 20);
            this.offsetX.TabIndex = 2;
            this.offsetX.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(153, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "X";
            // 
            // offsetY
            // 
            this.offsetY.Location = new System.Drawing.Point(173, 33);
            this.offsetY.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.offsetY.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.offsetY.Name = "offsetY";
            this.offsetY.Size = new System.Drawing.Size(45, 20);
            this.offsetY.TabIndex = 3;
            this.offsetY.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(102, 59);
            this.trackBar1.Maximum = 40;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(116, 45);
            this.trackBar1.TabIndex = 4;
            this.trackBar1.Value = 40;
            // 
            // buttonOK
            // 
            this.buttonOK.LanguageKey = "OK";
            this.buttonOK.Location = new System.Drawing.Point(62, 110);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 11;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.LanguageKey = "CANCEL";
            this.buttonCancel.Location = new System.Drawing.Point(143, 110);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelDarkness
            // 
            this.labelDarkness.AutoSize = true;
            this.labelDarkness.LanguageKey = "editor_dropshadow_darkness";
            this.labelDarkness.Location = new System.Drawing.Point(12, 73);
            this.labelDarkness.Name = "labelDarkness";
            this.labelDarkness.Size = new System.Drawing.Size(92, 13);
            this.labelDarkness.TabIndex = 13;
            this.labelDarkness.Text = "Shadow darkness";
            // 
            // labelOffset
            // 
            this.labelOffset.AutoSize = true;
            this.labelOffset.LanguageKey = "editor_dropshadow_offset";
            this.labelOffset.Location = new System.Drawing.Point(12, 35);
            this.labelOffset.Name = "labelOffset";
            this.labelOffset.Size = new System.Drawing.Size(75, 13);
            this.labelOffset.TabIndex = 14;
            this.labelOffset.Text = "Shadow offset";
            // 
            // labelThickness
            // 
            this.labelThickness.AutoSize = true;
            this.labelThickness.LanguageKey = "editor_dropshadow_thickness";
            this.labelThickness.Location = new System.Drawing.Point(12, 9);
            this.labelThickness.Name = "labelThickness";
            this.labelThickness.Size = new System.Drawing.Size(94, 13);
            this.labelThickness.TabIndex = 15;
            this.labelThickness.Text = "Shadow thickness";
            // 
            // DropShadowSettingsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(230, 144);
            this.ControlBox = false;
            this.Controls.Add(this.labelThickness);
            this.Controls.Add(this.labelOffset);
            this.Controls.Add(this.labelDarkness);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.offsetY);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.offsetX);
            this.Controls.Add(this.thickness);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.LanguageKey = "editor_dropshadow_settings";
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DropShadowSettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Drop shadow settings";
            ((System.ComponentModel.ISupportInitialize)(this.thickness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown thickness;
		private System.Windows.Forms.NumericUpDown offsetX;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown offsetY;
		private System.Windows.Forms.TrackBar trackBar1;
		private GreenshotPlugin.Controls.GreenshotButton buttonOK;
		private GreenshotPlugin.Controls.GreenshotButton buttonCancel;
		private GreenshotPlugin.Controls.GreenshotLabel labelDarkness;
		private GreenshotPlugin.Controls.GreenshotLabel labelOffset;
		private GreenshotPlugin.Controls.GreenshotLabel labelThickness;
	}
}