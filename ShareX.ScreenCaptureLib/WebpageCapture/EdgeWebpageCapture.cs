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
            viewProcess.Terminate();
        }
    }
}
