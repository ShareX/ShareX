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

/// <summary>A layered image document.</summary>
public sealed class PhotoDocument : IDisposable
{
    private static int _untitledCounter;

    public PhotoDocument(int width, int height, float resolution = 72, bool createBackground = true, SKColor? background = null)
    {
        Width = Math.Max(1, width);
        Height = Math.Max(1, height);
        Resolution = resolution;
        Root = new GroupLayer("Root") { BlendMode = LayerBlendMode.Normal };
        Title = $"Untitled-{Interlocked.Increment(ref _untitledCounter)}";
        History = new History(this);
        if (createBackground)
        {
            SKColor fill = background ?? SKColors.White;
            RasterLayer layer = new(fill.Alpha == 255 ? "Background" : "Layer 0", PixelSurface.CreateColor(Width, Height, fill))
            {
                IsBackground = fill.Alpha == 255,
                LockPosition = fill.Alpha == 255,
                LockTransparency = fill.Alpha == 255
            };
            Root.Add(layer);
            ActiveLayer = layer;
        }
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public SKRectI Bounds => new(0, 0, Width, Height);

    public float Resolution { get; set; }

    public GroupLayer Root { get; private set; }

    public Layer? ActiveLayer { get; set; }

    /// <summary>Ids of layers selected in the Layers panel (includes the active layer).</summary>
    public HashSet<int> SelectedLayerIds { get; } = [];

    /// <summary>When true, painting targets the active layer's mask instead of its pixels.</summary>
    public bool EditingMask { get; set; }

    public PixelSurface? Selection { get; private set; }

    public PixelSurface? LastSelection { get; private set; }

    public bool QuickMask { get; set; }

    public ChannelView ChannelView { get; set; } = ChannelView.Composite;

    public List<AlphaChannel> Channels { get; private set; } = [];

    public AlphaChannel? ActiveChannel { get; set; }

    public List<Guide> Guides { get; private set; } = [];

    public List<NoteItem> Notes { get; private set; } = [];

    public List<CountItem> Counts { get; private set; } = [];

    public List<CountGroup> CountGroups { get; private set; } = [new CountGroup()];

    public int ActiveCountGroup { get; set; }

    public List<ColorSamplerPoint> Samplers { get; private set; } = [];

    public List<SliceItem> Slices { get; private set; } = [];

    public List<NamedPath> Paths { get; private set; } = [];

    public VectorPath? WorkPath { get; set; }

    /// <summary>The path edited by pen tools: a saved path, the work path, or the active shape layer's path.</summary>
    public NamedPath? ActivePath { get; set; }

    public string? FilePath { get; set; }

    public string Title { get; set; }

    public bool IsDirty { get; set; }

    public History History { get; }

    /// <summary>Raised when pixels change. The rectangle is in document coordinates, or null for everything.</summary>
    public event Action<SKRectI?>? Invalidated;

    /// <summary>Raised when the layer structure, selection or other non-pixel state changes.</summary>
    public event Action? StructureChanged;

    public void Invalidate(SKRectI? area = null) => Invalidated?.Invoke(area);

    public void NotifyStructureChanged() => StructureChanged?.Invoke();

    public IEnumerable<Layer> AllLayers => Root.Descendants();

    public Layer? FindLayer(int id) => Root.Descendants().FirstOrDefault(l => l.Id == id);

    public IEnumerable<Layer> SelectedLayers
    {
        get
        {
            List<Layer> layers = Root.Descendants().Where(l => SelectedLayerIds.Contains(l.Id)).ToList();
            if (ActiveLayer != null && !layers.Contains(ActiveLayer)) layers.Add(ActiveLayer);
            return layers;
        }
    }

    /// <summary>The layers a move or nudge applies to: the selected layers plus every layer linked to one of them.</summary>
    public List<Layer> GetMoveTargets()
    {
        List<Layer> selected = SelectedLayers.ToList();
        HashSet<int> links = selected.Where(l => l.LinkId != 0).Select(l => l.LinkId).ToHashSet();
        List<Layer> targets = AllLayers.Where(l => selected.Contains(l) || l.LinkId != 0 && links.Contains(l.LinkId)).ToList();
        // A group already moves its children, so children of a moved group are left out.
        return targets.Where(l => !targets.Any(o => o != l && IsDescendant(l, o))).ToList();
    }

    public void SetSelection(PixelSurface? selection)
    {
        if (selection != null && MaskOps.IsEmpty(selection)) selection = null;
        if (Selection != null) LastSelection = Selection;
        Selection = selection;
        NotifyStructureChanged();
    }

    /// <summary>Sets the selection during an interactive edit without emptiness checks or notifications.</summary>
    public void SetSelectionLive(PixelSurface? selection) => Selection = selection;

    public bool HasSelection => Selection != null;

    /// <summary>Returns the selection coverage (0..1) at a document position. With no selection everything is selected.</summary>
    public float SelectionAt(int x, int y)
    {
        if (Selection == null) return 1;
        return MaskOps.Get(Selection, x, y) / 255f;
    }

    public SKRectI SelectionBounds => Selection != null ? MaskOps.GetBounds(Selection) : Bounds;

    /// <summary>Inserts a layer above the active layer (inside the same group).</summary>
    public void AddLayer(Layer layer, Layer? above = null)
    {
        Layer? anchor = above ?? ActiveLayer;
        GroupLayer parent = anchor is GroupLayer g && g.Expanded && above == null && g.Children.Count == 0 ? g : anchor?.Parent ?? Root;
        int index = anchor != null && anchor.Parent == parent ? parent.Children.IndexOf(anchor) + 1 : parent.Children.Count;
        parent.Add(layer, index);
        ActiveLayer = layer;
        SelectedLayerIds.Clear();
        SelectedLayerIds.Add(layer.Id);
        EditingMask = false;
        NotifyStructureChanged();
    }

    public void RemoveLayer(Layer layer)
    {
        GroupLayer? parent = layer.Parent;
        if (parent == null) return;
        int index = parent.Children.IndexOf(layer);
        parent.Remove(layer);
        if (ActiveLayer == layer || ActiveLayer != null && IsDescendant(ActiveLayer, layer))
        {
            ActiveLayer = parent.Children.Count > 0 ? parent.Children[Math.Clamp(index - 1, 0, parent.Children.Count - 1)] : parent == Root ? Root.Children.LastOrDefault() : parent;
        }
        SelectedLayerIds.Remove(layer.Id);
        NotifyStructureChanged();
    }

    public static bool IsDescendant(Layer layer, Layer ancestor)
    {
        for (Layer? l = layer.Parent; l != null; l = l.Parent)
        {
            if (l == ancestor) return true;
        }
        return false;
    }

    /// <summary>Flattened list of layers top to bottom, as shown in the Layers panel.</summary>
    public List<(Layer Layer, int Depth)> GetLayerListTopDown()
    {
        List<(Layer, int)> result = [];
        void Walk(GroupLayer group, int depth)
        {
            for (int i = group.Children.Count - 1; i >= 0; i--)
            {
                Layer layer = group.Children[i];
                result.Add((layer, depth));
                if (layer is GroupLayer child && child.Expanded) Walk(child, depth + 1);
            }
        }
        Walk(Root, 0);
        return result;
    }

    public void ResizeCanvas(int width, int height)
    {
        Width = Math.Max(1, width);
        Height = Math.Max(1, height);
    }

    public DocumentState CaptureState()
    {
        foreach (PixelSurface surface in Root.Surfaces) surface.Freeze();
        Selection?.Freeze();
        foreach (AlphaChannel channel in Channels) channel.Mask.Freeze();
        return new DocumentState
        {
            Width = Width,
            Height = Height,
            Resolution = Resolution,
            Root = (GroupLayer)Root.CloneShared(),
            ActiveLayerId = ActiveLayer?.Id ?? -1,
            EditingMask = EditingMask,
            Selection = Selection,
            Channels = Channels.Select(c => c.Clone()).ToList(),
            Guides = Guides.ToList(),
            Notes = Notes.Select(n => n.Clone()).ToList(),
            Counts = Counts.Select(c => c.Clone()).ToList(),
            CountGroups = CountGroups.Select(c => c.Clone()).ToList(),
            Samplers = Samplers.Select(s => s.Clone()).ToList(),
            Slices = Slices.Select(s => s.Clone()).ToList(),
            Paths = Paths.Select(p => p.Clone()).ToList(),
            WorkPath = WorkPath?.Clone()
        };
    }

    public void RestoreState(DocumentState state, bool keepHistory = true)
    {
        string? activePathName = ActivePath?.Name;
        bool activeWasWork = ActivePath != null && WorkPath != null && ActivePath.Path == WorkPath;
        Width = state.Width;
        Height = state.Height;
        Resolution = state.Resolution;
        Root = (GroupLayer)state.Root.CloneShared();
        ActiveLayer = FindLayer(state.ActiveLayerId) ?? Root.Children.LastOrDefault();
        SelectedLayerIds.Clear();
        if (ActiveLayer != null) SelectedLayerIds.Add(ActiveLayer.Id);
        EditingMask = state.EditingMask && ActiveLayer?.Mask != null;
        Selection = state.Selection;
        Channels = state.Channels.Select(c => c.Clone()).ToList();
        ActiveChannel = null;
        Guides = state.Guides.ToList();
        Notes = state.Notes.Select(n => n.Clone()).ToList();
        Counts = state.Counts.Select(c => c.Clone()).ToList();
        CountGroups = state.CountGroups.Select(c => c.Clone()).ToList();
        Samplers = state.Samplers.Select(s => s.Clone()).ToList();
        Slices = state.Slices.Select(s => s.Clone()).ToList();
        Paths = state.Paths.Select(p => p.Clone()).ToList();
        WorkPath = state.WorkPath?.Clone();
        ActivePath = activeWasWork && WorkPath != null ? new NamedPath("Work Path", WorkPath) : Paths.FirstOrDefault(p => p.Name == activePathName);
        NotifyStructureChanged();
        Invalidate();
    }

    public IEnumerable<PixelSurface> LiveSurfaces
    {
        get
        {
            foreach (PixelSurface s in Root.Surfaces) yield return s;
            if (Selection != null) yield return Selection;
            if (LastSelection != null) yield return LastSelection;
            foreach (AlphaChannel c in Channels) yield return c.Mask;
        }
    }

    /// <summary>Replaces the whole layer tree (used by loaders and flatten).</summary>
    public void ReplaceRoot(GroupLayer root)
    {
        Root = root;
        ActiveLayer = root.Children.LastOrDefault();
        SelectedLayerIds.Clear();
        if (ActiveLayer != null) SelectedLayerIds.Add(ActiveLayer.Id);
        NotifyStructureChanged();
    }

    public string DisplayName => (FilePath != null ? Path.GetFileName(FilePath) : Title) + (IsDirty ? "*" : string.Empty);

    public void Dispose()
    {
        History.DisposeAll();
        foreach (PixelSurface surface in LiveSurfaces.Distinct()) surface.Dispose();
    }
}
