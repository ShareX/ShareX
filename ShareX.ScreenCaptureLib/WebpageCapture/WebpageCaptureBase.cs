#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Windows.Foundation.Metadata;

namespace ShareX.ScreenCaptureLib.WebpageCapture
{
    public abstract class WebpageCaptureBase : IDisposable
    {
        public event Action<Bitmap> CaptureCompleted;

        public int CaptureDelay { get; set; }

        public abstract void CapturePage(string url, Size browserSize);

        public abstract void Stop();

        public abstract void Dispose();

        public void CapturePage(string url)
        {
            CapturePage(url, Screen.PrimaryScreen.Bounds.Size);
        }

        protected void OnCaptureCompleted(Bitmap bmp)
        {
            if (CaptureCompleted != null)
            {
                CaptureCompleted(bmp);
            }
            else if (bmp != null)
            {
                bmp.Dispose();
            }
        }

        private static bool WebViewSupported()
        {
            return ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6) && // 6th API contract is minimum for WebView support.
                File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "edgehtml.dll")); // Make sure EdgeHTML is present.
        }

        public static WebpageCaptureBase Create()
        {
            if (Helpers.IsWindows10OrGreater() && WebViewSupported())
            {
                try
                {
                    return new EdgeWebpageCapture();
                }
                catch { }
            }

            return new InternetExplorerWebpageCapture();
        }
    }
}