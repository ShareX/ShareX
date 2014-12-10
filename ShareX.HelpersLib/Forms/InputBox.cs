#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class InputBox : Form
    {
        public string Title { get; set; }
        public string InputText { get; set; }

        public InputBox(string title = null, string inputText = null)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            Title = title;
            InputText = inputText;

            if (!string.IsNullOrEmpty(Title)) Text = Title;
            if (!string.IsNullOrEmpty(InputText)) txtInputText.Text = InputText;
        }

        private void InputBox_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();

            txtInputText.SelectionLength = txtInputText.Text.Length;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            InputText = txtInputText.Text;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public static string GetInputText(string title = null, string inputText = null)
        {
            using (InputBox form = new InputBox(title, inputText))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    return form.InputText;
                }

                return null;
            }
        }

        #region Windows Form Designer generated code

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputBox));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInputText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            //
            // btnOK
            //
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // btnCancel
            //
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // txtInputText
            //
            resources.ApplyResources(this.txtInputText, "txtInputText");
            this.txtInputText.Name = "txtInputText";
            //
            // InputBox
            //
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtInputText);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBox";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.InputBox_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInputText;

        #endregion Windows Form Designer generated code
    }
}