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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using ShareX.PhotoEditor.UI;
using ShareX.PhotoEditor.UI.Theme;

namespace ShareX.PhotoEditor.Tools;

public enum ToolId
{
    Move, Artboard,
    RectangularMarquee, EllipticalMarquee, SingleRowMarquee, SingleColumnMarquee,
    Lasso, PolygonalLasso, MagneticLasso,
    ObjectSelection, QuickSelection, MagicWand, SelectionBrush,
    Crop, PerspectiveCrop, Slice, SliceSelect,
    Frame,
    Eyedropper, ColorSampler, Ruler, Note, Count,
    SpotHealingBrush, HealingBrush, Patch, ContentAwareMove, RedEye, Remove,
    Brush, Pencil, ColorReplacement, MixerBrush, AdjustmentBrush,
    CloneStamp, PatternStamp,
    HistoryBrush, ArtHistoryBrush,
    Eraser, BackgroundEraser, MagicEraser,
    Gradient, PaintBucket,
    Blur, Sharpen, Smudge,
    Dodge, Burn, Sponge,
    Pen, FreeformPen, CurvaturePen, AddAnchorPoint, DeleteAnchorPoint, ConvertPoint,
    HorizontalType, VerticalType, HorizontalTypeMask, VerticalTypeMask,
    PathSelection, DirectSelection,
    Rectangle, Ellipse, Triangle, Polygon, Line, CustomShape,
    Hand, RotateView, Zoom,
    FreeTransform
}

public enum ToolCursor
{
    Arrow,
    Crosshair,
    Precise,
    Move,
    Hand,
    Grab,
    ZoomIn,
    ZoomOut,
    Text,
    Brush,
    Eyedropper,
    Pen,
    Bucket,
    NotAllowed,
    SizeAll,
    Rotate
}

/// <summary>Pointer state passed to tools, in document and screen coordinates.</summary>
public sealed class ToolInput
{
    public SKPoint Doc { get; init; }
    public Point Screen { get; init; }
    public float Pressure { get; init; } = 1;
    public bool LeftButton { get; init; }
    public bool RightButton { get; init; }
    public bool IsPressed { get; init; }
    public KeyModifiers Modifiers { get; init; }
    public int ClickCount { get; init; } = 1;

    public bool Shift => Modifiers.HasFlag(KeyModifiers.Shift);
    public bool Ctrl => Modifiers.HasFlag(KeyModifiers.Control);
    public bool Alt => Modifiers.HasFlag(KeyModifiers.Alt);
    public SKPointI DocPixel => new((int)MathF.Floor(Doc.X), (int)MathF.Floor(Doc.Y));
}

/// <summary>Services the editor exposes to tools.</summary>
public interface IEditorHost
{
    PhotoDocument? Document { get; }
    DocumentView? View { get; }
    EditorSettings Settings { get; }
    SKColor Foreground { get; set; }
    SKColor Background { get; set; }
    GradientDefinition Gradient { get; set; }
    PatternDefinition? Pattern { get; set; }
    List<PatternDefinition> Patterns { get; }
    List<BrushPreset> BrushPresets { get; }
    ToolManager Tools { get; }
    Window Window { get; }

    /// <summary>Re-composites part of the document and repaints the canvas.</summary>
    void InvalidateCanvas(SKRectI? area = null);

    /// <summary>Repaints tool overlays without re-compositing.</summary>
    void InvalidateOverlay();

    void RefreshPanels();

    void RefreshOptions();

    void SetStatus(string? text);

    /// <summary>Returns a flattened copy of either the active layer or all visible layers at document size.</summary>
    SKBitmap GetSampleImage(bool allLayers);

    void BeginTextEditing(TextLayer layer, bool isNew);

    void EndTextEditing(bool commit);

    bool IsTextEditing { get; }

    Task RunBusyAsync(string message, Func<IProgress<float>?, Task> work);

    void ShowMessage(string text);

    Task<string?> PromptTextAsync(string title, string label, string initial);
}

/// <summary>Base class for every toolbar tool.</summary>
public abstract class ToolBase
{
    protected ToolBase(IEditorHost host)
    {
        Host = host;
    }

    protected IEditorHost Host { get; }

    protected PhotoDocument? Doc => Host.Document;

    public abstract ToolId Id { get; }

    public abstract string Name { get; }

    /// <summary>Short description shown in tooltips and the status bar.</summary>
    public virtual string Hint => string.Empty;

    public virtual ToolCursor Cursor => ToolCursor.Crosshair;

    /// <summary>Diameter (document pixels) of the brush outline cursor, or 0 for none.</summary>
    public virtual float BrushOutlineSize => 0;

    /// <summary>Whether the tool is in the middle of an interaction that Enter/Escape can finish.</summary>
    public virtual bool HasPendingOperation => false;

    public virtual void Activate()
    {
    }

    public virtual void Deactivate()
    {
        if (HasPendingOperation) Commit();
    }

    public virtual void PointerDown(ToolInput input)
    {
    }

