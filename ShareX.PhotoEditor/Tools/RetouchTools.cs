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

/// <summary>Shared logic for tools that drag a selection to a new location (patch and content-aware move).</summary>
public abstract class SelectionDragToolBase : ToolBase
{
    private readonly List<SKPoint> _lasso = [];
    private SKPoint _dragStart;
    private SKPoint _offset;
    private bool _lassoing;
    private bool _dragging;
    private List<(SKPoint A, SKPoint B)>? _outline;

    protected SelectionDragToolBase(IEditorHost host) : base(host)
    {
    }

    public override string Hint => "Draw a selection (or use any selection tool), then drag the selection.";

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        if (Doc.Selection != null && Doc.SelectionAt(input.DocPixel.X, input.DocPixel.Y) > 0.5f && !input.Shift && !input.Alt)
        {
            _dragging = true;
            _dragStart = input.Doc;
            _offset = default;
            _outline = MaskOps.TraceOutline(Doc.Selection, 60_000);
            return;
        }
        _lassoing = true;
        _lasso.Clear();
        _lasso.Add(input.Doc);
    }

    public override void PointerMove(ToolInput input)
    {
        if (_lassoing)
        {
            _lasso.Add(input.Doc);
            Host.InvalidateOverlay();
        }
        else if (_dragging)
        {
            _offset = input.Doc - _dragStart;
            _offset = new SKPoint(MathF.Round(_offset.X), MathF.Round(_offset.Y));
            Host.InvalidateOverlay();
        }
    }

    public override void PointerUp(ToolInput input)
    {
        if (Doc == null) return;
        if (_lassoing)
        {
            _lassoing = false;
            if (_lasso.Count > 2)
            {
                using SKPath path = new();
                path.AddPoly(_lasso.ToArray(), true);
                PixelSurface mask = MaskOps.FromPath(path, Doc.Width, Doc.Height);
                SelectionMode mode = input.Shift ? SelectionMode.Add : input.Alt ? SelectionMode.Subtract : SelectionMode.Replace;
                Doc.SetSelection(MaskOps.Combine(Doc.Selection, mask, mode));
                PushHistory("Lasso");
            }
            Host.InvalidateCanvas();
            return;
        }
        if (_dragging)
        {
            _dragging = false;
            _outline = null;
            Host.InvalidateOverlay();
            if (_offset.X == 0 && _offset.Y == 0) return;
            if (!RequireEditableLayer(out RasterLayer layer)) return;
            Perform(layer, (int)_offset.X, (int)_offset.Y);
        }
    }

    protected abstract void Perform(RasterLayer layer, int dx, int dy);

    /// <summary>Moves a document-space mask into the layer's pixel space.</summary>
    protected static PixelSurface ToLayerSpace(PixelSurface mask, RasterLayer layer, int dx = 0, int dy = 0) =>
        MaskOps.Reframe(mask, layer.Surface.Width, layer.Surface.Height, -layer.Offset.X + dx, -layer.Offset.Y + dy, 0);

    /// <summary>Returns a copy of the layer pixels shifted by an offset.</summary>
    protected static SKBitmap Shifted(SKBitmap source, int dx, int dy)
    {
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        result.Erase(SKColors.Transparent);
        using SKCanvas canvas = new(result);
        canvas.DrawBitmap(source, dx, dy);
        return result;
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_lassoing) context.Polygon(_lasso, false);
        if (_dragging && _outline != null)
        {
            foreach (var (a, b) in _outline.Take(20_000)) context.Line(a + _offset, b + _offset);
        }
    }
}

public sealed class PatchTool : SelectionDragToolBase
{
    public PatchTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Patch;

    public override string Name => "Patch Tool";

    public bool DestinationMode { get; set; }

    public bool Transparent { get; set; }

    protected override void Perform(RasterLayer layer, int dx, int dy)
    {
        PhotoDocument doc = Doc!;
        layer.EnsureCovers(doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        SKBitmap bitmap = surface.Bitmap;
        if (!DestinationMode)
        {
            // Source mode: replace the selected area with texture from where the selection was dragged to.
            using SKBitmap source = Shifted(bitmap, -dx, -dy);
            using PixelSurface mask = ToLayerSpace(doc.Selection!, layer);
            if (Transparent) BlendTransparent(bitmap, source, mask);
            else PoissonBlend.Blend(bitmap, source, mask);
        }
        else
        {
            // Destination mode: copy the selected area to the new location.
            using SKBitmap source = Shifted(bitmap, dx, dy);
            using PixelSurface mask = ToLayerSpace(doc.Selection!, layer, dx, dy);
            if (Transparent) BlendTransparent(bitmap, source, mask);
            else PoissonBlend.Blend(bitmap, source, mask);
            doc.SetSelection(MaskOps.Transform(doc.Selection!, SKMatrix.CreateTranslation(dx, dy), doc.Width, doc.Height, false));
        }
        PushHistory("Patch");
        Host.InvalidateCanvas();
    }

    private static unsafe void BlendTransparent(SKBitmap target, SKBitmap source, PixelSurface mask)
    {
        uint* t = (uint*)target.GetPixels(), s = (uint*)source.GetPixels();
        int stride = target.RowBytes / 4;
        for (int y = 0; y < target.Height; y++)
            for (int x = 0; x < target.Width; x++)
            {
                byte m = MaskOps.Get(mask, x, y);
                if (m > 0) t[y * stride + x] = ColorMath.LerpPremul(t[y * stride + x], s[y * stride + x], m / 255f * 0.5f);
            }
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo("Patch:", [("Source", false), ("Destination", true)], () => DestinationMode, v => DestinationMode = v, 110);
        options.Check("Transparent", () => Transparent, v => Transparent = v);
    }
}

public sealed class ContentAwareMoveTool : SelectionDragToolBase
{
    public ContentAwareMoveTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.ContentAwareMove;

