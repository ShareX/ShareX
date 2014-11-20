using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    class CaptureFullScreen : iScreenShot  
    {
        iCaptureType captureType = new TypeFullScreen();

        public void setCaptureType(iCaptureType c)
        {
            captureType = c;
        }

        public iCaptureType getCaptureType()
        {
            return captureType;
        }

        public override Image Screenshot()
        {
            return captureType.Capture(new Rectangle(0,0,0,0), new IntPtr(0), false);
        }
    }
}
