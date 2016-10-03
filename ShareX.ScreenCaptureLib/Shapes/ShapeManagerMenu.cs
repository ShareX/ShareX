#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal partial class ShapeManager
    {
        internal TextAnimation MenuTextAnimation = new TextAnimation(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(0.5));

        private Form menuForm;
        private ToolStripEx tsMain;
        private ToolStripButton tsbUndoObject, tsbDeleteAll;
        private ToolStripDropDownButton tsddbShapeOptions;
        private ToolStripMenuItem tsmiBorderColor, tsmiFillColor, tsmiHighlightColor, tsmiQuickCrop, tsmiRegionCapture;
        private ToolStripLabeledNumericUpDown tslnudBorderSize, tslnudCornerRadius, tslnudBlurRadius, tslnudPixelateSize;

        private void CreateMenu()
        {
            menuForm = new Form()
            {
                AutoScaleDimensions = new SizeF(6F, 13F),
                AutoScaleMode = AutoScaleMode.Font,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ClientSize = new Size(759, 509),
                FormBorderStyle = FormBorderStyle.None,
                Location = new Point(200, 200),
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                Text = "RegionCaptureFormMenu"
            };

            menuForm.SuspendLayout();

            tsMain = new ToolStripEx()
            {
                AutoSize = true,
                CanOverflow = false,
                ClickThrough = true,
                Dock = DockStyle.None,
                GripStyle = ToolStripGripStyle.Hidden,
                Location = new Point(0, 0),
                MinimumSize = new Size(300, 30),
                Padding = new Padding(0, 0, 0, 0),
                Renderer = new CustomToolStripProfessionalRenderer(),
                TabIndex = 0
            };

            tsMain.SuspendLayout();

            menuForm.Controls.Add(tsMain);

            ToolStripLabel tslDragLeft = new ToolStripLabel()
            {
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                Image = Resources.ui_radio_button_uncheck,
                Margin = new Padding(0),
                Padding = new Padding(2)
            };

            tslDragLeft.MouseDown += (sender, e) =>
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.DefWindowProc(menuForm.Handle, (uint)WindowsMessages.SYSCOMMAND, (UIntPtr)NativeConstants.MOUSE_MOVE, IntPtr.Zero);
            };

            tsMain.Items.Add(tslDragLeft);

            #region Editor mode

            if (form.Mode == RegionCaptureMode.Editor)
            {
                ToolStripButton tsbCompleteEdit = new ToolStripButton("Run after capture tasks");
                tsbCompleteEdit.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbCompleteEdit.Image = Resources.tick;
                tsbCompleteEdit.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateRunAfterCaptureTasks);
                tsMain.Items.Add(tsbCompleteEdit);

                ToolStripButton tsbSaveImage = new ToolStripButton("Save image");
                tsbSaveImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbSaveImage.Enabled = !string.IsNullOrEmpty(form.ImageFilePath);
                tsbSaveImage.Image = Resources.disk_black;
                tsbSaveImage.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateSaveImage);
                tsMain.Items.Add(tsbSaveImage);

                ToolStripButton tsbSaveImageAs = new ToolStripButton("Save image as...");
                tsbSaveImageAs.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbSaveImageAs.Image = Resources.disks_black;
                tsbSaveImageAs.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateSaveImageAs);
                tsMain.Items.Add(tsbSaveImageAs);

                ToolStripButton tsbCopyImage = new ToolStripButton("Copy image to clipboard");
                tsbCopyImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbCopyImage.Image = Resources.clipboard;
                tsbCopyImage.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateCopyImage);
                tsMain.Items.Add(tsbCopyImage);

                ToolStripButton tsbUploadImage = new ToolStripButton("Upload image");
                tsbUploadImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbUploadImage.Image = Resources.drive_globe;
                tsbUploadImage.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateUploadImage);
                tsMain.Items.Add(tsbUploadImage);

                ToolStripButton tsbPrintImage = new ToolStripButton("Print image...");
                tsbPrintImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbPrintImage.Image = Resources.printer;
                tsbPrintImage.MouseDown += (sender, e) => form.Close(RegionResult.AnnotatePrintImage);
                tsMain.Items.Add(tsbPrintImage);

                tsMain.Items.Add(new ToolStripSeparator());
            }

            #endregion Editor mode

            #region Tools

            foreach (ShapeType shapeType in Helpers.GetEnums<ShapeType>())
            {
                if (form.Mode == RegionCaptureMode.Editor)
                {
                    if (IsShapeTypeRegion(shapeType))
                    {
                        continue;
                    }
                }
                else if (shapeType == ShapeType.DrawingRectangle)
                {
                    tsMain.Items.Add(new ToolStripSeparator());
                }

                ToolStripButton tsbShapeType = new ToolStripButton(shapeType.GetLocalizedDescription());
                tsbShapeType.DisplayStyle = ToolStripItemDisplayStyle.Image;

                Image img = null;

                switch (shapeType)
                {
                    case ShapeType.RegionRectangle:
                        img = Resources.layer_shape_region;
                        break;
                    case ShapeType.RegionRoundedRectangle:
                        img = Resources.layer_shape_round_region;
                        break;
                    case ShapeType.RegionEllipse:
                        img = Resources.layer_shape_ellipse_region;
                        break;
                    case ShapeType.RegionFreehand:
                        img = Resources.layer_shape_polygon;
                        break;
                    case ShapeType.DrawingRectangle:
                        img = Resources.layer_shape;
                        break;
                    case ShapeType.DrawingRoundedRectangle:
                        img = Resources.layer_shape_round;
                        break;
                    case ShapeType.DrawingEllipse:
                        img = Resources.layer_shape_ellipse;
                        break;
                    case ShapeType.DrawingFreehand:
                        img = Resources.layer_shape_curve;
                        break;
                    case ShapeType.DrawingLine:
                        img = Resources.layer_shape_line;
                        break;
                    case ShapeType.DrawingArrow:
                        img = Resources.layer_shape_arrow;
                        break;
                    case ShapeType.DrawingText:
                        img = Resources.layer_shape_text;
                        break;
                    case ShapeType.DrawingSpeechBalloon:
                        img = Resources.balloon_box_left;
                        break;
                    case ShapeType.DrawingStep:
                        img = Resources.counter_reset;
                        break;
                    case ShapeType.DrawingImage:
                        img = Resources.image;
                        break;
                    case ShapeType.EffectBlur:
                        img = Resources.layer_shade;
                        break;
                    case ShapeType.EffectPixelate:
                        img = Resources.grid;
                        break;
                    case ShapeType.EffectHighlight:
                        img = Resources.highlighter_text;
                        break;
                }

                tsbShapeType.Image = img;
                tsbShapeType.Checked = shapeType == CurrentShapeType;
                tsbShapeType.Tag = shapeType;

                tsbShapeType.MouseDown += (sender, e) =>
                {
                    tsbShapeType.RadioCheck();
                    CurrentShapeType = shapeType;
                };

                tsMain.Items.Add(tsbShapeType);
            }

            #endregion Tools

            #region Selected object

            tsMain.Items.Add(new ToolStripSeparator());

            tsddbShapeOptions = new ToolStripDropDownButton("Shape options");
            tsddbShapeOptions.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsddbShapeOptions.Image = Resources.layer__pencil;
            tsMain.Items.Add(tsddbShapeOptions);

            tsmiBorderColor = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Border_color___);
            tsmiBorderColor.Click += (sender, e) =>
            {
                PauseForm();

                ShapeType shapeType = CurrentShapeType;

                Color borderColor;

                if (shapeType == ShapeType.DrawingText || shapeType == ShapeType.DrawingSpeechBalloon)
                {
                    borderColor = AnnotationOptions.TextBorderColor;
                }
                else if (shapeType == ShapeType.DrawingStep)
                {
                    borderColor = AnnotationOptions.StepBorderColor;
                }
                else
                {
                    borderColor = AnnotationOptions.BorderColor;
                }

                using (ColorPickerForm dialogColor = new ColorPickerForm(borderColor))
                {
                    if (dialogColor.ShowDialog() == DialogResult.OK)
                    {
                        if (shapeType == ShapeType.DrawingText || shapeType == ShapeType.DrawingSpeechBalloon)
                        {
                            AnnotationOptions.TextBorderColor = dialogColor.NewColor;
                        }
                        else if (shapeType == ShapeType.DrawingStep)
                        {
                            AnnotationOptions.StepBorderColor = dialogColor.NewColor;
                        }
                        else
                        {
                            AnnotationOptions.BorderColor = dialogColor.NewColor;
                        }

                        UpdateMenu();
                        UpdateCurrentShape();
                        UpdateCursor();
                    }
                }

                ResumeForm();
            };
            tsddbShapeOptions.DropDownItems.Add(tsmiBorderColor);

            tslnudBorderSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Border_size_);
            tslnudBorderSize.Content.Minimum = 0;
            tslnudBorderSize.Content.Maximum = 20;
            tslnudBorderSize.Content.ValueChanged = (sender, e) =>
            {
                ShapeType shapeType = CurrentShapeType;

                int borderSize = (int)tslnudBorderSize.Content.Value;

                if (shapeType == ShapeType.DrawingText || shapeType == ShapeType.DrawingSpeechBalloon)
                {
                    AnnotationOptions.TextBorderSize = borderSize;
                }
                else if (shapeType == ShapeType.DrawingStep)
                {
                    AnnotationOptions.StepBorderSize = borderSize;
                }
                else
                {
                    AnnotationOptions.BorderSize = borderSize;
                }

                UpdateCurrentShape();
                UpdateCursor();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudBorderSize);

            tsmiFillColor = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Fill_color___);
            tsmiFillColor.Click += (sender, e) =>
            {
                PauseForm();

                ShapeType shapeType = CurrentShapeType;

                Color fillColor;

                if (shapeType == ShapeType.DrawingText || shapeType == ShapeType.DrawingSpeechBalloon)
                {
                    fillColor = AnnotationOptions.TextFillColor;
                }
                else if (shapeType == ShapeType.DrawingStep)
                {
                    fillColor = AnnotationOptions.StepFillColor;
                }
                else
                {
                    fillColor = AnnotationOptions.FillColor;
                }

                using (ColorPickerForm dialogColor = new ColorPickerForm(fillColor))
                {
                    if (dialogColor.ShowDialog() == DialogResult.OK)
                    {
                        if (shapeType == ShapeType.DrawingText || shapeType == ShapeType.DrawingSpeechBalloon)
                        {
                            AnnotationOptions.TextFillColor = dialogColor.NewColor;
                        }
                        else if (shapeType == ShapeType.DrawingStep)
                        {
                            AnnotationOptions.StepFillColor = dialogColor.NewColor;
                        }
                        else
                        {
                            AnnotationOptions.FillColor = dialogColor.NewColor;
                        }

                        UpdateMenu();
                        UpdateCurrentShape();
                    }
                }

                ResumeForm();
            };
            tsddbShapeOptions.DropDownItems.Add(tsmiFillColor);

            tslnudCornerRadius = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Corner_radius_);
            tslnudCornerRadius.Content.Minimum = 0;
            tslnudCornerRadius.Content.Maximum = 150;
            tslnudCornerRadius.Content.Increment = 3;
            tslnudCornerRadius.Content.ValueChanged = (sender, e) =>
            {
                ShapeType shapeType = CurrentShapeType;

                if (shapeType == ShapeType.RegionRoundedRectangle || shapeType == ShapeType.DrawingRoundedRectangle)
                {
                    AnnotationOptions.RoundedRectangleRadius = (int)tslnudCornerRadius.Content.Value;
                }
                else if (shapeType == ShapeType.DrawingText)
                {
                    AnnotationOptions.TextCornerRadius = (int)tslnudCornerRadius.Content.Value;
                }

                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudCornerRadius);

            tslnudBlurRadius = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Blur_radius_);
            tslnudBlurRadius.Content.Minimum = 2;
            tslnudBlurRadius.Content.Maximum = 100;
            tslnudBlurRadius.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.BlurRadius = (int)tslnudBlurRadius.Content.Value;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudBlurRadius);

            tslnudPixelateSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Pixel_size_);
            tslnudPixelateSize.Content.Minimum = 2;
            tslnudPixelateSize.Content.Maximum = 100;
            tslnudPixelateSize.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.PixelateSize = (int)tslnudPixelateSize.Content.Value;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudPixelateSize);

            tsmiHighlightColor = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Highlight_color___);
            tsmiHighlightColor.Click += (sender, e) =>
            {
                PauseForm();

                using (ColorPickerForm dialogColor = new ColorPickerForm(AnnotationOptions.HighlightColor))
                {
                    if (dialogColor.ShowDialog() == DialogResult.OK)
                    {
                        AnnotationOptions.HighlightColor = dialogColor.NewColor;
                        UpdateMenu();
                        UpdateCurrentShape();
                    }
                }

                ResumeForm();
            };
            tsddbShapeOptions.DropDownItems.Add(tsmiHighlightColor);

            tsbUndoObject = new ToolStripButton("Undo object");
            tsbUndoObject.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbUndoObject.Image = Resources.arrow_circle_225_left;
            tsbUndoObject.MouseDown += (sender, e) => UndoShape();
            tsMain.Items.Add(tsbUndoObject);

            tsbDeleteAll = new ToolStripButton(Resources.ShapeManager_CreateContextMenu_Delete_all_objects);
            tsbDeleteAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbDeleteAll.Image = Resources.eraser;
            tsbDeleteAll.MouseDown += (sender, e) => DeleteAllShapes();
            tsMain.Items.Add(tsbDeleteAll);

            #endregion Selected object

            #region Capture

            if (form.Mode != RegionCaptureMode.Editor)
            {
                tsMain.Items.Add(new ToolStripSeparator());

                ToolStripDropDownButton tsddbCapture = new ToolStripDropDownButton("Capture");
                tsddbCapture.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsddbCapture.Image = Resources.camera;
                tsMain.Items.Add(tsddbCapture);

                tsmiRegionCapture = new ToolStripMenuItem("Capture regions");
                tsmiRegionCapture.Image = Resources.layers;
                tsmiRegionCapture.MouseDown += (sender, e) =>
                {
                    form.UpdateRegionPath();
                    form.Close(RegionResult.Region);
                };
                tsddbCapture.DropDownItems.Add(tsmiRegionCapture);

                ToolStripMenuItem tsmiFullscreenCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_fullscreen);
                tsmiFullscreenCapture.Image = Resources.layer_fullscreen;
                tsmiFullscreenCapture.MouseDown += (sender, e) => form.Close(RegionResult.Fullscreen);
                tsddbCapture.DropDownItems.Add(tsmiFullscreenCapture);

                ToolStripMenuItem tsmiActiveMonitorCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_active_monitor);
                tsmiActiveMonitorCapture.Image = Resources.monitor;
                tsmiActiveMonitorCapture.MouseDown += (sender, e) => form.Close(RegionResult.ActiveMonitor);
                tsddbCapture.DropDownItems.Add(tsmiActiveMonitorCapture);

                ToolStripMenuItem tsmiMonitorCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_monitor);
                tsmiMonitorCapture.HideImageMargin();
                tsmiMonitorCapture.Image = Resources.monitor_window;
                tsddbCapture.DropDownItems.Add(tsmiMonitorCapture);

                Screen[] screens = Screen.AllScreens;

                for (int i = 0; i < screens.Length; i++)
                {
                    Screen screen = screens[i];
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(string.Format("{0}. {1}x{2}", i + 1, screen.Bounds.Width, screen.Bounds.Height));
                    int index = i;
                    tsmi.MouseDown += (sender, e) =>
                    {
                        form.MonitorIndex = index;
                        form.Close(RegionResult.Monitor);
                    };
                    tsmiMonitorCapture.DropDownItems.Add(tsmi);
                }
            }

            #endregion Capture

            #region Options

            if (form.Mode != RegionCaptureMode.Editor)
            {
                tsMain.Items.Add(new ToolStripSeparator());

                ToolStripDropDownButton tsddbOptions = new ToolStripDropDownButton(Resources.ShapeManager_CreateContextMenu_Options);
                tsddbOptions.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsddbOptions.Image = Resources.gear;
                tsMain.Items.Add(tsddbOptions);

                tsmiQuickCrop = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Multi_region_mode);
                tsmiQuickCrop.Checked = !Config.QuickCrop;
                tsmiQuickCrop.CheckOnClick = true;
                tsmiQuickCrop.Click += (sender, e) => Config.QuickCrop = !tsmiQuickCrop.Checked;
                tsddbOptions.DropDownItems.Add(tsmiQuickCrop);

                ToolStripMenuItem tsmiTips = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_tips);
                tsmiTips.Checked = Config.ShowTips;
                tsmiTips.CheckOnClick = true;
                tsmiTips.Click += (sender, e) => Config.ShowTips = tsmiTips.Checked;
                tsddbOptions.DropDownItems.Add(tsmiTips);

                ToolStripMenuItem tsmiShowInfo = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_position_and_size_info);
                tsmiShowInfo.Checked = Config.ShowInfo;
                tsmiShowInfo.CheckOnClick = true;
                tsmiShowInfo.Click += (sender, e) => Config.ShowInfo = tsmiShowInfo.Checked;
                tsddbOptions.DropDownItems.Add(tsmiShowInfo);

                ToolStripMenuItem tsmiShowMagnifier = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_magnifier);
                tsmiShowMagnifier.Checked = Config.ShowMagnifier;
                tsmiShowMagnifier.CheckOnClick = true;
                tsmiShowMagnifier.Click += (sender, e) => Config.ShowMagnifier = tsmiShowMagnifier.Checked;
                tsddbOptions.DropDownItems.Add(tsmiShowMagnifier);

                ToolStripMenuItem tsmiUseSquareMagnifier = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Square_shape_magnifier);
                tsmiUseSquareMagnifier.Checked = Config.UseSquareMagnifier;
                tsmiUseSquareMagnifier.CheckOnClick = true;
                tsmiUseSquareMagnifier.Click += (sender, e) => Config.UseSquareMagnifier = tsmiUseSquareMagnifier.Checked;
                tsddbOptions.DropDownItems.Add(tsmiUseSquareMagnifier);

                ToolStripLabeledNumericUpDown tslnudMagnifierPixelCount = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Magnifier_pixel_count_);
                tslnudMagnifierPixelCount.Content.Minimum = RegionCaptureOptions.MagnifierPixelCountMinimum;
                tslnudMagnifierPixelCount.Content.Maximum = RegionCaptureOptions.MagnifierPixelCountMaximum;
                tslnudMagnifierPixelCount.Content.Increment = 2;
                tslnudMagnifierPixelCount.Content.Value = Config.MagnifierPixelCount;
                tslnudMagnifierPixelCount.Content.ValueChanged = (sender, e) => Config.MagnifierPixelCount = (int)tslnudMagnifierPixelCount.Content.Value;
                tsddbOptions.DropDownItems.Add(tslnudMagnifierPixelCount);

                ToolStripLabeledNumericUpDown tslnudMagnifierPixelSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Magnifier_pixel_size_);
                tslnudMagnifierPixelSize.Content.Minimum = RegionCaptureOptions.MagnifierPixelSizeMinimum;
                tslnudMagnifierPixelSize.Content.Maximum = RegionCaptureOptions.MagnifierPixelSizeMaximum;
                tslnudMagnifierPixelSize.Content.Value = Config.MagnifierPixelSize;
                tslnudMagnifierPixelSize.Content.ValueChanged = (sender, e) => Config.MagnifierPixelSize = (int)tslnudMagnifierPixelSize.Content.Value;
                tsddbOptions.DropDownItems.Add(tslnudMagnifierPixelSize);

                ToolStripMenuItem tsmiShowCrosshair = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_screen_wide_crosshair);
                tsmiShowCrosshair.Checked = Config.ShowCrosshair;
                tsmiShowCrosshair.CheckOnClick = true;
                tsmiShowCrosshair.Click += (sender, e) => Config.ShowCrosshair = tsmiShowCrosshair.Checked;
                tsddbOptions.DropDownItems.Add(tsmiShowCrosshair);

                ToolStripMenuItem tsmiFixedSize = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Fixed_size_region_mode);
                tsmiFixedSize.Checked = Config.IsFixedSize;
                tsmiFixedSize.CheckOnClick = true;
                tsmiFixedSize.Click += (sender, e) => Config.IsFixedSize = tsmiFixedSize.Checked;
                tsddbOptions.DropDownItems.Add(tsmiFixedSize);

                ToolStripDoubleLabeledNumericUpDown tslnudFixedSize = new ToolStripDoubleLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Width_,
                    Resources.ShapeManager_CreateContextMenu_Height_);
                tslnudFixedSize.Content.Minimum = 10;
                tslnudFixedSize.Content.Maximum = 10000;
                tslnudFixedSize.Content.Increment = 10;
                tslnudFixedSize.Content.Value = Config.FixedSize.Width;
                tslnudFixedSize.Content.Value2 = Config.FixedSize.Height;
                tslnudFixedSize.Content.ValueChanged = (sender, e) => Config.FixedSize = new Size((int)tslnudFixedSize.Content.Value, (int)tslnudFixedSize.Content.Value2);
                tsddbOptions.DropDownItems.Add(tslnudFixedSize);

                ToolStripMenuItem tsmiShowFPS = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_FPS);
                tsmiShowFPS.Checked = Config.ShowFPS;
                tsmiShowFPS.CheckOnClick = true;
                tsmiShowFPS.Click += (sender, e) => Config.ShowFPS = tsmiShowFPS.Checked;
                tsddbOptions.DropDownItems.Add(tsmiShowFPS);
            }

            #endregion Options

            ToolStripLabel tslDragRight = new ToolStripLabel()
            {
                Alignment = ToolStripItemAlignment.Right,
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                Image = Resources.ui_radio_button_uncheck,
                Margin = new Padding(0),
                Padding = new Padding(2)
            };

            tslDragRight.MouseDown += (sender, e) =>
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.DefWindowProc(menuForm.Handle, (uint)WindowsMessages.SYSCOMMAND, (UIntPtr)NativeConstants.MOUSE_MOVE, IntPtr.Zero);
            };

            tsMain.Items.Add(tslDragRight);

            tsMain.ResumeLayout(false);
            tsMain.PerformLayout();
            menuForm.ResumeLayout(false);

            menuForm.Show(form);

            Rectangle rectActiveScreen = CaptureHelpers.GetActiveScreenBounds();

            if (tsMain.Width < rectActiveScreen.Width)
            {
                menuForm.Location = new Point(rectActiveScreen.X + rectActiveScreen.Width / 2 - tsMain.Width / 2, rectActiveScreen.Y + 20);

                menuForm.LocationChanged += (sender, e) =>
                {
                    Rectangle rectMenu = menuForm.Bounds;
                    Rectangle rectScreen = CaptureHelpers.GetScreenBounds();
                    Point pos = rectMenu.Location;

                    if (rectMenu.X < rectScreen.X)
                    {
                        pos.X = rectScreen.X;
                    }
                    else if (rectMenu.Right > rectScreen.Right)
                    {
                        pos.X = rectScreen.Right - rectMenu.Width;
                    }

                    if (rectMenu.Y < rectScreen.Y)
                    {
                        pos.Y = rectScreen.Y;
                    }
                    else if (rectMenu.Bottom > rectScreen.Bottom)
                    {
                        pos.Y = rectScreen.Bottom - rectMenu.Height;
                    }

                    if (pos != rectMenu.Location)
                    {
                        menuForm.Location = pos;
                    }
                };
            }
            else
            {
                menuForm.Location = rectActiveScreen.Location;
            }

            tsMain.MouseLeave += (sender, e) => MenuTextAnimation.Stop();

            foreach (ToolStripItem tsi in tsMain.Items.OfType<ToolStripItem>())
            {
                if (!string.IsNullOrEmpty(tsi.Text))
                {
                    tsi.MouseEnter += (sender, e) =>
                    {
                        Point pos = CaptureHelpers.ScreenToClient(menuForm.PointToScreen(tsi.Bounds.Location));
                        pos.Y += tsi.Height + 8;
                        MenuTextAnimation.Position = pos;
                        MenuTextAnimation.Start(tsi.Text);
                    };

                    tsi.MouseLeave += (sender, e) => MenuTextAnimation.Stop();
                }
            }

            form.Activate();

            UpdateMenu();

            CurrentShapeTypeChanged += shapeType => UpdateMenu();

            CurrentShapeChanged += shape => UpdateMenu();
        }

        private void UpdateMenu()
        {
            if (menuForm == null) return;

            ShapeType shapeType = CurrentShapeType;

            foreach (ToolStripButton tsb in tsMain.Items.OfType<ToolStripButton>().Where(x => x.Tag is ShapeType))
            {
                if ((ShapeType)tsb.Tag == shapeType)
                {
                    tsb.RadioCheck();
                    break;
                }
            }

            Color borderColor;

            if (shapeType == ShapeType.DrawingText || shapeType == ShapeType.DrawingSpeechBalloon)
            {
                borderColor = AnnotationOptions.TextBorderColor;
            }
            else if (shapeType == ShapeType.DrawingStep)
            {
                borderColor = AnnotationOptions.StepBorderColor;
            }
            else
            {
                borderColor = AnnotationOptions.BorderColor;
            }

            if (tsmiBorderColor.Image != null) tsmiBorderColor.Image.Dispose();
            tsmiBorderColor.Image = ImageHelpers.CreateColorPickerIcon(borderColor, new Rectangle(0, 0, 16, 16));

            int borderSize;

            if (shapeType == ShapeType.DrawingText || shapeType == ShapeType.DrawingSpeechBalloon)
            {
                borderSize = AnnotationOptions.TextBorderSize;
            }
            else if (shapeType == ShapeType.DrawingStep)
            {
                borderSize = AnnotationOptions.StepBorderSize;
            }
            else
            {
                borderSize = AnnotationOptions.BorderSize;
            }

            tslnudBorderSize.Content.Value = borderSize;

            Color fillColor;

            if (shapeType == ShapeType.DrawingText || shapeType == ShapeType.DrawingSpeechBalloon)
            {
                fillColor = AnnotationOptions.TextFillColor;
            }
            else if (shapeType == ShapeType.DrawingStep)
            {
                fillColor = AnnotationOptions.StepFillColor;
            }
            else
            {
                fillColor = AnnotationOptions.FillColor;
            }

            if (tsmiFillColor.Image != null) tsmiFillColor.Image.Dispose();
            tsmiFillColor.Image = ImageHelpers.CreateColorPickerIcon(fillColor, new Rectangle(0, 0, 16, 16));

            int cornerRadius = 0;

            if (shapeType == ShapeType.RegionRoundedRectangle || shapeType == ShapeType.DrawingRoundedRectangle)
            {
                cornerRadius = AnnotationOptions.RoundedRectangleRadius;
            }
            else if (shapeType == ShapeType.DrawingText)
            {
                cornerRadius = AnnotationOptions.TextCornerRadius;
            }

            tslnudCornerRadius.Content.Value = cornerRadius;

            tslnudBlurRadius.Content.Value = AnnotationOptions.BlurRadius;

            tslnudPixelateSize.Content.Value = AnnotationOptions.PixelateSize;

            if (tsmiHighlightColor.Image != null) tsmiHighlightColor.Image.Dispose();
            tsmiHighlightColor.Image = ImageHelpers.CreateColorPickerIcon(AnnotationOptions.HighlightColor, new Rectangle(0, 0, 16, 16));

            switch (shapeType)
            {
                default:
                    tsddbShapeOptions.Enabled = false;
                    break;
                case ShapeType.RegionRoundedRectangle:
                case ShapeType.DrawingRectangle:
                case ShapeType.DrawingRoundedRectangle:
                case ShapeType.DrawingEllipse:
                case ShapeType.DrawingFreehand:
                case ShapeType.DrawingLine:
                case ShapeType.DrawingArrow:
                case ShapeType.DrawingText:
                case ShapeType.DrawingSpeechBalloon:
                case ShapeType.DrawingStep:
                case ShapeType.EffectBlur:
                case ShapeType.EffectPixelate:
                case ShapeType.EffectHighlight:
                    tsddbShapeOptions.Enabled = true;
                    break;
            }

            tsbUndoObject.Enabled = tsbDeleteAll.Enabled = Shapes.Count > 0;

            switch (shapeType)
            {
                default:
                    tsmiBorderColor.Visible = false;
                    tslnudBorderSize.Visible = false;
                    break;
                case ShapeType.DrawingRectangle:
                case ShapeType.DrawingRoundedRectangle:
                case ShapeType.DrawingEllipse:
                case ShapeType.DrawingFreehand:
                case ShapeType.DrawingLine:
                case ShapeType.DrawingArrow:
                case ShapeType.DrawingText:
                case ShapeType.DrawingSpeechBalloon:
                case ShapeType.DrawingStep:
                    tsmiBorderColor.Visible = true;
                    tslnudBorderSize.Visible = true;
                    break;
            }

            switch (shapeType)
            {
                default:
                    tsmiFillColor.Visible = false;
                    break;
                case ShapeType.DrawingRectangle:
                case ShapeType.DrawingRoundedRectangle:
                case ShapeType.DrawingEllipse:
                case ShapeType.DrawingText:
                case ShapeType.DrawingSpeechBalloon:
                case ShapeType.DrawingStep:
                    tsmiFillColor.Visible = true;
                    break;
            }

            switch (shapeType)
            {
                default:
                    tslnudCornerRadius.Visible = false;
                    break;
                case ShapeType.RegionRoundedRectangle:
                case ShapeType.DrawingRoundedRectangle:
                case ShapeType.DrawingText:
                    tslnudCornerRadius.Visible = true;
                    break;
            }

            tslnudBlurRadius.Visible = shapeType == ShapeType.EffectBlur;
            tslnudPixelateSize.Visible = shapeType == ShapeType.EffectPixelate;
            tsmiHighlightColor.Visible = shapeType == ShapeType.EffectHighlight;

            if (tsmiRegionCapture != null)
            {
                tsmiRegionCapture.Visible = !Config.QuickCrop && ValidRegions.Length > 0;
            }
        }
    }
}