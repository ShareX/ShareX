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

public enum PathToolMode
{
    Shape,
    Path
}

/// <summary>Shared helpers for tools that create or edit vector paths.</summary>
public abstract class PathToolBase : ToolBase
{
    protected PathToolBase(IEditorHost host) : base(host)
    {
    }

    public override ToolCursor Cursor => ToolCursor.Pen;

    protected float Tolerance => 6 / (float)(Host.View?.Zoom ?? 1);

    /// <summary>The path being edited: the active shape layer's path, the selected path, or the work path.</summary>
    protected VectorPath? GetTargetPath(bool create, PathToolMode mode = PathToolMode.Path)
    {
        PhotoDocument? doc = Doc;
        if (doc == null) return null;
        if (doc.ActiveLayer is ShapeLayer shape && (mode == PathToolMode.Shape || doc.ActivePath == null)) return shape.Path;
        if (doc.ActivePath != null) return doc.ActivePath.Path;
        if (doc.WorkPath != null)
        {
            doc.ActivePath = new NamedPath("Work Path", doc.WorkPath);
            return doc.WorkPath;
        }
        if (!create) return null;
        doc.WorkPath = new VectorPath();
        doc.ActivePath = new NamedPath("Work Path", doc.WorkPath);
        return doc.WorkPath;
    }

    protected void PathChanged()
    {
        if (Doc?.ActiveLayer is ShapeLayer) Host.InvalidateCanvas();
        else Host.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        VectorPath? path = GetTargetPath(false);
        if (path == null || path.IsEmpty) return;
        using SKPath skPath = path.ToSKPath();
        context.Path(skPath);
        context.PathAnchors(path, SelectedAnchor);
    }

    protected virtual PathAnchor? SelectedAnchor => null;

    /// <summary>Creates a shape layer filled with the foreground color for a new path.</summary>
    protected ShapeLayer CreateShapeLayer(VectorPath path)
    {
        PhotoDocument doc = Doc!;
        int number = doc.AllLayers.OfType<ShapeLayer>().Count() + 1;
        ShapeLayer layer = new($"Shape {number}", path) { FillColor = Host.Foreground };
        doc.ActivePath = null;
        doc.AddLayer(layer);
        return layer;
    }
}

public sealed class PenTool : PathToolBase
{
    private SubPath? _active;
    private PathAnchor? _dragAnchor;
    private SKPoint _hover;
    private bool _directDrag;
    private PathAnchor? _directAnchor;
    private int _directPart;
    private bool _changed;

    public PenTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Pen;

    public override string Name => "Pen Tool";

    public override string Hint => "Click to add corner points, drag to add smooth points. Click the first point to close. Ctrl edits points, Alt converts them.";

    public PathToolMode Mode { get; set; } = PathToolMode.Path;

    public bool AutoAddDelete { get; set; } = true;

    public bool RubberBand { get; set; } = true;

    public override bool HasPendingOperation => _active != null;

    protected override PathAnchor? SelectedAnchor => _active?.Anchors.LastOrDefault();

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        VectorPath? path = GetTargetPath(false, Mode);

        // Ctrl: temporary direct selection of anchors and handles.
        if (input.Ctrl && path != null)
        {
            foreach (PathAnchor a in path.AllAnchors)
            {
                if (SKPoint.Distance(a.In, input.Doc) < Tolerance && a.HasIn) { Begin(a, 1); return; }
                if (SKPoint.Distance(a.Out, input.Doc) < Tolerance && a.HasOut) { Begin(a, 2); return; }
                if (SKPoint.Distance(a.Point, input.Doc) < Tolerance) { Begin(a, 0); return; }
            }
            return;
        }

        if (input.Alt && path?.HitAnchor(input.Doc, Tolerance) is (var convertSub, var convertIndex))
        {
            PathAnchor anchor = convertSub.Anchors[convertIndex];
            if (anchor.HasIn || anchor.HasOut)
            {
                anchor.In = anchor.Out = anchor.Point;
                anchor.Smooth = false;
                PathChanged();
                PushHistory("Convert Point");
                return;
            }
            _dragAnchor = anchor;
            _changed = true;
            return;
        }

