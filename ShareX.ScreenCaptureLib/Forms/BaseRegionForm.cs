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
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public abstract class BaseRegionForm : Form
    {
        public static GraphicsPath LastRegionFillPath { get; protected set; }

        public RegionCaptureOptions Config { get; set; }
        public Rectangle ScreenRectangle { get; private set; }
        public Rectangle ScreenRectangle0Based { get; private set; }
        public Image Image { get; protected set; }
        public Rectangle ImageRectangle { get; protected set; }
        public RegionResult Result { get; private set; }
        public int FPS { get; private set; }
        public int MonitorIndex { get; set; }

        internal List<DrawableObject> DrawableObjects { get; private set; }

        protected TextureBrush backgroundBrush, backgroundHighlightBrush;
        protected GraphicsPath regionFillPath, regionDrawPath;
        protected Pen borderPen, borderDotPen, textBackgroundPenWhite, textBackgroundPenBlack, markerPen;
        protected Brush nodeBackgroundBrush, textBackgroundBrush;
        protected Font infoFont, infoFontMedium, infoFontBig;
        protected Stopwatch timerStart, timerFPS;
        protected int frameCount;
        protected bool pause, isKeyAllowed;

        public BaseRegionForm()
        {
            ScreenRectangle = CaptureHelpers.GetScreenBounds();
            ScreenRectangle0Based = CaptureHelpers.ScreenToClient(ScreenRectangle);
            ImageRectangle = ScreenRectangle0Based;

            InitializeComponent();
            Icon = ShareXResources.Icon;
            Cursor = Helpers.CreateCursor(Resources.Crosshair);

            DrawableObjects = new List<DrawableObject>();
            Config = new RegionCaptureOptions();
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
            markerPen = new Pen(Color.FromArgb(200, Color.Red));
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
            Text = "ShareX - " + Resources.BaseRegionForm_InitializeComponent_Region_capture;
            ShowInTaskbar = false;
#if !DEBUG
            TopMost = true;
#endif
            Shown += BaseRegionForm_Shown;
            KeyUp += BaseRegionForm_KeyUp;
            ResumeLayout(false);
        }

        private void BaseRegionForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void BaseRegionForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Close(RegionResult.Close);
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
            g.FillRectangle(backgroundBrush, ScreenRectangle0Based);
            g.CompositingMode = CompositingMode.SourceOver;

            Draw(g);

            if (Config.ShowFPS)
            {
                CheckFPS();
                DrawFPS(g, 10);
            }

            if (!pause)
            {
                Invalidate();
            }
        }

        public virtual Image GetResultImage()
        {
            if (Result == RegionResult.Region)
            {
                using (Image img = GetOutputImage())
                {
                    return RegionCaptureHelpers.ApplyRegionPathToImage(img, regionFillPath);
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

        protected virtual Image GetOutputImage()
        {
            return (Image)Image.Clone();
        }

        public virtual WindowInfo GetWindowInfo()
        {
            return null;
        }

        public void Close(RegionResult result)
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
                        obj.IsCursorHover = obj.Rectangle.Contains(InputManager.MousePosition0Based);

                        if (obj.IsCursorHover)
                        {
                            if (InputManager.IsMousePressed(MouseButtons.Left))
                            {
                                obj.IsDragging = true;
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

        private void DrawFPS(Graphics g, int offset)
        {
            ImageHelpers.DrawTextWithShadow(g, FPS.ToString(), new Point(offset, offset), infoFontBig, Brushes.White, Brushes.Black, new Point(0, 1));
        }

        public IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (backgroundBrush != null) backgroundBrush.Dispose();
            if (backgroundHighlightBrush != null) backgroundHighlightBrush.Dispose();
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
            }
            else
            {
                if (regionFillPath != null) regionFillPath.Dispose();
                if (regionDrawPath != null) regionDrawPath.Dispose();
            }

            if (Image != null) Image.Dispose();

            base.Dispose(disposing);
        }
    }
}