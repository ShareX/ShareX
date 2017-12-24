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
    public partial class NewImageForm : Form
    {
        public Size ImageSize { get; private set; }
        public bool Transparent { get; private set; }
        public Color BackgroundColor { get; private set; }

        public NewImageForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            btnChangeColor.Color = Color.White;

            nudWidth.TextChanged += NudWidth_TextChanged;
            nudHeight.TextChanged += NudHeight_TextChanged;
        }

        public NewImageForm(Size imageSize, bool transparent, Color backgroundColor) : this()
        {
            ImageSize = imageSize;
            Transparent = transparent;
            BackgroundColor = backgroundColor;

            nudWidth.Value = ImageSize.Width;
            nudHeight.Value = ImageSize.Height;
            cbTransparent.Checked = Transparent;
            btnChangeColor.Color = BackgroundColor;
        }

        private void CheckSize()
        {
            btnOK.Enabled = nudWidth.Value > 0 && nudHeight.Value > 0;
        }

        private void NudWidth_TextChanged(object sender, EventArgs e)
        {
            CheckSize();
        }

        private void NudHeight_TextChanged(object sender, EventArgs e)
        {
            CheckSize();
        }

        private void cbTransparent_CheckedChanged(object sender, EventArgs e)
        {
            btnChangeColor.Enabled = !cbTransparent.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ImageSize = new Size((int)nudWidth.Value, (int)nudHeight.Value);
            Transparent = cbTransparent.Checked;
            BackgroundColor = btnChangeColor.Color;

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