        if (_active != null && _active.Anchors.Count > 1 && SKPoint.Distance(_active.Anchors[0].Point, input.Doc) < Tolerance)
        {
            _active.Closed = true;
            _dragAnchor = _active.Anchors[0];
            _active = null;
            _changed = true;
            PathChanged();
            return;
        }

        if (_active == null && AutoAddDelete && path != null)
        {
            if (path.HitAnchor(input.Doc, Tolerance) is (var sub, var index))
            {
                sub.Anchors.RemoveAt(index);
                if (sub.Anchors.Count < 2) path.SubPaths.Remove(sub);
                PathChanged();
                PushHistory("Delete Anchor Point");
                return;
            }
            if (path.HitSegment(input.Doc, Tolerance) is (var segmentSub, var segment, var t))
            {
                path.InsertAnchor(segmentSub, segment, t);
                PathChanged();
                PushHistory("Add Anchor Point");
                return;
            }
        }

        SKPoint point = input.Shift && _active?.Anchors.Count > 0 ? Constrain45(_active.Anchors[^1].Point, input.Doc) : input.Doc;
        if (_active == null)
        {
            if (Mode == PathToolMode.Shape && Doc.ActiveLayer is not ShapeLayer)
            {
                VectorPath shapePath = new();
                CreateShapeLayer(shapePath);
                path = shapePath;
            }
            else
            {
                path = GetTargetPath(true, Mode);
            }
            _active = new SubPath();
            path!.SubPaths.Add(_active);
        }
        PathAnchor added = new(point);
        _active.Anchors.Add(added);
        _dragAnchor = added;
        _changed = true;
        PathChanged();
    }

    private void Begin(PathAnchor anchor, int part)
    {
        _directDrag = true;
        _directAnchor = anchor;
        _directPart = part;
    }

    public override void PointerMove(ToolInput input)
    {
        _hover = input.Doc;
        if (_directDrag && _directAnchor != null)
        {
            DirectSelectionTool.MovePart(_directAnchor, _directPart, input.Doc, input.Alt);
            _changed = true;
            PathChanged();
            return;
        }
        if (_dragAnchor != null && input.IsPressed)
        {
            // Dragging pulls out symmetric smooth handles.
            _dragAnchor.Out = input.Doc;
            _dragAnchor.In = _dragAnchor.Point - (input.Doc - _dragAnchor.Point);
            _dragAnchor.Smooth = true;
            PathChanged();
            return;
        }
        if (_active != null && RubberBand) Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        _dragAnchor = null;
        if (_directDrag)
        {
            _directDrag = false;
            _directAnchor = null;
        }
        if (_changed)
        {
            _changed = false;
            PushHistory(_active == null ? "Close Path" : "Pen");
        }
    }

    public override void Commit()
    {
        _active = null;
        Host.InvalidateOverlay();
    }

    public override void Cancel() => Commit();

    public override void DrawOverlay(OverlayContext context)
    {
        base.DrawOverlay(context);
        if (_active != null && RubberBand && _active.Anchors.Count > 0)
        {
            PathAnchor last = _active.Anchors[^1];
            VectorPath preview = new();
            SubPath sub = new();
            sub.Anchors.Add(last.Clone());
            sub.Anchors.Add(new PathAnchor(_hover));
            preview.SubPaths.Add(sub);
            using SKPath skPath = preview.ToSKPath();
            context.Path(skPath);
        }
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.EnumCombo("Mode:", () => Mode, v => Mode = v, 90);
        options.Check("Auto Add/Delete", () => AutoAddDelete, v => AutoAddDelete = v);
        options.Check("Rubber Band", () => RubberBand, v => RubberBand = v);
        PathOperationButtons(options, Host);
    }

    /// <summary>Make Selection / Mask / Shape buttons shown by pen tools.</summary>
    public static void PathOperationButtons(OptionsBuilder options, IEditorHost host)
    {
        options.Separator();
        options.Label("Make:");
        options.Button("Selection...", () => UI.Commands.EditorCommands.PathToSelection(host));
        options.Button("Mask", () => UI.Commands.EditorCommands.PathToVectorMask(host));
        options.Button("Shape", () => UI.Commands.EditorCommands.PathToShape(host));
    }
}

