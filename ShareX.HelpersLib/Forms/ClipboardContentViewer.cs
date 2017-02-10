#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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

using ShareX.HelpersLib.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class ClipboardContentViewer : Form
    {
        public bool IsClipboardContentValid { get; private set; }
        public bool DontShowThisWindow { get; private set; }

        public ClipboardContentViewer(bool showCheckBox = false)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            cbDontShowThisWindow.Visible = showCheckBox;
        }

        private void ClipboardContentViewer_Load(object sender, EventArgs e)
        {
            IsClipboardContentValid = CheckClipboardContents();
        }

        private bool CheckClipboardContents()
        {
            pbClipboard.Visible = txtClipboard.Visible = lbClipboard.Visible = false;

            if (Clipboard.ContainsImage())
            {
                using (Image img = ClipboardHelpers.GetImage())
                {
                    if (img != null)
                    {
                        pbClipboard.LoadImage(img);
                        pbClipboard.Visible = true;
                        lblQuestion.Text = string.Format(Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_content__Image__Size___0_x_1__, img.Width, img.Height);
                        return true;
                    }
                }
            }
            else if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();

                if (!string.IsNullOrEmpty(text))
                {
                    txtClipboard.Text = text;
                    txtClipboard.Visible = true;
                    lblQuestion.Text = string.Format(Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_content__Text__Length___0__, text.Length);
                    return true;
                }
            }
            else if (Clipboard.ContainsFileDropList())
            {
                string[] files = Clipboard.GetFileDropList().OfType<string>().ToArray();

                if (files.Length > 0)
                {
                    lbClipboard.Items.AddRange(files);
                    lbClipboard.Visible = true;
                    lblQuestion.Text = string.Format(Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_content__File__Count___0__, files.Length);
                    return true;
                }
            }

            lblQuestion.Text = Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_is_empty_or_contains_unknown_data_;
            return false;
        }

        private void ClipboardContentViewer_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void cbDontShowThisWindow_CheckedChanged(object sender, EventArgs e)
        {
            DontShowThisWindow = cbDontShowThisWindow.Checked;
        }
    }
}