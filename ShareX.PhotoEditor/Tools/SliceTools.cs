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
using Avalonia.Media;
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

public sealed class SliceTool : ToolBase
{
    private SKPoint _start;
    private SKPoint _current;
    private bool _dragging;

    public SliceTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Slice;

    public override string Name => "Slice Tool";

    public override string Hint => "Drag to create a slice. Use File > Export > Save Slices to export them.";

    public float FixedWidth { get; set; }

    public float FixedHeight { get; set; }

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _start = _current = input.Doc;
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        _current = input.Doc;
        Host.InvalidateOverlay();
    }

    private SKRectI Rect()
    {
        SKRect r = FixedWidth > 0 && FixedHeight > 0 ? SKRect.Create(_current.X, _current.Y, FixedWidth, FixedHeight) : DragRect(_start, _current, false, false);
        SKRectI i = SKRectI.Round(r);
        i.Intersect(Doc!.Bounds);
        return i;
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_dragging || Doc == null) return;
        _dragging = false;
        SKRectI rect = Rect();
        Host.InvalidateOverlay();
        if (rect.Width < 2 || rect.Height < 2) return;
        Doc.Slices.Add(new SliceItem { Bounds = rect, Name = $"{Path.GetFileNameWithoutExtension(Doc.DisplayName.TrimEnd('*'))}_{Doc.Slices.Count + 1:00}" });
        PushHistory("Slice");
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging) return;
        SKRectI r = Rect();
        context.Rect(SKRect.Create(r.Left, r.Top, r.Width, r.Height), OverlayContext.AccentPen);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Number("Fixed W:", 0, 30000, () => FixedWidth, v => FixedWidth = (float)v, "0", "px");
        options.Number("H:", 0, 30000, () => FixedHeight, v => FixedHeight = (float)v, "0", "px");
        options.Button("Slices From Guides", () =>
        {
            if (Doc == null) return;
            List<float> xs = Doc.Guides.Where(g => !g.Horizontal).Select(g => g.Position).Append(0).Append(Doc.Width).Distinct().OrderBy(x => x).ToList();
            List<float> ys = Doc.Guides.Where(g => g.Horizontal).Select(g => g.Position).Append(0).Append(Doc.Height).Distinct().OrderBy(y => y).ToList();
            Doc.Slices.Clear();
            for (int r = 0; r < ys.Count - 1; r++)
                for (int c = 0; c < xs.Count - 1; c++)
                    Doc.Slices.Add(new SliceItem { Bounds = new SKRectI((int)xs[c], (int)ys[r], (int)xs[c + 1], (int)ys[r + 1]), Name = $"slice_{r + 1:00}_{c + 1:00}" });
            PushHistory("Slices From Guides");
            Host.InvalidateOverlay();
        });
        options.Button("Clear Slices", () =>
        {
            if (Doc == null) return;
            Doc.Slices.Clear();
            PushHistory("Clear Slices");
            Host.InvalidateOverlay();
        });
    }
}

public sealed class SliceSelectTool : ToolBase
{
    private SliceItem? _selected;
    private int _handle = -1;
    private SKPoint _dragStart;
    private SKRectI _startBounds;

    public SliceSelectTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.SliceSelect;

    public override string Name => "Slice Select Tool";

    public override string Hint => "Click to select a slice, drag to move or resize it. Delete removes it, double-click renames it.";

    public override ToolCursor Cursor => ToolCursor.Arrow;

    public SliceItem? Selected => _selected;

    public override async void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton || Host.View == null) return;
        if (_selected != null)
        {
            SKRectI b = _selected.Bounds;
            SKPoint[] corners = [new(b.Left, b.Top), new(b.Right, b.Top), new(b.Right, b.Bottom), new(b.Left, b.Bottom)];
            for (int i = 0; i < 4; i++)
            {
                if (SKPoint.Distance(corners[i], input.Doc) * Host.View.Zoom < 8)
                {
                    _handle = i;
                    _dragStart = input.Doc;
                    _startBounds = b;
                    return;
                }
            }
        }
        _selected = Doc.Slices.LastOrDefault(s => s.Bounds.Contains(input.DocPixel.X, input.DocPixel.Y));
        Host.RefreshOptions();
        Host.InvalidateOverlay();
        if (_selected == null) return;
        if (input.ClickCount >= 2)
        {
            string? name = await Host.PromptTextAsync("Slice Options", "Name:", _selected.Name);
            if (name != null)
            {
                _selected.Name = name;
                PushHistory("Edit Slice Options");
            }
            return;
        }
        _handle = 8;
        _dragStart = input.Doc;
        _startBounds = _selected.Bounds;
    }

    public override void PointerMove(ToolInput input)
    {
        if (_selected == null || _handle < 0) return;
        int dx = (int)MathF.Round(input.Doc.X - _dragStart.X), dy = (int)MathF.Round(input.Doc.Y - _dragStart.Y);
        SKRectI b = _startBounds;
        switch (_handle)
        {
            case 8: b.Offset(dx, dy); break;
            case 0: b.Left += dx; b.Top += dy; break;
            case 1: b.Right += dx; b.Top += dy; break;
            case 2: b.Right += dx; b.Bottom += dy; break;
            case 3: b.Left += dx; b.Bottom += dy; break;
        }
        _selected.Bounds = b.Standardized;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (_handle >= 0 && _selected != null && _selected.Bounds != _startBounds) PushHistory("Edit Slice");
        _handle = -1;
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if ((key == Key.Delete || key == Key.Back) && _selected != null && Doc != null)
        {
            Doc.Slices.Remove(_selected);
            _selected = null;
            PushHistory("Delete Slice");
            Host.RefreshOptions();
            Host.InvalidateOverlay();
            return true;
        }
        return base.KeyDown(key, modifiers);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_selected == null) return;
        SKRectI b = _selected.Bounds;
        context.Rect(SKRect.Create(b.Left, b.Top, b.Width, b.Height), new Pen(Brushes.Orange, 2));
        foreach (SKPoint c in new SKPoint[] { new(b.Left, b.Top), new(b.Right, b.Top), new(b.Right, b.Bottom), new(b.Left, b.Bottom) }) context.Handle(c);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        if (_selected is not SliceItem slice)
        {
            options.Label("No slice selected");
            return;
        }
        // The options bar can outlive the selection, so the handlers work on the slice they were built for.
        options.Text("Name:", () => slice.Name, v => slice.Name = v, 160);
        options.Text("URL:", () => slice.Url, v => slice.Url = v, 160);
        options.Button("Divide...", async () =>
        {
            if (Doc == null || _selected == null) return;
            string? text = await Host.PromptTextAsync("Divide Slice", "Rows x Columns (for example 2x3):", "2x2");
            if (text == null) return;
            string[] parts = text.ToLowerInvariant().Split('x');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int rows) || !int.TryParse(parts[1], out int cols) || rows < 1 || cols < 1) return;
            SKRectI b = _selected.Bounds;
            Doc.Slices.Remove(_selected);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    Doc.Slices.Add(new SliceItem
                    {
                        Bounds = new SKRectI(b.Left + b.Width * c / cols, b.Top + b.Height * r / rows, b.Left + b.Width * (c + 1) / cols, b.Top + b.Height * (r + 1) / rows),
                        Name = $"{_selected.Name}_{r + 1}_{c + 1}"
                    });
            _selected = null;
            PushHistory("Divide Slice");
            Host.RefreshOptions();
            Host.InvalidateOverlay();
        });
    }
}
