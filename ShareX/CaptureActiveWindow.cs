using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    class CaptureActiveWindow : CaptureStrategy
    {
        public CaptureActiveWindow(MainForm mainForm)
            : base(mainForm)
        { }

        public override void capture(CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            DoCapture(() =>
            {
                Image img;
                string activeWindowTitle = NativeMethods.GetForegroundWindowText();
                string activeProcessName = null;

                using (Process process = NativeMethods.GetForegroundWindowProcess())
                {
                    if (process != null)
                    {
                        activeProcessName = process.ProcessName;
                    }
                }

                if (taskSettings.CaptureSettings.CaptureTransparent && !taskSettings.CaptureSettings.CaptureClientArea)
                {
                    img = Screenshot.CaptureActiveWindowTransparent();
                }
                else
                {
                    img = Screenshot.CaptureActiveWindow();
                }

                img.Tag = new ImageTag
                {
                    ActiveWindowTitle = activeWindowTitle,
                    ActiveProcessName = activeProcessName
                };

                return img;
            }, CaptureType.ActiveWindow, taskSettings, autoHideForm);
        }

    }
}
