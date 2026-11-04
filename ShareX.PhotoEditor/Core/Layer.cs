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

public enum LayerKind
{
    Raster,
    Text,
    Shape,
    Group,
    Adjustment,
    Fill,
    SmartObject,
    Frame
}

/// <summary>Base class for every layer type in a document.</summary>
public abstract class Layer
{
    private static int _nextId;

    protected Layer(string name)
    {
        Id = Interlocked.Increment(ref _nextId);
        Name = name;
    }

    /// <summary>Stable identity, shared by history copies of the same layer.</summary>
    public int Id { get; private set; }

    public string Name { get; set; }

    public abstract LayerKind Kind { get; }

    public bool Visible { get; set; } = true;

    public float Opacity { get; set; } = 1;

    /// <summary>Fill opacity scales layer content but not layer style effects.</summary>
    public float FillOpacity { get; set; } = 1;

    public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.Normal;

    /// <summary>The layer is clipped to the nearest unclipped layer below it.</summary>
    public bool IsClipped { get; set; }

    public bool LockTransparency { get; set; }

    public bool LockPixels { get; set; }

    public bool LockPosition { get; set; }

    public bool LockAll { get; set; }

    public bool IsBackground { get; set; }

    /// <summary>Layers with the same non-zero link id move together (Layers panel link button).</summary>
    public int LinkId { get; set; }

    /// <summary>Layer mask in document coordinates. 255 reveals, 0 hides.</summary>
    public PixelSurface? Mask { get; set; }

    public bool MaskEnabled { get; set; } = true;

    public LayerStyle? Style { get; set; }

    public GroupLayer? Parent { get; internal set; }

    public SKColor? LabelColor { get; set; }

    public bool CanEditPixels => !LockAll && !LockPixels;

    public bool CanMove => !LockAll && !LockPosition;

    public virtual IEnumerable<PixelSurface> Surfaces => Mask != null ? [Mask] : [];

    /// <summary>Bounds of the visible content in document coordinates.</summary>
    public abstract SKRect GetContentBounds();

    /// <summary>Draws the layer content (without opacity, mask or style) in document coordinates.</summary>
    public abstract void RenderContent(SKCanvas canvas, RenderContext context);

    protected abstract Layer CreateShallowCopy();

    /// <summary>Copies layer properties while sharing pixel surfaces. Used by history snapshots.</summary>
    public Layer CloneShared()
    {
        Layer copy = CreateShallowCopy();
        CopyBaseTo(copy);
        copy.Id = Id;
        return copy;
    }

    /// <summary>Creates an independent duplicate with a new identity.</summary>
    public Layer Duplicate()
    {
        Layer copy = CloneShared();
        copy.Id = Interlocked.Increment(ref _nextId);
        copy.LinkId = 0;
        copy.DetachSurfaces();
        return copy;
    }

    protected virtual void DetachSurfaces()
    {
        if (Mask != null) Mask = Mask.Clone();
    }

    private void CopyBaseTo(Layer copy)
    {
        copy.Name = Name;
        copy.Visible = Visible;
        copy.Opacity = Opacity;
        copy.FillOpacity = FillOpacity;
        copy.BlendMode = BlendMode;
        copy.IsClipped = IsClipped;
        copy.LockTransparency = LockTransparency;
        copy.LockPixels = LockPixels;
        copy.LockPosition = LockPosition;
        copy.LockAll = LockAll;
        copy.IsBackground = IsBackground;
        copy.LinkId = LinkId;
        copy.Mask = Mask;
        copy.MaskEnabled = MaskEnabled;
        copy.Style = Style?.Clone();
        copy.LabelColor = LabelColor;
    }

    public void FreezeSurfaces()
    {
        foreach (PixelSurface surface in Surfaces) surface.Freeze();
    }

    /// <summary>Returns a writable mask, copying it first if it is shared with history.</summary>
    public PixelSurface GetWritableMask(int width, int height)
    {
        if (Mask == null) Mask = PixelSurface.CreateMask(width, height, 255);
        else if (Mask.Frozen) Mask = Mask.Clone();
        return Mask;
    }

    /// <summary>Offsets the layer content by a whole-pixel or sub-pixel amount.</summary>
    public virtual void Translate(float dx, float dy)
    {
    }

    public bool IsEffectivelyVisible
    {
        get
        {
            for (Layer? layer = this; layer != null; layer = layer.Parent)
            {
                if (!layer.Visible) return false;
            }
            return true;
        }
    }

    public override string ToString() => Name;
}

public sealed class RenderContext
{
    public RenderContext(int documentWidth, int documentHeight)
    {
        DocumentWidth = documentWidth;
        DocumentHeight = documentHeight;
    }

    public int DocumentWidth { get; }

    public int DocumentHeight { get; }

    public SKRectI Area { get; set; }

    public SKSamplingOptions Sampling { get; set; } = new(SKCubicResampler.Mitchell);
}
