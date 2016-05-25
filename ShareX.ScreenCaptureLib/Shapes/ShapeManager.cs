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
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class ShapeManager
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
                    currentShape.ApplyShapeConfig();
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

                DeselectShape();

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

        public bool IsCurrentRectangleValid
        {
            get
            {
                if (CurrentShape != null)
                {
                    return CurrentShape.IsValidShape;
                }

                return false;
            }
        }

        public BaseShape[] Regions
        {
            get
            {
                return Shapes.OfType<BaseRegionShape>().ToArray();
            }
        }

        public BaseDrawingShape[] DrawingShapes
        {
            get
            {
                return Shapes.OfType<BaseDrawingShape>().ToArray();
            }
        }

        public BaseEffectShape[] EffectShapes
        {
            get
            {
                return Shapes.OfType<BaseEffectShape>().ToArray();
            }
        }

        public BaseShape[] ValidRegions
        {
            get
            {
                return Regions.Where(x => x.IsValidShape).ToArray();
            }
        }

        public Rectangle CurrentHoverRectangle { get; private set; }

        public bool IsCurrentHoverAreaValid
        {
            get
            {
                return !CurrentHoverRectangle.IsEmpty;
            }
        }

        public bool IsCurrentShapeTypeRegion
        {
            get
            {
                return CurrentShapeType == ShapeType.RegionRectangle || CurrentShapeType == ShapeType.RegionRoundedRectangle || CurrentShapeType == ShapeType.RegionEllipse;
            }
        }

        public Point CurrentPosition { get; private set; }
        public Point PositionOnClick { get; private set; }

        public ResizeManager ResizeManager { get; private set; }
        public bool IsCreating { get; private set; }
        public bool IsMoving { get; private set; }

        public bool IsResizing
        {
            get
            {
                return ResizeManager.IsResizing;
            }
        }

        public bool IsProportionalResizing { get; private set; }
        public bool IsSnapResizing { get; private set; }

        public List<SimpleWindowInfo> Windows { get; set; }
        public bool WindowCaptureMode { get; set; }
        public bool IncludeControls { get; set; }

        public SurfaceOptions Config { get; private set; }

        public AnnotationOptions AnnotationOptions
        {
            get
            {
                return Config.AnnotationOptions;
            }
        }

        public event Action<BaseShape> CurrentShapeChanged;
        public event Action<ShapeType> CurrentShapeTypeChanged;

        private RectangleRegionForm form;

        private ContextMenuStrip cmsContextMenu;
        private ToolStripSeparator tssObjectOptions, tssShapeOptions;
        private ToolStripMenuItem tsmiDeleteSelected, tsmiDeleteAll, tsmiBorderColor, tsmiFillColor, tsmiHighlightColor;
        private ToolStripLabeledNumericUpDown tslnudBorderSize, tslnudRoundedRectangleRadius, tslnudBlurRadius, tslnudPixelateSize;

        public ShapeManager(RectangleRegionForm form)
        {
            this.form = form;
            Config = form.Config;

            ResizeManager = new ResizeManager(form, this);

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
            CurrentShapeType = ShapeType.RegionRectangle;
        }

        private void CreateContextMenu()
        {
            cmsContextMenu = new ContextMenuStrip(form.components);
            cmsContextMenu.Renderer = new ToolStripCheckedBoldRenderer();

            #region Main

            ToolStripMenuItem tsmiCancelCapture = new ToolStripMenuItem("Cancel capture");
            tsmiCancelCapture.Image = Resources.prohibition;
            tsmiCancelCapture.Click += (sender, e) => form.Close(RegionResult.Close);
            cmsContextMenu.Items.Add(tsmiCancelCapture);

            ToolStripMenuItem tsmiCloseMenu = new ToolStripMenuItem("Close menu");
            tsmiCloseMenu.Image = Resources.cross;
            tsmiCloseMenu.Click += (sender, e) => cmsContextMenu.Close();
            cmsContextMenu.Items.Add(tsmiCloseMenu);

            #endregion Main

            #region Selected object

            tssObjectOptions = new ToolStripSeparator();
            cmsContextMenu.Items.Add(tssObjectOptions);

            tsmiDeleteSelected = new ToolStripMenuItem("Delete selected object");
            tsmiDeleteSelected.Image = Resources.layer__minus;
            tsmiDeleteSelected.Click += (sender, e) => DeleteCurrentShape();
            cmsContextMenu.Items.Add(tsmiDeleteSelected);

            tsmiDeleteAll = new ToolStripMenuItem("Delete all objects");
            tsmiDeleteAll.Image = Resources.minus;
            tsmiDeleteAll.Click += (sender, e) => ClearAll();
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
                    case ShapeType.DrawingRectangle:
                        img = Resources.layer_shape;
                        break;
                    case ShapeType.DrawingRoundedRectangle:
                        img = Resources.layer_shape_round;
                        break;
                    case ShapeType.DrawingEllipse:
                        img = Resources.layer_shape_ellipse;
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
                    case ShapeType.DrawingStep:
                        img = Resources.counter_reset;
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

            tsmiBorderColor = new ToolStripMenuItem("Border color...");
            tsmiBorderColor.Click += (sender, e) =>
            {
                PauseForm();

                ShapeType shapeType = CurrentShapeType;

                Color borderColor;

                if (shapeType == ShapeType.DrawingText)
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
                        if (shapeType == ShapeType.DrawingText)
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
                    }
                }

                ResumeForm();
            };
            cmsContextMenu.Items.Add(tsmiBorderColor);

            tslnudBorderSize = new ToolStripLabeledNumericUpDown("Border size:");
            tslnudBorderSize.Content.Minimum = 0;
            tslnudBorderSize.Content.Maximum = 20;
            tslnudBorderSize.Content.ValueChanged = (sender, e) =>
            {
                ShapeType shapeType = CurrentShapeType;

                int borderSize = (int)tslnudBorderSize.Content.Value;

                if (shapeType == ShapeType.DrawingText)
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
            };
            cmsContextMenu.Items.Add(tslnudBorderSize);

            tsmiFillColor = new ToolStripMenuItem("Fill color...");
            tsmiFillColor.Click += (sender, e) =>
            {
                PauseForm();

                ShapeType shapeType = CurrentShapeType;

                Color fillColor;

                if (shapeType == ShapeType.DrawingText)
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
                        if (shapeType == ShapeType.DrawingText)
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

            tslnudRoundedRectangleRadius = new ToolStripLabeledNumericUpDown("Corner radius:");
            tslnudRoundedRectangleRadius.Content.Minimum = 0;
            tslnudRoundedRectangleRadius.Content.Maximum = 150;
            tslnudRoundedRectangleRadius.Content.Increment = 3;
            tslnudRoundedRectangleRadius.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.RoundedRectangleRadius = (int)tslnudRoundedRectangleRadius.Content.Value;
                UpdateCurrentShape();
            };
            cmsContextMenu.Items.Add(tslnudRoundedRectangleRadius);

            tslnudBlurRadius = new ToolStripLabeledNumericUpDown("Blur radius:");
            tslnudBlurRadius.Content.Minimum = 2;
            tslnudBlurRadius.Content.Maximum = 100;
            tslnudBlurRadius.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.BlurRadius = (int)tslnudBlurRadius.Content.Value;
                UpdateCurrentShape();
            };
            cmsContextMenu.Items.Add(tslnudBlurRadius);

            tslnudPixelateSize = new ToolStripLabeledNumericUpDown("Pixel size:");
            tslnudPixelateSize.Content.Minimum = 2;
            tslnudPixelateSize.Content.Maximum = 100;
            tslnudPixelateSize.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.PixelateSize = (int)tslnudPixelateSize.Content.Value;
                UpdateCurrentShape();
            };
            cmsContextMenu.Items.Add(tslnudPixelateSize);

            tsmiHighlightColor = new ToolStripMenuItem("Highlight color...");
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

            ToolStripMenuItem tsmiFullscreenCapture = new ToolStripMenuItem("Capture fullscreen");
            tsmiFullscreenCapture.Image = Resources.layer_fullscreen;
            tsmiFullscreenCapture.Click += (sender, e) => form.Close(RegionResult.Fullscreen);
            cmsContextMenu.Items.Add(tsmiFullscreenCapture);

            ToolStripMenuItem tsmiActiveMonitorCapture = new ToolStripMenuItem("Capture active monitor");
            tsmiActiveMonitorCapture.Image = Resources.monitor;
            tsmiActiveMonitorCapture.Click += (sender, e) => form.Close(RegionResult.ActiveMonitor);
            cmsContextMenu.Items.Add(tsmiActiveMonitorCapture);

            ToolStripMenuItem tsmiMonitorCapture = new ToolStripMenuItem("Capture monitor");
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

            ToolStripMenuItem tsmiOptions = new ToolStripMenuItem("Options");
            tsmiOptions.Image = Resources.gear;
            cmsContextMenu.Items.Add(tsmiOptions);

            ToolStripMenuItem tsmiQuickCrop = new ToolStripMenuItem("Multi region mode");
            tsmiQuickCrop.Checked = !Config.QuickCrop;
            tsmiQuickCrop.CheckOnClick = true;
            tsmiQuickCrop.Click += (sender, e) => Config.QuickCrop = !tsmiQuickCrop.Checked;
            tsmiOptions.DropDownItems.Add(tsmiQuickCrop);

            ToolStripMenuItem tsmiShowInfo = new ToolStripMenuItem("Show position and size info");
            tsmiShowInfo.Checked = Config.ShowInfo;
            tsmiShowInfo.CheckOnClick = true;
            tsmiShowInfo.Click += (sender, e) => Config.ShowInfo = tsmiShowInfo.Checked;
            tsmiOptions.DropDownItems.Add(tsmiShowInfo);

            ToolStripMenuItem tsmiShowMagnifier = new ToolStripMenuItem("Show magnifier");
            tsmiShowMagnifier.Checked = Config.ShowMagnifier;
            tsmiShowMagnifier.CheckOnClick = true;
            tsmiShowMagnifier.Click += (sender, e) => Config.ShowMagnifier = tsmiShowMagnifier.Checked;
            tsmiOptions.DropDownItems.Add(tsmiShowMagnifier);

            ToolStripMenuItem tsmiUseSquareMagnifier = new ToolStripMenuItem("Square shape magnifier");
            tsmiUseSquareMagnifier.Checked = Config.UseSquareMagnifier;
            tsmiUseSquareMagnifier.CheckOnClick = true;
            tsmiUseSquareMagnifier.Click += (sender, e) => Config.UseSquareMagnifier = tsmiUseSquareMagnifier.Checked;
            tsmiOptions.DropDownItems.Add(tsmiUseSquareMagnifier);

            ToolStripLabeledNumericUpDown tslnudMagnifierPixelCount = new ToolStripLabeledNumericUpDown("Magnifier pixel count:");
            tslnudMagnifierPixelCount.Content.Minimum = 1;
            tslnudMagnifierPixelCount.Content.Maximum = 35;
            tslnudMagnifierPixelCount.Content.Increment = 2;
            tslnudMagnifierPixelCount.Content.Value = Config.MagnifierPixelCount;
            tslnudMagnifierPixelCount.Content.ValueChanged = (sender, e) => Config.MagnifierPixelCount = (int)tslnudMagnifierPixelCount.Content.Value;
            tsmiOptions.DropDownItems.Add(tslnudMagnifierPixelCount);

            ToolStripLabeledNumericUpDown tslnudMagnifierPixelSize = new ToolStripLabeledNumericUpDown("Magnifier pixel size:");
            tslnudMagnifierPixelSize.Content.Minimum = 2;
            tslnudMagnifierPixelSize.Content.Maximum = 30;
            tslnudMagnifierPixelSize.Content.Value = Config.MagnifierPixelSize;
            tslnudMagnifierPixelSize.Content.ValueChanged = (sender, e) => Config.MagnifierPixelSize = (int)tslnudMagnifierPixelSize.Content.Value;
            tsmiOptions.DropDownItems.Add(tslnudMagnifierPixelSize);

            ToolStripMenuItem tsmiShowCrosshair = new ToolStripMenuItem("Show screen wide crosshair");
            tsmiShowCrosshair.Checked = Config.ShowCrosshair;
            tsmiShowCrosshair.CheckOnClick = true;
            tsmiShowCrosshair.Click += (sender, e) => Config.ShowCrosshair = tsmiShowCrosshair.Checked;
            tsmiOptions.DropDownItems.Add(tsmiShowCrosshair);

            ToolStripMenuItem tsmiFixedSize = new ToolStripMenuItem("Fixed size mode");
            tsmiFixedSize.Checked = Config.IsFixedSize;
            tsmiFixedSize.CheckOnClick = true;
            tsmiFixedSize.Click += (sender, e) => Config.IsFixedSize = tsmiFixedSize.Checked;
            tsmiOptions.DropDownItems.Add(tsmiFixedSize);

            ToolStripDoubleLabeledNumericUpDown tslnudFixedSize = new ToolStripDoubleLabeledNumericUpDown("Width:", "Height:");
            tslnudFixedSize.Content.Minimum = 5;
            tslnudFixedSize.Content.Maximum = 10000;
            tslnudFixedSize.Content.Increment = 10;
            tslnudFixedSize.Content.Value = Config.FixedSize.Width;
            tslnudFixedSize.Content.Value2 = Config.FixedSize.Height;
            tslnudFixedSize.Content.ValueChanged = (sender, e) => Config.FixedSize = new Size((int)tslnudFixedSize.Content.Value, (int)tslnudFixedSize.Content.Value2);
            tsmiOptions.DropDownItems.Add(tslnudFixedSize);

            ToolStripMenuItem tsmiShowFPS = new ToolStripMenuItem("Show FPS");
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

            if (shapeType == ShapeType.DrawingText)
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

            if (shapeType == ShapeType.DrawingText)
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

            if (shapeType == ShapeType.DrawingText)
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
                case ShapeType.DrawingLine:
                case ShapeType.DrawingArrow:
                case ShapeType.DrawingText:
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
                case ShapeType.DrawingLine:
                case ShapeType.DrawingArrow:
                case ShapeType.DrawingText:
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
                case ShapeType.DrawingStep:
                    tsmiFillColor.Visible = true;
                    break;
            }

            tslnudRoundedRectangleRadius.Visible = shapeType == ShapeType.RegionRoundedRectangle || shapeType == ShapeType.DrawingRoundedRectangle;
            tslnudBlurRadius.Visible = shapeType == ShapeType.DrawingBlur;
            tslnudPixelateSize.Visible = shapeType == ShapeType.DrawingPixelate;
            tsmiHighlightColor.Visible = shapeType == ShapeType.DrawingHighlight;
        }

        private void form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!IsCreating)
                {
                    RegionSelection(InputManager.MousePosition0Based);
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
                else if (form.Mode == RectangleRegionMode.Annotation && cmsContextMenu != null)
                {
                    cmsContextMenu.Show(form, e.Location.Add(-10, -10));
                    Config.ShowMenuTip = false;
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
                form.Close(RegionResult.Close);
            }
        }

        private void form_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsCurrentShapeTypeRegion)
                {
                    form.Close(RegionResult.Region);
                }
                else if (CurrentShape != null)
                {
                    EndRegionSelection();

                    CurrentShape.OnShapeDoubleClicked();
                }
            }
        }

        private void form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys.HasFlag(Keys.Control))
            {
                if (e.Delta > 0)
                {
                    if (Config.MagnifierPixelCount < 41) Config.MagnifierPixelCount += 2;
                }
                else if (e.Delta < 0)
                {
                    if (Config.MagnifierPixelCount > 2) Config.MagnifierPixelCount -= 2;
                }
            }
            else
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
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Insert:
                    if (IsCreating)
                    {
                        EndRegionSelection();
                    }
                    else
                    {
                        if (ResizeManager.Visible)
                        {
                            DeselectShape();
                        }

                        if (CurrentShape == null || CurrentShape != GetShapeIntersect())
                        {
                            RegionSelection(InputManager.MousePosition0Based);
                        }
                    }
                    break;
                case Keys.ShiftKey:
                    IsProportionalResizing = true;
                    break;
                case Keys.Menu:
                    IsSnapResizing = true;
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
                    CurrentShapeType = ShapeType.DrawingBlur;
                    break;
                case Keys.NumPad8:
                    CurrentShapeType = ShapeType.DrawingPixelate;
                    break;
                case Keys.NumPad9:
                    CurrentShapeType = ShapeType.DrawingHighlight;
                    break;
            }
        }

        private void form_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey:
                    IsProportionalResizing = false;
                    break;
                case Keys.Menu:
                    IsSnapResizing = false;
                    break;
                case Keys.Delete:
                    DeleteCurrentShape();

                    if (IsCreating)
                    {
                        EndRegionSelection();
                    }
                    break;
            }
        }

        public void Update()
        {
            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                if (IsMoving)
                {
                    ResizeManager.MoveCurrentArea(InputManager.MouseVelocity.X, InputManager.MouseVelocity.Y);
                }
                else if (IsCreating && !CurrentRectangle.IsEmpty)
                {
                    CurrentPosition = InputManager.MousePosition0Based;

                    Point newPosition = CurrentPosition;

                    if (IsProportionalResizing)
                    {
                        if (shape.NodeType == NodeType.Rectangle)
                        {
                            newPosition = CaptureHelpers.SnapPositionToDegree(PositionOnClick, CurrentPosition, 90, 45);
                        }
                        else if (shape.NodeType == NodeType.Line)
                        {
                            newPosition = CaptureHelpers.SnapPositionToDegree(PositionOnClick, CurrentPosition, 45, 0);
                        }
                    }

                    if (IsSnapResizing)
                    {
                        newPosition = SnapPosition(PositionOnClick, newPosition);
                    }

                    shape.EndPosition = newPosition;
                }
            }

            CheckHover();

            ResizeManager.Update();
        }

        private void RegionSelection(Point location)
        {
            if (ResizeManager.IsCursorOnNode())
            {
                return;
            }

            PositionOnClick = location;

            BaseShape shape = GetShapeIntersect(location);

            if (shape != null && shape.ShapeType == CurrentShapeType) // Select shape
            {
                IsMoving = true;
                CurrentShape = shape;
                SelectShape();
            }
            else if (!IsCreating) // Create new shape
            {
                DeselectShape();

                shape = AddShape();

                if (shape.NodeType == NodeType.Point)
                {
                    IsMoving = true;
                    shape.Rectangle = new Rectangle(new Point(location.X - shape.Rectangle.Width / 2, location.Y - shape.Rectangle.Height / 2), shape.Rectangle.Size);
                }
                else if (Config.IsFixedSize && IsCurrentShapeTypeRegion)
                {
                    IsMoving = true;
                    shape.Rectangle = new Rectangle(new Point(location.X - Config.FixedSize.Width / 2, location.Y - Config.FixedSize.Height / 2), Config.FixedSize);
                }
                else
                {
                    IsCreating = true;
                    shape.StartPosition = location;
                }
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
                if (!IsCurrentRectangleValid)
                {
                    shape.Rectangle = Rectangle.Empty;

                    CheckHover();

                    if (IsCurrentHoverAreaValid)
                    {
                        shape.Rectangle = CurrentHoverRectangle;
                    }
                    else
                    {
                        DeleteCurrentShape();
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
                            shape.OnShapeCreated();
                        }

                        SelectShape();
                    }
                }
            }
        }

        private BaseShape AddShape(Rectangle rect)
        {
            BaseShape shape = AddShape();
            shape.Rectangle = rect;
            return shape;
        }

        private BaseShape AddShape()
        {
            BaseShape shape = CreateShape();
            Shapes.Add(shape);
            CurrentShape = shape;
            return shape;
        }

        public BaseShape CreateShape(Rectangle rect)
        {
            BaseShape shape = CreateShape();
            shape.Rectangle = rect;
            return shape;
        }

        public BaseShape CreateShape()
        {
            BaseShape shape;

            switch (CurrentShapeType)
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
                case ShapeType.DrawingRectangle:
                    shape = new RectangleDrawingShape();
                    break;
                case ShapeType.DrawingRoundedRectangle:
                    shape = new RoundedRectangleDrawingShape();
                    break;
                case ShapeType.DrawingEllipse:
                    shape = new EllipseDrawingShape();
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
                case ShapeType.DrawingStep:
                    shape = new StepDrawingShape();
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

            shape.UpdateShapeConfig();

            return shape;
        }

        private void UpdateCurrentShape()
        {
            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                shape.UpdateShapeConfig();
            }
        }

        private Point SnapPosition(Point posOnClick, Point posCurrent)
        {
            Rectangle currentRect = CaptureHelpers.CreateRectangle(posOnClick, posCurrent);
            Point newPosition = posCurrent;

            foreach (SnapSize size in Config.SnapSizes)
            {
                if (currentRect.Width.IsBetween(size.Width - Config.SnapDistance, size.Width + Config.SnapDistance) ||
                    currentRect.Height.IsBetween(size.Height - Config.SnapDistance, size.Height + Config.SnapDistance))
                {
                    newPosition = CaptureHelpers.CalculateNewPosition(posOnClick, posCurrent, size);
                    break;
                }
            }

            Rectangle newRect = CaptureHelpers.CreateRectangle(posOnClick, newPosition);

            if (form.ScreenRectangle0Based.Contains(newRect))
            {
                return newPosition;
            }

            return posCurrent;
        }

        private void CheckHover()
        {
            CurrentHoverRectangle = Rectangle.Empty;

            if (!ResizeManager.IsCursorOnNode() && !IsCreating && !IsMoving && !IsResizing)
            {
                BaseShape shape = GetShapeIntersect();

                if (shape != null && !shape.Rectangle.IsEmpty)
                {
                    CurrentHoverRectangle = shape.Rectangle;
                }
                else
                {
                    switch (CurrentShapeType)
                    {
                        case ShapeType.DrawingLine:
                        case ShapeType.DrawingArrow:
                        case ShapeType.DrawingStep:
                            return;
                    }

                    if (Config.IsFixedSize && IsCurrentShapeTypeRegion)
                    {
                        Point location = InputManager.MousePosition0Based;
                        CurrentHoverRectangle = new Rectangle(new Point(location.X - Config.FixedSize.Width / 2, location.Y - Config.FixedSize.Height / 2), Config.FixedSize);
                    }
                    else
                    {
                        SimpleWindowInfo window = FindSelectedWindow();

                        if (window != null && !window.Rectangle.IsEmpty)
                        {
                            Rectangle hoverArea = CaptureHelpers.ScreenToClient(window.Rectangle);
                            CurrentHoverRectangle = Rectangle.Intersect(form.ScreenRectangle0Based, hoverArea);
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

        public WindowInfo FindSelectedWindowInfo(Point mousePosition)
        {
            if (Windows != null)
            {
                SimpleWindowInfo windowInfo = Windows.FirstOrDefault(x => x.IsWindow && x.Rectangle.Contains(mousePosition));

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

        private void SelectShape()
        {
            BaseShape shape = CurrentShape;

            if (shape != null && !CurrentRectangle.IsEmpty && !Config.IsFixedSize && shape.NodeType != NodeType.Point)
            {
                ResizeManager.Show();
            }
        }

        private void DeselectShape()
        {
            CurrentShape = null;
            ResizeManager.Hide();
        }

        private void DeleteCurrentShape()
        {
            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                Shapes.Remove(shape);
                DeselectShape();
            }
        }

        private void DeleteIntersectShape()
        {
            BaseShape shape = GetShapeIntersect();

            if (shape != null)
            {
                Shapes.Remove(shape);
                DeselectShape();
            }
        }

        private void ClearAll()
        {
            Shapes.Clear();
            DeselectShape();
        }

        public BaseShape GetShapeIntersect(Point mousePosition)
        {
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                BaseShape shape = Shapes[i];

                if (shape.ShapeType == CurrentShapeType && shape.Rectangle.Contains(mousePosition))
                {
                    return shape;
                }
            }

            return null;
        }

        public BaseShape GetShapeIntersect()
        {
            return GetShapeIntersect(InputManager.MousePosition0Based);
        }

        public bool IsShapeIntersect()
        {
            return GetShapeIntersect() != null;
        }

        public Rectangle CombineAreas()
        {
            BaseShape[] areas = ValidRegions;

            if (areas.Length > 0)
            {
                Rectangle rect = areas[0].Rectangle;

                for (int i = 1; i < areas.Length; i++)
                {
                    rect = Rectangle.Union(rect, areas[i].Rectangle);
                }

                return rect;
            }

            return Rectangle.Empty;
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
    }
}