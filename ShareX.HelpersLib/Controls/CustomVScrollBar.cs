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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class CustomVScrollBar : Control
    {
        public event EventHandler ValueChanged;

        private int minimum;

        public int Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                minimum = value;
                Invalidate();
            }
        }

        private int maximum;

        public int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;
                Invalidate();
            }
        }

        private int currentValue;

        public int Value
        {
            get
            {
                return currentValue;
            }
            set
            {
                int newValue = Math.Max(Minimum, Math.Min(value, Maximum));

                if (currentValue != newValue)
                {
                    currentValue = newValue;
                    Invalidate();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private int pageSize;

        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
                Invalidate();
            }
        }

        public int SmallScrollStep { get; set; } = 20;
        public int LargeScrollStep { get; set; } = 100;

        private bool isDragging;
        private bool isThumbHovered;
        private int dragOffset = 0;

        public CustomVScrollBar()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        private Rectangle GetThumbRectangle()
        {
            int trackHeight = ClientRectangle.Height;
            int thumbHeight = (int)((float)trackHeight * PageSize / (Maximum + PageSize));
            thumbHeight = Math.Max(thumbHeight, 20);

            int movementRange = trackHeight - thumbHeight;
            int thumbTop = Maximum > 0 ? (int)((float)Value / Maximum * movementRange) : 0;

            return new Rectangle(0, thumbTop, ClientRectangle.Width, thumbHeight);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (SolidBrush trackBrush = new SolidBrush(ShareXResources.Theme.DarkBackgroundColor))
            {
                e.Graphics.FillRectangle(trackBrush, ClientRectangle);
            }

            int effectivePageSize = Math.Max(PageSize, 1);
            int effectiveMaximum = Math.Max(Maximum, 0);
            int sum = Math.Max(effectiveMaximum + effectivePageSize, 1);

            int thumbHeight = (int)((float)ClientRectangle.Height * effectivePageSize / sum);
            thumbHeight = Math.Max(thumbHeight, 20);

            int movementRange = Math.Max(ClientRectangle.Height - thumbHeight, 0);

            int thumbTop = effectiveMaximum > 0 ? movementRange * Value / effectiveMaximum : 0;
            Rectangle thumbRect = new Rectangle(0, thumbTop, ClientRectangle.Width, thumbHeight);

            Color thumbColor = isThumbHovered ? ColorHelpers.LighterColor(ShareXResources.Theme.LightBackgroundColor, 0.1f) : ShareXResources.Theme.LightBackgroundColor;
            using (SolidBrush thumbBrush = new SolidBrush(thumbColor))
            {
                e.Graphics.FillRectangle(thumbBrush, thumbRect);
            }

            using (Pen borderPen = new Pen(ColorHelpers.DarkerColor(ShareXResources.Theme.DarkBackgroundColor, 0.03f)))
            {
                e.Graphics.DrawLine(borderPen, thumbRect.X, thumbRect.Y, thumbRect.Right - 1, thumbRect.Y);
                e.Graphics.DrawLine(borderPen, thumbRect.X, thumbRect.Bottom - 1, thumbRect.Right - 1, thumbRect.Bottom - 1);
                e.Graphics.DrawLine(borderPen, ClientRectangle.X, ClientRectangle.Y, 0, ClientRectangle.Height);
                e.Graphics.DrawLine(borderPen, ClientRectangle.Right - 1, ClientRectangle.Y, ClientRectangle.Right - 1, ClientRectangle.Bottom - 1);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int thumbHeight = (int)((float)ClientRectangle.Height * PageSize / (Maximum + PageSize));
            thumbHeight = Math.Max(thumbHeight, 20);
            int movementRange = ClientRectangle.Height - thumbHeight;
            int thumbTop = movementRange * Value / Maximum;
            Rectangle thumbRect = new Rectangle(0, thumbTop, ClientRectangle.Width, thumbHeight);

            if (thumbRect.Contains(e.Location))
            {
                isDragging = true;
                dragOffset = e.Y - thumbTop;
            }
            else
            {
                int clickPosition = e.Y;

                if (clickPosition < thumbTop)
                {
                    Value = Math.Max(Minimum, Value - PageSize);
                }
                else if (clickPosition > thumbTop + thumbHeight)
                {
                    Value = Math.Min(Maximum, Value + PageSize);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
            {
                Rectangle thumbRect = GetThumbRectangle();

                int movementRange = ClientRectangle.Height - thumbRect.Height;
                int newThumbTop = e.Y - dragOffset;
                newThumbTop = Math.Max(0, Math.Min(newThumbTop, movementRange));

                Value = Maximum > 0 ? newThumbTop * Maximum / movementRange : 0;
            }
            else
            {
                Rectangle thumbRect = GetThumbRectangle();
                bool hovered = thumbRect.Contains(e.Location);

                if (isThumbHovered != hovered)
                {
                    isThumbHovered = hovered;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isDragging = false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (isThumbHovered)
            {
                isThumbHovered = false;
                Invalidate();
            }
        }
    }
}