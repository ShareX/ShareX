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
using ShareX.PixelEditor.UI;

namespace ShareX.PixelEditor.Tools;

/// <summary>Base of the tools that paint on the active cel through a <see cref="Painter"/>.</summary>
public abstract class PaintToolBase : ToolBase
{
    protected PaintToolBase(PixelEditorWindow editor, ToolId id, string name, string hint, string icon, Key shortcut) : base(editor, id, name, hint, icon, shortcut)
    {
    }

    protected Painter? Painter { get; private set; }

    protected virtual int Size => Settings.BrushSize;

    public override SKPointI[]? PreviewTip => Tip;

    public override bool HasPendingEdit => Painter != null;

    /// <summary>The brush tip in use: the custom brush when one is defined, else the round or square tip of the tool's size.</summary>
    protected virtual SKPointI[] Tip => Editor.CustomTip ?? Raster.BrushTip(Size, Settings.BrushRound);

    protected virtual void Configure(Painter painter, ToolPointer p)
    {
        painter.Tip = Tip;
        painter.Ink = Settings.Ink;
        painter.Opacity = Math.Clamp(Settings.Opacity, 1, 100) * 255 / 100;
        painter.Pattern = Settings.Pattern;
    }

    protected bool Begin(ToolPointer p)
    {
        Painter = Editor.CreatePainter(p.Right);
        if (Painter == null) return false;
        Configure(Painter, p);
        return true;
    }

    protected void Flush()
    {
        if (Painter == null) return;
        SKRectI dirty = Painter.TakeDirty();
        if (!dirty.IsEmpty) Editor.InvalidateCanvas(dirty);
    }

    protected void End()
    {
        if (Painter == null) return;
        bool changed = !Painter.TotalDirty.IsEmpty;
        Painter = null;
        if (changed) Editor.Commit(Name);
        else Editor.RevertEdit();
    }

    public override void Cancel()
    {
        if (Painter == null) return;
        Painter = null;
        Editor.RevertEdit();
    }

    protected void BrushOptions(OptionsBuilder options, bool ink = true)
    {
        if (Editor.CustomTip != null)
        {
            options.Label(Strings.Options_CustomBrush);
            options.Button(Strings.Options_ClearCustomBrush, () =>
            {
                Editor.CustomTip = null;
                Editor.RefreshOptions();
                Editor.Canvas.InvalidateVisual();
            });
        }
        else
        {
            options.Number(Strings.Options_Size, 1, 64, Settings.BrushSize, v => Settings.BrushSize = v, "px");
            options.Choice(
            [
                (PixelEditorIcons.TipRound, Strings.Options_RoundTip, true),
                (PixelEditorIcons.TipSquare, Strings.Options_SquareTip, false)
            ], Settings.BrushRound, v => Settings.BrushRound = v);
        }
        options.Separator();
        options.Number(Strings.Options_Opacity, 1, 100, Settings.Opacity, v => Settings.Opacity = v, "%");
        if (ink)
        {
            options.Combo(Strings.Options_Ink, EnumNames.Inks, Settings.Ink, v => Settings.Ink = v);
        }
        options.Combo(Strings.Options_Pattern, EnumNames.Patterns, Settings.Pattern, v => Settings.Pattern = v);
    }
}

public sealed class PencilTool : PaintToolBase
{
    private readonly List<SKPointI> _points = [];
    private SKPointI? _last;

    public PencilTool(PixelEditorWindow editor) : base(editor, ToolId.Pencil, Strings.Tool_Pencil, Strings.Tool_Pencil_Hint, PixelEditorIcons.Pencil, Key.B)
    {
    }

    public override void Down(ToolPointer p)
    {
        if (!Begin(p)) return;
        _points.Clear();
        if (p.Shift && _last is SKPointI last) Painter!.Line(last.X, last.Y, p.X, p.Y);
        else Painter!.Stamp(p.X, p.Y);
        _points.Add(p.Pixel);
        Flush();
    }

    public override void Move(ToolPointer p)
    {
        if (Painter == null || _points.Count == 0) return;
        SKPointI from = _points[^1];
        if (from == p.Pixel) return;
        bool first = true;
        Raster.Line(from.X, from.Y, p.X, p.Y, (x, y) =>
        {
            if (first)
            {
                first = false;
                return;
            }
            AddPoint(new SKPointI(x, y));
        });
        Flush();
    }