    public override string Name => "Content-Aware Move Tool";

    public bool Extend { get; set; }

    public int Structure { get; set; } = 4;

    protected override async void Perform(RasterLayer layer, int dx, int dy)
    {
        PhotoDocument doc = Doc!;
        layer.EnsureCovers(doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        SKBitmap bitmap = surface.Bitmap;
        using SKBitmap moved = Shifted(bitmap, dx, dy);
        PixelSurface originalMask = ToLayerSpace(doc.Selection!, layer);
        PixelSurface movedMask = ToLayerSpace(doc.Selection!, layer, dx, dy);
        int patch = Math.Clamp(Structure - 1, 2, 6);
        await Host.RunBusyAsync("Content-Aware Move...", progress => Task.Run(() =>
        {
            if (!Extend)
            {
                // Fill the area the content came from, excluding the destination as a texture source.
                using PixelSurface hole = MaskOps.Morph(originalMask, 2);
                PixelSurface dest = movedMask;
                Inpainting.Fill(bitmap, hole, patch, (x, y) => MaskOps.Get(dest, x, y) == 0, progress);
            }
            PoissonBlend.Blend(bitmap, moved, movedMask);
        }));
        originalMask.Dispose();
        movedMask.Dispose();
        doc.SetSelection(MaskOps.Transform(doc.Selection!, SKMatrix.CreateTranslation(dx, dy), doc.Width, doc.Height, false));
        PushHistory("Content-Aware Move");
        Host.InvalidateCanvas();
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo("Mode:", [("Move", false), ("Extend", true)], () => Extend, v => Extend = v, 100);
        options.Number("Structure:", 1, 7, () => Structure, v => Structure = (int)v);
    }
}

public sealed class RedEyeTool : ToolBase
{
    private SKPoint _start;
    private SKPoint _end;
    private bool _dragging;

    public RedEyeTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.RedEye;

    public override string Name => "Red Eye Tool";

    public override string Hint => "Click on a red eye (or drag around it) to remove the red reflection.";

    public float PupilSize { get; set; } = 50;

    public float Darken { get; set; } = 50;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _start = _end = input.Doc;
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        _end = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_dragging || Doc == null) return;
        _dragging = false;
        Host.InvalidateOverlay();
        if (!RequireEditableLayer(out RasterLayer layer)) return;
        SKRect rect = DragRect(_start, _end, false, false);
        if (rect.Width < 4 || rect.Height < 4)
        {
            // Find the red blob around the click.
            float radius = Math.Max(12, Math.Min(Doc.Width, Doc.Height) * 0.04f);
            rect = FindRedRegion(layer, new SKPointI((int)_start.X, (int)_start.Y), (int)radius) ??SKRect.Create(_start.X - radius, _start.Y - radius, radius * 2, radius * 2);
        }
        layer.EnsureCovers(Doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        SKRectI area = SKRectI.Round(rect);
        area.Inflate(2, 2);
        SmartSelection.RemoveRedEye(surface.Bitmap, layer.Offset, area, PupilSize, Darken);
        PushHistory("Red Eye");
        Host.InvalidateCanvas(area);
    }

    private static SKRect? FindRedRegion(RasterLayer layer, SKPointI center, int radius)
    {
        SKBitmap bitmap = layer.Surface.Bitmap;
        float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
        bool found = false;
        for (int y = center.Y - radius; y <= center.Y + radius; y++)
        {
            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                uint p = ImageOps.GetPixel(bitmap, x - layer.Offset.X, y - layer.Offset.Y);
                ColorMath.Unpack(p, out float r, out float g, out float b, out _);
                if (r > 0.3f && r - (g + b) / 2 > 0.15f)
                {
                    found = true;
                    minX = Math.Min(minX, x); minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y);
                }
            }
        }
        return found ? new SKRect(minX, minY, maxX + 1, maxY + 1) : null;
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_dragging && SKPoint.Distance(_start, _end) > 2) context.Rect(DragRect(_start, _end, false, false));
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Number("Pupil Size:", 1, 100, () => PupilSize, v => PupilSize = (float)v, "0", "%");
        options.Number("Darken Amount:", 1, 100, () => Darken, v => Darken = (float)v, "0", "%");
    }
}
