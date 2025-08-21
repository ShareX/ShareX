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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AIForm : Form
    {
        public AIOptions Options { get; private set; }

        private bool autoStart;

        public AIForm(AIOptions options)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            Options = options;
            cbModel.Text = Options.Model;
            txtAPIKey.Text = Options.ChatGPTAPIKey;
            cbInput.Text = Options.Input;
        }

        public AIForm(string filePath, AIOptions options) : this(options)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                txtImage.Text = filePath;
                pbImage.LoadImageFromFile(filePath);
                UpdateControls();
                autoStart = true;
            }
        }

        private void UpdateControls()
        {
            btnAnalyze.Enabled = !string.IsNullOrEmpty(txtAPIKey.Text) && !string.IsNullOrEmpty(txtImage.Text);
        }

        private async Task AnalyzeImage()
        {
            txtResult.Clear();
            string imagePath = txtImage.Text;
            pbImage.LoadImageFromFile(imagePath);

            if (!string.IsNullOrEmpty(Options.ChatGPTAPIKey) && !string.IsNullOrEmpty(imagePath))
            {
                btnAnalyze.Enabled = false;
                Cursor = Cursors.WaitCursor;
                txtResult.Cursor = Cursors.WaitCursor;
                // TODO: Translate
                txtResult.Text = "Thinking...";

                try
                {
                    ChatGPT chatGPT = new ChatGPT(Options.ChatGPTAPIKey, Options.Model);
                    string result = await chatGPT.AnalyzeImage(imagePath, Options.Input);
                    txtResult.Text = result.Replace("\n", "\r\n");
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    ex.ShowError();
                }
                finally
                {
                    btnAnalyze.Enabled = true;
                    Cursor = Cursors.Default;
                    txtResult.Cursor = Cursors.Default;
                }
            }
        }

        private void AIForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void AIForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && e.Data.GetData(DataFormats.FileDrop, false) is string[] files && files.Length > 0)
            {
                txtImage.Text = files[0];
            }
        }

        private async void AIForm_Shown(object sender, EventArgs e)
        {
            if (autoStart)
            {
                btnAnalyze.Focus();
                await AnalyzeImage();
            }
        }

        private void cbModel_TextChanged(object sender, EventArgs e)
        {
            Options.Model = cbModel.Text;
        }

        private void txtAPIKey_TextChanged(object sender, EventArgs e)
        {
            Options.ChatGPTAPIKey = txtAPIKey.Text;
            UpdateControls();
        }

        private void cbInput_TextChanged(object sender, EventArgs e)
        {
            Options.Input = cbInput.Text;
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
            await AnalyzeImage();
        }
    }
}