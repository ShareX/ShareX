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
using SkiaSharp;
using System.ComponentModel;
using System.Drawing;

namespace ShareX.ImageEffectsLib
{
    [Description("Text watermark")]
    public class DrawText : ImageEffect
    {
        [DefaultValue("Text watermark")]
        public string Text { get; set; }

        [DefaultValue(ImageAlignment.BottomRight)]
        public ImageAlignment Placement { get; set; }

        [DefaultValue(typeof(Point), "5, 5")]
        public Point Offset { get; set; }

        [DefaultValue(false), Description("If text watermark size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        // Workaround for "System.AccessViolationException: Attempted to read or write protected memory. This is often an indication that other memory is corrupt."
        [DefaultValue(typeof(FontInfo), "Arial, 11.25pt")]
        public FontInfo TextFont { get; set; }

        [DefaultValue(TextRenderingMode.SystemDefault)]
        public TextRenderingMode TextRenderingMode { get; set; }

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

        [DefaultValue(typeof(Insets), "5, 5, 5, 5")]
        public Insets Padding { get; set; }

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
            if (string.IsNullOrEmpty(Text) || TextFont == null || TextFont.SizeInPoints < 1)
            {
                return bmp;
            }

            NameParser parser = new NameParser(NameParserType.Text)
            {
                ImageWidth = bmp.Width,
                ImageHeight = bmp.Height
            };

            string parsedText = parser.Parse(Text);
            Size textSize = SkiaImageHelpers.MeasureText(parsedText, TextFont);
            Size watermarkSize = new Size(Padding.Left + textSize.Width + Padding.Right, Padding.Top + textSize.Height + Padding.Bottom);
            Point watermarkPosition = SkiaImageHelpers.GetPosition(Placement, Offset, new Size(bmp.Width, bmp.Height), watermarkSize);
            Rectangle watermarkRectangle = new Rectangle(watermarkPosition, watermarkSize);

            if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(watermarkRectangle))
            {
                return bmp;
            }

            using SKCanvas canvas = new SKCanvas(bmp);
            SKRect box = new SKRect(watermarkRectangle.Left, watermarkRectangle.Top, watermarkRectangle.Right - 1, watermarkRectangle.Bottom - 1);

            if (DrawBackground)
            {
                using SKShader shader = UseGradient && Gradient != null && Gradient.IsValid
                    ? Gradient.CreateShader(new SKRect(watermarkRectangle.Left, watermarkRectangle.Top, watermarkRectangle.Right, watermarkRectangle.Bottom))
                    : null;
                using SKPaint background = new SKPaint { IsAntialias = true, Color = BackgroundColor.ToSKColor(), Shader = shader };
                canvas.DrawRoundRect(box, CornerRadius, CornerRadius, background);
            }

            if (DrawBorder)
            {
                int borderSize = BorderSize.Max(1);
                using SKPaint border = new SKPaint { IsAntialias = true, IsStroke = true, StrokeWidth = borderSize, Color = BorderColor.ToSKColor() };
                float offset = borderSize % 2 == 0 ? 0 : 0.5f;
                canvas.DrawRoundRect(new SKRect(box.Left + offset, box.Top + offset, box.Right + offset, box.Bottom + offset), CornerRadius, CornerRadius, border);
            }

            bool antialias = TextRenderingMode is not (TextRenderingMode.SingleBitPerPixel or TextRenderingMode.SingleBitPerPixelGridFit);
            float textX = watermarkRectangle.X + Padding.Left;
            float textY = watermarkRectangle.Y + Padding.Top;

            if (DrawTextShadow)
            {
                using SKPaint shadow = new SKPaint { IsAntialias = antialias, Color = TextShadowColor.ToSKColor() };
                SkiaImageHelpers.DrawText(canvas, parsedText, TextFont, shadow, textX + TextShadowOffset.X, textY + TextShadowOffset.Y);
            }

            using SKPaint text = new SKPaint { IsAntialias = antialias, Color = TextColor.ToSKColor() };
            SkiaImageHelpers.DrawText(canvas, parsedText, TextFont, text, textX, textY);
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
