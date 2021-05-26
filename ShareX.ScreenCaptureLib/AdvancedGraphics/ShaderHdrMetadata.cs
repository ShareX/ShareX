using System.Runtime.InteropServices;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ShaderHdrMetadata
    {
        public bool EnableHdrProcessing;
        public float MonHdrDispNits;
        public float MonSdrDispNits;
        public float ExposureLevel;
    }
}
