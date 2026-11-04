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

/// <summary>Immutable capture of a document used by history and snapshots.</summary>
public sealed class DocumentState
{
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required float Resolution { get; init; }
    public required GroupLayer Root { get; init; }
    public required int ActiveLayerId { get; init; }
    public required bool EditingMask { get; init; }
    public required PixelSurface? Selection { get; init; }
    public required List<AlphaChannel> Channels { get; init; }
    public required List<Guide> Guides { get; init; }
    public required List<NoteItem> Notes { get; init; }
    public required List<CountItem> Counts { get; init; }
    public required List<CountGroup> CountGroups { get; init; }
    public required List<ColorSamplerPoint> Samplers { get; init; }
    public required List<SliceItem> Slices { get; init; }
    public required List<NamedPath> Paths { get; init; }
    public required VectorPath? WorkPath { get; init; }

    public IEnumerable<PixelSurface> Surfaces
    {
        get
        {
            foreach (PixelSurface s in Root.Surfaces) yield return s;
            if (Selection != null) yield return Selection;
            foreach (AlphaChannel c in Channels) yield return c.Mask;
        }
    }

    private SKBitmap? _composite;

    /// <summary>Lazily flattened image of this state (used by the history brush and Fade).</summary>
    public SKBitmap GetComposite()
    {
        if (_composite == null)
        {
            PhotoDocument temp = new(Width, Height, Resolution, createBackground: false);
            temp.RestoreState(this, keepHistory: true);
            _composite = Compositor.Flatten(temp);
        }
        return _composite;
    }

    /// <summary>Returns this state's pixels for a layer id, or null when the layer did not exist.</summary>
    public RasterLayer? FindRasterLayer(int id) => Root.Descendants().OfType<RasterLayer>().FirstOrDefault(l => l.Id == id);

    public void ReleaseCache()
    {
        _composite?.Dispose();
        _composite = null;
    }
}
