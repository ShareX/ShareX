#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public partial class ImageBeautifierForm : Form
    {
        public Bitmap SourceImage { get; private set; }
        public Bitmap PreviewImage { get; private set; }
        public ImageBeautifierOptions Options { get; private set; }

        private bool isReady, isBusy, isPending;

        public ImageBeautifierForm(Bitmap sourceImage, ImageBeautifierOptions options = null)
        {
            SourceImage = sourceImage;
            Options = options;

            if (Options == null)
            {
                Options = new ImageBeautifierOptions();
            }

            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            tbMargin.SetValue(Options.Margin);
            tbPadding.SetValue(Options.Padding);
            cbSmartPadding.Checked = Options.SmartPadding;
            tbRoundedCorner.SetValue(Options.RoundedCorner);
            tbShadowSize.SetValue(Options.ShadowSize);
            UpdateUI();
            UpdateBackgroundPreview();

            isReady = true;
        }

        private void UpdateUI()
        {
            lblMarginValue.Text = tbMargin.Value.ToString();
            lblPaddingValue.Text = tbPadding.Value.ToString();
            lblRoundedCornerValue.Text = tbRoundedCorner.Value.ToString();
            lblShadowSizeValue.Text = tbShadowSize.Value.ToString();
        }

        private void UpdateBackgroundPreview()
        {
            pbBackground.Image?.Dispose();
            pbBackground.Image = Options.Background.CreateGradientPreview(pbBackground.ClientRectangle.Width, pbBackground.ClientRectangle.Height, true, true);
        }

        private async Task UpdatePreview()
        {
            if (isReady)
            {
                UpdateUI();

                if (isBusy)
                {
                    isPending = true;
                }
                else
                {
                    isBusy = true;

                    UpdateOptions();

                    Bitmap resultImage = await RenderPreview(SourceImage, Options);
                    PreviewImage?.Dispose();
                    PreviewImage = resultImage;
                    pbPreview.LoadImage(PreviewImage);

                    isBusy = false;

                    if (isPending)
                    {
                        isPending = false;

                        await UpdatePreview();
                    }
                }
            }
        }

        private static async Task<Bitmap> RenderPreview(Bitmap sourceImage, ImageBeautifierOptions options)
        {
            return await Task.Run(() =>
            {
                Bitmap resultImage = (Bitmap)sourceImage.Clone();

                if (options.SmartPadding)
                {
                    resultImage = ImageHelpers.AutoCropImage(resultImage, true, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, options.Padding);
                }
                else if (options.Padding > 0)
                {
                    Color color = resultImage.GetPixel(0, 0);
                    Bitmap resultImageNew = ImageHelpers.AddCanvas(resultImage, options.Padding, color);
                    resultImage.Dispose();
                    resultImage = resultImageNew;
                }

                if (options.RoundedCorner > 0)
                {
                    resultImage = ImageHelpers.RoundedCorners(resultImage, options.RoundedCorner);
                }

                if (options.Margin > 0)
                {
                    Bitmap resultImageNew = ImageHelpers.AddCanvas(resultImage, options.Margin);
                    resultImage.Dispose();
                    resultImage = resultImageNew;
                }

                if (options.ShadowSize > 0)
                {
                    resultImage = ImageHelpers.AddShadow(resultImage, 1f, options.ShadowSize, 0f, Color.Black, new Point(0, 0), false);
                }

                if (options.Background != null)
                {
                    Bitmap resultImageNew = ImageHelpers.FillBackground(resultImage, options.Background);
                    resultImage.Dispose();
                    resultImage = resultImageNew;
                }

                return resultImage;
            });
        }

        private void UpdateOptions()
        {
            Options.Margin = tbMargin.Value;
            Options.Padding = tbPadding.Value;
            Options.SmartPadding = cbSmartPadding.Checked;
            Options.RoundedCorner = tbRoundedCorner.Value;
            Options.ShadowSize = tbShadowSize.Value;
        }

        private async void ImageBeautifierForm_Shown(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbMargin_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbPadding_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void cbSmartPadding_CheckedChanged(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbRoundedCorner_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbShadowSize_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void pbBackground_Click(object sender, EventArgs e)
        {
            using (GradientPickerForm gradientPickerForm = new GradientPickerForm(Options.Background.Copy()))
            {
                if (gradientPickerForm.ShowDialog() == DialogResult.OK)
                {
                    Options.Background = gradientPickerForm.Gradient;
                    UpdateBackgroundPreview();

                    await UpdatePreview();
                }
            }
        }
    }
}