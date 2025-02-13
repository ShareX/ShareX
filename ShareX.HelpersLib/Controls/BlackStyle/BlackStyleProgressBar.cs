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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class BlackStyleProgressBar : Control
    {
        [DefaultValue(0)]
        public int Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                if (minimum != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Minimum));
                    }

                    if (maximum < value)
                    {
                        maximum = value;
                    }

                    minimum = value;

                    if (this.value < minimum)
                    {
                        this.value = minimum;
                    }

                    Invalidate();
                }
            }
        }

        private int minimum;

        [DefaultValue(100)]
        public int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                if (maximum != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Maximum));
                    }

                    if (minimum > value)
                    {
                        minimum = value;
                    }

                    maximum = value;

                    if (this.value > maximum)
                    {
                        this.value = maximum;
                    }

                    Invalidate();
                }
            }
        }

        private int maximum;

        [DefaultValue(0)]
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value != value)
                {
                    if (value < minimum || value > maximum)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Value));
                    }

                    this.value = value;

                    Invalidate();
                }
            }
        }

        private int value;

        [DefaultValue(false)]
        public bool ShowPercentageText
        {
            get
            {
                return showPercentageText;
            }
            set
            {
                if (showPercentageText != value)
                {
                    showPercentageText = value;

                    Invalidate();
                }
            }
        }

        private bool showPercentageText;

        public override string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;

                    Invalidate();
                }
            }
        }

        private string text;

        public BlackStyleProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            minimum = 0;
            maximum = 100;
            value = 0;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            DrawBackground(g);

            if (Value > Minimum && Value <= Maximum)
            {
                DrawProgressBar(g);

                if (!string.IsNullOrEmpty(Text))
                {
                    DrawText(g, Text);
                }
                else if (ShowPercentageText)
                {
                    DrawText(g, Value + "%");
                }
            }
        }

        private void DrawBackground(Graphics g)
        {
            using (LinearGradientBrush backgroundBrush = new LinearGradientBrush(new Rectangle(1, 1, ClientSize.Width - 2, ClientSize.Height - 2),
                Color.FromArgb(50, 50, 50), Color.FromArgb(60, 60, 60), LinearGradientMode.Vertical))
            {
                g.FillRectangle(backgroundBrush, new Rectangle(1, 1, ClientSize.Width - 2, ClientSize.Height - 2));
            }

            using (Pen borderShadowPen = new Pen(Color.FromArgb(45, 45, 45)))
            {
                g.DrawLine(borderShadowPen, new Point(1, 1), new Point(ClientSize.Width - 2, 1));
                g.DrawLine(borderShadowPen, new Point(ClientSize.Width - 2, 1), new Point(ClientSize.Width - 2, ClientSize.Height - 2));
            }

            using (Pen borderPen = new Pen(Color.FromArgb(30, 30, 30)))
            {
                g.DrawRectangle(borderPen, new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1));
            }
        }

        private void DrawProgressBar(Graphics g)
        {
            double progressBarSize = (double)Value / Maximum * (ClientSize.Width - 2);
            Rectangle progressBarRect = new Rectangle(1, 1, (int)progressBarSize, ClientSize.Height - 2);

            using (LinearGradientBrush progressBarBrush = new LinearGradientBrush(progressBarRect, Color.Black, Color.Black, LinearGradientMode.Vertical))
            {
                ColorBlend cb = new ColorBlend();
                cb.Positions = new float[] { 0, 0.49f, 0.50f, 1 };
                cb.Colors = new Color[] { Color.FromArgb(102, 163, 226), Color.FromArgb(83, 135, 186), Color.FromArgb(75, 121, 175), Color.FromArgb(56, 93, 135) };
                progressBarBrush.InterpolationColors = cb;

                g.FillRectangle(progressBarBrush, progressBarRect);
            }

            using (LinearGradientBrush innerBorderBrush = new LinearGradientBrush(progressBarRect, Color.FromArgb(133, 192, 241), Color.FromArgb(76, 119, 163), LinearGradientMode.Vertical))
            using (Pen innerBorderPen = new Pen(innerBorderBrush))
            {
                g.DrawRectangle(innerBorderPen, new Rectangle(progressBarRect.X, progressBarRect.Y, progressBarRect.Width - 1, progressBarRect.Height - 1));
            }
        }

        private void DrawText(Graphics g, string text)
        {
            TextRenderer.DrawText(g, text, Font, ClientRectangle.LocationOffset(0, 1), Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(g, text, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}