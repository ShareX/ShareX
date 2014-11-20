using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    public interface iCaptureType
    {
        Image Capture(Rectangle r, IntPtr handle, bool captureCursor);
    }
}
