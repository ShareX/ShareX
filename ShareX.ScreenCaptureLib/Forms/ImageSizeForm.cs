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
    public partial class ImageSizeForm : Form
    {
        public Size ImageSize { get; private set; }
        public ImageInterpolationMode InterpolationMode { get; private set; }

        private double widthRatio, heightRatio;
        private bool ignoreValueChanged = true;

        public ImageSizeForm(Size size, ImageInterpolationMode interpolationMode)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            ImageSize = size;
            InterpolationMode = interpolationMode;

            widthRatio = (double)size.Width / size.Height;
            heightRatio = (double)size.Height / size.Width;

            nudWidth.SetValue(size.Width);
            nudHeight.SetValue(size.Height);
            VerifySize();

            nudWidth.TextChanged += NudWidth_TextChanged;
            nudHeight.TextChanged += NudHeight_TextChanged;

            cbResampling.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ImageInterpolationMode>());
            cbResampling.SelectedIndex = (int)InterpolationMode;

            ignoreValueChanged = false;
        }

        private void VerifySize()
        {
            btnOK.Enabled = nudWidth.Value > 0 && nudHeight.Value > 0;
        }

        private void ApplyWidthAspectRatio()
        {
            if (!ignoreValueChanged)
            {
                if (cbAspectRatio.Checked)
                {
                    ignoreValueChanged = true;
                    nudHeight.SetValue((int)Math.Round((double)nudWidth.Value * heightRatio));
                    ignoreValueChanged = false;
                }

                VerifySize();
            }
        }

        private void ApplyHeightAspectRatio()
        {
            if (!ignoreValueChanged)
            {
                if (cbAspectRatio.Checked)
                {
                    ignoreValueChanged = true;
                    nudWidth.SetValue((int)Math.Round((double)nudHeight.Value * widthRatio));
                    ignoreValueChanged = false;
                }

                VerifySize();
            }
        }

        private void ResizeSizeForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void NudWidth_TextChanged(object sender, EventArgs e)
        {
            ApplyWidthAspectRatio();
        }

        private void NudHeight_TextChanged(object sender, EventArgs e)
        {
            ApplyHeightAspectRatio();
        }

        private void cbAspectRatio_CheckedChanged(object sender, EventArgs e)
        {
            ApplyWidthAspectRatio();
        }

        private void cbResampling_SelectedIndexChanged(object sender, EventArgs e)
        {
            InterpolationMode = (ImageInterpolationMode)cbResampling.SelectedIndex;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ImageSize = new Size((int)nudWidth.Value, (int)nudHeight.Value);

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