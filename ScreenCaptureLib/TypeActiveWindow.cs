using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using HelpersLib;

namespace ScreenCaptureLib
{
    public class TypeActiveWindow : iCaptureType
    {
        TypeWindow t = new TypeWindow();

        public Image Capture(Rectangle r, IntPtr handle, bool captureCursor)
        {
            IntPtr hand = NativeMethods.GetForegroundWindow();

            return t.Capture(new Rectangle(0,0,0,0), hand, false);
        }

        public TypeActiveWindow()
        {
        }
    }
}
