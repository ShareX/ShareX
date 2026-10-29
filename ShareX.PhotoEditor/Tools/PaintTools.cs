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
using Avalonia.Threading;
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

/// <summary>The surface a painting tool writes to: layer pixels, a layer mask, the quick mask or an alpha channel.</summary>
public sealed class PaintTarget
{
    public required PixelSurface Surface { get; init; }
    public SKPointI Offset { get; init; }
    public bool IsMask => Surface.IsMask;
    public RasterLayer? Layer { get; init; }
    public bool LockTransparency { get; init; }
    public required string Description { get; init; }
    public Action? Apply { get; init; }
}

/// <summary>Common behavior of brush-based tools: target resolution, smoothing, airbrush, straight lines and history.</summary>
public abstract class BrushToolBase : ToolBase
{
    private DispatcherTimer? _airbrushTimer;
    private SKPoint _smoothed;
    private SKPoint _lastInput;
    private SKPoint? _lastStrokeEnd;
    private float _lastPressure = 1;

    protected BrushToolBase(IEditorHost host) : base(host)
    {
    }

    protected BrushStroke? Stroke { get; private set; }

    protected PaintTarget? Target { get; private set; }

    protected SKPoint? Hover { get; private set; }

    public BrushPreset Tip { get; set; } = new() { Name = "Soft Round", Hardness = 0, Spacing = 0.1f };

    public float Size { get; set; } = 30;

    public float Opacity { get; set; } = 1;

    public float Flow { get; set; } = 1;

    public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.Normal;

    /// <summary>Lazy-mouse smoothing 0..1.</summary>
    public float Smoothing { get; set; } = 0.1f;

    public bool Airbrush { get; set; }

    public override ToolCursor Cursor => ToolCursor.Brush;

    public override float BrushOutlineSize => Size;

    protected virtual StrokeMode StrokeMode => StrokeMode.Accumulate;

    protected virtual string HistoryName => Name.Replace(" Tool", string.Empty);

    /// <summary>Whether the tool can paint on masks (quick mask, layer masks, channels).</summary>
    protected virtual bool SupportsMaskTargets => true;

    protected virtual bool ShowFlow => true;

    protected abstract void ConfigureStroke(BrushStroke stroke, ToolInput input);

    protected virtual bool CanStart(ToolInput input) => true;

    protected virtual void OnStrokeFinished(BrushStroke stroke, PaintTarget target)
    {
        target.Apply?.Invoke();
        PushHistory(HistoryName);
    }

    /// <summary>Resolves where painting goes, creating masks when needed.</summary>
    protected PaintTarget? ResolveTarget()
    {
        PhotoDocument? doc = Doc;
        if (doc == null) return null;
        if (SupportsMaskTargets && doc.QuickMask)
        {
            PixelSurface mask = doc.Selection?.Clone() ?? PixelSurface.CreateMask(doc.Width, doc.Height, 255);
            return new PaintTarget
            {
                Surface = mask,
                Description = "Quick Mask",
                Apply = () => doc.SetSelectionLive(mask)
            };
        }
        if (SupportsMaskTargets && doc.ActiveChannel is AlphaChannel channel)
        {
            if (channel.Mask.Frozen) channel.Mask = channel.Mask.Clone();
            return new PaintTarget { Surface = channel.Mask, Description = channel.Name };
        }
        if (doc.EditingMask && doc.ActiveLayer is Layer masked && masked.Mask != null)
        {
            if (!SupportsMaskTargets)
            {
                Host.ShowMessage($"The {Name} cannot paint on a layer mask.");
                return null;
            }
            return new PaintTarget { Surface = masked.GetWritableMask(doc.Width, doc.Height), Description = "Layer Mask" };
        }
        if (!RequireEditableLayer(out RasterLayer layer)) return null;
        layer.EnsureCovers(doc.Bounds);
        return new PaintTarget
        {
            Surface = layer.GetWritableSurface(),
            Offset = layer.Offset,
            Layer = layer,
            LockTransparency = layer.LockTransparency
        , Description = layer.Name };
    }

    public override void PointerDown(ToolInput input)
    {
        Hover = input.Doc;
        if (!input.LeftButton || Doc == null || !CanStart(input)) return;
        PaintTarget? target = ResolveTarget();
        if (target == null) return;
        Target = target;
        BrushStroke stroke = new(target.Surface, target.Offset, Tip, StrokeMode)
        {
            Size = Size,
            Opacity = Opacity,
            Flow = ShowFlow ? Flow : 1,
            BlendMode = BlendMode,
            LockTransparency = target.LockTransparency,
            Selection = Doc.QuickMask && target.Description == "Quick Mask" ? null : Doc.Selection
        };
        ConfigureStroke(stroke, input);
        Stroke = stroke;
        _smoothed = input.Doc;
        _lastInput = input.Doc;
        _lastPressure = input.Pressure;
        if (input.Shift && _lastStrokeEnd is SKPoint last)
        {
            stroke.Begin(last, input.Pressure);
            stroke.LineTo(input.Doc, input.Pressure);
        }
        else
        {
            stroke.Begin(input.Doc, input.Pressure);
        }
        target.Apply?.Invoke();
        Host.InvalidateCanvas(stroke.Dirty);
        if (Airbrush)
        {
            _airbrushTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
            _airbrushTimer.Tick += (_, _) =>
            {
                if (Stroke == null) return;
                Stroke.Dab(_smoothed, _lastPressure);
                Host.InvalidateCanvas(DabRect(_smoothed));
            };
            _airbrushTimer.Start();
        }
    }

