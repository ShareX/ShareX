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
    [Description("Text")]
    public class DrawTextEx : ImageEffect
    {
        [DefaultValue("Text")]
        public string Text { get; set; }

        [DefaultValue(ImageAlignment.TopLeft)]
        public ImageAlignment Placement { get; set; }

        [DefaultValue(typeof(Point), "0, 0")]
        public Point Offset { get; set; }

        [DefaultValue(0)]
        public int Angle { get; set; }

        [DefaultValue(false), Description("If text size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        // Workaround for "System.AccessViolationException: Attempted to read or write protected memory. This is often an indication that other memory is corrupt."
        [DefaultValue(typeof(FontInfo), "Arial, 36pt")]
        public FontInfo Font { get; set; }

        [DefaultValue(typeof(Color), "235, 235, 235")]
        public Color Color { get; set; }

        [DefaultValue(false)]
        public bool UseGradient { get; set; }

        public GradientInfo Gradient { get; set; }

        [DefaultValue(false)]
        public bool Outline { get; set; }

        [DefaultValue(5)]
        public int OutlineSize { get; set; }

        [DefaultValue(typeof(Color), "235, 0, 0")]
        public Color OutlineColor { get; set; }

        [DefaultValue(false)]
        public bool OutlineUseGradient { get; set; }

        public GradientInfo OutlineGradient { get; set; }

        [DefaultValue(false)]
        public bool Shadow { get; set; }

        [DefaultValue(typeof(Point), "0, 5")]
        public Point ShadowOffset { get; set; }

        [DefaultValue(typeof(Color), "125, 0, 0, 0")]
        public Color ShadowColor { get; set; }

        [DefaultValue(false)]
        public bool ShadowUseGradient { get; set; }

        public GradientInfo ShadowGradient { get; set; }

        public DrawTextEx()
        {
            this.ApplyDefaultPropertyValues();
            Text = Localization.Strings.ImageEffectDefault_Text;
            Gradient = AddDefaultGradient();
            OutlineGradient = AddDefaultGradient();
            ShadowGradient = AddDefaultGradient();
        }

        private GradientInfo AddDefaultGradient()
        {
            GradientInfo gradientInfo = new GradientInfo();
            gradientInfo.Type = GradientDirection.Horizontal;

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

        public override SKBitmap Apply(SKBitmap bmp)
        {
            if (string.IsNullOrEmpty(Text) || Font == null || Font.SizeInPoints < 1)
            {
                return bmp;
            }

            NameParser parser = new NameParser(NameParserType.Text)
            {
                ImageWidth = bmp.Width,
                ImageHeight = bmp.Height
            };

            string parsedText = parser.Parse(Text);

            using SKPath path = SkiaImageHelpers.GetTextPath(parsedText, Font);

            if (Angle != 0)
            {
                path.Transform(SKMatrix.CreateRotationDegrees(Angle));
            }

            SKRect pathRect = path.Bounds;

            if (pathRect.IsEmpty)
            {
                return bmp;
            }

            Size textSize = new Size((int)pathRect.Width + 1, (int)pathRect.Height + 1);
            Point textPosition = SkiaImageHelpers.GetPosition(Placement, Offset, new Size(bmp.Width, bmp.Height), textSize);
            Rectangle textRectangle = new Rectangle(textPosition, textSize);

            if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(textRectangle))
            {
                return bmp;
            }

            path.Transform(SKMatrix.CreateTranslation(textRectangle.X - pathRect.Left, textRectangle.Y - pathRect.Top));

            using SKCanvas canvas = new SKCanvas(bmp);

            SKRect Grow(Rectangle rect, int amount, Point offset = default) =>
                new SKRect(rect.Left - amount + offset.X, rect.Top - amount + offset.Y, rect.Right + amount + offset.X, rect.Bottom + amount + offset.Y);

            if (Shadow && ((!ShadowUseGradient && ShadowColor.A > 0) || (ShadowUseGradient && ShadowGradient.IsVisible)))
            {
                canvas.Save();
                canvas.Translate(ShadowOffset.X, ShadowOffset.Y);
                bool outline = Outline && OutlineSize > 0;
                using SKShader shader = ShadowUseGradient ? ShadowGradient.CreateShader(Grow(textRectangle, outline ? OutlineSize + 1 : 1)) : null;
                using SKPaint paint = new SKPaint { IsAntialias = true, Color = ShadowColor.ToSKColor(), Shader = shader };

                if (outline)
                {
                    paint.IsStroke = true;
                    paint.StrokeWidth = OutlineSize;
                    paint.StrokeJoin = SKStrokeJoin.Round;
                }

                canvas.DrawPath(path, paint);
                canvas.Restore();
            }

            if (Outline && OutlineSize > 0 && (OutlineUseGradient ? OutlineGradient.IsVisible : OutlineColor.A > 0))
            {
                using SKShader shader = OutlineUseGradient ? OutlineGradient.CreateShader(Grow(textRectangle, OutlineSize + 1)) : null;
                using SKPaint paint = new SKPaint
                {
                    IsAntialias = true,
                    IsStroke = true,
                    StrokeWidth = OutlineSize,
                    StrokeJoin = SKStrokeJoin.Round,
                    Color = OutlineColor.ToSKColor(),
                    Shader = shader
                };
                canvas.DrawPath(path, paint);
            }

            if (UseGradient ? Gradient.IsVisible : Color.A > 0)
            {
                using SKShader shader = UseGradient ? Gradient.CreateShader(Grow(textRectangle, 1)) : null;
                using SKPaint paint = new SKPaint { IsAntialias = true, Color = Color.ToSKColor(), Shader = shader };
                canvas.DrawPath(path, paint);
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
