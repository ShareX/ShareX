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

using ShareX.HelpersLib;
using ShareX.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ClipboardUploadForm : Form
    {
        public bool IsClipboardContentValid { get; private set; }
        public bool DontShowThisWindow { get; private set; }
        public object ClipboardContent { get; private set; }
        public bool KeepClipboardContent { get; private set; }

        private TaskSettings taskSettings;

        public ClipboardUploadForm(TaskSettings taskSettings, bool showCheckBox = false)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
            this.taskSettings = taskSettings;

            if (ShareXResources.UseCustomTheme)
            {
                lblQuestion.BackColor = ShareXResources.Theme.BorderColor;
            }

            cbDontShowThisWindow.Visible = showCheckBox;

            IsClipboardContentValid = CheckClipboardContent();
            btnUpload.Enabled = IsClipboardContentValid;
        }

        private bool CheckClipboardContent()
        {
            pbClipboard.Visible = txtClipboard.Visible = lbClipboard.Visible = false;

            if (ClipboardHelpers.ContainsImage())
            {
                using (Bitmap bmp = ClipboardHelpers.GetImage())
                {
                    if (bmp != null)
                    {
                        ClipboardContent = bmp.Clone();
                        pbClipboard.LoadImage(bmp);
                        pbClipboard.Visible = true;
                        lblQuestion.Text = string.Format(Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_content__Image__Size___0_x_1__, bmp.Width, bmp.Height);
                        return true;
                    }
                }
            }
            else if (ClipboardHelpers.ContainsText())
            {
                string text = ClipboardHelpers.GetText();

                if (!string.IsNullOrEmpty(text))
                {
                    ClipboardContent = text;
                    txtClipboard.Text = text;
                    txtClipboard.Visible = true;
                    lblQuestion.Text = string.Format(Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_content__Text__Length___0__, text.Length);
                    return true;
                }
            }
            else if (ClipboardHelpers.ContainsFileDropList())
            {
                string[] files = ClipboardHelpers.GetFileDropList();

                if (files != null && files.Length > 0)
                {
                    ClipboardContent = files;
                    lbClipboard.Items.AddRange(files);
                    lbClipboard.Visible = true;
                    lblQuestion.Text = string.Format(Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_content__File__Count___0__, files.Length);
                    return true;
                }
            }

            lblQuestion.Text = Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_is_empty_or_contains_unknown_data_;
            return false;
        }

        private void ClipboardUpload()
        {
            if (IsClipboardContentValid)
            {
                switch (ClipboardContent)
                {
                    case Bitmap bmp:
                        KeepClipboardContent = true;
                        UploadManager.ProcessImageUpload(bmp, taskSettings);
                        break;
                    case string text:
                        UploadManager.ProcessTextUpload(text, taskSettings);
                        break;
                    case string[] files:
                        UploadManager.ProcessFilesUpload(files, taskSettings);
                        break;
                }
            }
        }

        private void ClipboardContentViewer_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void ClipboardUploadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!KeepClipboardContent && ClipboardContent is Bitmap bmp)
            {
                bmp.Dispose();
            }
        }

        private void cbDontShowThisWindow_CheckedChanged(object sender, EventArgs e)
        {
            DontShowThisWindow = cbDontShowThisWindow.Checked;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            ClipboardUpload();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}