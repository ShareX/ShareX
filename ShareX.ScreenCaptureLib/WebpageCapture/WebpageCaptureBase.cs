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
            return Helpers.IsWindows10OrGreater() &&
                ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6) && // 6th API contract is minimum for WebView support.
                File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "edgehtml.dll")); // Make sure EdgeHTML is present.
        }

        public static WebpageCaptureBase Create()
        {
            if (WebViewSupported())
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
