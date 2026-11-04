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

public sealed class GradientTool : ToolBase
{
    private SKPoint _start;
    private SKPoint _end;
    private bool _dragging;

    public GradientTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Gradient;

    public override string Name => "Gradient Tool";

    public override string Hint => "Drag to draw a gradient. Shift constrains the angle to 45 degrees.";

    public GradientType Type { get; set; } = GradientType.Linear;

    public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.Normal;

    public float Opacity { get; set; } = 1;

    public bool Reverse { get; set; }

    public bool Dither { get; set; } = true;

    public bool Transparency { get; set; } = true;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _start = _end = input.Doc;
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        _end = input.Shift ? Constrain45(_start, input.Doc) : input.Doc;
        SKPoint d = _end - _start;
        Host.SetStatus($"Angle: {-MathF.Atan2(d.Y, d.X) * 180 / MathF.PI:0.0}°  Distance: {d.Length:0.0} px");
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_dragging || Doc == null) return;
        _dragging = false;
        Host.InvalidateOverlay();
        if (SKPoint.Distance(_start, _end) < 1) return;
        GradientDefinition gradient = Host.Gradient.Clone();
        gradient.Type = Type;
        gradient.Reverse = Reverse;
        if (!Transparency) gradient.Stops = gradient.Stops.Select(s => s with { Color = s.Color.WithAlpha(255) }).ToList();
        Apply(gradient, _start, _end);
    }

    private void Apply(GradientDefinition gradient, SKPoint start, SKPoint end)
    {
        PhotoDocument doc = Doc!;
        if (doc.QuickMask || doc.EditingMask && doc.ActiveLayer?.Mask != null)
        {
            // Gradients on masks write gray levels.
            PixelSurface target = doc.QuickMask ? doc.Selection?.Clone() ?? PixelSurface.CreateMask(doc.Width, doc.Height, 255) : doc.ActiveLayer!.GetWritableMask(doc.Width, doc.Height);
            using SKBitmap rendered = LayerOps.RenderGradient(gradient, start, end, doc.Width, doc.Height, Dither);
            unsafe
            {
                byte* m = (byte*)target.Bitmap.GetPixels();
                for (int y = 0; y < doc.Height; y++)
                {
                    for (int x = 0; x < doc.Width; x++)
                    {
                        ColorMath.Unpack(ImageOps.GetPixel(rendered, x, y), out float r, out float g, out float b, out float a);
                        float v = ColorMath.Luminance(r, g, b) * a * 255;
                        float w = Opacity * (doc.QuickMask ? 1 : doc.SelectionAt(x, y));
                        m[y * target.RowBytes + x] = (byte)(m[y * target.RowBytes + x] * (1 - w) + v * w);
                    }
                }
            }
            if (doc.QuickMask) doc.SetSelectionLive(target);
            PushHistory("Gradient");
            Host.InvalidateCanvas();
            return;
        }
        if (!RequireEditableLayer(out RasterLayer layer)) return;
        using SKBitmap gradientBitmap = LayerOps.RenderGradient(gradient, start, end, doc.Width, doc.Height, Dither);
        SKRectI changed = LayerOps.CompositeMasked(doc, layer, (x, y) => ImageOps.GetPixel(gradientBitmap, x, y), null, Opacity, BlendMode);
        PushHistory("Gradient");
        Host.InvalidateCanvas(changed);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging) return;
        context.Line(_start, _end);
        context.Handle(_start, false, 6);
        context.Handle(_end, true, 6);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo("Gradient:", GradientDefinition.CreatePresets(Host.Foreground, Host.Background).Select(g => (g.Name, g.Name)),
            () => Host.Gradient.Name, name =>
            {
                GradientDefinition? preset = GradientDefinition.CreatePresets(Host.Foreground, Host.Background).FirstOrDefault(g => g.Name == name);
                if (preset != null) Host.Gradient = preset;
            }, 170);
        options.Button("Edit...", async () =>
        {
            GradientDefinition? edited = await UI.Dialogs.GradientEditorDialog.ShowAsync(Host.Window, Host.Gradient, Host);
            if (edited != null) Host.Gradient = edited;
        });
        options.Separator();
        foreach (GradientType type in Enum.GetValues<GradientType>())
        {
            GradientType captured = type;
            options.IconButton("gradient-" + type.ToString().ToLowerInvariant(), $"{type} Gradient", () => { Type = captured; Host.RefreshOptions(); }, Type == type);
        }
        options.Separator();
        options.BlendMode(() => BlendMode, m => BlendMode = m);
        options.Percent("Opacity:", () => Opacity, v => Opacity = v);
        options.Check("Reverse", () => Reverse, v => Reverse = v);
        options.Check("Dither", () => Dither, v => Dither = v);
        options.Check("Transparency", () => Transparency, v => Transparency = v);
    }
}

