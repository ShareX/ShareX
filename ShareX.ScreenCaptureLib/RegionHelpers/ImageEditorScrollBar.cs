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
        public int Thickness { get; set; } = 10;
        public int Margin { get; set; } = 15;
        public int Padding { get; set; } = 2;
        public float Opacity { get; set; }
        public Rectangle ThumbRectangle { get; set; }

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

                if (Orientation == Orientation.Horizontal)
                {
                    int horizontalTrackLength = form.ClientArea.Width - Margin * 2 - Padding * 2 - Thickness;

                    Rectangle = new Rectangle(Margin, form.ClientArea.Bottom - Margin - Padding * 2 - Thickness - 1, horizontalTrackLength, Padding * 2 + Thickness);

                    int horizontalThumbLength = Math.Max(Thickness, (int)Math.Round((float)imageRectangleVisible.Width /
                        form.CanvasRectangle.Width * (horizontalTrackLength - Padding * 2)));

                    double limitHorizontal = (horizontalTrackLength - horizontalThumbLength) / 2.0f;
                    double thumbHorizontalPositionX = Math.Round(Rectangle.X + Rectangle.Width / 2.0f - (horizontalThumbLength / 2.0f) + Padding -
                        Math.Min(limitHorizontal, Math.Max(-limitHorizontal, form.CanvasCenterOffset.X / form.CanvasRectangle.Width * (horizontalTrackLength - Padding * 2))));

                    ThumbRectangle = new Rectangle((int)thumbHorizontalPositionX, form.ClientArea.Bottom - Margin - Padding - Thickness - 1, horizontalThumbLength, Thickness);
                }
                else
                {
                    int verticalTrackLength = form.ClientArea.Height - Margin * 2 - Padding * 2 - Thickness;

                    Rectangle = new Rectangle(form.ClientArea.Right - Margin - Padding * 2 - Thickness - 1, Margin, Padding * 2 + Thickness, verticalTrackLength);

                    int verticalThumbLength = Math.Max(Thickness, (int)Math.Round((float)imageRectangleVisible.Height /
                        form.CanvasRectangle.Height * (verticalTrackLength - Padding * 2)));

                    double limitVertical = (verticalTrackLength - verticalThumbLength) / 2.0f;
                    double thumbVerticalPositionY = Math.Round(Rectangle.Y + Rectangle.Height / 2.0f - (verticalThumbLength / 2.0f) + Padding -
                        Math.Min(limitVertical, Math.Max(-limitVertical, form.CanvasCenterOffset.Y / form.CanvasRectangle.Height * verticalTrackLength)));

                    ThumbRectangle = new Rectangle(form.ClientArea.Right - Margin - Padding - Thickness - 1, (int)thumbVerticalPositionY, Thickness, verticalThumbLength);
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
            if (!Visible) return;

            using (Brush trackBrush = new SolidBrush(Color.FromArgb((int)(255 * Opacity), 60, 60, 60)))
            using (Brush thumbBrush = new SolidBrush(Color.FromArgb((int)(255 * Opacity), 130, 130, 130)))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawCapsule(trackBrush, Rectangle);
                g.DrawCapsule(thumbBrush, ThumbRectangle);
                g.SmoothingMode = SmoothingMode.None;
            }
        }

        public override void OnMouseDown(Point position)
        {
            base.OnMouseDown(position);

            // Pan here
        }
    }
}