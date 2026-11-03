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
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ShareX.PhotoEditor.UI;

/// <summary>
/// View state of an open document: zoom, pan, view rotation and the cached composite used for display and sampling.
/// Coordinate systems: document pixels (SKPoint) and canvas control pixels (Avalonia Point).
/// </summary>
public sealed class DocumentView : IDisposable
{
    public static readonly double[] ZoomSteps = [0.01, 0.02, 0.03, 0.04, 0.05, 0.0625, 0.0833, 0.125, 0.1667, 0.25, 0.3333, 0.5, 0.6667, 1, 2, 3, 4, 5, 6, 7, 8, 12, 16, 24, 32];

    private SKRectI? _dirty;

    public DocumentView(PhotoDocument document)
    {
        Document = document;
        Document.Invalidated += area => MarkDirty(area);
    }

    public PhotoDocument Document { get; }

    public double Zoom { get; set; } = 1;

    /// <summary>View rotation in degrees (Rotate View tool); does not change pixels.</summary>
    public double Rotation { get; set; }

    /// <summary>Offset of the document center from the viewport center, in screen pixels.</summary>
    public Vector Pan { get; set; }

    public Size Viewport { get; set; } = new(800, 600);

    /// <summary>Device pixels per layout unit of the window showing this view.</summary>
    public double RenderScaling { get; set; } = 1;

    public double AntsPhase { get; set; }

    /// <summary>Flattened image of all visible layers at document size.</summary>
    public SKBitmap? Composite { get; private set; }

    public WriteableBitmap? Display { get; private set; }

    public bool NeedsFit { get; set; } = true;

    public Matrix DocToScreenMatrix
    {
        get
        {
            Matrix matrix = Matrix.CreateTranslation(-Document.Width / 2.0, -Document.Height / 2.0) *
                Matrix.CreateScale(Zoom, Zoom) *
                Matrix.CreateRotation(Rotation * Math.PI / 180) *
                Matrix.CreateTranslation(Viewport.Width / 2 + Pan.X, Viewport.Height / 2 + Pan.Y);
            return Rotation % 90 == 0 ? SnapToPixels(matrix) : matrix;
        }
    }

    /// <summary>Puts the document edges on device pixels. Edges between pixels get antialiased, letting the transparency checker show through as a faint line.</summary>
    private Matrix SnapToPixels(Matrix matrix)
    {
        double Snap(double value) => Math.Floor(value * RenderScaling + 0.5) / RenderScaling;

        Rect bounds = new Rect(0, 0, Document.Width, Document.Height).TransformToAABB(matrix);
        double left = Snap(bounds.Left), top = Snap(bounds.Top);
        double width = Math.Max(Snap(bounds.Right) - left, 1 / RenderScaling), height = Math.Max(Snap(bounds.Bottom) - top, 1 / RenderScaling);
        return matrix * Matrix.CreateTranslation(-bounds.Left, -bounds.Top) * Matrix.CreateScale(width / bounds.Width, height / bounds.Height) * Matrix.CreateTranslation(left, top);
    }

    public Point DocToScreen(SKPoint p) => new Point(p.X, p.Y).Transform(DocToScreenMatrix);

    public SKPoint ScreenToDoc(Point p)
    {
        Matrix inverse = DocToScreenMatrix.Invert();
        Point d = p.Transform(inverse);
        return new SKPoint((float)d.X, (float)d.Y);
    }

    public void MarkDirty(SKRectI? area)
    {
        SKRectI rect = area ?? Document.Bounds;
        if (Composite == null || Composite.Width != Document.Width || Composite.Height != Document.Height) rect = Document.Bounds;
        _dirty = _dirty is SKRectI existing ? SKRectI.Union(existing, rect) : rect;
    }

    public bool IsDirty => _dirty != null;

