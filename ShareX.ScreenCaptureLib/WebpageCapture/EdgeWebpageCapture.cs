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

using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Web.UI;
using Windows.Web.UI.Interop;

namespace ShareX.ScreenCaptureLib.WebpageCapture
{
    public class EdgeWebpageCapture : WebpageCaptureBase
    {
        private WebViewControl webView;
        private WebViewControlProcess viewProcess;
        private Control viewHost;

        public EdgeWebpageCapture()
        {
            viewProcess = new WebViewControlProcess();
            viewHost = new Control();

            webView = viewProcess.CreateWebViewControlAsync(viewHost.Handle.ToInt64(), new Rect()).GetAwaiter().GetResult();
            webView.LongRunningScriptDetected += (s, e) => e.StopPageScriptExecution = true;
            webView.NavigationCompleted += webView_NavigationCompleted;
        }

        public override void CapturePage(string url, System.Drawing.Size browserSize)
        {
            if (!string.IsNullOrEmpty(url))
            {
                webView.Bounds = new Rect(0, 0, browserSize.Width, browserSize.Height);

                if (string.IsNullOrEmpty(url))
                {
                    url = "about:blank";
                }

                webView.Navigate(new Uri(url, UriKind.Absolute));
            }
        }

        public override void Stop()
        {
            webView.Stop();
        }

        private async void webView_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                await Task.Delay(CaptureDelay);

                using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await webView.CapturePreviewToStreamAsync(stream);
                    OnCaptureCompleted(new Bitmap(stream.AsStream()));
                }
            }
            else
            {
                OnCaptureCompleted(null);
            }
        }

        public override void Dispose()
        {
            webView.Close();
            viewHost.Dispose();

            if (viewProcess.ProcessId != 0)
            {
                viewProcess.Terminate();
            }
        }
    }
}