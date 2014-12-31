#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

using SevenZip;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class FFmpegDownloader
    {
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
    }
}