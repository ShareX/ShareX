#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using System.Windows.Forms;
using UploadersLib.OtherServices;

namespace UploadersLib
{
    public partial class GoogleTranslateGUI : Form
    {
        public void LoadSettings(GoogleTranslatorConfig config)
        {
            cbLanguageAutoDetect.Checked = config.GoogleAutoDetectSource;
            cbAutoTranslate.Checked = config.AutoTranslate;
            txtAutoTranslate.Text = config.AutoTranslateLength.ToString();

            if (Config.GoogleLanguages != null && Config.GoogleLanguages.Count > 0)
            {
                cbFromLanguage.Items.Clear();
                cbToLanguage.Items.Clear();

                foreach (GoogleLanguage lang in Config.GoogleLanguages)
                {
                    cbFromLanguage.Items.Add(lang.Name);
                    cbToLanguage.Items.Add(lang.Name);
                }

                SelectLanguage(Config.GoogleSourceLanguage, Config.GoogleTargetLanguage, Config.GoogleTargetLanguage2);

                if (cbFromLanguage.Items.Count > 0)
                {
                    cbFromLanguage.Enabled = true;
                }

                if (cbToLanguage.Items.Count > 0)
                {
                    cbToLanguage.Enabled = true;
                }
            }
        }

        public void TranslateFromTextBox()
        {
            if (!string.IsNullOrEmpty(txtTranslateText.Text))
            {
                StartBW_LanguageTranslator(new GoogleTranslateInfo
                {
                    Text = txtTranslateText.Text,
                    SourceLanguage = Config.GoogleAutoDetectSource ? null : Config.GoogleSourceLanguage,
                    TargetLanguage = Config.GoogleTargetLanguage
                });
            }
        }

        public void TranslateTo1()
        {
            if (Config.GoogleTargetLanguage2 == "?")
            {
                lblToLanguage.BorderStyle = BorderStyle.FixedSingle;
                MessageBox.Show("Drag n drop 'To:' label to this button for be able to set button language.", Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblToLanguage.BorderStyle = BorderStyle.None;
            }
            else
            {
                TranslateFromTextBox();
            }
        }

        public void SelectLanguage(string sourceLanguage, string targetLanguage, string targetLanguage2)
        {
            for (int i = 0; i < Config.GoogleLanguages.Count; i++)
            {
                if (Config.GoogleLanguages[i].Language == sourceLanguage)
                {
                    if (cbFromLanguage.Items.Count > i)
                    {
                        cbFromLanguage.SelectedIndex = i;
                    }

                    break;
                }
            }

            for (int i = 0; i < Config.GoogleLanguages.Count; i++)
            {
                if (Config.GoogleLanguages[i].Language == targetLanguage)
                {
                    if (cbToLanguage.Items.Count > i)
                    {
                        cbToLanguage.SelectedIndex = i;
                    }

                    break;
                }
            }

            btnTranslateTo.Text = "To " + GetLanguageName(targetLanguage2);
        }

        public string GetLanguageName(string language)
        {
            foreach (GoogleLanguage gl in Config.GoogleLanguages)
            {
                if (gl.Language == language) return gl.Name;
            }

            return string.Empty;
        }

        public void StartBW_LanguageTranslator(GoogleTranslateInfo gti)
        {
            btnTranslate.Enabled = false;
            btnTranslateTo.Enabled = false;
            CreateWorker().RunWorkerAsync(gti);
        }
    }
}