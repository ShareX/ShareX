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

namespace ShareX.PixelEditor.Tools;

public abstract class SelectToolBase : ToolBase
{
    protected SelectToolBase(PixelEditorWindow editor, ToolId id, string name, string hint, string icon, Key shortcut) : base(editor, id, name, hint, icon, shortcut)
    {
    }

    /// <summary>Shift adds, Alt subtracts, both intersect; otherwise the mode chosen in the options bar.</summary>
    protected SelectionMode ModeFor(ToolPointer p) =>
        p.Shift && p.Alt ? SelectionMode.Intersect : p.Shift ? SelectionMode.Add : p.Alt ? SelectionMode.Subtract : Settings.SelectionMode;

    protected void Apply(SelectionMask mask, SelectionMode mode)
    {
        if (Sprite == null) return;
        Editor.SetSelection(SelectionMask.Combine(Sprite.Selection, mask, mode));
    }

    private SelectionMask? _moveOriginal;
    private SKPointI _moveStart;
    private bool _moved;

    /// <summary>Starts dragging the selection outline (not its pixels) when a plain press lands inside the selection.</summary>
    protected bool TryBeginMove(ToolPointer p)
    {
        if (Sprite?.Selection is not SelectionMask selection || ModeFor(p) != SelectionMode.Replace || !selection[p.X, p.Y]) return false;
        Editor.CommitFloating();
        _moveOriginal = Sprite.Selection;
        _moveStart = p.Pixel;
        _moved = false;
        return true;
    }

    protected bool IsMoving => _moveOriginal != null;

    protected void ContinueMove(ToolPointer p)
    {
        if (Sprite == null || _moveOriginal == null) return;
        int dx = p.X - _moveStart.X, dy = p.Y - _moveStart.Y;
        _moved |= dx != 0 || dy != 0;
        Sprite.Selection = _moveOriginal.Offset(dx, dy);
        Editor.InvalidateOverlay();
    }

    protected void EndMove()
    {
        if (Sprite == null || _moveOriginal == null) return;
        SelectionMask? moved = Sprite.Selection;
        Sprite.Selection = _moveOriginal;
        _moveOriginal = null;
        // A click without a drag clears the selection.
        Editor.SetSelection(_moved ? moved : null);
    }

    protected void AbandonMove()
    {
        if (Sprite != null && _moveOriginal != null) Sprite.Selection = _moveOriginal;
        _moveOriginal = null;
    }

    protected void ModeOptions(OptionsBuilder options)
    {
        options.Choice(
        [
            (PixelEditorIcons.SelectNew, Strings.Options_SelectNew, SelectionMode.Replace),
            (PixelEditorIcons.SelectAdd, Strings.Options_SelectAdd, SelectionMode.Add),
            (PixelEditorIcons.SelectSubtract, Strings.Options_SelectSubtract, SelectionMode.Subtract),
            (PixelEditorIcons.SelectIntersect, Strings.Options_SelectIntersect, SelectionMode.Intersect)
        ], Settings.SelectionMode, v => Settings.SelectionMode = v);
    }
}

public abstract class BoxSelectToolBase : SelectToolBase
{
    private SKPointI _start, _end;
    private bool _dragging, _shiftAtDown;
    private SelectionMode _mode;

    protected BoxSelectToolBase(PixelEditorWindow editor, ToolId id, string name, string hint, string icon, Key shortcut) : base(editor, id, name, hint, icon, shortcut)
    {
    }

    protected abstract SelectionMask Create(Sprite sprite, SKRectI rect);

    protected SKRectI Box
    {
        get
        {
            int l = Math.Min(_start.X, _end.X), t = Math.Min(_start.Y, _end.Y), r = Math.Max(_start.X, _end.X), b = Math.Max(_start.Y, _end.Y);
            return new SKRectI(l, t, r, b);
        }
    }

    public override void Down(ToolPointer p)
    {
        if (Sprite == null) return;
        if (TryBeginMove(p)) return;
        Editor.CommitFloating();
        _start = _end = SnapToGrid(new SKPointI((int)Math.Round(p.Fx), (int)Math.Round(p.Fy)));
        _mode = ModeFor(p);
        _shiftAtDown = p.Shift;
        _dragging = true;
    }

