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

namespace ShareX.PhotoEditor.Core;

public sealed class RasterLayer : Layer
{
    public RasterLayer(string name, PixelSurface surface, SKPointI offset = default) : base(name)
    {
        Surface = surface;
        Offset = offset;
    }

    public override LayerKind Kind => LayerKind.Raster;

    public PixelSurface Surface { get; set; }

    /// <summary>Position of the surface's top-left pixel in document coordinates.</summary>
    public SKPointI Offset { get; set; }

    public SKRectI SurfaceBounds => SKRectI.Create(Offset.X, Offset.Y, Surface.Width, Surface.Height);

    public override IEnumerable<PixelSurface> Surfaces => Mask != null ? [Surface, Mask] : [Surface];

    public override SKRect GetContentBounds() => SurfaceBounds;

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        canvas.DrawBitmap(Surface.Bitmap, Offset.X, Offset.Y);
    }

    protected override Layer CreateShallowCopy() => new RasterLayer(Name, Surface, Offset);

    protected override void DetachSurfaces()
    {
        base.DetachSurfaces();
        Surface = Surface.Clone();
    }

    /// <summary>Returns the surface for writing, copying it first when it is shared with history.</summary>
    public PixelSurface GetWritableSurface()
    {
        if (Surface.Frozen) Surface = Surface.Clone();
        return Surface;
    }

    /// <summary>Grows the surface so that the given document rectangle is backed by pixels.</summary>
    public void EnsureCovers(SKRectI documentRect)
    {
        SKRectI current = SurfaceBounds;
        SKRectI union = SKRectI.Union(current, documentRect);
        if (union == current)
        {
            GetWritableSurface();
            return;
        }
        PixelSurface grown = PixelSurface.CreateColor(union.Width, union.Height);
        using (SKCanvas canvas = new(grown.Bitmap))
        {
            canvas.DrawBitmap(Surface.Bitmap, current.Left - union.Left, current.Top - union.Top);
        }
        Surface = grown;
        Offset = new SKPointI(union.Left, union.Top);
    }

    public override void Translate(float dx, float dy)
    {
        Offset = new SKPointI(Offset.X + (int)MathF.Round(dx), Offset.Y + (int)MathF.Round(dy));
    }

    /// <summary>Computes the bounds of non-transparent pixels in document coordinates.</summary>
    public SKRectI GetOpaqueBounds()
    {
        SKRectI local = ImageOps.GetOpaqueBounds(Surface.Bitmap);
        if (local.IsEmpty) return SKRectI.Empty;
        local.Offset(Offset.X, Offset.Y);
        return local;
    }
}
