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
using Avalonia.Input;
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

public enum TransformMode
{
    Free,
    Scale,
    Rotate,
    Skew,
    Distort,
    Perspective
}

/// <summary>
/// Free Transform session (Ctrl+T). Works on the active layer or a floating copy of the selected pixels.
/// Smart objects keep their source pixels and only store the new matrix.
/// </summary>
public sealed class TransformTool : ToolBase
{
    private Layer? _original;
    private Layer? _current;
    private RasterLayer? _floatingBase;
    private SKRect _source;
    private SKPoint[] _quad = new SKPoint[4];
    private SKPoint[] _dragQuad = new SKPoint[4];
    private int _handle = -1;
    private SKPoint _dragStart;
    private bool _active;
    private ToolBase? _returnTo;
    private string _historyName = "Free Transform";

    public TransformTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.FreeTransform;

    public override string Name => "Free Transform";

    public override string Hint => "Drag handles to scale (Shift: proportional, Alt: from center), outside to rotate, Ctrl-drag a corner to distort, Ctrl-drag an edge to skew. Enter commits, Esc cancels.";

    public override ToolCursor Cursor => ToolCursor.Arrow;

    public override bool HasPendingOperation => _active;

    public TransformMode ForcedMode { get; set; }

    public ResampleMethod Interpolation { get; set; } = ResampleMethod.Bicubic;

    public bool IsActive => _active;

    /// <summary>True when the layer being transformed was removed or replaced (for example by undo or a layer command).</summary>
    private bool IsStale => Doc == null || _current == null || !Doc.AllLayers.Contains(_current) || _floatingBase != null && !Doc.AllLayers.Contains(_floatingBase);

    /// <summary>Drops the pending transform without changing the document, because the document changed underneath it.</summary>
    public void Abandon()
    {
        if (!_active) return;
        End();
        Host.RefreshPanels();
        Host.InvalidateCanvas();
    }

    /// <summary>Starts a transform of the active layer (or selection). Returns false if nothing can be transformed.</summary>
    public bool Begin(TransformMode mode, string historyName = "Free Transform")
    {
        PhotoDocument? doc = Doc;
        if (doc?.ActiveLayer is not Layer layer) return false;
        if (!layer.CanMove || layer is AdjustmentLayer or FillLayer)
        {
            Host.ShowMessage("Could not transform because the layer is locked or has no content.");
            return false;
        }
        ForcedMode = mode;
        _historyName = historyName;
        _floatingBase = null;

        if (layer is RasterLayer raster && doc.Selection != null)
        {
            if (!raster.CanEditPixels)
            {
                Host.ShowMessage("Could not transform the selection because the layer is locked.");
                return false;
            }
            // Lift the selected pixels onto a temporary floating layer.
            raster.EnsureCovers(doc.Bounds);
            PixelSurface surface = raster.GetWritableSurface();
            SKRectI bounds = doc.SelectionBounds;
            SKBitmap floating = ImageOps.ExtractMasked(surface.Bitmap, raster.Offset, doc.Selection, bounds);
            if (raster.LockTransparency)
            {
                uint background = ColorMath.FromColor(Host.Background);
                LayerOps.CompositeMasked(doc, raster, (_, _) => background, null, 1, LayerBlendMode.Normal);
            }
            else
            {
                LayerOps.EraseMasked(doc, raster, doc.Selection, 1);
            }
            RasterLayer floatLayer = new("Floating Selection", new PixelSurface(floating), new SKPointI(bounds.Left, bounds.Top))
            {
                BlendMode = raster.BlendMode
            };
            doc.AddLayer(floatLayer, raster);
            _floatingBase = raster;
            layer = floatLayer;
        }

        _source = layer is RasterLayer r ? (SKRect)r.GetOpaqueBounds() : layer.GetContentBounds();
        if (layer is SmartObjectLayer smart) _source = SKRect.Create(smart.Source.Width, smart.Source.Height);
        if (_source.IsEmpty || _source.Width < 1 || _source.Height < 1)
        {
            Host.ShowMessage("Could not transform because the layer is empty.");
            Cancel();
            return false;
        }
        _original = layer.CloneShared();
        _current = layer;
        _quad = layer is SmartObjectLayer smartObject ? smartObject.GetCorners() : Corners(_source);
        if (layer is SmartObjectLayer)
        {
            // Smart objects are transformed relative to their source pixels.
            ((SmartObjectLayer)_original).Transform = SKMatrix.Identity;
        }
        _active = true;
        _returnTo = Host.Tools.Active == this ? _returnTo : Host.Tools.Active;
        Host.Tools.Select(this);
        Host.RefreshOptions();
        Host.InvalidateOverlay();
        return true;
    }

