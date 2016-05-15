/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
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
	partial class ResizeSettingsForm {
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
            this.buttonOK = new GreenshotPlugin.Controls.GreenshotButton();
            this.buttonCancel = new GreenshotPlugin.Controls.GreenshotButton();
            this.checkbox_aspectratio = new GreenshotPlugin.Controls.GreenshotCheckBox();
            this.label_width = new GreenshotPlugin.Controls.GreenshotLabel();
            this.label_height = new GreenshotPlugin.Controls.GreenshotLabel();
            this.textbox_height = new System.Windows.Forms.TextBox();
            this.textbox_width = new System.Windows.Forms.TextBox();
            this.combobox_width = new System.Windows.Forms.ComboBox();
            this.combobox_height = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(76, 87);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(157, 87);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkbox_aspectratio
            // 
            this.checkbox_aspectratio.AutoSize = true;
            this.checkbox_aspectratio.Location = new System.Drawing.Point(22, 64);
            this.checkbox_aspectratio.Name = "checkbox_aspectratio";
            this.checkbox_aspectratio.Size = new System.Drawing.Size(124, 17);
            this.checkbox_aspectratio.TabIndex = 5;
            this.checkbox_aspectratio.Text = "Maintain aspect ratio";
            this.checkbox_aspectratio.UseVisualStyleBackColor = true;
            // 
            // label_width
            // 
            this.label_width.AutoSize = true;
            this.label_width.Location = new System.Drawing.Point(19, 15);
            this.label_width.Name = "label_width";
            this.label_width.Size = new System.Drawing.Size(38, 13);
            this.label_width.TabIndex = 14;
            this.label_width.Text = "Width:";
            // 
            // label_height
            // 
            this.label_height.AutoSize = true;
            this.label_height.Location = new System.Drawing.Point(19, 38);
            this.label_height.Name = "label_height";
            this.label_height.Size = new System.Drawing.Size(41, 13);
            this.label_height.TabIndex = 15;
            this.label_height.Text = "Height:";
            // 
            // textbox_height
            // 
            this.textbox_height.Location = new System.Drawing.Point(90, 38);
            this.textbox_height.Name = "textbox_height";
            this.textbox_height.Size = new System.Drawing.Size(69, 20);
            this.textbox_height.TabIndex = 3;
            this.textbox_height.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyUp);
            this.textbox_height.Validating += new System.ComponentModel.CancelEventHandler(this.textbox_Validating);
            // 
            // textbox_width
            // 
            this.textbox_width.Location = new System.Drawing.Point(90, 12);
            this.textbox_width.Name = "textbox_width";
            this.textbox_width.Size = new System.Drawing.Size(69, 20);
            this.textbox_width.TabIndex = 1;
            this.textbox_width.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyUp);
            this.textbox_width.Validating += new System.ComponentModel.CancelEventHandler(this.textbox_Validating);
            // 
            // combobox_width
            // 
            this.combobox_width.FormattingEnabled = true;
            this.combobox_width.Location = new System.Drawing.Point(165, 11);
            this.combobox_width.Name = "combobox_width";
            this.combobox_width.Size = new System.Drawing.Size(65, 21);
            this.combobox_width.TabIndex = 2;
            // 
            // combobox_height
            // 
            this.combobox_height.FormattingEnabled = true;
            this.combobox_height.ItemHeight = 13;
            this.combobox_height.Location = new System.Drawing.Point(165, 38);
            this.combobox_height.Name = "combobox_height";
            this.combobox_height.Size = new System.Drawing.Size(65, 21);
            this.combobox_height.TabIndex = 4;
            // 
            // ResizeSettingsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(244, 122);
            this.ControlBox = false;
            this.Controls.Add(this.combobox_height);
            this.Controls.Add(this.combobox_width);
            this.Controls.Add(this.textbox_width);
            this.Controls.Add(this.textbox_height);
            this.Controls.Add(this.label_height);
            this.Controls.Add(this.label_width);
            this.Controls.Add(this.checkbox_aspectratio);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.LanguageKey = "";
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResizeSettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Resize settings";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private GreenshotPlugin.Controls.GreenshotButton buttonOK;
		private GreenshotPlugin.Controls.GreenshotButton buttonCancel;
		private GreenshotPlugin.Controls.GreenshotCheckBox checkbox_aspectratio;
		private GreenshotPlugin.Controls.GreenshotLabel label_width;
		private GreenshotPlugin.Controls.GreenshotLabel label_height;
		private System.Windows.Forms.TextBox textbox_height;
		private System.Windows.Forms.TextBox textbox_width;
		private System.Windows.Forms.ComboBox combobox_width;
		private System.Windows.Forms.ComboBox combobox_height;
	}
}