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
using System.Windows.Forms;

namespace ShareX
{
    public partial class ActionsForm : Form
    {
        public ExternalProgram FileAction { get; private set; }

        public ActionsForm()
            : this(new ExternalProgram())
        {
        }

        public ActionsForm(ExternalProgram fileAction)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            FileAction = fileAction;
            txtName.Text = fileAction.Name ?? "";
            txtPath.Text = fileAction.Path ?? "";
            txtArguments.Text = fileAction.Args ?? "";
            CodeMenu.Create<ActionsCodeMenuEntry>(txtArguments);
            txtOutputExtension.Text = fileAction.OutputExtension ?? "";
            txtExtensions.Text = fileAction.Extensions ?? "";
        }

        private void btnPathBrowse_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFile("ShareX - Choose file path", txtPath);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Name can't be empty.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtPath.Text))
            {
                MessageBox.Show("File path can't be empty.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FileAction.Name = txtName.Text;
            FileAction.Path = txtPath.Text;
            FileAction.Args = txtArguments.Text;
            FileAction.Extensions = txtExtensions.Text;
            FileAction.OutputExtension = txtOutputExtension.Text;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}