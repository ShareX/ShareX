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

public sealed class GroupLayer : Layer
{
    public GroupLayer(string name) : base(name)
    {
        BlendMode = LayerBlendMode.PassThrough;
    }

    public override LayerKind Kind => LayerKind.Group;

    public List<Layer> Children { get; } = [];

    public bool Expanded { get; set; } = true;

    /// <summary>When set, the group is an artboard that clips its content and has its own background.</summary>
    public SKRectI? ArtboardBounds { get; set; }

    public SKColor ArtboardBackground { get; set; } = SKColors.White;

    public bool IsArtboard => ArtboardBounds != null;

    public override IEnumerable<PixelSurface> Surfaces => base.Surfaces.Concat(Children.SelectMany(c => c.Surfaces));

    public void Add(Layer layer, int? index = null)
    {
        layer.Parent?.Children.Remove(layer);
        layer.Parent = this;
        if (index is int i) Children.Insert(Math.Clamp(i, 0, Children.Count), layer);
        else Children.Add(layer);
    }

    public void Remove(Layer layer)
    {
        if (Children.Remove(layer)) layer.Parent = null;
    }

    protected override Layer CreateShallowCopy()
    {
        GroupLayer copy = new(Name)
        {
            Expanded = Expanded,
            ArtboardBounds = ArtboardBounds,
            ArtboardBackground = ArtboardBackground
        };
        foreach (Layer child in Children) copy.Add(child.CloneShared());
        return copy;
    }

    protected override void DetachSurfaces()
    {
        base.DetachSurfaces();
        List<Layer> children = Children.ToList();
        Children.Clear();
        foreach (Layer child in children) Add(child.Duplicate());
    }

    public override void Translate(float dx, float dy)
    {
        foreach (Layer child in Children) child.Translate(dx, dy);
        if (ArtboardBounds is SKRectI bounds)
        {
            bounds.Offset((int)MathF.Round(dx), (int)MathF.Round(dy));
            ArtboardBounds = bounds;
        }
    }

    public override SKRect GetContentBounds()
    {
        if (ArtboardBounds is SKRectI artboard) return artboard;
        SKRect bounds = SKRect.Empty;
        foreach (Layer child in Children.Where(c => c.Visible && c is not AdjustmentLayer))
        {
            SKRect childBounds = child.GetContentBounds();
            bounds = bounds.IsEmpty ? childBounds : SKRect.Union(bounds, childBounds);
        }
        return bounds;
    }

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        // Groups are rendered by the compositor so that blend modes and adjustments apply per child.
    }

    public IEnumerable<Layer> Descendants()
    {
        foreach (Layer child in Children)
        {
            yield return child;
            if (child is GroupLayer group)
            {
                foreach (Layer descendant in group.Descendants()) yield return descendant;
            }
        }
    }
}
