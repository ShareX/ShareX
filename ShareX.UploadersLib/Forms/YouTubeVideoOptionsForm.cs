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
    public partial class YouTubeVideoOptionsForm : Form
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public YouTubeVideoPrivacy Visibility { get; private set; }

        public YouTubeVideoOptionsForm(string title = "", string description = "", YouTubeVideoPrivacy visibility = YouTubeVideoPrivacy.Private)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            Title = title;
            Description = description;
            Visibility = visibility;

            txtTitle.Text = title;
            txtDescription.Text = description;
            cbVisibility.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<YouTubeVideoPrivacy>());
            cbVisibility.SelectedIndex = (int)Visibility;
        }

        private void YouTubeVideoOptionsForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Title = txtTitle.Text;
            Description = txtDescription.Text;
            Visibility = (YouTubeVideoPrivacy)cbVisibility.SelectedIndex;

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