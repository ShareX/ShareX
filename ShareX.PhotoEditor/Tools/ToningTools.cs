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