public sealed class FreeformPenTool : PathToolBase
{
    private readonly List<SKPoint> _points = [];
    private bool _drawing;
    private LiveWire? _wire;

    public FreeformPenTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.FreeformPen;

    public override string Name => "Freeform Pen Tool";

    public override string Hint => "Drag to draw a path freehand; it is fitted with smooth curves. Magnetic snaps to edges.";

    public PathToolMode Mode { get; set; } = PathToolMode.Path;

    public float CurveFit { get; set; } = 2;

    public bool Magnetic { get; set; }

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _points.Clear();
        _points.Add(input.Doc);
        _drawing = true;
        if (Magnetic)
        {
            using SKBitmap sample = Host.GetSampleImage(true);
            _wire = new LiveWire(sample);
        }
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_drawing) return;
        SKPoint p = input.Doc;
        if (_wire != null)
        {
            SKPointI snapped = _wire.Snap(new SKPointI((int)p.X, (int)p.Y), 6);
            p = new SKPoint(snapped.X + 0.5f, snapped.Y + 0.5f);
        }
        if (SKPoint.Distance(_points[^1], p) >= 1) _points.Add(p);
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_drawing || Doc == null) return;
        _drawing = false;
        _wire = null;
        if (_points.Count < 2) return;
        bool closed = _points.Count > 3 && SKPoint.Distance(_points[0], _points[^1]) < Tolerance * 2;
        SubPath sub = VectorPath.FromFreehand(_points, CurveFit, closed);
        VectorPath? path;
        if (Mode == PathToolMode.Shape && Doc.ActiveLayer is not ShapeLayer)
        {
            path = new VectorPath();
            CreateShapeLayer(path);
        }
        else
        {
            path = GetTargetPath(true, Mode);
        }
        path!.SubPaths.Add(sub);
        _points.Clear();
        PathChanged();
        PushHistory("Freeform Pen");
    }

    public override void DrawOverlay(OverlayContext context)
    {
        base.DrawOverlay(context);
        if (_drawing) context.Polygon(_points, false, OverlayContext.AccentPen);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.EnumCombo("Mode:", () => Mode, v => Mode = v, 90);
        options.Number("Curve Fit:", 0.5, 10, () => CurveFit, v => CurveFit = (float)v, "0.0", "px", 0.5);
        options.Check("Magnetic", () => Magnetic, v => Magnetic = v);
        PenTool.PathOperationButtons(options, Host);
    }
}

public sealed class CurvaturePenTool : PathToolBase
{
    private readonly List<SKPoint> _points = [];
    private SubPath? _active;
    private VectorPath? _path;
    private SKPoint _hover;

    public CurvaturePenTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.CurvaturePen;

    public override string Name => "Curvature Pen Tool";

    public override string Hint => "Click to place points; the curve flows smoothly through them. Click the first point to close, Enter or Esc to finish.";

    public PathToolMode Mode { get; set; } = PathToolMode.Path;

