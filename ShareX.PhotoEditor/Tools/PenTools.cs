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

using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

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
