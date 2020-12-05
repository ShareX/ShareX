#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ThumbnailSizeForm : Form
    {
        public Size ThumbnailSize { get; set; }

        public ThumbnailSizeForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);
        }

        public ThumbnailSizeForm(Size thumbnailSize) : this()
        {
            ThumbnailSize = thumbnailSize;
            nudWidth.SetValue(ThumbnailSize.Width);
            nudHeight.SetValue(ThumbnailSize.Height);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ThumbnailSize = new Size((int)nudWidth.Value, (int)nudHeight.Value);
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