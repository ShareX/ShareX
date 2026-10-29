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

public enum MarqueeStyle
{
    Normal,
    FixedRatio,
    FixedSize
}

/// <summary>Shared behavior for tools that create selections: mode, feather, anti-alias and moving the outline.</summary>
public abstract class SelectionToolBase : ToolBase
{
    private SKPoint _moveStart;
    private PixelSurface? _moveOriginal;

    protected SelectionToolBase(IEditorHost host) : base(host)
    {
    }

    public SelectionMode Mode { get; set; } = SelectionMode.Replace;

    public float Feather { get; set; }

    public bool AntiAlias { get; set; } = true;

    protected bool IsMovingSelection => _moveOriginal != null;

    /// <summary>Resolves the selection mode from the options bar and the modifier keys held at press time.</summary>
    protected SelectionMode ResolveMode(ToolInput input)
    {
        if (input.Shift && input.Alt) return SelectionMode.Intersect;
        if (input.Shift) return SelectionMode.Add;
        if (input.Alt) return SelectionMode.Subtract;
        return Mode;
    }

    /// <summary>Starts dragging the selection outline when pressing inside an existing selection in replace mode.</summary>
    protected bool TryBeginMove(ToolInput input)
    {
        if (Doc?.Selection == null || ResolveMode(input) != SelectionMode.Replace) return false;
        if (Doc.SelectionAt(input.DocPixel.X, input.DocPixel.Y) < 0.5f) return false;
        _moveStart = input.Doc;
        _moveOriginal = Doc.Selection;
        return true;
    }

    protected void UpdateMove(ToolInput input)
    {
        if (_moveOriginal == null || Doc == null) return;
        SKPoint delta = input.Doc - _moveStart;
        PixelSurface moved = MaskOps.Transform(_moveOriginal, SKMatrix.CreateTranslation(MathF.Round(delta.X), MathF.Round(delta.Y)), Doc.Width, Doc.Height, false);
        Doc.SetSelection(moved);
        Host.InvalidateOverlay();
    }

    protected void EndMove()
    {
        if (_moveOriginal == null) return;
        _moveOriginal = null;
        PushHistory("Move Selection");
    }

    /// <summary>Combines a shape mask with the current selection and records history.</summary>
    protected void ApplySelection(PixelSurface shape, SelectionMode mode, string historyName)
    {
        if (Doc == null) return;
        if (Feather > 0) MaskOps.Blur(shape, Feather);
        PixelSurface? result = MaskOps.Combine(Doc.Selection, shape, mode);
        if (!ReferenceEquals(result, shape) && !ReferenceEquals(result, Doc.Selection)) shape.Dispose();
        Doc.SetSelection(result);
        PushHistory(historyName);
        Host.InvalidateCanvas();
    }

    protected void ApplyPath(SKPath path, SelectionMode mode, string historyName)
    {
        if (Doc == null) return;
        ApplySelection(MaskOps.FromPath(path, Doc.Width, Doc.Height, AntiAlias), mode, historyName);
    }

    protected void BuildCommonOptions(OptionsBuilder options, bool feather = true, bool antiAlias = true)
    {
        options.SelectionMode(() => Mode, m => Mode = m);
        options.Separator();
        if (feather) options.Number("Feather:", 0, 1000, () => Feather, v => Feather = (float)v, "0.#", "px", 0.5);
        if (antiAlias) options.Check("Anti-alias", () => AntiAlias, v => AntiAlias = v);
    }

    /// <summary>Clicking without dragging deselects (in replace mode), like the reference application.</summary>
    protected void DeselectOnClick(SelectionMode mode)
    {
        if (Doc?.Selection != null && mode == SelectionMode.Replace)
        {
            Doc.SetSelection(null);
            PushHistory("Deselect");
            Host.InvalidateCanvas();
        }
    }
}

public sealed class MarqueeTool : SelectionToolBase
{
    private readonly bool _ellipse;
    private SKPoint _start;
    private SKPoint _current;
    private bool _dragging;
    private SelectionMode _mode;
    private bool _square;
    private bool _fromCenter;

    public MarqueeTool(IEditorHost host, bool ellipse) : base(host)
    {
        _ellipse = ellipse;
    }

