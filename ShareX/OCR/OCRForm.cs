#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
            ShareXResources.ApplyTheme(this);

            OCRLanguage[] languages = OCRHelper.AvailableLanguages;

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

            if (Options.ServiceLinks != null && Options.ServiceLinks.Count > 0)
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
            btnSelectRegion.Visible = !busy;
            lblLanguage.Visible = !busy;
            cbLanguages.Visible = !busy;
            lblScaleFactor.Visible = !busy;
            nudScaleFactor.Visible = !busy;
            lblStatus.Visible = busy;
            pbStatus.Visible = busy;
        }

        private async Task OCR()
        {
            if (bmpSource != null && !string.IsNullOrEmpty(Options.Language))
            {
                busy = true;
                UpdateControls();

                Result = await OCRHelper.OCR(bmpSource, Options.Language, Options.ScaleFactor);

                if (Options.AutoCopy && !string.IsNullOrEmpty(Result))
                {
                    ClipboardHelpers.CopyText(Result);
                }

                if (!IsDisposed)
                {
                    busy = false;
                    txtResult.Text = Result;
                    UpdateControls();
                }
            }
        }

        private async void OCRForm_Shown(object sender, EventArgs e)
        {
            await OCR();
        }

        private async void btnSelectRegion_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            await Task.Delay(250);
            bmpSource?.Dispose();
            bmpSource = RegionCaptureTasks.GetRegionImage(new RegionCaptureOptions());
            WindowState = FormWindowState.Normal;

            await OCR();
        }

        private async void cbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                Options.Language = ((OCRLanguage)cbLanguages.SelectedItem).LanguageTag;

                await OCR();
            }
        }

        private async void nudScaleFactor_ValueChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                Options.ScaleFactor = (float)nudScaleFactor.Value;

                await OCR();
            }
        }

        private void cbServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.SelectedServiceLink = cbServices.SelectedIndex;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Result) && cbServices.SelectedItem is ServiceLink serviceLink)
            {
                serviceLink.OpenLink(Result);
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

        private void txtResult_TextChanged(object sender, EventArgs e)
        {
            Result = txtResult.Text.Trim();
            btnOpen.Enabled = !string.IsNullOrEmpty(Result);
        }
    }
}