    public override bool HasPendingOperation => _active != null;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        if (_active != null && _points.Count > 2 && SKPoint.Distance(_points[0], input.Doc) < Tolerance)
        {
            Rebuild(true);
            Finish();
            return;
        }
        if (_active == null)
        {
            if (Mode == PathToolMode.Shape && Doc.ActiveLayer is not ShapeLayer)
            {
                _path = new VectorPath();
                CreateShapeLayer(_path);
            }
            else
            {
                _path = GetTargetPath(true, Mode);
            }
            _active = new SubPath();
            _path!.SubPaths.Add(_active);
            _points.Clear();
        }
        _points.Add(input.Doc);
        if (input.ClickCount >= 2)
        {
            Rebuild(false);
            Finish();
            return;
        }
        Rebuild(false);
    }

    private void Rebuild(bool closed)
    {
        if (_active == null) return;
        SubPath rebuilt = VectorPath.FromCurvaturePoints(_points, closed);
        _active.Anchors.Clear();
        _active.Anchors.AddRange(rebuilt.Anchors);
        _active.Closed = closed;
        PathChanged();
    }

    private void Finish()
    {
        _active = null;
        _points.Clear();
        PushHistory("Curvature Pen");
        PathChanged();
    }

    public override void PointerMove(ToolInput input)
    {
        _hover = input.Doc;
        if (_active != null) Host.InvalidateOverlay();
    }

    public override void Commit()
    {
        if (_active == null) return;
        Finish();
    }

    public override void Cancel() => Commit();

    public override void DrawOverlay(OverlayContext context)
    {
        base.DrawOverlay(context);
        if (_active == null || _points.Count == 0) return;
        SubPath preview = VectorPath.FromCurvaturePoints(_points.Append(_hover).ToList(), false);
        VectorPath previewPath = new();
        previewPath.SubPaths.Add(preview);
        using SKPath skPath = previewPath.ToSKPath();
        context.Path(skPath);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.EnumCombo("Mode:", () => Mode, v => Mode = v, 90);
        PenTool.PathOperationButtons(options, Host);
    }
}

public sealed class AnchorPointTool : PathToolBase
{
    private readonly ToolId _id;
    private PathAnchor? _converting;
    private int _part;
    private bool _changed;

    public AnchorPointTool(IEditorHost host, ToolId id) : base(host)
    {
        _id = id;
    }

    public override ToolId Id => _id;

    public override string Name => _id switch
    {
        ToolId.AddAnchorPoint => "Add Anchor Point Tool",
        ToolId.DeleteAnchorPoint => "Delete Anchor Point Tool",
        _ => "Convert Point Tool"
    };

    public override string Hint => _id switch
    {
        ToolId.AddAnchorPoint => "Click a path segment to add an anchor point.",
        ToolId.DeleteAnchorPoint => "Click an anchor point to delete it.",
        _ => "Click a smooth point to make it a corner; drag a corner point to pull out smooth handles. Drag a handle to break it."
    };

    public override void PointerDown(ToolInput input)
    {
        VectorPath? path = GetTargetPath(false);
        if (path == null || !input.LeftButton) return;
        switch (_id)
        {
            case ToolId.AddAnchorPoint:
                if (path.HitSegment(input.Doc, Tolerance) is (var sub, var segment, var t))
                {
                    path.InsertAnchor(sub, segment, t);
                    PathChanged();
                    PushHistory("Add Anchor Point");
                }
                break;
            case ToolId.DeleteAnchorPoint:
                if (path.HitAnchor(input.Doc, Tolerance) is (var deleteSub, var index))
                {
                    deleteSub.Anchors.RemoveAt(index);
                    if (deleteSub.Anchors.Count < 2) path.SubPaths.Remove(deleteSub);
                    PathChanged();
                    PushHistory("Delete Anchor Point");
                }
                break;
            default:
                foreach (PathAnchor a in path.AllAnchors)
                {
                    if (a.HasIn && SKPoint.Distance(a.In, input.Doc) < Tolerance) { _converting = a; _part = 1; return; }
                    if (a.HasOut && SKPoint.Distance(a.Out, input.Doc) < Tolerance) { _converting = a; _part = 2; return; }
                }
                if (path.HitAnchor(input.Doc, Tolerance) is (var convertSub, var convertIndex))
                {
                    PathAnchor anchor = convertSub.Anchors[convertIndex];
                    if (anchor.HasIn || anchor.HasOut)
                    {
                        anchor.In = anchor.Out = anchor.Point;
                        anchor.Smooth = false;
                        PathChanged();
                        PushHistory("Convert Point");
                    }
                    else
                    {
                        _converting = anchor;
                        _part = 0;
                    }
                }
                break;
        }
    }