    /// <summary>Re-composites dirty regions and copies them into the display bitmap.</summary>
    public void Update(bool showQuickMask)
    {
        if (_dirty is not SKRectI dirty) return;
        _dirty = null;
        bool sizeChanged = Composite == null || Composite.Width != Document.Width || Composite.Height != Document.Height;
        if (sizeChanged)
        {
            Composite?.Dispose();
            Composite = ImageOps.CreateBitmap(Document.Width, Document.Height);
            Composite.Erase(SKColors.Transparent);
            Display?.Dispose();
            Display = new WriteableBitmap(new PixelSize(Document.Width, Document.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
            dirty = Document.Bounds;
        }
        dirty.Intersect(Document.Bounds);
        if (dirty.IsEmpty) return;
        Compositor.Render(Document, Composite!, dirty);
        CopyToDisplay(dirty, showQuickMask);
    }

    /// <summary>Copies composite pixels to the display, applying channel view and quick mask overlays.</summary>
    public unsafe void CopyToDisplay(SKRectI area, bool showQuickMask)
    {
        if (Composite == null || Display == null) return;
        area.Intersect(Document.Bounds);
        if (area.IsEmpty) return;
        using ILockedFramebuffer buffer = Display.Lock();
        uint* dst = (uint*)buffer.Address;
        int dstStride = buffer.RowBytes / 4;
        uint* src = (uint*)Composite.GetPixels();
        int srcStride = Composite.RowBytes / 4;
        ChannelView channel = Document.ChannelView;
        PixelSurface? quickMask = showQuickMask && Document.QuickMask ? Document.Selection : null;
        bool quickMaskEmpty = showQuickMask && Document.QuickMask && Document.Selection == null;
        AlphaChannel? visibleChannel = Document.Channels.FirstOrDefault(c => c.Visible);
        int left = area.Left, right = area.Right;

        Parallel.For(area.Top, area.Bottom, y =>
        {
            uint* s = src + y * srcStride;
            uint* d = dst + y * dstStride;
            for (int x = left; x < right; x++)
            {
                uint p = s[x];
                if (channel != ChannelView.Composite)
                {
                    ColorMath.Unpack(p, out float r, out float g, out float b, out float a);
                    float v = channel switch { ChannelView.Red => r, ChannelView.Green => g, _ => b } * a;
                    uint c = (uint)(v * 255);
                    p = 0xFF000000u | (c << 16) | (c << 8) | c;
                }
                if (quickMask != null || quickMaskEmpty)
                {
                    // Unselected areas are tinted red at 50%.
                    float coverage = quickMask != null ? MaskOps.Get(quickMask, x, y) / 255f : 1;
                    float tint = (1 - coverage) * 0.5f;
                    if (tint > 0) p = BlendModes.CompositePixel(p, 1, 0, 0, tint, LayerBlendMode.Normal);
                }
                if (visibleChannel != null)
                {
                    float coverage = MaskOps.Get(visibleChannel.Mask, x, y) / 255f;
                    float tint = (1 - coverage) * 0.5f;
                    if (tint > 0) p = BlendModes.CompositePixel(p, 1, 0, 0, tint, LayerBlendMode.Normal);
                }
                d[x] = p;
            }
        });
    }

    public void ZoomAt(double newZoom, Point anchor)
    {
        newZoom = Math.Clamp(newZoom, 0.005, 64);
        SKPoint docPoint = ScreenToDoc(anchor);
        Zoom = newZoom;
        Point after = DocToScreen(docPoint);
        Pan += anchor - after;
    }

    public void ZoomStep(bool zoomIn, Point anchor)
    {
        double next = zoomIn
            ? ZoomSteps.FirstOrDefault(z => z > Zoom * 1.001, ZoomSteps[^1])
            : ZoomSteps.LastOrDefault(z => z < Zoom / 1.001, ZoomSteps[0]);
        ZoomAt(next, anchor);
    }

    public void FitOnScreen()
    {
        if (Viewport.Width <= 0 || Viewport.Height <= 0) return;
        double zoom = Math.Min((Viewport.Width - 40) / Document.Width, (Viewport.Height - 40) / Document.Height);
        Zoom = Math.Clamp(zoom, 0.005, 16);
        Pan = default;
        Rotation = 0;
        NeedsFit = false;
    }

    /// <summary>First view of a document: large images shrink to fit the viewport, smaller ones stay at 100%.</summary>
    public void FitInitial()
    {
        FitOnScreen();
        Zoom = Math.Min(Zoom, 1);
    }

    public void FillScreen()
    {
        double zoom = Math.Max(Viewport.Width / Document.Width, Viewport.Height / Document.Height);
        Zoom = Math.Clamp(zoom, 0.005, 64);
        Pan = default;
    }

    public void ActualPixels()
    {
        Zoom = 1;
        Pan = default;
    }

    public void Dispose()
    {
        Composite?.Dispose();
        Display?.Dispose();
    }
}
