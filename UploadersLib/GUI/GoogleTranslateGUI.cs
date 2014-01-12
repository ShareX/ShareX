#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using System;
using System.Windows.Forms;

namespace UploadersLib
{
    public partial class GoogleTranslateGUI : Form
    {
        public GoogleTranslatorConfig Config { get; private set; }

        public GoogleTranslateGUI(GoogleTranslatorConfig config)
        {
            InitializeComponent();
            Config = config;
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            TranslateFromTextBox();
        }

        private void cbFromLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.GoogleSourceLanguage = Config.GoogleLanguages[cbFromLanguage.SelectedIndex].Language;
        }

        private void cbLanguageAutoDetect_CheckedChanged(object sender, EventArgs e)
        {
            Config.GoogleAutoDetectSource = cbLanguageAutoDetect.Checked;
        }

        private void cbToLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.GoogleTargetLanguage = Config.GoogleLanguages[cbToLanguage.SelectedIndex].Language;
        }

        private void cbAutoTranslate_CheckedChanged(object sender, EventArgs e)
        {
            Config.AutoTranslate = cbAutoTranslate.Checked;
        }

        private void txtTranslateText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                TranslateFromTextBox();
            }
        }

        private void lblToLanguage_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbToLanguage.SelectedIndex > -1)
            {
                cbToLanguage.DoDragDrop(Config.GoogleTargetLanguage, DragDropEffects.Move);
            }
        }

        private void btnTranslateTo1_DragDrop(object sender, DragEventArgs e)
        {
            Config.GoogleTargetLanguage2 = e.Data.GetData(DataFormats.Text).ToString();
            btnTranslateTo.Text = "To " + GetLanguageName(Config.GoogleTargetLanguage2);
        }

        private void btnTranslateTo_Click(object sender, EventArgs e)
        {
            TranslateTo1();
        }

        private void btnTranslateTo_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) && e.AllowedEffect == DragDropEffects.Move)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void GoogleTranslateGUI_Load(object sender, EventArgs e)
        {
            LoadSettings(Config);
        }

        private void txtAutoTranslate_TextChanged(object sender, EventArgs e)
        {
            int number;
            if (int.TryParse(txtAutoTranslate.Text, out number))
            {
                Config.AutoTranslateLength = number;
            }
        }

        private void txtGoogleApiKey_TextChanged(object sender, EventArgs e)
        {
            Config.APIKey = txtGoogleApiKey.Text;
        }
    }
}