    public override ToolId Id => _ellipse ? ToolId.EllipticalMarquee : ToolId.RectangularMarquee;

    public override string Name => _ellipse ? "Elliptical Marquee Tool" : "Rectangular Marquee Tool";

    public override string Hint => "Drag to select. Shift constrains, Alt draws from the center. Drag inside a selection to move it.";

    public MarqueeStyle Style { get; set; }

    public float FixedWidth { get; set; } = 64;

    public float FixedHeight { get; set; } = 64;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        if (TryBeginMove(input)) return;
        _mode = ResolveMode(input);
        _start = _current = input.Doc;
        _dragging = true;
        if (Style == MarqueeStyle.FixedSize) _current = _start + new SKPoint(FixedWidth, FixedHeight);
    }

    public override void PointerMove(ToolInput input)
    {
        if (IsMovingSelection)
        {
            UpdateMove(input);
            return;
        }
        if (!_dragging) return;
        _square = input.Shift && _mode != SelectionMode.Add || Style == MarqueeStyle.FixedRatio;
        _fromCenter = input.Alt && _mode != SelectionMode.Subtract;
        _current = Style == MarqueeStyle.FixedSize ? input.Doc : input.Doc;
        Host.InvalidateOverlay();
        SKRect r = GetRect();
        Host.SetStatus($"W: {r.Width:0} px  H: {r.Height:0} px");
    }

    private SKRect GetRect()
    {
        if (Style == MarqueeStyle.FixedSize)
        {
            return _fromCenter ? SKRect.Create(_current.X - FixedWidth / 2, _current.Y - FixedHeight / 2, FixedWidth, FixedHeight) : SKRect.Create(_current.X, _current.Y, FixedWidth, FixedHeight);
        }
        SKPoint end = _current;
        if (Style == MarqueeStyle.FixedRatio && FixedHeight > 0)
        {
            float ratio = FixedWidth / FixedHeight;
            float dx = end.X - _start.X, dy = end.Y - _start.Y;
            float h = MathF.Max(MathF.Abs(dy), MathF.Abs(dx) / ratio);
            end = new SKPoint(_start.X + MathF.Sign(dx == 0 ? 1 : dx) * h * ratio, _start.Y + MathF.Sign(dy == 0 ? 1 : dy) * h);
            return DragRect(_start, end, false, _fromCenter);
        }
        SKRect rect = DragRect(_start, end, _square, _fromCenter);
        return _ellipse ? rect : new SKRect(MathF.Round(rect.Left), MathF.Round(rect.Top), MathF.Round(rect.Right), MathF.Round(rect.Bottom));
    }

    public override void PointerUp(ToolInput input)
    {
        if (IsMovingSelection)
        {
            EndMove();
            return;
        }
        if (!_dragging || Doc == null) return;
        _dragging = false;
        SKRect rect = GetRect();
        Host.InvalidateOverlay();
        if (rect.Width < 1 || rect.Height < 1)
        {
            DeselectOnClick(_mode);
            return;
        }
        using SKPath path = new();
        if (_ellipse) path.AddOval(rect);
        else path.AddRect(rect);
        bool aa = AntiAlias;
        if (!_ellipse) AntiAlias = false;
        ApplyPath(path, _mode, _ellipse ? "Elliptical Marquee" : "Rectangular Marquee");
        AntiAlias = aa;
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging) return;
        SKRect rect = GetRect();
        if (_ellipse) context.Ellipse(rect);
        else context.Rect(rect);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        BuildCommonOptions(options, true, _ellipse);
        options.Separator();
        options.EnumCombo("Style:", () => Style, v => Style = v, 110);
        options.Number("W:", 1, 30000, () => FixedWidth, v => FixedWidth = (float)v, "0", null, 1, 64);
        options.Number("H:", 1, 30000, () => FixedHeight, v => FixedHeight = (float)v, "0", null, 1, 64);
        options.Separator();
        options.Button("Select and Mask...", () => UI.Commands.EditorCommands.SelectAndMask(Host), "Refine the selection edge");
    }
}