    private static SKPoint[] Corners(SKRect r) => [new(r.Left, r.Top), new(r.Right, r.Top), new(r.Right, r.Bottom), new(r.Left, r.Bottom)];

    private SKMatrix CurrentMatrix => ImageOps.RectToQuad(_source, _quad);

    private void UpdatePreview(bool final = false)
    {
        PhotoDocument? doc = Doc;
        if (doc == null || _original == null || _current == null) return;
        Layer updated = _original.CloneShared();
        DocumentTransforms.TransformLayerInPlace(updated, CurrentMatrix, doc.Width, doc.Height, final ? Interpolation : ResampleMethod.Bilinear, _original is RasterLayer ? SKRectI.Round(_source) : null);
        GroupLayer parent = _current.Parent ?? doc.Root;
        int index = parent.Children.IndexOf(_current);
        if (index < 0) return;
        parent.Remove(_current);
        parent.Add(updated, index);
        doc.ActiveLayer = updated;
        _current = updated;
        Host.InvalidateCanvas();
    }

    /// <summary>Applies a matrix immediately (used by rotate 90, flip and other Edit &gt; Transform commands).</summary>
    public void ApplyImmediate(Func<SKRect, SKMatrix> matrixForBounds, string historyName)
    {
        if (!Begin(TransformMode.Free, historyName)) return;
        SKMatrix m = matrixForBounds(_source);
        _quad = m.MapPoints(Corners(_source));
        Commit();
    }

    public override void Commit()
    {
        if (!_active || Doc == null) return;
        if (IsStale)
        {
            Abandon();
            return;
        }
        PhotoDocument doc = Doc;
        UpdatePreview(final: true);
        if (_floatingBase != null && _current is RasterLayer floating)
        {
            // Merge the floating pixels back into the layer they came from.
            _floatingBase.EnsureCovers(doc.Bounds);
            PixelSurface surface = _floatingBase.GetWritableSurface();
            using (SKCanvas canvas = new(surface.Bitmap))
            {
                canvas.DrawBitmap(floating.Surface.Bitmap, floating.Offset.X - _floatingBase.Offset.X, floating.Offset.Y - _floatingBase.Offset.Y);
            }
            doc.RemoveLayer(floating);
            doc.ActiveLayer = _floatingBase;
            if (doc.Selection != null) doc.SetSelectionLive(MaskOps.Transform(doc.Selection, CurrentMatrix, doc.Width, doc.Height));
        }
        End();
        PushHistory(_historyName);
        Host.InvalidateCanvas();
    }

    public override void Cancel()
    {
        if (Doc?.History.Current != null) Doc.RestoreState(Doc.History.Current.State);
        End();
        Host.RefreshPanels();
        Host.InvalidateCanvas();
    }

    private void End()
    {
        _active = false;
        _original = null;
        _current = null;
        _floatingBase = null;
        ToolBase? back = _returnTo;
        _returnTo = null;
        if (back != null && back != this) Host.Tools.Select(back);
        Host.RefreshOptions();
    }

    public override void Deactivate()
    {
        if (_active) Commit();
    }

    private int HitHandle(Point screen)
    {
        if (Host.View == null) return -1;
        Point[] corners = _quad.Select(p => Host.View.DocToScreen(p)).ToArray();
        for (int i = 0; i < 4; i++) if (Distance(screen, corners[i]) < 8) return i;
        for (int i = 0; i < 4; i++)
        {
            Point mid = (corners[i] + corners[(i + 1) % 4]) / 2;
            if (Distance(screen, mid) < 8) return 4 + i;
        }
        using SKPath path = new();
        path.AddPoly(_quad, true);
        SKPoint doc = Host.View.ScreenToDoc(screen);
        if (path.Contains(doc.X, doc.Y)) return 8;
        return 9;
    }

