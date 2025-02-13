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
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class NewImageForm : Form
    {
        public RegionCaptureOptions Options { get; private set; }

        public NewImageForm(RegionCaptureOptions options)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            Options = options;

            nudWidth.TextChanged += NudWidth_TextChanged;
            nudHeight.TextChanged += NudHeight_TextChanged;

            nudWidth.SetValue(Options.EditorNewImageSize.Width);
            nudHeight.SetValue(Options.EditorNewImageSize.Height);
            cbTransparent.Checked = Options.EditorNewImageTransparent;
            btnChangeColor.ColorPickerOptions = options.ColorPickerOptions;
            btnChangeColor.Color = options.EditorNewImageBackgroundColor;
        }

        public static Bitmap CreateNewImage(RegionCaptureOptions options, Form form = null)
        {
            using (NewImageForm newImageForm = new NewImageForm(options))
            {
                if (newImageForm.ShowDialog(form) == DialogResult.OK)
                {
                    Color backgroundColor;

                    if (options.EditorNewImageTransparent)
                    {
                        backgroundColor = Color.Transparent;
                    }
                    else
                    {
                        backgroundColor = options.EditorNewImageBackgroundColor;
                    }

                    return ImageHelpers.CreateBitmap(options.EditorNewImageSize.Width, options.EditorNewImageSize.Height, backgroundColor);
                }
            }

            return null;
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
            Options.EditorNewImageSize = new Size((int)nudWidth.Value, (int)nudHeight.Value);
            Options.EditorNewImageTransparent = cbTransparent.Checked;
            Options.EditorNewImageBackgroundColor = btnChangeColor.Color;

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