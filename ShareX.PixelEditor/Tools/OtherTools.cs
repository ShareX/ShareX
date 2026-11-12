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
using ShareX.PixelEditor.UI;
using ShareX.PixelEditor.UI.Dialogs;

namespace ShareX.PixelEditor.Tools;

public sealed class HandTool : ToolBase
{
    private double _startX, _startY;
    private bool _dragging;

    public HandTool(PixelEditorWindow editor) : base(editor, ToolId.Hand, Strings.Tool_Hand, Strings.Tool_Hand_Hint, PixelEditorIcons.Hand, Key.H)
    {
    }

    public override StandardCursorType Cursor => StandardCursorType.Hand;

    public override void Down(ToolPointer p)
    {
        _startX = p.Fx;
        _startY = p.Fy;
        _dragging = true;
    }

    public override void Move(ToolPointer p)
    {
        if (!_dragging || Editor.View == null) return;
        // Panning keeps the grabbed sprite position under the pointer.
        Editor.View.PanBy((p.Fx - _startX) * Editor.View.Zoom, (p.Fy - _startY) * Editor.View.Zoom);
        Editor.InvalidateOverlay();
    }

    public override void Up(ToolPointer p) => _dragging = false;

    public override void Cancel() => _dragging = false;

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Button(Strings.Command_FitOnScreen, () => Editor.Execute("view.fit"));
        options.Button("100%", () => Editor.Execute("view.actualSize"));
    }
}

public sealed class ZoomTool : ToolBase
{
    public ZoomTool(PixelEditorWindow editor) : base(editor, ToolId.Zoom, Strings.Tool_Zoom, Strings.Tool_Zoom_Hint, PixelEditorIcons.Zoom, Key.Z)
    {
    }

    public override void Down(ToolPointer p)
    {
        Editor.View?.ZoomAtDocument(p.Fx, p.Fy, !(p.Right || p.Alt));
        Editor.InvalidateOverlay();
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.IconButton(PixelEditorIcons.ZoomIn, Strings.Command_ZoomIn, () => Editor.Execute("view.zoomIn"));
        options.IconButton(PixelEditorIcons.ZoomOut, Strings.Command_ZoomOut, () => Editor.Execute("view.zoomOut"));
        options.Separator();
        options.Button(Strings.Command_FitOnScreen, () => Editor.Execute("view.fit"));
        options.Button("100%", () => Editor.Execute("view.actualSize"));
    }
}

/// <summary>Click to type text, which is then placed as a movable selection.</summary>
public sealed class TextTool : ToolBase
{
    public TextTool(PixelEditorWindow editor) : base(editor, ToolId.Text, Strings.Tool_Text, Strings.Tool_Text_Hint, PixelEditorIcons.Text, Key.T)
    {
    }

    public override StandardCursorType Cursor => StandardCursorType.Ibeam;

    public override async void Down(ToolPointer p)
    {
        if (Sprite == null || Editor.CreatePainter(false) == null) return;
        Editor.RevertEdit();
        PixelBuffer? text = await TextDialog.ShowAsync(Editor);
        if (text == null || Editor.Sprite == null) return;
        Editor.PasteFloating(text, p.X, p.Y, Strings.Tool_Text);
    }

    public override void BuildOptions(OptionsBuilder options) => options.Label(Strings.Options_TextHint);
}

/// <summary>Draws and edits slices: named rectangles that are exported with sprite sheets.</summary>
public sealed class SliceTool : ToolBase
{
    private SKPointI _start, _end;
    private bool _creating, _moving;
    private SKPointI _moveOffset;

    public SliceTool(PixelEditorWindow editor) : base(editor, ToolId.Slice, Strings.Tool_Slice, Strings.Tool_Slice_Hint, PixelEditorIcons.Slice, Key.C)
    {
    }

    /// <summary>The slice last clicked or created.</summary>
    public Slice? Selected { get; private set; }

    private SKRectI Box => new(Math.Min(_start.X, _end.X), Math.Min(_start.Y, _end.Y), Math.Max(_start.X, _end.X), Math.Max(_start.Y, _end.Y));

    public override void Down(ToolPointer p)
    {
        if (Sprite == null) return;
        Slice? hit = Sprite.Slices.LastOrDefault(s => s.Bounds.Contains(p.X, p.Y));
        if (hit != null)
        {
            Selected = hit;
            _moving = true;
            _moveOffset = new SKPointI(p.X - hit.Bounds.Left, p.Y - hit.Bounds.Top);
            Editor.RefreshOptions();
        }
        else
        {
            Selected = null;
            _start = _end = SnapToGrid(new SKPointI((int)Math.Round(p.Fx), (int)Math.Round(p.Fy)));
            _creating = true;
        }
        Editor.InvalidateOverlay();
    }

