#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HelpersLib
{
    public partial class ClipboardContentViewer : Form
    {
        public bool IsClipboardEmpty { get; private set; }

        public bool DontShowThisWindow { get; private set; }

        public ClipboardContentViewer(bool showCheckBox = false)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            cbDontShowThisWindow.Visible = showCheckBox;
        }

        private void ClipboardContentViewer_Load(object sender, EventArgs e)
        {
            pbClipboard.Visible = txtClipboard.Visible = lbClipboard.Visible = false;

            if (Clipboard.ContainsImage())
            {
                using (Image img = Clipboard.GetImage())
                {
                    pbClipboard.LoadImage(img);
                    lblQuestion.Text = string.Format("Clipboard content: Image (Size: {0}x{1})", img.Width, img.Height);
                }

                pbClipboard.Visible = true;
            }
            else if (!string.IsNullOrEmpty(txtClipboard.Text))
            {
                lblQuestion.Text = string.Format("Content: Text (Length: {0})", txtClipboard.Text.Length);
                txtClipboard.Visible = true;
            }
            else if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                lblQuestion.Text = string.Format("Clipboard content: Text (Length: {0})", text.Length);
                txtClipboard.Text = text;
                txtClipboard.Visible = true;
            }
            else if (Clipboard.ContainsFileDropList())
            {
                string[] files = Clipboard.GetFileDropList().OfType<string>().ToArray();
                lblQuestion.Text = string.Format("Clipboard content: File (Count: {0})", files.Length);
                lbClipboard.Items.AddRange(files);
                lbClipboard.Visible = true;
            }
            else
            {
                lblQuestion.Text = "Clipboard is empty or contains unknown data.";
                IsClipboardEmpty = true;
            }
        }

        private void ClipboardContentViewer_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cbDontShowThisWindow_CheckedChanged(object sender, EventArgs e)
        {
            DontShowThisWindow = cbDontShowThisWindow.Checked;
        }
    }
}