    private void AddPoint(SKPointI point)
    {
        // Pixel perfect: a point that only forms the corner of an L between its neighbors is removed again.
        if (Settings.PixelPerfect && Size == 1 && _points.Count >= 2)
        {
            SKPointI a = _points[^2], b = _points[^1];
            if ((a.X == b.X || a.Y == b.Y) && (point.X == b.X || point.Y == b.Y) && a.X != point.X && a.Y != point.Y)
            {
                Painter!.RestoreStamp(b.X, b.Y);
                _points.RemoveAt(_points.Count - 1);
            }
        }
        Painter!.Stamp(point.X, point.Y);
        _points.Add(point);
    }

    public override void Up(ToolPointer p)
    {
        if (Painter == null) return;
        _last = _points.Count > 0 ? _points[^1] : p.Pixel;
        End();
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        BrushOptions(options);
        options.Separator();
        options.Check(Strings.Options_PixelPerfect, Settings.PixelPerfect, v => Settings.PixelPerfect = v, Strings.Options_PixelPerfect_Tip);
    }
}

public sealed class EraserTool : PaintToolBase
{
    private SKPointI _previous;

    public EraserTool(PixelEditorWindow editor) : base(editor, ToolId.Eraser, Strings.Tool_Eraser, Strings.Tool_Eraser_Hint, PixelEditorIcons.Eraser, Key.E)
    {
    }

    protected override int Size => Settings.EraserSize;

    protected override void Configure(Painter painter, ToolPointer p)
    {
        base.Configure(painter, p);
        painter.Ink = Ink.Erase;
        painter.Pattern = BrushPattern.Solid;
    }

    public override void Down(ToolPointer p)
    {
        if (!Begin(p)) return;
        Painter!.Stamp(p.X, p.Y);
        _previous = p.Pixel;
        Flush();
    }

    public override void Move(ToolPointer p)
    {
        if (Painter == null || p.Pixel == _previous) return;
        Painter.Line(_previous.X, _previous.Y, p.X, p.Y);
        _previous = p.Pixel;
        Flush();
    }

    public override void Up(ToolPointer p) => End();

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Number(Strings.Options_Size, 1, 64, Settings.EraserSize, v => Settings.EraserSize = v, "px");
        options.Choice(
        [
            (PixelEditorIcons.TipRound, Strings.Options_RoundTip, true),
            (PixelEditorIcons.TipSquare, Strings.Options_SquareTip, false)
        ], Settings.BrushRound, v => Settings.BrushRound = v);
        options.Separator();
        options.Number(Strings.Options_Opacity, 1, 100, Settings.Opacity, v => Settings.Opacity = v, "%");
    }
}

public sealed class SprayTool : PaintToolBase
{
    private readonly Random _random = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(40) };
    private SKPointI _position;

    public SprayTool(PixelEditorWindow editor) : base(editor, ToolId.Spray, Strings.Tool_Spray, Strings.Tool_Spray_Hint, PixelEditorIcons.Spray, Key.B)
    {
        _timer.Tick += (_, _) => Emit();
    }

    public override SKPointI[]? PreviewTip => Raster.BrushTip(Math.Max(1, Settings.SprayRadius * 2), true);

    private void Emit()
    {
        if (Painter == null) return;
        int radius = Math.Max(1, Settings.SprayRadius), count = Math.Max(1, Settings.SprayDensity * radius / 8);
        for (int i = 0; i < count; i++)
        {
            double angle = _random.NextDouble() * Math.PI * 2, distance = Math.Sqrt(_random.NextDouble()) * radius;
            Painter.Stamp(_position.X + (int)Math.Round(Math.Cos(angle) * distance), _position.Y + (int)Math.Round(Math.Sin(angle) * distance));
        }
        Flush();
    }

    public override void Down(ToolPointer p)
    {
        if (!Begin(p)) return;
        _position = p.Pixel;
        Emit();
        _timer.Start();
    }

    public override void Move(ToolPointer p) => _position = p.Pixel;

    public override void Up(ToolPointer p)
    {
        _timer.Stop();
        End();
    }

    public override void Cancel()
    {
        _timer.Stop();
        base.Cancel();
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Number(Strings.Options_Radius, 1, 64, Settings.SprayRadius, v => Settings.SprayRadius = v, "px");
        options.Number(Strings.Options_Density, 1, 100, Settings.SprayDensity, v => Settings.SprayDensity = v);
        options.Separator();
        BrushOptions(options);
    }
}

