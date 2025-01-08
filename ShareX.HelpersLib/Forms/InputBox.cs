#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class InputBox : Form
    {
        public string InputText { get; private set; }

        private InputBox(string title, string inputText = null, string okText = null, string cancelText = null)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            InputText = inputText;

            Text = "ShareX - " + title;
            if (!string.IsNullOrEmpty(InputText)) txtInputText.Text = InputText;
            if (!string.IsNullOrEmpty(okText)) btnOK.Text = okText;
            if (!string.IsNullOrEmpty(cancelText)) btnCancel.Text = cancelText;
        }

        public static string Show(string title, string inputText = null, string okText = null, string cancelText = null)
        {
            using (InputBox form = new InputBox(title, inputText, okText, cancelText))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    return form.InputText;
                }

                return null;
            }
        }

        private void InputBox_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
            MinimumSize = new Size(384, Size.Height);
            MaximumSize = new Size(1000, Size.Height);

            txtInputText.SelectionLength = txtInputText.Text.Length;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            InputText = txtInputText.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
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
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            txtInputText = new System.Windows.Forms.TextBox();
            SuspendLayout();
            //
            // btnOK
            //
            resources.ApplyResources(btnOK, "btnOK");
            btnOK.Name = "btnOK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += new System.EventHandler(btnOK_Click);
            //
            // btnCancel
            //
            resources.ApplyResources(btnCancel, "btnCancel");
            btnCancel.Name = "btnCancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += new System.EventHandler(btnCancel_Click);
            //
            // txtInputText
            //
            resources.ApplyResources(txtInputText, "txtInputText");
            txtInputText.Name = "txtInputText";
            //
            // InputBox
            //
            AcceptButton = btnOK;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(txtInputText);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputBox";
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            TopMost = true;
            Shown += new System.EventHandler(InputBox_Shown);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInputText;

        #endregion Windows Form Designer generated code
    }
}