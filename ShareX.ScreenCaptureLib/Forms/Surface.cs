#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
    public class Surface : Form
    {
        public Image SurfaceImage { get; set; }
        public SurfaceOptions Config { get; set; }
        public int FPS { get; private set; }
        public Rectangle ScreenRectangle { get; private set; }
        public Rectangle ScreenRectangle0Based { get; private set; }
        public SurfaceResult Result { get; private set; }
        public int MonitorIndex { get; private set; }

        protected List<DrawableObject> DrawableObjects { get; set; }

        protected TextureBrush darkBackgroundBrush, lightBackgroundBrush;
        protected GraphicsPath regionFillPath, regionDrawPath;
        protected Pen borderPen, borderDotPen;
        protected Brush nodeBackgroundBrush;
        protected Font textFont, tipFont;
        protected Stopwatch timer;
        protected int frameCount;

        public static GraphicsPath LastRegionFillPath, LastRegionDrawPath;

        public Surface()
        {
            ScreenRectangle = CaptureHelpers.GetScreenBounds();
            ScreenRectangle0Based = CaptureHelpers.ScreenToClient(ScreenRectangle);

            InitializeComponent();

            using (MemoryStream cursorStream = new MemoryStream(Resources.Crosshair))
            {
                Cursor = new Cursor(cursorStream);
            }

            DrawableObjects = new List<DrawableObject>();
            Config = new SurfaceOptions();
            timer = new Stopwatch();

            borderPen = new Pen(Color.Black);
            borderDotPen = new Pen(Color.White);
            borderDotPen.DashPattern = new float[] { 5, 5 };
            nodeBackgroundBrush = new SolidBrush(Color.White);
            textFont = new Font("Verdana", 16, FontStyle.Bold);
            tipFont = new Font("Verdana", 9);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            StartPosition = FormStartPosition.Manual;
            Bounds = ScreenRectangle;
            FormBorderStyle = FormBorderStyle.None;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Text = "ShareX - " + Resources.Surface_InitializeComponent_Region_capture;
            ShowInTaskbar = false;
            TopMost = true;
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
                using (Image darkSurfaceImage = ColorMatrixManager.Contrast(0.9f).Apply(SurfaceImage))
                {
                    darkBackgroundBrush = new TextureBrush(darkSurfaceImage) { WrapMode = WrapMode.Clamp };
                }

                using (Image lightSurfaceImage = ColorMatrixManager.Contrast(1.1f).Apply(SurfaceImage))
                {
                    lightBackgroundBrush = new TextureBrush(lightSurfaceImage) { WrapMode = WrapMode.Clamp };
                }
            }
            else
            {
                darkBackgroundBrush = new TextureBrush(SurfaceImage);
            }
        }

        private void Surface_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        private void Surface_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                MonitorKey(e.KeyCode - Keys.D0);
                return;
            }

            if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                MonitorKey(e.KeyCode - Keys.NumPad0);
                return;
            }

            switch (e.KeyCode)
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
                case Keys.Q:
                    Config.QuickCrop = !Config.QuickCrop;
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
                DrawInfo(g);
            }

            Invalidate();
        }

        public virtual Image GetRegionImage()
        {
            return ShapeCaptureHelpers.GetRegionImage(SurfaceImage, regionFillPath, regionDrawPath, Config);
        }

        public void Close(SurfaceResult result)
        {
            Result = result;
            Close();
        }

        protected new virtual void Update()
        {
            if (!timer.IsRunning) timer.Start();

            InputManager.Update();

            DrawableObject[] objects = DrawableObjects.OrderByDescending(x => x.Order).ToArray();

            if (objects.All(x => x.Visible && !x.IsDragging))
            {
                for (int i = 0; i < objects.Count(); i++)
                {
                    DrawableObject obj = objects[i];

                    obj.IsMouseHover = obj.Rectangle.Contains(InputManager.MousePosition0Based);

                    if (obj.IsMouseHover)
                    {
                        for (int y = i + 1; y < objects.Count(); y++)
                        {
                            objects[y].IsMouseHover = false;
                        }

                        break;
                    }
                }

                foreach (DrawableObject obj in objects)
                {
                    if (obj.IsMouseHover && InputManager.IsMousePressed(MouseButtons.Left))
                    {
                        obj.IsDragging = true;
                        break;
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

            borderDotPen.DashOffset = (float)timer.Elapsed.TotalSeconds * 10;
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

            if (timer.ElapsedMilliseconds >= 1000)
            {
                FPS = (int)(frameCount / timer.Elapsed.TotalSeconds);
                frameCount = 0;
                timer.Reset();
                timer.Start();
            }
        }

        private void DrawInfo(Graphics g)
        {
            string text = "FPS: " + FPS;

            SizeF textSize = g.MeasureString(text, textFont);

            int offset = 30;

            Rectangle primaryScreenBounds = CaptureHelpers.GetPrimaryScreenBounds0Based();
            Rectangle textRectangle = new Rectangle(primaryScreenBounds.X + offset, primaryScreenBounds.Y + offset, (int)textSize.Width, (int)textSize.Height);

            if (textRectangle.Offset(10).Contains(InputManager.MousePosition0Based))
            {
                textRectangle.Y = primaryScreenBounds.Height - textRectangle.Height - offset;
            }

            ImageHelpers.DrawTextWithOutline(g, text, textRectangle.Location, textFont, Color.White, Color.Black);
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

        private IContainer components = null;

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
            if (textFont != null) textFont.Dispose();
            if (tipFont != null) tipFont.Dispose();

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
    }
}