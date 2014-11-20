using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace ScreenCaptureLib
{
    public abstract class iScreenShot
    {
        iCaptureType captureType { get; set; }

        public abstract Image Screenshot();
    }
}
