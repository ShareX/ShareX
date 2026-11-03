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

public sealed class LassoTool : SelectionToolBase
{
    private readonly List<SKPoint> _points = [];
    private bool _dragging;
    private SelectionMode _mode;

    public LassoTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Lasso;

    public override string Name => "Lasso Tool";

    public override string Hint => "Drag to draw a freehand selection. Release to close it.";

    public override ToolCursor Cursor => ToolCursor.Precise;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        if (TryBeginMove(input)) return;
        _mode = ResolveMode(input);
        _points.Clear();
        _points.Add(input.Doc);
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (IsMovingSelection)
        {
            UpdateMove(input);
            return;
        }
        if (!_dragging) return;
        if (SKPoint.Distance(_points[^1], input.Doc) >= 0.75f) _points.Add(input.Doc);
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (IsMovingSelection)
        {
            EndMove();
            return;
        }
        if (!_dragging) return;
        _dragging = false;
        if (_points.Count < 3)
        {
            DeselectOnClick(_mode);
            return;
        }
        using SKPath path = new();
        path.AddPoly(_points.ToArray(), true);
        _points.Clear();
        ApplyPath(path, _mode, "Lasso");
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_dragging) context.Polygon(_points, false);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        BuildCommonOptions(options);
        options.Separator();
        options.Button("Select and Mask...", () => UI.Commands.EditorCommands.SelectAndMask(Host));
    }
}

public sealed class PolygonalLassoTool : SelectionToolBase
{
    private readonly List<SKPoint> _points = [];
    private SKPoint _hover;
    private SelectionMode _mode;

    public PolygonalLassoTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.PolygonalLasso;

    public override string Name => "Polygonal Lasso Tool";

    public override string Hint => "Click to add points. Double-click, Enter or click the start point to close. Backspace removes the last point.";

    public override ToolCursor Cursor => ToolCursor.Precise;

    public override bool HasPendingOperation => _points.Count > 0;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        SKPoint p = input.Shift && _points.Count > 0 ? Constrain45(_points[^1], input.Doc) : input.Doc;
        if (_points.Count == 0)
        {
            _mode = ResolveMode(input);
            _points.Add(p);
        }
        else if (input.ClickCount >= 2 || _points.Count > 2 && SKPoint.Distance(p, _points[0]) * Host.View!.Zoom < 8)
        {
            Commit();
            return;
        }
        else
        {
            _points.Add(p);
        }
        Host.InvalidateOverlay();
    }

    public override void PointerMove(ToolInput input)
    {
        _hover = input.Shift && _points.Count > 0 ? Constrain45(_points[^1], input.Doc) : input.Doc;
        if (_points.Count > 0) Host.InvalidateOverlay();
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if ((key == Key.Back || key == Key.Delete) && _points.Count > 0)
        {
            _points.RemoveAt(_points.Count - 1);
            Host.InvalidateOverlay();
            return true;
        }
        return base.KeyDown(key, modifiers);
    }

    public override void Commit()
    {
        if (_points.Count < 3)
        {
            Cancel();
            return;
        }
        using SKPath path = new();
        path.AddPoly(_points.ToArray(), true);
        _points.Clear();
        ApplyPath(path, _mode, "Polygonal Lasso");
    }

    public override void Cancel()
    {
        _points.Clear();
        Host.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_points.Count == 0) return;
        context.Polygon(_points.Append(_hover).ToList(), false);
        context.Handle(_points[0], true, 6);
    }

    public override void BuildOptions(OptionsBuilder options) => BuildCommonOptions(options);
}

/// <summary>Lasso that snaps to edges using live-wire (intelligent scissors) path finding.</summary>
public sealed class MagneticLassoTool : SelectionToolBase
{
    private readonly List<SKPoint> _committed = [];
    private readonly List<SKPointI> _anchors = [];
    private List<SKPointI> _live = [];
    private LiveWire? _wire;
    private SelectionMode _mode;
    private bool _active;

    public MagneticLassoTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.MagneticLasso;

    public override string Name => "Magnetic Lasso Tool";