    private SKRectI DabRect(SKPoint p)
    {
        int r = (int)MathF.Ceiling(Size / 2 + Tip.Scatter * Size + 2);
        return new SKRectI((int)p.X - r, (int)p.Y - r, (int)p.X + r, (int)p.Y + r);
    }

    public override void PointerMove(ToolInput input)
    {
        Hover = input.Doc;
        if (Stroke == null)
        {
            Host.InvalidateOverlay();
            return;
        }
        _lastInput = input.Doc;
        _lastPressure = input.Pressure;
        SKPoint target = input.Doc;
        if (Smoothing > 0)
        {
            // Pulled-string smoothing: the brush follows the pointer only after it moves beyond a radius.
            float radius = Smoothing * Math.Max(10, Size) * 0.8f;
            SKPoint d = target - _smoothed;
            float distance = d.Length;
            if (distance <= radius) return;
            float move = distance - radius;
            target = _smoothed + new SKPoint(d.X / distance * move, d.Y / distance * move);
        }
        SKRectI before = Stroke.Dirty;
        Stroke.LineTo(target, input.Pressure);
        _smoothed = target;
        Target?.Apply?.Invoke();
        SKRectI changed = DabRect(target);
        changed.Union(DabRect(_smoothed));
        Host.InvalidateCanvas(Stroke.Dirty == before ? changed : SKRectI.Union(changed, DabRect(target)));
        OnStrokeMoved(Stroke);
    }

    protected virtual void OnStrokeMoved(BrushStroke stroke)
    {
    }

    public override void PointerUp(ToolInput input)
    {
        _airbrushTimer?.Stop();
        _airbrushTimer = null;
        if (Stroke == null || Target == null) return;
        if (Smoothing > 0) Stroke.LineTo(_lastInput, _lastPressure);
        BrushStroke stroke = Stroke;
        PaintTarget target = Target;
        Stroke = null;
        Target = null;
        _lastStrokeEnd = _lastInput;
        if (stroke.HasPainted) OnStrokeFinished(stroke, target);
        Host.InvalidateCanvas(stroke.Dirty);
        stroke.Dispose();
    }

    public override void Deactivate()
    {
        Hover = null;
        base.Deactivate();
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if (key == Key.OemOpenBrackets && !modifiers.HasFlag(KeyModifiers.Shift))
        {
            Size = Math.Max(1, Size < 10 ? Size - 1 : Size < 100 ? Size - 5 : Size * 0.9f);
            Host.RefreshOptions();
            Host.InvalidateOverlay();
            return true;
        }
        if (key == Key.OemCloseBrackets && !modifiers.HasFlag(KeyModifiers.Shift))
        {
            Size = Math.Min(5000, Size < 10 ? Size + 1 : Size < 100 ? Size + 5 : Size * 1.1f);
            Host.RefreshOptions();
            Host.InvalidateOverlay();
            return true;
        }
        if (key == Key.OemOpenBrackets || key == Key.OemCloseBrackets)
        {
            Tip.Hardness = Math.Clamp(Tip.Hardness + (key == Key.OemCloseBrackets ? 0.25f : -0.25f), 0, 1);
            Host.RefreshOptions();
            return true;
        }
        if (key >= Key.D0 && key <= Key.D9 && modifiers == KeyModifiers.None)
        {
            int digit = key - Key.D0;
            Opacity = digit == 0 ? 1 : digit / 10f;
            Host.RefreshOptions();
            return true;
        }
        return base.KeyDown(key, modifiers);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (Hover is SKPoint p && Size * context.Zoom >= 3)
        {
            if (Tip.Roundness < 0.99f || Tip.Shape == BrushTipShape.Square)
            {
                float r = Size / 2;
                float angle = Tip.Angle * MathF.PI / 180;
                List<SKPoint> points = [];
                for (int i = 0; i < 48; i++)
                {
                    float t = i / 48f * MathF.PI * 2;
                    float u = MathF.Cos(t) * r, v = MathF.Sin(t) * r * Tip.Roundness;
                    points.Add(new SKPoint(p.X + u * MathF.Cos(angle) - v * MathF.Sin(angle), p.Y + u * MathF.Sin(angle) + v * MathF.Cos(angle)));
                }
                context.Polygon(points, true);
            }
            else
            {
                context.Circle(p, Size / 2 * context.Zoom);
            }
        }
    }

