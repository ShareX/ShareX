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

/// <summary>Which cels an operation applies to.</summary>
public enum CelScope
{
    Cel,
    SelectedFrames,
    Layer,
    Sprite
}

/// <summary>Operations on whole sprites, layers and groups of cels.</summary>
public static class SpriteOps
{
    public static IEnumerable<(PixelLayer Layer, int Frame)> Cels(Sprite sprite, CelScope scope)
    {
        if (scope == CelScope.Sprite)
        {
            foreach (PixelLayer layer in sprite.PixelLayers)
                for (int f = 0; f < sprite.Frames.Count; f++)
                    yield return (layer, f);
            yield break;
        }
        if (sprite.ActiveLayer is not PixelLayer active) yield break;
        int from = scope == CelScope.Cel ? sprite.ActiveFrame : scope == CelScope.SelectedFrames ? sprite.RangeFrom : 0;
        int to = scope == CelScope.Cel ? sprite.ActiveFrame : scope == CelScope.SelectedFrames ? sprite.RangeTo : sprite.Frames.Count - 1;
        for (int f = from; f <= to; f++) yield return (active, f);
    }

    /// <summary>Runs an effect over the cels of a scope, once per shared image, skipping locked layers. Returns whether anything changed.</summary>
    public static bool Apply(Sprite sprite, CelScope scope, Func<PixelBuffer, bool[]?, PixelBuffer> effect)
    {
        HashSet<CelImage> done = [];
        bool changed = false;
        bool[]? mask = sprite.Selection?.Data;
        foreach ((PixelLayer layer, int frame) in Cels(sprite, scope).ToList())
        {
            if (layer.IsEffectivelyLocked) continue;
            Cel? cel = sprite.GetCel(layer, frame);
            if (cel == null || !done.Add(cel.Image)) continue;
            PixelBuffer result = effect(cel.Buffer, mask);
            if (result.ContentEquals(cel.Buffer)) continue;
            sprite.SetBuffer(layer, frame, result);
            changed = true;
        }
        return changed;
    }

    public static void Resize(Sprite sprite, int width, int height, ResizeMethod method)
    {
        int oldW = sprite.Width, oldH = sprite.Height;
        sprite.ReplaceContent(width, height, b => Effects.Resize(b, width, height, method), _ => null);
        foreach (Slice slice in sprite.Slices)
        {
            SKRectI b = slice.Bounds;
            slice.Bounds = new SKRectI(b.Left * width / oldW, b.Top * height / oldH, Math.Max(b.Left * width / oldW + 1, b.Right * width / oldW), Math.Max(b.Top * height / oldH + 1, b.Bottom * height / oldH));
        }
    }

    /// <summary>Changes the canvas size; the old content is placed at the given offset.</summary>
    public static void ResizeCanvas(Sprite sprite, int width, int height, int offsetX, int offsetY)
    {
        sprite.ReplaceContent(width, height, b =>
        {
            PixelBuffer result = new(width, height);
            result.Blit(b, offsetX, offsetY, false);
            return result;
        }, s => s?.Resized(width, height, offsetX, offsetY));
        foreach (Slice slice in sprite.Slices)
        {
            SKRectI b = slice.Bounds;
            b.Offset(offsetX, offsetY);
            slice.Bounds = b;
        }
    }

    public static void Crop(Sprite sprite, SKRectI area) => ResizeCanvas(sprite, area.Width, area.Height, -area.Left, -area.Top);

    /// <summary>The bounds of everything drawn in any cel, or null when the sprite is empty.</summary>
    public static SKRectI? ContentBounds(Sprite sprite)
    {
        SKRectI? bounds = null;
        HashSet<PixelBuffer> seen = [];
        foreach (PixelLayer layer in sprite.PixelLayers)
        {
            foreach (Cel? cel in layer.Cels)
            {
                if (cel == null || !seen.Add(cel.Buffer)) continue;
                if (cel.Buffer.ContentBounds() is SKRectI b) bounds = bounds is SKRectI current ? SKRectI.Union(current, b) : b;
            }
        }
        return bounds;
    }

    public static void Flip(Sprite sprite, bool horizontal)
    {
        sprite.ReplaceContent(sprite.Width, sprite.Height, b => horizontal ? b.FlipHorizontal() : b.FlipVertical(), _ => null);
    }

    public static void Rotate(Sprite sprite, int quarterTurns)
    {
        bool swap = (quarterTurns & 1) != 0;
        sprite.ReplaceContent(swap ? sprite.Height : sprite.Width, swap ? sprite.Width : sprite.Height, b => b.Rotate90(quarterTurns), _ => null);
        sprite.Slices.Clear();
    }