/// <summary>Single row or single column marquee: selects a 1 pixel line across the document.</summary>
public sealed class SingleLineMarqueeTool : SelectionToolBase
{
    private readonly bool _row;
    private SKPoint? _hover;

    public SingleLineMarqueeTool(IEditorHost host, bool row) : base(host)
    {
        _row = row;
    }

    public override ToolId Id => _row ? ToolId.SingleRowMarquee : ToolId.SingleColumnMarquee;

    public override string Name => _row ? "Single Row Marquee Tool" : "Single Column Marquee Tool";

    public override string Hint => "Click to select a 1 pixel " + (_row ? "row" : "column") + ".";

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        SKPointI p = input.DocPixel;
        SKRectI rect = _row ? new SKRectI(0, p.Y, Doc.Width, p.Y + 1) : new SKRectI(p.X, 0, p.X + 1, Doc.Height);
        ApplySelection(MaskOps.FromRect(rect, Doc.Width, Doc.Height), ResolveMode(input), Name.Replace(" Tool", string.Empty));
    }

    public override void PointerMove(ToolInput input)
    {
        _hover = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_hover is not SKPoint p || Doc == null) return;
        if (_row) context.Line(new SKPoint(0, MathF.Floor(p.Y) + 0.5f), new SKPoint(Doc.Width, MathF.Floor(p.Y) + 0.5f));
        else context.Line(new SKPoint(MathF.Floor(p.X) + 0.5f, 0), new SKPoint(MathF.Floor(p.X) + 0.5f, Doc.Height));
    }

    public override void BuildOptions(OptionsBuilder options) => BuildCommonOptions(options, false, false);
}

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

public sealed class MagicWandTool : SelectionToolBase
{
    public MagicWandTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.MagicWand;

    public override string Name => "Magic Wand Tool";

    public override string Hint => "Click to select similar colors. Shift adds, Alt subtracts.";

    public int Tolerance { get; set; } = 32;

    public bool Contiguous { get; set; } = true;

    public bool SampleAllLayers { get; set; }

    public int SampleSize { get; set; } = 1;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        SKPointI p = input.DocPixel;
        if (p.X < 0 || p.Y < 0 || p.X >= Doc.Width || p.Y >= Doc.Height) return;
        SelectionMode mode = ResolveMode(input);
        using SKBitmap sample = Host.GetSampleImage(SampleAllLayers);
        SKColor average = ImageOps.SampleAverage(sample, p.X, p.Y, SampleSize);
        uint seed = ColorMath.FromColor(average);
        PixelSurface mask = FloodFill.SelectColor(sample, seed, Tolerance, Contiguous ? (p.X, p.Y) : null, AntiAlias);
        ApplySelection(mask, mode, "Magic Wand");
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.SelectionMode(() => Mode, m => Mode = m);
        options.Separator();
        options.Combo("Sample Size:", [("Point Sample", 1), ("3 by 3 Average", 3), ("5 by 5 Average", 5), ("11 by 11 Average", 11), ("31 by 31 Average", 31)], () => SampleSize, v => SampleSize = v, 130);
        options.Number("Tolerance:", 0, 255, () => Tolerance, v => Tolerance = (int)v);
        options.Check("Anti-alias", () => AntiAlias, v => AntiAlias = v);
        options.Check("Contiguous", () => Contiguous, v => Contiguous = v);
        options.Check("Sample All Layers", () => SampleAllLayers, v => SampleAllLayers = v);
        options.Separator();
        options.Button("Select Subject", () => UI.Commands.EditorCommands.SelectSubject(Host));
        options.Button("Select and Mask...", () => UI.Commands.EditorCommands.SelectAndMask(Host));
    }
}

/// <summary>Draw a rectangle (or lasso) around an object to select it automatically.</summary>
public sealed class ObjectSelectionTool : SelectionToolBase
{
    private readonly List<SKPoint> _lasso = [];
    private SKPoint _start;
    private SKPoint _current;
    private bool _dragging;
    private SelectionMode _mode;

    public ObjectSelectionTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.ObjectSelection;

    public override string Name => "Object Selection Tool";

    public override string Hint => "Drag a rectangle or lasso around an object to select it.";

    public bool LassoMode { get; set; }

