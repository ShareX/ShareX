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
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib
{
    internal class ScrollbarManager
    {
        public bool IsVisible => IsHorizontalScrollbarVisible || IsVerticalScrollbarVisible;
        public bool IsHorizontalScrollbarVisible { get; private set; }
        public bool IsVerticalScrollbarVisible { get; private set; }

        private bool shouldDrawHorizontalScrollbar = true;
        private bool shouldDrawVerticalScrollbar = true;
        private bool shouldDrawHorizontalScrollbarBefore = true;
        private bool shouldDrawVerticalScrollbarBefore = true;

        private Stopwatch horizontalScrollbarChangeTime;
        private Stopwatch verticalScrollbarChangeTime;

        public int Thickness { get; set; } = 10;
        public int Margin { get; set; } = 15;
        public int Padding { get; set; } = 2;

        // timings in milliseconds
        public int FadeInTime { get; set; } = 150;
        public int FadeOutDelay { get; set; } = 500;
        public int FadeOutTime { get; set; } = 150;

        private int HorizontalScrollbarOpacityLast = 255;
        private int HorizontalScrollbarOpacityCurrent = 255;
        private int VerticalScrollbarOpacityLast = 255;
        private int VerticalScrollbarOpacityCurrent = 255;

        private RegionCaptureForm form;
        private Rectangle horizontalTrackRectangle, horizontalThumbRectangle, verticalTrackRectangle, verticalThumbRectangle;

        public ScrollbarManager(RegionCaptureForm regionCaptureForm)
        {
            form = regionCaptureForm;
            horizontalScrollbarChangeTime = Stopwatch.StartNew();
            verticalScrollbarChangeTime = Stopwatch.StartNew();
            if (form.ClientArea.Contains(form.CanvasRectangle))
            {
                HorizontalScrollbarOpacityLast = 0;
                HorizontalScrollbarOpacityCurrent = 0;
                VerticalScrollbarOpacityLast = 0;
                VerticalScrollbarOpacityCurrent = 0;
            }
        }

        int opacityGain(Stopwatch changeTime)
        {
            return (int)Math.Min(255.0f, 255.0f * (float)changeTime.ElapsedMilliseconds / Math.Max(0, (float)FadeInTime));
        }

        int opacityLoss(Stopwatch changeTime)
        {
            int deltaTime = Math.Max(0, (int)changeTime.ElapsedMilliseconds - FadeOutDelay);
            return (int)Math.Min(255.0f, 255.0f * (float)deltaTime / Math.Max(0, (float)FadeOutTime));
        }

        public void Update()
        {
            Rectangle imageRectangleVisible = form.CanvasRectangle;
            imageRectangleVisible.Intersect(form.ClientArea);

            shouldDrawHorizontalScrollbar = form.ShapeManager.IsPanning && (form.CanvasRectangle.Left < form.ClientArea.Left || form.CanvasRectangle.Right > form.ClientArea.Right);

            if (shouldDrawHorizontalScrollbar ^ shouldDrawHorizontalScrollbarBefore)
            {
                horizontalScrollbarChangeTime = Stopwatch.StartNew();
                HorizontalScrollbarOpacityLast = HorizontalScrollbarOpacityCurrent;
            }
            shouldDrawHorizontalScrollbarBefore = shouldDrawHorizontalScrollbar;

            HorizontalScrollbarOpacityCurrent = shouldDrawHorizontalScrollbar
                ? HorizontalScrollbarOpacityLast + opacityGain(horizontalScrollbarChangeTime)
                : HorizontalScrollbarOpacityLast - opacityLoss(horizontalScrollbarChangeTime);
            HorizontalScrollbarOpacityCurrent = Math.Min(255, Math.Max(0, HorizontalScrollbarOpacityCurrent));

            IsHorizontalScrollbarVisible = HorizontalScrollbarOpacityCurrent > 0;

            if (IsHorizontalScrollbarVisible)
            {
                int horizontalTrackLength = form.ClientArea.Width - Margin * 2 - Thickness - Padding * 2;
                int horizontalThumbLength = Math.Max(Thickness, (int)Math.Round((float)imageRectangleVisible.Width / form.CanvasRectangle.Width * horizontalTrackLength));

                horizontalTrackRectangle = new Rectangle(new Point(Margin - Padding, form.ClientArea.Bottom - (Thickness + Margin) - Padding),
                    new Size(horizontalTrackLength + Padding * 2, Thickness + Padding * 2));

                double limitHorizontal = (horizontalTrackLength - horizontalThumbLength) / 2.0f;
                double thumbHorizontalPositionX = Math.Round(horizontalTrackRectangle.X + horizontalTrackRectangle.Width / 2.0f - (horizontalThumbLength / 2.0f) -
                    Math.Min(limitHorizontal, Math.Max(-limitHorizontal, form.CanvasCenterOffset.X / form.CanvasRectangle.Width * horizontalTrackLength)));

                horizontalThumbRectangle = new Rectangle(new Point((int)thumbHorizontalPositionX, form.ClientArea.Bottom - (Thickness + Margin)),
                    new Size(horizontalThumbLength, Thickness));
            }

            shouldDrawVerticalScrollbar = form.ShapeManager.IsPanning && (form.CanvasRectangle.Top < form.ClientArea.Top || form.CanvasRectangle.Bottom > form.ClientArea.Bottom);

            if (shouldDrawVerticalScrollbar ^ shouldDrawVerticalScrollbarBefore)
            {
                verticalScrollbarChangeTime = Stopwatch.StartNew();
                VerticalScrollbarOpacityLast = VerticalScrollbarOpacityCurrent;
            }
            shouldDrawVerticalScrollbarBefore = shouldDrawVerticalScrollbar;

            VerticalScrollbarOpacityCurrent = shouldDrawVerticalScrollbar
                ? VerticalScrollbarOpacityLast + opacityGain(verticalScrollbarChangeTime)
                : VerticalScrollbarOpacityLast - opacityLoss(verticalScrollbarChangeTime);
            VerticalScrollbarOpacityCurrent = Math.Min(255, Math.Max(0, VerticalScrollbarOpacityCurrent));

            IsVerticalScrollbarVisible = VerticalScrollbarOpacityCurrent > 0;
            if (IsVerticalScrollbarVisible)
            {
                int verticalTrackLength = form.ClientArea.Height - Margin * 2 - Thickness - Padding * 2;
                int verticalThumbLength = Math.Max(Thickness, (int)Math.Round((float)imageRectangleVisible.Height / form.CanvasRectangle.Height * verticalTrackLength));

                verticalTrackRectangle = new Rectangle(new Point(form.ClientArea.Right - (Thickness + Margin) - Padding, Margin - Padding),
                    new Size(Thickness + Padding * 2, verticalTrackLength + Padding * 2));

                double limitVertical = (verticalTrackLength - verticalThumbLength) / 2.0f;
                double thumbVerticalPositionY = Math.Round(verticalTrackRectangle.Y + verticalTrackRectangle.Height / 2.0f - (verticalThumbLength / 2.0f) -
                    Math.Min(limitVertical, Math.Max(-limitVertical, form.CanvasCenterOffset.Y / form.CanvasRectangle.Height * verticalTrackLength)));

                verticalThumbRectangle = new Rectangle(new Point(form.ClientArea.Right - (Thickness + Margin), (int)thumbVerticalPositionY),
                    new Size(Thickness, verticalThumbLength));
            }
        }

        public void Draw(Graphics g)
        {
            if (IsVisible)
            {
                if (IsHorizontalScrollbarVisible)
                {
                    using (Brush trackBrush = new SolidBrush(Color.FromArgb(HorizontalScrollbarOpacityCurrent, 60, 60, 60)))
                    using (Brush thumbBrush = new SolidBrush(Color.FromArgb(HorizontalScrollbarOpacityCurrent, 130, 130, 130)))
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.DrawCapsule(trackBrush, horizontalTrackRectangle);
                        g.DrawCapsule(thumbBrush, horizontalThumbRectangle);
                        g.SmoothingMode = SmoothingMode.None;
                    }
                }

                if (IsVerticalScrollbarVisible)
                {
                    using (Brush trackBrush = new SolidBrush(Color.FromArgb(VerticalScrollbarOpacityCurrent, 60, 60, 60)))
                    using (Brush thumbBrush = new SolidBrush(Color.FromArgb(VerticalScrollbarOpacityCurrent, 130, 130, 130)))
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.DrawCapsule(trackBrush, verticalTrackRectangle);
                        g.DrawCapsule(thumbBrush, verticalThumbRectangle);
                        g.SmoothingMode = SmoothingMode.None;
                    }
                }
            }
        }
    }
}