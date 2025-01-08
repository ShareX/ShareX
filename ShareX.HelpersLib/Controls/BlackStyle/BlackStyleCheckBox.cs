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
    [DefaultEvent("CheckedChanged")]
    public class BlackStyleCheckBox : Control
    {
        [DefaultValue(false)]
        public bool Checked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;

                    OnCheckedChanged(EventArgs.Empty);

                    Invalidate();
                }
            }
        }

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

        [DefaultValue(3)]
        public int SpaceAfterCheckBox { get; set; }

        [DefaultValue(false)]
        public bool IgnoreClick { get; set; }

        private bool isChecked, isHover;
        private string text;

        private LinearGradientBrush backgroundBrush, backgroundCheckedBrush, innerBorderBrush, innerBorderCheckedBrush;
        private Pen innerBorderPen, innerBorderCheckedPen, borderPen;

        private const int checkBoxSize = 13;

        public event EventHandler CheckedChanged;

        public BlackStyleCheckBox()
        {
            SpaceAfterCheckBox = 3;

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            ForeColor = Color.White;

            // http://connect.microsoft.com/VisualStudio/feedback/details/348321/bug-in-fillrectangle-using-lineargradientbrush
            backgroundBrush = new LinearGradientBrush(new Rectangle(2, 2, checkBoxSize - 4, checkBoxSize - 3), Color.FromArgb(105, 105, 105), Color.FromArgb(55, 55, 55), LinearGradientMode.Vertical);

            innerBorderBrush = new LinearGradientBrush(new Rectangle(1, 1, checkBoxSize - 2, checkBoxSize - 2), Color.FromArgb(125, 125, 125), Color.FromArgb(65, 75, 75), LinearGradientMode.Vertical);
            innerBorderPen = new Pen(innerBorderBrush);

            backgroundCheckedBrush = new LinearGradientBrush(new Rectangle(2, 2, checkBoxSize - 4, checkBoxSize - 3), Color.Black, Color.Black, LinearGradientMode.Vertical);
            ColorBlend cb = new ColorBlend();
            cb.Positions = new float[] { 0, 0.49f, 0.50f, 1 };
            cb.Colors = new Color[] { Color.FromArgb(102, 163, 226), Color.FromArgb(83, 135, 186), Color.FromArgb(75, 121, 175), Color.FromArgb(56, 93, 135) };
            backgroundCheckedBrush.InterpolationColors = cb;

            innerBorderCheckedBrush = new LinearGradientBrush(new Rectangle(1, 1, checkBoxSize - 2, checkBoxSize - 2), Color.FromArgb(133, 192, 241), Color.FromArgb(76, 119, 163), LinearGradientMode.Vertical);
            innerBorderCheckedPen = new Pen(innerBorderCheckedBrush);

            borderPen = new Pen(Color.FromArgb(30, 30, 30));

            Font = new Font("Arial", 8);
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

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (!IgnoreClick)
            {
                Checked = !Checked;
            }
        }

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            CheckedChanged?.Invoke(this, e);
        }

        private void DrawBackground(Graphics g)
        {
            if (Checked)
            {
                g.FillRectangle(backgroundCheckedBrush, new Rectangle(2, 2, checkBoxSize - 4, checkBoxSize - 4));
                g.DrawRectangle(innerBorderCheckedPen, new Rectangle(1, 1, checkBoxSize - 3, checkBoxSize - 3));
            }
            else
            {
                g.FillRectangle(backgroundBrush, new Rectangle(2, 2, checkBoxSize - 4, checkBoxSize - 4));

                if (isHover)
                {
                    g.DrawRectangle(innerBorderCheckedPen, new Rectangle(1, 1, checkBoxSize - 3, checkBoxSize - 3));
                }
                else
                {
                    g.DrawRectangle(innerBorderPen, new Rectangle(1, 1, checkBoxSize - 3, checkBoxSize - 3));
                }
            }

            g.DrawRectangle(borderPen, new Rectangle(0, 0, checkBoxSize - 1, checkBoxSize - 1));
        }

        private void DrawText(Graphics g)
        {
            Rectangle rect = new Rectangle(checkBoxSize + SpaceAfterCheckBox, 0, ClientRectangle.Width - checkBoxSize + SpaceAfterCheckBox, ClientRectangle.Height);
            TextFormatFlags tff = TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak;
            TextRenderer.DrawText(g, Text, Font, new Rectangle(rect.X, rect.Y + 1, rect.Width, rect.Height + 1), Color.Black, tff);
            TextRenderer.DrawText(g, Text, Font, rect, ForeColor, tff);
        }

        protected override void Dispose(bool disposing)
        {
            if (backgroundBrush != null) backgroundBrush.Dispose();
            if (backgroundCheckedBrush != null) backgroundCheckedBrush.Dispose();
            if (innerBorderBrush != null) innerBorderBrush.Dispose();
            if (innerBorderPen != null) innerBorderPen.Dispose();
            if (innerBorderCheckedBrush != null) innerBorderCheckedBrush.Dispose();
            if (innerBorderCheckedPen != null) innerBorderCheckedPen.Dispose();
            if (borderPen != null) borderPen.Dispose();

            base.Dispose(disposing);
        }
    }
}