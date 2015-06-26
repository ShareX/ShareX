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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class WebpageCapture : IDisposable
    {
        public event Action<Bitmap> CaptureCompleted;

        public int CaptureDelay { get; set; }

        private WebBrowser webBrowser;

        public WebpageCapture()
        {
            webBrowser = new WebBrowser();
            webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
        }

        public void CapturePage(string url)
        {
            CapturePage(url, Screen.PrimaryScreen.Bounds.Size);
        }

        public void CapturePage(string url, Size browserSize)
        {
            if (!string.IsNullOrEmpty(url))
            {
                webBrowser.ScriptErrorsSuppressed = true;
                webBrowser.ScrollBarsEnabled = false;
                webBrowser.Size = browserSize;
                webBrowser.Navigate(url);
            }
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                TaskEx.RunDelayed(GetWebpageBitmap, CaptureDelay);
            }
        }

        private void GetWebpageBitmap()
        {
            Rectangle rect = webBrowser.Document.Body.ScrollRectangle;
            webBrowser.Size = new Size(rect.Width, rect.Height);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);

            try
            {
                webBrowser.DrawToBitmap(bmp, rect);
                OnCaptureCompleted(bmp);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);

                if (bmp != null)
                {
                    bmp.Dispose();
                }

                OnCaptureCompleted(null);
            }
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

        public void Dispose()
        {
            if (webBrowser != null)
            {
                webBrowser.Dispose();
            }
        }
    }
}