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
