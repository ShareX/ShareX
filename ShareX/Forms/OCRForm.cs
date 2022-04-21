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
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class OCRForm : Form
    {
        public OCROptions Options { get; set; }
        public string Result { get; private set; }

        private Stream stream;
        private bool loaded;

        public OCRForm(OCROptions options, Stream stream)
        {
            Options = options;
            this.stream = stream;

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

            txtResult.SupportSelectAll();

            loaded = true;
        }

        private async Task OCR()
        {
            if (stream != null && stream.Length > 0 && !string.IsNullOrEmpty(Options.Language))
            {
                Result = await OCRHelper.OCR(stream, Options.Language);

                if (Options.AutoCopy && !string.IsNullOrEmpty(Result))
                {
                    ClipboardHelpers.CopyText(Result);
                }

                if (!IsDisposed)
                {
                    txtResult.Text = Result;
                }
            }
        }

        private async void OCRForm_Shown(object sender, EventArgs e)
        {
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
    }
}