    private static double Distance(Point a, Point b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

    public override void PointerDown(ToolInput input)
    {
        if (!_active || !input.LeftButton) return;
        if (IsStale)
        {
            Abandon();
            return;
        }
        _handle = HitHandle(input.Screen);
        _dragStart = input.Doc;
        _dragQuad = (SKPoint[])_quad.Clone();
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_active || _handle < 0) return;
        if (IsStale)
        {
            Abandon();
            return;
        }
        SKPoint delta = input.Doc - _dragStart;
        TransformMode mode = ForcedMode;
        if (mode == TransformMode.Free)
        {
            if (input.Ctrl && input.Alt && input.Shift) mode = TransformMode.Perspective;
            else if (input.Ctrl && _handle >= 4 && _handle < 8) mode = TransformMode.Skew;
            else if (input.Ctrl && _handle < 4) mode = TransformMode.Distort;
        }
        SKPoint[] q = (SKPoint[])_dragQuad.Clone();
        SKPoint center = Center(_dragQuad);

        if (_handle == 8)
        {
            if (input.Shift) delta = MathF.Abs(delta.X) > MathF.Abs(delta.Y) ? new SKPoint(delta.X, 0) : new SKPoint(0, delta.Y);
            for (int i = 0; i < 4; i++) q[i] = _dragQuad[i] + delta;
        }
        else if (_handle == 9 || mode == TransformMode.Rotate)
        {
            float a0 = MathF.Atan2(_dragStart.Y - center.Y, _dragStart.X - center.X);
            float a1 = MathF.Atan2(input.Doc.Y - center.Y, input.Doc.X - center.X);
            float angle = a1 - a0;
            if (input.Shift) angle = MathF.Round(angle / (MathF.PI / 12)) * (MathF.PI / 12);
            SKMatrix rotation = SKMatrix.CreateRotation(angle, center.X, center.Y);
            q = rotation.MapPoints(_dragQuad);
        }
        else if (mode == TransformMode.Distort && _handle < 4)
        {
            q[_handle] = _dragQuad[_handle] + delta;
        }
        else if (mode == TransformMode.Perspective && _handle < 4)
        {
            // Move the corner and mirror the movement on the adjacent corner of the same horizontal edge.
            int partner = _handle switch { 0 => 1, 1 => 0, 2 => 3, _ => 2 };
            q[_handle] = _dragQuad[_handle] + delta;
            q[partner] = _dragQuad[partner] + new SKPoint(-delta.X, delta.Y);
        }
        else if (mode == TransformMode.Skew && _handle >= 4)
        {
            int a = _handle - 4, b = (a + 1) % 4;
            SKPoint edge = _dragQuad[b] - _dragQuad[a];
            float length = Math.Max(1e-3f, edge.Length);
            SKPoint dir = new(edge.X / length, edge.Y / length);
            float along = delta.X * dir.X + delta.Y * dir.Y;
            SKPoint shift = new(dir.X * along, dir.Y * along);
            q[a] = _dragQuad[a] + shift;
            q[b] = _dragQuad[b] + shift;
        }
        else
        {
            q = Scale(_dragQuad, _handle, input.Doc, input.Shift, input.Alt);
        }
        _quad = q;
        UpdatePreview();
        Host.RefreshOptions();
    }

    private static SKPoint Center(SKPoint[] q) => new((q[0].X + q[1].X + q[2].X + q[3].X) / 4, (q[0].Y + q[1].Y + q[2].Y + q[3].Y) / 4);

