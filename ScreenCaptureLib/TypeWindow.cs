using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using HelpersLib;

namespace ScreenCaptureLib
{
    public class TypeWindow : iCaptureType
    {
        public bool AutoHideTaskbar = Screenshot.AutoHideTaskbar;
        public bool getAutoHideTaskbar() { return AutoHideTaskbar; }
        public void setAutoHideTaskbar(bool x) { AutoHideTaskbar = x; }

        public bool CaptureClientArea = Screenshot.CaptureClientArea;
        public bool getCaptureClientArea() { return CaptureClientArea; }
        public void setCaptureClientArea(bool x) { CaptureClientArea = x; }

        public Image Capture(Rectangle r, IntPtr handle, bool captureCursor)
        {
            if (handle.ToInt32() > 0)
            {
                Rectangle rect;

                if (CaptureClientArea)
                {
                    rect = NativeMethods.GetClientRect(handle);
                }
                else
                {
                    rect = CaptureHelpers.GetWindowRectangle(handle);
                }

                bool isTaskbarHide = false;

                try
                {
                    if (AutoHideTaskbar)
                    {
                        isTaskbarHide = NativeMethods.SetTaskbarVisibilityIfIntersect(false, rect);
                    }

                    return TypeRectangle.Capture(rect);
                }
                finally
                {
                    if (isTaskbarHide)
                    {
                        NativeMethods.SetTaskbarVisibility(true);
                    }
                }
            }

            return null;
        }

        public TypeWindow()
        {
        } 
    }
}