    protected void BuildBrushOptions(OptionsBuilder options, bool blendMode = true, bool opacity = true, bool flow = true)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Separator();
        if (blendMode) options.BlendMode(() => BlendMode, m => BlendMode = m);
        if (opacity) options.Percent("Opacity:", () => Opacity, v => Opacity = v, "Maximum paint coverage per stroke (0-9 keys)");
        if (flow && ShowFlow) options.Percent("Flow:", () => Flow, v => Flow = v, "Paint applied per dab");
        if (flow && ShowFlow) options.IconButton("airbrush", "Enable airbrush-style build-up effects", () => { Airbrush = !Airbrush; Host.RefreshOptions(); }, Airbrush);
        options.Percent("Smoothing:", () => Smoothing, v => Smoothing = v);
        options.Check("Pressure Size", () => Tip.PressureSize, v => Tip.PressureSize = v, "Pen pressure controls size");
    }
}

public sealed class BrushTool : BrushToolBase
{
    public BrushTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Brush;

    public override string Name => "Brush Tool";

    public override string Hint => "Paint with the foreground color. Shift-click draws a straight line. [ ] change size.";

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input) => stroke.Color = Host.Foreground;

    public override void BuildOptions(OptionsBuilder options) => BuildBrushOptions(options);
}

public sealed class PencilTool : BrushToolBase
{
    public PencilTool(IEditorHost host) : base(host)
    {
        Size = 1;
        Tip = new BrushPreset { Name = "Hard Pixel", Hardness = 1, Spacing = 0.05f, PressureOpacity = false };
        Smoothing = 0;
    }

    public override ToolId Id => ToolId.Pencil;

    public override string Name => "Pencil Tool";

    public override string Hint => "Paint hard-edged, aliased lines with the foreground color.";

    public bool AutoErase { get; set; }

    protected override bool ShowFlow => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        stroke.Aliased = true;
        stroke.Color = Host.Foreground;
        if (AutoErase && Doc?.ActiveLayer is RasterLayer layer)
        {
            // Auto Erase paints the background color when starting on a foreground colored pixel.
            uint start = ImageOps.GetPixel(layer.Surface.Bitmap, input.DocPixel.X - layer.Offset.X, input.DocPixel.Y - layer.Offset.Y);
            if (ColorMath.Distance(start, ColorMath.FromColor(Host.Foreground)) < 4) stroke.Color = Host.Background;
        }
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        BuildBrushOptions(options);
        options.Check("Auto Erase", () => AutoErase, v => AutoErase = v);
    }
}

public sealed class ColorReplacementTool : BrushToolBase
{
    public ColorReplacementTool(IEditorHost host) : base(host)
    {
        Tip = new BrushPreset { Hardness = 1, Spacing = 0.25f };
    }

    public override ToolId Id => ToolId.ColorReplacement;

    public override string Name => "Color Replacement Tool";

    public override string Hint => "Replace colors similar to the sampled color with the foreground color.";

    public DabProcessors.ReplaceMode ReplaceMode { get; set; } = DabProcessors.ReplaceMode.Color;

    public int Tolerance { get; set; } = 30;

    public bool Continuous { get; set; } = true;

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        DabProcessors.ColorReplace processor = new(Host.Foreground, ReplaceMode, (int)(Tolerance * 2.55f), Continuous);
        stroke.Processor = processor.Process;
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.EnumCombo("Mode:", () => ReplaceMode, v => ReplaceMode = v, 100);
        options.Combo("Sampling:", [("Continuous", true), ("Once", false)], () => Continuous, v => Continuous = v, 100);
        options.Number("Tolerance:", 1, 100, () => Tolerance, v => Tolerance = (int)v, "0", "%");
    }
}

public sealed class MixerBrushTool : BrushToolBase
{
    public MixerBrushTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.MixerBrush;

    public override string Name => "Mixer Brush Tool";

    public override string Hint => "Mix the loaded paint with the canvas colors like real paint.";

    public float Wet { get; set; } = 0.5f;

    public float Load { get; set; } = 0.5f;

    public float Mix { get; set; } = 0.5f;

    public bool CleanAfterStroke { get; set; } = true;

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        DabProcessors.Mixer mixer = new(Host.Foreground, Wet, Load, Mix, false);
        stroke.Processor = mixer.Process;
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.ColorButton("Load color (foreground)", () => Host.Foreground, c => Host.Foreground = c);
        options.Percent("Wet:", () => Wet, v => Wet = v);
        options.Percent("Load:", () => Load, v => Load = v);
        options.Percent("Mix:", () => Mix, v => Mix = v);
        options.Percent("Opacity:", () => Opacity, v => Opacity = v);
        options.Check("Clean after each stroke", () => CleanAfterStroke, v => CleanAfterStroke = v);
    }
}

/// <summary>Paints a local, non-destructive adjustment by painting the mask of an adjustment layer.</summary>
public sealed class AdjustmentBrushTool : BrushToolBase
{
    private AdjustmentLayer? _layer;

    public AdjustmentBrushTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.AdjustmentBrush;

    public override string Name => "Adjustment Brush Tool";

    public override string Hint => "Paint a local adjustment. Each stroke edits the mask of an adjustment layer. Alt erases the adjustment.";

    public AdjustmentKind Kind { get; set; } = AdjustmentKind.Exposure;

    public float Amount { get; set; } = 1;

    public bool NewLayerEachTime { get; set; }

    protected override bool SupportsMaskTargets => true;

