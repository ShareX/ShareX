#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class BlackStyleLabel : Control
    {
        private string text;

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [SettingsBindable(true)]
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
                    OnTextChanged(EventArgs.Empty);
                }
            }
        }

        private ContentAlignment textAlign;

        [DefaultValue(ContentAlignment.TopLeft)]
        public ContentAlignment TextAlign
        {
            get
            {
                return textAlign;
            }
            set
            {
                textAlign = value;

                Invalidate();
            }
        }

        private Color textShadowColor;

        [DefaultValue(typeof(Color), "Black")]
        public Color TextShadowColor
        {
            get
            {
                return textShadowColor;
            }
            set
            {
                textShadowColor = value;

                Invalidate();
            }
        }

        private bool drawBorder;

        [DefaultValue(false)]
        public bool DrawBorder
        {
            get
            {
                return drawBorder;
            }
            set
            {
                drawBorder = value;

                Invalidate();
            }
        }

        private bool autoEllipsis;

        [DefaultValue(false)]
        public bool AutoEllipsis
        {
            get
            {
                return autoEllipsis;
            }
            set
            {
                autoEllipsis = value;

                Invalidate();
            }
        }

        public BlackStyleLabel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            TextAlign = ContentAlignment.TopLeft;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            TextShadowColor = Color.Black;
            Font = new Font("Arial", 12);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            if (!string.IsNullOrEmpty(Text))
            {
                DrawText(g);

                if (drawBorder)
                {
                    g.DrawRectangleProper(Pens.Black, ClientRectangle);
                }
            }
        }

        private void DrawText(Graphics g)
        {
            TextFormatFlags tff;

            switch (TextAlign)
            {
                case ContentAlignment.TopLeft:
                    tff = TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
                case ContentAlignment.MiddleLeft:
                    tff = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case ContentAlignment.BottomLeft:
                    tff = TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopCenter:
                    tff = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                default:
                case ContentAlignment.MiddleCenter:
                    tff = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.BottomCenter:
                    tff = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopRight:
                    tff = TextFormatFlags.Top | TextFormatFlags.Right;
                    break;
                case ContentAlignment.MiddleRight:
                    tff = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;
                case ContentAlignment.BottomRight:
                    tff = TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;
            }

            if (AutoEllipsis)
            {
                tff |= TextFormatFlags.EndEllipsis;
            }

            TextRenderer.DrawText(g, Text, Font, new Rectangle(ClientRectangle.X, ClientRectangle.Y + 1, ClientRectangle.Width, ClientRectangle.Height + 1), TextShadowColor, tff);
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor, tff);
        }
    }
}