    /// <summary>Scales the quad by dragging a corner or edge handle, using the quad's own axes.</summary>
    private static SKPoint[] Scale(SKPoint[] q, int handle, SKPoint pointer, bool proportional, bool fromCenter)
    {
        SKPoint u = q[1] - q[0], v = q[3] - q[0];
        // Handle position and anchor in (u, v) parameter space; corners are (0,0),(1,0),(1,1),(0,1).
        (float hu, float hv) = handle switch
        {
            0 => (0f, 0f), 1 => (1f, 0f), 2 => (1f, 1f), 3 => (0f, 1f),
            4 => (0.5f, 0f), 5 => (1f, 0.5f), 6 => (0.5f, 1f), _ => (0f, 0.5f)
        };
        float au = fromCenter ? 0.5f : 1 - hu, av = fromCenter ? 0.5f : 1 - hv;
        if (handle == 4 || handle == 6) au = 0.5f;
        if (handle == 5 || handle == 7) av = 0.5f;
        SKPoint anchor = q[0] + new SKPoint(u.X * au + v.X * av, u.Y * au + v.Y * av);
        SKPoint handlePos = q[0] + new SKPoint(u.X * hu + v.X * hv, u.Y * hu + v.Y * hv);

        // Solve pointer - anchor = a*u + b*v.
        float det = u.X * v.Y - u.Y * v.X;
        if (MathF.Abs(det) < 1e-6f) return q;
        SKPoint d = pointer - anchor;
        float a = (d.X * v.Y - d.Y * v.X) / det;
        float b = (u.X * d.Y - u.Y * d.X) / det;
        float su = MathF.Abs(hu - au) < 1e-6f ? 1 : a / (hu - au);
        float sv = MathF.Abs(hv - av) < 1e-6f ? 1 : b / (hv - av);
        if (handle >= 4)
        {
            if (handle == 4 || handle == 6) su = proportional ? sv : 1;
            else sv = proportional ? su : 1;
        }
        else if (proportional)
        {
            float s = MathF.Abs(su) > MathF.Abs(sv) ? su : sv;
            su = MathF.Sign(su) * MathF.Abs(s);
            sv = MathF.Sign(sv) * MathF.Abs(s);
        }
        SKPoint[] result = new SKPoint[4];
        float[] cu = [0, 1, 1, 0], cv = [0, 0, 1, 1];
        for (int i = 0; i < 4; i++)
        {
            float nu = au + (cu[i] - au) * su;
            float nv = av + (cv[i] - av) * sv;
            result[i] = q[0] + new SKPoint(u.X * nu + v.X * nv, u.Y * nu + v.Y * nv);
        }
        _ = handlePos;
        return result;
    }

    public override void PointerUp(ToolInput input) => _handle = -1;

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if (!_active) return false;
        if (IsStale)
        {
            Abandon();
            return false;
        }
        float step = modifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;
        SKPoint delta = key switch
        {
            Key.Left => new SKPoint(-step, 0),
            Key.Right => new SKPoint(step, 0),
            Key.Up => new SKPoint(0, -step),
            Key.Down => new SKPoint(0, step),
            _ => default
        };
        if (delta != default)
        {
            for (int i = 0; i < 4; i++) _quad[i] += delta;
            UpdatePreview();
            return true;
        }
        return base.KeyDown(key, modifiers);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_active) return;
        context.Polygon(_quad, true, OverlayContext.AccentPen);
        for (int i = 0; i < 4; i++)
        {
            context.Handle(_quad[i]);
            context.Handle(new SKPoint((_quad[i].X + _quad[(i + 1) % 4].X) / 2, (_quad[i].Y + _quad[(i + 1) % 4].Y) / 2));
        }
        context.Circle(Center(_quad), 4);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        if (!_active)
        {
            options.Label("Use Edit > Free Transform (Ctrl+T) to transform the active layer or selection.");
            return;
        }
        SKPoint center = Center(_quad);
        float width = SKPoint.Distance(_quad[0], _quad[1]);
        float height = SKPoint.Distance(_quad[0], _quad[3]);
        float angle = MathF.Atan2(_quad[1].Y - _quad[0].Y, _quad[1].X - _quad[0].X) * 180 / MathF.PI;
        void Rebuild(float cx, float cy, float w, float h, float a)
        {
            SKMatrix m = SKMatrix.CreateScale(w / 2, h / 2).PostConcat(SKMatrix.CreateRotationDegrees(a)).PostConcat(SKMatrix.CreateTranslation(cx, cy));
            _quad = m.MapPoints([new(-1, -1), new(1, -1), new(1, 1), new(-1, 1)]);
            UpdatePreview();
        }
        options.Number("X:", -100000, 100000, () => center.X, v => Rebuild((float)v, center.Y, width, height, angle), "0.0", "px", 1, 80);
        options.Number("Y:", -100000, 100000, () => center.Y, v => Rebuild(center.X, (float)v, width, height, angle), "0.0", "px", 1, 80);
        options.Number("W:", -10000, 10000, () => width / _source.Width * 100, v => Rebuild(center.X, center.Y, (float)v / 100 * _source.Width, height, angle), "0.00", "%", 1, 72);
        options.Number("H:", -10000, 10000, () => height / _source.Height * 100, v => Rebuild(center.X, center.Y, width, (float)v / 100 * _source.Height, angle), "0.00", "%", 1, 72);
        options.Number("Angle:", -180, 180, () => angle, v => Rebuild(center.X, center.Y, width, height, (float)v), "0.00", "°", 1, 72);
        options.EnumCombo("Interpolation:", () => Interpolation, v => Interpolation = v, 140);
        options.Separator();
        options.Button("Cancel", Cancel, "Cancel transform (Esc)");
        options.Button("Commit", Commit, "Commit transform (Enter)");
    }
}