    private AdjustmentSettings CreateSettings()
    {
        AdjustmentSettings settings = new(Kind);
        switch (Kind)
        {
            case AdjustmentKind.Exposure: settings.Set("exposure", Amount); break;
            case AdjustmentKind.BrightnessContrast: settings.Set("brightness", Amount * 50); break;
            case AdjustmentKind.HueSaturation: settings.Set("saturation", Amount * 40); break;
            case AdjustmentKind.Vibrance: settings.Set("vibrance", Amount * 60); break;
            case AdjustmentKind.ColorBalance: settings.Set("midtones.cr", Amount * 40); break;
        }
        return settings;
    }

    protected override bool CanStart(ToolInput input)
    {
        if (Doc == null) return false;
        if (!NewLayerEachTime && Doc.ActiveLayer is AdjustmentLayer existing && existing.Mask != null && existing.Settings.Kind == Kind)
        {
            _layer = existing;
        }
        else
        {
            _layer = new AdjustmentLayer($"{AdjustmentSettings.GetName(Kind)} Brush", CreateSettings())
            {
                Mask = PixelSurface.CreateMask(Doc.Width, Doc.Height, 0)
            };
            Doc.AddLayer(_layer);
        }
        Doc.ActiveLayer = _layer;
        Doc.EditingMask = true;
        Host.RefreshPanels();
        return true;
    }

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input) => stroke.Color = input.Alt ? SKColors.Black : SKColors.White;

    protected override void OnStrokeFinished(BrushStroke stroke, PaintTarget target)
    {
        PushHistory("Adjustment Brush");
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Combo("Adjustment:", new[] { AdjustmentKind.Exposure, AdjustmentKind.BrightnessContrast, AdjustmentKind.HueSaturation, AdjustmentKind.Vibrance, AdjustmentKind.ColorBalance }
            .Select(k => (AdjustmentSettings.GetName(k), k)), () => Kind, v => Kind = v, 150);
        options.Number("Amount:", -3, 3, () => Amount, v => Amount = (float)v, "0.00", null, 0.05);
        options.Percent("Opacity:", () => Opacity, v => Opacity = v);
        options.Percent("Flow:", () => Flow, v => Flow = v);
        options.Check("New layer per stroke", () => NewLayerEachTime, v => NewLayerEachTime = v);
    }
}

/// <summary>Captures source pixels at stroke start for cloning tools.</summary>
public abstract class SourceBrushToolBase : BrushToolBase
{
    protected SourceBrushToolBase(IEditorHost host) : base(host)
    {
    }

    public bool SampleAllLayers { get; set; }

    protected SKBitmap? SourceImage { get; set; }

    protected static Func<int, int, uint> Sampler(SKBitmap bitmap, Func<int, int, (int X, int Y)> map) => (x, y) =>
    {
        var (sx, sy) = map(x, y);
        return ImageOps.GetPixel(bitmap, sx, sy);
    };

    public override void PointerUp(ToolInput input)
    {
        base.PointerUp(input);
        SourceImage?.Dispose();
        SourceImage = null;
    }
}

public sealed class CloneStampTool : SourceBrushToolBase
{
    private SKPoint? _source;
    private SKPoint? _offset;
    private SKPoint _strokeStart;

    public CloneStampTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.CloneStamp;

    public override string Name => "Clone Stamp Tool";

    public override string Hint => "Alt-click to set the source point, then paint to clone it.";

    public bool Aligned { get; set; } = true;

    protected override bool CanStart(ToolInput input)
    {
        if (input.Alt)
        {
            _source = input.Doc;
            _offset = null;
            Host.SetStatus($"Clone source: {input.Doc.X:0}, {input.Doc.Y:0}");
            return false;
        }
        if (_source == null)
        {
            Host.ShowMessage("Could not use the Clone Stamp tool because the area to clone has not been defined (Alt-click to define a source point).");
            return false;
        }
        return true;
    }

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        SourceImage = Host.GetSampleImage(SampleAllLayers);
        if (_offset == null || !Aligned) _offset = _source!.Value - input.Doc;
        _strokeStart = input.Doc;
        SKPoint offset = _offset.Value;
        stroke.Source = Sampler(SourceImage, (x, y) => ((int)MathF.Round(x + offset.X), (int)MathF.Round(y + offset.Y)));
    }

    public override void DrawOverlay(OverlayContext context)
    {
        base.DrawOverlay(context);
        if (Hover is SKPoint hover && (_offset is SKPoint offset || _source != null))
        {
            SKPoint src = _offset is SKPoint o ? hover + o : _source!.Value;
            context.Line(src - new SKPoint(context.DocLength(6), 0), src + new SKPoint(context.DocLength(6), 0));
            context.Line(src - new SKPoint(0, context.DocLength(6)), src + new SKPoint(0, context.DocLength(6)));
        }
        _ = _strokeStart;
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        BuildBrushOptions(options);
        options.Check("Aligned", () => Aligned, v => Aligned = v);
        options.Combo("Sample:", [("Current Layer", false), ("All Layers", true)], () => SampleAllLayers, v => SampleAllLayers = v, 110);
    }
}

public sealed class PatternStampTool : BrushToolBase
{
    private SKPoint _strokeOrigin;

    public PatternStampTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.PatternStamp;

    public override string Name => "Pattern Stamp Tool";

