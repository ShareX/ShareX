#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal class ImageEditorScrollbar : ImageEditorControl
    {
        public Orientation Orientation { get; set; }
        public int Thickness { get; set; } = 15;
        public int Margin { get; set; } = 5;
        public int Padding { get; set; } = 1;
        public bool IsCapsule { get; set; } = true;
        public Color TrackColor { get; set; } = Color.FromArgb(49, 54, 66);
        public Color ThumbColor { get; set; } = Color.FromArgb(90, 94, 104);
        public Color ActiveThumbColor { get; set; } = Color.FromArgb(111, 115, 123);
        public bool AutoHide { get; set; } = true;
        public RectangleF ThumbRectangle { get; private set; }

        private RegionCaptureForm form;

        public ImageEditorScrollbar(Orientation orientation, RegionCaptureForm form)
        {
            Orientation = orientation;
            this.form = form;
        }

        public void Update()
        {
            if (AutoHide)
            {
                RectangleF imageRectangle = form.CanvasRectangle.Scale(form.ZoomFactor);
                bool isScrollbarNeeded;

                if (Orientation == Orientation.Horizontal)
                {
                    isScrollbarNeeded = imageRectangle.Left < form.ClientArea.Left ||
                        (imageRectangle.Right * form.ZoomFactor) > form.ClientArea.Right;
                }
                else
                {
                    isScrollbarNeeded = imageRectangle.Top < form.ClientArea.Top ||
                        (imageRectangle.Bottom * form.ZoomFactor) > form.ClientArea.Bottom;
                }

                Visible = isScrollbarNeeded || IsDragging;
            }
            else
            {
                Visible = true;
            }

            if (Visible)
            {
                if (IsDragging)
                {
                    Scroll(form.ShapeManager.InputManager.ClientMousePosition);
                }

                RectangleF imageRectangle = form.CanvasRectangle.Scale(form.ZoomFactor);
                RectangleF imageRectangleVisible = imageRectangle;
                imageRectangleVisible.Intersect(form.ClientArea);

                float inClientAreaSize, inImageVisibleSize, inImageSize, sideOffsetBase;
                float inCanvasCenterOffset;

                if (Orientation == Orientation.Horizontal)
                {
                    inClientAreaSize = form.ClientArea.Width;
                    inImageVisibleSize = imageRectangleVisible.Width;
                    inImageSize = imageRectangle.Width;
                    sideOffsetBase = form.ClientArea.Bottom;
                    inCanvasCenterOffset = form.CanvasCenterOffset.X;
                }
                else
                {
                    inClientAreaSize = form.ClientArea.Height;
                    inImageVisibleSize = imageRectangleVisible.Height;
                    inImageSize = imageRectangle.Height;
                    sideOffsetBase = form.ClientArea.Right;
                    inCanvasCenterOffset = form.CanvasCenterOffset.Y;
                }

                float trackLength = inClientAreaSize - (Margin * 2) - (Padding * 2) - Thickness;
                float trackLengthInternal = trackLength - (Padding * 2);

                int thumbLength = Math.Max(Thickness, (int)Math.Round((float)inImageVisibleSize / inImageSize * trackLengthInternal));
                double thumbLimit = (trackLengthInternal - thumbLength) / 2.0f;
                int thumbPosition = (int)Math.Round(Margin + (trackLength / 2.0f) - (thumbLength / 2.0f) -
                    Math.Min(thumbLimit, Math.Max(-thumbLimit, inCanvasCenterOffset / inImageSize * trackLengthInternal)));

                int trackWidth = (Padding * 2) + Thickness;
                float thumbSideOffset = sideOffsetBase - Margin - Padding - Thickness;
                float trackSideOffset = thumbSideOffset - Padding;

                if (Orientation == Orientation.Horizontal)
                {
                    Rectangle = new RectangleF(Margin, trackSideOffset, trackLength, trackWidth);
                    ThumbRectangle = new RectangleF(thumbPosition, thumbSideOffset, thumbLength, Thickness);
                }
                else
                {
                    Rectangle = new RectangleF(trackSideOffset, Margin, trackWidth, trackLength);
                    ThumbRectangle = new RectangleF(thumbSideOffset, thumbPosition, Thickness, thumbLength);
                }
            }
        }

        public override void OnDraw(Graphics g)
        {
            Color thumbColor;

            if (IsDragging || form.ShapeManager.IsPanning || IsCursorHover)
            {
                thumbColor = ActiveThumbColor;
            }
            else
            {
                thumbColor = ThumbColor;
            }

            using (Brush trackBrush = new SolidBrush(TrackColor))
            using (Brush thumbBrush = new SolidBrush(thumbColor))
            {
                Matrix savedTransform = g.Transform;
                form.ZoomTransform(g, true);

                if (IsCapsule)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.Half;

                    g.DrawCapsule(trackBrush, Rectangle);
                    g.DrawCapsule(thumbBrush, ThumbRectangle);

                    g.SmoothingMode = SmoothingMode.None;
                    g.PixelOffsetMode = PixelOffsetMode.Default;
                }
                else
                {
                    g.FillRectangle(trackBrush, Rectangle);
                    g.FillRectangle(thumbBrush, ThumbRectangle);
                }

                g.Transform = savedTransform;
            }
        }

        private void Scroll(Point position)
        {
            RectangleF imageRectangle = form.CanvasRectangle.Scale(form.ZoomFactor);
            float inMousePosition, inClientAreaSize, inImageSize;

            if (Orientation == Orientation.Horizontal)
            {
                inMousePosition = position.X;
                inClientAreaSize = form.ClientArea.Width;
                inImageSize = imageRectangle.Width;
            }
            else
            {
                inMousePosition = position.Y;
                inClientAreaSize = form.ClientArea.Height;
                inImageSize = imageRectangle.Height;
            }

            float mousePositionLocal = inMousePosition - Margin - Padding;

            float trackLength = inClientAreaSize - (Margin * 2) - (Padding * 2) - Thickness;
            float trackLengthInternal = trackLength - (Padding * 2);

            float centerOffsetNew = ((trackLengthInternal / 2.0f) - mousePositionLocal) / trackLengthInternal * inImageSize;

            Vector2 canvasCenterOffset = Orientation == Orientation.Horizontal ?
                new Vector2(centerOffsetNew, form.CanvasCenterOffset.Y) :
                new Vector2(form.CanvasCenterOffset.X, centerOffsetNew);
            form.PanToOffset(canvasCenterOffset);
        }
    }
}