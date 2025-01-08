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
using ShareX.MediaLib.Properties;
using System;
using System.Diagnostics;
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
        public string FilePath { get; private set; }

        public event Action<Bitmap> UploadImageRequested;
        public event Action<Bitmap> PrintImageRequested;

        private bool isReady, isBusy, isPending, pendingQuickRender;
        private string title;
        private ImageBeautifier imageBeautifier;

        private ImageBeautifierForm(ImageBeautifierOptions options = null)
        {
            Options = options;

            if (Options == null)
            {
                Options = new ImageBeautifierOptions();
            }

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
            title = Text;

            LoadOptions();
        }

        public ImageBeautifierForm(Bitmap sourceImage, ImageBeautifierOptions options = null) : this(options)
        {
            SourceImage = sourceImage;
        }

        public ImageBeautifierForm(string filePath, ImageBeautifierOptions options = null) : this(options)
        {
            FilePath = filePath;
            SourceImage = ImageHelpers.LoadImage(filePath);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                SourceImage?.Dispose();
                PreviewImage?.Dispose();
                imageBeautifier?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void LoadOptions()
        {
            isReady = false;

            tbMargin.SetValue(Options.Margin);
            tbPadding.SetValue(Options.Padding);
            cbSmartPadding.Checked = Options.SmartPadding;
            tbRoundedCorner.SetValue(Options.RoundedCorner);
            tbShadowRadius.SetValue(Options.ShadowRadius);
            tbShadowOpacity.SetValue(Options.ShadowOpacity);
            tbShadowDistance.SetValue(Options.ShadowDistance);
            tbShadowAngle.SetValue(Options.ShadowAngle);
            btnShadowColor.Color = Options.ShadowColor;
            if (cbBackgroundType.Items.Count == 0)
            {
                cbBackgroundType.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ImageBeautifierBackgroundType>());
            }
            cbBackgroundType.SelectedIndex = (int)Options.BackgroundType;
            lblBackgroundImageFilePath.Text = Options.BackgroundImageFilePath;
            UpdateUI();
            UpdateBackgroundPreview();

            isReady = true;
        }

        private void UpdateUI()
        {
            lblMarginValue.Text = tbMargin.Value.ToString() + " px";
            lblPaddingValue.Text = tbPadding.Value.ToString() + " px";
            lblRoundedCornerValue.Text = tbRoundedCorner.Value.ToString();
            lblShadowRadiusValue.Text = tbShadowRadius.Value.ToString();
            lblShadowOpacityValue.Text = tbShadowOpacity.Value.ToString() + "%";
            lblShadowDistanceValue.Text = tbShadowDistance.Value.ToString() + " px";
            lblShadowAngleValue.Text = tbShadowAngle.Value.ToString() + "°";
            lblBackgroundImageFilePath.Text = Options.BackgroundImageFilePath;
        }

        private void UpdateBackgroundPreview()
        {
            pbBackground.Image?.Dispose();
            pbBackground.Image = null;

            switch (Options.BackgroundType)
            {
                case ImageBeautifierBackgroundType.Gradient:
                    pbBackground.Visible = true;
                    lblBackgroundImageFilePath.Visible = false;
                    btnBackgroundImageFilePathBrowse.Visible = false;
                    pbBackground.Image = Options.BackgroundGradient.CreateGradientPreview(pbBackground.ClientRectangle.Width, pbBackground.ClientRectangle.Height, true, true);
                    break;
                case ImageBeautifierBackgroundType.Color:
                    pbBackground.Visible = true;
                    lblBackgroundImageFilePath.Visible = false;
                    btnBackgroundImageFilePathBrowse.Visible = false;
                    pbBackground.Image = new GradientInfo(Options.BackgroundColor).
                        CreateGradientPreview(pbBackground.ClientRectangle.Width, pbBackground.ClientRectangle.Height, true, true);
                    break;
                case ImageBeautifierBackgroundType.Image:
                    pbBackground.Visible = false;
                    lblBackgroundImageFilePath.Visible = true;
                    btnBackgroundImageFilePathBrowse.Visible = true;
                    break;
                case ImageBeautifierBackgroundType.Desktop:
                case ImageBeautifierBackgroundType.Transparent:
                    pbBackground.Visible = false;
                    lblBackgroundImageFilePath.Visible = false;
                    btnBackgroundImageFilePathBrowse.Visible = false;
                    break;
            }
        }

        private async Task UpdatePreview(bool quickRender = false)
        {
            if (isReady)
            {
                UpdateUI();

                if (isBusy)
                {
                    isPending = true;
                    pendingQuickRender = quickRender;
                }
                else
                {
                    isBusy = true;

                    UpdateOptions();

                    ImageBeautifierOptions options = Options.Copy();

                    if (quickRender)
                    {
                        options.ShadowOpacity = 0;
                    }

                    if (imageBeautifier == null)
                    {
                        imageBeautifier = new ImageBeautifier();
                        imageBeautifier.LoadImage(SourceImage);

                        if (imageBeautifier.SourceImageCropped == null)
                        {
                            cbSmartPadding.Enabled = false;
                        }
                    }

                    imageBeautifier.Options = options;

                    Stopwatch renderTime = Stopwatch.StartNew();
                    Bitmap resultImage = await imageBeautifier.RenderAsync();
                    renderTime.Stop();

                    if (IsDisposed)
                    {
                        resultImage?.Dispose();
                        return;
                    }

                    if (HelpersOptions.DevMode)
                    {
                        Text = $"{title} - Render time: {renderTime.ElapsedMilliseconds} ms";
                    }

                    PreviewImage?.Dispose();
                    PreviewImage = resultImage;
                    pbPreview.LoadImage(PreviewImage);

                    isBusy = false;

                    if (isPending)
                    {
                        isPending = false;

                        await UpdatePreview(pendingQuickRender);
                    }
                }
            }
        }

        private void UpdateOptions()
        {
            Options.Margin = tbMargin.Value;
            Options.Padding = tbPadding.Value;
            Options.SmartPadding = cbSmartPadding.Checked;
            Options.RoundedCorner = tbRoundedCorner.Value;
            Options.ShadowRadius = tbShadowRadius.Value;
            Options.ShadowOpacity = tbShadowOpacity.Value;
            Options.ShadowDistance = tbShadowDistance.Value;
            Options.ShadowAngle = tbShadowAngle.Value;
            Options.ShadowColor = btnShadowColor.Color;
        }

        private void OnUploadImageRequested()
        {
            UploadImageRequested?.Invoke(PreviewImage.CloneSafe());
        }

        private void OnPrintImageRequested()
        {
            PrintImageRequested?.Invoke(PreviewImage.CloneSafe());
        }

        private async void ImageBeautifierForm_Shown(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbMargin_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview(true);
        }

        private async void tbMargin_MouseUp(object sender, MouseEventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbPadding_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview(true);
        }

        private async void tbPadding_MouseUp(object sender, MouseEventArgs e)
        {
            await UpdatePreview();
        }

        private async void cbSmartPadding_CheckedChanged(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbRoundedCorner_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview(true);
        }

        private async void tbRoundedCorner_MouseUp(object sender, MouseEventArgs e)
        {
            await UpdatePreview();
        }

        private void btnShadowExpand_Click(object sender, EventArgs e)
        {
            if (btnShadowExpand.Tag is "+")
            {
                gbShadow.Size = new Size(gbShadow.Width, btnShadowColor.Bottom + 16);
                btnShadowExpand.Image = Resources.minus_white;
                btnShadowExpand.Tag = "-";
            }
            else
            {
                gbShadow.Size = new Size(gbShadow.Width, tbShadowRadius.Bottom + 16);
                btnShadowExpand.Image = Resources.plus_white;
                btnShadowExpand.Tag = "+";
            }
        }

        private async void tbShadowRadius_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbShadowOpacity_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbShadowDistance_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void tbShadowAngle_Scroll(object sender, EventArgs e)
        {
            await UpdatePreview();
        }

        private async void btnShadowColor_ColorChanged(Color color)
        {
            await UpdatePreview();
        }

        private async void cbBackgroundType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.BackgroundType = (ImageBeautifierBackgroundType)cbBackgroundType.SelectedIndex;
            UpdateBackgroundPreview();

            await UpdatePreview();
        }

        private async void pbBackground_Click(object sender, EventArgs e)
        {
            switch (Options.BackgroundType)
            {
                case ImageBeautifierBackgroundType.Gradient:
                    GradientInfo currentGradient = Options.BackgroundGradient;

                    using (GradientPickerForm gradientPickerForm = new GradientPickerForm(currentGradient.Copy()))
                    {
                        gradientPickerForm.GradientChanged += async () =>
                        {
                            Options.BackgroundGradient = gradientPickerForm.Gradient;
                            await UpdatePreview(true);
                        };

                        if (gradientPickerForm.ShowDialog() == DialogResult.OK)
                        {
                            Options.BackgroundGradient = gradientPickerForm.Gradient;
                            UpdateBackgroundPreview();
                        }
                        else
                        {
                            Options.BackgroundGradient = currentGradient;
                        }

                        await UpdatePreview();
                    }
                    break;
                case ImageBeautifierBackgroundType.Color:
                    if (ColorPickerForm.PickColor(Options.BackgroundColor, out Color newColor, this))
                    {
                        Options.BackgroundColor = newColor;
                        UpdateBackgroundPreview();

                        await UpdatePreview();
                    }
                    break;
            }
        }

        private async void btnBackgroundImageFilePathBrowse_Click(object sender, EventArgs e)
        {
            string filePath = ImageHelpers.OpenImageFileDialog(this);

            if (!string.IsNullOrEmpty(filePath))
            {
                Options.BackgroundImageFilePath = filePath;

                await UpdatePreview();
            }
        }

        private async void btnResetOptions_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.WouldYouLikeToResetOptions, "ShareX - " + Resources.Confirmation, MessageBoxButtons.YesNo,
                MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Options.ResetOptions();
                LoadOptions();

                await UpdatePreview();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (PreviewImage != null)
            {
                ClipboardHelpers.CopyImage(PreviewImage);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (PreviewImage != null && !string.IsNullOrEmpty(FilePath))
            {
                ImageHelpers.SaveImage(PreviewImage, FilePath);
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (PreviewImage != null)
            {
                string filePath = ImageHelpers.SaveImageFileDialog(PreviewImage, FilePath);

                if (!string.IsNullOrEmpty(filePath))
                {
                    FilePath = filePath;
                }
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (PreviewImage != null)
            {
                OnUploadImageRequested();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (PreviewImage != null)
            {
                OnPrintImageRequested();
            }
        }
    }
}