    /// <summary>Merges a layer into the pixel layer below it. Returns the merged layer, or null when there is nothing to merge into.</summary>
    public static PixelLayer? MergeDown(Sprite sprite, Layer upper)
    {
        GroupLayer? parent = upper.Parent;
        if (parent == null) return null;
        int index = parent.Children.IndexOf(upper);
        if (index <= 0 || parent.Children[index - 1] is not PixelLayer lower) return null;
        for (int f = 0; f < sprite.Frames.Count; f++)
        {
            PixelBuffer result = new(sprite.Width, sprite.Height);
            Cel? below = lower.Cels[f];
            if (below != null) Compositor.BlendBuffer(result, below.Buffer, result.Bounds, below.Opacity, BlendMode.Normal);
            ComposeLayer(sprite, upper, f, result);
            lower.Cels[f] = result.IsEmpty ? null : new Cel(new CelImage(result));
        }
        sprite.RemoveLayer(upper);
        sprite.ActiveLayer = lower;
        return lower;
    }

    private static void ComposeLayer(Sprite sprite, Layer layer, int frame, PixelBuffer target)
    {
        if (layer is PixelLayer pixels)
        {
            Cel? cel = pixels.Cels[frame];
            if (cel != null) Compositor.BlendBuffer(target, cel.Buffer, target.Bounds, (pixels.Opacity * cel.Opacity + 127) / 255, pixels.Blend);
        }
        else if (layer is GroupLayer group)
        {
            PixelBuffer temp = new(sprite.Width, sprite.Height);
            foreach (Layer child in group.Children)
                if (child.Visible) ComposeLayer(sprite, child, frame, temp);
            Compositor.BlendBuffer(target, temp, target.Bounds, group.Opacity, group.Blend);
        }
    }

    /// <summary>Replaces a group by one pixel layer holding its flattened content.</summary>
    public static PixelLayer FlattenGroup(Sprite sprite, GroupLayer group)
    {
        PixelLayer flat = sprite.CreatePixelLayer(group.Name);
        flat.Visible = group.Visible;
        for (int f = 0; f < sprite.Frames.Count; f++)
        {
            PixelBuffer result = new(sprite.Width, sprite.Height);
            ComposeLayer(sprite, group, f, result);
            flat.Cels[f] = result.IsEmpty ? null : new Cel(new CelImage(result));
        }
        GroupLayer parent = group.Parent ?? sprite.Root;
        int index = parent.Children.IndexOf(group);
        sprite.RemoveLayer(group);
        sprite.InsertLayer(flat, parent, index);
        sprite.ActiveLayer = flat;
        return flat;
    }

    /// <summary>Flattens all visible layers into a single layer.</summary>
    public static void Flatten(Sprite sprite, string layerName)
    {
        PixelLayer flat = sprite.CreatePixelLayer(layerName);
        for (int f = 0; f < sprite.Frames.Count; f++)
        {
            PixelBuffer result = Compositor.Flatten(sprite, f);
            flat.Cels[f] = result.IsEmpty ? null : new Cel(new CelImage(result));
        }
        foreach (Layer layer in sprite.Root.Children.ToList()) sprite.RemoveLayer(layer);
        sprite.AddLayer(flat);
        sprite.ActiveLayer = flat;
    }

    public static Layer Duplicate(Sprite sprite, Layer layer, string name)
    {
        Layer copy = layer.Clone([]);
        sprite.AssignNewIds(copy);
        copy.Name = name;
        sprite.AddLayer(copy, layer);
        sprite.ActiveLayer = copy;
        return copy;
    }

    /// <summary>The frames an animation range plays, in order, for one pass of the given direction.</summary>
    public static List<int> FrameSequence(int from, int to, TagDirection direction)
    {
        List<int> frames = [];
        for (int f = from; f <= to; f++) frames.Add(f);
        switch (direction)
        {
            case TagDirection.Reverse:
                frames.Reverse();
                break;
            case TagDirection.PingPong:
                for (int f = to - 1; f > from; f--) frames.Add(f);
                break;
            case TagDirection.PingPongReverse:
                frames.Reverse();
                for (int f = from + 1; f < to; f++) frames.Add(f);
                break;
        }
        return frames;
    }

    /// <summary>Makes a cel independent of the cels it was linked to.</summary>
    public static void Unlink(Sprite sprite, PixelLayer layer, int frame)
    {
        Cel? cel = sprite.GetCel(layer, frame);
        if (cel == null) return;
        cel.Buffer.Frozen = true;
        cel.Image = new CelImage(cel.Buffer);
        sprite.Touch();
    }

    /// <summary>Links the cels of a frame range to the first one, so they share its image.</summary>
    public static void Link(Sprite sprite, PixelLayer layer, int from, int to)
    {
        Cel? source = null;
        for (int f = from; f <= to; f++)
        {
            source = sprite.GetCel(layer, f);
            if (source != null) break;
        }
        if (source == null) return;
        for (int f = from; f <= to; f++) layer.Cels[f] = new Cel(source.Image) { Opacity = source.Opacity };
        sprite.Touch();
    }
}
