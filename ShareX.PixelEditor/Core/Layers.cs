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

namespace ShareX.PixelEditor.Core;

public enum BlendMode
{
    Normal,
    Darken,
    Multiply,
    ColorBurn,
    Lighten,
    Screen,
    ColorDodge,
    Addition,
    Overlay,
    SoftLight,
    HardLight,
    Difference,
    Exclusion,
    Subtract,
    Divide,
    Hue,
    Saturation,
    Color,
    Luminosity
}

/// <summary>Holds the pixels of a cel. Linked cels share one image, so painting on one changes them all.</summary>
public sealed class CelImage
{
    public CelImage(PixelBuffer buffer)
    {
        Buffer = buffer;
    }

    public PixelBuffer Buffer { get; set; }

    /// <summary>On a tilemap layer, the tile in each cell, row by row; null until the cel is split into tiles.</summary>
    public int[]? Map { get; set; }

    /// <summary>On a tilemap layer, the buffer last drawn from the tiles; a different buffer means the pixels were painted since.</summary>
    public PixelBuffer? Rendered { get; set; }

    /// <summary>A separate image with the same pixels and tiles; the pixels are frozen so either side copies them before painting.</summary>
    public CelImage Share()
    {
        Buffer.Frozen = true;
        return new CelImage(Buffer) { Map = Map, Rendered = Rendered };
    }
}

/// <summary>The content of one layer in one frame.</summary>
public sealed class Cel
{
    public Cel(CelImage image)
    {
        Image = image;
    }

    public CelImage Image { get; set; }

    public int Opacity { get; set; } = 255;

    public PixelBuffer Buffer => Image.Buffer;
}

public abstract class Layer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Visible { get; set; } = true;

    public bool Locked { get; set; }

    public int Opacity { get; set; } = 255;

    public BlendMode Blend { get; set; } = BlendMode.Normal;

    public GroupLayer? Parent { get; set; }

    public bool IsEffectivelyVisible => Visible && (Parent?.IsEffectivelyVisible ?? true);

    public bool IsEffectivelyLocked => Locked || (Parent?.IsEffectivelyLocked ?? false);

    /// <summary>Nesting level below the root group.</summary>
    public int Depth
    {
        get
        {
            int depth = -1;
            for (GroupLayer? p = Parent; p != null; p = p.Parent) depth++;
            return Math.Max(0, depth);
        }
    }

    public bool IsDescendantOf(GroupLayer group)
    {
        for (GroupLayer? p = Parent; p != null; p = p.Parent)
            if (p == group) return true;
        return false;
    }

    /// <summary>Copies the layer. Buffers are shared and frozen; cel images are copied through the map so linked cels stay linked.</summary>
    public abstract Layer Clone(Dictionary<CelImage, CelImage> map);

    protected void CopyTo(Layer target)
    {
        target.Id = Id;
        target.Name = Name;
        target.Visible = Visible;
        target.Locked = Locked;
        target.Opacity = Opacity;
        target.Blend = Blend;
    }
}

/// <summary>A layer of pixels with one optional cel per frame.</summary>
public sealed class PixelLayer : Layer
{
    public List<Cel?> Cels { get; } = [];

    /// <summary>Reference layers are shown while editing but left out of exports.</summary>
    public bool IsReference { get; set; }

    /// <summary>The tiles of a tilemap layer; null for a plain pixel layer.</summary>
    public Tileset? Tileset { get; set; }

    public override Layer Clone(Dictionary<CelImage, CelImage> map)
    {
        PixelLayer copy = new() { IsReference = IsReference, Tileset = Tileset?.Clone() };
        CopyTo(copy);
        foreach (Cel? cel in Cels)
        {
            if (cel == null)
            {
                copy.Cels.Add(null);
                continue;
            }
            if (!map.TryGetValue(cel.Image, out CelImage? image))
            {
                image = cel.Image.Share();
                map[cel.Image] = image;
            }
            copy.Cels.Add(new Cel(image) { Opacity = cel.Opacity });
        }
        return copy;
    }
}

/// <summary>A folder of layers; children are stored bottom to top.</summary>
public sealed class GroupLayer : Layer
{
    public List<Layer> Children { get; } = [];

    public bool Collapsed { get; set; }

    public override Layer Clone(Dictionary<CelImage, CelImage> map)
    {
        GroupLayer copy = new() { Collapsed = Collapsed };
        CopyTo(copy);
        foreach (Layer child in Children)
        {
            Layer childCopy = child.Clone(map);
            childCopy.Parent = copy;
            copy.Children.Add(childCopy);
        }
        return copy;
    }

    /// <summary>Every layer below this group in drawing order (bottom to top); a group comes after its children.</summary>
    public IEnumerable<Layer> Descendants()
    {
        foreach (Layer child in Children)
        {
            if (child is GroupLayer group)
                foreach (Layer nested in group.Descendants()) yield return nested;
            yield return child;
        }
    }

    /// <summary>The layers as listed in the interface: top to bottom, a group before its children, skipping collapsed groups.</summary>
    public IEnumerable<Layer> Rows(bool includeCollapsed = false)
    {
        for (int i = Children.Count - 1; i >= 0; i--)
        {
            Layer child = Children[i];
            yield return child;
            if (child is GroupLayer group && (includeCollapsed || !group.Collapsed))
                foreach (Layer nested in group.Rows(includeCollapsed)) yield return nested;
        }
    }
}

public sealed class FrameInfo
{
    /// <summary>How long the frame is shown, in milliseconds.</summary>
    public int Duration { get; set; } = 100;

    public FrameInfo Clone() => new() { Duration = Duration };
}

public enum TagDirection
{
    Forward,
    Reverse,
    PingPong,
    PingPongReverse
}

/// <summary>A named range of frames: one animation of the sprite.</summary>
public sealed class Tag
{
    public string Name { get; set; } = string.Empty;

    public int From { get; set; }

    public int To { get; set; }

    public uint Color { get; set; } = 0xFF4A90D9;

    public TagDirection Direction { get; set; }

    /// <summary>How many times the tag plays before the animation continues; 0 repeats forever.</summary>
    public int Repeat { get; set; }

    public Tag Clone() => new() { Name = Name, From = From, To = To, Color = Color, Direction = Direction, Repeat = Repeat };
}

/// <summary>A named rectangle of the sprite, exported with sprite sheets (hit boxes, UI parts, 9-slices).</summary>
public sealed class Slice
{
    public string Name { get; set; } = string.Empty;

    public SKRectI Bounds { get; set; }

    public uint Color { get; set; } = 0xFF3FB950;

    public Slice Clone() => new() { Name = Name, Bounds = Bounds, Color = Color };
}
