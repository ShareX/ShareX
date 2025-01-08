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

        [DefaultValue(0)]
        public int Angle { get; set; }

        [DefaultValue(false), Description("If text size bigger than source image then don't draw it.")]
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
        public Color Color { get; set; }

        [DefaultValue(false)]
        public bool UseGradient { get; set; }

        [Editor(typeof(GradientEditor), typeof(UITypeEditor))]
        public GradientInfo Gradient { get; set; }

        [DefaultValue(false)]
        public bool Outline { get; set; }

        [DefaultValue(5)]
        public int OutlineSize { get; set; }

        [DefaultValue(typeof(Color), "235, 0, 0"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color OutlineColor { get; set; }

        [DefaultValue(false)]
        public bool OutlineUseGradient { get; set; }

        [Editor(typeof(GradientEditor), typeof(UITypeEditor))]
        public GradientInfo OutlineGradient { get; set; }

        [DefaultValue(false)]
        public bool Shadow { get; set; }

        [DefaultValue(typeof(Point), "0, 5")]
        public Point ShadowOffset { get; set; }

        [DefaultValue(typeof(Color), "125, 0, 0, 0"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color ShadowColor { get; set; }

        [DefaultValue(false)]
        public bool ShadowUseGradient { get; set; }

        [Editor(typeof(GradientEditor), typeof(UITypeEditor))]
        public GradientInfo ShadowGradient { get; set; }

        public DrawTextEx()
        {
            this.ApplyDefaultPropertyValues();
            Gradient = AddDefaultGradient();
            OutlineGradient = AddDefaultGradient();
            ShadowGradient = AddDefaultGradient();
        }

        private GradientInfo AddDefaultGradient()
        {
            GradientInfo gradientInfo = new GradientInfo();
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

            return gradientInfo;
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
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.Half;

                    gp.FillMode = FillMode.Winding;
                    float emSize = g.DpiY * font.SizeInPoints / 72;
                    gp.AddString(parsedText, font.FontFamily, (int)font.Style, emSize, Point.Empty, StringFormat.GenericDefault);

                    if (Angle != 0)
                    {
                        using (Matrix matrix = new Matrix())
                        {
                            matrix.Rotate(Angle);
                            gp.Transform(matrix);
                        }
                    }

                    RectangleF pathRect = gp.GetBounds();

                    if (pathRect.IsEmpty)
                    {
                        return bmp;
                    }

                    Size textSize = pathRect.Size.ToSize().Offset(1);
                    Point textPosition = Helpers.GetPosition(Placement, Offset, bmp.Size, textSize);
                    Rectangle textRectangle = new Rectangle(textPosition, textSize);

                    if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(textRectangle))
                    {
                        return bmp;
                    }

                    using (Matrix matrix = new Matrix())
                    {
                        matrix.Translate(textRectangle.X - pathRect.X, textRectangle.Y - pathRect.Y);
                        gp.Transform(matrix);
                    }

                    // Draw text shadow
                    if (Shadow && ((!ShadowUseGradient && ShadowColor.A > 0) || (ShadowUseGradient && ShadowGradient.IsVisible)))
                    {
                        using (Matrix matrix = new Matrix())
                        {
                            matrix.Translate(ShadowOffset.X, ShadowOffset.Y);
                            gp.Transform(matrix);

                            if (Outline && OutlineSize > 0)
                            {
                                if (ShadowUseGradient)
                                {
                                    using (LinearGradientBrush textShadowBrush = ShadowGradient.GetGradientBrush(
                                        Rectangle.Round(textRectangle).Offset(OutlineSize + 1).LocationOffset(ShadowOffset)))
                                    using (Pen textShadowPen = new Pen(textShadowBrush, OutlineSize) { LineJoin = LineJoin.Round })
                                    {
                                        g.DrawPath(textShadowPen, gp);
                                    }
                                }
                                else
                                {
                                    using (Pen textShadowPen = new Pen(ShadowColor, OutlineSize) { LineJoin = LineJoin.Round })
                                    {
                                        g.DrawPath(textShadowPen, gp);
                                    }
                                }
                            }
                            else
                            {
                                if (ShadowUseGradient)
                                {
                                    using (Brush textShadowBrush = ShadowGradient.GetGradientBrush(
                                        Rectangle.Round(textRectangle).Offset(1).LocationOffset(ShadowOffset)))
                                    {
                                        g.FillPath(textShadowBrush, gp);
                                    }
                                }
                                else
                                {
                                    using (Brush textShadowBrush = new SolidBrush(ShadowColor))
                                    {
                                        g.FillPath(textShadowBrush, gp);
                                    }
                                }
                            }

                            matrix.Reset();
                            matrix.Translate(-ShadowOffset.X, -ShadowOffset.Y);
                            gp.Transform(matrix);
                        }
                    }

                    // Draw text outline
                    if (Outline && OutlineSize > 0)
                    {
                        if (OutlineUseGradient)
                        {
                            if (OutlineGradient.IsVisible)
                            {
                                using (LinearGradientBrush textOutlineBrush = OutlineGradient.GetGradientBrush(Rectangle.Round(textRectangle).Offset(OutlineSize + 1)))
                                using (Pen textOutlinePen = new Pen(textOutlineBrush, OutlineSize) { LineJoin = LineJoin.Round })
                                {
                                    g.DrawPath(textOutlinePen, gp);
                                }
                            }
                        }
                        else if (OutlineColor.A > 0)
                        {
                            using (Pen textOutlinePen = new Pen(OutlineColor, OutlineSize) { LineJoin = LineJoin.Round })
                            {
                                g.DrawPath(textOutlinePen, gp);
                            }
                        }
                    }

                    // Draw text
                    if (UseGradient)
                    {
                        if (Gradient.IsVisible)
                        {
                            using (Brush textBrush = Gradient.GetGradientBrush(Rectangle.Round(textRectangle).Offset(1)))
                            {
                                g.FillPath(textBrush, gp);
                            }
                        }
                    }
                    else if (Color.A > 0)
                    {
                        using (Brush textBrush = new SolidBrush(Color))
                        {
                            g.FillPath(textBrush, gp);
                        }
                    }
                }

                return bmp;
            }
        }

        protected override string GetSummary()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                return Text.Truncate(20, "...");
            }

            return null;
        }
    }
}