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

/// <summary>Everything the history saves of a sprite. Buffers are shared with the live sprite and frozen.</summary>
public sealed class SpriteState
{
    public int Width, Height;
    public GroupLayer Root = new();
    public List<FrameInfo> Frames = [];
    public List<Tag> Tags = [];
    public List<Slice> Slices = [];
    public List<uint> Palette = [];
    public SelectionMask? Selection;
    public int ActiveLayerId;
    public int ActiveFrame, RangeFrom, RangeTo;

    public IEnumerable<PixelBuffer> Buffers()
    {
        foreach (Layer layer in Root.Descendants())
        {
            if (layer is not PixelLayer pixels) continue;
            foreach (Cel? cel in pixels.Cels)
                if (cel != null) yield return cel.Buffer;
        }
    }
}

/// <summary>A pixel art document: layers and frames whose intersections are cels, plus tags, a palette and a selection.</summary>
public sealed class Sprite
{
    private int _nextLayerId = 1;

    public Sprite(int width, int height)
    {
        Width = Math.Clamp(width, 1, MaxSize);
        Height = Math.Clamp(height, 1, MaxSize);
        Frames.Add(new FrameInfo());
        History = new History(this);
    }

    public const int MaxSize = 8192;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public GroupLayer Root { get; private set; } = new();

    public List<FrameInfo> Frames { get; private set; } = [];

    public List<Tag> Tags { get; private set; } = [];

    public List<Slice> Slices { get; private set; } = [];

    public List<uint> Palette { get; set; } = [];

    public SelectionMask? Selection { get; set; }

    public Layer? ActiveLayer { get; set; }

    public int ActiveFrame { get; private set; }

    /// <summary>First and last frame of the range selected in the timeline; it always holds the active frame.</summary>
    public int RangeFrom { get; private set; }

    public int RangeTo { get; private set; }

    public string Title { get; set; } = string.Empty;

    public string? FilePath { get; set; }

    public History History { get; }

    public bool IsDirty => History.IsModified;

    /// <summary>Grows with every change; caches compare it to know when they are stale.</summary>
    public int Version { get; private set; }

    public SKRectI Bounds => new(0, 0, Width, Height);

    public string DisplayName => string.IsNullOrEmpty(FilePath) ? Title : Path.GetFileName(FilePath);

    public PixelLayer? ActivePixelLayer => ActiveLayer as PixelLayer;

    public IEnumerable<Layer> AllLayers => Root.Descendants();

    public IEnumerable<PixelLayer> PixelLayers => Root.Descendants().OfType<PixelLayer>();

    public void Touch() => Version++;

    // ------------------------------------------------------------------ frames

    public void SetActiveFrame(int frame, bool extendRange = false)
    {
        frame = Math.Clamp(frame, 0, Frames.Count - 1);
        if (extendRange)
        {
            int anchor = ActiveFrame == RangeFrom ? RangeTo : RangeFrom;
            RangeFrom = Math.Min(anchor, frame);
            RangeTo = Math.Max(anchor, frame);
        }
        else
        {
            RangeFrom = RangeTo = frame;
        }
        ActiveFrame = frame;
    }

    public void SetRange(int from, int to, int active)
    {
        RangeFrom = Math.Clamp(Math.Min(from, to), 0, Frames.Count - 1);
        RangeTo = Math.Clamp(Math.Max(from, to), 0, Frames.Count - 1);
        ActiveFrame = Math.Clamp(active, RangeFrom, RangeTo);
    }

    /// <summary>Inserts a frame. With a source frame its cels are copied, or linked when <paramref name="linked"/> is set.</summary>
    public void InsertFrame(int index, int? copyFrom = null, bool linked = false)
    {
        index = Math.Clamp(index, 0, Frames.Count);
        Frames.Insert(index, copyFrom is int source ? Frames[source].Clone() : new FrameInfo { Duration = Frames.Count > 0 ? Frames[Math.Clamp(index - 1, 0, Frames.Count - 1)].Duration : 100 });
        foreach (PixelLayer layer in PixelLayers)
        {
            Cel? cel = null;
            if (copyFrom is int from && layer.Cels[from] is Cel original)
            {
                original.Buffer.Frozen = true;
                cel = new Cel(linked ? original.Image : new CelImage(original.Buffer)) { Opacity = original.Opacity };
            }
            layer.Cels.Insert(index, cel);
        }
        foreach (Tag tag in Tags)
        {
            if (tag.From >= index) tag.From++;
            if (tag.To >= index) tag.To++;
        }
        Touch();
    }

