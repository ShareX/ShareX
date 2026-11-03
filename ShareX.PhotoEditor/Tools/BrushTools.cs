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
        options.Combo("Sampling:", [("Continuous", true), ("Once", false)], () => Continuous, v => Continuous = v, 120);
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
