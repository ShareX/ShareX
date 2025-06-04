using System;
using System.Drawing;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics.GDI
{
    public class MonitorInfo
    {
        public bool IsPrimary { get; set; }
        public Rectangle MonitorArea { get; set; }
        public Rectangle WorkArea { get; set; }
        public string DeviceName { get; set; }
        public IntPtr Hmon { get; set; }
    }
}
