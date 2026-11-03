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
