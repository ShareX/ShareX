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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class ImageSizeForm : Form
    {
        public Size Result { get; private set; }

        public ImageSizeForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
        }

        public ImageSizeForm(Size size) : this()
        {
            Result = size;
            nudWidth.SetValue(size.Width);
            nudHeight.SetValue(size.Height);
        }

        private void VerifySize()
        {
            btnOK.Enabled = nudWidth.Value > 0 && nudHeight.Value > 0;
        }

        private void ResizeSizeForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void nudWidth_ValueChanged(object sender, EventArgs e)
        {
            VerifySize();
        }

        private void nudHeight_ValueChanged(object sender, EventArgs e)
        {
            VerifySize();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Result = new Size((int)nudWidth.Value, (int)nudHeight.Value);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