    public override string Hint => "Click to start, then move along an edge. Click to add anchors, double-click or Enter to close, Backspace removes an anchor.";

    public override ToolCursor Cursor => ToolCursor.Precise;

    public override bool HasPendingOperation => _active;

    public int Width { get; set; } = 10;

    public int Contrast { get; set; } = 10;

    public int Frequency { get; set; } = 57;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        SKPointI p = input.DocPixel;
        if (!_active)
        {
            _mode = ResolveMode(input);
            using SKBitmap sample = Host.GetSampleImage(true);
            _wire = new LiveWire(sample);
            SKPointI start = _wire.Snap(p, Width);
            _anchors.Clear();
            _committed.Clear();
            _anchors.Add(start);
            _committed.Add(new SKPoint(start.X + 0.5f, start.Y + 0.5f));
            _live = [];
            _active = true;
            return;
        }
        if (input.ClickCount >= 2)
        {
            Commit();
            return;
        }
        AddAnchor(p);
        if (_anchors.Count > 2 && SKPoint.Distance(input.Doc, _committed[0]) * Host.View!.Zoom < 8) Commit();
    }

    private void AddAnchor(SKPointI target)
    {
        if (_wire == null) return;
        SKPointI snapped = _wire.Snap(target, Width);
        List<SKPointI> path = _wire.FindPath(_anchors[^1], snapped);
        foreach (SKPointI p in path.Skip(1)) _committed.Add(new SKPoint(p.X + 0.5f, p.Y + 0.5f));
        _anchors.Add(snapped);
        _live = [];
        Host.InvalidateOverlay();
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_active || _wire == null) return;
        SKPointI snapped = _wire.Snap(input.DocPixel, Width);
        _live = _wire.FindPath(_anchors[^1], snapped);
        // Automatically drop anchors as the path gets long (frequency).
        float spacing = 20 + (100 - Frequency) * 1.5f;
        if (_live.Count > spacing) AddAnchor(_live[(int)(spacing * 0.7f)]);
        Host.InvalidateOverlay();
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if ((key == Key.Back || key == Key.Delete) && _active && _anchors.Count > 1)
        {
            _anchors.RemoveAt(_anchors.Count - 1);
            // Rebuild committed points from anchors.
            _committed.Clear();
            _committed.Add(new SKPoint(_anchors[0].X + 0.5f, _anchors[0].Y + 0.5f));
            for (int i = 1; i < _anchors.Count; i++)
            {
                foreach (SKPointI p in _wire!.FindPath(_anchors[i - 1], _anchors[i]).Skip(1)) _committed.Add(new SKPoint(p.X + 0.5f, p.Y + 0.5f));
            }
            Host.InvalidateOverlay();
            return true;
        }
        return base.KeyDown(key, modifiers);
    }

    public override void Commit()
    {
        if (!_active || _wire == null) return;
        List<SKPointI> closing = _wire.FindPath(_anchors[^1], _anchors[0]);
        List<SKPoint> points = _committed.Concat(closing.Select(p => new SKPoint(p.X + 0.5f, p.Y + 0.5f))).ToList();
        _active = false;
        _live = [];
        Host.InvalidateOverlay();
        if (points.Count < 3) return;
        using SKPath path = new();
        path.AddPoly(points.ToArray(), true);
        ApplyPath(path, _mode, "Magnetic Lasso");
    }

    public override void Cancel()
    {
        _active = false;
        _committed.Clear();
        _live = [];
        Host.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_active) return;
        context.Polygon(_committed.Concat(_live.Select(p => new SKPoint(p.X + 0.5f, p.Y + 0.5f))).ToList(), false);
        foreach (SKPointI a in _anchors) context.Handle(new SKPoint(a.X + 0.5f, a.Y + 0.5f), false, 5);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        BuildCommonOptions(options);
        options.Separator();
        options.Number("Width:", 1, 256, () => Width, v => Width = (int)v, "0", "px");
        options.Number("Contrast:", 1, 100, () => Contrast, v => Contrast = (int)v, "0", "%");
        options.Number("Frequency:", 0, 100, () => Frequency, v => Frequency = (int)v);
    }
}
