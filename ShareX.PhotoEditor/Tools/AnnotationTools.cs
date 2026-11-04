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

using Avalonia.Input;
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

public sealed class NoteTool : ToolBase
{
    private NoteItem? _dragging;
    private SKPoint _dragOffset;
    private bool _moved;

    public NoteTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Note;

    public override string Name => "Note Tool";

    public override string Hint => "Click to add a note. Click a note to edit it, drag to move it, Alt-click to delete it.";

    public override ToolCursor Cursor => ToolCursor.Arrow;

    public string Author { get; set; } = Environment.UserName;

    public SKColor Color { get; set; } = new(0xFF, 0xD7, 0x40);

    private NoteItem? Hit(SKPoint p) => Doc?.Notes.LastOrDefault(n =>
    {
        SKPoint d = p - n.Position;
        float size = 18 / (float)Host.View!.Zoom;
        return d.X >= 0 && d.Y >= 0 && d.X <= size && d.Y <= size;
    });

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        NoteItem? hit = Hit(input.Doc);
        if (hit != null && input.Alt)
        {
            Doc.Notes.Remove(hit);
            PushHistory("Delete Note");
            Host.InvalidateOverlay();
            return;
        }
        if (hit != null)
        {
            _dragging = hit;
            _dragOffset = input.Doc - hit.Position;
            _moved = false;
            return;
        }
        _ = AddNoteAsync(input.Doc);
    }

    private async Task AddNoteAsync(SKPoint position)
    {
        string? text = await Host.PromptTextAsync("Note", $"Author: {Author}", string.Empty);
        if (text == null || Doc == null) return;
        Doc.Notes.Add(new NoteItem { Position = position, Text = text, Author = Author, Color = Color });
        PushHistory("Note");
        Host.InvalidateOverlay();
    }

    public override void PointerMove(ToolInput input)
    {
        if (_dragging == null) return;
        _dragging.Position = input.Doc - _dragOffset;
        _moved = true;
        Host.InvalidateOverlay();
    }

    public override async void PointerUp(ToolInput input)
    {
        NoteItem? note = _dragging;
        _dragging = null;
        if (note == null) return;
        if (_moved)
        {
            PushHistory("Move Note");
            return;
        }
        string? text = await Host.PromptTextAsync("Note", $"Author: {note.Author}", note.Text);
        if (text == null) return;
        note.Text = text;
        PushHistory("Edit Note");
        Host.InvalidateOverlay();
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Text("Author:", () => Author, v => Author = v, 140);
        options.ColorButton("Note color", () => Color, c => Color = c);
        options.Button("Clear All", () =>
        {
            if (Doc == null || Doc.Notes.Count == 0) return;
            Doc.Notes.Clear();
            PushHistory("Clear Notes");
            Host.InvalidateOverlay();
        });
    }
}

public sealed class CountTool : ToolBase
{
    private CountItem? _dragging;

    public CountTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Count;

    public override string Name => "Count Tool";

    public override string Hint => "Click to place numbered count markers. Drag to move, Alt-click to remove.";

    private CountItem? Hit(SKPoint p) => Doc?.Counts.LastOrDefault(c => SKPoint.Distance(c.Position, p) * Host.View!.Zoom < 8);

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        CountItem? hit = Hit(input.Doc);
        if (hit != null && input.Alt)
        {
            Doc.Counts.Remove(hit);
            PushHistory("Delete Count");
            Host.RefreshOptions();
            Host.InvalidateOverlay();
            return;
        }
        if (hit != null)
        {
            _dragging = hit;
            return;
        }
        Doc.Counts.Add(new CountItem { Position = input.Doc, Group = Doc.ActiveCountGroup });
        PushHistory("Count");
        Host.RefreshOptions();
        Host.InvalidateOverlay();
    }

    public override void PointerMove(ToolInput input)
    {
        if (_dragging == null) return;
        _dragging.Position = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (_dragging == null) return;
        _dragging = null;
        PushHistory("Move Count");
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        if (Doc == null) return;
        CountGroup group = Doc.CountGroups[Math.Clamp(Doc.ActiveCountGroup, 0, Doc.CountGroups.Count - 1)];
        options.Label($"Count: {Doc.Counts.Count(c => c.Group == Doc.ActiveCountGroup)}");
        options.Combo("Group:", Doc.CountGroups.Select((g, i) => (g.Name, i)), () => Doc.ActiveCountGroup, v => { Doc.ActiveCountGroup = v; Host.RefreshOptions(); }, 140);
        options.Button("+", () =>
        {
            Doc.CountGroups.Add(new CountGroup { Name = $"Count Group {Doc.CountGroups.Count + 1}", Color = ColorMath.FromHsv(Doc.CountGroups.Count * 67 % 360, 0.8f, 0.95f) });
            Doc.ActiveCountGroup = Doc.CountGroups.Count - 1;
            Host.RefreshOptions();
        }, "Create a new count group");
        options.ColorButton("Group color", () => group.Color, c => { group.Color = c; Host.InvalidateOverlay(); });
        options.Number("Marker Size:", 1, 10, () => group.MarkerSize, v => { group.MarkerSize = (float)v; Host.InvalidateOverlay(); }, "0", "px");
        options.Number("Label Size:", 8, 72, () => group.LabelSize, v => { group.LabelSize = (float)v; Host.InvalidateOverlay(); }, "0", "pt");
        options.Button("Clear", () =>
        {
            Doc.Counts.RemoveAll(c => c.Group == Doc.ActiveCountGroup);
            PushHistory("Clear Count");
            Host.RefreshOptions();
            Host.InvalidateOverlay();
        });
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers) => base.KeyDown(key, modifiers);
}
