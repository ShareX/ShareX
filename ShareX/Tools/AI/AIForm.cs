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
using System.Windows.Forms;

namespace ShareX
{
    public partial class AIForm : Form
    {
        public AIOptions Options { get; private set; }

        public AIForm(AIOptions options)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            Options = options;
            int index = cbModel.FindStringExact(Options.Model);
            if (index >= 0)
            {
                cbModel.SelectedIndex = index;
            }
            else
            {
                cbModel.SelectedIndex = 1;
            }
            txtAPIKey.Text = Options.ChatGPTAPIKey;
            txtInput.Text = Options.Input;
        }

        private void UpdateControls()
        {
            btnAnalyze.Enabled = !string.IsNullOrEmpty(txtAPIKey.Text) && !string.IsNullOrEmpty(txtImage.Text);
        }

        private void cbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.Model = cbModel.SelectedItem.ToString();
        }

        private void txtAPIKey_TextChanged(object sender, EventArgs e)
        {
            Options.ChatGPTAPIKey = txtAPIKey.Text;
            UpdateControls();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            Options.Input = txtInput.Text;
        }

        private void txtImage_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void btnImageBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFile(txtImage);
        }

        private async void btnAnalyze_Click(object sender, EventArgs e)
        {
            string imagePath = txtImage.Text;
            pbImage.ImageLocation = imagePath;

            if (!string.IsNullOrEmpty(Options.ChatGPTAPIKey) && !string.IsNullOrEmpty(imagePath))
            {
                btnAnalyze.Enabled = false;

                try
                {
                    ChatGPT chatGPT = new ChatGPT(Options.ChatGPTAPIKey, Options.Model);
                    string result = await chatGPT.AnalyzeImage(imagePath, Options.Input);
                    txtResult.Text = result;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    ex.ShowError();
                }
                finally
                {
                    btnAnalyze.Enabled = true;
                }
            }
        }
    }
}