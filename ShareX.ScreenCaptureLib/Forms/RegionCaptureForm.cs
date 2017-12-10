#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public sealed class RegionCaptureForm : Form
    {
        public static GraphicsPath LastRegionFillPath { get; private set; }

        public RegionCaptureOptions Options { get; set; }
        public Rectangle ClientArea { get; private set; }
        public Image Canvas { get; private set; }
        public Rectangle CanvasRectangle { get; private set; }
        public RegionResult Result { get; private set; }
        public int FPS { get; private set; }
        public int MonitorIndex { get; set; }
        public string ImageFilePath { get; set; }
        public bool IsFullscreen { get; private set; }

        public RegionCaptureMode Mode { get; private set; }
        public bool IsEditorMode => Mode == RegionCaptureMode.Editor || Mode == RegionCaptureMode.TaskEditor;
        public bool IsAnnotationMode => Mode == RegionCaptureMode.Annotation || IsEditorMode;
        public bool IsAnnotated => ShapeManager != null && ShapeManager.IsAnnotated;

        public Point CurrentPosition { get; private set; }
        public Point PanningStrech = new Point();

        public Color CurrentColor
        {
            get
            {
                if (bmpBackgroundImage != null)
                {
                    Point position = CaptureHelpers.ScreenToClient(CurrentPosition);

                    if (position.X.IsBetween(0, bmpBackgroundImage.Width - 1) && position.Y.IsBetween(0, bmpBackgroundImage.Height - 1))
                    {
                        return bmpBackgroundImage.GetPixel(position.X, position.Y);
                    }
                }

                return Color.Empty;
            }
        }

        public SimpleWindowInfo SelectedWindow { get; private set; }

        public Vector2 CanvasCenterOffset { get; set; } = new Vector2(0f, 0f);

        internal ShapeManager ShapeManager { get; private set; }
        internal List<DrawableObject> DrawableObjects { get; private set; }
        internal bool IsClosing { get; private set; }

        internal Image CustomNodeImage = Resources.CircleNode;
        internal int ToolbarHeight;

        private InputManager InputManager => ShapeManager.InputManager;

        private TextureBrush backgroundBrush, backgroundHighlightBrush;
        private GraphicsPath regionFillPath, regionDrawPath;
        private Pen borderPen, borderDotPen, borderDotStaticPen, textOuterBorderPen, textInnerBorderPen, markerPen, canvasBorderPen;
        private Brush nodeBackgroundBrush, textBackgroundBrush;
        private Font infoFont, infoFontMedium, infoFontBig;
        private Stopwatch timerStart, timerFPS;
        private int frameCount;
        private bool pause, isKeyAllowed;
        private RectangleAnimation regionAnimation;
        private TextAnimation editorPanTipAnimation;
        private Bitmap bmpBackgroundImage;
        private Cursor defaultCursor;
        private ScrollbarManager scrollbarManager;

        public RegionCaptureForm(RegionCaptureMode mode, RegionCaptureOptions options, Image canvas = null)
        {
            Mode = mode;
            Options = options;
            IsFullscreen = !IsEditorMode || Options.ImageEditorStartMode == ImageEditorStartMode.Fullscreen;

            ClientArea = CaptureHelpers.GetScreenBounds0Based();
            CanvasRectangle = ClientArea;

            DrawableObjects = new List<DrawableObject>();
            timerStart = new Stopwatch();
            timerFPS = new Stopwatch();
            regionAnimation = new RectangleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(200)
            };

            if (IsEditorMode)
            {
                scrollbarManager = new ScrollbarManager(this);

                if (Options.ShowEditorPanTip)
                {
                    editorPanTipAnimation = new TextAnimation()
                    {
                        Duration = TimeSpan.FromMilliseconds(5000),
                        FadeOutDuration = TimeSpan.FromMilliseconds(1000),
                        Text = Resources.RegionCaptureForm_TipYouCanPanImageByHoldingMouseMiddleButtonAndDragging
                    };
                }
            }

            borderPen = new Pen(Color.Black);
            borderDotPen = new Pen(Color.White) { DashPattern = new float[] { 5, 5 } };
            borderDotStaticPen = new Pen(Color.White) { DashPattern = new float[] { 5, 5 } };
            nodeBackgroundBrush = new SolidBrush(Color.White);
            infoFont = new Font("Verdana", 9);
            infoFontMedium = new Font("Verdana", 12);
            infoFontBig = new Font("Verdana", 16, FontStyle.Bold);
            textBackgroundBrush = new SolidBrush(Color.FromArgb(150, Color.FromArgb(42, 131, 199)));
            textOuterBorderPen = new Pen(Color.FromArgb(150, Color.White));
            textInnerBorderPen = new Pen(Color.FromArgb(150, Color.FromArgb(0, 81, 145)));
            markerPen = new Pen(Color.FromArgb(200, Color.Red));
            canvasBorderPen = new Pen(Color.FromArgb(30, Color.Black));

            Prepare(canvas);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScaleMode = AutoScaleMode.None;
            defaultCursor = Helpers.CreateCursor(Resources.Crosshair);
            SetDefaultCursor();
            Icon = ShareXResources.Icon;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateTitle();
            StartPosition = FormStartPosition.Manual;

            if (IsFullscreen)
            {
                FormBorderStyle = FormBorderStyle.None;
                Bounds = CaptureHelpers.GetScreenBounds();
                ShowInTaskbar = false;
#if !DEBUG
                TopMost = true;
#endif
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                MinimumSize = new Size(800, 550);

                if (Options.ImageEditorStartMode == ImageEditorStartMode.PreviousState)
                {
                    Options.ImageEditorWindowState.ApplyFormState(this);
                }
                else
                {
                    Rectangle activeScreenWorkingArea = CaptureHelpers.GetActiveScreenWorkingArea();
                    Size size = new Size(900, 700);
                    bool isMaximized = Options.ImageEditorStartMode == ImageEditorStartMode.Maximized;

                    if (Options.ImageEditorStartMode == ImageEditorStartMode.AutoSize)
                    {
                        int margin = 100;
                        Size canvasWindowSize = new Size(Canvas.Width + SystemInformation.BorderSize.Width * 2 + margin,
                            Canvas.Height + SystemInformation.CaptionHeight + SystemInformation.BorderSize.Height * 2 + margin);
                        canvasWindowSize = new Size(Math.Max(MinimumSize.Width, canvasWindowSize.Width), Math.Max(MinimumSize.Height, canvasWindowSize.Height));

                        if (canvasWindowSize.Width < activeScreenWorkingArea.Width && canvasWindowSize.Height < activeScreenWorkingArea.Height)
                        {
                            size = canvasWindowSize;
                        }
                        else
                        {
                            isMaximized = true;
                        }
                    }

                    Bounds = new Rectangle(activeScreenWorkingArea.X + (activeScreenWorkingArea.Width / 2) - (size.Width / 2),
                        activeScreenWorkingArea.Y + (activeScreenWorkingArea.Height / 2) - (size.Height / 2), size.Width, size.Height);

                    if (isMaximized)
                    {
                        WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        WindowState = FormWindowState.Normal;
                    }
                }

                ShowInTaskbar = true;
            }

            Shown += RegionCaptureForm_Shown;
            KeyDown += RegionCaptureForm_KeyDown;
            KeyUp += RegionCaptureForm_KeyUp;
            MouseDown += RegionCaptureForm_MouseDown;
            Resize += RegionCaptureForm_Resize;
            LocationChanged += RegionCaptureForm_LocationChanged;
            FormClosing += RegionCaptureForm_FormClosing;

            ResumeLayout(false);
        }

        internal void UpdateTitle()
        {
            string text;

            if (IsEditorMode)
            {
                text = "ShareX - " + Resources.RegionCaptureForm_InitializeComponent_ImageEditor;

                if (Canvas != null)
                {
                    text += $" - {Canvas.Width}x{Canvas.Height}";
                }

                string filename = Helpers.GetFilenameSafe(ImageFilePath);

                if (!string.IsNullOrEmpty(filename))
                {
                    text += " - " + filename;
                }

                if (!IsFullscreen && Options.ShowFPS)
                {
                    text += " - FPS: " + FPS.ToString();
                }
            }
            else
            {
                text = "ShareX - " + Resources.BaseRegionForm_InitializeComponent_Region_capture;
            }

            Text = text;
        }

        private void Prepare(Image canvas = null)
        {
            if (canvas == null)
            {
                canvas = new Screenshot().CaptureFullscreen();
            }

            ShapeManager = new ShapeManager(this);
            ShapeManager.WindowCaptureMode = !IsEditorMode && Options.DetectWindows;
            ShapeManager.IncludeControls = Options.DetectControls;

            InitBackground(canvas);

            if (Mode == RegionCaptureMode.OneClick || ShapeManager.WindowCaptureMode)
            {
                IntPtr handle = Handle;

                TaskEx.Run(() =>
                {
                    WindowsRectangleList wla = new WindowsRectangleList();
                    wla.IgnoreHandle = handle;
                    wla.IncludeChildWindows = ShapeManager.IncludeControls;
                    ShapeManager.Windows = wla.GetWindowInfoListAsync(5000);
                });
            }
        }

        internal void InitBackground(Image canvas)
        {
            if (Canvas != null) Canvas.Dispose();
            if (backgroundBrush != null) backgroundBrush.Dispose();
            if (backgroundHighlightBrush != null) backgroundHighlightBrush.Dispose();

            Canvas = canvas;

            if (IsEditorMode)
            {
                UpdateTitle();

                CanvasRectangle = new Rectangle(CanvasRectangle.X, CanvasRectangle.Y, Canvas.Width, Canvas.Height);

                using (Bitmap background = new Bitmap(Canvas.Width, Canvas.Height))
                using (Graphics g = Graphics.FromImage(background))
                {
                    Rectangle sourceRect = new Rectangle(0, 0, Canvas.Width, Canvas.Height);

                    using (Image checkers = ImageHelpers.DrawCheckers(Canvas.Width, Canvas.Height))
                    {
                        g.DrawImage(checkers, sourceRect);
                    }

                    g.DrawImage(Canvas, sourceRect);

                    backgroundBrush = new TextureBrush(background) { WrapMode = WrapMode.Clamp };
                    backgroundBrush.TranslateTransform(CanvasRectangle.X, CanvasRectangle.Y);
                }

                CenterCanvas();
            }
            else if (!IsEditorMode && Options.UseDimming)
            {
                using (Bitmap darkBackground = (Bitmap)Canvas.Clone())
                using (Graphics g = Graphics.FromImage(darkBackground))
                using (Brush brush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                {
                    g.FillRectangle(brush, 0, 0, darkBackground.Width, darkBackground.Height);

                    backgroundBrush = new TextureBrush(darkBackground) { WrapMode = WrapMode.Clamp };
                }

                backgroundHighlightBrush = new TextureBrush(Canvas) { WrapMode = WrapMode.Clamp };
            }
            else
            {
                backgroundBrush = new TextureBrush(Canvas) { WrapMode = WrapMode.Clamp };
            }

            if (Options.UseCustomInfoText || Mode == RegionCaptureMode.ScreenColorPicker)
            {
                if (bmpBackgroundImage != null) bmpBackgroundImage.Dispose();
                bmpBackgroundImage = new Bitmap(Canvas);
            }
        }

        private void OnMoved()
        {
            if (ShapeManager != null)
            {
                UpdateCoordinates();

                if (IsAnnotationMode && ShapeManager.ToolbarCreated)
                {
                    ShapeManager.UpdateMenuPosition();
                }
            }
        }

        private void Pan(int deltaX, int deltaY, bool usePanningStretch = true)
        {
            if (usePanningStretch)
            {
                PanningStrech.X -= deltaX;
                PanningStrech.Y -= deltaY;
            }

            Size panLimitSize = new Size(
                Math.Min((int)Math.Round(ClientArea.Width * 0.25f), CanvasRectangle.Width),
                Math.Min((int)Math.Round(ClientArea.Height * 0.25f), CanvasRectangle.Height));

            Rectangle limitRectangle = new Rectangle(
                ClientArea.X + panLimitSize.Width, ClientArea.Y + panLimitSize.Height,
                ClientArea.Width - panLimitSize.Width * 2, ClientArea.Height - panLimitSize.Height * 2);

            deltaX = Math.Max(deltaX, limitRectangle.Left - CanvasRectangle.Right);
            deltaX = Math.Min(deltaX, limitRectangle.Right - CanvasRectangle.Left);
            deltaY = Math.Max(deltaY, limitRectangle.Top - CanvasRectangle.Bottom);
            deltaY = Math.Min(deltaY, limitRectangle.Bottom - CanvasRectangle.Top);

            if (usePanningStretch)
            {
                deltaX -= Math.Min(Math.Max(deltaX, 0), Math.Max(0, PanningStrech.X));
                deltaX -= Math.Max(Math.Min(deltaX, 0), Math.Min(0, PanningStrech.X));
                deltaY -= Math.Min(Math.Max(deltaY, 0), Math.Max(0, PanningStrech.Y));
                deltaY -= Math.Max(Math.Min(deltaY, 0), Math.Min(0, PanningStrech.Y));

                PanningStrech.X += deltaX;
                PanningStrech.Y += deltaY;
            }

            CanvasRectangle = CanvasRectangle.LocationOffset(deltaX, deltaY);

            if (backgroundBrush != null)
            {
                backgroundBrush.TranslateTransform(deltaX, deltaY);
            }

            if (ShapeManager != null)
            {
                ShapeManager.MoveAll(deltaX, deltaY);
            }
        }

        private void Pan(Point delta)
        {
            Pan(delta.X, delta.Y);
        }

        private void AutomaticPan(Vector2 centerOffset)
        {
            if (IsEditorMode)
            {
                int x = (int)Math.Round(ClientArea.Width * 0.5f + centerOffset.X);
                int y = (int)Math.Round(ClientArea.Height * 0.5f + centerOffset.Y);
                int newX = x - CanvasRectangle.Width / 2;
                int newY = y - CanvasRectangle.Height / 2;
                int deltaX = newX - CanvasRectangle.X;
                int deltaY = newY - CanvasRectangle.Y;
                Pan(deltaX, deltaY, false);
            }
        }

        private void AutomaticPan()
        {
            AutomaticPan(CanvasCenterOffset);
        }

        private void UpdateCenterOffset()
        {
            CanvasCenterOffset = new Vector2(
                (CanvasRectangle.X + CanvasRectangle.Width / 2f) - ClientArea.Width / 2f,
                (CanvasRectangle.Y + CanvasRectangle.Height / 2f) - ClientArea.Height / 2f);
        }

        public void CenterCanvas()
        {
            CanvasCenterOffset = new Vector2(0f, ToolbarHeight / 2f);
            AutomaticPan();
        }

        public void SetDefaultCursor()
        {
            if (Cursor != defaultCursor)
            {
                Cursor = defaultCursor;
            }
        }

        private void RegionCaptureForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();

            OnMoved();
            CenterCanvas();

            if (IsEditorMode && Options.ShowEditorPanTip && editorPanTipAnimation != null)
            {
                editorPanTipAnimation.Start();
            }
        }

        private void RegionCaptureForm_Resize(object sender, EventArgs e)
        {
            OnMoved();
            AutomaticPan();
        }

        private void RegionCaptureForm_LocationChanged(object sender, EventArgs e)
        {
            OnMoved();
        }

        private void RegionCaptureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsEditorMode && Options.ImageEditorStartMode == ImageEditorStartMode.PreviousState)
            {
                Options.ImageEditorWindowState.UpdateFormState(this);
            }
        }

        internal void RegionCaptureForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Control | Keys.C:
                    CopyAreaInfo();
                    break;
            }
        }

        internal void RegionCaptureForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Close();
                return;
            }

            if (!isKeyAllowed && timerStart.ElapsedMilliseconds < 1000)
            {
                return;
            }

            isKeyAllowed = true;

            if (e.KeyData >= Keys.D0 && e.KeyData <= Keys.D9)
            {
                MonitorKey(e.KeyData - Keys.D0);
                return;
            }

            switch (e.KeyData)
            {
                case Keys.Space:
                    Close(RegionResult.Fullscreen);
                    break;
                case Keys.Enter:
                    Close(RegionResult.Region);
                    break;
                case Keys.Oemtilde:
                    Close(RegionResult.ActiveMonitor);
                    break;
            }
        }

        private void RegionCaptureForm_MouseDown(object sender, MouseEventArgs e)
        {
            if ((Mode == RegionCaptureMode.OneClick || Mode == RegionCaptureMode.ScreenColorPicker) && e.Button == MouseButtons.Left)
            {
                CurrentPosition = InputManager.MousePosition;

                if (Mode == RegionCaptureMode.OneClick)
                {
                    SelectedWindow = ShapeManager.FindSelectedWindow();
                }

                Close(RegionResult.Region);
            }
        }

        private void MonitorKey(int index)
        {
            if (index == 0)
            {
                index = 10;
            }

            index--;

            MonitorIndex = index;

            Close(RegionResult.Monitor);
        }

        internal void Close(RegionResult result)
        {
            Result = result;

            Close();
        }

        internal void Pause()
        {
            pause = true;
        }

        internal void Resume()
        {
            pause = false;

            Invalidate();
        }

        private void CopyAreaInfo()
        {
            string clipboardText;

            if (ShapeManager.IsCurrentShapeValid)
            {
                clipboardText = GetAreaText(ShapeManager.CurrentRectangle);
            }
            else
            {
                CurrentPosition = InputManager.MousePosition;
                clipboardText = GetInfoText();
            }

            ClipboardHelpers.CopyText(clipboardText);
        }

        public WindowInfo GetWindowInfo()
        {
            return ShapeManager.FindSelectedWindowInfo(CurrentPosition);
        }

        public void AddCursor(IntPtr cursorHandle, Point position)
        {
            if (ShapeManager != null)
            {
                ShapeManager.AddCursor(cursorHandle, position);
            }
        }

        private void UpdateCoordinates()
        {
            ClientArea = ClientRectangle;

            InputManager.Update(this);
        }

        private new void Update()
        {
            if (!timerStart.IsRunning)
            {
                timerStart.Start();
                timerFPS.Start();
            }

            UpdateCoordinates();

            UpdateDrawableObjects();

            if (ShapeManager.IsPanning)
            {
                Pan(InputManager.MouseVelocity);
                UpdateCenterOffset();
            }

            borderDotPen.DashOffset = (float)timerStart.Elapsed.TotalSeconds * -15;

            ShapeManager.Update();

            if (scrollbarManager != null)
            {
                scrollbarManager.Update();
            }
        }

        private void UpdateDrawableObjects()
        {
            DrawableObject[] objects = DrawableObjects.OrderByDescending(x => x.Order).ToArray();

            Point position = InputManager.ClientMousePosition;

            if (objects.All(x => !x.IsDragging))
            {
                for (int i = 0; i < objects.Count(); i++)
                {
                    DrawableObject obj = objects[i];

                    if (obj.Visible)
                    {
                        obj.IsCursorHover = obj.Rectangle.Contains(position);

                        if (obj.IsCursorHover)
                        {
                            if (InputManager.IsMousePressed(MouseButtons.Left))
                            {
                                obj.OnMousePressed(position);
                            }

                            for (int y = i + 1; y < objects.Count(); y++)
                            {
                                objects[y].IsCursorHover = false;
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                if (InputManager.IsMouseReleased(MouseButtons.Left))
                {
                    foreach (DrawableObject obj in objects)
                    {
                        if (obj.IsDragging)
                        {
                            obj.OnMouseReleased(position);
                        }
                    }
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Update();

            Graphics g = e.Graphics;

            if (IsEditorMode && !CanvasRectangle.Contains(ClientArea))
            {
                g.Clear(Options.ImageEditorBackgroundColor);
                g.DrawRectangleProper(canvasBorderPen, CanvasRectangle.Offset(1));
            }

            g.CompositingMode = CompositingMode.SourceCopy;
            g.FillRectangle(backgroundBrush, CanvasRectangle);
            g.CompositingMode = CompositingMode.SourceOver;

            Draw(g);

            if (Options.ShowFPS)
            {
                CheckFPS();

                if (IsFullscreen)
                {
                    DrawFPS(g, 10);
                }
            }

            if (!pause)
            {
                Invalidate();
            }
        }

        private void Draw(Graphics g)
        {
            // Draw snap rectangles
            if (ShapeManager.IsCreating && ShapeManager.IsSnapResizing)
            {
                BaseShape shape = ShapeManager.CurrentShape;

                if (shape != null && shape.ShapeType != ShapeType.RegionFreehand && shape.ShapeType != ShapeType.DrawingFreehand)
                {
                    foreach (Size size in Options.SnapSizes)
                    {
                        Rectangle snapRect = CaptureHelpers.CalculateNewRectangle(shape.StartPosition, shape.EndPosition, size);
                        g.DrawRectangleProper(markerPen, snapRect);
                    }
                }
            }

            List<BaseShape> areas = ShapeManager.ValidRegions.ToList();

            if (areas.Count > 0)
            {
                // Create graphics path from all regions
                UpdateRegionPath();

                // If background is dimmed then draw non dimmed background to region selections
                if (!IsEditorMode && Options.UseDimming)
                {
                    using (Region region = new Region(regionDrawPath))
                    {
                        g.Clip = region;
                        g.FillRectangle(backgroundHighlightBrush, ClientArea);
                        g.ResetClip();
                    }
                }

                g.DrawPath(borderPen, regionDrawPath);
                g.DrawPath(borderDotStaticPen, regionDrawPath);
            }

            // Draw effect shapes
            foreach (BaseEffectShape effectShape in ShapeManager.EffectShapes)
            {
                effectShape.OnDraw(g);
            }

            // Draw drawing shapes
            foreach (BaseDrawingShape drawingShape in ShapeManager.DrawingShapes)
            {
                drawingShape.OnDraw(g);
            }

            // Draw animated rectangle on hover area
            if (ShapeManager.IsCurrentHoverShapeValid)
            {
                if (Options.EnableAnimations)
                {
                    if (!ShapeManager.PreviousHoverRectangle.IsEmpty && ShapeManager.CurrentHoverShape.Rectangle != ShapeManager.PreviousHoverRectangle)
                    {
                        regionAnimation.FromRectangle = ShapeManager.PreviousHoverRectangle;
                        regionAnimation.ToRectangle = ShapeManager.CurrentHoverShape.Rectangle;
                        regionAnimation.Start();
                    }

                    regionAnimation.Update();
                }

                using (GraphicsPath hoverDrawPath = new GraphicsPath { FillMode = FillMode.Winding })
                {
                    if (Options.EnableAnimations && regionAnimation.IsActive && regionAnimation.CurrentRectangle.Width > 2 && regionAnimation.CurrentRectangle.Height > 2)
                    {
                        ShapeManager.CurrentHoverShape.OnShapePathRequested(hoverDrawPath, regionAnimation.CurrentRectangle.SizeOffset(-1));
                    }
                    else
                    {
                        ShapeManager.CurrentHoverShape.AddShapePath(hoverDrawPath, -1);
                    }

                    g.DrawPath(borderPen, hoverDrawPath);
                    g.DrawPath(borderDotPen, hoverDrawPath);
                }
            }

            // Draw animated rectangle on selection area
            if (ShapeManager.IsCurrentShapeTypeRegion && ShapeManager.IsCurrentShapeValid)
            {
                if (Mode == RegionCaptureMode.Ruler)
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
                    {
                        g.FillRectangle(brush, ShapeManager.CurrentRectangle);
                    }

                    DrawRuler(g, ShapeManager.CurrentRectangle, borderPen, 5, 10);
                    DrawRuler(g, ShapeManager.CurrentRectangle, borderPen, 15, 100);

                    Point centerPos = new Point(ShapeManager.CurrentRectangle.X + ShapeManager.CurrentRectangle.Width / 2, ShapeManager.CurrentRectangle.Y + ShapeManager.CurrentRectangle.Height / 2);
                    int markSize = 10;
                    g.DrawLine(borderPen, centerPos.X, centerPos.Y - markSize, centerPos.X, centerPos.Y + markSize);
                    g.DrawLine(borderPen, centerPos.X - markSize, centerPos.Y, centerPos.X + markSize, centerPos.Y);
                }

                DrawRegionArea(g, ShapeManager.CurrentRectangle, true);
            }

            // Draw all regions rectangle info
            if (Options.ShowInfo)
            {
                // Add hover area to list so rectangle info can be shown
                if (ShapeManager.IsCurrentShapeTypeRegion && ShapeManager.IsCurrentHoverShapeValid && areas.All(area => area.Rectangle != ShapeManager.CurrentHoverShape.Rectangle))
                {
                    areas.Add(ShapeManager.CurrentHoverShape);
                }

                foreach (BaseShape regionInfo in areas)
                {
                    if (regionInfo.Rectangle.IsValid())
                    {
                        string areaText = GetAreaText(regionInfo.Rectangle);
                        DrawAreaText(g, areaText, regionInfo.Rectangle);
                    }
                }
            }

            // Draw resize nodes
            DrawObjects(g);

            // Draw F1 tips
            if (!IsEditorMode && Options.ShowHotkeys)
            {
                DrawTips(g);
            }

            // Draw magnifier
            if (Options.ShowMagnifier || Options.ShowInfo)
            {
                DrawCursorGraphics(g);
            }

            // Draw screen wide crosshair
            if (Options.ShowCrosshair)
            {
                DrawCrosshair(g);
            }

            // Draw image editor bottom tip
            if (IsEditorMode && Options.ShowEditorPanTip && editorPanTipAnimation != null && editorPanTipAnimation.Update())
            {
                DrawBottomTipAnimation(g, editorPanTipAnimation);
            }

            // Draw menu tooltips
            if (IsAnnotationMode && ShapeManager.MenuTextAnimation.Update())
            {
                DrawTextAnimation(g, ShapeManager.MenuTextAnimation);
            }

            // Draw scroll bars
            if (scrollbarManager != null)
            {
                scrollbarManager.Draw(g);
            }
        }

        internal void DrawRegionArea(Graphics g, Rectangle rect, bool isAnimated)
        {
            g.DrawRectangleProper(borderPen, rect);

            if (isAnimated)
            {
                g.DrawRectangleProper(borderDotPen, rect);
            }
            else
            {
                g.DrawRectangleProper(borderDotStaticPen, rect);
            }
        }

        private void DrawObjects(Graphics g)
        {
            foreach (DrawableObject drawObject in DrawableObjects)
            {
                if (drawObject.Visible)
                {
                    drawObject.OnDraw(g);
                }
            }
        }

        private void CheckFPS()
        {
            frameCount++;

            if (timerFPS.ElapsedMilliseconds >= 1000)
            {
                FPS = (int)(frameCount / timerFPS.Elapsed.TotalSeconds);
                frameCount = 0;
                timerFPS.Reset();
                timerFPS.Start();

                if (!IsFullscreen)
                {
                    UpdateTitle();
                }
            }
        }

        private void DrawFPS(Graphics g, int offset)
        {
            Point textPosition = new Point(offset, offset);

            if (IsFullscreen)
            {
                Rectangle rectScreen = CaptureHelpers.GetActiveScreenBounds0Based();
                textPosition = textPosition.Add(rectScreen.Location);
            }

            g.DrawTextWithShadow(FPS.ToString(), textPosition, infoFontBig, Brushes.White, Brushes.Black, new Point(0, 1));
        }

        private void DrawInfoText(Graphics g, string text, Rectangle rect, Font font, int padding)
        {
            DrawInfoText(g, text, rect, font, padding, textBackgroundBrush, textOuterBorderPen, textInnerBorderPen, Brushes.White, Brushes.Black);
        }

        private void DrawInfoText(Graphics g, string text, Rectangle rect, Font font, int padding,
            Brush backgroundBrush, Pen outerBorderPen, Pen innerBorderPen, Brush textBrush, Brush textShadowBrush)
        {
            g.FillRectangle(backgroundBrush, rect.Offset(-2));
            g.DrawRectangleProper(innerBorderPen, rect.Offset(-1));
            g.DrawRectangleProper(outerBorderPen, rect);

            g.DrawTextWithShadow(text, rect.Offset(-padding).Location, font, textBrush, textShadowBrush);
        }

        private void DrawAreaText(Graphics g, string text, Rectangle area)
        {
            int offset = 6;
            int backgroundPadding = 3;
            Size textSize = g.MeasureString(text, infoFont).ToSize();
            Point textPos;

            if (area.Y - offset - textSize.Height - backgroundPadding * 2 < ClientArea.Y)
            {
                textPos = new Point(area.X + offset + backgroundPadding, area.Y + offset + backgroundPadding);
            }
            else
            {
                textPos = new Point(area.X + backgroundPadding, area.Y - offset - backgroundPadding - textSize.Height);
            }

            if (textPos.X + textSize.Width + backgroundPadding >= ClientArea.Width)
            {
                textPos.X = ClientArea.Width - textSize.Width - backgroundPadding;
            }

            Rectangle backgroundRect = new Rectangle(textPos.X - backgroundPadding, textPos.Y - backgroundPadding, textSize.Width + backgroundPadding * 2, textSize.Height + backgroundPadding * 2);

            DrawInfoText(g, text, backgroundRect, infoFont, backgroundPadding);
        }

        private void DrawTips(Graphics g)
        {
            StringBuilder sb = new StringBuilder();
            WriteTips(sb);
            string tipText = sb.ToString().Trim();

            Size textSize = g.MeasureString(tipText, infoFont).ToSize();
            int offset = 10;
            int padding = 10;
            int rectWidth = textSize.Width + padding * 2 + 2;
            int rectHeight = textSize.Height + padding * 2;
            Rectangle screenBounds = CaptureHelpers.GetActiveScreenBounds0Based();
            Rectangle textRectangle = new Rectangle(screenBounds.X + screenBounds.Width - rectWidth - offset, screenBounds.Y + offset, rectWidth, rectHeight);

            if (textRectangle.Offset(10).Contains(InputManager.ClientMousePosition))
            {
                textRectangle.Y = screenBounds.Height - rectHeight - offset;
            }

            DrawInfoText(g, tipText, textRectangle, infoFont, padding);
        }

        private void DrawTextAnimation(Graphics g, TextAnimation textAnimation)
        {
            Size textSize = g.MeasureString(textAnimation.Text, infoFontMedium).ToSize();
            int padding = 3;
            textSize.Width += padding * 2;
            textSize.Height += padding * 2;
            Rectangle textRectangle = new Rectangle(textAnimation.Position.X, textAnimation.Position.Y, textSize.Width, textSize.Height);
            DrawTextAnimation(g, textAnimation, textRectangle, padding);
        }

        private void DrawTextAnimation(Graphics g, TextAnimation textAnimation, Rectangle textRectangle, int padding)
        {
            using (Brush backgroundBrush = new SolidBrush(Color.FromArgb((int)(textAnimation.Opacity * 175), Color.FromArgb(44, 135, 206))))
            using (Pen outerBorderPen = new Pen(Color.FromArgb((int)(textAnimation.Opacity * 175), Color.White)))
            using (Pen innerBorderPen = new Pen(Color.FromArgb((int)(textAnimation.Opacity * 175), Color.FromArgb(0, 81, 145))))
            using (Brush textBrush = new SolidBrush(Color.FromArgb((int)(textAnimation.Opacity * 255), Color.White)))
            using (Brush textShadowBrush = new SolidBrush(Color.FromArgb((int)(textAnimation.Opacity * 255), Color.Black)))
            {
                DrawInfoText(g, textAnimation.Text, textRectangle, infoFontMedium, padding, backgroundBrush, outerBorderPen, innerBorderPen, textBrush, textShadowBrush);
            }
        }

        private void DrawBottomTipAnimation(Graphics g, TextAnimation textAnimation)
        {
            Size textSize = g.MeasureString(textAnimation.Text, infoFontMedium).ToSize();
            int padding = 5;
            textSize.Width += padding * 2;
            textSize.Height += padding * 2;
            int margin = 20;
            Rectangle textRectangle = new Rectangle(ClientArea.Width / 2 - textSize.Width / 2, ClientArea.Height - textSize.Height - margin, textSize.Width, textSize.Height);
            DrawTextAnimation(g, textAnimation, textRectangle, padding);
        }

        private void WriteTips(StringBuilder sb)
        {
            sb.AppendLine(Resources.RectangleRegion_WriteTips__F1__Hide_tips);
            sb.AppendLine();

            if (ShapeManager.IsCreating)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Insert__Stop_region_selection);
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Right_click__Cancel_region_selection);
            }
            else
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Left_click__Start_region_selection);
                sb.AppendLine(Resources.RegionCaptureForm_WriteTips_RightClickCancelCaptureRemoveRegion);
            }

            sb.AppendLine(Resources.RectangleRegion_WriteTips__Esc__Cancel_capture);

            if (!ShapeManager.IsCreating && !Options.QuickCrop && ShapeManager.Regions.Length > 0)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Double_Left_click___Enter__Capture_regions);
            }

            sb.AppendLine();

            if ((!Options.QuickCrop || !ShapeManager.IsCurrentShapeTypeRegion) && ShapeManager.CurrentShape != null && !ShapeManager.IsCreating)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Right_click_on_selection___Delete__Remove_region);
                sb.AppendLine(Resources.RegionCaptureForm_WriteTips_ArrowKeysResizeRegionFromBottomRightCorner);
                sb.AppendLine(Resources.RegionCaptureForm_WriteTips_HoldAltArrowKeysResizeRegionFromTopLeftCorner);
                sb.AppendLine(Resources.RectangleRegionForm_WriteTips__Hold_Ctrl___Arrow_keys__Move_region);
                sb.AppendLine(Resources.RectangleRegionForm_WriteTips__Hold_Shift___Arrow_keys__Resize_or_move_region_faster);
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Left_click_on_selection__Move_region);
            }
            else
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Arrow_keys__Move_cursor_position);
                sb.AppendLine(Resources.RectangleRegionForm_WriteTips__Hold_Shift___Arrow_keys__Move_cursor_position_faster);
            }

            if (ShapeManager.IsCreating)
            {
                sb.AppendLine(Resources.RectangleRegionForm_WriteTips__Hold_Ctrl__Move_selection);
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Shift__Proportional_resizing);

                if (ShapeManager.CurrentTool != ShapeType.RegionFreehand && ShapeManager.CurrentTool != ShapeType.DrawingFreehand)
                {
                    sb.AppendLine(Resources.RectangleRegion_WriteTips__Hold_Alt__Snap_resizing_to_preset_sizes);
                }
            }

            if (ShapeManager.Shapes.Count > 0)
            {
                sb.AppendLine(Resources.RegionCaptureForm_WriteTips_CtrlZUndoShape);
            }

            sb.AppendLine();

            if (ShapeManager.IsCurrentShapeValid)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Ctrl___C__Copy_position_and_size);
            }
            else if (Options.UseCustomInfoText)
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Ctrl___C__Copy_info);
            }
            else
            {
                sb.AppendLine(Resources.RectangleRegion_WriteTips__Ctrl___C__Copy_position);
            }

            if (IsAnnotationMode)
            {
                sb.AppendLine(Resources.RegionCaptureForm_WriteTips_CtrlVPasteImageOrText);
            }

            sb.AppendLine(Resources.RectangleRegionForm_WriteTips__Mouse_wheel__Change_current_tool);

            sb.AppendLine();

            sb.AppendLine(Resources.RectangleRegion_WriteTips__Space__Fullscreen_capture);
            sb.AppendLine(Resources.RectangleRegion_WriteTips__1__2__3_____0__Monitor_capture);
            sb.AppendLine(Resources.RectangleRegion_WriteTips_____Active_monitor_capture);

            if (Mode == RegionCaptureMode.Annotation && !ShapeManager.IsCreating)
            {
                sb.AppendLine();

                if (ShapeManager.IsCurrentShapeTypeRegion)
                {
                    sb.AppendLine(Resources.RectangleRegionForm_WriteTips__Tab___Mouse_4_click__Select_last_annotation_tool);
                }
                else
                {
                    sb.AppendLine(Resources.RectangleRegionForm_WriteTips__Tab___Mouse_4_click__Select_last_region_tool);
                }

                sb.AppendLine(Resources.RectangleRegionForm_WriteTips__Ctrl___Mouse_wheel__Change_magnifier_size);
                if (ShapeManager.CurrentTool == ShapeType.RegionRectangle) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 0", ShapeType.RegionRectangle.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.RegionEllipse) sb.Append("-> ");
                sb.AppendLine(ShapeType.RegionEllipse.GetLocalizedDescription());
                if (ShapeManager.CurrentTool == ShapeType.RegionFreehand) sb.Append("-> ");
                sb.AppendLine(ShapeType.RegionFreehand.GetLocalizedDescription());
                if (ShapeManager.CurrentTool == ShapeType.DrawingRectangle) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 1", ShapeType.DrawingRectangle.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.DrawingEllipse) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 2", ShapeType.DrawingEllipse.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.DrawingFreehand) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 3", ShapeType.DrawingFreehand.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.DrawingLine) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 4", ShapeType.DrawingLine.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.DrawingArrow) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 5", ShapeType.DrawingArrow.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.DrawingTextOutline) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 6", ShapeType.DrawingTextOutline.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.DrawingSpeechBalloon) sb.Append("-> ");
                sb.AppendLine(ShapeType.DrawingSpeechBalloon.GetLocalizedDescription());
                if (ShapeManager.CurrentTool == ShapeType.DrawingStep) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 7", ShapeType.DrawingStep.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.DrawingImage) sb.Append("-> ");
                sb.AppendLine(ShapeType.DrawingImage.GetLocalizedDescription());
                if (ShapeManager.CurrentTool == ShapeType.EffectBlur) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 8", ShapeType.EffectBlur.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.EffectPixelate) sb.Append("-> ");
                sb.AppendLine(string.Format("[{0}] {1}", "Numpad 9", ShapeType.EffectPixelate.GetLocalizedDescription()));
                if (ShapeManager.CurrentTool == ShapeType.EffectHighlight) sb.Append("-> ");
                sb.AppendLine(ShapeType.EffectHighlight.GetLocalizedDescription());
            }

            sb.AppendLine();
            sb.AppendLine(Resources.RegionCaptureForm_WriteTips_NoteHidingTheseTipsWillIncreaseFPSGreatly);
        }

        private string GetAreaText(Rectangle area)
        {
            if (Mode == RegionCaptureMode.Ruler)
            {
                Point endPos = new Point(area.Right - 1, area.Bottom - 1);
                return string.Format(Resources.RectangleRegion_GetRulerText_Ruler_info, area.X, area.Y, endPos.X, endPos.Y,
                    area.Width, area.Height, MathHelpers.Distance(area.Location, endPos), MathHelpers.LookAtDegree(area.Location, endPos));
            }

            return string.Format(Resources.RectangleRegion_GetAreaText_Area, area.X, area.Y, area.Width, area.Height);
        }

        private string GetInfoText()
        {
            if (IsEditorMode)
            {
                Point canvasRelativePosition = new Point(InputManager.ClientMousePosition.X - CanvasRectangle.X, InputManager.ClientMousePosition.Y - CanvasRectangle.Y);
                return $"X: {canvasRelativePosition.X} Y: {canvasRelativePosition.Y}";
            }
            else if (Mode == RegionCaptureMode.ScreenColorPicker || Options.UseCustomInfoText)
            {
                Color color = CurrentColor;

                if (Mode != RegionCaptureMode.ScreenColorPicker && !string.IsNullOrEmpty(Options.CustomInfoText))
                {
                    return CodeMenuEntryPixelInfo.Parse(Options.CustomInfoText, color, CurrentPosition);
                }

                return string.Format(Resources.RectangleRegion_GetColorPickerText, color.R, color.G, color.B, ColorHelpers.ColorToHex(color), CurrentPosition.X, CurrentPosition.Y);
            }

            return $"X: {CurrentPosition.X} Y: {CurrentPosition.Y}";
        }

        private void DrawCrosshair(Graphics g)
        {
            int offset = 5;
            Point mousePos = InputManager.ClientMousePosition;
            Point left = new Point(mousePos.X - offset, mousePos.Y), left2 = new Point(0, mousePos.Y);
            Point right = new Point(mousePos.X + offset, mousePos.Y), right2 = new Point(ClientArea.Width - 1, mousePos.Y);
            Point top = new Point(mousePos.X, mousePos.Y - offset), top2 = new Point(mousePos.X, 0);
            Point bottom = new Point(mousePos.X, mousePos.Y + offset), bottom2 = new Point(mousePos.X, ClientArea.Height - 1);

            if (left.X - left2.X > 10)
            {
                g.DrawLine(borderPen, left, left2);
                g.DrawLine(borderDotPen, left, left2);
            }

            if (right2.X - right.X > 10)
            {
                g.DrawLine(borderPen, right, right2);
                g.DrawLine(borderDotPen, right, right2);
            }

            if (top.Y - top2.Y > 10)
            {
                g.DrawLine(borderPen, top, top2);
                g.DrawLine(borderDotPen, top, top2);
            }

            if (bottom2.Y - bottom.Y > 10)
            {
                g.DrawLine(borderPen, bottom, bottom2);
                g.DrawLine(borderDotPen, bottom, bottom2);
            }
        }

        private void DrawCursorGraphics(Graphics g)
        {
            Point mousePos = InputManager.ClientMousePosition;
            Rectangle currentScreenRect0Based = CaptureHelpers.GetActiveScreenBounds0Based();
            int cursorOffsetX = 10, cursorOffsetY = 10, itemGap = 10, itemCount = 0;
            Size totalSize = Size.Empty;

            int magnifierPosition = 0;
            Bitmap magnifier = null;

            if (Options.ShowMagnifier)
            {
                if (itemCount > 0) totalSize.Height += itemGap;
                magnifierPosition = totalSize.Height;

                magnifier = Magnifier(Canvas, mousePos, Options.MagnifierPixelCount, Options.MagnifierPixelCount, Options.MagnifierPixelSize);
                totalSize.Width = Math.Max(totalSize.Width, magnifier.Width);

                totalSize.Height += magnifier.Height;
                itemCount++;
            }

            int infoTextPadding = 3;
            int infoTextPosition = 0;
            Rectangle infoTextRect = Rectangle.Empty;
            string infoText = "";

            if (Options.ShowInfo)
            {
                if (itemCount > 0) totalSize.Height += itemGap;
                infoTextPosition = totalSize.Height;

                CurrentPosition = InputManager.MousePosition;
                infoText = GetInfoText();
                Size textSize = g.MeasureString(infoText, infoFont).ToSize();
                infoTextRect.Size = new Size(textSize.Width + infoTextPadding * 2, textSize.Height + infoTextPadding * 2);
                totalSize.Width = Math.Max(totalSize.Width, infoTextRect.Width);

                totalSize.Height += infoTextRect.Height;
                itemCount++;
            }

            int x = mousePos.X + cursorOffsetX;

            if (x + totalSize.Width > currentScreenRect0Based.Right)
            {
                x = mousePos.X - cursorOffsetX - totalSize.Width;
            }

            int y = mousePos.Y + cursorOffsetY;

            if (y + totalSize.Height > currentScreenRect0Based.Bottom)
            {
                y = mousePos.Y - cursorOffsetY - totalSize.Height;
            }

            if (Options.ShowMagnifier)
            {
                using (GraphicsQualityManager quality = new GraphicsQualityManager(g))
                using (TextureBrush brush = new TextureBrush(magnifier))
                {
                    brush.TranslateTransform(x, y + magnifierPosition);

                    if (Options.UseSquareMagnifier)
                    {
                        g.FillRectangle(brush, x, y + magnifierPosition, magnifier.Width, magnifier.Height);
                        g.DrawRectangleProper(Pens.White, x - 1, y + magnifierPosition - 1, magnifier.Width + 2, magnifier.Height + 2);
                        g.DrawRectangleProper(Pens.Black, x, y + magnifierPosition, magnifier.Width, magnifier.Height);
                    }
                    else
                    {
                        g.FillEllipse(brush, x, y + magnifierPosition, magnifier.Width, magnifier.Height);
                        g.DrawEllipse(Pens.White, x - 1, y + magnifierPosition - 1, magnifier.Width + 2 - 1, magnifier.Height + 2 - 1);
                        g.DrawEllipse(Pens.Black, x, y + magnifierPosition, magnifier.Width - 1, magnifier.Height - 1);
                    }
                }
            }

            if (Options.ShowInfo)
            {
                infoTextRect.Location = new Point(x + (totalSize.Width / 2) - (infoTextRect.Width / 2), y + infoTextPosition);
                DrawInfoText(g, infoText, infoTextRect, infoFont, infoTextPadding);
            }
        }

        private Bitmap Magnifier(Image img, Point position, int horizontalPixelCount, int verticalPixelCount, int pixelSize)
        {
            horizontalPixelCount = (horizontalPixelCount | 1).Between(1, 101);
            verticalPixelCount = (verticalPixelCount | 1).Between(1, 101);
            pixelSize = pixelSize.Between(1, 1000);

            if (horizontalPixelCount * pixelSize > ClientArea.Width || verticalPixelCount * pixelSize > ClientArea.Height)
            {
                horizontalPixelCount = verticalPixelCount = 15;
                pixelSize = 10;
            }

            int width = horizontalPixelCount * pixelSize;
            int height = verticalPixelCount * pixelSize;
            Bitmap bmp = new Bitmap(width - 1, height - 1);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;

                g.DrawImage(img, new Rectangle(0, 0, width, height), new Rectangle(position.X - horizontalPixelCount / 2 - CanvasRectangle.X,
                    position.Y - verticalPixelCount / 2 - CanvasRectangle.Y, horizontalPixelCount, verticalPixelCount), GraphicsUnit.Pixel);

                g.PixelOffsetMode = PixelOffsetMode.None;

                using (SolidBrush crosshairBrush = new SolidBrush(Color.FromArgb(125, Color.LightBlue)))
                {
                    g.FillRectangle(crosshairBrush, new Rectangle(0, (height - pixelSize) / 2, (width - pixelSize) / 2, pixelSize)); // Left
                    g.FillRectangle(crosshairBrush, new Rectangle((width + pixelSize) / 2, (height - pixelSize) / 2, (width - pixelSize) / 2, pixelSize)); // Right
                    g.FillRectangle(crosshairBrush, new Rectangle((width - pixelSize) / 2, 0, pixelSize, (height - pixelSize) / 2)); // Top
                    g.FillRectangle(crosshairBrush, new Rectangle((width - pixelSize) / 2, (height + pixelSize) / 2, pixelSize, (height - pixelSize) / 2)); // Bottom
                }

                using (Pen pen = new Pen(Color.FromArgb(75, Color.Black)))
                {
                    for (int x = 1; x < horizontalPixelCount; x++)
                    {
                        g.DrawLine(pen, new Point(x * pixelSize - 1, 0), new Point(x * pixelSize - 1, height - 1));
                    }

                    for (int y = 1; y < verticalPixelCount; y++)
                    {
                        g.DrawLine(pen, new Point(0, y * pixelSize - 1), new Point(width - 1, y * pixelSize - 1));
                    }
                }

                g.DrawRectangle(Pens.Black, (width - pixelSize) / 2 - 1, (height - pixelSize) / 2 - 1, pixelSize, pixelSize);

                if (pixelSize >= 6)
                {
                    g.DrawRectangle(Pens.White, (width - pixelSize) / 2, (height - pixelSize) / 2, pixelSize - 2, pixelSize - 2);
                }
            }

            return bmp;
        }

        private void DrawRuler(Graphics g, Rectangle rect, Pen pen, int rulerSize, int rulerWidth)
        {
            if (rect.Width >= rulerSize && rect.Height >= rulerSize)
            {
                for (int x = 1; x <= rect.Width / rulerWidth; x++)
                {
                    g.DrawLine(pen, new Point(rect.X + x * rulerWidth, rect.Y), new Point(rect.X + x * rulerWidth, rect.Y + rulerSize));
                    g.DrawLine(pen, new Point(rect.X + x * rulerWidth, rect.Bottom), new Point(rect.X + x * rulerWidth, rect.Bottom - rulerSize));
                }

                for (int y = 1; y <= rect.Height / rulerWidth; y++)
                {
                    g.DrawLine(pen, new Point(rect.X, rect.Y + y * rulerWidth), new Point(rect.X + rulerSize, rect.Y + y * rulerWidth));
                    g.DrawLine(pen, new Point(rect.Right, rect.Y + y * rulerWidth), new Point(rect.Right - rulerSize, rect.Y + y * rulerWidth));
                }
            }
        }

        internal void UpdateRegionPath()
        {
            if (regionFillPath != null)
            {
                regionFillPath.Dispose();
                regionFillPath = null;
            }

            if (regionDrawPath != null)
            {
                regionDrawPath.Dispose();
                regionDrawPath = null;
            }

            BaseShape[] areas = ShapeManager.ValidRegions;

            if (areas != null && areas.Length > 0)
            {
                regionFillPath = new GraphicsPath { FillMode = FillMode.Winding };
                regionDrawPath = new GraphicsPath { FillMode = FillMode.Winding };

                foreach (BaseShape regionShape in ShapeManager.ValidRegions)
                {
                    regionShape.AddShapePath(regionFillPath);
                    regionShape.AddShapePath(regionDrawPath, -1);
                }
            }
        }

        public Image GetResultImage()
        {
            if (IsEditorMode)
            {
                foreach (BaseShape shape in ShapeManager.Shapes)
                {
                    shape.Move(-CanvasRectangle.X, -CanvasRectangle.Y);
                }

                Image img = GetOutputImage();

                foreach (BaseShape shape in ShapeManager.Shapes)
                {
                    shape.Move(CanvasRectangle.X, CanvasRectangle.Y);
                }

                return img;
            }
            else if (Result == RegionResult.Region || Result == RegionResult.LastRegion)
            {
                GraphicsPath gp;

                if (Result == RegionResult.LastRegion)
                {
                    gp = LastRegionFillPath;
                }
                else
                {
                    gp = regionFillPath;
                }

                using (Image img = GetOutputImage())
                {
                    return RegionCaptureTasks.ApplyRegionPathToImage(img, gp);
                }
            }
            else if (Result == RegionResult.Fullscreen)
            {
                return GetOutputImage();
            }
            else if (Result == RegionResult.Monitor)
            {
                Screen[] screens = Screen.AllScreens;

                if (MonitorIndex < screens.Length)
                {
                    Screen screen = screens[MonitorIndex];
                    Rectangle screenRect = CaptureHelpers.ScreenToClient(screen.Bounds);

                    using (Image img = GetOutputImage())
                    {
                        return ImageHelpers.CropImage(img, screenRect);
                    }
                }
            }
            else if (Result == RegionResult.ActiveMonitor)
            {
                Rectangle activeScreenRect = CaptureHelpers.GetActiveScreenBounds0Based();

                using (Image img = GetOutputImage())
                {
                    return ImageHelpers.CropImage(img, activeScreenRect);
                }
            }

            return null;
        }

        private Image GetOutputImage()
        {
            return ShapeManager.RenderOutputImage(Canvas);
        }

        protected override void Dispose(bool disposing)
        {
            IsClosing = true;

            ShapeManager?.Dispose();
            bmpBackgroundImage?.Dispose();
            backgroundBrush?.Dispose();
            backgroundHighlightBrush?.Dispose();
            borderPen?.Dispose();
            borderDotPen?.Dispose();
            borderDotStaticPen?.Dispose();
            nodeBackgroundBrush?.Dispose();
            infoFont?.Dispose();
            infoFontMedium?.Dispose();
            infoFontBig?.Dispose();
            textBackgroundBrush?.Dispose();
            textOuterBorderPen?.Dispose();
            textInnerBorderPen?.Dispose();
            markerPen?.Dispose();
            canvasBorderPen?.Dispose();
            defaultCursor?.Dispose();
            CustomNodeImage?.Dispose();

            if (regionFillPath != null)
            {
                if (Result == RegionResult.Region)
                {
                    LastRegionFillPath?.Dispose();
                    LastRegionFillPath = regionFillPath;
                }
                else
                {
                    regionFillPath.Dispose();
                }
            }

            regionDrawPath?.Dispose();
            Canvas?.Dispose();

            base.Dispose(disposing);
        }
    }
}