#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    public partial class FileExistForm : Form
    {
        public string Filepath { get; private set; }

        private string uniqueFilepath;

        public FileExistForm(string filepath)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            Filepath = filepath;
            btnOverwrite.Text += Path.GetFileName(Filepath);
            uniqueFilepath = Helpers.GetUniqueFilePath(Filepath);
            btnNewName.Text += Path.GetFileName(uniqueFilepath);
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNewName_Click(object sender, EventArgs e)
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