    public override void PointerMove(ToolInput input)
    {
        if (_converting == null) return;
        if (_part == 0)
        {
            _converting.Out = input.Doc;
            _converting.In = _converting.Point - (input.Doc - _converting.Point);
            _converting.Smooth = true;
        }
        else
        {
            // Dragging a handle with the convert tool breaks the smooth link.
            _converting.Smooth = false;
            if (_part == 1) _converting.In = input.Doc;
            else _converting.Out = input.Doc;
        }
        _changed = true;
        PathChanged();
    }

    public override void PointerUp(ToolInput input)
    {
        _converting = null;
        if (_changed)
        {
            _changed = false;
            PushHistory("Convert Point");
        }
    }
}

public sealed class PathSelectionTool : PathToolBase
{
    private readonly HashSet<SubPath> _selected = [];
    private SKPoint _last;
    private bool _dragging;
    private bool _moved;

    public PathSelectionTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.PathSelection;

    public override string Name => "Path Selection Tool";

    public override string Hint => "Click a path component to select it, drag to move it. Alt-drag duplicates, Delete removes.";

    public override ToolCursor Cursor => ToolCursor.Arrow;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        VectorPath? path = GetTargetPath(false);
        // Clicking a shape layer's path on canvas activates that layer.
        if (path?.HitSubPath(input.Doc, Tolerance) == null)
        {
            ShapeLayer? hit = Doc.AllLayers.OfType<ShapeLayer>().LastOrDefault(s => s.IsEffectivelyVisible && s.Path.HitSubPath(input.Doc, Tolerance) != null);
            if (hit != null)
            {
                Doc.ActiveLayer = hit;
                Doc.ActivePath = null;
                Host.RefreshPanels();
                path = hit.Path;
            }
        }
        SubPath? sub = path?.HitSubPath(input.Doc, Tolerance);
        if (!input.Shift) _selected.Clear();
        if (sub != null)
        {
            if (input.Alt)
            {
                SubPath copy = sub.Clone();
                path!.SubPaths.Add(copy);
                sub = copy;
            }
            _selected.Add(sub);
            _dragging = true;
            _last = input.Doc;
        }
        Host.InvalidateOverlay();
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        SKPoint delta = input.Doc - _last;
        _last = input.Doc;
        foreach (SubPath sub in _selected)
            foreach (PathAnchor anchor in sub.Anchors) anchor.Offset(delta);
        _moved = true;
        PathChanged();
    }

    public override void PointerUp(ToolInput input)
    {
        _dragging = false;
        if (_moved)
        {
            _moved = false;
            PushHistory("Move Path");
        }
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if ((key == Key.Delete || key == Key.Back) && _selected.Count > 0)
        {
            VectorPath? path = GetTargetPath(false);
            if (path != null)
            {
                path.SubPaths.RemoveAll(_selected.Contains);
                _selected.Clear();
                PathChanged();
                PushHistory("Delete Path");
                return true;
            }
        }
        return base.KeyDown(key, modifiers);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        VectorPath? path = GetTargetPath(false);
        if (path == null) return;
        using SKPath skPath = path.ToSKPath();
        context.Path(skPath);
        foreach (SubPath sub in _selected)
            foreach (PathAnchor anchor in sub.Anchors) context.Handle(anchor.Point, true, 6);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Button("Combine", () => Combine(SKPathOp.Union), "Merge overlapping path components");
        options.Button("Subtract", () => Combine(SKPathOp.Difference), "Subtract later components from the first");
        options.Button("Intersect", () => Combine(SKPathOp.Intersect));
        options.Button("Exclude", () => Combine(SKPathOp.Xor));
        PenTool.PathOperationButtons(options, Host);
    }

    private void Combine(SKPathOp op)
    {
        VectorPath? path = GetTargetPath(false);
        if (path == null || path.SubPaths.Count < 2) return;
        SKPath? result = null;
        foreach (SubPath sub in path.SubPaths)
        {
            VectorPath single = new();
            single.SubPaths.Add(sub);
            SKPath piece = single.ToSKPath();
            if (result == null)
            {
                result = piece;
                continue;
            }
            SKPath? combined = result.Op(piece, op);
            result.Dispose();
            piece.Dispose();
            result = combined ?? new SKPath();
        }
        if (result == null) return;
        VectorPath merged = VectorPath.FromSKPath(result);
        result.Dispose();
        path.SubPaths.Clear();
        path.SubPaths.AddRange(merged.SubPaths);
        path.FillType = merged.FillType;
        _selected.Clear();
        PathChanged();
        PushHistory("Combine Shapes");
    }
}