    public virtual void PointerMove(ToolInput input)
    {
    }

    public virtual void PointerUp(ToolInput input)
    {
    }

    /// <summary>Handles a key; returns true if consumed.</summary>
    public virtual bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if (key == Key.Enter && HasPendingOperation)
        {
            Commit();
            return true;
        }
        if (key == Key.Escape && HasPendingOperation)
        {
            Cancel();
            return true;
        }
        return false;
    }

    public virtual bool KeyUp(Key key, KeyModifiers modifiers) => false;

    public virtual void Commit()
    {
    }

    public virtual void Cancel()
    {
    }

    public virtual void DrawOverlay(OverlayContext context)
    {
    }

    /// <summary>Builds the options bar contents for this tool.</summary>
    public virtual void BuildOptions(OptionsBuilder options)
    {
    }

    /// <summary>Records a history state and refreshes panels.</summary>
    protected void PushHistory(string name)
    {
        if (Doc == null) return;
        Doc.History.Push(name);
        Host.RefreshPanels();
    }

    protected bool RequireDocument() => Doc != null;

    protected bool RequireEditableLayer(out RasterLayer layer)
    {
        layer = null!;
        if (Doc?.ActiveLayer is RasterLayer raster && raster.CanEditPixels && raster.IsEffectivelyVisible)
        {
            layer = raster;
            return true;
        }
        if (Doc?.ActiveLayer is RasterLayer locked && !locked.CanEditPixels)
        {
            Host.ShowMessage("Could not use the " + Name + " because the layer is locked.");
        }
        else if (Doc?.ActiveLayer is RasterLayer hidden && !hidden.IsEffectivelyVisible)
        {
            Host.ShowMessage("Could not use the " + Name + " because the target layer is hidden.");
        }
        else if (Doc?.ActiveLayer != null)
        {
            Host.ShowMessage("Could not use the " + Name + " because the target layer is not a pixel layer. Rasterize the layer first (Layer > Rasterize).");
        }
        return false;
    }

    /// <summary>Constrains a drag vector to 45 degree steps (Shift).</summary>
    protected static SKPoint Constrain45(SKPoint start, SKPoint end)
    {
        SKPoint d = end - start;
        float angle = MathF.Atan2(d.Y, d.X);
        float snapped = MathF.Round(angle / (MathF.PI / 4)) * (MathF.PI / 4);
        float length = d.Length;
        return start + new SKPoint(MathF.Cos(snapped) * length, MathF.Sin(snapped) * length);
    }

    /// <summary>Builds a rectangle from a drag with Shift (square) and Alt (from center) modifiers.</summary>
    protected static SKRect DragRect(SKPoint start, SKPoint end, bool square, bool fromCenter)
    {
        float dx = end.X - start.X, dy = end.Y - start.Y;
        if (square)
        {
            float size = Math.Max(MathF.Abs(dx), MathF.Abs(dy));
            dx = MathF.Sign(dx == 0 ? 1 : dx) * size;
            dy = MathF.Sign(dy == 0 ? 1 : dy) * size;
        }
        if (fromCenter) return new SKRect(start.X - MathF.Abs(dx), start.Y - MathF.Abs(dy), start.X + MathF.Abs(dx), start.Y + MathF.Abs(dy));
        return new SKRect(Math.Min(start.X, start.X + dx), Math.Min(start.Y, start.Y + dy), Math.Max(start.X, start.X + dx), Math.Max(start.Y, start.Y + dy));
    }
}

