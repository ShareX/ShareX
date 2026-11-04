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

using Avalonia.Controls;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Commands;

public static partial class EditorCommands
{
    private static void RegisterType()
    {
        R("type.horizontal", "Horizontal", null, w => SetTextOrientation(w, false), w => w.Document?.ActiveLayer is TextLayer, w => w.Document?.ActiveLayer is TextLayer { Vertical: false });
        R("type.vertical", "Vertical", null, w => SetTextOrientation(w, true), w => w.Document?.ActiveLayer is TextLayer, w => w.Document?.ActiveLayer is TextLayer { Vertical: true });
        foreach (TextAntiAlias mode in Enum.GetValues<TextAntiAlias>())
        {
            TextAntiAlias captured = mode;
            R($"type.aa.{mode}", mode.ToString(), null, w => { if (w.Document!.ActiveLayer is TextLayer t) { t.AntiAlias = captured; t.Invalidate(); w.Commit("Anti-Alias"); } },
                w => w.Document?.ActiveLayer is TextLayer, w => (w.Document?.ActiveLayer as TextLayer)?.AntiAlias == captured);
        }
        R("type.sizeUp", "Increase Font Size", "Ctrl+Shift+OemPeriod", w => ChangeFontSize(w, 2), w => w.Document?.ActiveLayer is TextLayer);
        R("type.sizeDown", "Decrease Font Size", "Ctrl+Shift+OemComma", w => ChangeFontSize(w, -2), w => w.Document?.ActiveLayer is TextLayer);
        R("type.rasterize", "Rasterize Type Layer", null, w => Rasterize(w, false), w => w.Document?.ActiveLayer is TextLayer);
        R("type.convertToShape", "Convert to Shape", null, TextToShape, w => w.Document?.ActiveLayer is TextLayer);
        R("type.createWorkPath", "Create Work Path", null, TextToWorkPath, w => w.Document?.ActiveLayer is TextLayer);
    }

    private static void SetTextOrientation(PhotoEditorWindow w, bool vertical)
    {
        if (w.Document!.ActiveLayer is not TextLayer text) return;
        text.Vertical = vertical;
        text.Invalidate();
        w.Commit("Change Text Orientation");
    }

    private static void ChangeFontSize(PhotoEditorWindow w, float delta)
    {
        if (w.Document!.ActiveLayer is not TextLayer text) return;
        text.FontSize = Math.Max(1, text.FontSize + delta);
        text.Invalidate();
        w.Commit("Set Font Size");
    }

