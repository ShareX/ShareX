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

using Avalonia.Controls;
using Avalonia.Media.Imaging;
using static ShareX.ImageEditor.Presentation.Rendering.AnnotationVisualHelpers;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class ImageAnnotation
{
    internal virtual void UpdateVisual(Image imageControl, bool useInteractiveRender = false)
    {
        if (ImageBitmap != null)
        {
            ReplaceImageSource(imageControl, BitmapConversionHelpers.ToAvaloniBitmap(ImageBitmap));
        }

        var imageBounds = GetBounds();
        Canvas.SetLeft(imageControl, imageBounds.Left);
        Canvas.SetTop(imageControl, imageBounds.Top);
        imageControl.Width = Math.Max(1, imageBounds.Width);
        imageControl.Height = Math.Max(1, imageBounds.Height);
        ApplyRotationTransform(imageControl, RotationAngle);
    }

    protected static void ReplaceImageSource(Image imageControl, Bitmap? bitmapSource)
    {
        var previousSource = imageControl.Source;
        if (ReferenceEquals(previousSource, bitmapSource)) return;

        imageControl.Source = bitmapSource;
        (previousSource as IDisposable)?.Dispose();
    }

    public virtual Control CreateVisual()
    {
        var image = new Image
        {
            Tag = this
        };

        if (ImageBitmap != null)
        {
            ReplaceImageSource(image, BitmapConversionHelpers.ToAvaloniBitmap(ImageBitmap));
        }

        var imageBounds = GetBounds();
        image.Width = Math.Max(1, imageBounds.Width);
        image.Height = Math.Max(1, imageBounds.Height);
        ApplyRotationTransform(image, RotationAngle);

        return image;
    }
}
