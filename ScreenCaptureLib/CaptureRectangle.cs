using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    public class CaptureRectangle : iScreenShot
    {
        iCaptureType captureType = new TypeRectangle();

        Rectangle rect;
        IntPtr p = new IntPtr(0);

        public void setRectangle(Rectangle rect)
        {
            this.rect = rect;
        }

        public Rectangle getRectangle()
        {
            return rect;
        }

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
            return captureType.Capture(rect, p, false);
        }
    }
}
