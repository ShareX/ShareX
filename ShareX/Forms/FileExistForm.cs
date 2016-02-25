#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
        public string Filepath { get; private set; }

        private string filename;
        private string uniqueFilepath;

        public FileExistForm(string filepath)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            Filepath = filepath;
            filename = Path.GetFileNameWithoutExtension(Filepath);
            txtNewName.Text = filename;
            btnOverwrite.Text += Path.GetFileName(Filepath);
            uniqueFilepath = Helpers.GetUniqueFilePath(Filepath);
            btnUniqueName.Text += Path.GetFileName(uniqueFilepath);
        }

        private void FileExistForm_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        private string GetNewFilename()
        {
            string newFilename = txtNewName.Text;

            if (!string.IsNullOrEmpty(newFilename))
            {
                return newFilename + Path.GetExtension(Filepath);
            }

            return string.Empty;
        }

        private void btnNewName_Click(object sender, EventArgs e)
        {
            string newFilename = GetNewFilename();

            if (!string.IsNullOrEmpty(newFilename))
            {
                Filepath = Path.Combine(Path.GetDirectoryName(Filepath), newFilename);
                Close();
            }
        }

        private void txtNewName_TextChanged(object sender, EventArgs e)
        {
            string newFilename = txtNewName.Text;
            btnNewName.Enabled = !string.IsNullOrEmpty(newFilename) && !newFilename.Equals(filename, StringComparison.InvariantCultureIgnoreCase);
            btnNewName.Text = Resources.FileExistForm_txtNewName_TextChanged_Use_new_name__ + GetNewFilename();
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUniqueName_Click(object sender, EventArgs e)
        {
            Filepath = uniqueFilepath;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Filepath = string.Empty;
            Close();
        }
    }
}