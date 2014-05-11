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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public class FFmpegHelper : ExternalCLIManager
    {
        public ScreencastOptions Options { get; private set; }

        public FFmpegHelper(ScreencastOptions options)
        {
            Options = options;

            Helpers.CreateDirectoryIfNotExist(Options.OutputPath);

            // It is actually output data
            ErrorDataReceived += FFmpegCLIHelper_OutputDataReceived;
        }

        private void FFmpegCLIHelper_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //DebugHelper.WriteLine(e.Data);
        }

        public bool Record()
        {
            /*
            // https://github.com/rdp/screen-capture-recorder-to-video-windows-free configuration section
            string dshowRegistryPath = "Software\\screen-capture-recorder";
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "start_x", captureRectangle.X);
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "start_y", captureRectangle.Y);
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "capture_width", captureRectangle.Width);
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "capture_height", captureRectangle.Height);
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "default_max_fps", Options.FPS);

            // input FPS
            args.AppendFormat("-r {0} ", Options.FPS);

            args.Append("-f dshow -i ");

            // dshow audio/video device: https://github.com/rdp/screen-capture-recorder-to-video-windows-free
            args.AppendFormat("audio=\"{0}\":", "virtual-audio-capturer");
            args.AppendFormat("video=\"{0}\" ", "screen-capture-recorder");
            */

            int result = Open(Options.FFmpeg.CLIPath, Options.GetFFmpegArgs());
            return result == 0;
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

        public void ListDevices()
        {
            WriteInput("-list_devices true -f dshow -i dummy");
        }

        public override void Close()
        {
            WriteInput("q");
        }
    }
}