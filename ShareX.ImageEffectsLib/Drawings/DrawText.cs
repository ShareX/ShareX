#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using SkiaSharp;

using System.Windows.Forms;

namespace ShareX.ImageEffectsLib
{
    [Description("Text watermark")]
    public class DrawText : ImageEffect
    {
        [DefaultValue("Text watermark")]
        public string Text { get; set; }

        [DefaultValue(ContentAlignment.BottomRight)]
        public ContentAlignment Placement { get; set; }

        [DefaultValue(typeof(Point), "5, 5")]
        public Point Offset { get; set; }

        [DefaultValue(false), Description("If text watermark size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        private FontSafe textFontSafe = new FontSafe();

        [DefaultValue(typeof(ImageFont), "Arial, 11.25pt")]
        public ImageFont TextFont
        {
            get
            {
                return textFontSafe.GetImageFont();
            }
            set
            {
                using (value)
                {
                    textFontSafe.SetImageFont(value);
                }
            }
        }

        [DefaultValue(ImageTextRenderingMode.SystemDefault)]
        public ImageTextRenderingMode TextRenderingMode { get; set; }

        [DefaultValue(typeof(Color), "235, 235, 235")]
        public Color TextColor { get; set; }

        [DefaultValue(true)]
        public bool DrawTextShadow { get; set; }

        [DefaultValue(typeof(Color), "Black")]
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

        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor { get; set; }

        [DefaultValue(1)]
        public int BorderSize { get; set; }

        [DefaultValue(true)]
        public bool DrawBackground { get; set; }

        [DefaultValue(typeof(Color), "42, 47, 56")]
        public Color BackgroundColor { get; set; }

        [DefaultValue(false)]
        public bool UseGradient { get; set; }

        public GradientInfo Gradient { get; set; }

        public DrawText()
        {
            this.ApplyDefaultPropertyValues();
            Text = Localization.Strings.ImageEffectDefault_Text_watermark;
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

        public override SKBitmap Apply(SKBitmap bmp)
        {
            using ImageFont font = TextFont;
            if (string.IsNullOrEmpty(Text) || font == null || font.Size < 1) return bmp;
            NameParser parser = new(NameParserType.Text) { ImageWidth = bmp.Width, ImageHeight = bmp.Height };
            string text = parser.Parse(Text);
            Size textSize = SkiaDrawing.MeasureText(text, font);
            Size size = new(Padding.Horizontal + textSize.Width, Padding.Vertical + textSize.Height);
            Point position = Helpers.GetPosition(Placement, Offset, bmp.GetSize(), size);
            Rectangle rectangle = new(position, size);
            if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(rectangle)) return bmp;
            using SKCanvas canvas = new(bmp);
            using SKPath path = new();
            path.AddRoundedRectangleProper(rectangle, CornerRadius);
            if (DrawBackground)
            {
                using SKPaint background = UseGradient && Gradient != null && Gradient.IsValid
                    ? Gradient.GetSkiaPaint(rectangle) : SkiaDrawing.Fill(BackgroundColor);
                canvas.DrawPath(path, background);
            }
            if (DrawBorder)
            {
                using SKPaint border = SkiaDrawing.Stroke(BorderColor, Math.Max(1, BorderSize));
                canvas.DrawPath(path, border);
            }
            PointF textPosition = new(position.X + Padding.Left, position.Y + Padding.Top);
            if (DrawTextShadow)
            {
                using SKPaint shadow = SkiaDrawing.Fill(TextShadowColor);
                canvas.DrawText(text, new PointF(textPosition.X + TextShadowOffset.X, textPosition.Y + TextShadowOffset.Y), font, shadow, TextRenderingMode);
            }
            using SKPaint paint = SkiaDrawing.Fill(TextColor);
            paint.IsAntialias = TextRenderingMode is not ImageTextRenderingMode.SingleBitPerPixel and not ImageTextRenderingMode.SingleBitPerPixelGridFit;
            canvas.DrawText(text, textPosition, font, paint, TextRenderingMode);
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
