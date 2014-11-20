using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    class CaptureWindow : iScreenShot
    {
        TypeWindow captureType = new TypeWindow();
        public void setCaptureType(TypeWindow c)
        {
            captureType = c;
        }
        public iCaptureType getCaptureType()
        {
            return captureType;
        }

        Rectangle rect;
        public void setRectangle(Rectangle rect)
        {
            this.rect = rect;
        }
        public Rectangle getRectangle()
        {
            return rect;
        }

        public void setAutoTaskBar(bool x)
        {
            captureType.setAutoHideTaskbar(x);
        }
        public bool getAutoTaskBar()
        {
            return captureType.getAutoHideTaskbar();
        }

        public void setClientCaptureArea(bool x)
        {
            captureType.setCaptureClientArea(x);
        }
        public bool getClientCaptureType()
        {
            return captureType.getCaptureClientArea();
        }
            

        public override Image Screenshot()
        {
            return captureType.Capture(rect, new IntPtr(0), false);
        }
    }
}
