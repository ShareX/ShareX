#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using ScreenCapture.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScreenCapture
{
    public class Surface : Form
    {
        public Image SurfaceImage { get; set; }
        public SurfaceOptions Config { get; set; }
        public int FPS { get; private set; }
        public Rectangle ScreenRectangle { get; private set; }
        public Rectangle ScreenRectangle0Based { get; private set; }
        public SurfaceResult Result { get; private set; }

        protected List<DrawableObject> DrawableObjects { get; set; }

        protected TextureBrush darkBackgroundBrush, lightBackgroundBrush;
        protected GraphicsPath regionFillPath, regionDrawPath;
        protected Pen borderPen, borderDotPen, borderDotPen2;
        protected Brush nodeBackgroundBrush;
        protected Font textFont;
        protected Stopwatch timer;
        protected int frameCount;

        public static GraphicsPath LastRegionFillPath, LastRegionDrawPath;

        public Surface(Image backgroundImage = null)
        {
            ScreenRectangle = CaptureHelpers.GetScreenBounds();
            ScreenRectangle0Based = CaptureHelpers.ScreenToClient(ScreenRectangle);

            InitializeComponent();

            using (MemoryStream cursorStream = new MemoryStream(Resources.Crosshair))
            {
                Cursor = new Cursor(cursorStream);
            }

            if (backgroundImage != null)
            {
                SurfaceImage = backgroundImage;
                Prepare();
            }

            DrawableObjects = new List<DrawableObject>();
            Config = new SurfaceOptions();
            timer = new Stopwatch();

            borderPen = new Pen(Color.Black);
            borderDotPen = new Pen(Color.Black, 1);
            borderDotPen2 = new Pen(Color.White, 1);
            borderDotPen2.DashPattern = new float[] { 5, 5 };
            nodeBackgroundBrush = new SolidBrush(Color.White);
            textFont = new Font("Arial", 17, FontStyle.Bold);
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
            Text = "ShareX - Region Capture";
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

            using (Image darkSurfaceImage = ColorMatrixManager.Contrast(0.8f).Apply(SurfaceImage))
            {
                darkBackgroundBrush = new TextureBrush(darkSurfaceImage);
            }

            using (Image lightSurfaceImage = ColorMatrixManager.Contrast(1.1f).Apply(SurfaceImage))
            {
                lightBackgroundBrush = new TextureBrush(lightSurfaceImage);
            }
        }

        private void Surface_Shown(object sender, EventArgs e)
        {
            Activate();
        }

        private void Surface_KeyUp(object sender, KeyEventArgs e)
        {
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
                case Keys.Q:
                    Config.QuickCrop = !Config.QuickCrop;
                    break;
            }
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
            if (!timer.IsRunning) timer.Start();

            Update();

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.FillRectangle(darkBackgroundBrush, ScreenRectangle0Based);

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

            Rectangle primaryScreen = Screen.PrimaryScreen.Bounds;

            Point position = CaptureHelpers.ScreenToClient(new Point(primaryScreen.X + offset, primaryScreen.Y + offset));
            Rectangle rect = new Rectangle(position, new Size((int)textSize.Width, (int)textSize.Height));

            if (rect.Contains(InputManager.MousePosition0Based))
            {
                position = CaptureHelpers.ScreenToClient(new Point(primaryScreen.X + offset, primaryScreen.Y + primaryScreen.Height - (int)textSize.Height - offset));
            }

            ImageHelpers.DrawTextWithOutline(g, text, position, textFont, Color.White, Color.Black);
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

        public NodeObject MakeNode()
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
            if (borderDotPen2 != null) borderDotPen2.Dispose();
            if (nodeBackgroundBrush != null) nodeBackgroundBrush.Dispose();
            if (textFont != null) textFont.Dispose();

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