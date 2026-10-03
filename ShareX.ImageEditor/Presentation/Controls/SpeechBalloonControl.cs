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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Helpers;

namespace ShareX.ImageEditor.Presentation.Controls
{
    /// <summary>
    /// Custom control for rendering a speech balloon with a draggable tail using Avalonia Path
    /// </summary>
    public class SpeechBalloonControl : Control
    {
        public static readonly StyledProperty<SpeechBalloonAnnotation?> AnnotationProperty =
            AvaloniaProperty.Register<SpeechBalloonControl, SpeechBalloonAnnotation?>(nameof(Annotation));

        public SpeechBalloonAnnotation? Annotation
        {
            get => GetValue(AnnotationProperty);
            set => SetValue(AnnotationProperty, value);
        }

        static SpeechBalloonControl()
        {
            AffectsRender<SpeechBalloonControl>(AnnotationProperty);
            AffectsMeasure<SpeechBalloonControl>(AnnotationProperty);
        }

        public SpeechBalloonControl()
        {
            ClipToBounds = false;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (Annotation == null || Bounds.Width <= 0 || Bounds.Height <= 0) return;

            var bodyBounds = Annotation.GetBounds();
            var renderBounds = Annotation.GetInteractionBounds();
            var bodyOffset = new Point(bodyBounds.Left - renderBounds.Left, bodyBounds.Top - renderBounds.Top);
            var width = Math.Max(bodyBounds.Width, 20);
            var height = Math.Max(bodyBounds.Height, 20);

            var bodyGeometry = CreateBodyGeometry(bodyOffset.X, bodyOffset.Y, width, height, Annotation.CornerRadius);
            var tailGeometry = CreateTailGeometry(Annotation, renderBounds);
            Geometry geometry = tailGeometry != null
                ? new CombinedGeometry(GeometryCombineMode.Union, bodyGeometry, tailGeometry)
                : bodyGeometry;

            // Parse colors
            var strokeColor = Color.Parse(Annotation.StrokeColor);
            var fillColor = Color.Parse(Annotation.FillColor);
            var fillBrush = new SolidColorBrush(fillColor);
            var strokePen = new Pen(new SolidColorBrush(strokeColor), Annotation.StrokeWidth);

            context.DrawGeometry(
                fillBrush,
                null,
                geometry
            );

            context.DrawGeometry(
                null,
                strokePen,
                geometry
            );

            // Draw text if present
            if (!string.IsNullOrEmpty(Annotation.Text))
            {
                var textColor = Color.Parse(Annotation.TextColor);
                var fontFamily = FontFamilyResolver.Resolve(Annotation.FontFamily);
                var typeface = new Typeface(
                    fontFamily,
                    Annotation.IsItalic ? FontStyle.Italic : FontStyle.Normal,
                    Annotation.IsBold ? FontWeight.Bold : FontWeight.Normal);
                var formattedText = new FormattedText(
                    Annotation.Text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    Annotation.FontSize,
                    new SolidColorBrush(textColor)
                );

                // Calculate centered text position with padding
                var padding = 12; // Adjusted to match TextBox padding

                // Allow wrapping
                var maxTextWidth = Math.Max(0, width - (padding * 2));
                formattedText.MaxTextWidth = maxTextWidth;
                formattedText.TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(Annotation.HorizontalAlignment);
                var textX = bodyOffset.X + padding;
                var textY = bodyOffset.Y + Math.Max(padding, (height - formattedText.Height) / 2);

                // Ensure text stays within bounds
                textY = Math.Min(textY, bodyOffset.Y + height - formattedText.Height - padding);

                context.DrawText(formattedText, new Point(textX, textY));
            }
        }

        private static Geometry CreateBodyGeometry(double x, double y, double width, double height, int cornerRadius)
        {
            var geometry = new StreamGeometry();
            double radius = Math.Clamp(cornerRadius, 0, (int)(Math.Min(width, height) / 2d));

            using (var ctx = geometry.Open())
            {
                double left = x;
                double top = y;
                double right = x + width;
                double bottom = y + height;

                if (radius <= 0)
                {
                    ctx.BeginFigure(new Point(left, top), true);
                    ctx.LineTo(new Point(right, top));
                    ctx.LineTo(new Point(right, bottom));
                    ctx.LineTo(new Point(left, bottom));
                    ctx.EndFigure(true);
                    return geometry;
                }

                // Start at top-left after the rounded corner
                ctx.BeginFigure(new Point(left + radius, top), true);

                ctx.LineTo(new Point(right - radius, top));

                // Top-right corner
                ctx.ArcTo(
                    new Point(right, top + radius),
                    new Size(radius, radius),
                    0,
                    false,
                    SweepDirection.Clockwise
                );

                ctx.LineTo(new Point(right, bottom - radius));

                // Bottom-right corner
                ctx.ArcTo(
                    new Point(right - radius, bottom),
                    new Size(radius, radius),
                    0,
                    false,
                    SweepDirection.Clockwise
                );

                ctx.LineTo(new Point(left + radius, bottom));

                // Bottom-left corner
                ctx.ArcTo(
                    new Point(left, bottom - radius),
                    new Size(radius, radius),
                    0,
                    false,
                    SweepDirection.Clockwise
                );

                ctx.LineTo(new Point(left, top + radius));

                // Top-left corner
                ctx.ArcTo(
                    new Point(left + radius, top),
                    new Size(radius, radius),
                    0,
                    false,
                    SweepDirection.Clockwise
                );

                ctx.EndFigure(true);
            }

            return geometry;
        }

        private Geometry? CreateTailGeometry(SpeechBalloonAnnotation annotation, SkiaSharp.SKRect renderBounds)
        {
            if (!annotation.TryGetTailPolygon(out var tailBaseStart, out var tailTip, out var tailBaseEnd))
            {
                return null;
            }

            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(ToRenderPoint(tailBaseStart, renderBounds), true);
                ctx.LineTo(ToRenderPoint(tailTip, renderBounds));
                ctx.LineTo(ToRenderPoint(tailBaseEnd, renderBounds));
                ctx.EndFigure(true);
            }

            return geometry;
        }

        private static Point ToRenderPoint(SkiaSharp.SKPoint point, SkiaSharp.SKRect renderBounds)
        {
            return new Point(point.X - renderBounds.Left, point.Y - renderBounds.Top);
        }
    }
}
