using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    class CaptureCustomRegion : CaptureStrategy
    {
        public CaptureCustomRegion(MainForm mainForm)
            : base(mainForm)
        { }

        public override void capture(CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            DoCapture(() =>
            {
                Rectangle regionBounds = taskSettings.CaptureSettings.CaptureCustomRegion;
                Image img = Screenshot.CaptureRectangle(regionBounds);

                return img;
            }, CaptureType.CustomRegion, taskSettings, autoHideForm);
        }

    }
}
