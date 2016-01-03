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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    [DefaultEvent("MouseClick")]
    public class GreenlightButton : Control
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

                    Refresh();
                }
            }
        }

        private string text;
        private bool isHover;
        private LinearGradientBrush backgroundBrush, backgroundHoverBrush, borderBrush;
        private Pen borderPen;
        private bool ready;

        public GreenlightButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        }

        private void Prepare()
        {
            ForeColor = Color.White;
            backgroundBrush = new LinearGradientBrush(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), Color.FromArgb(121, 153, 5), Color.FromArgb(83, 105, 5),
                LinearGradientMode.Vertical);
            backgroundHoverBrush = new LinearGradientBrush(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), Color.FromArgb(140, 170, 5), Color.FromArgb(93, 115, 5),
                LinearGradientMode.Vertical);
            borderBrush = new LinearGradientBrush(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), Color.White, Color.Black, LinearGradientMode.Vertical);
            borderPen = new Pen(borderBrush);
            Font = new Font("Arial", 12);
            ready = true;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (ready)
            {
                Graphics g = pe.Graphics;

                DrawBackground(g);

                if (!string.IsNullOrEmpty(Text))
                {
                    DrawText(g);
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHover = true;
            Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHover = false;
            Refresh();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Prepare();
        }

        private void DrawBackground(Graphics g)
        {
            g.SetHighQuality();

            if (isHover)
            {
                g.DrawRoundedRectangle(backgroundHoverBrush, borderPen, new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1), 2);
            }
            else
            {
                g.DrawRoundedRectangle(backgroundBrush, borderPen, new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1), 2);
            }
        }

        private void DrawText(Graphics g)
        {
            TextRenderer.DrawText(g, Text, Font, new Rectangle(ClientRectangle.X, ClientRectangle.Y + 1, ClientRectangle.Width, ClientRectangle.Height + 1), Color.Black);
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor);
        }

        protected override void Dispose(bool disposing)
        {
            if (backgroundBrush != null) backgroundBrush.Dispose();
            if (backgroundHoverBrush != null) backgroundHoverBrush.Dispose();
            if (borderBrush != null) borderBrush.Dispose();
            if (borderPen != null) borderPen.Dispose();

            base.Dispose(disposing);
        }
    }
}