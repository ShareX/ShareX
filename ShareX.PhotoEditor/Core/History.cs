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

public sealed class HistoryEntry
{
    public HistoryEntry(string name, DocumentState state, string? icon = null)
    {
        Name = name;
        State = state;
        Icon = icon;
    }

    public string Name { get; set; }

    public DocumentState State { get; }

    public string? Icon { get; }

    public DateTime Time { get; } = DateTime.Now;

    public override string ToString() => Name;
}

/// <summary>
/// Linear undo history. Each entry is a full document state that shares unchanged pixel surfaces with its
/// neighbors; editing code copies frozen surfaces before writing (copy-on-write).
/// </summary>
public sealed class History
{
    private readonly PhotoDocument _document;

    public History(PhotoDocument document)
    {
        _document = document;
    }

    public List<HistoryEntry> Entries { get; } = [];

    public List<HistoryEntry> Snapshots { get; } = [];

    public int Index { get; private set; } = -1;

    public int MaxStates { get; set; } = 50;

    /// <summary>Approximate memory the history may hold before the oldest states are dropped, so huge images cannot exhaust memory.</summary>
    public long MemoryBudget { get; set; } = 1_500_000_000;

    /// <summary>The fewest states kept when trimming for memory, so a few undo steps always remain.</summary>
    private const int MinStates = 4;

    /// <summary>The state used by the history brush, art history brush and "erase to history".</summary>
    public HistoryEntry? Source { get; set; }

    public bool CanUndo => Index > 0;

    public bool CanRedo => Index < Entries.Count - 1;

    public HistoryEntry? Current => Index >= 0 && Index < Entries.Count ? Entries[Index] : null;

    public event Action? Changed;

    public void Initialize(string name)
    {
        DisposeAll();
        Entries.Clear();
        Snapshots.Clear();
        HistoryEntry entry = new(name, _document.CaptureState());
        Entries.Add(entry);
        Snapshots.Add(new HistoryEntry(_document.DisplayName.TrimEnd('*'), entry.State));
        Source = Snapshots[0];
        Index = 0;
        Changed?.Invoke();
    }

    /// <summary>Records the current document as a new history state.</summary>
    public void Push(string name)
    {
        if (Index < Entries.Count - 1)
        {
            List<HistoryEntry> removed = Entries.GetRange(Index + 1, Entries.Count - Index - 1);
            Entries.RemoveRange(Index + 1, removed.Count);
            Release(removed);
        }
        Entries.Add(new HistoryEntry(name, _document.CaptureState()));
        while (Entries.Count > MaxStates)
        {
            HistoryEntry oldest = Entries[0];
            Entries.RemoveAt(0);
            if (Source == oldest) Source = Snapshots.FirstOrDefault();
            Release([oldest]);
        }
        // Each state shares unchanged surfaces, but edited layers are copied, so large documents can grow quickly.
        while (Entries.Count > MinStates && EstimateBytes() > MemoryBudget)
        {
            HistoryEntry oldest = Entries[0];
            Entries.RemoveAt(0);
            if (Source == oldest) Source = Snapshots.FirstOrDefault();
            Release([oldest]);
        }
        Index = Entries.Count - 1;
        _document.IsDirty = true;
        Changed?.Invoke();
    }

    /// <summary>Bytes held by the distinct pixel surfaces referenced by the history, snapshots and the live document.</summary>
    private long EstimateBytes()
    {
        HashSet<PixelSurface> seen = new(_document.LiveSurfaces);
        foreach (HistoryEntry entry in Entries.Concat(Snapshots))
        {
            foreach (PixelSurface surface in entry.State.Surfaces) seen.Add(surface);
        }
        long total = 0;
        foreach (PixelSurface surface in seen)
        {
            if (!surface.IsDisposed) total += (long)surface.RowBytes * surface.Height;
        }
        return total;
    }

    /// <summary>Renames the last entry, used when an operation is refined after recording.</summary>
    public void RenameCurrent(string name)
    {
        if (Current != null)
        {
            Current.Name = name;
            Changed?.Invoke();
        }
    }

    public void Undo()
    {
        if (!CanUndo) return;
        JumpTo(Index - 1);
    }

    public void Redo()
    {
        if (!CanRedo) return;
        JumpTo(Index + 1);
    }

    public void JumpTo(int index)
    {
        if (index < 0 || index >= Entries.Count) return;
        Index = index;
        _document.RestoreState(Entries[index].State);
        _document.IsDirty = true;
        Changed?.Invoke();
    }

    /// <summary>Restores the document to a snapshot and records that as a new state.</summary>
    public void RestoreSnapshot(HistoryEntry snapshot)
    {
        _document.RestoreState(snapshot.State);
        Push(snapshot.Name);
    }

    public HistoryEntry CreateSnapshot(string? name = null)
    {
        HistoryEntry snapshot = new(name ?? $"Snapshot {Snapshots.Count}", Current?.State ?? _document.CaptureState());
        Snapshots.Add(snapshot);
        Changed?.Invoke();
        return snapshot;
    }

    public void DeleteSnapshot(HistoryEntry snapshot)
    {
        Snapshots.Remove(snapshot);
        if (Source == snapshot) Source = Snapshots.FirstOrDefault() ?? Entries.FirstOrDefault();
        Release([snapshot]);
        Changed?.Invoke();
    }

    /// <summary>Discards the states after the current one (the redo chain).</summary>
    public void DiscardRedo()
    {
        if (!CanRedo) return;
        List<HistoryEntry> removed = Entries.GetRange(Index + 1, Entries.Count - Index - 1);
        Entries.RemoveRange(Index + 1, removed.Count);
        if (Source != null && removed.Contains(Source)) Source = Snapshots.FirstOrDefault() ?? Current;
        Release(removed);
        Changed?.Invoke();
    }

    /// <summary>Discards all states except the current one (Edit &gt; Purge &gt; Histories).</summary>
    public void Purge()
    {
        HistoryEntry? current = Current;
        if (current == null) return;
        List<HistoryEntry> removed = Entries.Where(e => e != current).ToList();
        Entries.Clear();
        Entries.Add(current);
        Index = 0;
        if (Source != null && removed.Contains(Source)) Source = Snapshots.FirstOrDefault() ?? current;
        Release(removed);
        Changed?.Invoke();
    }

    /// <summary>Returns the state before the current one (used by Edit &gt; Fade).</summary>
    public HistoryEntry? Previous => Index > 0 ? Entries[Index - 1] : null;

    private void Release(IEnumerable<HistoryEntry> removed)
    {
        HashSet<PixelSurface> live = new(_document.LiveSurfaces);
        foreach (HistoryEntry entry in Entries.Concat(Snapshots))
        {
            foreach (PixelSurface surface in entry.State.Surfaces) live.Add(surface);
        }
        foreach (HistoryEntry entry in removed)
        {
            entry.State.ReleaseCache();
            foreach (PixelSurface surface in entry.State.Surfaces)
            {
                if (!live.Contains(surface)) surface.Dispose();
            }
        }
    }

    public void DisposeAll()
    {
        HashSet<PixelSurface> live = new(_document.LiveSurfaces);
        foreach (HistoryEntry entry in Entries.Concat(Snapshots))
        {
            entry.State.ReleaseCache();
            foreach (PixelSurface surface in entry.State.Surfaces)
            {
                if (!live.Contains(surface)) surface.Dispose();
            }
        }
    }
}
