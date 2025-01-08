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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    [DefaultEvent("MouseClick")]
    public class BlackStyleButton : Control
    {
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public override string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }

                if (text != value)
                {
                    text = value;

                    Invalidate();
                }
            }
        }

        private string text;
        private bool isHover;
        private LinearGradientBrush backgroundBrush, backgroundHoverBrush, innerBorderBrush;
        private Pen innerBorderPen, borderPen;

        public BlackStyleButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            ForeColor = Color.White;
            Font = new Font("Arial", 12);
            borderPen = new Pen(Color.FromArgb(30, 30, 30));
            Prepare();
        }

        private void Prepare()
        {
            backgroundBrush = new LinearGradientBrush(new Rectangle(2, 2, ClientSize.Width - 4, ClientSize.Height - 4), Color.FromArgb(105, 105, 105), Color.FromArgb(65, 65, 65), LinearGradientMode.Vertical);
            backgroundHoverBrush = new LinearGradientBrush(new Rectangle(2, 2, ClientSize.Width - 4, ClientSize.Height - 4), Color.FromArgb(115, 115, 115), Color.FromArgb(75, 75, 75), LinearGradientMode.Vertical);
            innerBorderBrush = new LinearGradientBrush(new Rectangle(1, 1, ClientSize.Width - 2, ClientSize.Height - 2), Color.FromArgb(125, 125, 125), Color.FromArgb(75, 75, 75), LinearGradientMode.Vertical);
            innerBorderPen = new Pen(innerBorderBrush);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            DrawBackground(g);

            if (!string.IsNullOrEmpty(Text))
            {
                DrawText(g);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            isHover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            isHover = false;
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Prepare();
        }

        private void DrawBackground(Graphics g)
        {
            if (isHover)
            {
                g.FillRectangle(backgroundHoverBrush, new Rectangle(2, 2, ClientSize.Width - 4, ClientSize.Height - 4));
            }
            else
            {
                g.FillRectangle(backgroundBrush, new Rectangle(2, 2, ClientSize.Width - 4, ClientSize.Height - 4));
            }

            g.DrawRectangle(innerBorderPen, new Rectangle(1, 1, ClientSize.Width - 3, ClientSize.Height - 3));
            g.DrawRectangle(borderPen, new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1));
        }

        private void DrawText(Graphics g)
        {
            TextRenderer.DrawText(g, Text, Font, new Rectangle(ClientRectangle.X, ClientRectangle.Y + 1, ClientRectangle.Width, ClientRectangle.Height), Color.Black);
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor);
        }

        protected override void Dispose(bool disposing)
        {
            if (backgroundBrush != null) backgroundBrush.Dispose();
            if (backgroundHoverBrush != null) backgroundHoverBrush.Dispose();
            if (innerBorderBrush != null) innerBorderBrush.Dispose();
            if (innerBorderPen != null) innerBorderPen.Dispose();
            if (borderPen != null) borderPen.Dispose();

            base.Dispose(disposing);
        }
    }
}