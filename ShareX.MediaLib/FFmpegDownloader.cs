#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShareX.MediaLib
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
                form.Proxy = HelpersOptions.CurrentProxy.GetWebProxy();
                form.InstallType = InstallType.Event;
                form.RunInstallerInBackground = async;
                form.InstallRequested += installRequested;
                return form.ShowDialog();
            }
        }

        public static bool ExtractFFmpeg(string archivePath, string extractPath)
        {
            try
            {
                SevenZipManager sevenZipManager = new SevenZipManager();
                List<string> files = new List<string>() { "ffmpeg.exe" };
                return sevenZipManager.Extract(archivePath, extractPath, files);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }
    }
}