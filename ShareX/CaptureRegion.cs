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
    class CaptureRegion : CaptureStrategy
    {
        public CaptureRegion(MainForm mainForm)
            : base(mainForm)
        { }

        public override void capture(CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            Surface surface;

            switch (captureType)
            {
                default:
                case CaptureType.Rectangle:
                    surface = new RectangleRegion();
                    break;
                case CaptureType.RectangleWindow:
                    RectangleRegion rectangleRegion = new RectangleRegion();
                    rectangleRegion.AreaManager.WindowCaptureMode = true;
                    rectangleRegion.AreaManager.IncludeControls = true;
                    surface = rectangleRegion;
                    break;
                case CaptureType.Polygon:
                    surface = new PolygonRegion();
                    break;
                case CaptureType.Freehand:
                    surface = new FreeHandRegion();
                    break;
            }

            DoCapture(() =>
            {
                Image img = null;
                Image screenshot = Screenshot.CaptureFullscreen();

                try
                {
                    surface.Config = taskSettings.CaptureSettingsReference.SurfaceOptions;
                    surface.SurfaceImage = screenshot;
                    surface.Prepare();
                    surface.ShowDialog();

                    if (surface.Result == SurfaceResult.Region)
                    {
                        using (screenshot)
                        {
                            img = surface.GetRegionImage();
                        }
                    }
                    else if (surface.Result == SurfaceResult.Fullscreen)
                    {
                        img = screenshot;
                    }
                    else if (surface.Result == SurfaceResult.Monitor)
                    {
                        Screen[] screens = Screen.AllScreens;

                        if (surface.MonitorIndex < screens.Length)
                        {
                            Screen screen = screens[surface.MonitorIndex];
                            Rectangle screenRect = CaptureHelpers.ScreenToClient(screen.Bounds);

                            using (screenshot)
                            {
                                img = ImageHelpers.CropImage(screenshot, screenRect);
                            }
                        }
                    }
                    else if (surface.Result == SurfaceResult.ActiveMonitor)
                    {
                        Rectangle activeScreenRect = CaptureHelpers.GetActiveScreenBounds0Based();

                        using (screenshot)
                        {
                            img = ImageHelpers.CropImage(screenshot, activeScreenRect);
                        }
                    }

                    if (img != null)
                    {
                        lastRegionCaptureType = LastRegionCaptureType.Surface;
                    }
                }
                finally
                {
                    surface.Dispose();
                }

                return img;
            }, captureType, taskSettings, autoHideForm);
        }

    }
}