    public override string Hint => "Paint with the selected pattern.";

    public bool Aligned { get; set; } = true;

    public bool Impressionist { get; set; }

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        PatternDefinition pattern = Host.Pattern ?? Host.Patterns[0];
        _strokeOrigin = Aligned ? default : input.Doc;
        int ox = (int)_strokeOrigin.X, oy = (int)_strokeOrigin.Y;
        if (Impressionist)
        {
            Random random = new();
            stroke.Source = (x, y) => pattern.Sample(x - ox + random.Next(-3, 4), y - oy + random.Next(-3, 4));
        }
        else
        {
            stroke.Source = (x, y) => pattern.Sample(x - ox, y - oy);
        }
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        BuildBrushOptions(options);
        options.Combo("Pattern:", Host.Patterns.Select(p => (p.Name, p)), () => Host.Pattern ?? Host.Patterns[0], p => Host.Pattern = p, 130);
        options.Check("Aligned", () => Aligned, v => Aligned = v);
        options.Check("Impressionist", () => Impressionist, v => Impressionist = v);
    }
}

public sealed class HistoryBrushTool : BrushToolBase
{
    public HistoryBrushTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.HistoryBrush;

    public override string Name => "History Brush Tool";

    public override string Hint => "Paint pixels back from the history state selected as the source in the History panel.";

    protected override bool CanStart(ToolInput input)
    {
        if (Doc?.History.Source == null)
        {
            Host.ShowMessage("Set a history source in the History panel first.");
            return false;
        }
        if (Doc.History.Source.State.Width != Doc.Width || Doc.History.Source.State.Height != Doc.Height)
        {
            Host.ShowMessage("Could not use the History Brush because the history state does not have the same canvas size.");
            return false;
        }
        return true;
    }

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        stroke.Source = HistorySampler(Doc!, Target?.Layer);
    }

    /// <summary>Samples the history source: the same layer in that state if it exists, otherwise the flattened state.</summary>
    public static Func<int, int, uint> HistorySampler(PhotoDocument doc, RasterLayer? layer)
    {
        DocumentState state = doc.History.Source!.State;
        RasterLayer? sourceLayer = layer != null ? state.FindRasterLayer(layer.Id) : null;
        if (sourceLayer != null)
        {
            SKBitmap bitmap = sourceLayer.Surface.Bitmap;
            SKPointI offset = sourceLayer.Offset;
            return (x, y) => ImageOps.GetPixel(bitmap, x - offset.X, y - offset.Y);
        }
        SKBitmap composite = state.GetComposite();
        return (x, y) => ImageOps.GetPixel(composite, x, y);
    }

    public override void BuildOptions(OptionsBuilder options) => BuildBrushOptions(options);
}

/// <summary>Paints stylized strokes using colors from the history source.</summary>
public sealed class ArtHistoryBrushTool : BrushToolBase
{
    public enum ArtStyle { TightShort, TightMedium, TightLong, LooseMedium, LooseLong, Dab, TightCurl, LooseCurl }

    public ArtHistoryBrushTool(IEditorHost host) : base(host)
    {
        Size = 10;
        Tip = new BrushPreset { Hardness = 0.5f, Spacing = 0.5f };
    }

    public override ToolId Id => ToolId.ArtHistoryBrush;

    public override string Name => "Art History Brush Tool";

    public override string Hint => "Paint stylized strokes sampled from the history source.";

    public ArtStyle Style { get; set; } = ArtStyle.TightShort;

    public float Area { get; set; } = 50;

    public int Tolerance { get; set; }

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override bool CanStart(ToolInput input)
    {
        if (Doc?.History.Source == null)
        {
            Host.ShowMessage("Set a history source in the History panel first.");
            return false;
        }
        return true;
    }

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        Func<int, int, uint> sample = HistoryBrushTool.HistorySampler(Doc!, Target?.Layer);
        Random random = new();
        float area = Area;
        float size = Size;
        ArtStyle style = Style;
        int tolerance = Tolerance;
        stroke.SpacingPixels = Math.Max(4, area / 3);
        stroke.Processor = (s, rect, alpha, center) =>
        {
            using SKCanvas canvas = new(s.Target.Bitmap);
            int count = (int)Math.Clamp(area / 4, 4, 40);
            (float length, float curl, float spread) = style switch
            {
                ArtStyle.TightShort => (size * 1.5f, 0f, 0.5f),
                ArtStyle.TightMedium => (size * 3, 0f, 0.5f),
                ArtStyle.TightLong => (size * 5, 0f, 0.5f),
                ArtStyle.LooseMedium => (size * 3, 0.3f, 1f),
                ArtStyle.LooseLong => (size * 5, 0.3f, 1f),
                ArtStyle.TightCurl => (size * 3, 1.2f, 0.5f),
                ArtStyle.LooseCurl => (size * 4, 1.6f, 1f),
                _ => (size * 0.2f, 0f, 1f)
            };
            using SKPaint paint = new() { IsAntialias = true, StrokeWidth = Math.Max(1, size / 3), StrokeCap = SKStrokeCap.Round, Style = SKPaintStyle.Stroke };
            for (int i = 0; i < count; i++)
            {
                float px = center.X + (random.NextSingle() * 2 - 1) * area * spread;
                float py = center.Y + (random.NextSingle() * 2 - 1) * area * spread;
                uint c = sample((int)px + s.Offset.X, (int)py + s.Offset.Y);
                if ((c >> 24) == 0) continue;
                uint current = ImageOps.GetPixel(s.Target.Bitmap, (int)px, (int)py);
                if (tolerance > 0 && ColorMath.Distance(current, c) <= tolerance * 2.55f) continue;
                if (s.Selection != null && MaskOps.Get(s.Selection, (int)px + s.Offset.X, (int)py + s.Offset.Y) < 128) continue;
                paint.Color = ColorMath.ToColor(c).WithAlpha((byte)(s.Opacity * 255));
                float angle = random.NextSingle() * MathF.PI * 2;
                using SKPath path = new();
                path.MoveTo(px, py);
                float x = px, y = py;
                for (int k = 0; k < 6; k++)
                {
                    angle += curl * (random.NextSingle() - 0.3f);
                    x += MathF.Cos(angle) * length / 6;
                    y += MathF.Sin(angle) * length / 6;
                    path.LineTo(x, y);
                }
                canvas.DrawPath(path, paint);
            }
        };
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Percent("Opacity:", () => Opacity, v => Opacity = v);
        options.EnumCombo("Style:", () => Style, v => Style = v, 120);
        options.Number("Area:", 1, 500, () => Area, v => Area = (float)v, "0", "px");
        options.Number("Tolerance:", 0, 100, () => Tolerance, v => Tolerance = (int)v, "0", "%");
    }
}

