using System;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics.Direct3D
{
    class ModernCaptureMonitorSession : ModernCaptureMonitorDescription, IDisposable
    {
        private bool disposedValue;

        public GraphicsCaptureItem CaptureItem { get; set; }
        public Direct3D11CaptureFramePool FramePool { get; set; }
        public GraphicsCaptureSession Session { get; set; }

        public ModernCaptureMonitorSession(IDirect3DDevice device, ModernCaptureMonitorDescription description) : base(description)
        {
            this.CaptureItem = WinRTCaptureHelper.CreateItemForMonitor(description.MonitorInfo.Hmon);
            this.FramePool = Direct3D11CaptureFramePool.Create(device,
                description.HdrMetadata.EnableHdrProcessing ? DirectXPixelFormat.R16G16B16A16Float : DirectXPixelFormat.B8G8R8A8UIntNormalized,
                2, CaptureItem.Size);
            this.Session = FramePool.CreateCaptureSession(CaptureItem);
            this.Session.IsCursorCaptureEnabled = description.CaptureCursor;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Session?.Dispose();
                    FramePool?.Dispose();
                }

                Session = null;
                FramePool = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