public sealed class PaintBucketTool : ToolBase
{
    public PaintBucketTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.PaintBucket;

    public override string Name => "Paint Bucket Tool";

    public override string Hint => "Click to fill similarly colored pixels with the foreground color or a pattern.";

    public override ToolCursor Cursor => ToolCursor.Bucket;

    public bool UsePattern { get; set; }

    public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.Normal;

    public float Opacity { get; set; } = 1;

    public int Tolerance { get; set; } = 32;

    public bool AntiAlias { get; set; } = true;

    public bool Contiguous { get; set; } = true;

    public bool AllLayers { get; set; }

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        if (!RequireEditableLayer(out RasterLayer layer)) return;
        SKPointI p = input.DocPixel;
        if (p.X < 0 || p.Y < 0 || p.X >= Doc.Width || p.Y >= Doc.Height) return;
        using SKBitmap sample = Host.GetSampleImage(AllLayers);
        using PixelSurface mask = FloodFill.Select(sample, p.X, p.Y, Tolerance, Contiguous, AntiAlias);
        Func<int, int, uint> source;
        if (UsePattern)
        {
            PatternDefinition pattern = Host.Pattern ?? Host.Patterns[0];
            source = pattern.Sample;
        }
        else
        {
            uint color = ColorMath.FromColor(Host.Foreground);
            source = (_, _) => color;
        }
        SKRectI changed = LayerOps.CompositeMasked(Doc, layer, source, mask, Opacity, BlendMode);
        PushHistory("Paint Bucket");
        Host.InvalidateCanvas(changed);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo("Fill:", [("Foreground", false), ("Pattern", true)], () => UsePattern, v => { UsePattern = v; Host.RefreshOptions(); }, 110);
        if (UsePattern) options.Combo(null, Host.Patterns.Select(p => (p.Name, p)), () => Host.Pattern ?? Host.Patterns[0], p => Host.Pattern = p, 120);
        options.BlendMode(() => BlendMode, m => BlendMode = m);
        options.Percent("Opacity:", () => Opacity, v => Opacity = v);
        options.Number("Tolerance:", 0, 255, () => Tolerance, v => Tolerance = (int)v);
        options.Check("Anti-alias", () => AntiAlias, v => AntiAlias = v);
        options.Check("Contiguous", () => Contiguous, v => Contiguous = v);
        options.Check("All Layers", () => AllLayers, v => AllLayers = v);
    }
}

public sealed class MagicEraserTool : ToolBase
{
    public MagicEraserTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.MagicEraser;

    public override string Name => "Magic Eraser Tool";

    public override string Hint => "Click to erase similarly colored pixels to transparency.";

    public int Tolerance { get; set; } = 32;

    public bool AntiAlias { get; set; } = true;

    public bool Contiguous { get; set; } = true;

    public bool AllLayers { get; set; }

    public float Opacity { get; set; } = 1;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        if (!RequireEditableLayer(out RasterLayer layer)) return;
        SKPointI p = input.DocPixel;
        if (p.X < 0 || p.Y < 0 || p.X >= Doc.Width || p.Y >= Doc.Height) return;
        LayerOps.UnlockBackground(layer);
        using SKBitmap sample = Host.GetSampleImage(AllLayers);
        using PixelSurface mask = FloodFill.Select(sample, p.X, p.Y, Tolerance, Contiguous, AntiAlias);
        SKRectI changed = LayerOps.EraseMasked(Doc, layer, mask, Opacity);
        PushHistory("Magic Eraser");
        Host.InvalidateCanvas(changed);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Number("Tolerance:", 0, 255, () => Tolerance, v => Tolerance = (int)v);
        options.Check("Anti-alias", () => AntiAlias, v => AntiAlias = v);
        options.Check("Contiguous", () => Contiguous, v => Contiguous = v);
        options.Check("Sample All Layers", () => AllLayers, v => AllLayers = v);
        options.Percent("Opacity:", () => Opacity, v => Opacity = v);
    }
}