public enum EraserMode
{
    Brush,
    Pencil,
    Block
}

public sealed class EraserTool : BrushToolBase
{
    public EraserTool(IEditorHost host) : base(host)
    {
        Tip = new BrushPreset { Hardness = 1, Spacing = 0.1f };
        Smoothing = 0;
    }

    public override ToolId Id => ToolId.Eraser;

    public override string Name => "Eraser Tool";

    public override string Hint => "Erase to transparency, or to the background color on locked layers. Alt erases to history.";

    public EraserMode EraseMode { get; set; }

    public bool EraseToHistory { get; set; }

    public override float BrushOutlineSize => EraseMode == EraserMode.Block ? 16 / (float)(Host.View?.Zoom ?? 1) : Size;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        if (EraseMode == EraserMode.Block)
        {
            stroke.Size = 16 / (float)(Host.View?.Zoom ?? 1);
            stroke.Aliased = true;
        }
        if (EraseMode == EraserMode.Pencil) stroke.Aliased = true;
        if (EraseToHistory || input.Alt)
        {
            if (Doc?.History.Source != null) stroke.Source = HistoryBrushTool.HistorySampler(Doc, Target?.Layer);
            return;
        }
        if (Target?.IsMask == true)
        {
            stroke.Color = Host.Background;
            return;
        }
        if (stroke.LockTransparency)
        {
            // Layers with locked transparency (like the background) are erased to the background color.
            stroke.Color = Host.Background;
            stroke.BlendMode = LayerBlendMode.Normal;
        }
        else
        {
            stroke.BlendMode = LayerBlendMode.Clear;
            stroke.Color = SKColors.Black;
        }
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (EraseMode == EraserMode.Block && Hover is SKPoint p)
        {
            float half = context.DocLength(8);
            context.Rect(new SKRect(p.X - half, p.Y - half, p.X + half, p.Y + half));
            return;
        }
        base.DrawOverlay(context);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.EnumCombo("Mode:", () => EraseMode, v => EraseMode = v, 90);
        options.Percent("Opacity:", () => Opacity, v => Opacity = v);
        options.Percent("Flow:", () => Flow, v => Flow = v);
        options.Percent("Smoothing:", () => Smoothing, v => Smoothing = v);
        options.Check("Erase to History", () => EraseToHistory, v => EraseToHistory = v);
    }
}

public sealed class BackgroundEraserTool : BrushToolBase
{
    public enum Sampling { Continuous, Once, BackgroundSwatch }

    public BackgroundEraserTool(IEditorHost host) : base(host)
    {
        Tip = new BrushPreset { Hardness = 1, Spacing = 0.25f };
        Size = 50;
        Smoothing = 0;
    }

    public override ToolId Id => ToolId.BackgroundEraser;

    public override string Name => "Background Eraser Tool";

    public override string Hint => "Erase pixels that match the color under the brush center (hot spot).";

    public Sampling SampleMode { get; set; }

    public bool Contiguous { get; set; } = true;

    public int Tolerance { get; set; } = 50;