    public void RemoveFrames(int from, int to)
    {
        from = Math.Clamp(from, 0, Frames.Count - 1);
        to = Math.Clamp(to, from, Frames.Count - 1);
        int count = to - from + 1;
        // A sprite always keeps one frame.
        if (count >= Frames.Count)
        {
            count = Frames.Count - 1;
            to = from + count - 1;
            foreach (PixelLayer layer in PixelLayers) layer.Cels[^1] = null;
            if (count <= 0)
            {
                Touch();
                return;
            }
        }
        Frames.RemoveRange(from, count);
        foreach (PixelLayer layer in PixelLayers) layer.Cels.RemoveRange(from, count);
        for (int i = Tags.Count - 1; i >= 0; i--)
        {
            Tag tag = Tags[i];
            int newFrom = tag.From > to ? tag.From - count : Math.Min(tag.From, from);
            int newTo = tag.To > to ? tag.To - count : tag.To >= from ? from - 1 : tag.To;
            if (newTo < newFrom) Tags.RemoveAt(i);
            else
            {
                tag.From = newFrom;
                tag.To = newTo;
            }
        }
        SetActiveFrame(Math.Min(from, Frames.Count - 1));
        Touch();
    }

    /// <summary>Moves a block of frames so it starts at <paramref name="target"/> (an index in the list without the block).</summary>
    public void MoveFrames(int from, int to, int target)
    {
        int count = to - from + 1;
        target = Math.Clamp(target, 0, Frames.Count - count);
        if (target == from) return;
        List<FrameInfo> block = Frames.GetRange(from, count);
        Frames.RemoveRange(from, count);
        Frames.InsertRange(target, block);
        foreach (PixelLayer layer in PixelLayers)
        {
            List<Cel?> cels = layer.Cels.GetRange(from, count);
            layer.Cels.RemoveRange(from, count);
            layer.Cels.InsertRange(target, cels);
        }
        SetRange(target, target + count - 1, target);
        Touch();
    }

    public void ReverseFrames(int from, int to)
    {
        Frames.Reverse(from, to - from + 1);
        foreach (PixelLayer layer in PixelLayers) layer.Cels.Reverse(from, to - from + 1);
        Touch();
    }

    public int TotalDuration => Frames.Sum(f => f.Duration);

    public Tag? TagAt(int frame) => Tags.Where(t => frame >= t.From && frame <= t.To).OrderBy(t => t.To - t.From).FirstOrDefault();

    // ------------------------------------------------------------------ layers

    public PixelLayer CreatePixelLayer(string name)
    {
        PixelLayer layer = new() { Id = _nextLayerId++, Name = name };
        for (int i = 0; i < Frames.Count; i++) layer.Cels.Add(null);
        return layer;
    }

    public GroupLayer CreateGroup(string name) => new() { Id = _nextLayerId++, Name = name };

    /// <summary>Adds a layer above <paramref name="above"/>, or on top of the root when it is null.</summary>
    public void AddLayer(Layer layer, Layer? above = null)
    {
        GroupLayer parent = above?.Parent ?? Root;
        int index = above != null ? parent.Children.IndexOf(above) + 1 : parent.Children.Count;
        InsertLayer(layer, parent, index);
    }

    public void InsertLayer(Layer layer, GroupLayer parent, int index)
    {
        layer.Parent = parent;
        parent.Children.Insert(Math.Clamp(index, 0, parent.Children.Count), layer);
        Touch();
    }

    public void RemoveLayer(Layer layer)
    {
        layer.Parent?.Children.Remove(layer);
        layer.Parent = null;
        if (ActiveLayer == layer || ActiveLayer != null && layer is GroupLayer group && ActiveLayer.IsDescendantOf(group))
        {
            ActiveLayer = Root.Rows(true).FirstOrDefault();
        }
        Touch();
    }

    /// <summary>Gives a copied layer tree fresh ids so it can live next to the original.</summary>
    public void AssignNewIds(Layer layer)
    {
        layer.Id = _nextLayerId++;
        if (layer is GroupLayer group)
            foreach (Layer child in group.Children) AssignNewIds(child);
    }

    public Layer? FindLayer(int id) => AllLayers.FirstOrDefault(l => l.Id == id);

    // ------------------------------------------------------------------ cels

    public Cel? GetCel(PixelLayer layer, int frame) => (uint)frame < (uint)layer.Cels.Count ? layer.Cels[frame] : null;

    public PixelBuffer? GetBuffer(PixelLayer layer, int frame) => GetCel(layer, frame)?.Buffer;

