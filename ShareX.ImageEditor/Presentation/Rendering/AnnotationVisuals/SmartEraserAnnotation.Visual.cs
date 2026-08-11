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
using ShareX.AvaloniaUI.Imaging;
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

    public void ApplyFill(Avalonia.Controls.Shapes.Rectangle rectangle)
    {
        ArgumentNullException.ThrowIfNull(rectangle);

        if (FillMode == SmartEraserFillMode.SolidColor || EdgePixels is not { Length: > 0 })
        {
            rectangle.Fill = new SolidColorBrush(Color.Parse(FillColor));
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

        rectangle.Fill = new ImageBrush(BitmapConversionHelpers.ToAvaloniBitmap(edgeBitmap))
        {
            Stretch = Stretch.Fill,
            SourceRect = new RelativeRect(0, 0, 1, 1, RelativeUnit.Relative)
        };
        RenderOptions.SetBitmapInterpolationMode(rectangle, BitmapInterpolationMode.None);
    }
}