/// <summary>Drawing helpers for tool overlays; converts document coordinates to screen coordinates.</summary>
public sealed class OverlayContext
{
    public static readonly IPen DarkPen = new Pen(new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)), 1);
    public static readonly IPen LightPen = new Pen(Brushes.White, 1, new DashStyle([4, 4], 0));
    public static readonly IBrush AccentBrush = PhotoEditorTheme.AccentBrush;
    public static readonly IPen AccentPen = new Pen(AccentBrush, 1);

    public OverlayContext(DrawingContext drawing, DocumentView view)
    {
        Drawing = drawing;
        View = view;
    }

    public DrawingContext Drawing { get; }

    public DocumentView View { get; }

    public double Zoom => View.Zoom;

    public Point ToScreen(SKPoint p) => View.DocToScreen(p);

    /// <summary>Converts a screen distance to document pixels.</summary>
    public float DocLength(double screenPixels) => (float)(screenPixels / View.Zoom);

    public void Line(SKPoint a, SKPoint b, IPen? pen = null)
    {
        Point pa = ToScreen(a), pb = ToScreen(b);
        Drawing.DrawLine(DarkPen, pa + new Vector(0.5, 0.5), pb + new Vector(0.5, 0.5));
        Drawing.DrawLine(pen ?? LightPen, pa, pb);
    }

    public void Polygon(IReadOnlyList<SKPoint> points, bool closed, IPen? pen = null)
    {
        if (points.Count < 2) return;
        for (int i = 0; i < points.Count - 1; i++) Line(points[i], points[i + 1], pen);
        if (closed) Line(points[^1], points[0], pen);
    }

    public void Rect(SKRect rect, IPen? pen = null) =>
        Polygon([new(rect.Left, rect.Top), new(rect.Right, rect.Top), new(rect.Right, rect.Bottom), new(rect.Left, rect.Bottom)], true, pen);

    public void Ellipse(SKRect rect, IPen? pen = null)
    {
        List<SKPoint> points = [];
        for (int i = 0; i < 72; i++)
        {
            float a = i / 72f * MathF.PI * 2;
            points.Add(new SKPoint(rect.MidX + MathF.Cos(a) * rect.Width / 2, rect.MidY + MathF.Sin(a) * rect.Height / 2));
        }
        Polygon(points, true, pen);
    }

    public void Handle(SKPoint p, bool filled = false, double size = 7)
    {
        Point s = ToScreen(p);
        Rect r = new(s.X - size / 2, s.Y - size / 2, size, size);
        Drawing.DrawRectangle(filled ? AccentBrush : Brushes.White, DarkPen, r);
    }

    public void Circle(SKPoint center, double screenRadius, IPen? pen = null, IBrush? fill = null)
    {
        Point s = ToScreen(center);
        Drawing.DrawEllipse(fill, DarkPen, s, screenRadius + 1, screenRadius + 1);
        Drawing.DrawEllipse(null, pen ?? new Pen(Brushes.White, 1), s, screenRadius, screenRadius);
    }

    public void Path(SKPath path, IPen? pen = null, IBrush? fill = null)
    {
        Geometry geometry = ToGeometry(path);
        if (fill != null) Drawing.DrawGeometry(fill, null, geometry);
        Drawing.DrawGeometry(null, new Pen(new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)), 2), geometry);
        Drawing.DrawGeometry(null, pen ?? AccentPen, geometry);
    }

    public Geometry ToGeometry(SKPath path)
    {
        StreamGeometry geometry = new();
        using StreamGeometryContext context = geometry.Open();
        using SKPath.RawIterator iterator = path.CreateRawIterator();
        SKPoint[] pts = new SKPoint[4];
        SKPathVerb verb;
        bool open = false;
        while ((verb = iterator.Next(pts)) != SKPathVerb.Done)
        {
            switch (verb)
            {
                case SKPathVerb.Move:
                    if (open) context.EndFigure(false);
                    context.BeginFigure(ToScreen(pts[0]), true);
                    open = true;
                    break;
                case SKPathVerb.Line:
                    context.LineTo(ToScreen(pts[1]));
                    break;
                case SKPathVerb.Quad:
                    context.QuadraticBezierTo(ToScreen(pts[1]), ToScreen(pts[2]));
                    break;
                case SKPathVerb.Conic:
                    context.QuadraticBezierTo(ToScreen(pts[1]), ToScreen(pts[2]));
                    break;
                case SKPathVerb.Cubic:
                    context.CubicBezierTo(ToScreen(pts[1]), ToScreen(pts[2]), ToScreen(pts[3]));
                    break;
                case SKPathVerb.Close:
                    if (open) context.EndFigure(true);
                    open = false;
                    break;
            }
        }
        if (open) context.EndFigure(false);
        return geometry;
    }

    public void Text(SKPoint docPoint, string text, double offsetX = 8, double offsetY = 8, IBrush? background = null)
    {
        Point s = ToScreen(docPoint);
        FormattedText formatted = new(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 11, Brushes.White);
        Rect box = new(s.X + offsetX - 3, s.Y + offsetY - 2, formatted.Width + 6, formatted.Height + 4);
        Drawing.DrawRectangle(background ?? new SolidColorBrush(Color.FromArgb(200, 30, 30, 30)), null, box, 2, 2);
        Drawing.DrawText(formatted, new Point(s.X + offsetX, s.Y + offsetY));
    }

    /// <summary>Draws a selection-like outline with the editor's animated dash pattern.</summary>
    public void Ants(IReadOnlyList<SKPoint> points, bool closed)
    {
        Polygon(points, closed, new Pen(Brushes.White, 1, new DashStyle([4, 4], View.AntsPhase)));
    }

    /// <summary>Draws the path's anchors and handles for pen tools.</summary>
    public void PathAnchors(VectorPath path, PathAnchor? selected = null, bool showHandles = true)
    {
        foreach (SubPath sub in path.SubPaths)
        {
            foreach (PathAnchor anchor in sub.Anchors)
            {
                bool isSelected = anchor == selected;
                if (showHandles && (isSelected || anchor.Smooth))
                {
                    if (anchor.HasIn)
                    {
                        Line(anchor.Point, anchor.In, AccentPen);
                        Circle(anchor.In, 3, AccentPen, AccentBrush);
                    }
                    if (anchor.HasOut)
                    {
                        Line(anchor.Point, anchor.Out, AccentPen);
                        Circle(anchor.Out, 3, AccentPen, AccentBrush);
                    }
                }
                Handle(anchor.Point, isSelected, 6);
            }
        }
    }
}