public sealed class MoveTool : ToolBase
{
    private readonly List<Layer> _layers = [];
    private SKPoint _start;
    private SKPoint _applied;
    private bool _moving;
    private int _guideIndex = -1;
    private RasterLayer? _floating;
    private RasterLayer? _floatingSource;
    private PixelSurface? _selectionAtStart;

    public MoveTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Move;

    public override string Name => "Move Tool";

    public override string Hint => "Drag to move the layer or selected pixels. Alt-drag duplicates, Shift constrains. Arrow keys nudge.";

    public override ToolCursor Cursor => ToolCursor.Move;

    public bool AutoSelect { get; set; }

    public bool AutoSelectGroups { get; set; }

    public bool ShowTransformControls { get; set; }

    public override void PointerDown(ToolInput input)
    {
        PhotoDocument? doc = Doc;
        if (doc == null || !input.LeftButton) return;

        _guideIndex = HitGuide(input.Doc);
        if (_guideIndex >= 0 && !Host.Settings.LockGuides)
        {
            _moving = true;
            return;
        }

        if (ShowTransformControls && doc.ActiveLayer != null)
        {
            TransformTool transform = Host.Tools.Get<TransformTool>();
            SKRect bounds = doc.ActiveLayer.GetContentBounds();
            if (!bounds.IsEmpty && Host.View != null)
            {
                bool nearEdge = new[] { new SKPoint(bounds.Left, bounds.Top), new SKPoint(bounds.Right, bounds.Top), new SKPoint(bounds.Right, bounds.Bottom), new SKPoint(bounds.Left, bounds.Bottom) }
                    .Any(c => SKPoint.Distance(c, input.Doc) * Host.View.Zoom < 8);
                if (nearEdge && transform.Begin(TransformMode.Free))
                {
                    transform.PointerDown(input);
                    return;
                }
            }
        }

        if (AutoSelect || input.Ctrl)
        {
            Layer? picked = PickLayer(input.DocPixel);
            if (picked != null)
            {
                if (AutoSelectGroups) while (picked.Parent != null && picked.Parent != doc.Root) picked = picked.Parent;
                doc.ActiveLayer = picked;
                doc.SelectedLayerIds.Clear();
                doc.SelectedLayerIds.Add(picked.Id);
                Host.RefreshPanels();
            }
        }

        Layer? layer = doc.ActiveLayer;
        if (layer == null) return;
        if (!layer.CanMove)
        {
            Host.ShowMessage("Could not complete the request because the layer is locked.");
            return;
        }

        _layers.Clear();
        if (layer is RasterLayer raster && doc.Selection != null)
        {
            if (!raster.CanEditPixels) return;
            BeginFloating(raster, input.Alt);
            _layers.Add(_floating!);
        }
        else
        {
            // Every selected layer (and the layers linked to them) moves together.
            List<Layer> targets = doc.GetMoveTargets().Where(l => l.CanMove).ToList();
            if (targets.Count == 0) targets.Add(layer);
            if (input.Alt)
            {
                List<Layer> copies = [];
                Layer? activeCopy = null;
                foreach (Layer original in targets)
                {
                    Layer copy = original.Duplicate();
                    copy.Name = original.Name + " copy";
                    doc.AddLayer(copy, original);
                    copies.Add(copy);
                    if (original == layer) activeCopy = copy;
                }
                targets = copies;
                doc.SelectedLayerIds.Clear();
                foreach (Layer copy in copies) doc.SelectedLayerIds.Add(copy.Id);
                doc.ActiveLayer = activeCopy ?? copies[^1];
            }
            _layers.AddRange(targets);
        }
        _start = input.Doc;
        _applied = default;
        _moving = true;
    }

