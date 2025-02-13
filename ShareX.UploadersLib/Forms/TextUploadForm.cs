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
using System;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class TextUploadForm : Form
    {
        public string Content { get; private set; }

        public TextUploadForm(string content = null)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            if (string.IsNullOrEmpty(content) && ClipboardHelpers.ContainsText())
            {
                content = ClipboardHelpers.GetText();
            }

            if (!string.IsNullOrEmpty(content))
            {
                txtContent.Text = content;
                txtContent.SelectAll();
            }
        }

        private void TextUploadForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            Content = txtContent.Text;

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