/// <summary>Lightens (left button) or darkens (right button) pixels along the palette.</summary>
public sealed class ShadeTool : PaintToolBase
{
    private SKPointI _previous;

    public ShadeTool(PixelEditorWindow editor) : base(editor, ToolId.Shade, Strings.Tool_Shade, Strings.Tool_Shade_Hint, PixelEditorIcons.Shade, Key.O)
    {
    }

    protected override void Configure(Painter painter, ToolPointer p)
    {
        base.Configure(painter, p);
        painter.Ink = p.Right ? Ink.Darken : Ink.Lighten;
        painter.Pattern = Settings.Pattern;
    }

    public override void Down(ToolPointer p)
    {
        if (!Begin(p)) return;
        Painter!.Stamp(p.X, p.Y);
        _previous = p.Pixel;
        Flush();
    }

    public override void Move(ToolPointer p)
    {
        if (Painter == null || p.Pixel == _previous) return;
        Painter.Line(_previous.X, _previous.Y, p.X, p.Y);
        _previous = p.Pixel;
        Flush();
    }

    public override void Up(ToolPointer p) => End();

    public override void BuildOptions(OptionsBuilder options)
    {
        BrushOptions(options, false);
        options.Separator();
        options.Label(Strings.Options_ShadeHint);
    }
}

/// <summary>Paints the foreground color only over pixels of the background color (and the reverse with the right button).</summary>
public sealed class ColorReplaceTool : PaintToolBase
{
    private SKPointI _previous;

    public ColorReplaceTool(PixelEditorWindow editor) : base(editor, ToolId.ColorReplace, Strings.Tool_ColorReplace, Strings.Tool_ColorReplace_Hint, PixelEditorIcons.ColorReplace, Key.O)
    {
    }

    protected override void Configure(Painter painter, ToolPointer p)
    {
        base.Configure(painter, p);
        painter.Ink = Ink.ReplaceColor;
        painter.SecondaryColor = p.Right ? Editor.Foreground : Editor.Background;
        painter.Tolerance = Settings.FillTolerance;
    }

    public override void Down(ToolPointer p)
    {
        if (!Begin(p)) return;
        Painter!.Stamp(p.X, p.Y);
        _previous = p.Pixel;
        Flush();
    }

    public override void Move(ToolPointer p)
    {
        if (Painter == null || p.Pixel == _previous) return;
        Painter.Line(_previous.X, _previous.Y, p.X, p.Y);
        _previous = p.Pixel;
        Flush();
    }

    public override void Up(ToolPointer p) => End();

    public override void BuildOptions(OptionsBuilder options)
    {
        BrushOptions(options, false);
        options.Separator();
        options.Number(Strings.Options_Tolerance, 0, 255, Settings.FillTolerance, v => Settings.FillTolerance = v);
    }
}

public sealed class BucketTool : PaintToolBase
{
    public BucketTool(PixelEditorWindow editor) : base(editor, ToolId.Bucket, Strings.Tool_Bucket, Strings.Tool_Bucket_Hint, PixelEditorIcons.Bucket, Key.G)
    {
    }

    public override SKPointI[]? PreviewTip => null;

