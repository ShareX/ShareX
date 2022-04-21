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
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class OCRForm : Form
    {
        public OCRLanguage Language { get; set; }
        public OCROptions Options { get; set; }
        public string Result { get; private set; }

        private Stream stream;

        public OCRForm(OCROptions options, Stream stream)
        {
            Options = options;
            this.stream = stream;

            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            cbLanguages.Items.AddRange(OCRHelper.AvailableLanguages);
            cbLanguages.SelectedIndex = 0;
            txtResult.SupportSelectAll();
        }

        private async Task OCR(string languageTag)
        {
            Result = await OCRHelper.OCR(stream, languageTag);

            if (Options.AutoCopy && !string.IsNullOrEmpty(Result))
            {
                ClipboardHelpers.CopyText(Result);
            }

            if (!IsDisposed)
            {
                txtResult.Focus();
                txtResult.Text = Result;
            }
        }

        private async void OCRForm_Shown(object sender, System.EventArgs e)
        {
            if (Options.ProcessOnLoad)
            {
                await OCR(Language.LanguageTag);
            }
        }

        private void cbLanguages_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Language = cbLanguages.SelectedItem as OCRLanguage;
        }
    }
}