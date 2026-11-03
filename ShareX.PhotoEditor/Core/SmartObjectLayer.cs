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

/// <summary>Embedded source pixels displayed through a non-destructive transform.</summary>
public sealed class SmartObjectLayer : Layer
{
    public SmartObjectLayer(string name, PixelSurface source) : base(name)
    {
        Source = source;
    }

    public override LayerKind Kind => LayerKind.SmartObject;

    public PixelSurface Source { get; set; }

    public SKMatrix Transform { get; set; } = SKMatrix.Identity;

    /// <summary>Non-destructive filters re-applied to the source whenever the layer is rendered.</summary>
    public List<SmartFilter> Filters { get; } = [];

    private SKBitmap? _filtered;
    private string? _filterKey;

    public override IEnumerable<PixelSurface> Surfaces => Mask != null ? [Source, Mask] : [Source];

    protected override Layer CreateShallowCopy()
    {
        SmartObjectLayer copy = new(Name, Source) { Transform = Transform };
        copy.Filters.AddRange(Filters.Select(f => f.Clone()));
        return copy;
    }

    protected override void DetachSurfaces()
    {
        base.DetachSurfaces();
        Source = Source.Clone();
    }

    public override void Translate(float dx, float dy) => Transform = Transform.PostConcat(SKMatrix.CreateTranslation(dx, dy));

    public SKPoint[] GetCorners()
    {
        SKPoint[] corners = [new(0, 0), new(Source.Width, 0), new(Source.Width, Source.Height), new(0, Source.Height)];
        return Transform.MapPoints(corners);
    }

    public override SKRect GetContentBounds() => Transform.MapRect(SKRect.Create(0, 0, Source.Width, Source.Height));

    public SKBitmap GetFilteredSource()
    {
        if (Filters.Count == 0) return Source.Bitmap;
        string key = string.Join("|", Filters.Select(f => (f.Enabled ? "+" : "-") + f.Key)) + "|" + System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(Source);
        if (_filtered == null || _filterKey != key)
        {
            _filtered?.Dispose();
            SKBitmap current = Source.Bitmap.Copy();
            foreach (SmartFilter filter in Filters.Where(f => f.Enabled))
            {
                SKBitmap next = filter.Apply(current);
                if (!ReferenceEquals(next, current)) current.Dispose();
                current = next;
            }
            _filtered = current;
            _filterKey = key;
        }
        return _filtered;
    }

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        canvas.Save();
        canvas.Concat(Transform);
        using SKImage image = SKImage.FromBitmap(GetFilteredSource());
        using SKPaint paint = new() { IsAntialias = true };
        canvas.DrawImage(image, 0, 0, context.Sampling, paint);
        canvas.Restore();
    }
}

/// <summary>A filter stored on a smart object and applied non-destructively.</summary>
public sealed class SmartFilter
{
    public SmartFilter(string name, string key, Func<SKBitmap, SKBitmap> apply)
    {
        Name = name;
        Key = key;
        Apply = apply;
    }

    public string Name { get; }

    public string Key { get; }

    public bool Enabled { get; set; } = true;

    public Func<SKBitmap, SKBitmap> Apply { get; }

    /// <summary>Serializable description used to rebuild the filter when a project is loaded.</summary>
    public SmartFilterRecipe? Recipe { get; init; }

    public SmartFilter Clone() => new(Name, Key, Apply) { Enabled = Enabled, Recipe = Recipe };
}

/// <summary>Serializable parameters of a smart filter. Which fields are used depends on <see cref="Kind"/>.</summary>
public sealed class SmartFilterRecipe
{
    public string Kind { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public Dictionary<string, float>? Values { get; set; }
    public int Seed { get; set; }
    public SKColor Foreground { get; set; } = SKColors.Black;
    public SKColor Background { get; set; } = SKColors.White;
    public AdjustmentSettings? Adjustment { get; set; }
    public Dictionary<string, string>? Library { get; set; }
}
