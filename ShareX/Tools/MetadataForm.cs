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
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class MetadataForm : Form
    {
        public static string ExifToolPath => FileHelpers.GetAbsolutePath("exiftool.exe");

        public string FilePath { get; private set; }

        private string title;

        public MetadataForm(string filePath = null)
        {
            InitializeComponent();
            rtbMetadata.AddContextMenu();
            ShareXResources.ApplyTheme(this, true);
            title = Text;

            FilePath = filePath;

            UpdateControls(false);
        }

        private void UpdateControls(bool isBusy)
        {
            btnOpen.Enabled = !isBusy;
            btnCopyAll.Enabled = !isBusy && !string.IsNullOrEmpty(rtbMetadata.Text);
            btnStripMetadata.Enabled = !isBusy && !string.IsNullOrEmpty(FilePath) && File.Exists(FilePath);

            if (!string.IsNullOrEmpty(rtbMetadata.Text) && !string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
            {
                string fileName = Path.GetFileName(FilePath);
                Text = $"{title} - {fileName}";
            }
            else
            {
                Text = title;
            }
        }

        private string GetFileMetadata(string filePath)
        {
            StringBuilder sbArguments = new StringBuilder();
            sbArguments.Append($"\"{filePath}\"");
            sbArguments.Append(" -G"); // -G[NUM...]  (-groupNames)        Print group name for each tag
            sbArguments.Append(" -t"); // -t          (-tab)               Output in tab-delimited list format
            sbArguments.Append(" -m"); // -m          (-ignoreMinorErrors) Ignore minor errors and warnings
            sbArguments.Append(" -q"); // -q          (-quiet)             Quiet processing

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ExifToolPath,
                Arguments = sbArguments.ToString(),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start exiftool process.");
                }

                using (StreamReader reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    process.WaitForExit();
                    return output;
                }
            }
        }

        private async Task<string> GetFileMetadataAsync(string filePath)
        {
            return await Task.Run(() => GetFileMetadata(filePath));
        }

        public static void StripFileMetadata(string filePath)
        {
            StringBuilder sbArguments = new StringBuilder();
            sbArguments.Append($"\"{filePath}\"");
            sbArguments.Append(" -all= "); // Removes all metadata
            sbArguments.Append(" -overwrite_original"); // -overwrite_original              Overwrite original by renaming tmp file
            sbArguments.Append(" -q"); // -q          (-quiet)             Quiet processing

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ExifToolPath,
                Arguments = sbArguments.ToString(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start exiftool process.");
                }

                string errorOutput = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"ExifTool failed to strip metadata. Error: {errorOutput}");
                }
            }
        }

        private async Task StripFileMetadataAsync(string filePath)
        {
            await Task.Run(() => StripFileMetadata(filePath));
        }

        private void PopulateMetadataRichTextBox(string metadata)
        {
            rtbMetadata.Clear();

            if (string.IsNullOrWhiteSpace(metadata))
            {
                return;
            }

            string[] lines = metadata.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            HashSet<string> addedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string line in lines)
            {
                string[] parts = line.Split(new[] { '\t' }, 3);

                if (parts.Length >= 2)
                {
                    string groupName = parts[0].Trim();

                    if (groupName.Equals("ExifTool", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string tagName = parts[1].Trim();
                    string tagValue = parts.Length == 3 ? parts[2].Trim() : string.Empty;

                    if (!addedGroups.Contains(groupName))
                    {
                        rtbMetadata.SelectionFont = new Font(rtbMetadata.Font.FontFamily, 14f, FontStyle.Bold);
                        rtbMetadata.AppendLine($"# {groupName}");
                        addedGroups.Add(groupName);
                    }

                    rtbMetadata.SelectionFont = new Font(rtbMetadata.Font.FontFamily, 10f, FontStyle.Regular);
                    rtbMetadata.AppendLine($"    {tagName}: {tagValue}");
                }
            }

            rtbMetadata.SelectionStart = 0;
        }

        public async Task<bool> LoadMetadata()
        {
            try
            {
                UpdateControls(true);

                string metadata = await GetFileMetadataAsync(FilePath);

                PopulateMetadataRichTextBox(metadata);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
                ex.ShowError();
                return false;
            }
            finally
            {
                UpdateControls(false);
            }

            return true;
        }

        private async Task OpenFile()
        {
            string filePath = FileHelpers.BrowseFile(this);

            await OpenFile(filePath);
        }

        private async Task OpenFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                FilePath = filePath;

                await LoadMetadata();
            }
        }

        private async void MetadataForm_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
            {
                await LoadMetadata();
            }
            else
            {
                await OpenFile();
            }
        }

        private void rtbMetadata_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(e.LinkText);
        }

        private async void btnOpen_Click(object sender, EventArgs e)
        {
            await OpenFile();
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(rtbMetadata.Text);
        }

        private async void btnStripMetadata_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
            {
                try
                {
                    // TODO: Translate
                    if (MessageBox.Show(this, "Are you sure you want to strip all non essential metadata from this file?", "ShareX - " + Resources.Confirmation,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        return;
                    }

                    UpdateControls(true);

                    await StripFileMetadataAsync(FilePath);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    ex.ShowError();
                    UpdateControls(false);
                    return;
                }

                TaskHelpers.PlayNotificationSoundAsync(NotificationSound.ActionCompleted);

                await LoadMetadata();
            }
        }

        private void MetadataForm_DragEnter(object sender, DragEventArgs e)
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

        private async void MetadataForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && e.Data.GetData(DataFormats.FileDrop, false) is string[] files && files.Length > 0)
            {
                await OpenFile(files[0]);
            }
        }
    }
}