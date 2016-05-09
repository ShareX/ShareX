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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public abstract class SurfaceForm : Form
    {
        public Image SurfaceImage { get; set; }
        public SurfaceOptions Config { get; set; }
        public int FPS { get; private set; }
        public Rectangle ScreenRectangle { get; private set; }
        public Rectangle ScreenRectangle0Based { get; private set; }
        public SurfaceResult Result { get; private set; }
        public int MonitorIndex { get; set; }

        protected List<DrawableObject> DrawableObjects { get; set; }

        protected TextureBrush darkBackgroundBrush, lightBackgroundBrush;
        protected GraphicsPath regionFillPath, regionDrawPath;
        protected Pen borderPen, borderDotPen, textBackgroundPenWhite, textBackgroundPenBlack, markerPen;
        protected Brush nodeBackgroundBrush, textBackgroundBrush;
        protected Font infoFont, infoFontMedium, infoFontBig;
        protected Stopwatch timerStart, timerFPS;
        protected int frameCount;
        protected bool isKeyAllowed;

        public static GraphicsPath LastRegionFillPath, LastRegionDrawPath;

        private bool pause;

        public SurfaceForm()
        {
            ScreenRectangle = CaptureHelpers.GetScreenBounds();
            ScreenRectangle0Based = CaptureHelpers.ScreenToClient(ScreenRectangle);

            InitializeComponent();
            Icon = ShareXResources.Icon;

            using (MemoryStream cursorStream = new MemoryStream(Resources.Crosshair))
            {
                Cursor = new Cursor(cursorStream);
            }

            DrawableObjects = new List<DrawableObject>();
            Config = new SurfaceOptions();
            timerStart = new Stopwatch();
            timerFPS = new Stopwatch();

            borderPen = new Pen(Color.Black);
            borderDotPen = new Pen(Color.White);
            borderDotPen.DashPattern = new float[] { 5, 5 };
            nodeBackgroundBrush = new SolidBrush(Color.White);
            infoFont = new Font("Verdana", 9);
            infoFontMedium = new Font("Verdana", 12);
            infoFontBig = new Font("Verdana", 16, FontStyle.Bold);
            textBackgroundBrush = new SolidBrush(Color.FromArgb(75, Color.Black));
            textBackgroundPenWhite = new Pen(Color.FromArgb(50, Color.White));
            textBackgroundPenBlack = new Pen(Color.FromArgb(150, Color.Black));
            markerPen = new Pen(Color.FromArgb(200, Color.Red)) { DashStyle = DashStyle.Dash };
        }

        private void InitializeComponent()
        {
            components = new Container();

            SuspendLayout();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            Bounds = ScreenRectangle;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Text = "ShareX - " + Resources.Surface_InitializeComponent_Region_capture;
            ShowInTaskbar = false;
#if !DEBUG
            TopMost = true;
#endif
            Shown += Surface_Shown;
            KeyUp += Surface_KeyUp;
            MouseDoubleClick += Surface_MouseDoubleClick;
            ResumeLayout(false);
        }

        /// <summary>Must be called before show form</summary>
        public virtual void Prepare()
        {
            if (SurfaceImage == null)
            {
                SurfaceImage = Screenshot.CaptureFullscreen();
            }

            if (Config.UseDimming)
            {
                /*
                using (Image darkSurfaceImage = ColorMatrixManager.Contrast(0.9f).Apply(SurfaceImage))
                {
                    darkBackgroundBrush = new TextureBrush(darkSurfaceImage) { WrapMode = WrapMode.Clamp };
                }

                using (Image lightSurfaceImage = ColorMatrixManager.Contrast(1.1f).Apply(SurfaceImage))
                {
                    lightBackgroundBrush = new TextureBrush(lightSurfaceImage) { WrapMode = WrapMode.Clamp };
                }
                */

                using (Bitmap darkBackground = (Bitmap)SurfaceImage.Clone())
                using (Graphics g = Graphics.FromImage(darkBackground))
                {
                    using (Brush brush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                    {
                        g.FillRectangle(brush, 0, 0, darkBackground.Width, darkBackground.Height);
                    }

                    darkBackgroundBrush = new TextureBrush(darkBackground) { WrapMode = WrapMode.Clamp };
                    lightBackgroundBrush = new TextureBrush(SurfaceImage) { WrapMode = WrapMode.Clamp };
                }
            }
            else
            {
                darkBackgroundBrush = new TextureBrush(SurfaceImage) { WrapMode = WrapMode.Clamp };
            }
        }

        private void Surface_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void Surface_KeyUp(object sender, KeyEventArgs e)
        {
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

            /*if (e.KeyData >= Keys.NumPad0 && e.KeyData <= Keys.NumPad9)
            {
                MonitorKey(e.KeyData - Keys.NumPad0);
                return;
            }*/

            switch (e.KeyData)
            {
                case Keys.Escape:
                    Close(SurfaceResult.Close);
                    break;
                case Keys.Space:
                    Close(SurfaceResult.Fullscreen);
                    break;
                case Keys.Enter:
                    Close(SurfaceResult.Region);
                    break;
                case Keys.Oemtilde:
                    Close(SurfaceResult.ActiveMonitor);
                    break;
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

            Close(SurfaceResult.Monitor);
        }

        private void Surface_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Close(SurfaceResult.Region);
            }
        }

        public void Pause()
        {
            pause = true;
        }

        public void Resume()
        {
            pause = false;
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Update();

            Graphics g = e.Graphics;
            g.CompositingMode = CompositingMode.SourceCopy;
            g.FillRectangle(darkBackgroundBrush, ScreenRectangle0Based);
            g.CompositingMode = CompositingMode.SourceOver;

            Draw(g);

            if (Config.ShowFPS)
            {
                CheckFPS();
                DrawFPS(g);
            }

            if (!pause)
            {
                Invalidate();
            }
        }

        public virtual Image GetResultImage()
        {
            if (Result == SurfaceResult.Region)
            {
                using (Image img = GetOutputImage())
                {
                    return ShapeCaptureHelpers.GetRegionImage(img, regionFillPath, regionDrawPath, Config);
                }
            }
            else if (Result == SurfaceResult.Fullscreen)
            {
                return GetOutputImage();
            }
            else if (Result == SurfaceResult.Monitor)
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
            else if (Result == SurfaceResult.ActiveMonitor)
            {
                Rectangle activeScreenRect = CaptureHelpers.GetActiveScreenBounds0Based();

                using (Image img = GetOutputImage())
                {
                    return ImageHelpers.CropImage(img, activeScreenRect);
                }
            }

            return null;
        }

        protected virtual Image GetOutputImage()
        {
            return (Image)SurfaceImage.Clone();
        }

        public virtual WindowInfo GetWindowInfo()
        {
            return null;
        }

        public void Close(SurfaceResult result)
        {
            Result = result;
            Close();
        }

        protected new virtual void Update()
        {
            if (!timerStart.IsRunning)
            {
                timerStart.Start();
                timerFPS.Start();
            }

            InputManager.Update();

            DrawableObject[] objects = DrawableObjects.OrderByDescending(x => x.Order).ToArray();

            if (objects.All(x => !x.IsDragging))
            {
                for (int i = 0; i < objects.Count(); i++)
                {
                    DrawableObject obj = objects[i];

                    if (obj.Visible)
                    {
                        obj.IsMouseHover = obj.Rectangle.Contains(InputManager.MousePosition0Based);

                        if (obj.IsMouseHover)
                        {
                            if (InputManager.IsMousePressed(MouseButtons.Left))
                            {
                                obj.IsDragging = true;
                            }

                            for (int y = i + 1; y < objects.Count(); y++)
                            {
                                objects[y].IsMouseHover = false;
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
                        obj.IsDragging = false;
                    }
                }
            }

            borderDotPen.DashOffset = (float)timerStart.Elapsed.TotalSeconds * -15;
        }

        protected virtual void Draw(Graphics g)
        {
            DrawObjects(g);
        }

        protected void DrawObjects(Graphics g)
        {
            foreach (DrawableObject drawObject in DrawableObjects)
            {
                if (drawObject.Visible)
                {
                    drawObject.Draw(g);
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
            }
        }

        private void DrawFPS(Graphics g)
        {
            string text = "FPS: " + FPS;

            SizeF textSize = g.MeasureString(text, infoFontBig);

            int offset = 10;

            Rectangle primaryScreenBounds = CaptureHelpers.GetPrimaryScreenBounds0Based();
            Rectangle textRectangle = new Rectangle(primaryScreenBounds.X + offset, primaryScreenBounds.Y + offset, (int)textSize.Width, (int)textSize.Height);

            if (textRectangle.Offset(10).Contains(InputManager.MousePosition0Based))
            {
                textRectangle.Y = primaryScreenBounds.Height - textRectangle.Height - offset;
            }

            ImageHelpers.DrawTextWithOutline(g, text, textRectangle.Location, infoFontBig, Color.White, Color.Black);
        }

        protected Rectangle CalculateAreaFromNodes()
        {
            IEnumerable<NodeObject> nodes = DrawableObjects.OfType<NodeObject>().Where(x => x.Visible);

            if (nodes.Count() > 1)
            {
                int left = (int)nodes.Min(x => x.Position.X);
                int top = (int)nodes.Min(x => x.Position.Y);
                int right = (int)nodes.Max(x => x.Position.X);
                int bottom = (int)nodes.Max(x => x.Position.Y);

                return CaptureHelpers.CreateRectangle(new Point(left, top), new Point(right, bottom));
            }

            return Rectangle.Empty;
        }

        internal NodeObject MakeNode()
        {
            NodeObject node = new NodeObject();
            DrawableObjects.Add(node);
            return node;
        }

        protected void ShowNodes()
        {
            foreach (NodeObject node in DrawableObjects.OfType<NodeObject>())
            {
                node.Visible = true;
            }
        }

        protected void HideNodes()
        {
            foreach (NodeObject node in DrawableObjects.OfType<NodeObject>())
            {
                node.Visible = false;
            }
        }

        public IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (darkBackgroundBrush != null) darkBackgroundBrush.Dispose();
            if (lightBackgroundBrush != null) lightBackgroundBrush.Dispose();
            if (borderPen != null) borderPen.Dispose();
            if (borderDotPen != null) borderDotPen.Dispose();
            if (nodeBackgroundBrush != null) nodeBackgroundBrush.Dispose();
            if (infoFont != null) infoFont.Dispose();
            if (infoFontMedium != null) infoFontMedium.Dispose();
            if (infoFontBig != null) infoFontBig.Dispose();
            if (textBackgroundBrush != null) textBackgroundBrush.Dispose();
            if (textBackgroundPenWhite != null) textBackgroundPenWhite.Dispose();
            if (textBackgroundPenBlack != null) textBackgroundPenBlack.Dispose();
            if (markerPen != null) markerPen.Dispose();

            if (regionFillPath != null)
            {
                if (LastRegionFillPath != null) LastRegionFillPath.Dispose();
                LastRegionFillPath = regionFillPath;
                if (LastRegionDrawPath != null) LastRegionDrawPath.Dispose();
                LastRegionDrawPath = regionDrawPath;
            }
            else
            {
                if (regionFillPath != null) regionFillPath.Dispose();
                if (regionDrawPath != null) regionDrawPath.Dispose();
            }

            base.Dispose(disposing);
        }

        public static bool SelectRegion(out Rectangle rect)
        {
            return SelectRegion(out rect, new SurfaceOptions());
        }

        public static bool SelectRegion(out Rectangle rect, SurfaceOptions options)
        {
            using (RectangleRegionForm surface = new RectangleRegionForm())
            {
                surface.Config = options;
                surface.Config.ShowTips = false;
                surface.Config.QuickCrop = true;
                surface.Config.DetectWindows = true;
                surface.Prepare();
                surface.ShowDialog();

                if (surface.Result == SurfaceResult.Region)
                {
                    if (surface.AreaManager.IsCurrentRegionValid)
                    {
                        rect = CaptureHelpers.ClientToScreen(surface.AreaManager.CurrentRectangle);
                        return true;
                    }
                }
                else if (surface.Result == SurfaceResult.Fullscreen)
                {
                    rect = CaptureHelpers.GetScreenBounds();
                    return true;
                }
                else if (surface.Result == SurfaceResult.Monitor)
                {
                    Screen[] screens = Screen.AllScreens;

                    if (surface.MonitorIndex < screens.Length)
                    {
                        Screen screen = screens[surface.MonitorIndex];
                        rect = screen.Bounds;
                        return true;
                    }
                }
                else if (surface.Result == SurfaceResult.ActiveMonitor)
                {
                    rect = CaptureHelpers.GetActiveScreenBounds();
                    return true;
                }
            }

            rect = Rectangle.Empty;
            return false;
        }
    }
}