    public override void Move(ToolPointer p)
    {
        if (_creating)
        {
            _end = SnapToGrid(new SKPointI((int)Math.Round(p.Fx), (int)Math.Round(p.Fy)));
            Editor.SetStatus($"{Box.Width} × {Box.Height} px");
        }
        else if (_moving && Selected != null)
        {
            SKPointI corner = SnapToGrid(new SKPointI(p.X - _moveOffset.X, p.Y - _moveOffset.Y));
            Selected.Bounds = SKRectI.Create(corner.X, corner.Y, Selected.Bounds.Width, Selected.Bounds.Height);
        }
        Editor.InvalidateOverlay();
    }

    public override void Up(ToolPointer p)
    {
        if (Sprite == null) return;
        if (_creating)
        {
            _creating = false;
            Editor.SetStatus(null);
            SKRectI box = Box;
            box.Intersect(Sprite.Bounds);
            if (box.Width > 0 && box.Height > 0)
            {
                Selected = new Slice { Name = string.Format(Strings.Slice_DefaultName, Sprite.Slices.Count + 1), Bounds = box };
                Sprite.Slices.Add(Selected);
                Editor.Commit(Strings.History_NewSlice);
                Editor.RefreshOptions();
            }
        }
        else if (_moving)
        {
            _moving = false;
            Editor.Commit(Strings.History_MoveSlice);
        }
        Editor.InvalidateOverlay();
    }

    public override async void DoubleClick(ToolPointer p)
    {
        if (Sprite == null || Selected == null) return;
        string? name = await Editor.PromptTextAsync(Strings.Slice_Rename, Strings.Dialog_Name, Selected.Name);
        if (string.IsNullOrWhiteSpace(name) || Selected == null) return;
        Selected.Name = name.Trim();
        Editor.Commit(Strings.History_RenameSlice);
        Editor.RefreshOptions();
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if (key is Key.Delete or Key.Back && Selected != null && Sprite != null)
        {
            Sprite.Slices.Remove(Selected);
            Selected = null;
            Editor.Commit(Strings.History_DeleteSlice);
            Editor.RefreshOptions();
            return true;
        }
        return false;
    }

    public override void Cancel()
    {
        _creating = false;
        _moving = false;
    }

    public override void Activate()
    {
        // After undo the selected slice object no longer belongs to the sprite.
        if (Selected != null && Sprite?.Slices.Contains(Selected) != true) Selected = null;
    }

    public override void DrawOverlay(OverlayContext overlay)
    {
        if (Sprite == null) return;
        if (Selected != null && !Sprite.Slices.Contains(Selected)) Selected = Sprite.Slices.FirstOrDefault(s => s.Name == Selected.Name);
        if (!Settings.ShowSlices)
        {
            // Slices are always visible while their tool is active.
            foreach (Slice slice in Sprite.Slices) overlay.Slice(slice, false);
        }
        if (Selected != null) overlay.Slice(Selected, true);
        if (_creating) overlay.RectOutline(Box);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        if (Selected == null)
        {
            options.Label(Strings.Options_SliceHint);
            return;
        }
        Slice slice = Selected;
        options.Label(slice.Name);
        options.Separator();
        void Change(Func<SKRectI, SKRectI> change)
        {
            SKRectI b = change(slice.Bounds);
            if (b.Width < 1 || b.Height < 1) return;
            slice.Bounds = b;
            Editor.Commit(Strings.History_MoveSlice);
        }
        options.Number("X", 0, Sprite.MaxSize, slice.Bounds.Left, v => Change(b => SKRectI.Create(v, b.Top, b.Width, b.Height)));
        options.Number("Y", 0, Sprite.MaxSize, slice.Bounds.Top, v => Change(b => SKRectI.Create(b.Left, v, b.Width, b.Height)));
        options.Number(Strings.Options_Width, 1, Sprite.MaxSize, slice.Bounds.Width, v => Change(b => SKRectI.Create(b.Left, b.Top, v, b.Height)));
        options.Number(Strings.Options_Height, 1, Sprite.MaxSize, slice.Bounds.Height, v => Change(b => SKRectI.Create(b.Left, b.Top, b.Width, v)));
        options.Separator();
        options.ColorButton(Strings.Dialog_Color, slice.Color, c =>
        {
            slice.Color = c;
            Editor.Commit(Strings.History_RenameSlice);
        });
        options.Button(Strings.Slice_Rename, () => DoubleClick(default));
        options.Button(Strings.Command_Delete, () => KeyDown(Key.Delete, KeyModifiers.None));
    }
}
