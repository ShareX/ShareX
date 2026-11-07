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

public sealed class BlurTool : BrushToolBase
{
    public BlurTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Blur;

    public override string Name => Strings.ToningTools_BlurTool;

    public override string Hint => Strings.ToningTools_BlurHint;

    public float Strength { get; set; } = 0.5f;

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input) => stroke.Processor = DabProcessors.Blur(Strength);

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Percent(Strings.HdrToningEditor_StrengthColon, () => Strength, v => Strength = v);
    }
}

public sealed class SharpenTool : BrushToolBase
{
    public SharpenTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Sharpen;

    public override string Name => Strings.ToningTools_SharpenTool;

    public override string Hint => Strings.ToningTools_SharpenHint;

    public float Strength { get; set; } = 0.5f;

    public bool ProtectDetail { get; set; } = true;

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input) => stroke.Processor = DabProcessors.Sharpen(Strength, ProtectDetail);

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Percent(Strings.HdrToningEditor_StrengthColon, () => Strength, v => Strength = v);
        options.Check(Strings.ToningTools_ProtectDetail, () => ProtectDetail, v => ProtectDetail = v);
    }
}

public sealed class SmudgeTool : BrushToolBase
{
    public SmudgeTool(IEditorHost host) : base(host)
    {
        Tip = new BrushPreset { Hardness = 0.3f, Spacing = 0.05f };
    }

    public override ToolId Id => ToolId.Smudge;

    public override string Name => Strings.ToningTools_SmudgeTool;

    public override string Hint => Strings.ToningTools_SmudgeHint;

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
        options.Percent(Strings.HdrToningEditor_StrengthColon, () => Strength, v => Strength = v);
        options.Check(Strings.ToningTools_FingerPainting, () => FingerPainting, v => FingerPainting = v);
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

    public override string Name => _dodge ? Strings.ToningTools_DodgeTool : Strings.ToningTools_BurnTool;

    public override string Hint => _dodge ? Strings.ToningTools_DodgeHint : Strings.ToningTools_BurnHint;

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
        options.EnumCombo(Strings.ToningTools_RangeColon, () => Range, v => Range = v, 110);
        options.Percent(Strings.AdjustmentEditor_ExposureColon, () => Exposure, v => Exposure = v);
        options.IconButton(PhotoEditorIcons.Airbrush, Strings.BrushToolBase_AirbrushTip, () => { Airbrush = !Airbrush; Host.RefreshOptions(); }, Airbrush);
        options.Check(Strings.ToningTools_ProtectTones, () => ProtectTones, v => ProtectTones = v);
    }
}

public sealed class SpongeTool : BrushToolBase
{
    public SpongeTool(IEditorHost host) : base(host)
    {
        Size = 65;
    }

    public override ToolId Id => ToolId.Sponge;

    public override string Name => Strings.ToningTools_SpongeTool;

    public override string Hint => Strings.ToningTools_SpongeHint;

    public bool Saturate { get; set; }

    public bool Vibrance { get; set; } = true;

    protected override StrokeMode StrokeMode => StrokeMode.Direct;

    protected override bool SupportsMaskTargets => false;

    protected override void ConfigureStroke(BrushStroke stroke, ToolInput input) => stroke.Processor = DabProcessors.Sponge(Saturate, Flow * 2, Vibrance);

    public override void BuildOptions(OptionsBuilder options)
    {
        options.BrushPicker(() => Tip, t => Tip = t, () => Size, v => Size = v);
        options.Combo(Strings.EditorCommands_ModeColon, [(Strings.Adjustments_Desaturate, false), (Strings.ToningTools_Saturate, true)], () => Saturate, v => Saturate = v, 110);
        options.Percent(Strings.Tools_FlowColon, () => Flow, v => Flow = v);
        options.Check(Strings.Adjustments_Vibrance, () => Vibrance, v => Vibrance = v);
    }
}
