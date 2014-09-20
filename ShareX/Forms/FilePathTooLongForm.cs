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

using HelpersLib;
using System;
using System.IO;
using System.Windows.Forms;

namespace ShareX
{
	public partial class FilePathTooLongForm : Form
	{
		public string Filepath { get; private set; }

		private readonly string _initialFileNameWithoutExtension;

		private readonly string _initialFileNameWithExtension;

		private readonly string _presetShortenedName;

		public FilePathTooLongForm(string filepath)
		{
			Filepath = filepath;
			_initialFileNameWithoutExtension = Path.GetFileNameWithoutExtension(Filepath);
			_initialFileNameWithExtension = Path.GetFileName(Filepath);

			InitializeComponent();
			Icon = ShareXResources.Icon;

			txtNewName.Text = _initialFileNameWithoutExtension;

			_presetShortenedName = _initialFileNameWithExtension.Substring(0, GetMaxFileNameLengthWithoutExtension());
			btnShortenedName.Text += _presetShortenedName;
		}

		private string GetNewFilename(string newFileNameWithoutExtension)
		{
			if (!string.IsNullOrEmpty(newFileNameWithoutExtension))
			{
				return newFileNameWithoutExtension + Path.GetExtension(Filepath);
			}

			return string.Empty;
		}

		private string GetFullyQualifiedName(string newFileName)
		{
			// GetDirectoryName will throw an exception for filepaths greather than the MAX_PATH
			int directoryPathLength = Filepath.Length - _initialFileNameWithExtension.Length;
			string directoryPath = Filepath.Substring(0, directoryPathLength);

			return directoryPath + newFileName;
		}

		private int GetMaxFileNameLengthWithoutExtension()
		{
			int directoryPathAndExtensionLength = Filepath.Length - _initialFileNameWithoutExtension.Length;
			return NativeMethods.MAX_PATH - directoryPathAndExtensionLength;
		}

		private bool IsNewFileNameValid(string newFileName)
		{
			return newFileName.Length <= GetMaxFileNameLengthWithoutExtension();
		}

		private void btnNewName_Click(object sender, EventArgs e)
		{
			string newFilename = GetNewFilename(txtNewName.Text);

			if (!string.IsNullOrEmpty(newFilename))
			{
				Filepath = GetFullyQualifiedName(newFilename);
				int length = Filepath.Length;
				Close();
			}
		}

		private void txtNewName_TextChanged(object sender, EventArgs e)
		{
			string newFilename = txtNewName.Text;
			btnNewName.Enabled = !string.IsNullOrEmpty(newFilename) && IsNewFileNameValid(newFilename);
			btnNewName.Text = "Use new name: " + GetNewFilename(txtNewName.Text);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Filepath = string.Empty;
			Close();
		}

		private void btnShortenedName_Click(object sender, EventArgs e)
		{
			Filepath = GetFullyQualifiedName(
				GetNewFilename(_presetShortenedName)
			);
			int length = Filepath.Length;
			Close();
		}
	}
}