#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public class RectangleRegion : Surface
    {
        public AreaManager AreaManager { get; private set; }

        // For screen color picker
        public bool OneClickMode { get; set; }
        public Point OneClickPosition { get; set; }

        // For screen ruler
        public bool RulerMode { get; set; }

        public RectangleRegion(Image backgroundImage = null)
            : base(backgroundImage)
        {
            AreaManager = new AreaManager(this);
            KeyDown += RectangleRegion_KeyDown;
            MouseDown += RectangleRegion_MouseDown;
            MouseWheel += RectangleRegion_MouseWheel;
        }

        private void RectangleRegion_MouseDown(object sender, MouseEventArgs e)
        {
            if (OneClickMode && e.Button == MouseButtons.Left)
            {
                OneClickPosition = e.Location;
                Close(SurfaceResult.Region);
            }
        }

        private void RectangleRegion_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.I:
                    Config.ShowInfo = !Config.ShowInfo;
                    break;
                case Keys.C:
                    Config.ShowCrosshair = !Config.ShowCrosshair;
                    break;
                case Keys.M:
                    Config.ShowMagnifier = !Config.ShowMagnifier;
                    break;
                case Keys.Control | Keys.C:
                    CopyAreaInfo();
                    break;
            }
        }

        private void CopyAreaInfo()
        {
            if (AreaManager.IsCurrentAreaValid)
            {
                string clipboardText;

                if (RulerMode)
                {
                    clipboardText = GetRulerText(AreaManager.CurrentArea);
                }
                else
                {
                    clipboardText = GetAreaText(AreaManager.CurrentArea);
                }

                ClipboardHelpers.CopyText(clipboardText);
            }
        }

        private void RectangleRegion_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    if (Config.MagnifierPixelSize < 30) Config.MagnifierPixelSize++;
                }
                else
                {
                    if (Config.MagnifierPixelCount < 41) Config.MagnifierPixelCount += 2;
                }
            }
            else if (e.Delta < 0)
            {
                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    if (Config.MagnifierPixelSize > 2) Config.MagnifierPixelSize--;
                }
                else
                {
                    if (Config.MagnifierPixelCount > 2) Config.MagnifierPixelCount -= 2;
                }
            }
        }

        public override void Prepare()
        {
            base.Prepare();

            if (Config != null)
            {
                AreaManager.WindowCaptureMode |= Config.ForceWindowCapture;
                AreaManager.IncludeControls |= Config.IncludeControls;

                if (AreaManager.WindowCaptureMode)
                {
                    IntPtr handle = Handle;

                    TaskEx.Run(() =>
                    {
                        WindowsRectangleList wla = new WindowsRectangleList();
                        wla.IgnoreHandle = handle;
                        wla.IncludeChildWindows = AreaManager.IncludeControls;
                        AreaManager.Windows = wla.GetWindowsRectangleList();
                    });
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            AreaManager.Update();
        }

        protected override void Draw(Graphics g)
        {
            List<Rectangle> areas = AreaManager.GetValidAreas;

            if (areas.Count > 0 || !AreaManager.CurrentHoverArea.IsEmpty)
            {
                UpdateRegionPath();

                if (areas.Count > 0)
                {
                    using (Region region = new Region(regionDrawPath))
                    {
                        g.Clip = region;
                        g.FillRectangle(lightBackgroundBrush, ScreenRectangle0Based);
                        g.ResetClip();
                    }

                    g.DrawPath(borderPen, regionDrawPath);

                    if (areas.Count > 1)
                    {
                        Rectangle totalArea = AreaManager.CombineAreas();
                        g.DrawCrossRectangle(borderPen, totalArea, 15);

                        if (Config.ShowInfo)
                        {
                            ImageHelpers.DrawTextWithOutline(g, string.Format("X: {0} / Y: {1} / W: {2} / H: {3}", totalArea.X, totalArea.Y,
                                totalArea.Width, totalArea.Height), new PointF(totalArea.X + 5, totalArea.Y - 25), textFont, Color.White, Color.Black);
                        }
                    }
                }

                if (AreaManager.IsCurrentHoverAreaValid)
                {
                    using (GraphicsPath hoverDrawPath = new GraphicsPath { FillMode = FillMode.Winding })
                    {
                        AddShapePath(hoverDrawPath, AreaManager.CurrentHoverArea.SizeOffset(-1));

                        g.DrawPath(borderPen, hoverDrawPath);
                        g.DrawPath(borderDotPen, hoverDrawPath);
                    }
                }

                if (AreaManager.IsCurrentAreaValid)
                {
                    g.DrawRectangleProper(borderPen, AreaManager.CurrentArea);
                    g.DrawRectangleProper(borderDotPen, AreaManager.CurrentArea);
                    DrawObjects(g);

                    if (RulerMode)
                    {
                        DrawRuler(g, AreaManager.CurrentArea, borderPen, 5, 10);
                        DrawRuler(g, AreaManager.CurrentArea, borderPen, 15, 100);

                        Point centerPos = new Point(AreaManager.CurrentArea.X + AreaManager.CurrentArea.Width / 2, AreaManager.CurrentArea.Y + AreaManager.CurrentArea.Height / 2);
                        int markSize = 10;
                        g.DrawLine(borderPen, centerPos.X, centerPos.Y - markSize, centerPos.X, centerPos.Y + markSize);
                        g.DrawLine(borderPen, centerPos.X - markSize, centerPos.Y, centerPos.X + markSize, centerPos.Y);
                    }
                }

                if (Config.ShowInfo)
                {
                    foreach (Rectangle area in areas)
                    {
                        if (area.IsValid())
                        {
                            if (RulerMode)
                            {
                                ImageHelpers.DrawTextWithOutline(g, GetRulerText(area), new PointF(area.X + 15, area.Y + 15), textFont, Color.White, Color.Black);
                            }
                            else
                            {
                                ImageHelpers.DrawTextWithOutline(g, GetAreaText(area), new PointF(area.X + 5, area.Y + 5), textFont, Color.White, Color.Black);
                            }
                        }
                    }
                }
            }

            if (Config.ShowMagnifier)
            {
                DrawMagnifier(g);
            }

            if (Config.ShowCrosshair)
            {
                DrawCrosshair(g);
            }
        }

        private string GetRulerText(Rectangle area)
        {
            Point endPos = new Point(area.X + area.Width - 1, area.Y + area.Height - 1);
            return string.Format("X: {0} / Y: {1} / X2: {2} / Y2: {3}\nWidth: {4} px / Height: {5} px\nDistance: {6:0.00} px / Angle: {7:0.00}°", area.X, area.Y, endPos.X, endPos.Y,
                area.Width, area.Height, MathHelpers.Distance(area.Location, endPos), MathHelpers.LookAtDegree(area.Location, endPos));
        }

        private string GetAreaText(Rectangle area)
        {
            return string.Format("X: {0} / Y: {1}\nW: {2} / H: {3}", area.X, area.Y, area.Width, area.Height);
        }

        private void DrawCrosshair(Graphics g)
        {
            int offset = 5;
            Point mousePos = InputManager.MousePosition0Based;
            Point left = new Point(mousePos.X - offset, mousePos.Y), left2 = new Point(0, mousePos.Y);
            Point right = new Point(mousePos.X + offset, mousePos.Y), right2 = new Point(ScreenRectangle0Based.Width - 1, mousePos.Y);
            Point top = new Point(mousePos.X, mousePos.Y - offset), top2 = new Point(mousePos.X, 0);
            Point bottom = new Point(mousePos.X, mousePos.Y + offset), bottom2 = new Point(mousePos.X, ScreenRectangle0Based.Height - 1);

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

        private void DrawMagnifier(Graphics g)
        {
            Point mousePos = InputManager.MousePosition0Based;
            int offsetX = 10, offsetY = 10;

            if (Config.ShowInfo && AreaManager.IsCurrentAreaValid && AreaManager.CurrentArea.Location == mousePos)
            {
                offsetY = 50;
            }

            using (Bitmap magnifier = Magnifier(SurfaceImage, mousePos, Config.MagnifierPixelCount, Config.MagnifierPixelCount, Config.MagnifierPixelSize))
            {
                int x = mousePos.X + offsetX;

                if (x + magnifier.Width > ScreenRectangle0Based.Width)
                {
                    x = mousePos.X - offsetX - magnifier.Width;
                }

                int y = mousePos.Y + offsetY;

                if (y + magnifier.Height > ScreenRectangle0Based.Height)
                {
                    y = mousePos.Y - offsetY - magnifier.Height;
                }

                g.SetHighQuality();

                using (TextureBrush brush = new TextureBrush(magnifier))
                {
                    brush.TranslateTransform(x, y);
                    g.FillEllipse(brush, x, y, magnifier.Width, magnifier.Height);
                    g.DrawEllipse(Pens.Black, x, y, magnifier.Width, magnifier.Height);
                }
            }
        }

        private Bitmap Magnifier(Image img, Point position, int horizontalPixelCount, int verticalPixelCount, int pixelSize)
        {
            horizontalPixelCount = (horizontalPixelCount | 1).Between(1, 101);
            verticalPixelCount = (verticalPixelCount | 1).Between(1, 101);
            pixelSize = pixelSize.Between(1, 1000);

            if (horizontalPixelCount * pixelSize > ScreenRectangle0Based.Width || verticalPixelCount * pixelSize > ScreenRectangle0Based.Height)
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

                g.DrawImage(img, new Rectangle(0, 0, width, height), new Rectangle(position.X - horizontalPixelCount / 2, position.Y - verticalPixelCount / 2,
                    horizontalPixelCount, verticalPixelCount), GraphicsUnit.Pixel);

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
                g.DrawRectangle(Pens.White, (width - pixelSize) / 2, (height - pixelSize) / 2, pixelSize - 2, pixelSize - 2);
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

        public void UpdateRegionPath()
        {
            regionFillPath = new GraphicsPath { FillMode = FillMode.Winding };
            regionDrawPath = new GraphicsPath { FillMode = FillMode.Winding };

            foreach (Rectangle area in AreaManager.GetValidAreas)
            {
                AddShapePath(regionFillPath, area);
                AddShapePath(regionDrawPath, area.SizeOffset(-1));
            }
        }

        protected virtual void AddShapePath(GraphicsPath graphicsPath, Rectangle rect)
        {
            graphicsPath.AddRectangle(rect);
        }
    }
}