    /// <summary>The buffer to paint on: creates the cel when the frame has none and copies a buffer the history still shares.</summary>
    public PixelBuffer GetWritable(PixelLayer layer, int frame)
    {
        Cel? cel = layer.Cels[frame];
        if (cel == null)
        {
            cel = new Cel(new CelImage(new PixelBuffer(Width, Height)));
            layer.Cels[frame] = cel;
        }
        else if (cel.Buffer.Frozen)
        {
            cel.Image.Buffer = cel.Buffer.Clone();
        }
        Touch();
        return cel.Buffer;
    }

    /// <summary>Replaces the pixels of a cel (and of the cels linked to it); null or an empty buffer removes the cel.</summary>
    public void SetBuffer(PixelLayer layer, int frame, PixelBuffer? buffer)
    {
        Cel? cel = layer.Cels[frame];
        if (buffer == null)
        {
            layer.Cels[frame] = null;
        }
        else if (cel == null)
        {
            layer.Cels[frame] = new Cel(new CelImage(buffer));
        }
        else
        {
            cel.Image.Buffer = buffer;
        }
        Touch();
    }

    public bool IsLinked(PixelLayer layer, int frame)
    {
        Cel? cel = GetCel(layer, frame);
        return cel != null && layer.Cels.Count(c => c != null && c.Image == cel.Image) > 1;
    }

    // ------------------------------------------------------------------ history state

    public SpriteState Capture()
    {
        Dictionary<CelImage, CelImage> map = [];
        return new SpriteState
        {
            Width = Width,
            Height = Height,
            Root = (GroupLayer)Root.Clone(map),
            Frames = Frames.Select(f => f.Clone()).ToList(),
            Tags = Tags.Select(t => t.Clone()).ToList(),
            Slices = Slices.Select(s => s.Clone()).ToList(),
            Palette = [.. Palette],
            Selection = Selection,
            ActiveLayerId = ActiveLayer?.Id ?? 0,
            ActiveFrame = ActiveFrame,
            RangeFrom = RangeFrom,
            RangeTo = RangeTo
        };
    }

    public void Restore(SpriteState state)
    {
        Dictionary<CelImage, CelImage> map = [];
        Width = state.Width;
        Height = state.Height;
        Root = (GroupLayer)state.Root.Clone(map);
        Frames = state.Frames.Select(f => f.Clone()).ToList();
        Tags = state.Tags.Select(t => t.Clone()).ToList();
        Slices = state.Slices.Select(s => s.Clone()).ToList();
        Palette = [.. state.Palette];
        Selection = state.Selection;
        ActiveLayer = FindLayer(state.ActiveLayerId) ?? Root.Rows(true).FirstOrDefault();
        ActiveFrame = Math.Clamp(state.ActiveFrame, 0, Frames.Count - 1);
        RangeFrom = Math.Clamp(state.RangeFrom, 0, ActiveFrame);
        RangeTo = Math.Clamp(state.RangeTo, ActiveFrame, Frames.Count - 1);
        _nextLayerId = Math.Max(_nextLayerId, AllLayers.Select(l => l.Id).DefaultIfEmpty(0).Max() + 1);
        Touch();
    }

    /// <summary>Replaces the size and layer tree, for operations that rebuild every cel (resize, rotate, crop).</summary>
    public void ReplaceContent(int width, int height, Func<PixelBuffer, PixelBuffer> transform, Func<SelectionMask?, SelectionMask?>? selection = null)
    {
        Dictionary<CelImage, PixelBuffer> done = [];
        foreach (PixelLayer layer in PixelLayers)
        {
            foreach (Cel? cel in layer.Cels)
            {
                if (cel == null) continue;
                if (!done.TryGetValue(cel.Image, out PixelBuffer? result))
                {
                    result = transform(cel.Buffer);
                    done[cel.Image] = result;
                }
                cel.Image.Buffer = result;
            }
        }
        Width = Math.Clamp(width, 1, MaxSize);
        Height = Math.Clamp(height, 1, MaxSize);
        Selection = selection?.Invoke(Selection);
        Touch();
    }

    /// <summary>Adopts layers, frames and tags built by a file loader.</summary>
    public void Adopt(GroupLayer root, List<FrameInfo> frames, List<Tag> tags, List<Slice>? slices = null)
    {
        Root = root;
        Frames = frames.Count > 0 ? frames : [new FrameInfo()];
        Tags = tags;
        Slices = slices ?? [];
        foreach (Layer layer in AllLayers) layer.Id = _nextLayerId++;
        foreach (PixelLayer layer in PixelLayers)
        {
            while (layer.Cels.Count < Frames.Count) layer.Cels.Add(null);
            if (layer.Cels.Count > Frames.Count) layer.Cels.RemoveRange(Frames.Count, layer.Cels.Count - Frames.Count);
        }
        ActiveLayer = Root.Rows(true).FirstOrDefault(l => l is PixelLayer) ?? Root.Rows(true).FirstOrDefault();
        SetActiveFrame(0);
        Touch();
    }

