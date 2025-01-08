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
using ShareX.ScreenCaptureLib;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class OCRForm : Form
    {
        public OCROptions Options { get; set; }
        public string Result { get; private set; }

        private Bitmap bmpSource;
        private bool loaded;
        private bool busy;

        public OCRForm(Bitmap bmp, OCROptions options)
        {
            bmpSource = (Bitmap)bmp.Clone();
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            OCRLanguage[] languages = OCRHelper.AvailableLanguages.OrderBy(x => x.DisplayName).ToArray();

            if (languages.Length > 0)
            {
                cbLanguages.Items.AddRange(languages);

                if (Options.Language == null)
                {
                    cbLanguages.SelectedIndex = 0;
                    Options.Language = languages[0].LanguageTag;
                }
                else
                {
                    int index = Array.FindIndex(languages, x => x.LanguageTag.Equals(Options.Language, StringComparison.OrdinalIgnoreCase));

                    if (index >= 0)
                    {
                        cbLanguages.SelectedIndex = index;
                    }
                    else
                    {
                        cbLanguages.SelectedIndex = 0;
                        Options.Language = languages[0].LanguageTag;
                    }
                }
            }
            else
            {
                cbLanguages.Enabled = false;
            }

            nudScaleFactor.SetValue((decimal)Options.ScaleFactor);
            cbSingleLine.Checked = Options.SingleLine;

            if (Helpers.IsDefaultSettings(Options.ServiceLinks, OCROptions.DefaultServiceLinks, (x, y) => x.Name == y.Name))
            {
                Options.ServiceLinks = OCROptions.DefaultServiceLinks;
            }

            if (Options.ServiceLinks.Count > 0)
            {
                cbServices.Items.AddRange(Options.ServiceLinks.ToArray());
                cbServices.SelectedIndex = Options.SelectedServiceLink;
            }
            else
            {
                cbServices.Enabled = false;
            }

            txtResult.SupportSelectAll();
            UpdateControls();

            loaded = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                bmpSource?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateControls()
        {
            if (busy)
            {
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                Cursor = Cursors.Default;
            }

            btnSelectRegion.Enabled = !busy;
            cbLanguages.Enabled = !busy;
            nudScaleFactor.Enabled = !busy;
            cbSingleLine.Enabled = !busy;
        }

        private async Task OCR(Bitmap bmp)
        {
            if (bmp != null && !string.IsNullOrEmpty(Options.Language))
            {
                busy = true;
                txtResult.Text = "";
                UpdateControls();

                try
                {
                    Result = await OCRHelper.OCR(bmp, Options.Language, Options.ScaleFactor, Options.SingleLine);

                    if (Options.AutoCopy && !string.IsNullOrEmpty(Result))
                    {
                        ClipboardHelpers.CopyText(Result);
                    }
                }
                catch (Exception e)
                {
                    e.ShowError(false);
                }

                if (!IsDisposed)
                {
                    busy = false;
                    txtResult.Text = Result;
                    txtResult.Focus();
                    txtResult.DeselectAll();
                    UpdateControls();
                }
            }
        }

        private async void OCRForm_Shown(object sender, EventArgs e)
        {
            await OCR(bmpSource);
        }

        private async void btnSelectRegion_Click(object sender, EventArgs e)
        {
            FormWindowState previousState = WindowState;
            WindowState = FormWindowState.Minimized;
            await Task.Delay(250);
            Bitmap regionImage = RegionCaptureTasks.GetRegionImage();
            WindowState = previousState;

            if (regionImage != null)
            {
                bmpSource?.Dispose();
                bmpSource = regionImage;
                await Task.Delay(250);
                await OCR(bmpSource);
            }
        }

        private async void cbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                Options.Language = ((OCRLanguage)cbLanguages.SelectedItem).LanguageTag;

                await OCR(bmpSource);
            }
        }

        private void btnOpenOCRHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.DocsOCR);
        }

        private async void nudScaleFactor_ValueChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                Options.ScaleFactor = (float)nudScaleFactor.Value;

                await OCR(bmpSource);
            }
        }

        private async void cbSingleLine_CheckedChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                Options.SingleLine = cbSingleLine.Checked;

                await OCR(bmpSource);
            }
        }

        private void cbServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.SelectedServiceLink = cbServices.SelectedIndex;
        }

        private void btnOpenServiceLink_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Result) && cbServices.SelectedItem is ServiceLink serviceLink)
            {
                serviceLink.OpenLink(Result);

                if (Options.CloseWindowAfterOpeningServiceLink)
                {
                    Close();
                }
            }
        }

        private void cbEditServices_Click(object sender, EventArgs e)
        {
            using (ServiceLinksForm form = new ServiceLinksForm(Options.ServiceLinks))
            {
                form.ShowDialog();

                cbServices.Items.Clear();

                if (Options.ServiceLinks.Count > 0)
                {
                    cbServices.Items.AddRange(Options.ServiceLinks.ToArray());
                    cbServices.SelectedIndex = 0;
                    Options.SelectedServiceLink = 0;
                }

                cbServices.Enabled = cbServices.Items.Count > 0;
            }
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(txtResult.Text);
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {
            Result = txtResult.Text.Trim();
            btnOpenServiceLink.Enabled = btnCopyAll.Enabled = !string.IsNullOrEmpty(Result);
        }
    }
}