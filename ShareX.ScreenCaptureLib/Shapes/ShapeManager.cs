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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal class ShapeManager : IDisposable
    {
        public List<BaseShape> Shapes { get; private set; } = new List<BaseShape>();

        private BaseShape currentShape;

        public BaseShape CurrentShape
        {
            get
            {
                return currentShape;
            }
            private set
            {
                currentShape = value;

                if (currentShape != null)
                {
                    currentShape.OnConfigSave();
                }

                OnCurrentShapeChanged(currentShape);
            }
        }

        private ShapeType currentShapeType;

        public ShapeType CurrentShapeType
        {
            get
            {
                return currentShapeType;
            }
            private set
            {
                currentShapeType = value;

                if (form.Mode == RectangleRegionMode.Annotation)
                {
                    if (IsCurrentShapeTypeRegion)
                    {
                        Config.LastRegionTool = CurrentShapeType;
                    }
                    else
                    {
                        Config.LastAnnotationTool = CurrentShapeType;
                    }

                    UpdateCursor();
                }

                DeselectCurrentShape();

                OnCurrentShapeTypeChanged(currentShapeType);
            }
        }

        public Rectangle CurrentRectangle
        {
            get
            {
                if (CurrentShape != null)
                {
                    return CurrentShape.Rectangle;
                }

                return Rectangle.Empty;
            }
        }

        public bool IsCurrentShapeValid => CurrentShape != null && CurrentShape.IsValidShape;

        public BaseShape[] Regions => Shapes.OfType<BaseRegionShape>().ToArray();

        public BaseShape[] ValidRegions => Regions.Where(x => x.IsValidShape).ToArray();

        public BaseDrawingShape[] DrawingShapes => Shapes.OfType<BaseDrawingShape>().ToArray();

        public BaseEffectShape[] EffectShapes => Shapes.OfType<BaseEffectShape>().ToArray();

        public BaseShape CurrentHoverShape { get; private set; }

        public bool IsCurrentHoverShapeValid => CurrentHoverShape != null && CurrentHoverShape.IsValidShape;

        public bool IsCurrentShapeTypeRegion
        {
            get
            {
                switch (CurrentShapeType)
                {
                    case ShapeType.RegionRectangle:
                    case ShapeType.RegionRoundedRectangle:
                    case ShapeType.RegionEllipse:
                    case ShapeType.RegionFreehand:
                        return true;
                }

                return false;
            }
        }

        public bool IsCreating { get; set; }
        public bool IsMoving { get; set; }
        public bool IsResizing { get; set; }

        public bool IsCornerMoving { get; private set; }
        public bool IsProportionalResizing { get; private set; }
        public bool IsSnapResizing { get; private set; }

        public List<SimpleWindowInfo> Windows { get; set; }
        public bool WindowCaptureMode { get; set; }
        public bool IncludeControls { get; set; }

        public RegionCaptureOptions Config { get; private set; }

        public AnnotationOptions AnnotationOptions => Config.AnnotationOptions;

        public List<ResizeNode> ResizeNodes { get; private set; }

        private bool nodesVisible;

        public bool NodesVisible
        {
            get
            {
                return nodesVisible;
            }
            set
            {
                nodesVisible = value;

                if (!nodesVisible)
                {
                    foreach (ResizeNode node in ResizeNodes)
                    {
                        node.Visible = false;
                    }
                }
                else
                {
                    BaseShape shape = CurrentShape;

                    if (shape != null)
                    {
                        shape.OnNodePositionUpdate();
                        shape.OnNodeVisible();
                    }
                }
            }
        }

        public bool IsCursorOnNode => NodesVisible && ResizeNodes.Any(node => node.IsCursorHover);

        public event Action<BaseShape> CurrentShapeChanged;
        public event Action<ShapeType> CurrentShapeTypeChanged;

        private RectangleRegionForm form;
        private ContextMenuStrip cmsContextMenu;
        private ToolStripSeparator tssObjectOptions, tssShapeOptions;
        private ToolStripMenuItem tsmiDeleteSelected, tsmiDeleteAll, tsmiBorderColor, tsmiFillColor, tsmiHighlightColor, tsmiQuickCrop;
        private ToolStripLabeledNumericUpDown tslnudBorderSize, tslnudRoundedRectangleRadius, tslnudBlurRadius, tslnudPixelateSize;
        private bool isLeftPressed, isRightPressed, isUpPressed, isDownPressed;

        public ShapeManager(RectangleRegionForm form)
        {
            this.form = form;
            Config = form.Config;

            ResizeNodes = new List<ResizeNode>();

            for (int i = 0; i < 9; i++)
            {
                ResizeNode node = new ResizeNode();
                form.DrawableObjects.Add(node);
                ResizeNodes.Add(node);
            }

            ResizeNodes[(int)NodePosition.BottomRight].Order = 10;

            form.LostFocus += form_LostFocus;
            form.MouseDown += form_MouseDown;
            form.MouseUp += form_MouseUp;
            form.MouseDoubleClick += form_MouseDoubleClick;
            form.MouseWheel += form_MouseWheel;
            form.KeyDown += form_KeyDown;
            form.KeyUp += form_KeyUp;

            if (form.Mode == RectangleRegionMode.Annotation)
            {
                CreateContextMenu();
            }

            CurrentShape = null;

            if (form.Mode == RectangleRegionMode.Annotation)
            {
                CurrentShapeType = Config.LastRegionTool;
            }
            else
            {
                CurrentShapeType = ShapeType.RegionRectangle;
            }
        }

        private void CreateContextMenu()
        {
            cmsContextMenu = new ContextMenuStrip(form.components);
            cmsContextMenu.Renderer = new ToolStripCheckedBoldRenderer();
            cmsContextMenu.IgnoreSeparatorClick();

            cmsContextMenu.PreviewKeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    e.IsInputKey = true;
                }
            };

            cmsContextMenu.KeyUp += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    cmsContextMenu.Close();
                }
            };

            #region Main

            ToolStripMenuItem tsmiCancelCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Cancel_capture);
            tsmiCancelCapture.Image = Resources.prohibition;
            tsmiCancelCapture.Click += (sender, e) => form.Close(RegionResult.Close);
            cmsContextMenu.Items.Add(tsmiCancelCapture);

            ToolStripMenuItem tsmiCloseMenu = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Close_menu);
            tsmiCloseMenu.Image = Resources.cross;
            tsmiCloseMenu.Click += (sender, e) => cmsContextMenu.Close();
            cmsContextMenu.Items.Add(tsmiCloseMenu);

            #endregion Main

            #region Selected object

            tssObjectOptions = new ToolStripSeparator();
            cmsContextMenu.Items.Add(tssObjectOptions);

            tsmiDeleteSelected = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Delete_selected_object);
            tsmiDeleteSelected.Image = Resources.layer__minus;
            tsmiDeleteSelected.Click += (sender, e) => DeleteCurrentShape();
            cmsContextMenu.Items.Add(tsmiDeleteSelected);

            tsmiDeleteAll = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Delete_all_objects);
            tsmiDeleteAll.Image = Resources.minus;
            tsmiDeleteAll.Click += (sender, e) => DeleteAllShapes();
            cmsContextMenu.Items.Add(tsmiDeleteAll);

            #endregion Selected object

            #region Tools

            cmsContextMenu.Items.Add(new ToolStripSeparator());

            foreach (ShapeType shapeType in Helpers.GetEnums<ShapeType>())
            {
                ToolStripMenuItem tsmiShapeType = new ToolStripMenuItem(shapeType.GetLocalizedDescription());

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
                    case ShapeType.DrawingBlur:
                        img = Resources.layer_shade;
                        break;
                    case ShapeType.DrawingPixelate:
                        img = Resources.grid;
                        break;
                    case ShapeType.DrawingHighlight:
                        img = Resources.highlighter_text;
                        break;
                }

                tsmiShapeType.Image = img;

                tsmiShapeType.Checked = shapeType == CurrentShapeType;
                tsmiShapeType.Tag = shapeType;
                tsmiShapeType.Click += (sender, e) =>
                {
                    tsmiShapeType.RadioCheck();
                    CurrentShapeType = shapeType;
                };
                cmsContextMenu.Items.Add(tsmiShapeType);
            }

            #endregion Tools

            #region Shape options

            tssShapeOptions = new ToolStripSeparator();
            cmsContextMenu.Items.Add(tssShapeOptions);

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

                        UpdateContextMenu();
                        UpdateCurrentShape();
                        UpdateCursor();
                    }
                }

                ResumeForm();
            };
            cmsContextMenu.Items.Add(tsmiBorderColor);

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
            cmsContextMenu.Items.Add(tslnudBorderSize);

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

                        UpdateContextMenu();
                        UpdateCurrentShape();
                    }
                }

                ResumeForm();
            };
            cmsContextMenu.Items.Add(tsmiFillColor);

            tslnudRoundedRectangleRadius = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Corner_radius_);
            tslnudRoundedRectangleRadius.Content.Minimum = 0;
            tslnudRoundedRectangleRadius.Content.Maximum = 150;
            tslnudRoundedRectangleRadius.Content.Increment = 3;
            tslnudRoundedRectangleRadius.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.RoundedRectangleRadius = (int)tslnudRoundedRectangleRadius.Content.Value;
                UpdateCurrentShape();
            };
            cmsContextMenu.Items.Add(tslnudRoundedRectangleRadius);

            tslnudBlurRadius = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Blur_radius_);
            tslnudBlurRadius.Content.Minimum = 2;
            tslnudBlurRadius.Content.Maximum = 100;
            tslnudBlurRadius.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.BlurRadius = (int)tslnudBlurRadius.Content.Value;
                UpdateCurrentShape();
            };
            cmsContextMenu.Items.Add(tslnudBlurRadius);

            tslnudPixelateSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Pixel_size_);
            tslnudPixelateSize.Content.Minimum = 2;
            tslnudPixelateSize.Content.Maximum = 100;
            tslnudPixelateSize.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.PixelateSize = (int)tslnudPixelateSize.Content.Value;
                UpdateCurrentShape();
            };
            cmsContextMenu.Items.Add(tslnudPixelateSize);

            tsmiHighlightColor = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Highlight_color___);
            tsmiHighlightColor.Click += (sender, e) =>
            {
                PauseForm();

                using (ColorPickerForm dialogColor = new ColorPickerForm(AnnotationOptions.HighlightColor))
                {
                    if (dialogColor.ShowDialog() == DialogResult.OK)
                    {
                        AnnotationOptions.HighlightColor = dialogColor.NewColor;
                        UpdateContextMenu();
                        UpdateCurrentShape();
                    }
                }

                ResumeForm();
            };
            cmsContextMenu.Items.Add(tsmiHighlightColor);

            #endregion Shape options

            #region Capture

            cmsContextMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiFullscreenCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_fullscreen);
            tsmiFullscreenCapture.Image = Resources.layer_fullscreen;
            tsmiFullscreenCapture.Click += (sender, e) => form.Close(RegionResult.Fullscreen);
            cmsContextMenu.Items.Add(tsmiFullscreenCapture);

            ToolStripMenuItem tsmiActiveMonitorCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_active_monitor);
            tsmiActiveMonitorCapture.Image = Resources.monitor;
            tsmiActiveMonitorCapture.Click += (sender, e) => form.Close(RegionResult.ActiveMonitor);
            cmsContextMenu.Items.Add(tsmiActiveMonitorCapture);

            ToolStripMenuItem tsmiMonitorCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_monitor);
            tsmiMonitorCapture.HideImageMargin();
            tsmiMonitorCapture.Image = Resources.monitor_window;
            cmsContextMenu.Items.Add(tsmiMonitorCapture);

            tsmiMonitorCapture.DropDownItems.Clear();

            Screen[] screens = Screen.AllScreens;

            for (int i = 0; i < screens.Length; i++)
            {
                Screen screen = screens[i];
                ToolStripMenuItem tsmi = new ToolStripMenuItem(string.Format("{0}. {1}x{2}", i + 1, screen.Bounds.Width, screen.Bounds.Height));
                int index = i;
                tsmi.Click += (sender, e) =>
                {
                    form.MonitorIndex = index;
                    form.Close(RegionResult.Monitor);
                };
                tsmiMonitorCapture.DropDownItems.Add(tsmi);
            }

            #endregion Capture

            #region Options

            cmsContextMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiOptions = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Options);
            tsmiOptions.Image = Resources.gear;
            cmsContextMenu.Items.Add(tsmiOptions);

            tsmiQuickCrop = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Multi_region_mode);
            tsmiQuickCrop.Checked = !Config.QuickCrop;
            tsmiQuickCrop.CheckOnClick = true;
            tsmiQuickCrop.Click += (sender, e) => Config.QuickCrop = !tsmiQuickCrop.Checked;
            tsmiOptions.DropDownItems.Add(tsmiQuickCrop);

            ToolStripMenuItem tsmiTips = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_tips);
            tsmiTips.Checked = Config.ShowTips;
            tsmiTips.CheckOnClick = true;
            tsmiTips.Click += (sender, e) => Config.ShowTips = tsmiTips.Checked;
            tsmiOptions.DropDownItems.Add(tsmiTips);

            ToolStripMenuItem tsmiShowInfo = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_position_and_size_info);
            tsmiShowInfo.Checked = Config.ShowInfo;
            tsmiShowInfo.CheckOnClick = true;
            tsmiShowInfo.Click += (sender, e) => Config.ShowInfo = tsmiShowInfo.Checked;
            tsmiOptions.DropDownItems.Add(tsmiShowInfo);

            ToolStripMenuItem tsmiShowMagnifier = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_magnifier);
            tsmiShowMagnifier.Checked = Config.ShowMagnifier;
            tsmiShowMagnifier.CheckOnClick = true;
            tsmiShowMagnifier.Click += (sender, e) => Config.ShowMagnifier = tsmiShowMagnifier.Checked;
            tsmiOptions.DropDownItems.Add(tsmiShowMagnifier);

            ToolStripMenuItem tsmiUseSquareMagnifier = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Square_shape_magnifier);
            tsmiUseSquareMagnifier.Checked = Config.UseSquareMagnifier;
            tsmiUseSquareMagnifier.CheckOnClick = true;
            tsmiUseSquareMagnifier.Click += (sender, e) => Config.UseSquareMagnifier = tsmiUseSquareMagnifier.Checked;
            tsmiOptions.DropDownItems.Add(tsmiUseSquareMagnifier);

            ToolStripLabeledNumericUpDown tslnudMagnifierPixelCount = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Magnifier_pixel_count_);
            tslnudMagnifierPixelCount.Content.Minimum = RegionCaptureOptions.MagnifierPixelCountMinimum;
            tslnudMagnifierPixelCount.Content.Maximum = RegionCaptureOptions.MagnifierPixelCountMaximum;
            tslnudMagnifierPixelCount.Content.Increment = 2;
            tslnudMagnifierPixelCount.Content.Value = Config.MagnifierPixelCount;
            tslnudMagnifierPixelCount.Content.ValueChanged = (sender, e) => Config.MagnifierPixelCount = (int)tslnudMagnifierPixelCount.Content.Value;
            tsmiOptions.DropDownItems.Add(tslnudMagnifierPixelCount);

            ToolStripLabeledNumericUpDown tslnudMagnifierPixelSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Magnifier_pixel_size_);
            tslnudMagnifierPixelSize.Content.Minimum = RegionCaptureOptions.MagnifierPixelSizeMinimum;
            tslnudMagnifierPixelSize.Content.Maximum = RegionCaptureOptions.MagnifierPixelSizeMaximum;
            tslnudMagnifierPixelSize.Content.Value = Config.MagnifierPixelSize;
            tslnudMagnifierPixelSize.Content.ValueChanged = (sender, e) => Config.MagnifierPixelSize = (int)tslnudMagnifierPixelSize.Content.Value;
            tsmiOptions.DropDownItems.Add(tslnudMagnifierPixelSize);

            ToolStripMenuItem tsmiShowCrosshair = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_screen_wide_crosshair);
            tsmiShowCrosshair.Checked = Config.ShowCrosshair;
            tsmiShowCrosshair.CheckOnClick = true;
            tsmiShowCrosshair.Click += (sender, e) => Config.ShowCrosshair = tsmiShowCrosshair.Checked;
            tsmiOptions.DropDownItems.Add(tsmiShowCrosshair);

            ToolStripMenuItem tsmiFixedSize = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Fixed_size_region_mode);
            tsmiFixedSize.Checked = Config.IsFixedSize;
            tsmiFixedSize.CheckOnClick = true;
            tsmiFixedSize.Click += (sender, e) => Config.IsFixedSize = tsmiFixedSize.Checked;
            tsmiOptions.DropDownItems.Add(tsmiFixedSize);

            ToolStripDoubleLabeledNumericUpDown tslnudFixedSize = new ToolStripDoubleLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Width_,
                Resources.ShapeManager_CreateContextMenu_Height_);
            tslnudFixedSize.Content.Minimum = 10;
            tslnudFixedSize.Content.Maximum = 10000;
            tslnudFixedSize.Content.Increment = 10;
            tslnudFixedSize.Content.Value = Config.FixedSize.Width;
            tslnudFixedSize.Content.Value2 = Config.FixedSize.Height;
            tslnudFixedSize.Content.ValueChanged = (sender, e) => Config.FixedSize = new Size((int)tslnudFixedSize.Content.Value, (int)tslnudFixedSize.Content.Value2);
            tsmiOptions.DropDownItems.Add(tslnudFixedSize);

            ToolStripMenuItem tsmiShowFPS = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_FPS);
            tsmiShowFPS.Checked = Config.ShowFPS;
            tsmiShowFPS.CheckOnClick = true;
            tsmiShowFPS.Click += (sender, e) => Config.ShowFPS = tsmiShowFPS.Checked;
            tsmiOptions.DropDownItems.Add(tsmiShowFPS);

            #endregion Options

            CurrentShapeTypeChanged += shapeType => UpdateContextMenu();

            CurrentShapeChanged += shape => UpdateContextMenu();
        }

        private void UpdateContextMenu()
        {
            ShapeType shapeType = CurrentShapeType;

            tssObjectOptions.Visible = tsmiDeleteAll.Visible = Shapes.Count > 0;
            tsmiDeleteSelected.Visible = CurrentShape != null;

            foreach (ToolStripMenuItem tsmi in cmsContextMenu.Items.OfType<ToolStripMenuItem>().Where(x => x.Tag is ShapeType))
            {
                if ((ShapeType)tsmi.Tag == shapeType)
                {
                    tsmi.RadioCheck();
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

            tslnudRoundedRectangleRadius.Content.Value = AnnotationOptions.RoundedRectangleRadius;

            tslnudBlurRadius.Content.Value = AnnotationOptions.BlurRadius;

            tslnudPixelateSize.Content.Value = AnnotationOptions.PixelateSize;

            if (tsmiHighlightColor.Image != null) tsmiHighlightColor.Image.Dispose();
            tsmiHighlightColor.Image = ImageHelpers.CreateColorPickerIcon(AnnotationOptions.HighlightColor, new Rectangle(0, 0, 16, 16));

            switch (shapeType)
            {
                default:
                    tssShapeOptions.Visible = false;
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
                case ShapeType.DrawingBlur:
                case ShapeType.DrawingPixelate:
                case ShapeType.DrawingHighlight:
                    tssShapeOptions.Visible = true;
                    break;
            }

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

            tslnudRoundedRectangleRadius.Visible = shapeType == ShapeType.RegionRoundedRectangle || shapeType == ShapeType.DrawingRoundedRectangle;
            tslnudBlurRadius.Visible = shapeType == ShapeType.DrawingBlur;
            tslnudPixelateSize.Visible = shapeType == ShapeType.DrawingPixelate;
            tsmiHighlightColor.Visible = shapeType == ShapeType.DrawingHighlight;
        }

        private void form_LostFocus(object sender, EventArgs e)
        {
            IsCornerMoving = IsProportionalResizing = IsSnapResizing = false;
        }

        private void form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!IsCreating)
                {
                    StartRegionSelection();
                }
            }
        }

        private void form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsMoving || IsCreating)
                {
                    EndRegionSelection();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (IsCreating)
                {
                    DeleteCurrentShape();
                    EndRegionSelection();
                }
                else if (form.Mode == RectangleRegionMode.Annotation)
                {
                    RunAction(Config.MouseRightClickAction);
                }
                else if (IsShapeIntersect())
                {
                    DeleteIntersectShape();
                }
                else
                {
                    form.Close(RegionResult.Close);
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                RunAction(Config.MouseMiddleClickAction);
            }
            else if (e.Button == MouseButtons.XButton1)
            {
                RunAction(Config.Mouse4ClickAction);
            }
            else if (e.Button == MouseButtons.XButton2)
            {
                RunAction(Config.Mouse5ClickAction);
            }
        }

        private void form_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsCurrentShapeTypeRegion && ValidRegions.Length > 0)
                {
                    form.UpdateRegionPath();
                    form.Close(RegionResult.Region);
                }
                else if (CurrentShape != null && !IsCreating)
                {
                    CurrentShape.OnDoubleClicked();
                }
            }
        }

        private void form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys.HasFlag(Keys.Control) && form.Mode == RectangleRegionMode.Annotation)
            {
                if (e.Delta > 0)
                {
                    CurrentShapeType = CurrentShapeType.Previous<ShapeType>();
                }
                else if (e.Delta < 0)
                {
                    CurrentShapeType = CurrentShapeType.Next<ShapeType>();
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    Config.MagnifierPixelCount = Math.Min(Config.MagnifierPixelCount + 2, RegionCaptureOptions.MagnifierPixelCountMaximum);
                }
                else if (e.Delta < 0)
                {
                    Config.MagnifierPixelCount = Math.Max(Config.MagnifierPixelCount - 2, RegionCaptureOptions.MagnifierPixelCountMinimum);
                }
            }
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    IsCornerMoving = true;
                    break;
                case Keys.ShiftKey:
                    IsProportionalResizing = true;
                    break;
                case Keys.Menu:
                    IsSnapResizing = true;
                    break;
                case Keys.Left:
                case Keys.A:
                    isLeftPressed = true;
                    break;
                case Keys.Right:
                case Keys.D:
                    isRightPressed = true;
                    break;
                case Keys.Up:
                case Keys.W:
                    isUpPressed = true;
                    break;
                case Keys.Down:
                case Keys.S:
                    isDownPressed = true;
                    break;
            }

            switch (e.KeyData)
            {
                case Keys.Insert:
                    if (IsCreating)
                    {
                        EndRegionSelection();
                    }
                    else
                    {
                        StartRegionSelection();
                    }
                    break;
                case Keys.Control | Keys.Z:
                    UndoShape();
                    break;
            }

            if (form.Mode == RectangleRegionMode.Annotation && !IsCreating)
            {
                switch (e.KeyData)
                {
                    case Keys.Tab:
                        SwapShapeType();
                        break;
                    case Keys.NumPad0:
                        CurrentShapeType = ShapeType.RegionRectangle;
                        break;
                    case Keys.NumPad1:
                        CurrentShapeType = ShapeType.DrawingRectangle;
                        break;
                    case Keys.NumPad2:
                        CurrentShapeType = ShapeType.DrawingRoundedRectangle;
                        break;
                    case Keys.NumPad3:
                        CurrentShapeType = ShapeType.DrawingEllipse;
                        break;
                    case Keys.NumPad4:
                        CurrentShapeType = ShapeType.DrawingLine;
                        break;
                    case Keys.NumPad5:
                        CurrentShapeType = ShapeType.DrawingArrow;
                        break;
                    case Keys.NumPad6:
                        CurrentShapeType = ShapeType.DrawingText;
                        break;
                    case Keys.NumPad7:
                        CurrentShapeType = ShapeType.DrawingStep;
                        break;
                    case Keys.NumPad8:
                        CurrentShapeType = ShapeType.DrawingBlur;
                        break;
                    case Keys.NumPad9:
                        CurrentShapeType = ShapeType.DrawingPixelate;
                        break;
                    case Keys.Control | Keys.V:
                        PasteFromClipboard();
                        break;
                }
            }

            int speed;

            if (e.Shift)
            {
                speed = RegionCaptureOptions.MoveSpeedMaximum;
            }
            else
            {
                speed = RegionCaptureOptions.MoveSpeedMinimum;
            }

            int x = 0;

            if (isLeftPressed)
            {
                x -= speed;
            }

            if (isRightPressed)
            {
                x += speed;
            }

            int y = 0;

            if (isUpPressed)
            {
                y -= speed;
            }

            if (isDownPressed)
            {
                y += speed;
            }

            if (x != 0 || y != 0)
            {
                BaseShape shape = CurrentShape;

                if (shape == null || IsCreating)
                {
                    Cursor.Position = Cursor.Position.Add(x, y);
                }
                else
                {
                    if (e.Control)
                    {
                        shape.Move(x, y);
                    }
                    else
                    {
                        shape.Resize(x, y, !e.Alt);
                    }
                }
            }
        }

        private void form_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    IsCornerMoving = false;
                    break;
                case Keys.ShiftKey:
                    IsProportionalResizing = false;
                    break;
                case Keys.Menu:
                    IsSnapResizing = false;
                    break;
                case Keys.Left:
                case Keys.A:
                    isLeftPressed = false;
                    break;
                case Keys.Right:
                case Keys.D:
                    isRightPressed = false;
                    break;
                case Keys.Up:
                case Keys.W:
                    isUpPressed = false;
                    break;
                case Keys.Down:
                case Keys.S:
                    isDownPressed = false;
                    break;
            }

            switch (e.KeyData)
            {
                case Keys.Delete:
                    DeleteCurrentShape();

                    if (IsCreating)
                    {
                        EndRegionSelection();
                    }
                    break;
            }

            if (form.Mode == RectangleRegionMode.Annotation)
            {
                switch (e.KeyData)
                {
                    case Keys.Apps:
                        OpenOptionsMenu();
                        break;
                    case Keys.Q:
                        Config.QuickCrop = !Config.QuickCrop;
                        tsmiQuickCrop.Checked = !Config.QuickCrop;
                        break;
                }
            }
        }

        private void RunAction(RegionCaptureAction action)
        {
            switch (action)
            {
                case RegionCaptureAction.CancelCapture:
                    form.Close(RegionResult.Close);
                    break;
                case RegionCaptureAction.RemoveShapeCancelCapture:
                    if (IsShapeIntersect())
                    {
                        DeleteIntersectShape();
                    }
                    else
                    {
                        form.Close(RegionResult.Close);
                    }
                    break;
                case RegionCaptureAction.RemoveShape:
                    DeleteIntersectShape();
                    break;
                case RegionCaptureAction.OpenOptionsMenu:
                    OpenOptionsMenu();
                    break;
                case RegionCaptureAction.SwapToolType:
                    SwapShapeType();
                    break;
                case RegionCaptureAction.CaptureFullscreen:
                    form.Close(RegionResult.Fullscreen);
                    break;
                case RegionCaptureAction.CaptureActiveMonitor:
                    form.Close(RegionResult.ActiveMonitor);
                    break;
            }
        }

        public void Update()
        {
            OrderStepShapes();

            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                shape.OnUpdate();
            }

            CheckHover();

            UpdateNodes();
        }

        private void StartRegionSelection()
        {
            if (IsCursorOnNode)
            {
                return;
            }

            BaseShape shape = GetIntersectShape();

            if (shape != null && shape.ShapeType == CurrentShapeType) // Select shape
            {
                IsMoving = true;
                CurrentShape = shape;
                SelectCurrentShape();
            }
            else if (!IsCreating) // Create new shape
            {
                DeselectCurrentShape();

                shape = AddShape();
                shape.OnCreating();
            }
        }

        private void EndRegionSelection()
        {
            bool wasCreating = IsCreating;

            IsCreating = false;
            IsMoving = false;

            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                if (!shape.IsValidShape)
                {
                    shape.Rectangle = Rectangle.Empty;

                    CheckHover();

                    if (IsCurrentHoverShapeValid)
                    {
                        shape.Rectangle = CurrentHoverShape.Rectangle;
                    }
                    else
                    {
                        DeleteCurrentShape();
                        shape = null;
                    }
                }

                if (shape != null)
                {
                    if (Config.QuickCrop && IsCurrentShapeTypeRegion)
                    {
                        form.UpdateRegionPath();
                        form.Close(RegionResult.Region);
                    }
                    else
                    {
                        if (wasCreating)
                        {
                            shape.OnCreated();
                        }

                        SelectCurrentShape();
                    }
                }
            }
        }

        private BaseShape AddShape()
        {
            BaseShape shape = CreateShape();
            AddShape(shape);
            return shape;
        }

        private void AddShape(BaseShape shape)
        {
            Shapes.Add(shape);
            CurrentShape = shape;
        }

        private BaseShape CreateShape()
        {
            return CreateShape(CurrentShapeType);
        }

        private BaseShape CreateShape(ShapeType shapeType)
        {
            BaseShape shape;

            switch (shapeType)
            {
                default:
                case ShapeType.RegionRectangle:
                    shape = new RectangleRegionShape();
                    break;
                case ShapeType.RegionRoundedRectangle:
                    shape = new RoundedRectangleRegionShape();
                    break;
                case ShapeType.RegionEllipse:
                    shape = new EllipseRegionShape();
                    break;
                case ShapeType.RegionFreehand:
                    shape = new FreehandRegionShape();
                    break;
                case ShapeType.DrawingRectangle:
                    shape = new RectangleDrawingShape();
                    break;
                case ShapeType.DrawingRoundedRectangle:
                    shape = new RoundedRectangleDrawingShape();
                    break;
                case ShapeType.DrawingEllipse:
                    shape = new EllipseDrawingShape();
                    break;
                case ShapeType.DrawingFreehand:
                    shape = new FreehandDrawingShape();
                    break;
                case ShapeType.DrawingLine:
                    shape = new LineDrawingShape();
                    break;
                case ShapeType.DrawingArrow:
                    shape = new ArrowDrawingShape();
                    break;
                case ShapeType.DrawingText:
                    shape = new TextDrawingShape();
                    break;
                case ShapeType.DrawingSpeechBalloon:
                    shape = new SpeechBalloonDrawingShape();
                    break;
                case ShapeType.DrawingStep:
                    shape = new StepDrawingShape();
                    break;
                case ShapeType.DrawingImage:
                    shape = new ImageDrawingShape();
                    break;
                case ShapeType.DrawingBlur:
                    shape = new BlurEffectShape();
                    break;
                case ShapeType.DrawingPixelate:
                    shape = new PixelateEffectShape();
                    break;
                case ShapeType.DrawingHighlight:
                    shape = new HighlightEffectShape();
                    break;
            }

            shape.Manager = this;

            shape.OnConfigLoad();

            return shape;
        }

        private void UpdateCurrentShape()
        {
            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                shape.OnConfigLoad();
            }
        }

        private void SwapShapeType()
        {
            if (form.Mode == RectangleRegionMode.Annotation)
            {
                if (IsCurrentShapeTypeRegion)
                {
                    CurrentShapeType = Config.LastAnnotationTool;
                }
                else
                {
                    CurrentShapeType = Config.LastRegionTool;
                }
            }
        }

        private void OpenOptionsMenu()
        {
            SelectIntersectShape();

            if (form.Mode == RectangleRegionMode.Annotation && cmsContextMenu != null)
            {
                cmsContextMenu.Show(form, InputManager.MousePosition0Based.Add(-10, -10));
                Config.ShowMenuTip = false;
            }
        }

        public Point SnapPosition(Point posOnClick, Point posCurrent)
        {
            Size currentSize = CaptureHelpers.CreateRectangle(posOnClick, posCurrent).Size;
            Vector2 vector = new Vector2(currentSize.Width, currentSize.Height);

            SnapSize snapSize = (from size in Config.SnapSizes
                                 let distance = MathHelpers.Distance(vector, new Vector2(size.Width, size.Height))
                                 where distance > 0 && distance < RegionCaptureOptions.SnapDistance
                                 orderby distance
                                 select size).FirstOrDefault();

            if (snapSize != null)
            {
                Point posNew = CaptureHelpers.CalculateNewPosition(posOnClick, posCurrent, snapSize);

                Rectangle newRect = CaptureHelpers.CreateRectangle(posOnClick, posNew);

                if (form.ScreenRectangle0Based.Contains(newRect))
                {
                    return posNew;
                }
            }

            return posCurrent;
        }

        private void CheckHover()
        {
            CurrentHoverShape = null;

            if (!IsCursorOnNode && !IsCreating && !IsMoving && !IsResizing)
            {
                BaseShape shape = GetIntersectShape();

                if (shape != null && shape.IsValidShape)
                {
                    CurrentHoverShape = shape;
                }
                else
                {
                    switch (CurrentShapeType)
                    {
                        case ShapeType.RegionFreehand:
                        case ShapeType.DrawingFreehand:
                        case ShapeType.DrawingLine:
                        case ShapeType.DrawingArrow:
                        case ShapeType.DrawingText:
                        case ShapeType.DrawingSpeechBalloon:
                        case ShapeType.DrawingStep:
                        case ShapeType.DrawingImage:
                            return;
                    }

                    if (Config.IsFixedSize && IsCurrentShapeTypeRegion)
                    {
                        Point location = InputManager.MousePosition0Based;

                        CurrentHoverShape = new RectangleRegionShape()
                        {
                            Rectangle = new Rectangle(new Point(location.X - Config.FixedSize.Width / 2, location.Y - Config.FixedSize.Height / 2), Config.FixedSize)
                        };
                    }
                    else
                    {
                        SimpleWindowInfo window = FindSelectedWindow();

                        if (window != null && !window.Rectangle.IsEmpty)
                        {
                            Rectangle hoverArea = CaptureHelpers.ScreenToClient(window.Rectangle);

                            CurrentHoverShape = new RectangleRegionShape()
                            {
                                Rectangle = Rectangle.Intersect(form.ScreenRectangle0Based, hoverArea)
                            };
                        }
                    }
                }
            }
        }

        public SimpleWindowInfo FindSelectedWindow()
        {
            if (Windows != null)
            {
                return Windows.FirstOrDefault(x => x.Rectangle.Contains(InputManager.MousePosition));
            }

            return null;
        }

        public WindowInfo FindSelectedWindowInfo(Point position)
        {
            if (Windows != null)
            {
                SimpleWindowInfo windowInfo = Windows.FirstOrDefault(x => x.IsWindow && x.Rectangle.Contains(position));

                if (windowInfo != null)
                {
                    return windowInfo.WindowInfo;
                }
            }

            return null;
        }

        public Image RenderOutputImage(Image img)
        {
            Bitmap bmp = new Bitmap(img);

            if (DrawingShapes.Length > 0 || EffectShapes.Length > 0)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    foreach (BaseEffectShape shape in EffectShapes)
                    {
                        if (shape != null)
                        {
                            shape.OnDrawFinal(g, bmp);
                        }
                    }

                    foreach (BaseDrawingShape shape in DrawingShapes)
                    {
                        if (shape != null)
                        {
                            shape.OnDraw(g);
                        }
                    }
                }
            }

            return bmp;
        }

        private void SelectShape(BaseShape shape)
        {
            if (shape != null)
            {
                shape.ShowNodes();
            }
        }

        private void SelectCurrentShape()
        {
            SelectShape(CurrentShape);
        }

        private void SelectIntersectShape()
        {
            BaseShape shape = GetIntersectShape();

            if (shape != null)
            {
                CurrentShape = shape;
                SelectShape(shape);
            }
        }

        private void DeselectShape(BaseShape shape)
        {
            if (shape == CurrentShape)
            {
                CurrentShape = null;
                NodesVisible = false;
            }
        }

        private void DeselectCurrentShape()
        {
            DeselectShape(CurrentShape);
        }

        public void DeleteShape(BaseShape shape)
        {
            if (shape != null)
            {
                shape.Dispose();
                Shapes.Remove(shape);
                DeselectShape(shape);
            }
        }

        private void DeleteCurrentShape()
        {
            DeleteShape(CurrentShape);
        }

        private void DeleteIntersectShape()
        {
            DeleteShape(GetIntersectShape());
        }

        private void DeleteAllShapes()
        {
            foreach (BaseShape shape in Shapes)
            {
                shape.Dispose();
            }

            Shapes.Clear();
            DeselectCurrentShape();
        }

        public BaseShape GetIntersectShape()
        {
            return GetIntersectShape(InputManager.MousePosition0Based);
        }

        public BaseShape GetIntersectShape(Point position)
        {
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                BaseShape shape = Shapes[i];

                if (shape.ShapeType == CurrentShapeType && shape.Intersects(position))
                {
                    return shape;
                }
            }

            return null;
        }

        public bool IsShapeIntersect()
        {
            return GetIntersectShape() != null;
        }

        public void UndoShape()
        {
            if (Shapes.Count > 0)
            {
                DeleteShape(Shapes[Shapes.Count - 1]);
            }
        }

        private void UpdateNodes()
        {
            BaseShape shape = CurrentShape;

            if (shape != null && NodesVisible)
            {
                if (InputManager.IsMouseDown(MouseButtons.Left))
                {
                    shape.OnNodeUpdate();
                }
                else
                {
                    IsResizing = false;
                }

                shape.OnNodePositionUpdate();
            }
        }

        private void UpdateCursor()
        {
            try
            {
                Cursor cursor = Helpers.CreateCursor(Resources.Crosshair);

                if ((CurrentShapeType == ShapeType.DrawingRectangle || CurrentShapeType == ShapeType.DrawingRoundedRectangle || CurrentShapeType == ShapeType.DrawingEllipse ||
                    CurrentShapeType == ShapeType.DrawingFreehand || CurrentShapeType == ShapeType.DrawingLine || CurrentShapeType == ShapeType.DrawingArrow) &&
                    Config.AnnotationOptions.BorderSize > 0)
                {
                    using (Bitmap bmp = new Bitmap(32, 32))
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        if (Config.AnnotationOptions.BorderSize < 5)
                        {
                            using (Pen pen = new Pen(Config.AnnotationOptions.BorderColor, Config.AnnotationOptions.BorderSize) { Alignment = PenAlignment.Inset })
                            {
                                g.DrawRectangleProper(pen, new Rectangle(0, 0, 10, 10));
                            }
                        }
                        else
                        {
                            using (Brush brush = new SolidBrush(Config.AnnotationOptions.BorderColor))
                            {
                                g.FillRectangle(brush, new Rectangle(0, 0, 10, 10));
                            }
                        }

                        cursor.Draw(g, new Rectangle(0, 0, 32, 32));
                        cursor.Dispose();

                        IntPtr iconPtr = IntPtr.Zero;

                        try
                        {
                            iconPtr = bmp.GetHicon();
                            IconInfo iconInfo = new IconInfo();
                            NativeMethods.GetIconInfo(iconPtr, out iconInfo);
                            iconInfo.xHotspot = 15;
                            iconInfo.yHotspot = 15;
                            iconInfo.fIcon = false;
                            IntPtr newIconPtr = NativeMethods.CreateIconIndirect(ref iconInfo);
                            cursor = new Cursor(newIconPtr);
                        }
                        finally
                        {
                            if (iconPtr != IntPtr.Zero) NativeMethods.DestroyIcon(iconPtr);
                        }
                    }
                }

                Cursor temp = form.Cursor;
                form.Cursor = cursor;
                if (temp != null) temp.Dispose();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        public void PauseForm()
        {
            form.Pause();
        }

        public void ResumeForm()
        {
            form.Resume();
        }

        public void OrderStepShapes()
        {
            int i = 1;

            foreach (StepDrawingShape shape in Shapes.OfType<StepDrawingShape>())
            {
                shape.Number = i++;
            }
        }

        private void PasteFromClipboard()
        {
            if (Clipboard.ContainsImage())
            {
                Image img = ClipboardHelpers.GetImage();

                if (img != null)
                {
                    CurrentShapeType = ShapeType.DrawingImage;
                    ImageDrawingShape shape = (ImageDrawingShape)CreateShape(ShapeType.DrawingImage);
                    shape.StartPosition = shape.EndPosition = InputManager.MousePosition0Based;
                    shape.SetImage(img, true);
                    AddShape(shape);
                    SelectCurrentShape();
                }
            }
            else if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();

                if (!string.IsNullOrEmpty(text))
                {
                    CurrentShapeType = ShapeType.DrawingText;
                    TextDrawingShape shape = (TextDrawingShape)CreateShape(ShapeType.DrawingText);
                    shape.StartPosition = shape.EndPosition = InputManager.MousePosition0Based;
                    shape.Text = text.Trim();
                    shape.AutoSize(true);
                    AddShape(shape);
                    SelectCurrentShape();
                }
            }
        }

        private void OnCurrentShapeChanged(BaseShape shape)
        {
            if (CurrentShapeChanged != null)
            {
                CurrentShapeChanged(shape);
            }
        }

        private void OnCurrentShapeTypeChanged(ShapeType shapeType)
        {
            if (CurrentShapeTypeChanged != null)
            {
                CurrentShapeTypeChanged(shapeType);
            }
        }

        public void Dispose()
        {
            DeleteAllShapes();
        }
    }
}