using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    class CaptureManagedRectangle : iScreenShot
    {
        iCaptureType captureType = new TypeManagedRectangle();

        Rectangle rect;
        public void setRectangle(Rectangle rect)
        {
            this.rect = rect;
        }
        public Rectangle getRectangle()
        {
            return rect;
        }

        public override Image Screenshot()
        {
            return captureType.Capture(rect, new IntPtr(0), false);
        }
    }
}