    /// <summary>A new sprite with one transparent layer.</summary>
    public static Sprite Create(int width, int height, string title, string layerName, uint background = 0)
    {
        Sprite sprite = new(width, height) { Title = title };
        PixelLayer layer = sprite.CreatePixelLayer(layerName);
        sprite.AddLayer(layer);
        sprite.ActiveLayer = layer;
        if (background >> 24 != 0) sprite.GetWritable(layer, 0).Fill(background);
        return sprite;
    }

    /// <summary>A single-layer, single-frame sprite from an image.</summary>
    public static Sprite FromBuffer(PixelBuffer buffer, string title, string layerName)
    {
        Sprite sprite = new(buffer.Width, buffer.Height) { Title = title };
        PixelLayer layer = sprite.CreatePixelLayer(layerName);
        sprite.AddLayer(layer);
        sprite.ActiveLayer = layer;
        layer.Cels[0] = new Cel(new CelImage(buffer));
        return sprite;
    }
}

/// <summary>Undo history made of whole-sprite states that share their unchanged buffers.</summary>
public sealed class History
{
    private readonly Sprite _sprite;
    private readonly List<(string Name, SpriteState State)> _entries = [];
    private SpriteState? _saved;

    public History(Sprite sprite)
    {
        _sprite = sprite;
    }

    public int MaxStates { get; set; } = 100;

    /// <summary>Memory the buffers of all states may use before the oldest states are dropped.</summary>
    public long MemoryBudget { get; set; } = 768L * 1024 * 1024;

    public int Index { get; private set; } = -1;

    public int Count => _entries.Count;

    public IEnumerable<string> Names => _entries.Select(e => e.Name);

    public bool CanUndo => Index > 0;

    public bool CanRedo => Index < _entries.Count - 1;

    public string? UndoName => CanUndo ? _entries[Index].Name : null;

    public string? RedoName => CanRedo ? _entries[Index + 1].Name : null;

    public bool IsModified => Index >= 0 && !ReferenceEquals(_entries[Index].State, _saved);

    /// <summary>The state the sprite is at; it is a different object after every change, undo or redo.</summary>
    public object? CurrentState => Index >= 0 ? _entries[Index].State : null;

    public event Action? Changed;

    public void Initialize(string name)
    {
        _entries.Clear();
        _entries.Add((name, _sprite.Capture()));
        Index = 0;
        _saved = _entries[0].State;
        Changed?.Invoke();
    }

    public void MarkSaved()
    {
        if (Index >= 0) _saved = _entries[Index].State;
        Changed?.Invoke();
    }

    /// <summary>Marks the sprite as changed even when it is back at the saved state (for example after importing).</summary>
    public void MarkModified() => _saved = null;

    public void Push(string name)
    {
        if (Index < _entries.Count - 1) _entries.RemoveRange(Index + 1, _entries.Count - Index - 1);
        _entries.Add((name, _sprite.Capture()));
        Index = _entries.Count - 1;
        Trim();
        Changed?.Invoke();
    }

    private void Trim()
    {
        while (_entries.Count > Math.Max(2, MaxStates))
        {
            _entries.RemoveAt(0);
            Index--;
        }
        while (_entries.Count > 2 && MemoryUse() > MemoryBudget)
        {
            _entries.RemoveAt(0);
            Index--;
        }
    }

    private long MemoryUse()
    {
        HashSet<PixelBuffer> seen = [];
        long total = 0;
        foreach ((_, SpriteState state) in _entries)
            foreach (PixelBuffer buffer in state.Buffers())
                if (seen.Add(buffer)) total += buffer.ByteSize;
        return total;
    }

    public bool Undo() => JumpTo(Index - 1);

    public bool Redo() => JumpTo(Index + 1);

    public bool JumpTo(int index)
    {
        if (index < 0 || index >= _entries.Count || index == Index) return false;
        Index = index;
        _sprite.Restore(_entries[index].State);
        Changed?.Invoke();
        return true;
    }

    /// <summary>Throws away changes made since the last recorded state (a cancelled stroke or preview).</summary>
    public void Revert()
    {
        if (Index >= 0) _sprite.Restore(_entries[Index].State);
    }
}