    private void BeginFloating(RasterLayer raster, bool duplicate)
    {
        PhotoDocument doc = Doc!;
        raster.EnsureCovers(doc.Bounds);
        PixelSurface surface = raster.GetWritableSurface();
        SKRectI bounds = doc.SelectionBounds;
        SKBitmap pixels = ImageOps.ExtractMasked(surface.Bitmap, raster.Offset, doc.Selection!, bounds);
        if (!duplicate)
        {
            if (raster.LockTransparency)
            {
                uint background = ColorMath.FromColor(Host.Background);
                LayerOps.CompositeMasked(doc, raster, (_, _) => background, null, 1, LayerBlendMode.Normal);
            }
            else
            {
                LayerOps.EraseMasked(doc, raster, doc.Selection, 1);
            }
        }
        _floating = new RasterLayer("Floating", new PixelSurface(pixels), new SKPointI(bounds.Left, bounds.Top));
        _floatingSource = raster;
        _selectionAtStart = doc.Selection;
        doc.AddLayer(_floating, raster);
    }

    private Layer? PickLayer(SKPointI p)
    {
        PhotoDocument doc = Doc!;
        foreach (var (layer, _) in doc.GetLayerListTopDown())
        {
            if (!layer.IsEffectivelyVisible || layer is GroupLayer or AdjustmentLayer) continue;
            switch (layer)
            {
                case RasterLayer raster when (ImageOps.GetPixel(raster.Surface.Bitmap, p.X - raster.Offset.X, p.Y - raster.Offset.Y) >> 24) > 10:
                    return layer;
                case ShapeLayer shape:
                {
                    using SKPath path = shape.Path.ToSKPath();
                    if (path.Contains(p.X, p.Y)) return layer;
                    break;
                }
                case TextLayer or SmartObjectLayer or FrameLayer or FillLayer when layer.GetContentBounds().Contains(p.X, p.Y):
                    return layer;
            }
        }
        return null;
    }

