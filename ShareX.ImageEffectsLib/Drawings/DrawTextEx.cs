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

        [DefaultValue(ContentAlignment.TopLeft)]
        public ContentAlignment Placement { get; set; }

        [DefaultValue(typeof(Point), "0, 0")]
        public Point Offset { get; set; }

        [DefaultValue(0)]
        public int Angle { get; set; }

        [DefaultValue(false), Description("If text size bigger than source image then don't draw it.")]
        public bool AutoHide { get; set; }

        private FontSafe fontSafe = new FontSafe();

        [DefaultValue(typeof(ImageFont), "Arial, 36pt")]
        public ImageFont Font
        {
            get
            {
                return fontSafe.GetImageFont();
            }
            set
            {
                using (value)
                {
                    fontSafe.SetImageFont(value);
                }
            }
        }

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
            gradientInfo.Type = ImageGradientMode.Horizontal;

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
            using ImageFont font = Font;
            if (string.IsNullOrEmpty(Text) || font == null || font.Size < 1) return bmp;
            NameParser parser = new(NameParserType.Text) { ImageWidth = bmp.Width, ImageHeight = bmp.Height };
            using SKPath path = SkiaDrawing.TextPath(parser.Parse(Text), font);
            if (Angle != 0) path.Transform(SKMatrix.CreateRotationDegrees(Angle));
            SKRect bounds = path.Bounds;
            if (bounds.IsEmpty) return bmp;
            Size size = new((int)Math.Ceiling(bounds.Width) + 1, (int)Math.Ceiling(bounds.Height) + 1);
            Point position = Helpers.GetPosition(Placement, Offset, bmp.GetSize(), size);
            Rectangle rectangle = new(position, size);
            if (AutoHide && !new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(rectangle)) return bmp;
            path.Transform(SKMatrix.CreateTranslation(position.X - bounds.Left, position.Y - bounds.Top));
            using SKCanvas canvas = new(bmp);
            if (Shadow && (ShadowUseGradient ? ShadowGradient.IsVisible : ShadowColor.A > 0))
            {
                using SKPath shadowPath = new(path);
                shadowPath.Transform(SKMatrix.CreateTranslation(ShadowOffset.X, ShadowOffset.Y));
                using SKPaint shadow = ShadowUseGradient
                    ? ShadowGradient.GetSkiaPaint(rectangle.Offset(Outline ? OutlineSize + 1 : 1).LocationOffset(ShadowOffset))
                    : SkiaDrawing.Fill(ShadowColor);
                if (Outline && OutlineSize > 0) { shadow.Style = SKPaintStyle.Stroke; shadow.StrokeWidth = OutlineSize; shadow.StrokeJoin = SKStrokeJoin.Round; }
                canvas.DrawPath(shadowPath, shadow);
            }
            if (Outline && OutlineSize > 0 && (OutlineUseGradient ? OutlineGradient.IsVisible : OutlineColor.A > 0))
            {
                using SKPaint outline = OutlineUseGradient ? OutlineGradient.GetSkiaPaint(rectangle.Offset(OutlineSize + 1)) : SkiaDrawing.Fill(OutlineColor);
                outline.Style = SKPaintStyle.Stroke; outline.StrokeWidth = OutlineSize; outline.StrokeJoin = SKStrokeJoin.Round;
                canvas.DrawPath(path, outline);
            }
            if (UseGradient ? Gradient.IsVisible : Color.A > 0)
            {
                using SKPaint paint = UseGradient ? Gradient.GetSkiaPaint(rectangle.Offset(1)) : SkiaDrawing.Fill(Color);
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