    private static void TextToShape(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is not TextLayer text) return;
        using SKPath path = text.GetTextPath();
        ShapeLayer shape = new(text.Name, VectorPath.FromSKPath(path)) { FillColor = text.Color, StrokeColor = null, Opacity = text.Opacity, BlendMode = text.BlendMode, Mask = text.Mask, Style = text.Style };
        ReplaceLayer(doc, text, shape);
        w.Commit("Convert to Shape");
    }

    private static void TextToWorkPath(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is not TextLayer text) return;
        using SKPath path = text.GetTextPath();
        doc.WorkPath = VectorPath.FromSKPath(path);
        doc.ActivePath = new NamedPath("Work Path", doc.WorkPath);
        w.Commit("Create Work Path");
    }

    private static void RegisterSelect()
    {
        R("select.all", "All", "Ctrl+A", w => { w.Document!.SetSelection(MaskOps.FromRect(w.Document.Bounds, w.Document.Width, w.Document.Height)); w.Commit("Select All"); }, HasDoc);
        R("select.deselect", "Deselect", "Ctrl+D", w => { w.Document!.SetSelection(null); w.Commit("Deselect"); }, HasSelection);
        R("select.reselect", "Reselect", "Ctrl+Shift+D", w => { w.Document!.SetSelection(w.Document.LastSelection?.Clone()); w.Commit("Reselect"); }, w => w.Document?.LastSelection != null);
        R("select.inverse", "Inverse", "Ctrl+Shift+I", Inverse, HasDoc);
        R("select.allLayers", "All Layers", "Ctrl+Alt+A", w => { PhotoDocument d = w.Document!; foreach (Layer l in d.AllLayers.Where(l => !l.IsBackground)) d.SelectedLayerIds.Add(l.Id); w.RefreshPanels(); }, HasDoc);
        R("select.colorRange", "Color Range...", null, w => _ = ColorRangeAsync(w), HasDoc);
        R("select.subject", "Subject", null, w => SelectSubject(w), HasDoc);
        R("select.selectAndMask", "Select and Mask...", "Ctrl+Alt+R", w => SelectAndMask(w), HasSelection);
        R("select.modify.border", "Border...", null, w => _ = ModifyAsync(w, "Border", "Width (px):", 10, (m, v) => MaskOps.Border(m, v)), HasSelection);
        R("select.modify.smooth", "Smooth...", null, w => _ = ModifyAsync(w, "Smooth", "Sample Radius (px):", 2, (m, v) => MaskOps.Smooth(m, v)), HasSelection);
        R("select.modify.expand", "Expand...", null, w => _ = ModifyAsync(w, "Expand", "Expand By (px):", 5, (m, v) => MaskOps.Morph(m, v)), HasSelection);
        R("select.modify.contract", "Contract...", null, w => _ = ModifyAsync(w, "Contract", "Contract By (px):", 5, (m, v) => MaskOps.Morph(m, -v)), HasSelection);
        R("select.modify.feather", "Feather...", "Shift+F6", w => _ = ModifyAsync(w, "Feather", "Feather Radius (px):", 5, (m, v) => { PixelSurface c = m.Clone(); MaskOps.Blur(c, v); return c; }), HasSelection);
        R("select.grow", "Grow", null, w => GrowSimilar(w, true), HasSelection);
        R("select.similar", "Similar", null, w => GrowSimilar(w, false), HasSelection);
        R("select.transform", "Transform Selection...", null, w => _ = TransformSelectionAsync(w), HasSelection);
        R("select.load", "Load Selection...", null, LoadChannelSelection, w => w.Document?.Channels.Count > 0);
        R("select.save", "Save Selection...", null, SaveSelectionToChannel, HasSelection);
        R("paths.fromSelection", "Make Work Path", null, w => SelectionToPath(w), HasSelection);
    }

    private static void Inverse(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        PixelSurface mask = doc.Selection?.Clone() ?? PixelSurface.CreateMask(doc.Width, doc.Height);
        MaskOps.Invert(mask);
        doc.SetSelection(mask);
        w.Commit("Inverse");
    }

    public static async void SelectSubject(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        if (w.Document == null) return;
        SKBitmap sample = w.GetSampleImage(true);
        PixelSurface? mask = null;
        await w.RunBusyAsync("Selecting subject...", _ => Task.Run(() => mask = SmartSelection.SelectSubject(sample)));
        sample.Dispose();
        if (mask == null) return;
        w.Document.SetSelection(mask);
        w.Commit("Select Subject");
    }

    public static async void SelectAndMask(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument? doc = w.Document;
        if (doc?.Selection == null)
        {
            w.ShowMessage("Make a selection first.");
            return;
        }
        PixelSurface original = doc.Selection;
        float smooth = 0, feather = 0, contrast = 0, shift = 0;
        void Update()
        {
            PixelSurface result = original.Clone();
            if (shift != 0)
            {
                PixelSurface morphed = MaskOps.Morph(result, shift);
                result.Dispose();
                result = morphed;
            }
            if (smooth > 0)
            {
                PixelSurface smoothed = MaskOps.Smooth(result, smooth);
                result.Dispose();
                result = smoothed;
            }
            if (feather > 0) MaskOps.Blur(result, feather);
            if (contrast > 0) ApplyContrast(result, contrast / 100f);
            doc.SetSelectionLive(result);
            w.InvalidateCanvas();
        }
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Smooth:", DialogBuilder.SliderNumber(0, 100, 0, v => { smooth = (float)v; Update(); }));
        DialogBuilder.AddRow(form, "Feather:", DialogBuilder.SliderNumber(0, 250, 0, v => { feather = (float)v; Update(); }, 0.5, "0.0"));
        DialogBuilder.AddRow(form, "Contrast:", DialogBuilder.SliderNumber(0, 100, 0, v => { contrast = (float)v; Update(); }));
        DialogBuilder.AddRow(form, "Shift Edge:", DialogBuilder.SliderNumber(-100, 100, 0, v => { shift = (float)v / 5; Update(); }));
        bool quickMask = doc.QuickMask;
        doc.QuickMask = true;
        w.InvalidateCanvas();
        bool ok = await DialogBuilder.ShowAsync(w, "Select and Mask", form, 440);
        doc.QuickMask = quickMask;
        if (ok)
        {
            doc.SetSelection(doc.Selection);
            w.Commit("Select and Mask");
        }
        else
        {
            doc.SetSelectionLive(original);
            w.InvalidateCanvas();
        }
    }

    private static unsafe void ApplyContrast(PixelSurface mask, float amount)
    {
        byte* p = (byte*)mask.Bitmap.GetPixels();
        float k = 1 + amount * 8;
        for (int i = 0; i < mask.RowBytes * mask.Height; i++)
        {
            float v = (p[i] / 255f - 0.5f) * k + 0.5f;
            p[i] = (byte)(Math.Clamp(v, 0, 1) * 255);
        }
    }

    private static async Task ModifyAsync(PhotoEditorWindow w, string name, string label, float initial, Func<PixelSurface, float, PixelSurface> modify)
    {
        PhotoDocument doc = w.Document!;
        NumericUpDown amount = DialogBuilder.Number(0.5, 1000, initial, 0.5, "0.#");
        amount.AttachedToVisualTree += (_, _) => amount.Focus();
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, label, amount);
        if (!await DialogBuilder.ShowAsync(w, name + " Selection", form, 380) || doc.Selection == null) return;
        float value = (float)(amount.Value ?? (decimal)initial);
        if (value <= 0) return;
        await w.RunBusyAsync(name + "...", _ => Task.Run(() => doc.SetSelectionLive(modify(doc.Selection, value))));
        doc.SetSelection(doc.Selection);
        w.Commit(name);
    }

    private static void GrowSimilar(PhotoEditorWindow w, bool contiguous)
    {
        PhotoDocument doc = w.Document!;
        if (doc.Selection == null) return;
        int tolerance = w.Tools.Get<MagicWandTool>().Tolerance;
        using SKBitmap sample = w.GetSampleImage(true);
        SKRectI bounds = doc.SelectionBounds;
        PixelSurface result = doc.Selection.Clone();
        // Grow from the average color of the selected area.
        SKColor average = ImageOps.SampleAverage(sample, bounds.MidX, bounds.MidY, Math.Max(1, Math.Min(bounds.Width, bounds.Height) / 2));
        uint seed = ColorMath.FromColor(average);
        if (contiguous)
        {
            List<(int, int)> seeds = [];
            for (int y = bounds.Top; y < bounds.Bottom; y += Math.Max(1, bounds.Height / 16))
                for (int x = bounds.Left; x < bounds.Right; x += Math.Max(1, bounds.Width / 16))
                    if (doc.SelectionAt(x, y) > 0.5f) seeds.Add((x, y));
            foreach (var (x, y) in seeds.Take(64))
            {
                using PixelSurface grown = FloodFill.SelectColor(sample, ImageOps.GetPixel(sample, x, y), tolerance, (x, y), true);
                PixelSurface? combined = MaskOps.Combine(result, grown, SelectionMode.Add);
                if (!ReferenceEquals(combined, result)) result.Dispose();
                result = combined!;
            }
        }
        else
        {
            using PixelSurface similar = FloodFill.SelectColor(sample, seed, tolerance, null, true);
            PixelSurface? combined = MaskOps.Combine(result, similar, SelectionMode.Add);
            if (!ReferenceEquals(combined, result)) result.Dispose();
            result = combined!;
        }
        doc.SetSelection(result);
        w.Commit(contiguous ? "Grow" : "Similar");
    }

    private static async Task TransformSelectionAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        if (doc.Selection == null) return;
        double scaleX = 100, scaleY = 100, angle = 0, dx = 0, dy = 0;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Width (%):", DialogBuilder.SliderNumber(1, 500, 100, v => scaleX = v));
        DialogBuilder.AddRow(form, "Height (%):", DialogBuilder.SliderNumber(1, 500, 100, v => scaleY = v));
        DialogBuilder.AddRow(form, "Angle (°):", DialogBuilder.SliderNumber(-180, 180, 0, v => angle = v));
        DialogBuilder.AddRow(form, "Offset X:", DialogBuilder.SliderNumber(-5000, 5000, 0, v => dx = v));
        DialogBuilder.AddRow(form, "Offset Y:", DialogBuilder.SliderNumber(-5000, 5000, 0, v => dy = v));
        if (!await DialogBuilder.ShowAsync(w, "Transform Selection", form, 460)) return;
        SKRectI b = doc.SelectionBounds;
        SKMatrix m = SKMatrix.CreateScale((float)scaleX / 100, (float)scaleY / 100, b.MidX, b.MidY)
            .PostConcat(SKMatrix.CreateRotationDegrees((float)angle, b.MidX, b.MidY))
            .PostConcat(SKMatrix.CreateTranslation((float)dx, (float)dy));
        doc.SetSelection(MaskOps.Transform(doc.Selection, m, doc.Width, doc.Height));
        w.Commit("Transform Selection");
    }

    private static async Task ColorRangeAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        using SKBitmap sample = w.GetSampleImage(true);
        List<SKColor> colors = [w.Foreground];
        float fuzziness = 40;
        int range = 0;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Select:", DialogBuilder.Combo([("Sampled Colors (foreground)", 0), ("Highlights", 1), ("Midtones", 2), ("Shadows", 3), ("Reds", 4), ("Greens", 5), ("Blues", 6), ("Skin Tones", 7)], range, v => range = v));
        DialogBuilder.AddRow(form, "Fuzziness:", DialogBuilder.SliderNumber(1, 200, fuzziness, v => fuzziness = (float)v));
        DialogBuilder.AddFull(form, new TextBlock { Text = "Tip: sample a color with the Eyedropper first; it is used as the foreground color.", FontSize = 11, TextWrapping = Avalonia.Media.TextWrapping.Wrap });
        if (!await DialogBuilder.ShowAsync(w, "Color Range", form, 420)) return;
        colors = range switch
        {
            1 => [new SKColor(235, 235, 235)],
            2 => [new SKColor(128, 128, 128)],
            3 => [new SKColor(20, 20, 20)],
            4 => [SKColors.Red, new SKColor(200, 30, 30)],
            5 => [SKColors.Lime, new SKColor(30, 160, 30)],
            6 => [SKColors.Blue, new SKColor(30, 60, 200)],
            7 => [new SKColor(224, 172, 140), new SKColor(198, 134, 98), new SKColor(141, 85, 54)],
            _ => colors
        };
        PixelSurface mask = FloodFill.ColorRange(sample, colors, range is 1 or 2 or 3 ? fuzziness * 0.6f : fuzziness);
        doc.SetSelection(mask);
        w.Commit("Color Range");
    }

    public static void LoadChannelSelection(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        AlphaChannel? channel = doc.ActiveChannel ?? doc.Channels.LastOrDefault();
        if (channel == null)
        {
            w.ShowMessage("There are no saved selections (alpha channels).");
            return;
        }
        doc.SetSelection(channel.Mask.Clone());
        w.Commit("Load Selection");
    }

    public static void SaveSelectionToChannel(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        if (doc.Selection == null)
        {
            w.ShowMessage("Make a selection first.");
            return;
        }
        doc.Channels.Add(new AlphaChannel($"Alpha {doc.Channels.Count + 1}", doc.Selection.Clone()));
        w.Commit("Save Selection");
    }

    public static void NewChannel(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        AlphaChannel channel = new($"Alpha {doc.Channels.Count + 1}", PixelSurface.CreateMask(doc.Width, doc.Height));
        doc.Channels.Add(channel);
        doc.ActiveChannel = channel;
        channel.Visible = true;
        w.Commit("New Channel");
    }

    public static void DeleteChannel(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        if (doc.ActiveChannel == null) return;
        doc.Channels.Remove(doc.ActiveChannel);
        doc.ActiveChannel = null;
        w.Commit("Delete Channel");
    }

    public static void SetChannelView(IEditorHost host, ChannelView view)
    {
        PhotoEditorWindow w = W(host);
        if (w.Document == null) return;
        w.Document.ChannelView = view;
        w.InvalidateCanvas();
        w.RefreshPanels();
    }

    // ---------------------------------------------------------------- paths

    private static VectorPath? CurrentPath(PhotoEditorWindow w) =>
        w.Document?.ActivePath?.Path ?? (w.Document?.ActiveLayer as ShapeLayer)?.Path ?? w.Document?.WorkPath;

    public static void PathToSelection(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        VectorPath? path = CurrentPath(w);
        if (path == null || path.IsEmpty || w.Document == null)
        {
            w.ShowMessage("There is no path to load as a selection.");
            return;
        }
        using SKPath sk = path.ToSKPath();
        w.Document.SetSelection(MaskOps.FromPath(sk, w.Document.Width, w.Document.Height));
        w.Commit("Make Selection");
    }

    public static void PathToShape(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument? doc = w.Document;
        VectorPath? path = doc?.ActivePath?.Path ?? doc?.WorkPath;
        if (doc == null || path == null || path.IsEmpty) return;
        ShapeLayer shape = new($"Shape {doc.AllLayers.OfType<ShapeLayer>().Count() + 1}", path.Clone()) { FillColor = w.Foreground };
        doc.ActivePath = null;
        doc.AddLayer(shape);
        w.Commit("New Shape Layer");
    }

    /// <summary>Adds a mask to the active layer from the path (a rasterized vector mask).</summary>
    public static void PathToVectorMask(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument? doc = w.Document;
        VectorPath? path = doc?.ActivePath?.Path ?? doc?.WorkPath;
        if (doc?.ActiveLayer == null || path == null || path.IsEmpty) return;
        using SKPath sk = path.ToSKPath();
        doc.ActiveLayer.Mask = MaskOps.FromPath(sk, doc.Width, doc.Height);
        w.Commit("Add Vector Mask");
    }

    public static void SelectionToPath(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument? doc = w.Document;
        if (doc?.Selection == null) return;
        using SKPath path = MaskOps.ToPath(doc.Selection, 1.5f);
        doc.WorkPath = VectorPath.FromSKPath(path);
        doc.ActivePath = new NamedPath("Work Path", doc.WorkPath);
        w.Commit("Make Work Path");
    }

    public static void FillPath(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        VectorPath? path = CurrentPath(w);
        if (path == null || w.Document == null) return;
        RasterLayer? layer = EditableRaster(w, "fill the path");
        if (layer == null) return;
        using SKPath sk = path.ToSKPath();
        using PixelSurface mask = MaskOps.FromPath(sk, w.Document.Width, w.Document.Height);
        uint color = ColorMath.FromColor(w.Foreground);
        LayerOps.CompositeMasked(w.Document, layer, (_, _) => color, mask, 1, LayerBlendMode.Normal);
        w.Commit("Fill Path");
    }

    public static void StrokePath(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        VectorPath? path = CurrentPath(w);
        if (path == null || w.Document == null) return;
        RasterLayer? layer = EditableRaster(w, "stroke the path");
        if (layer == null) return;
        BrushTool brush = w.Tools.Get<BrushTool>();
        layer.EnsureCovers(w.Document.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        BrushStroke stroke = new(surface, layer.Offset, brush.Tip, StrokeMode.Accumulate)
        {
            Size = brush.Size,
            Opacity = brush.Opacity,
            Flow = brush.Flow,
            Color = w.Foreground,
            LockTransparency = layer.LockTransparency,
            Selection = w.Document.Selection
        };
        // Walk the path outline and stamp the brush along it.
        using SKPath sk = path.ToSKPath();
        using SKPathMeasure measure = new(sk, false);
        do
        {
            float length = measure.Length;
            bool first = true;
            for (float d = 0; d <= length; d += 1)
            {
                if (!measure.GetPosition(d, out SKPoint p)) continue;
                if (first) { stroke.Begin(p, 1); first = false; }
                else stroke.LineTo(p, 1);
            }
        }
        while (measure.NextContour());
        w.Commit("Stroke Path");
    }

    public static void NewPath(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        NamedPath path = new($"Path {doc.Paths.Count + 1}", new VectorPath());
        doc.Paths.Add(path);
        doc.ActivePath = path;
        w.Commit("New Path");
    }

    public static void DeletePath(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument? doc = w.Document;
        if (doc?.ActivePath == null) return;
        if (doc.ActivePath.Path == doc.WorkPath) doc.WorkPath = null;
        else doc.Paths.Remove(doc.ActivePath);
        doc.ActivePath = null;
        w.Commit("Delete Path");
    }
}