    public bool SampleAllLayers { get; set; } = true;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _mode = ResolveMode(input);
        _start = _current = input.Doc;
        _lasso.Clear();
        _lasso.Add(input.Doc);
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        _current = input.Doc;
        if (LassoMode) _lasso.Add(input.Doc);
        Host.InvalidateOverlay();
    }

    public override async void PointerUp(ToolInput input)
    {
        if (!_dragging || Doc == null) return;
        _dragging = false;
        Host.InvalidateOverlay();
        SKRectI rect;
        SKPath? limit = null;
        if (LassoMode && _lasso.Count > 2)
        {
            limit = new SKPath();
            limit.AddPoly(_lasso.ToArray(), true);
            rect = SKRectI.Round(limit.Bounds);
        }
        else
        {
            rect = SKRectI.Round(DragRect(_start, _current, false, false));
        }
        if (rect.Width < 4 || rect.Height < 4)
        {
            limit?.Dispose();
            return;
        }
        SKBitmap sample = Host.GetSampleImage(SampleAllLayers);
        PixelSurface? mask = null;
        await Host.RunBusyAsync("Selecting object...", _ => Task.Run(() => mask = SmartSelection.SelectObject(sample, rect)));
        sample.Dispose();
        if (mask == null) return;
        if (limit != null)
        {
            using PixelSurface lassoMask = MaskOps.FromPath(limit, Doc.Width, Doc.Height);
            PixelSurface? limited = MaskOps.Combine(mask, lassoMask, SelectionMode.Intersect);
            mask.Dispose();
            mask = limited!;
            limit.Dispose();
        }
        ApplySelection(mask, _mode, "Object Selection");
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging) return;
        if (LassoMode) context.Polygon(_lasso, false);
        else context.Rect(DragRect(_start, _current, false, false));
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.SelectionMode(() => Mode, m => Mode = m);
        options.Separator();
        options.Combo("Mode:", [("Rectangle", false), ("Lasso", true)], () => LassoMode, v => LassoMode = v, 100);
        options.Check("Sample All Layers", () => SampleAllLayers, v => SampleAllLayers = v);
        options.Separator();
        options.Button("Select Subject", () => UI.Commands.EditorCommands.SelectSubject(Host));
        options.Button("Select and Mask...", () => UI.Commands.EditorCommands.SelectAndMask(Host));
    }
}

/// <summary>Paint over an area to grow a selection that follows edges.</summary>
public sealed class QuickSelectionTool : SelectionToolBase
{
    private readonly List<SKPoint> _stroke = [];
    private LiveWire? _edges;
    private SKBitmap? _sample;
    private SelectionMode _mode;
    private PixelSurface? _startSelection;
    private SKPoint? _hover;

    public QuickSelectionTool(IEditorHost host) : base(host)
    {
        Mode = SelectionMode.Add;
    }

    public override ToolId Id => ToolId.QuickSelection;

    public override string Name => "Quick Selection Tool";

    public override string Hint => "Paint over an area to select it; the selection grows to find edges. Alt subtracts.";

    public override ToolCursor Cursor => ToolCursor.Brush;

    public override float BrushOutlineSize => Size;

    public float Size { get; set; } = 30;

    public bool SampleAllLayers { get; set; } = true;