    public bool ProtectForeground { get; set; }

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override bool CanStart(ToolInput input)
    {
        // The background eraser converts locked background layers to normal layers to allow transparency.
        if (Doc?.ActiveLayer is RasterLayer { IsBackground: true } layer)
        {
            layer.IsBackground = false;
            layer.LockTransparency = false;
            layer.LockPosition = false;
            layer.Name = "Layer 0";
            Host.RefreshPanels();
        }
        return true;
    }

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        DabProcessors.BackgroundEraser processor = new((int)(Tolerance * 2.55f), SampleMode == Sampling.Continuous, Contiguous,
            ProtectForeground ? Host.Foreground : null, SampleMode == Sampling.BackgroundSwatch ? Host.Background : null);
        stroke.Processor = processor.Process;
        stroke.LockTransparency = false;
    }

    public override void DrawOverlay(OverlayContext context)
    {
        base.DrawOverlay(context);
        if (Hover is SKPoint p)
        {
            float s = context.DocLength(4);
            context.Line(p - new SKPoint(s, 0), p + new SKPoint(s, 0));
            context.Line(p - new SKPoint(0, s), p + new SKPoint(0, s));
        }
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.EnumCombo("Sampling:", () => SampleMode, v => SampleMode = v, 140);
        options.Combo("Limits:", [("Contiguous", true), ("Discontiguous", false)], () => Contiguous, v => Contiguous = v, 110);
        options.Number("Tolerance:", 0, 100, () => Tolerance, v => Tolerance = (int)v, "0", "%");
        options.Check("Protect Foreground Color", () => ProtectForeground, v => ProtectForeground = v);
    }
}

public sealed class BlurTool : BrushToolBase
{
    public BlurTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Blur;

    public override string Name => "Blur Tool";

    public override string Hint => "Paint to soften edges and details.";

    public float Strength { get; set; } = 0.5f;

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input) => stroke.Processor = DabProcessors.Blur(Strength);

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Percent("Strength:", () => Strength, v => Strength = v);
    }
}

public sealed class SharpenTool : BrushToolBase
{
    public SharpenTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Sharpen;

    public override string Name => "Sharpen Tool";

    public override string Hint => "Paint to increase local contrast and sharpen details.";

    public float Strength { get; set; } = 0.5f;

    public bool ProtectDetail { get; set; } = true;

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input) => stroke.Processor = DabProcessors.Sharpen(Strength, ProtectDetail);

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Percent("Strength:", () => Strength, v => Strength = v);
        options.Check("Protect Detail", () => ProtectDetail, v => ProtectDetail = v);
    }
}

public sealed class SmudgeTool : BrushToolBase
{
    public SmudgeTool(IEditorHost host) : base(host)
    {
        Tip = new BrushPreset { Hardness = 0.3f, Spacing = 0.05f };
    }

    public override ToolId Id => ToolId.Smudge;

    public override string Name => "Smudge Tool";

    public override string Hint => "Drag to smear paint. Finger Painting starts with the foreground color.";

    public float Strength { get; set; } = 0.5f;

    public bool FingerPainting { get; set; }

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        DabProcessors.Smudge smudge = new(Strength, FingerPainting || input.Alt ? Host.Foreground : null);
        stroke.Processor = smudge.Process;
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Percent("Strength:", () => Strength, v => Strength = v);
        options.Check("Finger Painting", () => FingerPainting, v => FingerPainting = v);
    }
}

public sealed class DodgeBurnTool : BrushToolBase
{
    private readonly bool _dodge;

    public DodgeBurnTool(IEditorHost host, bool dodge) : base(host)
    {
        _dodge = dodge;
        Size = 65;
    }

    public override ToolId Id => _dodge ? ToolId.Dodge : ToolId.Burn;

    public override string Name => _dodge ? "Dodge Tool" : "Burn Tool";

    public override string Hint => _dodge ? "Paint to lighten areas. Alt burns." : "Paint to darken areas. Alt dodges.";

    public DabProcessors.ToneRange Range { get; set; } = DabProcessors.ToneRange.Midtones;

    public float Exposure { get; set; } = 0.5f;

    public bool ProtectTones { get; set; } = true;

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input) =>
        stroke.Processor = DabProcessors.DodgeBurn(_dodge != input.Alt, Range, Exposure, ProtectTones);

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.EnumCombo("Range:", () => Range, v => Range = v, 110);
        options.Percent("Exposure:", () => Exposure, v => Exposure = v);
        options.IconButton("airbrush", "Enable airbrush-style build-up effects", () => { Airbrush = !Airbrush; Host.RefreshOptions(); }, Airbrush);
        options.Check("Protect Tones", () => ProtectTones, v => ProtectTones = v);
    }
}

public sealed class SpongeTool : BrushToolBase
{
    public SpongeTool(IEditorHost host) : base(host)
    {
        Size = 65;
    }

    public override ToolId Id => ToolId.Sponge;

    public override string Name => "Sponge Tool";

    public override string Hint => "Paint to saturate or desaturate colors.";

    public bool Saturate { get; set; }

    public bool Vibrance { get; set; } = true;

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input) => stroke.Processor = DabProcessors.Sponge(Saturate, Flow * 2, Vibrance);

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Combo("Mode:", [("Desaturate", false), ("Saturate", true)], () => Saturate, v => Saturate = v, 110);
        options.Percent("Flow:", () => Flow, v => Flow = v);
        options.Check("Vibrance", () => Vibrance, v => Vibrance = v);
    }
}

public sealed class HealingBrushTool : SourceBrushToolBase
{
    private SKPoint? _source;
    private SKPoint? _offset;

