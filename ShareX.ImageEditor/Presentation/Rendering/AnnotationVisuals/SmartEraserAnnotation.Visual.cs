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
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class SmartEraserAnnotation
{
    public new Control CreateVisual()
    {
        var rectangle = (Avalonia.Controls.Shapes.Rectangle)base.CreateVisual();
        ApplyFill(rectangle);
        return rectangle;
    }

    internal void ClampVisualBounds(Avalonia.Controls.Shapes.Rectangle rectangle, Size canvasSize)
    {
        double left = Canvas.GetLeft(rectangle);
        double top = Canvas.GetTop(rectangle);
        double right = left + rectangle.Width;
        double bottom = top + rectangle.Height;

        double clippedLeft = Math.Clamp(left, 0, canvasSize.Width);
        double clippedTop = Math.Clamp(top, 0, canvasSize.Height);
        double clippedRight = Math.Clamp(right, 0, canvasSize.Width);
        double clippedBottom = Math.Clamp(bottom, 0, canvasSize.Height);

        Canvas.SetLeft(rectangle, clippedLeft);
        Canvas.SetTop(rectangle, clippedTop);
        rectangle.Width = Math.Max(0, clippedRight - clippedLeft);
        rectangle.Height = Math.Max(0, clippedBottom - clippedTop);
        StartPoint = new SKPoint((float)clippedLeft, (float)clippedTop);
        EndPoint = new SKPoint((float)clippedRight, (float)clippedBottom);
    }

    private static void ReplaceFill(Avalonia.Controls.Shapes.Rectangle rectangle, IBrush fill)
    {
        var previousBitmap = (rectangle.Fill as ImageBrush)?.Source;
        rectangle.Fill = fill;
        (previousBitmap as IDisposable)?.Dispose();
    }

    public void ApplyFill(Avalonia.Controls.Shapes.Rectangle rectangle)
    {
        ArgumentNullException.ThrowIfNull(rectangle);

        if (FillMode == SmartEraserFillMode.SolidColor || EdgePixels is not { Length: > 0 })
        {
            ReplaceFill(rectangle, new SolidColorBrush(Color.Parse(FillColor)));
            return;
        }

        int width = FillMode == SmartEraserFillMode.StretchVertically ? EdgePixels.Length : 1;
        int height = FillMode == SmartEraserFillMode.StretchHorizontally ? EdgePixels.Length : 1;
        using var edgeBitmap = new SKBitmap(width, height);

        for (int i = 0; i < EdgePixels.Length; i++)
        {
            int x = FillMode == SmartEraserFillMode.StretchVertically ? i : 0;
            int y = FillMode == SmartEraserFillMode.StretchHorizontally ? i : 0;
            edgeBitmap.SetPixel(x, y, UnpackColor(EdgePixels[i]));
        }

        ReplaceFill(rectangle, new ImageBrush(BitmapConversionHelpers.ToAvaloniBitmap(edgeBitmap))
        {
            Stretch = Stretch.Fill,
            SourceRect = new RelativeRect(0, 0, 1, 1, RelativeUnit.Relative)
        });
        RenderOptions.SetBitmapInterpolationMode(rectangle, BitmapInterpolationMode.None);
    }
}
