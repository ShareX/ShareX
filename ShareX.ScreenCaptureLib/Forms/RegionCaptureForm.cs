#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public sealed class RegionCaptureForm : Form
    {
        public static GraphicsPath LastRegionFillPath { get; private set; }

        public event Func<Bitmap, string, string> SaveImageRequested;
        public event Func<Bitmap, string, string> SaveImageAsRequested;
        public event Action<Bitmap> CopyImageRequested;
        public event Action<Bitmap> UploadImageRequested;
        public event Action<Bitmap> PrintImageRequested;

        public RegionCaptureOptions Options { get; set; }
        public Rectangle ClientArea { get; private set; }
        public Bitmap Canvas { get; private set; }
        public Rectangle CanvasRectangle { get; internal set; }
        public RegionResult Result { get; private set; }
        public int FPS { get; private set; }
        public int MonitorIndex { get; set; }
        public string ImageFilePath { get; set; }
        public bool IsFullscreen { get; private set; }

        public RegionCaptureMode Mode { get; private set; }
        public bool IsEditorMode => Mode == RegionCaptureMode.Editor || Mode == RegionCaptureMode.TaskEditor;
        public bool IsAnnotationMode => Mode == RegionCaptureMode.Annotation || IsEditorMode;
        public bool IsModified => ShapeManager != null && ShapeManager.IsModified;

        public Point CurrentPosition { get; private set; }
        public Point PanningStrech = new Point();

        public SimpleWindowInfo SelectedWindow { get; private set; }

        public Vector2 CanvasCenterOffset { get; set; } = new Vector2(0f, 0f);

        internal ShapeManager ShapeManager { get; private set; }
        internal bool IsClosing { get; private set; }

        internal Bitmap DimmedCanvas;
        internal Image CustomNodeImage = Resources.CircleNode;
        internal int ToolbarHeight;

        private InputManager InputManager => ShapeManager.InputManager;
        private TextureBrush backgroundBrush, backgroundHighlightBrush;
        private GraphicsPath regionFillPath, regionDrawPath;
        private Pen borderPen, borderDotPen, borderDotStaticPen, textOuterBorderPen, textInnerBorderPen, markerPen, canvasBorderPen;
        private Brush textBrush, textShadowBrush, textBackgroundBrush;
        private Font infoFont, infoFontMedium, infoFontBig;
        private Stopwatch timerStart, timerFPS;
        private int frameCount;
        private bool pause, isKeyAllowed, forceClose;
        private RectangleAnimation regionAnimation;
        private TextAnimation editorPanTipAnimation;
        private Cursor defaultCursor, openHandCursor, closedHandCursor;
        private Color canvasBackgroundColor, canvasBorderColor, textColor, textShadowColor, textBackgroundColor, textOuterBorderColor, textInnerBorderColor;

        public RegionCaptureForm(RegionCaptureMode mode, RegionCaptureOptions options, Bitmap canvas = null)
        {
            Mode = mode;
            Options = options;

            if (canvas == null)
            {
                canvas = new Screenshot().CaptureFullscreen();
            }

            IsFullscreen = !IsEditorMode || Options.ImageEditorStartMode == ImageEditorStartMode.Fullscreen;

            ClientArea = CaptureHelpers.GetScreenBounds0Based();
            CanvasRectangle = ClientArea;

            timerStart = new Stopwatch();
            timerFPS = new Stopwatch();
            regionAnimation = new RectangleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(200)
            };

            if (IsEditorMode && Options.ShowEditorPanTip)
            {
                editorPanTipAnimation = new TextAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(5000),
                    FadeOutDuration = TimeSpan.FromMilliseconds(1000),
                    Text = Resources.RegionCaptureForm_TipYouCanPanImageByHoldingMouseMiddleButtonAndDragging
                };
            }

            borderPen = new Pen(Color.Black);
            borderDotPen = new Pen(Color.White) { DashPattern = new float[] { 5, 5 } };
            borderDotStaticPen = new Pen(Color.White) { DashPattern = new float[] { 5, 5 } };
            infoFont = new Font("Verdana", 9);
            infoFontMedium = new Font("Verdana", 12);
            infoFontBig = new Font("Verdana", 16, FontStyle.Bold);
            markerPen = new Pen(Color.FromArgb(200, Color.Red));

            if (ShareXResources.UseCustomTheme)
            {
                canvasBackgroundColor = ShareXResources.Theme.BackgroundColor;
                canvasBorderColor = ShareXResources.Theme.BorderColor;
                textColor = ShareXResources.Theme.TextColor;
                textShadowColor = ShareXResources.Theme.BorderColor;
                textBackgroundColor = Color.FromArgb(200, ShareXResources.Theme.BackgroundColor);
                textOuterBorderColor = Color.FromArgb(200, ShareXResources.Theme.SeparatorDarkColor);
                textInnerBorderColor = Color.FromArgb(200, ShareXResources.Theme.SeparatorLightColor);
            }
            else
            {
                canvasBackgroundColor = Color.FromArgb(200, 200, 200);
                canvasBorderColor = Color.FromArgb(176, 176, 176);
                textColor = Color.White;
                textShadowColor = Color.Black;
                textBackgroundColor = Color.FromArgb(200, Color.FromArgb(42, 131, 199));
                textOuterBorderColor = Color.FromArgb(200, Color.White);
                textInnerBorderColor = Color.FromArgb(200, Color.FromArgb(0, 81, 145));
            }

            canvasBorderPen = new Pen(canvasBorderColor);
            textBrush = new SolidBrush(textColor);
            textShadowBrush = new SolidBrush(textShadowColor);
            textBackgroundBrush = new SolidBrush(textBackgroundColor);
            textOuterBorderPen = new Pen(textOuterBorderColor);
            textInnerBorderPen = new Pen(textInnerBorderColor);

            Prepare(canvas);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScaleMode = AutoScaleMode.None;
            defaultCursor = Helpers.CreateCursor(Resources.Crosshair);
            openHandCursor = Helpers.CreateCursor(Resources.openhand);
            closedHandCursor = Helpers.CreateCursor(Resources.closedhand);
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
                        Size canvasWindowSize = new Size(Canvas.Width + (SystemInformation.BorderSize.Width * 2) + margin,
                            Canvas.Height + SystemInformation.CaptionHeight + (SystemInformation.BorderSize.Height * 2) + margin);
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
            MouseDown += RegionCaptureForm_MouseDown;
            Resize += RegionCaptureForm_Resize;
            LocationChanged += RegionCaptureForm_LocationChanged;
            LostFocus += RegionCaptureForm_LostFocus;
            GotFocus += RegionCaptureForm_GotFocus;
            FormClosing += RegionCaptureForm_FormClosing;

            ResumeLayout(false);
        }

        internal void UpdateTitle()
        {
            if (forceClose) return;

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

        private void Prepare(Bitmap canvas = null)
        {
            ShapeManager = new ShapeManager(this);
            ShapeManager.WindowCaptureMode = !IsEditorMode && Options.DetectWindows;
            ShapeManager.IncludeControls = Options.DetectControls;

            InitBackground(canvas);

            if (Mode == RegionCaptureMode.OneClick || ShapeManager.WindowCaptureMode)
            {
                IntPtr handle = Handle;

                Task.Run(() =>
                {
                    WindowsRectangleList wla = new WindowsRectangleList();
                    wla.IgnoreHandle = handle;
                    wla.IncludeChildWindows = ShapeManager.IncludeControls;
                    ShapeManager.Windows = wla.GetWindowInfoListAsync(5000);
                });
            }
        }

        internal void InitBackground(Bitmap canvas, bool centerCanvas = true)
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

                    if (ShareXResources.Theme.CheckerSize > 0)
                    {
                        using (Bitmap checkers = ImageHelpers.DrawCheckers(Canvas.Width, Canvas.Height, ShareXResources.Theme.CheckerSize,
                            ShareXResources.Theme.CheckerColor, ShareXResources.Theme.CheckerColor2))
                        {
                            g.DrawImage(checkers, sourceRect);
                        }
                    }
                    else
                    {
                        using (Brush canvasBrush = new SolidBrush(ShareXResources.Theme.CheckerColor))
                        {
                            g.FillRectangle(canvasBrush, sourceRect);
                        }
                    }

                    g.DrawImage(Canvas, sourceRect);

                    backgroundBrush = new TextureBrush(background) { WrapMode = WrapMode.Clamp };
                    backgroundBrush.TranslateTransform(CanvasRectangle.X, CanvasRectangle.Y);
                }

                if (centerCanvas)
                {
                    CenterCanvas();
                }
            }
            else if (Options.UseDimming)
            {
                DimmedCanvas?.Dispose();
                DimmedCanvas = (Bitmap)Canvas.Clone();

                using (Graphics g = Graphics.FromImage(DimmedCanvas))
                using (Brush brush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                {
                    g.FillRectangle(brush, 0, 0, DimmedCanvas.Width, DimmedCanvas.Height);

                    backgroundBrush = new TextureBrush(DimmedCanvas) { WrapMode = WrapMode.Clamp };
                }

                backgroundHighlightBrush = new TextureBrush(Canvas) { WrapMode = WrapMode.Clamp };
            }
            else
            {
                backgroundBrush = new TextureBrush(Canvas) { WrapMode = WrapMode.Clamp };
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
                ClientArea.Width - (panLimitSize.Width * 2), ClientArea.Height - (panLimitSize.Height * 2));

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
                int x = (int)Math.Round((ClientArea.Width * 0.5f) + centerOffset.X);
                int y = (int)Math.Round((ClientArea.Height * 0.5f) + centerOffset.Y);
                int newX = x - (CanvasRectangle.Width / 2);
                int newY = y - (CanvasRectangle.Height / 2);
                int deltaX = newX - CanvasRectangle.X;
                int deltaY = newY - CanvasRectangle.Y;
                Pan(deltaX, deltaY, false);
            }
        }

        public void AutomaticPan()
        {
            AutomaticPan(CanvasCenterOffset);
        }

        private void UpdateCenterOffset()
        {
            CanvasCenterOffset = new Vector2(
                (CanvasRectangle.X + (CanvasRectangle.Width / 2f)) - (ClientArea.Width / 2f),
                (CanvasRectangle.Y + (CanvasRectangle.Height / 2f)) - (ClientArea.Height / 2f));
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

        public void SetHandCursor(bool grabbing)
        {
            if (grabbing)
            {
                if (Cursor != closedHandCursor)
                {
                    Cursor = closedHandCursor;
                }
            }
            else
            {
                if (Cursor != openHandCursor)
                {
                    Cursor = openHandCursor;
                }
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

        private void RegionCaptureForm_GotFocus(object sender, EventArgs e)
        {
            Resume();
        }

        private void RegionCaptureForm_LostFocus(object sender, EventArgs e)
        {
            Pause();
        }

        private void RegionCaptureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsEditorMode)
            {
                if (e.CloseReason == CloseReason.UserClosing && !forceClose && !IsFullscreen && !ShowExitConfirmation())
                {
                    e.Cancel = true;
                    return;
                }

                if (Options.ImageEditorStartMode == ImageEditorStartMode.PreviousState)
                {
                    Options.ImageEditorWindowState.UpdateFormState(this);
                }
            }
        }

        internal bool ShowExitConfirmation()
        {
            bool result = true;

            if (IsModified)
            {
                Pause();
                result = MessageBox.Show(this, Resources.RegionCaptureForm_ShowExitConfirmation_Text, Resources.RegionCaptureForm_ShowExitConfirmation_ShareXImageEditor,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                Resume();
            }

            return result;
        }

        internal void RegionCaptureForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if (ShapeManager.HandleEscape())
                {
                    return;
                }

                if (!IsEditorMode || ShowExitConfirmation())
                {
                    CloseWindow();
                }

                return;
            }

            if (!isKeyAllowed && timerStart.ElapsedMilliseconds < 1000)
            {
                return;
            }

            isKeyAllowed = true;

            switch (e.KeyData)
            {
                case Keys.Space:
                    CloseWindow(RegionResult.Fullscreen);
                    break;
                case Keys.Enter:
                    if (ShapeManager.IsCurrentShapeTypeRegion)
                    {
                        ShapeManager.StartRegionSelection();
                        ShapeManager.EndRegionSelection();
                    }

                    CloseWindow(RegionResult.Region);
                    break;
                case Keys.Oemtilde:
                    CloseWindow(RegionResult.ActiveMonitor);
                    break;
                case Keys.Control | Keys.C:
                    CopyAreaInfo();
                    break;
            }

            if (!IsEditorMode && e.KeyData >= Keys.D0 && e.KeyData <= Keys.D9)
            {
                MonitorKey(e.KeyData - Keys.D0);
                return;
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

                CloseWindow(RegionResult.Region);
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

            CloseWindow(RegionResult.Monitor);
        }

        internal void CloseWindow(RegionResult result = RegionResult.Close)
        {
            Result = result;
            forceClose = true;
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

            ShapeManager.UpdateObjects();

            if (ShapeManager.IsPanning)
            {
                Pan(InputManager.MouseVelocity);
                UpdateCenterOffset();
            }

            if (Options.EnableAnimations)
            {
                borderDotPen.DashOffset = (float)timerStart.Elapsed.TotalSeconds * -15;
            }

            ShapeManager.Update();
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
                g.Clear(canvasBackgroundColor);
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

            // Draw tools
            foreach (BaseTool toolShape in ShapeManager.ToolShapes)
            {
                toolShape.OnDraw(g);
            }

            // Draw animated rectangle on hover area
            if (ShapeManager.IsCurrentHoverShapeValid)
            {
                if (Options.EnableAnimations)
                {
                    if (!ShapeManager.PreviousHoverRectangle.IsEmpty && ShapeManager.CurrentHoverShape.Rectangle != ShapeManager.PreviousHoverRectangle)
                    {
                        if (regionAnimation.CurrentRectangle.Width > 2 && regionAnimation.CurrentRectangle.Height > 2)
                        {
                            regionAnimation.FromRectangle = regionAnimation.CurrentRectangle;
                        }
                        else
                        {
                            regionAnimation.FromRectangle = ShapeManager.PreviousHoverRectangle;
                        }

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
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, 255, 255, 255)))
                    {
                        g.FillRectangle(brush, ShapeManager.CurrentRectangle);
                    }

                    DrawRuler(g, ShapeManager.CurrentRectangle, borderPen, 5, 10);
                    DrawRuler(g, ShapeManager.CurrentRectangle, borderPen, 15, 100);

                    g.DrawCross(borderPen, ShapeManager.CurrentRectangle.Center(), 10);
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
            ShapeManager.DrawObjects(g);

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
            DrawInfoText(g, text, rect, font, new Point(padding, padding));
        }

        private void DrawInfoText(Graphics g, string text, Rectangle rect, Font font, Point padding)
        {
            DrawInfoText(g, text, rect, font, padding, textBackgroundBrush, textOuterBorderPen, textInnerBorderPen, textBrush, textShadowBrush);
        }

        private void DrawInfoText(Graphics g, string text, Rectangle rect, Font font, int padding,
            Brush backgroundBrush, Pen outerBorderPen, Pen innerBorderPen, Brush textBrush, Brush textShadowBrush)
        {
            DrawInfoText(g, text, rect, font, new Point(padding, padding), backgroundBrush, outerBorderPen, innerBorderPen, textBrush, textShadowBrush);
        }

        private void DrawInfoText(Graphics g, string text, Rectangle rect, Font font, Point padding,
            Brush backgroundBrush, Pen outerBorderPen, Pen innerBorderPen, Brush textBrush, Brush textShadowBrush)
        {
            g.FillRectangle(backgroundBrush, rect.Offset(-2));
            g.DrawRectangleProper(innerBorderPen, rect.Offset(-1));
            g.DrawRectangleProper(outerBorderPen, rect);

            g.DrawTextWithShadow(text, rect.LocationOffset(padding.X, padding.Y).Location, font, textBrush, textShadowBrush);
        }

        internal void DrawAreaText(Graphics g, string text, Rectangle area)
        {
            int offset = 6;
            int backgroundPadding = 3;
            Size textSize = g.MeasureString(text, infoFont).ToSize();
            Point textPos;

            if (area.Y - offset - textSize.Height - (backgroundPadding * 2) < ClientArea.Y)
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

            Rectangle backgroundRect = new Rectangle(textPos.X - backgroundPadding, textPos.Y - backgroundPadding, textSize.Width + (backgroundPadding * 2), textSize.Height + (backgroundPadding * 2));

            DrawInfoText(g, text, backgroundRect, infoFont, backgroundPadding);
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
            using (Brush backgroundBrush = new SolidBrush(Color.FromArgb((int)(textAnimation.Opacity * 200), textBackgroundColor)))
            using (Pen outerBorderPen = new Pen(Color.FromArgb((int)(textAnimation.Opacity * 200), textOuterBorderColor)))
            using (Pen innerBorderPen = new Pen(Color.FromArgb((int)(textAnimation.Opacity * 200), textInnerBorderColor)))
            using (Brush textBrush = new SolidBrush(Color.FromArgb((int)(textAnimation.Opacity * 255), textColor)))
            using (Brush textShadowBrush = new SolidBrush(Color.FromArgb((int)(textAnimation.Opacity * 255), textShadowColor)))
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
            Rectangle textRectangle = new Rectangle((ClientArea.Width / 2) - (textSize.Width / 2), ClientArea.Height - textSize.Height - margin, textSize.Width, textSize.Height);
            DrawTextAnimation(g, textAnimation, textRectangle, padding);
        }

        internal string GetAreaText(Rectangle rect)
        {
            if (IsEditorMode)
            {
                rect = new Rectangle(rect.X - CanvasRectangle.X, rect.Y - CanvasRectangle.Y, rect.Width, rect.Height);
            }
            else if (Mode == RegionCaptureMode.Ruler)
            {
                Point endLocation = new Point(rect.Right - 1, rect.Bottom - 1);
                string text = $"X: {rect.X} | Y: {rect.Y} | Right: {endLocation.X} | Bottom: {endLocation.Y}\r\n" +
                    $"Width: {rect.Width} px | Height: {rect.Height} px | Area: {rect.Area()} px | Perimeter: {rect.Perimeter()} px\r\n" +
                    $"Distance: {MathHelpers.Distance(rect.Location, endLocation):0.00} px | Angle: {MathHelpers.LookAtDegree(rect.Location, endLocation):0.00}°";
                return text;
            }

            return string.Format(Resources.RectangleRegion_GetAreaText_Area, rect.X, rect.Y, rect.Width, rect.Height);
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
                Color color = ShapeManager.GetCurrentColor();

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
                infoTextRect.Size = new Size(textSize.Width + (infoTextPadding * 2), textSize.Height + (infoTextPadding * 2));
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
                if (Mode == RegionCaptureMode.ScreenColorPicker)
                {
                    int colorBoxOffset = 2;
                    int colorBoxSize = infoTextRect.Height - (colorBoxOffset * 2);
                    int textOffset = 4;
                    int colorBoxExtraWidth = colorBoxSize + textOffset;
                    infoTextRect.Width += colorBoxExtraWidth;
                    infoTextRect.Location = new Point(x + (totalSize.Width / 2) - (infoTextRect.Width / 2), y + infoTextPosition);
                    Point padding = new Point(infoTextPadding + colorBoxExtraWidth, infoTextPadding);

                    Rectangle colorRect = new Rectangle(infoTextRect.X + colorBoxOffset, infoTextRect.Y + colorBoxOffset, colorBoxSize, colorBoxSize);

                    DrawInfoText(g, infoText, infoTextRect, infoFont, padding);

                    using (Brush colorBrush = new SolidBrush(ShapeManager.GetCurrentColor()))
                    {
                        g.FillRectangle(colorBrush, colorRect);
                    }

                    g.DrawLine(textInnerBorderPen, colorRect.Right, colorRect.Top, colorRect.Right, colorRect.Bottom - 1);
                }
                else
                {
                    infoTextRect.Location = new Point(x + (totalSize.Width / 2) - (infoTextRect.Width / 2), y + infoTextPosition);
                    Point padding = new Point(infoTextPadding, infoTextPadding);

                    DrawInfoText(g, infoText, infoTextRect, infoFont, padding);
                }
            }
        }

        private Bitmap Magnifier(Image img, Point position, int horizontalPixelCount, int verticalPixelCount, int pixelSize)
        {
            horizontalPixelCount = (horizontalPixelCount | 1).Clamp(1, 101);
            verticalPixelCount = (verticalPixelCount | 1).Clamp(1, 101);
            pixelSize = pixelSize.Clamp(1, 1000);

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

                g.DrawImage(img, new Rectangle(0, 0, width, height), new Rectangle(position.X - (horizontalPixelCount / 2) - CanvasRectangle.X,
                    position.Y - (verticalPixelCount / 2) - CanvasRectangle.Y, horizontalPixelCount, verticalPixelCount), GraphicsUnit.Pixel);

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
                        g.DrawLine(pen, new Point((x * pixelSize) - 1, 0), new Point((x * pixelSize) - 1, height - 1));
                    }

                    for (int y = 1; y < verticalPixelCount; y++)
                    {
                        g.DrawLine(pen, new Point(0, (y * pixelSize) - 1), new Point(width - 1, (y * pixelSize) - 1));
                    }
                }

                g.DrawRectangle(Pens.Black, ((width - pixelSize) / 2) - 1, ((height - pixelSize) / 2) - 1, pixelSize, pixelSize);

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
                    g.DrawLine(pen, new Point(rect.X + (x * rulerWidth), rect.Y), new Point(rect.X + (x * rulerWidth), rect.Y + rulerSize));
                    g.DrawLine(pen, new Point(rect.X + (x * rulerWidth), rect.Bottom), new Point(rect.X + (x * rulerWidth), rect.Bottom - rulerSize));
                }

                for (int y = 1; y <= rect.Height / rulerWidth; y++)
                {
                    g.DrawLine(pen, new Point(rect.X, rect.Y + (y * rulerWidth)), new Point(rect.X + rulerSize, rect.Y + (y * rulerWidth)));
                    g.DrawLine(pen, new Point(rect.Right, rect.Y + (y * rulerWidth)), new Point(rect.Right - rulerSize, rect.Y + (y * rulerWidth)));
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

        public Bitmap GetResultImage()
        {
            if (IsEditorMode)
            {
                return ShapeManager.RenderOutputImage(Canvas, CanvasRectangle.Location);
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

                if (gp != null)
                {
                    using (Bitmap bmp = RegionCaptureTasks.ApplyRegionPathToImage(Canvas, gp, out Rectangle rect))
                    {
                        return ShapeManager.RenderOutputImage(bmp, rect.Location);
                    }
                }
            }
            else if (Result == RegionResult.Fullscreen)
            {
                return ShapeManager.RenderOutputImage(Canvas);
            }
            else if (Result == RegionResult.Monitor)
            {
                Screen[] screens = Screen.AllScreens;

                if (MonitorIndex < screens.Length)
                {
                    Screen screen = screens[MonitorIndex];
                    Rectangle screenRect = CaptureHelpers.ScreenToClient(screen.Bounds);

                    using (Bitmap bmp = ShapeManager.RenderOutputImage(Canvas))
                    {
                        return ImageHelpers.CropBitmap(bmp, screenRect);
                    }
                }
            }
            else if (Result == RegionResult.ActiveMonitor)
            {
                Rectangle activeScreenRect = CaptureHelpers.GetActiveScreenBounds0Based();

                using (Bitmap bmp = ShapeManager.RenderOutputImage(Canvas))
                {
                    return ImageHelpers.CropBitmap(bmp, activeScreenRect);
                }
            }

            return null;
        }

        private Bitmap ReceiveImageForTask()
        {
            Bitmap bmp = GetResultImage();

            ShapeManager.IsModified = false;

            if (Options.AutoCloseEditorOnTask)
            {
                CloseWindow();
            }

            return bmp;
        }

        internal void OnSaveImageRequested()
        {
            if (SaveImageRequested != null)
            {
                Bitmap bmp = ReceiveImageForTask();

                string imageFilePath = SaveImageRequested(bmp, ImageFilePath);

                if (!string.IsNullOrEmpty(imageFilePath))
                {
                    ImageFilePath = imageFilePath;
                    UpdateTitle();
                    // TODO: Translate
                    ShapeManager.ShowMenuTooltip("Image saved");
                }
            }
        }

        internal void OnSaveImageAsRequested()
        {
            if (SaveImageAsRequested != null)
            {
                Bitmap bmp = ReceiveImageForTask();

                string imageFilePath = SaveImageAsRequested(bmp, ImageFilePath);

                if (!string.IsNullOrEmpty(imageFilePath))
                {
                    ImageFilePath = imageFilePath;
                    UpdateTitle();
                    // TODO: Translate
                    ShapeManager.ShowMenuTooltip("Image saved");
                }
            }
        }

        internal void OnCopyImageRequested()
        {
            if (CopyImageRequested != null)
            {
                Bitmap bmp = ReceiveImageForTask();

                CopyImageRequested(bmp);
                // TODO: Translate
                ShapeManager.ShowMenuTooltip("Image copied");
            }
        }

        internal void OnUploadImageRequested()
        {
            if (UploadImageRequested != null)
            {
                Bitmap bmp = ReceiveImageForTask();

                UploadImageRequested(bmp);
                // TODO: Translate
                ShapeManager.ShowMenuTooltip("Image uploading");
            }
        }

        internal void OnPrintImageRequested()
        {
            if (PrintImageRequested != null)
            {
                Bitmap bmp = ReceiveImageForTask();

                PrintImageRequested(bmp);
            }
        }

        protected override void Dispose(bool disposing)
        {
            IsClosing = true;

            ShapeManager?.Dispose();
            backgroundBrush?.Dispose();
            backgroundHighlightBrush?.Dispose();
            borderPen?.Dispose();
            borderDotPen?.Dispose();
            borderDotStaticPen?.Dispose();
            infoFont?.Dispose();
            infoFontMedium?.Dispose();
            infoFontBig?.Dispose();
            textBrush?.Dispose();
            textShadowBrush?.Dispose();
            textBackgroundBrush?.Dispose();
            textOuterBorderPen?.Dispose();
            textInnerBorderPen?.Dispose();
            markerPen?.Dispose();
            canvasBorderPen?.Dispose();
            defaultCursor?.Dispose();
            openHandCursor?.Dispose();
            closedHandCursor?.Dispose();
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
            DimmedCanvas?.Dispose();
            Canvas?.Dispose();

            base.Dispose(disposing);
        }
    }
}