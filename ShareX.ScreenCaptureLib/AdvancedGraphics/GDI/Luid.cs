using System.Runtime.InteropServices;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics.GDI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    {
        public uint LowPart;
        public int HighPart;
    }
}
