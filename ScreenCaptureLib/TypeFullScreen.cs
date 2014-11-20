using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using HelpersLib;

namespace ScreenCaptureLib
{
    class TypeFullScreen : iCaptureType
    {
        public Image Capture(Rectangle r, IntPtr handle, bool captureCursor)
        {
            Rectangle bounds = CaptureHelpers.GetScreenBounds();
            return TypeRectangle.Capture(bounds);
        }

        public TypeFullScreen()
        {
        }

    }
}
