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
        private ContentAlignment textAlign;
        private Color textShadowColor;
        private bool drawBorder;
        private bool autoEllipsis;
        private bool wordWrap = true;

        public ThemedLabel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            this.TextAlign = ContentAlignment.TopLeft;
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.White;
            this.TextShadowColor = Color.Black;
            this.Font = new Font("Arial", 12);
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [SettingsBindable(true)]
        public override string Text
        {
            get => this.text;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (this.text != value)
                {
                    this.text = value;

                    this.OnTextChanged(EventArgs.Empty);

                    this.Invalidate();
                }
            }
        }

        [DefaultValue(ContentAlignment.TopLeft)]
        public ContentAlignment TextAlign
        {
            get => this.textAlign;
            set
            {
                this.textAlign = value;

                this.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color TextShadowColor
        {
            get => this.textShadowColor;
            set
            {
                this.textShadowColor = value;

                this.Invalidate();
            }
        }

        [DefaultValue(false)]
        public bool DrawBorder
        {
            get => this.drawBorder;

            set
            {
                this.drawBorder = value;

                this.Invalidate();
            }
        }

        [DefaultValue(false)]
        public bool AutoEllipsis
        {
            get => this.autoEllipsis;
            set
            {
                this.autoEllipsis = value;

                this.Invalidate();
            }
        }

        [DefaultValue(true)]
        public bool WordWrap
        {
            get => this.wordWrap;
            set
            {
                this.wordWrap = value;

                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            var g = pe.Graphics;

            if (!string.IsNullOrEmpty(this.Text))
            {
                this.DrawText(g);

                if (this.drawBorder)
                {
                    g.DrawRectangle(Pens.Black, this.ClientRectangle);
                }
            }
        }

        private void DrawText(Graphics g)
        {
            TextFormatFlags tff;

            switch (this.TextAlign)
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
                // on my program if this is not added; my program will fail compile on c# 8.x.
                default:
                    tff = TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
            }

            if (this.AutoEllipsis)
            {
                tff |= TextFormatFlags.EndEllipsis;
            }

            if (this.WordWrap)
            {
                tff |= TextFormatFlags.WordBreak;
            }

            if (this.TextShadowColor.A > 0)
            {
                TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle.LocationOffset(0, 1), this.TextShadowColor, tff);
            }

            TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle, this.ForeColor, tff);
        }
    }
}
