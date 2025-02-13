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
using System.IO;
using System.Windows.Forms;

namespace ShareX
{
    public partial class FileExistForm : Form
    {
        public string FilePath { get; private set; }

        private string fileName;
        private string uniqueFilePath;

        public FileExistForm(string filePath)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            FilePath = filePath;
            fileName = Path.GetFileNameWithoutExtension(FilePath);
            txtNewName.Text = fileName;
            btnOverwrite.Text += Path.GetFileName(FilePath);
            uniqueFilePath = FileHelpers.GetUniqueFilePath(FilePath);
            btnUniqueName.Text += Path.GetFileName(uniqueFilePath);
        }

        private void FileExistForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private string GetNewFileName()
        {
            string newFileName = txtNewName.Text;

            if (!string.IsNullOrEmpty(newFileName))
            {
                return newFileName + Path.GetExtension(FilePath);
            }

            return "";
        }

        private void UseNewFileName()
        {
            string newFileName = GetNewFileName();

            if (!string.IsNullOrEmpty(newFileName))
            {
                FilePath = Path.Combine(Path.GetDirectoryName(FilePath), newFileName);
                Close();
            }
        }

        private void UseUniqueFileName()
        {
            FilePath = uniqueFilePath;
            Close();
        }

        private void Cancel()
        {
            FilePath = "";
            Close();
        }

        private void txtNewName_TextChanged(object sender, EventArgs e)
        {
            string newFileName = txtNewName.Text;
            btnNewName.Enabled = !string.IsNullOrEmpty(newFileName) && !newFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase);
            btnNewName.Text = Resources.FileExistForm_txtNewName_TextChanged_Use_new_name__ + GetNewFileName();
        }

        private void txtNewName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Escape)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void txtNewName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                string newFileName = txtNewName.Text;

                if (!string.IsNullOrEmpty(newFileName))
                {
                    if (newFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        Close();
                    }
                    else
                    {
                        UseNewFileName();
                    }
                }
            }
            else if (e.KeyData == Keys.Escape)
            {
                Cancel();
            }
        }

        private void btnNewName_Click(object sender, EventArgs e)
        {
            UseNewFileName();
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUniqueName_Click(object sender, EventArgs e)
        {
            UseUniqueFileName();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }
    }
}