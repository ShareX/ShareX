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

    public override string Name => Strings.AnnotationTools_NoteTool;

    public override string Hint => Strings.AnnotationTools_NoteHint;

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
            PushHistory(Strings.AnnotationTools_DeleteNote);
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
        string? text = await Host.PromptTextAsync(Strings.AnnotationTools_Note, string.Format(Strings.AnnotationTools_AuthorLabel, Author), string.Empty, multiline: true);
        if (text == null || Doc == null) return;
        Doc.Notes.Add(new NoteItem { Position = position, Text = text, Author = Author, Color = Color });
        PushHistory(Strings.AnnotationTools_Note);
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
            PushHistory(Strings.AnnotationTools_MoveNote);
            return;
        }
        string? text = await Host.PromptTextAsync(Strings.AnnotationTools_Note, string.Format(Strings.AnnotationTools_AuthorLabel, note.Author), note.Text, multiline: true);
        if (text == null) return;
        note.Text = text;
        PushHistory(Strings.AnnotationTools_EditNote);
        Host.InvalidateOverlay();
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Text(Strings.AnnotationTools_AuthorColon, () => Author, v => Author = v, 140);
        options.ColorButton(Strings.AnnotationTools_NoteColorTip, () => Color, c => Color = c);
        options.Button(Strings.AnnotationTools_ClearAll, () =>
        {
            if (Doc == null || Doc.Notes.Count == 0) return;
            Doc.Notes.Clear();
            PushHistory(Strings.AnnotationTools_ClearNotes);
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

    public override string Name => Strings.AnnotationTools_CountTool;

    public override string Hint => Strings.AnnotationTools_CountHint;

    private CountItem? Hit(SKPoint p) => Doc?.Counts.LastOrDefault(c => SKPoint.Distance(c.Position, p) * Host.View!.Zoom < 8);

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        CountItem? hit = Hit(input.Doc);
        if (hit != null && input.Alt)
        {
            Doc.Counts.Remove(hit);
            PushHistory(Strings.AnnotationTools_DeleteCount);
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
        PushHistory(Strings.EditorCommands_Count);
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
        PushHistory(Strings.AnnotationTools_MoveCount);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        if (Doc == null) return;
        CountGroup group = Doc.CountGroups[Math.Clamp(Doc.ActiveCountGroup, 0, Doc.CountGroups.Count - 1)];
        options.Label(string.Format(Strings.AnnotationTools_CountLabel, Doc.Counts.Count(c => c.Group == Doc.ActiveCountGroup)));
        options.Combo(Strings.AnnotationTools_GroupColon, Doc.CountGroups.Select((g, i) => (g.Name, i)), () => Doc.ActiveCountGroup, v => { Doc.ActiveCountGroup = v; Host.RefreshOptions(); }, 140);
        options.Button("+", () =>
        {
            Doc.CountGroups.Add(new CountGroup { Name = string.Format(Strings.AnnotationTools_CountGroupNumber, Doc.CountGroups.Count + 1), Color = ColorMath.FromHsv(Doc.CountGroups.Count * 67 % 360, 0.8f, 0.95f) });
            Doc.ActiveCountGroup = Doc.CountGroups.Count - 1;
            Host.RefreshOptions();
        }, Strings.AnnotationTools_NewCountGroupTip);
        options.ColorButton(Strings.AnnotationTools_GroupColorTip, () => group.Color, c => { group.Color = c; Host.InvalidateOverlay(); });
        options.Number(Strings.AnnotationTools_MarkerSizeColon, 1, 10, () => group.MarkerSize, v => { group.MarkerSize = (float)v; Host.InvalidateOverlay(); }, "0", "px");
        options.Number(Strings.AnnotationTools_LabelSizeColon, 8, 72, () => group.LabelSize, v => { group.LabelSize = (float)v; Host.InvalidateOverlay(); }, "0", "pt");
        options.Button(Strings.EditorCommands_Clear, () =>
        {
            Doc.Counts.RemoveAll(c => c.Group == Doc.ActiveCountGroup);
            PushHistory(Strings.AnnotationTools_ClearCount);
            Host.RefreshOptions();
            Host.InvalidateOverlay();
        });
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers) => base.KeyDown(key, modifiers);
}
