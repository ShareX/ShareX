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
using System.Globalization;

namespace ShareX.ImageEditor.Presentation.Controls
{
    /// <summary>
    /// Custom control for rendering text with both an outline (stroke) and a fill.
    /// Avalonia's native TextBox does not support text strokes, so we render via FormattedText and Geometry.
    /// </summary>
    public class OutlinedTextControl : Control
    {
        private const double TextPadding = 8;

        public static readonly StyledProperty<TextAnnotation?> AnnotationProperty =
            AvaloniaProperty.Register<OutlinedTextControl, TextAnnotation?>(nameof(Annotation));

        public TextAnnotation? Annotation
        {
            get => GetValue(AnnotationProperty);
            set => SetValue(AnnotationProperty, value);
        }

        static OutlinedTextControl()
        {
            AffectsRender<OutlinedTextControl>(AnnotationProperty);
            AffectsMeasure<OutlinedTextControl>(AnnotationProperty);
        }

        public OutlinedTextControl()
        {
            ClipToBounds = true;
        }

        internal static Size MeasureNaturalSize(TextAnnotation annotation)
        {
            if (annotation == null || string.IsNullOrEmpty(annotation.Text))
            {
                return new Size(0, 0);
            }

            var typeface = new Typeface(
                FontFamilyResolver.Resolve(annotation.FontFamily),
                annotation.IsItalic ? FontStyle.Italic : FontStyle.Normal,
                annotation.IsBold ? FontWeight.Bold : FontWeight.Normal);

            var formattedText = new FormattedText(
                annotation.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                typeface,
                annotation.FontSize,
                Brushes.Black);

            double strokePadding = annotation.StrokeWidth;
            double padding = TextPadding * 2;
            double width = formattedText.Width + padding + (strokePadding * 2);
            double height = formattedText.Height + padding + (strokePadding * 2);

            if (double.IsNaN(width) || width < 0) width = 0;
            if (double.IsNaN(height) || height < 0) height = 0;

            return new Size(width, height);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (Annotation == null || string.IsNullOrEmpty(Annotation.Text)) return;

            var typeface = new Typeface(
                FontFamilyResolver.Resolve(Annotation.FontFamily),
                Annotation.IsItalic ? FontStyle.Italic : FontStyle.Normal,
                Annotation.IsBold ? FontWeight.Bold : FontWeight.Normal);

            double strokePadding = Annotation.StrokeWidth;
            double horizontalPadding = TextPadding + strokePadding;
            double verticalPadding = TextPadding + strokePadding;
            double maxTextWidth = Math.Max(0, Bounds.Width - (horizontalPadding * 2));

            var formattedText = new FormattedText(
                Annotation.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                typeface,
                Annotation.FontSize,
                Brushes.Black); // Avalonia requires a brush to properly construct text extents in some backends

            if (maxTextWidth > 0)
            {
                formattedText.MaxTextWidth = maxTextWidth;
            }

            formattedText.TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(Annotation.HorizontalAlignment);

            double textWidth = formattedText.Width;
            double textHeight = formattedText.Height;
            double originX = horizontalPadding;
            double originY = Math.Max(verticalPadding, (Bounds.Height - textHeight) / 2.0);

            originY = Math.Min(originY, Math.Max(verticalPadding, Bounds.Height - textHeight - verticalPadding));

            var textGeometry = formattedText.BuildGeometry(new Point(originX, originY));
            if (textGeometry == null) return;

            // Setup brushes based on the annotation's color properties.
            // Text color is now TextColor, Outline color is StrokeColor.

            // Standard behavior in many editors is that transparent fill means no fill.
            IBrush? fillBrush = null;
            if (!string.IsNullOrEmpty(Annotation.TextColor))
            {
                var textColor = Color.Parse(Annotation.TextColor);
                if (textColor.A > 0)
                {
                    fillBrush = new SolidColorBrush(textColor);
                }
            }

            IPen? strokePen = null;
            if (Annotation.StrokeWidth > 0 && !string.IsNullOrEmpty(Annotation.StrokeColor))
            {
                var strokeColor = Color.Parse(Annotation.StrokeColor);
                if (strokeColor.A > 0)
                {
                    strokePen = new Pen(new SolidColorBrush(strokeColor), Annotation.StrokeWidth, lineJoin: PenLineJoin.Round);
                }
            }

            // Draw main text (Stroke then Fill, so fill is on top of stroke)
            // To get the stroke BEHIND the fill (standard typography outline), we must draw the stroke geometry first, then fill geometry.
            if (strokePen != null)
            {
                context.DrawGeometry(null, strokePen, textGeometry);
            }

            if (fillBrush != null)
            {
                context.DrawGeometry(fillBrush, null, textGeometry);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Annotation == null || string.IsNullOrEmpty(Annotation.Text))
            {
                return new Size(0, 0);
            }

            var typeface = new Typeface(
                FontFamilyResolver.Resolve(Annotation.FontFamily),
                Annotation.IsItalic ? FontStyle.Italic : FontStyle.Normal,
                Annotation.IsBold ? FontWeight.Bold : FontWeight.Normal);

            var formattedText = new FormattedText(
                Annotation.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                typeface,
                Annotation.FontSize,
                Brushes.Black);

            double strokePadding = Annotation.StrokeWidth;
            double padding = TextPadding * 2;
            var annotationBounds = Annotation.GetBounds();
            double boundedWidth = Math.Max(0, annotationBounds.Width - padding - (strokePadding * 2));

            if (boundedWidth > 0)
            {
                formattedText.MaxTextWidth = boundedWidth;
            }

            double width = Math.Max(annotationBounds.Width, formattedText.Width + padding + (strokePadding * 2));
            double height = Math.Max(annotationBounds.Height, formattedText.Height + padding + (strokePadding * 2));

            if (double.IsNaN(width) || width < 0) width = 0;
            if (double.IsNaN(height) || height < 0) height = 0;

            return new Size(width, height);
        }
    }
}