    private int HitGuide(SKPoint p)
    {
        if (Doc == null || Host.View == null || !Host.Settings.ShowGuides) return -1;
        float tolerance = 4 / (float)Host.View.Zoom;
        for (int i = Doc.Guides.Count - 1; i >= 0; i--)
        {
            Guide g = Doc.Guides[i];
            if (MathF.Abs((g.Horizontal ? p.Y : p.X) - g.Position) <= tolerance) return i;
        }
        return -1;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_moving || Doc == null) return;
        if (_guideIndex >= 0)
        {
            Guide g = Doc.Guides[_guideIndex];
            Doc.Guides[_guideIndex] = g with { Position = MathF.Round(g.Horizontal ? input.Doc.Y : input.Doc.X) };
            Host.InvalidateOverlay();
            return;
        }
        if (_layers.Count == 0) return;
        SKPoint delta = input.Doc - _start;
        if (input.Shift) delta = MathF.Abs(delta.X) > MathF.Abs(delta.Y) ? new SKPoint(delta.X, 0) : new SKPoint(0, delta.Y);
        delta = new SKPoint(MathF.Round(delta.X), MathF.Round(delta.Y));
        SKPoint step = delta - _applied;
        if (step.X == 0 && step.Y == 0) return;
        SKRect before = _layers.Select(l => l.GetContentBounds()).Aggregate(SKRect.Union);
        foreach (Layer moved in _layers) moved.Translate(step.X, step.Y);
        _applied = delta;
        if (_floating != null && _selectionAtStart != null)
        {
            Doc.SetSelectionLive(MaskOps.Transform(_selectionAtStart, SKMatrix.CreateTranslation(delta.X, delta.Y), Doc.Width, Doc.Height, false));
        }
        SKRect after = _layers.Select(l => l.GetContentBounds()).Aggregate(SKRect.Union);
        SKRect union = SKRect.Union(before, after);
        union.Inflate(2, 2);
        Host.InvalidateCanvas(_layers.Any(l => l is GroupLayer or AdjustmentLayer || l.Style != null) ? null : SKRectI.Round(union));
        Host.SetStatus($"Δx: {delta.X:0}  Δy: {delta.Y:0}");
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_moving || Doc == null) return;
        _moving = false;
        if (_guideIndex >= 0)
        {
            Guide g = Doc.Guides[_guideIndex];
            bool outside = g.Position < 0 || g.Position > (g.Horizontal ? Doc.Height : Doc.Width);
            if (outside) Doc.Guides.RemoveAt(_guideIndex);
            _guideIndex = -1;
            PushHistory(outside ? "Delete Guide" : "Move Guide");
            Host.InvalidateOverlay();
            return;
        }
        if (_floating != null && _floatingSource != null)
        {
            // Drop the floating pixels back into their layer.
            _floatingSource.EnsureCovers(Doc.Bounds);
            PixelSurface surface = _floatingSource.GetWritableSurface();
            using (SKCanvas canvas = new(surface.Bitmap))
            {
                canvas.DrawBitmap(_floating.Surface.Bitmap, _floating.Offset.X - _floatingSource.Offset.X, _floating.Offset.Y - _floatingSource.Offset.Y);
            }
            Doc.RemoveLayer(_floating);
            Doc.ActiveLayer = _floatingSource;
            _floating = null;
            _floatingSource = null;
            _selectionAtStart = null;
            Doc.SetSelection(Doc.Selection);
            Host.InvalidateCanvas();
        }
        _layers.Clear();
        if (_applied != default || input.Alt) PushHistory("Move");
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if (Doc?.ActiveLayer is not Layer layer || !layer.CanMove) return base.KeyDown(key, modifiers);
        float step = modifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;
        SKPoint delta = key switch
        {
            Key.Left => new SKPoint(-step, 0),
            Key.Right => new SKPoint(step, 0),
            Key.Up => new SKPoint(0, -step),
            Key.Down => new SKPoint(0, step),
            _ => default
        };
        if (delta == default) return base.KeyDown(key, modifiers);
        if (layer is RasterLayer raster && Doc.Selection != null && raster.CanEditPixels)
        {
            BeginFloating(raster, false);
            _floating!.Translate(delta.X, delta.Y);
            _layers.Clear();
            _layers.Add(_floating);
            _selectionAtStart = Doc.Selection;
            Doc.SetSelectionLive(MaskOps.Transform(Doc.Selection!, SKMatrix.CreateTranslation(delta.X, delta.Y), Doc.Width, Doc.Height, false));
            _moving = true;
            _applied = delta;
            PointerUp(new ToolInput());
        }
        else
        {
            List<Layer> targets = Doc.GetMoveTargets().Where(l => l.CanMove).ToList();
            if (targets.Count == 0) targets.Add(layer);
            foreach (Layer target in targets) target.Translate(delta.X, delta.Y);
            PushHistory("Nudge");
        }
        Host.InvalidateCanvas();
        return true;
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!ShowTransformControls || Doc?.ActiveLayer == null) return;
        SKRect bounds = Doc.ActiveLayer is RasterLayer r ? (SKRect)r.GetOpaqueBounds() : Doc.ActiveLayer.GetContentBounds();
        if (bounds.IsEmpty) return;
        context.Rect(bounds, OverlayContext.AccentPen);
        foreach (SKPoint c in new[] { new SKPoint(bounds.Left, bounds.Top), new SKPoint(bounds.Right, bounds.Top), new SKPoint(bounds.Right, bounds.Bottom), new SKPoint(bounds.Left, bounds.Bottom) })
            context.Handle(c);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Check("Auto-Select:", () => AutoSelect, v => AutoSelect = v, "Click selects the topmost layer under the pointer (Ctrl temporarily)");
        options.Combo(null, [("Layer", false), ("Group", true)], () => AutoSelectGroups, v => AutoSelectGroups = v, 80);
        options.Check("Show Transform Controls", () => ShowTransformControls, v => { ShowTransformControls = v; Host.InvalidateOverlay(); });
        options.Separator();
        foreach (var (icon, tip, align) in new (string, string, LayerAlignment)[]
        {
            ("align-left", "Align left edges", LayerAlignment.Left),
            ("align-hcenter", "Align horizontal centers", LayerAlignment.HorizontalCenter),
            ("align-right", "Align right edges", LayerAlignment.Right),
            ("align-top", "Align top edges", LayerAlignment.Top),
            ("align-vcenter", "Align vertical centers", LayerAlignment.VerticalCenter),
            ("align-bottom", "Align bottom edges", LayerAlignment.Bottom)
        })
        {
            LayerAlignment captured = align;
            options.IconButton(icon, tip, () => UI.Commands.EditorCommands.AlignLayers(Host, captured));
        }
    }
}

public enum LayerAlignment
{
    Left,
    HorizontalCenter,
    Right,
    Top,
    VerticalCenter,
    Bottom
}
