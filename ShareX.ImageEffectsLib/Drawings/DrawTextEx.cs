#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace ShareX.ImageEffectsLib
{
    [Description("Text")]
    public class DrawTextEx : ImageEffect
    {
        [DefaultValue("Text"), Editor(typeof(NameParserEditor), typeof(UITypeEditor))]
        public string Text { get; set; }

        [DefaultValue(ContentAlignment.TopLeft), TypeConverter(typeof(EnumProperNameConverter))]
        public ContentAlignment Placement { get; set; }

        [DefaultValue(typeof(Point), "0, 0")]
        public Point Offset { get; set; }

        [DefaultValue(true), Description("If text size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        private FontSafe fontSafe = new FontSafe();

        // Workaround for "System.AccessViolationException: Attempted to read or write protected memory. This is often an indication that other memory is corrupt."
        [DefaultValue(typeof(Font), "Arial, 36pt")]
        public Font Font
        {
            get
            {
                return fontSafe.GetFont();
            }
            set
            {
                using (value)
                {
                    fontSafe.SetFont(value);
                }
            }
        }

        [DefaultValue(typeof(Color), "235, 235, 235"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextColor { get; set; }

        [DefaultValue(false)]
        public bool TextUseGradient { get; set; }

        [Editor(typeof(GradientEditor), typeof(UITypeEditor))]
        public GradientInfo TextGradient { get; set; }

        [DefaultValue(false)]
        public bool DrawTextOutline { get; set; }

        [DefaultValue(5)]
        public int TextOutlineSize { get; set; }

        [DefaultValue(typeof(Color), "235, 0, 0"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextOutlineColor { get; set; }

        [DefaultValue(false)]
        public bool TextOutlineUseGradient { get; set; }

        [Editor(typeof(GradientEditor), typeof(UITypeEditor))]
        public GradientInfo TextOutlineGradient { get; set; }

        [DefaultValue(false)]
        public bool DrawTextShadow { get; set; }

        [DefaultValue(typeof(Point), "0, 5")]
        public Point TextShadowOffset { get; set; }

        [DefaultValue(typeof(Color), "125, 0, 0, 0"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextShadowColor { get; set; }

        [DefaultValue(false)]
        public bool TextShadowUseGradient { get; set; }

        [Editor(typeof(GradientEditor), typeof(UITypeEditor))]
        public GradientInfo TextShadowGradient { get; set; }

        public DrawTextEx()
        {
            this.ApplyDefaultPropertyValues();

            TextGradient = new GradientInfo();
            AddDefaultGradient(TextGradient);
            TextOutlineGradient = new GradientInfo();
            AddDefaultGradient(TextOutlineGradient);
            TextShadowGradient = new GradientInfo();
            AddDefaultGradient(TextShadowGradient);
        }

        private void AddDefaultGradient(GradientInfo gradientInfo)
        {
            gradientInfo.Type = LinearGradientMode.Horizontal;

            switch (RandomFast.Next(0, 2))
            {
                case 0:
                    gradientInfo.Colors.Add(new GradientStop(Color.FromArgb(0, 187, 138), 0f));
                    gradientInfo.Colors.Add(new GradientStop(Color.FromArgb(0, 105, 163), 100f));
                    break;
                case 1:
                    gradientInfo.Colors.Add(new GradientStop(Color.FromArgb(255, 3, 135), 0f));
                    gradientInfo.Colors.Add(new GradientStop(Color.FromArgb(255, 143, 3), 100f));
                    break;
                case 2:
                    gradientInfo.Colors.Add(new GradientStop(Color.FromArgb(184, 11, 195), 0f));
                    gradientInfo.Colors.Add(new GradientStop(Color.FromArgb(98, 54, 255), 100f));
                    break;
            }
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return bmp;
            }

            using (Font font = Font)
            {
                if (font == null || font.Size < 1)
                {
                    return bmp;
                }

                NameParser parser = new NameParser(NameParserType.Text);
                parser.ImageWidth = bmp.Width;
                parser.ImageHeight = bmp.Height;

                string parsedText = parser.Parse(Text);

                using (Graphics g = Graphics.FromImage(bmp))
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.FillMode = FillMode.Winding;
                    float emSize = g.DpiY * font.SizeInPoints / 72;
                    gp.AddString(parsedText, font.FontFamily, (int)font.Style, emSize, Point.Empty, StringFormat.GenericDefault);

                    RectangleF pathRect = gp.GetBounds();

                    if (pathRect.IsEmpty)
                    {
                        return bmp;
                    }

                    Size textSize = pathRect.Size.ToSize();
                    Point textPosition = Helpers.GetPosition(Placement, Offset, bmp.Size, textSize);
                    Rectangle textRectangle = new Rectangle(textPosition, textSize);

                    if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(textRectangle))
                    {
                        return bmp;
                    }

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.Half;

                    using (Matrix matrix = new Matrix())
                    {
                        matrix.Translate(textRectangle.X - pathRect.X, textRectangle.Y - pathRect.Y);
                        gp.Transform(matrix);
                    }

                    // Draw text shadow
                    if (DrawTextShadow && ((!TextShadowUseGradient && TextShadowColor.A > 0) || (TextShadowUseGradient && TextShadowGradient.IsVisible)))
                    {
                        using (Matrix matrix = new Matrix())
                        {
                            matrix.Translate(TextShadowOffset.X, TextShadowOffset.Y);
                            gp.Transform(matrix);

                            if (DrawTextOutline && TextOutlineSize > 0)
                            {
                                if (TextShadowUseGradient)
                                {
                                    using (LinearGradientBrush textShadowBrush = TextShadowGradient.GetGradientBrush(
                                        Rectangle.Round(textRectangle).Offset(TextOutlineSize + 1).LocationOffset(TextShadowOffset)))
                                    using (Pen textShadowPen = new Pen(textShadowBrush, TextOutlineSize) { LineJoin = LineJoin.Round })
                                    {
                                        g.DrawPath(textShadowPen, gp);
                                    }
                                }
                                else
                                {
                                    using (Pen textShadowPen = new Pen(TextShadowColor, TextOutlineSize) { LineJoin = LineJoin.Round })
                                    {
                                        g.DrawPath(textShadowPen, gp);
                                    }
                                }
                            }
                            else
                            {
                                if (TextShadowUseGradient)
                                {
                                    using (Brush textShadowBrush = TextShadowGradient.GetGradientBrush(
                                        Rectangle.Round(textRectangle).Offset(1).LocationOffset(TextShadowOffset)))
                                    {
                                        g.FillPath(textShadowBrush, gp);
                                    }
                                }
                                else
                                {
                                    using (Brush textShadowBrush = new SolidBrush(TextShadowColor))
                                    {
                                        g.FillPath(textShadowBrush, gp);
                                    }
                                }
                            }

                            matrix.Reset();
                            matrix.Translate(-TextShadowOffset.X, -TextShadowOffset.Y);
                            gp.Transform(matrix);
                        }
                    }

                    // Draw text outline
                    if (DrawTextOutline && TextOutlineSize > 0)
                    {
                        if (TextOutlineUseGradient)
                        {
                            if (TextOutlineGradient.IsVisible)
                            {
                                using (LinearGradientBrush textOutlineBrush = TextOutlineGradient.GetGradientBrush(Rectangle.Round(textRectangle).Offset(TextOutlineSize + 1)))
                                using (Pen textOutlinePen = new Pen(textOutlineBrush, TextOutlineSize) { LineJoin = LineJoin.Round })
                                {
                                    g.DrawPath(textOutlinePen, gp);
                                }
                            }
                        }
                        else if (TextOutlineColor.A > 0)
                        {
                            using (Pen textOutlinePen = new Pen(TextOutlineColor, TextOutlineSize) { LineJoin = LineJoin.Round })
                            {
                                g.DrawPath(textOutlinePen, gp);
                            }
                        }
                    }

                    // Draw text
                    if (TextUseGradient)
                    {
                        if (TextGradient.IsVisible)
                        {
                            using (Brush textBrush = TextGradient.GetGradientBrush(Rectangle.Round(textRectangle).Offset(1)))
                            {
                                g.FillPath(textBrush, gp);
                            }
                        }
                    }
                    else if (TextColor.A > 0)
                    {
                        using (Brush textBrush = new SolidBrush(TextColor))
                        {
                            g.FillPath(textBrush, gp);
                        }
                    }
                }

                return bmp;
            }
        }
    }
}