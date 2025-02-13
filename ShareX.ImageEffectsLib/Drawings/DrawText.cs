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
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ImageEffectsLib
{
    [Description("Text watermark")]
    public class DrawText : ImageEffect
    {
        [DefaultValue("Text watermark"), Editor(typeof(NameParserEditor), typeof(UITypeEditor))]
        public string Text { get; set; }

        [DefaultValue(ContentAlignment.BottomRight), TypeConverter(typeof(EnumProperNameConverter))]
        public ContentAlignment Placement { get; set; }

        [DefaultValue(typeof(Point), "5, 5")]
        public Point Offset { get; set; }

        [DefaultValue(false), Description("If text watermark size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        private FontSafe textFontSafe = new FontSafe();

        // Workaround for "System.AccessViolationException: Attempted to read or write protected memory. This is often an indication that other memory is corrupt."
        [DefaultValue(typeof(Font), "Arial, 11.25pt")]
        public Font TextFont
        {
            get
            {
                return textFontSafe.GetFont();
            }
            set
            {
                using (value)
                {
                    textFontSafe.SetFont(value);
                }
            }
        }

        [DefaultValue(TextRenderingHint.SystemDefault), TypeConverter(typeof(EnumProperNameConverter))]
        public TextRenderingHint TextRenderingMode { get; set; }

        [DefaultValue(typeof(Color), "235, 235, 235"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextColor { get; set; }

        [DefaultValue(true)]
        public bool DrawTextShadow { get; set; }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextShadowColor { get; set; }

        [DefaultValue(typeof(Point), "-1, -1")]
        public Point TextShadowOffset { get; set; }

        private int cornerRadius;

        [DefaultValue(4)]
        public int CornerRadius
        {
            get
            {
                return cornerRadius;
            }
            set
            {
                cornerRadius = value.Max(0);
            }
        }

        [DefaultValue(typeof(Padding), "5, 5, 5, 5")]
        public Padding Padding { get; set; }

        [DefaultValue(true)]
        public bool DrawBorder { get; set; }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BorderColor { get; set; }

        [DefaultValue(1)]
        public int BorderSize { get; set; }

        [DefaultValue(true)]
        public bool DrawBackground { get; set; }

        [DefaultValue(typeof(Color), "42, 47, 56"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BackgroundColor { get; set; }

        [DefaultValue(false)]
        public bool UseGradient { get; set; }

        [Editor(typeof(GradientEditor), typeof(UITypeEditor))]
        public GradientInfo Gradient { get; set; }

        public DrawText()
        {
            this.ApplyDefaultPropertyValues();
            AddDefaultGradient();
        }

        private void AddDefaultGradient()
        {
            Gradient = new GradientInfo();
            Gradient.Colors.Add(new GradientStop(Color.FromArgb(68, 120, 194), 0f));
            Gradient.Colors.Add(new GradientStop(Color.FromArgb(13, 58, 122), 50f));
            Gradient.Colors.Add(new GradientStop(Color.FromArgb(6, 36, 78), 50f));
            Gradient.Colors.Add(new GradientStop(Color.FromArgb(23, 89, 174), 100f));
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return bmp;
            }

            using (Font textFont = TextFont)
            {
                if (textFont == null || textFont.Size < 1)
                {
                    return bmp;
                }

                NameParser parser = new NameParser(NameParserType.Text);

                if (bmp != null)
                {
                    parser.ImageWidth = bmp.Width;
                    parser.ImageHeight = bmp.Height;
                }

                string parsedText = parser.Parse(Text);

                Size textSize = Helpers.MeasureText(parsedText, textFont);
                Size watermarkSize = new Size(Padding.Left + textSize.Width + Padding.Right, Padding.Top + textSize.Height + Padding.Bottom);
                Point watermarkPosition = Helpers.GetPosition(Placement, Offset, bmp.Size, watermarkSize);
                Rectangle watermarkRectangle = new Rectangle(watermarkPosition, watermarkSize);

                if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(watermarkRectangle))
                {
                    return bmp;
                }

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        gp.AddRoundedRectangleProper(watermarkRectangle, CornerRadius);

                        if (DrawBackground)
                        {
                            Brush backgroundBrush = null;

                            try
                            {
                                if (UseGradient && Gradient != null && Gradient.IsValid)
                                {
                                    backgroundBrush = Gradient.GetGradientBrush(watermarkRectangle);
                                }
                                else
                                {
                                    backgroundBrush = new SolidBrush(BackgroundColor);
                                }

                                g.FillPath(backgroundBrush, gp);
                            }
                            finally
                            {
                                if (backgroundBrush != null) backgroundBrush.Dispose();
                            }
                        }

                        if (DrawBorder)
                        {
                            int borderSize = BorderSize.Max(1);

                            if (borderSize.IsEvenNumber())
                            {
                                g.PixelOffsetMode = PixelOffsetMode.Half;
                            }

                            using (Pen borderPen = new Pen(BorderColor, borderSize))
                            {
                                g.DrawPath(borderPen, gp);
                            }

                            g.PixelOffsetMode = PixelOffsetMode.Default;
                        }
                    }

                    g.TextRenderingHint = TextRenderingMode;

                    if (DrawTextShadow)
                    {
                        using (Brush textShadowBrush = new SolidBrush(TextShadowColor))
                        {
                            g.DrawString(parsedText, textFont, textShadowBrush, watermarkRectangle.X + Padding.Left + TextShadowOffset.X,
                                watermarkRectangle.Y + Padding.Top + TextShadowOffset.Y);
                        }
                    }

                    using (Brush textBrush = new SolidBrush(TextColor))
                    {
                        g.DrawString(parsedText, textFont, textBrush, watermarkRectangle.X + Padding.Left, watermarkRectangle.Y + Padding.Top);
                    }
                }
            }

            return bmp;
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