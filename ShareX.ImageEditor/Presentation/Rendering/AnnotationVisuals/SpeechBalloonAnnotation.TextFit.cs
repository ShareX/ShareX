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

using Avalonia.Media;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class SpeechBalloonAnnotation
{
    /// <summary>
    /// The body size that fits <paramref name="text"/> on its own lines, without wrapping, with
    /// <see cref="TextPadding"/> on every side. Empty text gets the height of one line and no text width.
    /// </summary>
    internal SKSize MeasureTextFitSize(string? text)
    {
        // Same typeface as SpeechBalloonControl, which draws the text.
        string fontFamily = string.IsNullOrWhiteSpace(FontFamily) ? "Segoe UI" : FontFamily;
        var typeface = new Typeface(
            fontFamily,
            IsItalic ? FontStyle.Italic : FontStyle.Normal,
            IsBold ? FontWeight.Bold : FontWeight.Normal);

        bool isEmpty = string.IsNullOrEmpty(text);
        string measuredText = isEmpty ? " " : text!;

        // A trailing line break gets no height of its own, so measure the new empty line as a space,
        // which the text box already shows the caret on.
        if (measuredText.EndsWith('\n') || measuredText.EndsWith('\r'))
        {
            measuredText += " ";
        }

        var formattedText = new FormattedText(
            measuredText,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            FontSize,
            Brushes.Black);

        double textWidth = isEmpty ? 0 : formattedText.WidthIncludingTrailingWhitespace;

        // One spare pixel so rounding never wraps the last word.
        float width = (float)Math.Ceiling(textWidth + TextPadding * 2 + StrokeWidth + 1);
        float height = (float)Math.Ceiling(formattedText.Height + TextPadding * 2 + StrokeWidth);
        return new SKSize(width, height);
    }

    /// <summary>
    /// Sizes the body to fit <paramref name="text"/> (see <see cref="MeasureTextFitSize"/>), keeping its top-left
    /// corner where it is on screen, rotated or not. The tail keeps pointing at the same spot.
    /// </summary>
    internal void FitToText(string? text)
    {
        SKSize size = MeasureTextFitSize(text);
        SKRect bounds = GetBounds();

        float left = bounds.Left;
        float top = bounds.Top;

        if (RotationAngle != 0)
        {
            // The body rotates around its center, so move the center by the rotated half of the size change.
            float halfDeltaX = (size.Width - bounds.Width) / 2;
            float halfDeltaY = (size.Height - bounds.Height) / 2;
            float radians = RotationAngle * MathF.PI / 180f;
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            float centerX = bounds.MidX + halfDeltaX * cos - halfDeltaY * sin;
            float centerY = bounds.MidY + halfDeltaX * sin + halfDeltaY * cos;
            left = centerX - size.Width / 2;
            top = centerY - size.Height / 2;
        }

        // Pin the tail before the body moves, so a balloon that never had its tail placed doesn't follow the new size.
        if (TailEnabled)
        {
            EnsureTailPointInitialized();
        }

        StartPoint = new SKPoint(left, top);
        EndPoint = new SKPoint(left + size.Width, top + size.Height);
    }
}