    public override void Move(ToolPointer p)
    {
        if (IsMoving)
        {
            ContinueMove(p);
            return;
        }
        if (!_dragging) return;
        SKPointI end = SnapToGrid(new SKPointI((int)Math.Round(p.Fx), (int)Math.Round(p.Fy)));
        if (p.Shift && !_shiftAtDown) end = ConstrainSquare(_start, end);
        _end = end;
        Editor.SetStatus($"{Box.Width} × {Box.Height} px");
        Editor.InvalidateOverlay();
    }

    public override void Up(ToolPointer p)
    {
        if (IsMoving)
        {
            EndMove();
            return;
        }
        if (!_dragging || Sprite == null) return;
        _dragging = false;
        Editor.SetStatus(null);
        SKRectI box = Box;
        if (box.Width <= 0 || box.Height <= 0)
        {
            // A click without a drag clears the selection.
            if (_mode == SelectionMode.Replace) Editor.SetSelection(null);
        }
        else
        {
            Apply(Create(Sprite, box), _mode);
        }
        Editor.InvalidateOverlay();
    }

    public override void Cancel()
    {
        AbandonMove();
        _dragging = false;
        Editor.InvalidateOverlay();
    }

    protected bool IsDragging => _dragging;

    public override void BuildOptions(OptionsBuilder options) => ModeOptions(options);
}

public sealed class RectSelectTool : BoxSelectToolBase
{
    public RectSelectTool(PixelEditorWindow editor) : base(editor, ToolId.RectSelect, Strings.Tool_RectSelect, Strings.Tool_Select_Hint, PixelEditorIcons.RectSelect, Key.M)
    {
    }

    protected override SelectionMask Create(Sprite sprite, SKRectI rect) => SelectionMask.FromRect(sprite.Width, sprite.Height, rect);

    public override void DrawOverlay(OverlayContext overlay)
    {
        if (IsDragging) overlay.RectOutline(Box);
    }
}

public sealed class EllipseSelectTool : BoxSelectToolBase
{
    public EllipseSelectTool(PixelEditorWindow editor) : base(editor, ToolId.EllipseSelect, Strings.Tool_EllipseSelect, Strings.Tool_Select_Hint, PixelEditorIcons.EllipseSelect, Key.M)
    {
    }

    protected override SelectionMask Create(Sprite sprite, SKRectI rect) => SelectionMask.FromEllipse(sprite.Width, sprite.Height, rect);

    public override void DrawOverlay(OverlayContext overlay)
    {
        if (IsDragging) overlay.EllipseOutline(Box);
    }
}

public sealed class LassoTool : SelectToolBase
{
    private readonly List<SKPointI> _points = [];
    private SelectionMode _mode;
    private bool _dragging;

    public LassoTool(PixelEditorWindow editor) : base(editor, ToolId.Lasso, Strings.Tool_Lasso, Strings.Tool_Lasso_Hint, PixelEditorIcons.Lasso, Key.Q)
    {
    }

    public override void Down(ToolPointer p)
    {
        if (Sprite == null) return;
        if (TryBeginMove(p)) return;
        Editor.CommitFloating();
        _points.Clear();
        _points.Add(p.Pixel);
        _mode = ModeFor(p);
        _dragging = true;
    }

    public override void Move(ToolPointer p)
    {
        if (IsMoving)
        {
            ContinueMove(p);
            return;
        }
        if (!_dragging || _points[^1] == p.Pixel) return;
        _points.Add(p.Pixel);
        Editor.InvalidateOverlay();
    }

    public override void Up(ToolPointer p)
    {
        if (IsMoving)
        {
            EndMove();
            return;
        }
        if (!_dragging || Sprite == null) return;
        _dragging = false;
        if (_points.Count < 3)
        {
            if (_mode == SelectionMode.Replace) Editor.SetSelection(null);
        }
        else
        {
            Apply(SelectionMask.FromPolygon(Sprite.Width, Sprite.Height, _points), _mode);
        }
        _points.Clear();
        Editor.InvalidateOverlay();
    }

    public override void Cancel()
    {
        AbandonMove();
        _dragging = false;
        _points.Clear();
        Editor.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext overlay)
    {
        if (_dragging) overlay.PixelPath(_points, true);
    }

