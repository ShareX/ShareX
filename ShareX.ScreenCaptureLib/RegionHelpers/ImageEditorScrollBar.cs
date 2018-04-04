#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
    internal class ImageEditorScrollbar : DrawableObject
    {
        public Orientation Orientation { get; set; }
        public int Thickness { get; set; } = 15; //10;
        public int Margin { get; set; } = 0; //15;
        public int Padding { get; set; } = 1; //2;
        public bool IsCapsule { get; set; } = false; //true;
        public Color TrackColor { get; set; } = Color.FromArgb(255, 0, 0); //60, 60, 60);
        public Color ThumbColor { get; set; } = Color.FromArgb(255, 255, 255); //130, 130, 130);
        public float Opacity { get; private set; }
        public Rectangle ThumbRectangle { get; private set; }

        private RegionCaptureForm form;

        public ImageEditorScrollbar(Orientation orientation, RegionCaptureForm form)
        {
            Orientation = orientation;
            this.form = form;
        }

        public void Update()
        {
            UpdateOpacity();

            if (Visible)
            {
                Rectangle imageRectangleVisible = form.CanvasRectangle;
                imageRectangleVisible.Intersect(form.ClientArea);

                int inClientAreaSize, inImageVisibleSize, inImageSize, sideOffsetBase;
                float inCanvasCenterOffset;

                if (Orientation == Orientation.Horizontal)
                {
                    inClientAreaSize = form.ClientArea.Width;
                    inImageVisibleSize = imageRectangleVisible.Width;
                    inImageSize = form.CanvasRectangle.Width;
                    sideOffsetBase = form.ClientArea.Bottom;
                    inCanvasCenterOffset = form.CanvasCenterOffset.X;
                }
                else
                {
                    inClientAreaSize = form.ClientArea.Height;
                    inImageVisibleSize = imageRectangleVisible.Height;
                    inImageSize = form.CanvasRectangle.Height;
                    sideOffsetBase = form.ClientArea.Right;
                    inCanvasCenterOffset = form.CanvasCenterOffset.Y;
                }

                int trackLength = inClientAreaSize - Margin * 2 - Padding * 2 - Thickness;
                int trackLengthInternal = trackLength - Padding * 2;

                int thumbLength = Math.Max(Thickness, (int)Math.Round((float)inImageVisibleSize / inImageSize * trackLengthInternal));
                double thumbLimit = (trackLengthInternal - thumbLength) / 2.0f;
                int thumbPosition = (int)Math.Round(Margin + trackLength / 2.0f - (thumbLength / 2.0f) -
                        Math.Min(thumbLimit, Math.Max(-thumbLimit, inCanvasCenterOffset / inImageSize * trackLengthInternal)));

                int trackWidth = Padding * 2 + Thickness;
                int trackSideOffset = sideOffsetBase - Margin - Thickness - 1 - Padding * 2;
                int thumbSideOffset = sideOffsetBase - Margin - Thickness - 1 - Padding;

                if (Orientation == Orientation.Horizontal)
                {
                    Rectangle = new Rectangle(Margin, trackSideOffset, trackLength, trackWidth);
                    ThumbRectangle = new Rectangle(thumbPosition, thumbSideOffset, thumbLength, Thickness);
                }
                else
                {
                    Rectangle = new Rectangle(trackSideOffset, Margin, trackWidth, trackLength);
                    ThumbRectangle = new Rectangle(thumbSideOffset, thumbPosition, Thickness, thumbLength);
                }
            }
        }

        private void UpdateOpacity()
        {
            bool isScrollbarNeeded;

            if (Orientation == Orientation.Horizontal)
            {
                isScrollbarNeeded = form.CanvasRectangle.Left < form.ClientArea.Left || form.CanvasRectangle.Right > form.ClientArea.Right;
            }
            else
            {
                isScrollbarNeeded = form.CanvasRectangle.Top < form.ClientArea.Top || form.CanvasRectangle.Bottom > form.ClientArea.Bottom;
            }

            if (!isScrollbarNeeded)
            {
                Opacity = 0f;
            }
            else if (form.ShapeManager.IsPanning || IsCursorHover)
            {
                Opacity = 1f;
            }
            else
            {
                Opacity = 0.8f;
            }

            Visible = Opacity > 0;
        }

        public override void OnDraw(Graphics g)
        {
            if (Visible)
            {
                using (Brush trackBrush = new SolidBrush(Color.FromArgb((int)(255 * Opacity), TrackColor)))
                using (Brush thumbBrush = new SolidBrush(Color.FromArgb((int)(255 * Opacity), ThumbColor)))
                {
                    if (IsCapsule)
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.DrawCapsule(trackBrush, Rectangle);
                        g.DrawCapsule(thumbBrush, ThumbRectangle);
                        g.SmoothingMode = SmoothingMode.None;
                    }
                    else
                    {
                        g.FillRectangle(trackBrush, Rectangle);
                        g.FillRectangle(thumbBrush, ThumbRectangle);
                    }
                }
            }
        }

        public override void OnMouseDown(Point position)
        {
            base.OnMouseDown(position);

            // Pan here
        }
    }
}