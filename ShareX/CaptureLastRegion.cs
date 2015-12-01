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
    class CaptureLastRegion : CaptureStrategy
    {
        public CaptureLastRegion(MainForm mainForm)
            : base(mainForm)
        { }

        public override void capture(CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            switch (lastRegionCaptureType)
            {
                case LastRegionCaptureType.Surface:
                    if (Surface.LastRegionFillPath != null)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = Screenshot.CaptureFullscreen())
                            {
                                return ShapeCaptureHelpers.GetRegionImage(screenshot, Surface.LastRegionFillPath, Surface.LastRegionDrawPath, taskSettings.CaptureSettings.SurfaceOptions);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureStrategy captureRegion = new CaptureRegion(mainForm);
                        captureRegion.capture(CaptureType.Rectangle, taskSettings, autoHideForm);
                    }
                    break;
                case LastRegionCaptureType.Light:
                    if (!RectangleLight.LastSelectionRectangle0Based.IsEmpty)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = Screenshot.CaptureFullscreen())
                            {
                                return ImageHelpers.CropImage(screenshot, RectangleLight.LastSelectionRectangle0Based);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureRectangleLight(taskSettings, autoHideForm);
                    }
                    break;
                case LastRegionCaptureType.Transparent:
                    if (!RectangleTransparent.LastSelectionRectangle0Based.IsEmpty)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = Screenshot.CaptureFullscreen())
                            {
                                return ImageHelpers.CropImage(screenshot, RectangleTransparent.LastSelectionRectangle0Based);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureRectangleTransparent(taskSettings, autoHideForm);
                    }
                    break;
                case LastRegionCaptureType.Annotate:
                    if (!RectangleAnnotate.LastSelectionRectangle0Based.IsEmpty)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = Screenshot.CaptureFullscreen())
                            {
                                return ImageHelpers.CropImage(screenshot, RectangleAnnotate.LastSelectionRectangle0Based);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureRectangleAnnotate(taskSettings, autoHideForm);
                    }
                    break;
            }
        }

        private void CaptureRectangleAnnotate(TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            DoCapture(() =>
            {
                Image img = null;

                using (RectangleAnnotate rectangleAnnotate = new RectangleAnnotate(taskSettings.CaptureSettingsReference.RectangleAnnotateOptions))
                {
                    if (rectangleAnnotate.ShowDialog() == DialogResult.OK)
                    {
                        img = rectangleAnnotate.GetAreaImage();

                        if (img != null)
                        {
                            lastRegionCaptureType = LastRegionCaptureType.Annotate;
                        }
                    }
                }

                return img;
            }, CaptureType.Rectangle, taskSettings, autoHideForm);
        }

        private void CaptureRectangleLight(TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            DoCapture(() =>
            {
                Image img = null;

                using (RectangleLight rectangleLight = new RectangleLight())
                {
                    if (rectangleLight.ShowDialog() == DialogResult.OK)
                    {
                        img = rectangleLight.GetAreaImage();

                        if (img != null)
                        {
                            lastRegionCaptureType = LastRegionCaptureType.Light;
                        }
                    }
                }

                return img;
            }, CaptureType.Rectangle, taskSettings, autoHideForm);
        }

        private void CaptureRectangleTransparent(TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            DoCapture(() =>
            {
                Image img = null;

                using (RectangleTransparent rectangleTransparent = new RectangleTransparent())
                {
                    if (rectangleTransparent.ShowDialog() == DialogResult.OK)
                    {
                        img = rectangleTransparent.GetAreaImage();

                        if (img != null)
                        {
                            lastRegionCaptureType = LastRegionCaptureType.Transparent;
                        }
                    }
                }

                return img;
            }, CaptureType.Rectangle, taskSettings, autoHideForm);
        }
    }
}