    public bool AutoEnhance { get; set; } = true;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _mode = input.Alt ? SelectionMode.Subtract : Doc.Selection == null ? SelectionMode.Replace : Mode == SelectionMode.Replace ? SelectionMode.Add : Mode;
        _sample = Host.GetSampleImage(SampleAllLayers);
        _edges = new LiveWire(_sample);
        _startSelection = Doc.Selection;
        _stroke.Clear();
        _stroke.Add(input.Doc);
        Grow();
    }

    public override void PointerMove(ToolInput input)
    {
        _hover = input.Doc;
        if (_sample == null)
        {
            Host.InvalidateOverlay();
            return;
        }
        if (SKPoint.Distance(_stroke[^1], input.Doc) < Size / 4) return;
        _stroke.Add(input.Doc);
        Grow();
    }

    private void Grow()
    {
        if (Doc == null || _sample == null || _edges == null) return;
        PixelSurface grown = SmartSelection.QuickSelect(_sample, _stroke, Size / 2, _edges);
        if (AutoEnhance) MaskOps.Blur(grown, 1);
        PixelSurface? result = MaskOps.Combine(_startSelection, grown, _mode);
        Doc.SetSelection(result);
        Host.InvalidateCanvas();
    }

    public override void PointerUp(ToolInput input)
    {
        if (_sample == null) return;
        _sample.Dispose();
        _sample = null;
        _edges = null;
        PushHistory("Quick Selection");
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_hover is SKPoint p) context.Circle(p, Size / 2 * context.Zoom);
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if (key == Key.OemOpenBrackets) { Size = Math.Max(1, Size * 0.9f - 1); Host.RefreshOptions(); return true; }
        if (key == Key.OemCloseBrackets) { Size = Math.Min(5000, Size * 1.1f + 1); Host.RefreshOptions(); return true; }
        return base.KeyDown(key, modifiers);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.SelectionMode(() => Mode, m => Mode = m);
        options.Separator();
        options.Number("Size:", 1, 2500, () => Size, v => Size = (float)v, "0", "px");
        options.Check("Sample All Layers", () => SampleAllLayers, v => SampleAllLayers = v);
        options.Check("Enhance Edge", () => AutoEnhance, v => AutoEnhance = v);
        options.Separator();
        options.Button("Select Subject", () => UI.Commands.EditorCommands.SelectSubject(Host));
        options.Button("Select and Mask...", () => UI.Commands.EditorCommands.SelectAndMask(Host));
    }
}

/// <summary>Paints the selection directly with a brush (add or subtract).</summary>
public sealed class SelectionBrushTool : SelectionToolBase
{
    private BrushStroke? _stroke;
    private SKPoint? _hover;
    private bool _subtract;

    public SelectionBrushTool(IEditorHost host) : base(host)
    {
        Mode = SelectionMode.Add;
    }

    public override ToolId Id => ToolId.SelectionBrush;

    public override string Name => "Selection Brush Tool";

    public override string Hint => "Paint to add to the selection. Alt (or Subtract mode) removes from it.";

    public override ToolCursor Cursor => ToolCursor.Brush;

    public override float BrushOutlineSize => Size;

    public float Size { get; set; } = 40;

    public float Hardness { get; set; } = 1;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _subtract = input.Alt || Mode == SelectionMode.Subtract;
        PixelSurface selection = Doc.Selection?.Clone() ?? PixelSurface.CreateMask(Doc.Width, Doc.Height);
        Doc.SetSelection(null);
        _stroke = new BrushStroke(selection, default, new BrushPreset { Hardness = Hardness, Spacing = 0.1f, PressureOpacity = false }, StrokeMode.Accumulate)
        {
            Size = Size,
            Color = _subtract ? SKColors.Black : SKColors.White
        };
        _stroke.Begin(input.Doc, input.Pressure);
        SetLiveSelection();
    }

    private void SetLiveSelection()
    {
        if (Doc == null || _stroke == null) return;
        // Assign directly so that the selection is not dropped while it is temporarily empty.
        Doc.SetSelectionLive(_stroke.Target);
        Host.InvalidateCanvas(_stroke.Dirty);
    }

    public override void PointerMove(ToolInput input)
    {
        _hover = input.Doc;
        if (_stroke == null)
        {
            Host.InvalidateOverlay();
            return;
        }
        _stroke.LineTo(input.Doc, input.Pressure);
        SetLiveSelection();
    }

    public override void PointerUp(ToolInput input)
    {
        if (_stroke == null || Doc == null) return;
        PixelSurface result = _stroke.Target;
        _stroke = null;
        Doc.SetSelection(result);
        PushHistory("Selection Brush");
        Host.InvalidateCanvas();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_hover is SKPoint p) context.Circle(p, Size / 2 * context.Zoom);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo("Mode:", [("Add", SelectionMode.Add), ("Subtract", SelectionMode.Subtract)], () => Mode, v => Mode = v, 100);
        options.Number("Size:", 1, 2500, () => Size, v => Size = (float)v, "0", "px");
        options.Percent("Hardness:", () => Hardness, v => Hardness = v);
    }
}
