#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;

namespace ShareX.ImageEffectsLib
{
    [Description("Text watermark")]
    public class DrawText : ImageEffect
    {
        [DefaultValue(ContentAlignment.BottomRight)]
        public ContentAlignment Placement { get; set; }

        private Point offset;

        [DefaultValue(typeof(Point), "5, 5")]
        public Point Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = new Point(value.X.Min(0), value.Y.Min(0));
            }
        }

        [DefaultValue(true), Description("If text watermark size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        [DefaultValue("getsharex.com"), Editor(typeof(NameParserEditor), typeof(UITypeEditor))]
        public string Text { get; set; }

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

        [DefaultValue(typeof(Color), "White"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextColor { get; set; }

        [DefaultValue(true)]
        public bool DrawTextShadow { get; set; }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextShadowColor { get; set; }

        [DefaultValue(typeof(Point), "-1, -1")]
        public Point TextShadowOffset { get; set; }

        [DefaultValue(true)]
        public bool DrawBackground { get; set; }

        [DefaultValue(5)]
        public int BackgroundPadding { get; set; }

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
                cornerRadius = value.Min(0);
            }
        }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BorderColor { get; set; }

        [DefaultValue(typeof(Color), "10, 110, 230"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BackgroundColor { get; set; }

        [DefaultValue(true)]
        public bool UseGradient { get; set; }

        [DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode GradientType { get; set; }

        [DefaultValue(typeof(Color), "0, 20, 40"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BackgroundColor2 { get; set; }

        [DefaultValue(false)]
        public bool UseCustomGradient { get; set; }

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

        public override Image Apply(Image img)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return img;
            }

            using (Font textFont = TextFont)
            {
                if (textFont == null || textFont.Size < 1)
                {
                    return img;
                }

                NameParser parser = new NameParser(NameParserType.Text) { Picture = img };
                string parsedText = parser.Parse(Text);

                Size textSize = Helpers.MeasureText(parsedText, textFont);
                Size watermarkSize = new Size(textSize.Width + BackgroundPadding * 2, textSize.Height + BackgroundPadding * 2);
                Point watermarkPosition = Helpers.GetPosition(Placement, Offset, img.Size, watermarkSize);
                Rectangle watermarkRectangle = new Rectangle(watermarkPosition, watermarkSize);

                if (AutoHide && !new Rectangle(0, 0, img.Width, img.Height).Contains(watermarkRectangle))
                {
                    return img;
                }

                using (Bitmap bmpWatermark = new Bitmap(watermarkSize.Width, watermarkSize.Height))
                using (Graphics gWatermark = Graphics.FromImage(bmpWatermark))
                {
                    gWatermark.SetHighQuality();

                    if (DrawBackground)
                    {
                        Rectangle backgroundRect = new Rectangle(0, 0, watermarkSize.Width, watermarkSize.Height);
                        Brush backgroundBrush = null;

                        try
                        {
                            if (UseGradient)
                            {
                                if (UseCustomGradient && Gradient != null && Gradient.IsValid)
                                {
                                    backgroundBrush = new LinearGradientBrush(backgroundRect, Color.Transparent, Color.Transparent, Gradient.Type);
                                    ColorBlend colorBlend = new ColorBlend();
                                    IEnumerable<GradientStop> gradient = Gradient.Colors.OrderBy(x => x.Location);
                                    colorBlend.Colors = gradient.Select(x => x.Color).ToArray();
                                    colorBlend.Positions = gradient.Select(x => x.Location / 100).ToArray();
                                    ((LinearGradientBrush)backgroundBrush).InterpolationColors = colorBlend;
                                }
                                else
                                {
                                    backgroundBrush = new LinearGradientBrush(backgroundRect, BackgroundColor, BackgroundColor2, GradientType);
                                }
                            }
                            else
                            {
                                backgroundBrush = new SolidBrush(BackgroundColor);
                            }

                            using (Pen borderPen = new Pen(BorderColor))
                            {
                                gWatermark.DrawRoundedRectangle(backgroundBrush, borderPen, backgroundRect, CornerRadius);
                            }
                        }
                        finally
                        {
                            if (backgroundBrush != null) backgroundBrush.Dispose();
                        }
                    }

                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        float centerX = bmpWatermark.Width / 2f;
                        float centerY = bmpWatermark.Height / 2f;

                        if (DrawTextShadow)
                        {
                            using (Brush textShadowBrush = new SolidBrush(TextShadowColor))
                            {
                                gWatermark.DrawString(parsedText, textFont, textShadowBrush, centerX + TextShadowOffset.X, centerY + TextShadowOffset.Y, sf);
                            }
                        }

                        using (Brush textBrush = new SolidBrush(TextColor))
                        {
                            gWatermark.DrawString(parsedText, textFont, textBrush, centerX, centerY, sf);
                        }
                    }

                    using (Graphics gResult = Graphics.FromImage(img))
                    {
                        gResult.SetHighQuality();
                        gResult.DrawImage(bmpWatermark, watermarkRectangle);
                    }
                }
            }

            return img;
        }
    }
}