using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    public class CaptureActiveWindow : iScreenShot
    {
        iCaptureType captureType = new TypeActiveWindow();

        public CaptureActiveWindow()
        {
        }

        public override Image Screenshot()
        {
            return captureType.Capture(new Rectangle(0, 0, 0, 0), new IntPtr(0), false);
        }
    }
}
