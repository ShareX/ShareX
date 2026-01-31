#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AIForm : Form
    {
        public AIOptions Options { get; private set; }

        public AIForm(AIOptions options)
        {
            Options = options;
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            txtInput.Text = Options.Input;

            // Add preset prompts to context menu
            AddPresetPrompt("What is in this image?");
            AddPresetPrompt("Thoroughly describe this image.");
            AddPresetPrompt("Transcribe the image's text. Do not write anything else.");
            AddPresetPrompt("Translate this text into English. Do not write anything else.");
        }

        private void AddPresetPrompt(string prompt)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(prompt);
            item.Click += (s, e) => txtInput.Text = prompt;
            cmsPresets.Items.Add(item);
        }

        public AIForm(string filePath, AIOptions options) : this(options)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                txtImage.Text = filePath;
                pbImage.LoadImageFromFile(filePath);
                UpdateControls();
            }
        }

        private void UpdateControls()
        {
            btnAnalyze.Enabled = IsAPIKeyAvailable() && (!string.IsNullOrEmpty(txtImage.Text) || pbImage.Image != null);
            btnResultCopy.Enabled = !string.IsNullOrEmpty(txtResult.Text);
        }

        private bool IsAPIKeyAvailable()
        {
            switch (Options.Provider)
            {
                case AIProvider.OpenAI:
                case AIProvider.Custom:
                    return !string.IsNullOrEmpty(Options.OpenAIAPIKey);
                case AIProvider.Gemini:
                    return !string.IsNullOrEmpty(Options.GeminiAPIKey);
                case AIProvider.OpenRouter:
                    return !string.IsNullOrEmpty(Options.OpenRouterAPIKey);
                default:
                    return false;
            }
        }

        private async Task AnalyzeImage()
        {
            txtResult.Clear();
            lblTimer.ResetText();

            if (IsAPIKeyAvailable() && (!string.IsNullOrEmpty(txtImage.Text) || pbImage.Image != null))
            {
                btnAnalyze.Enabled = false;
                Cursor = Cursors.WaitCursor;
                txtResult.Cursor = Cursors.WaitCursor;
                // TODO: Translate
                txtResult.Text = "Thinking...";
                Stopwatch timer = Stopwatch.StartNew();

                try
                {
                    IAIProvider provider = AIProviderFactory.GetProvider(Options);
                    string result = null;
                    string imagePath = txtImage.Text;
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        result = await provider.AnalyzeImage(imagePath, Options.Input, Options.OpenAIReasoningEffort, Options.OpenAIVerbosity);
                    }
                    else if (pbImage.Image != null)
                    {
                        result = await provider.AnalyzeImage(pbImage.Image, Options.Input, Options.OpenAIReasoningEffort, Options.OpenAIVerbosity);
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Replace("\n", "\r\n");
                        // TODO: Translate
                        lblTimer.Text = $"Time: {timer.ElapsedMilliseconds} ms";
                        txtResult.Text = result;
                        if (Options.AutoCopyResult)
                        {
                            ClipboardHelpers.CopyText(result);
                        }
                        TaskHelpers.PlayNotificationSoundAsync(NotificationSound.ActionCompleted);
                    }
                }
                catch (Exception ex)
                {
                    txtResult.Clear();
                    DebugHelper.WriteException(ex);
                    ex.ShowError();
                }
                finally
                {
                    UpdateControls();
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

        private void AIForm_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtImage.Text) && Options.AutoStartRegion)
            {
                Bitmap regionImage = RegionCaptureTasks.GetRegionImage();

                if (regionImage != null)
                {
                    pbImage.LoadImage(regionImage);
                    UpdateControls();
                }
            }
        }

        private async void AIForm_Shown(object sender, EventArgs e)
        {
            if (Options.AutoStartAnalyze)
            {
                btnAnalyze.Focus();
                await AnalyzeImage();
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            using (AIOptionsForm optionsForm = new AIOptionsForm(Options))
            {
                optionsForm.ShowDialog(this);
                UpdateControls();
            }
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
            if (FileHelpers.BrowseFile(txtImage))
            {
                pbImage.LoadImageFromFile(txtImage.Text);
            }
        }

        private async void btnAnalyze_Click(object sender, EventArgs e)
        {
            await AnalyzeImage();
        }

        private async void btnCapture_Click(object sender, EventArgs e)
        {
            FormWindowState previousState = WindowState;
            WindowState = FormWindowState.Minimized;
            await Task.Delay(250);
            Bitmap regionImage = RegionCaptureTasks.GetRegionImage();
            WindowState = previousState;

            if (regionImage != null)
            {
                pbImage.LoadImage(regionImage);
                txtImage.ResetText();
                await AnalyzeImage();
            }
        }

        private void btnResultCopy_Click(object sender, EventArgs e)
        {
            if (ClipboardHelpers.CopyText(txtResult.Text))
            {
                TaskHelpers.PlayNotificationSoundAsync(NotificationSound.ActionCompleted);
            }
        }
    }
}