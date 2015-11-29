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
    class CaptureScreen : CaptureStrategy
    {
        public CaptureScreen(MainForm mainForm)
            : base(mainForm)
        { }

        public override void capture(CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            DoCapture(Screenshot.CaptureFullscreen, captureType, taskSettings, autoHideForm);
        }

    }
}
