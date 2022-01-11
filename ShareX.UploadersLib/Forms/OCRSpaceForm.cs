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
using ShareX.UploadersLib.OtherServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public enum OCRSpaceSites
    {
        [Description("Google Translate")]
        GoogleTranslate,
        [Description("Google Search")]
        GoogleSearch,
        [Description("DeepL Translate")]
        DeepL,
        [Description("Jisho")]
        Jisho,
        [Description("ichi.moe")]
        Ichi
    }

    public partial class OCRSpaceForm : Form
    {
        public OCRSpaceLanguages Language { get; set; }
        public string Result { get; private set; }

        private Stream data;
        private string fileName;
        private OCROptions ocrOptions;

        private Dictionary<OCRSpaceSites, string> defaultSiteLinks = new Dictionary<OCRSpaceSites, string>()
        {
            { OCRSpaceSites.GoogleTranslate, "https://translate.google.com/#auto/en/" },
            { OCRSpaceSites.GoogleSearch, "https://www.google.com/search?q=" },
            { OCRSpaceSites.DeepL, "https://www.deepl.com/translator#auto/en/" },
            { OCRSpaceSites.Jisho, "https://jisho.org/search/" },
            { OCRSpaceSites.Ichi, "https://ichi.moe/cl/qr/?q=" }
        };

        public OCRSpaceForm(OCROptions ocrOptions)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            this.ocrOptions = ocrOptions;
            cbLanguages.Items.AddRange(Helpers.GetEnumDescriptions<OCRSpaceLanguages>());
            cbLanguages.SelectedIndex = (int)ocrOptions.DefaultLanguage;

            cbDefaultSite.Items.AddRange(Helpers.GetEnumDescriptions<OCRSpaceSites>());
            cbDefaultSite.SelectedIndex = (int)ocrOptions.DefaultSite;

            Language = ocrOptions.DefaultLanguage;
            txtResult.SupportSelectAll();
        }

        public OCRSpaceForm(Stream data, string fileName, OCROptions ocrOptions) : this(ocrOptions)
        {
            this.data = data;
            this.fileName = fileName;
        }

        private async void OCRSpaceResultForm_Shown(object sender, EventArgs e)
        {
            UpdateControls();

            if (ocrOptions.ProcessOnLoad && string.IsNullOrEmpty(Result))
            {
                await StartOCR(data, fileName);
            }
        }

        private void UpdateControls()
        {
            cbLanguages.SelectedIndex = (int)Language;

            if (!string.IsNullOrEmpty(Result))
            {
                txtResult.Text = Result;
            }

            btnStartOCR.Visible = data != null && data.Length > 0 && !string.IsNullOrEmpty(fileName);
        }

        public async Task StartOCR(Stream stream, string fileName)
        {
            if (stream != null && stream.Length > 0 && !string.IsNullOrEmpty(fileName))
            {
                cbLanguages.Enabled = btnStartOCR.Enabled = txtResult.Enabled = btnOpenInBrowser.Enabled = cbDefaultSite.Enabled = false;
                pbProgress.Visible = true;

                Result = await OCRSpace.DoOCRAsync(Language, stream, fileName);

                if (!string.IsNullOrEmpty(Result) && ocrOptions.AutoCopy)
                {
                    ClipboardHelpers.CopyText(Result);
                }

                if (!IsDisposed)
                {
                    UpdateControls();
                    cbLanguages.Enabled = btnStartOCR.Enabled = txtResult.Enabled = btnOpenInBrowser.Enabled = cbDefaultSite.Enabled = true;
                    pbProgress.Visible = false;
                    txtResult.Focus();
                }
            }
        }

        private void cbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Language = (OCRSpaceLanguages)cbLanguages.SelectedIndex;
        }

        private async void btnStartOCR_Click(object sender, EventArgs e)
        {
            await StartOCR(data, fileName);
        }

        private void cbDefaultSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            ocrOptions.DefaultSite = (OCRSpaceSites)cbDefaultSite.SelectedIndex;
        }

        private void btnOpenInBrowser_Click(object sender, EventArgs e)
        {
            string result = txtResult.Text;

            if (!string.IsNullOrWhiteSpace(result))
            {
                string site = defaultSiteLinks[ocrOptions.DefaultSite] + URLHelpers.URLEncode(result.Trim());
                URLHelpers.OpenURL(site);
            }
        }
    }
}