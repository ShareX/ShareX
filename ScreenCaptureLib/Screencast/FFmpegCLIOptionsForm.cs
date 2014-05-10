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

using HelpersLib;
using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public partial class FFmpegCLIOptionsForm : Form
    {
        private ScreencastOptions Options = null;

        public FFmpegCLIOptionsForm()
        {
            InitializeComponent();

            this.Text = string.Format("{0} - FFmpeg CLI Options", Application.ProductName);
            this.Icon = ShareXResources.Icon;
        }

        public FFmpegCLIOptionsForm(ScreencastOptions options)
            : this()
        {
            Options = options;

            if (options != null)
            {
                LoadSettings();
                UpdateUI();
            }
        }

        private void LoadSettings()
        {
            comboBoxCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegVideoCodec>());
            comboBoxCodec.SelectedIndex = (int)Options.FFmpeg.VideoCodec;
            comboBoxCodec.SelectedIndexChanged += (sender, e) => UpdateUI();

            comboBoxExtension.Text = Options.FFmpeg.Extension;
            comboBoxExtension.SelectedIndexChanged += (sender, e) => UpdateUI();

            nudCRF.Value = Options.FFmpeg.CRF.Between((int)nudCRF.Minimum, (int)nudCRF.Maximum);
            nudCRF.ValueChanged += (sender, e) => UpdateUI();

            comboBoxPreset.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPreset>());
            comboBoxPreset.SelectedIndex = (int)Options.FFmpeg.Preset;
            comboBoxPreset.SelectedIndexChanged += (sender, e) => UpdateUI();

            nudQscale.Value = Options.FFmpeg.qscale.Between((int)nudQscale.Minimum, (int)nudQscale.Maximum);
            nudQscale.ValueChanged += (sender, e) => UpdateUI();

            textBoxUserArgs.Text = Options.FFmpeg.UserArgs;
            textBoxUserArgs.TextChanged += (sender, e) => UpdateUI();

            string cli = "ffmpeg.exe";
            if (string.IsNullOrEmpty(Options.FFmpeg.CLIPath) && File.Exists(cli))
            {
                Options.FFmpeg.CLIPath = cli;
            }

            textBoxFFmpegPath.Text = Options.FFmpeg.CLIPath;
            textBoxFFmpegPath.TextChanged += (sender, e) => UpdateUI();
        }

        public void SaveSettings()
        {
            Options.FFmpeg.VideoCodec = (FFmpegVideoCodec)comboBoxCodec.SelectedIndex;
            Options.FFmpeg.Extension = comboBoxExtension.Text;

            Options.FFmpeg.CRF = (int)nudCRF.Value;
            Options.FFmpeg.Preset = (FFmpegPreset)comboBoxPreset.SelectedIndex;

            Options.FFmpeg.qscale = (int)nudQscale.Value;

            Options.FFmpeg.UserArgs = textBoxUserArgs.Text;

            Options.FFmpeg.CLIPath = textBoxFFmpegPath.Text;
        }

        public void UpdateUI()
        {
            SaveSettings();

            groupBoxH263.Enabled = Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libxvid;
            groupBoxH264.Enabled = Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libx264 || Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libvpx;

            if ((Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libx264 || Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libxvid) && comboBoxExtension.Text == "webm")
                comboBoxExtension.Text = "mp4";
            else if ((FFmpegVideoCodec)comboBoxCodec.SelectedIndex == FFmpegVideoCodec.libvpx && comboBoxExtension.Text != "webm")
                comboBoxExtension.Text = "webm";

            textBoxCommandLinePreview.Text = Options.GetFFmpegArgs();
        }

        private void buttonFFmpegBrowse_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFile("Browse for ffmpeg.exe", textBoxFFmpegPath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            UpdateUI();
        }

        private void buttonFFmpegHelp_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://www.ffmpeg.org/ffmpeg.html");
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadFFmpeg();
        }

        public DialogResult DownloadFFmpeg(bool runInstallerInBackground = true)
        {
            using (DownloaderForm form = new DownloaderForm(FFmpegDownloadLink, "ffmpeg.7z"))
            {
                form.Proxy = ProxyInfo.Current.GetWebProxy();
                form.InstallType = InstallType.Event;
                form.RunInstallerInBackground = runInstallerInBackground;
                form.InstallRequested += form_InstallRequested;
                return form.ShowDialog();
            }
        }

        public static string FFmpegDownloadLink
        {
            get
            {
                if (NativeMethods.Is64Bit())
                {
                    return "http://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.7z";
                }
                else
                {
                    return "http://ffmpeg.zeranoe.com/builds/win32/static/ffmpeg-latest-win32-static.7z";
                }
            }
        }

        private void form_InstallRequested(string filePath)
        {
            string extractPath = Path.Combine(Application.StartupPath, "ffmpeg.exe");
            bool result = ExtractFFmpeg(filePath, extractPath);

            if (result)
            {
                this.InvokeSafe(() => textBoxFFmpegPath.Text = "ffmpeg.exe");
                MessageBox.Show("FFmpeg successfully downloaded.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Download of FFmpeg failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool ExtractFFmpeg(string zipPath, string extractPath)
        {
            try
            {
                if (NativeMethods.Is64Bit())
                {
                    SevenZipExtractor.SetLibraryPath(Path.Combine(Application.StartupPath, "7z-x64.dll"));
                }
                else
                {
                    SevenZipExtractor.SetLibraryPath(Path.Combine(Application.StartupPath, "7z.dll"));
                }

                using (SevenZipExtractor zip = new SevenZipExtractor(zipPath))
                {
                    Regex regex = new Regex(@"^ffmpeg-.+\\bin\\ffmpeg\.exe$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

                    foreach (ArchiveFileInfo item in zip.ArchiveFileData)
                    {
                        if (regex.IsMatch(item.FileName))
                        {
                            using (FileStream fs = new FileStream(extractPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                zip.ExtractFile(item.Index, fs);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }
    }
}