    public HealingBrushTool(IEditorHost host) : base(host)
    {
        Tip = new BrushPreset { Hardness = 1, Spacing = 0.15f };
        Size = 19;
    }

    public override ToolId Id => ToolId.HealingBrush;

    public override string Name => "Healing Brush Tool";

    public override string Hint => "Alt-click to set a source, then paint. Texture comes from the source; color and lighting blend with the destination.";

    public bool Aligned { get; set; } = true;

    protected override bool SupportsMaskTargets => false;

    protected override bool CanStart(ToolInput input)
    {
        if (input.Alt)
        {
            _source = input.Doc;
            _offset = null;
            return false;
        }
        if (_source == null)
        {
            Host.ShowMessage("Alt-click to define a source point for the Healing Brush.");
            return false;
        }
        return true;
    }

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        SourceImage = Host.GetSampleImage(SampleAllLayers);
        if (_offset == null || !Aligned) _offset = _source!.Value - input.Doc;
        SKPoint offset = _offset.Value;
        stroke.Source = Sampler(SourceImage, (x, y) => ((int)MathF.Round(x + offset.X), (int)MathF.Round(y + offset.Y)));
    }

    protected override void OnStrokeFinished(BrushStroke stroke, PaintTarget target)
    {
        // Seamless (Poisson) blend of the cloned texture into the original pixels.
        using SKBitmap original = stroke.CreateBaseBitmap();
        using PixelSurface mask = stroke.GetLocalCoverageMask(8);
        SKRectI local = stroke.Dirty;
        local.Offset(-stroke.Offset.X, -stroke.Offset.Y);
        PoissonBlend.Blend(original, target.Surface.Bitmap, mask, local);
        using (SKCanvas canvas = new(target.Surface.Bitmap))
        using (SKPaint paint = new() { BlendMode = SKBlendMode.Src })
        {
            canvas.Save();
            canvas.ClipRect(SKRect.Create(local.Left, local.Top, local.Width, local.Height));
            canvas.DrawBitmap(original, 0, 0, paint);
            canvas.Restore();
        }
        base.OnStrokeFinished(stroke, target);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        base.DrawOverlay(context);
        if (Hover is SKPoint hover && (_offset != null || _source != null))
        {
            SKPoint src = _offset is SKPoint o ? hover + o : _source!.Value;
            context.Circle(src, 3);
        }
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Check("Aligned", () => Aligned, v => Aligned = v);
        options.Combo("Sample:", [("Current Layer", false), ("All Layers", true)], () => SampleAllLayers, v => SampleAllLayers = v, 110);
    }
}

/// <summary>Paint over blemishes or objects; the painted area is filled with content-aware inpainting on release.</summary>
public sealed class SpotHealingTool : BrushToolBase
{
    private readonly bool _remove;

    public SpotHealingTool(IEditorHost host, bool remove) : base(host)
    {
        _remove = remove;
        Tip = new BrushPreset { Hardness = remove ? 0.8f : 1, Spacing = 0.1f, PressureOpacity = false };
        Size = remove ? 60 : 25;
        Smoothing = 0;
    }

    public override ToolId Id => _remove ? ToolId.Remove : ToolId.SpotHealingBrush;

    public override string Name => _remove ? "Remove Tool" : "Spot Healing Brush Tool";

    public override string Hint => _remove
        ? "Paint over distractions to remove them. The area is reconstructed from its surroundings."
        : "Paint over spots and blemishes to heal them automatically.";

    public enum HealType { ContentAware, CreateTexture, ProximityMatch }

    public HealType Type { get; set; } = HealType.ContentAware;

    public bool SampleAllLayers { get; set; }

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input)
    {
        // Preview the painted area as a translucent overlay; pixels are restored before healing.
        stroke.Color = _remove ? new SKColor(255, 80, 120) : new SKColor(60, 60, 60);
        stroke.Opacity = 0.5f;
        stroke.BlendMode = LayerBlendMode.Normal;
    }

    protected override async void OnStrokeFinished(BrushStroke stroke, PaintTarget target)
    {
        SKRectI local = stroke.Dirty;
        local.Offset(-stroke.Offset.X, -stroke.Offset.Y);
        stroke.RestoreBase(local);
        PixelSurface mask = stroke.GetLocalCoverageMask(10);
        // Grow the hole slightly so the edges of the blemish are replaced too.
        PixelSurface grown = MaskOps.Morph(mask, _remove ? 3 : 2);
        mask.Dispose();
        SKBitmap bitmap = target.Surface.Bitmap;
        int patch = Type switch { HealType.CreateTexture => 2, HealType.ProximityMatch => 4, _ => 3 };
        Host.InvalidateCanvas(stroke.Dirty);
        await Host.RunBusyAsync(_remove ? "Removing..." : "Healing...", progress => Task.Run(() => Inpainting.Fill(bitmap, grown, patch, null, progress)));
        grown.Dispose();
        target.Apply?.Invoke();
        PushHistory(HistoryName);
        Host.InvalidateCanvas(stroke.Dirty);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        if (!_remove) options.EnumCombo("Type:", () => Type, v => Type = v, 130);
        options.Check("Sample All Layers", () => SampleAllLayers, v => SampleAllLayers = v);
    }
}