    public override void BuildOptions(OptionsBuilder options) => ModeOptions(options);
}

public sealed class PolygonLassoTool : SelectToolBase
{
    private readonly List<SKPointI> _points = [];
    private SKPointI _cursor;
    private SelectionMode _mode;

    public PolygonLassoTool(PixelEditorWindow editor) : base(editor, ToolId.PolygonLasso, Strings.Tool_PolygonLasso, Strings.Tool_PolygonLasso_Hint, PixelEditorIcons.PolygonLasso, Key.Q)
    {
    }

    public override void Down(ToolPointer p)
    {
        if (Sprite == null) return;
        if (_points.Count == 0)
        {
            Editor.CommitFloating();
            _mode = ModeFor(p);
        }
        SKPointI point = p.Shift && _points.Count > 0 ? ConstrainAngle(_points[^1], p.Pixel) : p.Pixel;
        // Clicking the first point again closes the polygon.
        if (_points.Count >= 3 && Math.Abs(point.X - _points[0].X) <= 1 && Math.Abs(point.Y - _points[0].Y) <= 1)
        {
            Finish();
            return;
        }
        _points.Add(point);
        _cursor = point;
        Editor.InvalidateOverlay();
    }

    public override void Hover(ToolPointer p)
    {
        if (_points.Count == 0) return;
        _cursor = p.Shift ? ConstrainAngle(_points[^1], p.Pixel) : p.Pixel;
        Editor.InvalidateOverlay();
    }

    public override void Move(ToolPointer p) => Hover(p);

    public override bool DoubleClick(ToolPointer p)
    {
        if (_points.Count == 0) return false;
        Finish();
        return true;
    }

    private void Finish()
    {
        if (Sprite != null && _points.Count >= 3) Apply(SelectionMask.FromPolygon(Sprite.Width, Sprite.Height, _points), _mode);
        _points.Clear();
        Editor.InvalidateOverlay();
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if (_points.Count == 0) return false;
        switch (key)
        {
            case Key.Enter:
                Finish();
                return true;
            case Key.Escape:
                Cancel();
                return true;
            case Key.Back:
            case Key.Delete:
                _points.RemoveAt(_points.Count - 1);
                Editor.InvalidateOverlay();
                return true;
        }
        return false;
    }

    public override void Cancel()
    {
        _points.Clear();
        Editor.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext overlay)
    {
        if (_points.Count == 0) return;
        List<SKPointI> preview = [.. _points, _cursor];
        overlay.PixelPath(preview, false);
    }

    public override void BuildOptions(OptionsBuilder options) => ModeOptions(options);
}

public sealed class MagicWandTool : SelectToolBase
{
    public MagicWandTool(PixelEditorWindow editor) : base(editor, ToolId.MagicWand, Strings.Tool_MagicWand, Strings.Tool_MagicWand_Hint, PixelEditorIcons.MagicWand, Key.W)
    {
    }

    public override void Down(ToolPointer p)
    {
        if (Sprite == null || !Sprite.Bounds.Contains(p.X, p.Y)) return;
        Editor.CommitFloating();
        PixelBuffer? source = Settings.WandAllLayers || Sprite.ActiveLayer is not PixelLayer layer
            ? Compositor.Flatten(Sprite, Sprite.ActiveFrame, true)
            : Sprite.GetBuffer(layer, Sprite.ActiveFrame) ?? new PixelBuffer(Sprite.Width, Sprite.Height);
        bool[] region = Raster.FloodRegion(source, p.X, p.Y, Settings.WandTolerance, Settings.WandContiguous, false);
        Apply(new SelectionMask(Sprite.Width, Sprite.Height, region), ModeFor(p));
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        ModeOptions(options);
        options.Separator();
        options.Number(Strings.Options_Tolerance, 0, 255, Settings.WandTolerance, v => Settings.WandTolerance = v);
        options.Check(Strings.Options_Contiguous, Settings.WandContiguous, v => Settings.WandContiguous = v, Strings.Options_Contiguous_Tip);
        options.Check(Strings.Options_AllLayers, Settings.WandAllLayers, v => Settings.WandAllLayers = v, Strings.Options_SampleAllLayers_Tip);
    }
}
