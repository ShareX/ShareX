#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public class FFmpegHelper : ExternalCLIManager
    {
        public static readonly int libmp3lame_qscale_end = 9;
        public static readonly string SourceNone = "None";
        public static readonly string SourceGDIGrab = "GDI grab";

        public StringBuilder Output { get; private set; }
        public ScreencastOptions Options { get; private set; }

        public FFmpegHelper(ScreencastOptions options)
        {
            Output = new StringBuilder();
            OutputDataReceived += FFmpegHelper_DataReceived;
            ErrorDataReceived += FFmpegHelper_DataReceived;
            Options = options;
            Helpers.CreateDirectoryIfNotExist(Options.OutputPath);
        }

        private void FFmpegHelper_DataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (this)
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Output.AppendLine(e.Data);
                }
            }
        }

        public bool Record()
        {
            int errorCode = Open(Options.FFmpeg.CLIPath, Options.GetFFmpegCommands());
            bool result = errorCode == 0;
            if (Options.FFmpeg.ShowError && !result)
            {
                new OutputBox(Output.ToString(), "FFmpeg error").ShowDialog();
            }
            return result;
        }

        public static DialogResult DownloadFFmpeg(bool async, DownloaderForm.DownloaderInstallEventHandler installRequested)
        {
            string url;

            if (NativeMethods.Is64Bit())
            {
                url = "http://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.7z";
            }
            else
            {
                url = "http://ffmpeg.zeranoe.com/builds/win32/static/ffmpeg-latest-win32-static.7z";
            }

            using (DownloaderForm form = new DownloaderForm(url, "ffmpeg.7z"))
            {
                form.Proxy = ProxyInfo.Current.GetWebProxy();
                form.InstallType = InstallType.Event;
                form.RunInstallerInBackground = async;
                form.InstallRequested += installRequested;
                return form.ShowDialog();
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

                Helpers.CreateDirectoryIfNotExist(extractPath);

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

        public DirectShowDevices GetDirectShowDevices()
        {
            DirectShowDevices devices = new DirectShowDevices();

            if (File.Exists(Options.FFmpeg.CLIPath))
            {
                string arg = "-list_devices true -f dshow -i dummy";
                Open(Options.FFmpeg.CLIPath, arg);
                string output = Output.ToString();
                string[] lines = output.Lines();
                bool isVideo = true;
                Regex regex = new Regex("\\[dshow @ \\w+\\]  \"(.+)\"", RegexOptions.Compiled | RegexOptions.CultureInvariant);
                foreach (string line in lines)
                {
                    if (line.EndsWith("] DirectShow video devices", StringComparison.InvariantCulture))
                    {
                        isVideo = true;
                        continue;
                    }
                    else if (line.EndsWith("] DirectShow audio devices", StringComparison.InvariantCulture))
                    {
                        isVideo = false;
                        continue;
                    }

                    Match match = regex.Match(line);

                    if (match.Success)
                    {
                        string value = match.Groups[1].Value;

                        if (isVideo)
                        {
                            devices.VideoDevices.Add(value);
                        }
                        else
                        {
                            devices.AudioDevices.Add(value);
                        }
                    }
                }
            }

            return devices;
        }

        public override void Close()
        {
            WriteInput("q");
        }
    }

    public class DirectShowDevices
    {
        public List<string> VideoDevices = new List<string>();
        public List<string> AudioDevices = new List<string>();
    }
}