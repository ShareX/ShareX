using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using HelpersLib;

namespace ScreenCaptureLib
{
    public class TypeActiveMonitor : iCaptureType
    {
        public Image Capture(Rectangle r, IntPtr handle, bool captureCursor)
        {
            Rectangle bounds = CaptureHelpers.GetActiveScreenBounds();

            return TypeRectangle.Capture(bounds);
        }

        public TypeActiveMonitor()
        {
        }
    }
}
