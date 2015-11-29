using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib;
using System;
using System.Linq;
using System.Text;

namespace ShareX
{
    abstract class CaptureStrategy
    {
        abstract public void capture(CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true);
    }
}