public sealed class DirectSelectionTool : PathToolBase
{
    private readonly List<(PathAnchor Anchor, int Part)> _selected = [];
    private SKPoint _last;
    private bool _dragging;
    private bool _moved;
    private SKPoint? _marqueeStart;
    private SKPoint _marqueeEnd;

    public DirectSelectionTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.DirectSelection;

    public override string Name => "Direct Selection Tool";

    public override string Hint => "Click or drag a marquee to select anchor points; drag points or handles to reshape. Alt drags a handle independently.";

    public override ToolCursor Cursor => ToolCursor.Arrow;

    protected override PathAnchor? SelectedAnchor => _selected.Count > 0 ? _selected[0].Anchor : null;

    /// <summary>Moves an anchor (part 0) or one of its handles (1 = in, 2 = out), keeping smooth points collinear.</summary>
    public static void MovePart(PathAnchor anchor, int part, SKPoint position, bool breakSmooth)
    {
        if (part == 0)
        {
            anchor.Offset(position - anchor.Point);
            return;
        }
        if (breakSmooth) anchor.Smooth = false;
        if (part == 1)
        {
            anchor.In = position;
            if (anchor.Smooth && anchor.HasOut)
            {
                SKPoint dir = anchor.Point - position;
                float length = SKPoint.Distance(anchor.Out, anchor.Point);
                float d = Math.Max(1e-3f, dir.Length);
                anchor.Out = anchor.Point + new SKPoint(dir.X / d * length, dir.Y / d * length);
            }
        }
        else
        {
            anchor.Out = position;
            if (anchor.Smooth && anchor.HasIn)
            {
                SKPoint dir = anchor.Point - position;
                float length = SKPoint.Distance(anchor.In, anchor.Point);
                float d = Math.Max(1e-3f, dir.Length);
                anchor.In = anchor.Point + new SKPoint(dir.X / d * length, dir.Y / d * length);
            }
        }
    }

    public override void PointerDown(ToolInput input)
    {
        VectorPath? path = GetTargetPath(false);
        if (Doc == null || !input.LeftButton) return;
        if (path == null)
        {
            ShapeLayer? hit = Doc.AllLayers.OfType<ShapeLayer>().LastOrDefault(s => s.Path.HitSubPath(input.Doc, Tolerance) != null);
            if (hit == null) return;
            Doc.ActiveLayer = hit;
            Host.RefreshPanels();
            path = hit.Path;
        }
        (PathAnchor, int)? found = null;
        foreach (PathAnchor a in path.AllAnchors)
        {
            if (a.HasIn && SKPoint.Distance(a.In, input.Doc) < Tolerance) { found = (a, 1); break; }
            if (a.HasOut && SKPoint.Distance(a.Out, input.Doc) < Tolerance) { found = (a, 2); break; }
            if (SKPoint.Distance(a.Point, input.Doc) < Tolerance) { found = (a, 0); break; }
        }
        if (found is (PathAnchor anchor, int part))
        {
            if (!input.Shift && !_selected.Any(s => s.Anchor == anchor)) _selected.Clear();
            if (part != 0) _selected.RemoveAll(s => s.Anchor == anchor);
            if (!_selected.Any(s => s.Anchor == anchor && s.Part == part)) _selected.Add((anchor, part));
            _dragging = true;
            _last = input.Doc;
        }
        else
        {
            if (!input.Shift) _selected.Clear();
            _marqueeStart = input.Doc;
            _marqueeEnd = input.Doc;
        }
        Host.InvalidateOverlay();
    }

