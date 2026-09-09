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

using SkiaSharp;

namespace ShareX.ImageEditor.Core.Annotations;

/// <summary>
/// Image annotation - stickers or inserted images
/// </summary>
public partial class ImageAnnotation : Annotation, IDisposable
{
    public override AnnotationCategory Category => AnnotationCategory.Shapes;
    private SKBitmap? _imageBitmap;
    internal bool IsDisposed { get; private set; }

    /// <summary>
    /// File path to the image (if external)
    /// </summary>
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The loaded image bitmap
    /// </summary>
    public SKBitmap? ImageBitmap => _imageBitmap;

    public ImageAnnotation()
    {
        ToolType = EditorTool.Image;
        StrokeWidth = 0; // Usually no border
    }

    public void LoadImage(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                ImagePath = path;
                _imageBitmap?.Dispose();
                _imageBitmap = SKBitmap.Decode(path);
            }
            catch { }
        }
    }

    public void SetImage(SKBitmap bitmap)
    {
        if (ReferenceEquals(_imageBitmap, bitmap)) return;
        _imageBitmap?.Dispose();
        _imageBitmap = bitmap;
    }

    public void ClearImage()
    {
        _imageBitmap?.Dispose();
        _imageBitmap = null;
    }

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        var bounds = GetBounds();

        point = GetUnrotatedPoint(point, bounds);

        var inflated = SKRect.Inflate(bounds, tolerance, tolerance);
        return inflated.Contains(point);
    }

    /// <summary>
    /// Dispose unmanaged resources (ImageBitmap)
    /// </summary>
    public void Dispose()
    {
        IsDisposed = true;
        _imageBitmap?.Dispose();
        _imageBitmap = null;
        GC.SuppressFinalize(this);
    }

    public override Annotation Clone()
    {
        var clone = (ImageAnnotation)base.Clone();
        // Deep-copy bitmap for undo/redo to properly preserve image data
        clone.IsDisposed = false;
        clone._imageBitmap = _imageBitmap?.Copy();
        return clone;
    }
}