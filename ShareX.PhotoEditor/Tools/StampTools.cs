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
        options.Combo("Sample:", [("Current Layer", false), ("All Layers", true)], () => SampleAllLayers, v => SampleAllLayers = v, 130);
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
        options.Combo("Sample:", [("Current Layer", false), ("All Layers", true)], () => SampleAllLayers, v => SampleAllLayers = v, 130);
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