    public override void PointerMove(ToolInput input)
    {
        if (_marqueeStart != null)
        {
            _marqueeEnd = input.Doc;
            Host.InvalidateOverlay();
            return;
        }
        if (!_dragging) return;
        SKPoint delta = input.Doc - _last;
        _last = input.Doc;
        foreach (var (anchor, part) in _selected)
        {
            SKPoint target = (part == 0 ? anchor.Point : part == 1 ? anchor.In : anchor.Out) + delta;
            MovePart(anchor, part, target, input.Alt);
        }
        _moved = true;
        PathChanged();
    }

    public override void PointerUp(ToolInput input)
    {
        if (_marqueeStart is SKPoint start)
        {
            SKRect rect = DragRect(start, _marqueeEnd, false, false);
            VectorPath? path = GetTargetPath(false);
            if (path != null)
                foreach (PathAnchor a in path.AllAnchors.Where(a => rect.Contains(a.Point))) _selected.Add((a, 0));
            _marqueeStart = null;
            Host.InvalidateOverlay();
            return;
        }
        _dragging = false;
        if (_moved)
        {
            _moved = false;
            PushHistory("Drag Anchor");
        }
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        VectorPath? path = GetTargetPath(false);
        if ((key == Key.Delete || key == Key.Back) && _selected.Count > 0 && path != null)
        {
            foreach (SubPath sub in path.SubPaths) sub.Anchors.RemoveAll(a => _selected.Any(s => s.Anchor == a && s.Part == 0));
            path.SubPaths.RemoveAll(s => s.Anchors.Count < 2);
            _selected.Clear();
            PathChanged();
            PushHistory("Delete Anchor Point");
            return true;
        }
        float step = modifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;
        SKPoint delta = key switch { Key.Left => new(-step, 0), Key.Right => new(step, 0), Key.Up => new(0, -step), Key.Down => new(0, step), _ => default };
        if (delta != default && _selected.Count > 0)
        {
            foreach (var (anchor, part) in _selected) MovePart(anchor, part, (part == 0 ? anchor.Point : part == 1 ? anchor.In : anchor.Out) + delta, false);
            PathChanged();
            PushHistory("Nudge Anchor");
            return true;
        }
        return base.KeyDown(key, modifiers);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        VectorPath? path = GetTargetPath(false);
        if (path != null)
        {
            using SKPath skPath = path.ToSKPath();
            context.Path(skPath);
            foreach (SubPath sub in path.SubPaths)
            {
                foreach (PathAnchor a in sub.Anchors)
                {
                    bool selected = _selected.Any(s => s.Anchor == a);
                    if (selected || a.Smooth)
                    {
                        if (a.HasIn) { context.Line(a.Point, a.In, OverlayContext.AccentPen); context.Circle(a.In, 3, OverlayContext.AccentPen, OverlayContext.AccentBrush); }
                        if (a.HasOut) { context.Line(a.Point, a.Out, OverlayContext.AccentPen); context.Circle(a.Out, 3, OverlayContext.AccentPen, OverlayContext.AccentBrush); }
                    }
                    context.Handle(a.Point, selected, 6);
                }
            }
        }
        if (_marqueeStart is SKPoint start) context.Rect(DragRect(start, _marqueeEnd, false, false));
    }

    public override void BuildOptions(OptionsBuilder options) => PenTool.PathOperationButtons(options, Host);
}