    public override void Down(ToolPointer p)
    {
        if (Sprite == null || !Sprite.Bounds.Contains(p.X, p.Y)) return;
        if (!Begin(p)) return;
        Painter!.Tip = [new SKPointI(0, 0)];
        Painter.Symmetry = SymmetryMode.None;
        PixelBuffer source = Settings.FillAllLayers ? Compositor.Flatten(Sprite, Sprite.ActiveFrame, true) : Painter.Original;
        bool[] region = Raster.FloodRegion(source, p.X, p.Y, Settings.FillTolerance, Settings.FillContiguous, Settings.FillDiagonal, Sprite.Selection?.Data);
        Painter.FillRegion(region);
        Flush();
        End();
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Number(Strings.Options_Tolerance, 0, 255, Settings.FillTolerance, v => Settings.FillTolerance = v);
        options.Check(Strings.Options_Contiguous, Settings.FillContiguous, v => Settings.FillContiguous = v, Strings.Options_Contiguous_Tip);
        options.Check(Strings.Options_Diagonal, Settings.FillDiagonal, v => Settings.FillDiagonal = v, Strings.Options_Diagonal_Tip);
        options.Check(Strings.Options_AllLayers, Settings.FillAllLayers, v => Settings.FillAllLayers = v, Strings.Options_AllLayers_Tip);
        options.Separator();
        options.Number(Strings.Options_Opacity, 1, 100, Settings.Opacity, v => Settings.Opacity = v, "%");
        options.Combo(Strings.Options_Ink, EnumNames.Inks, Settings.Ink, v => Settings.Ink = v);
        options.Combo(Strings.Options_Pattern, EnumNames.Patterns, Settings.Pattern, v => Settings.Pattern = v);
    }
}

public sealed class GradientTool : PaintToolBase
{
    private SKPoint _start, _end;
    private bool _right;

    public GradientTool(PixelEditorWindow editor) : base(editor, ToolId.Gradient, Strings.Tool_Gradient, Strings.Tool_Gradient_Hint, PixelEditorIcons.Gradient, Key.G)
    {
    }

    public override SKPointI[]? PreviewTip => null;

    public override void Down(ToolPointer p)
    {
        if (!Begin(p)) return;
        Painter!.Symmetry = SymmetryMode.None;
        Painter.Pattern = BrushPattern.Solid;
        _right = p.Right;
        _start = _end = new SKPoint(p.X + 0.5f, p.Y + 0.5f);
    }

    public override void Move(ToolPointer p)
    {
        if (Painter == null) return;
        SKPointI end = p.Shift ? ConstrainAngle(new SKPointI((int)_start.X, (int)_start.Y), p.Pixel) : p.Pixel;
        _end = new SKPoint(end.X + 0.5f, end.Y + 0.5f);
        Painter.RestoreAll();
        uint from = _right ? Editor.Background : Editor.Foreground, to = Settings.GradientToTransparent ? 0 : _right ? Editor.Foreground : Editor.Background;
        Painter.Gradient(_start.X, _start.Y, _end.X, _end.Y, from, to, Settings.GradientRadial, Settings.GradientDither);
        Flush();
    }

    public override void Up(ToolPointer p) => End();

    public override void DrawOverlay(OverlayContext overlay)
    {
        if (Painter != null) overlay.Line(_start.X, _start.Y, _end.X, _end.Y);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Choice(
        [
            (PixelEditorIcons.GradientLinear, Strings.Options_Linear, false),
            (PixelEditorIcons.GradientRadial, Strings.Options_Radial, true)
        ], Settings.GradientRadial, v => Settings.GradientRadial = v);
        options.Combo(Strings.Options_Dither, EnumNames.Dithers, Settings.GradientDither, v => Settings.GradientDither = v);
        options.Check(Strings.Options_ToTransparent, Settings.GradientToTransparent, v => Settings.GradientToTransparent = v, Strings.Options_ToTransparent_Tip);
        options.Separator();
        options.Number(Strings.Options_Opacity, 1, 100, Settings.Opacity, v => Settings.Opacity = v, "%");
    }
}

public sealed class EyedropperTool : ToolBase
{
    public EyedropperTool(PixelEditorWindow editor) : base(editor, ToolId.Eyedropper, Strings.Tool_Eyedropper, Strings.Tool_Eyedropper_Hint, PixelEditorIcons.Eyedropper, Key.I)
    {
    }

    private void Pick(ToolPointer p)
    {
        uint? color = Editor.SampleColor(p.X, p.Y, Settings.EyedropperAllLayers);
        if (color is not uint c || c >> 24 == 0) return;
        if (p.Right) Editor.Background = c;
        else Editor.Foreground = c;
    }

    public override void Down(ToolPointer p) => Pick(p);

    public override void Move(ToolPointer p) => Pick(p);

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Check(Strings.Options_AllLayers, Settings.EyedropperAllLayers, v => Settings.EyedropperAllLayers = v, Strings.Options_SampleAllLayers_Tip);
        options.Separator();
        options.Label(Strings.Options_EyedropperHint);
    }
}
