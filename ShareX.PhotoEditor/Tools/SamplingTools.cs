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

using Avalonia;
using Avalonia.Media;
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

public sealed class EyedropperTool : ToolBase
{
    private SKColor? _preview;
    private SKPoint _position;
    private bool _dragging;
    private bool _background;

    public EyedropperTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Eyedropper;

    public override string Name => Strings.SamplingTools_EyedropperTool;

    public override string Hint => Strings.SamplingTools_EyedropperHint;

    public override ToolCursor Cursor => ToolCursor.Eyedropper;

    public int SampleSize { get; set; } = 1;

    public bool AllLayers { get; set; } = true;

    public bool ShowRing { get; set; } = true;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _dragging = true;
        _background = input.Alt;
        Sample(input);
    }

    public override void PointerMove(ToolInput input)
    {
        if (_dragging) Sample(input);
    }

    public override void PointerUp(ToolInput input)
    {
        _dragging = false;
        _preview = null;
        Host.InvalidateOverlay();
    }

    private void Sample(ToolInput input)
    {
        SKColor? color = SampleAt(input.DocPixel);
        if (color is not SKColor c) return;
        _preview = c;
        _position = input.Doc;
        if (_background) Host.Background = c;
        else Host.Foreground = c;
        Host.SetStatus($"R: {c.Red}  G: {c.Green}  B: {c.Blue}  #{ColorMath.ToHex(c)}");
        Host.InvalidateOverlay();
    }

    public SKColor? SampleAt(SKPointI p)
    {
        if (Doc == null || p.X < 0 || p.Y < 0 || p.X >= Doc.Width || p.Y >= Doc.Height) return null;
        if (AllLayers && Host.View?.Composite is SKBitmap composite)
        {
            return ImageOps.SampleAverage(composite, p.X, p.Y, SampleSize);
        }
        using SKBitmap sample = Host.GetSampleImage(AllLayers);
        return ImageOps.SampleAverage(sample, p.X, p.Y, SampleSize);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging || !ShowRing || _preview is not SKColor c) return;
        Point center = context.ToScreen(_position);
        Color current = Color.FromArgb(255, c.Red, c.Green, c.Blue);
        SKColor previous = _background ? Host.Background : Host.Foreground;
        context.Drawing.DrawEllipse(null, new Pen(new SolidColorBrush(Color.FromRgb(128, 128, 128)), 22), center, 46, 46);
        context.Drawing.DrawGeometry(null, new Pen(new SolidColorBrush(current), 18), new ArcGeometry(center, 46, 180, 360));
        context.Drawing.DrawGeometry(null, new Pen(new SolidColorBrush(Color.FromRgb(previous.Red, previous.Green, previous.Blue)), 18), new ArcGeometry(center, 46, 0, 180));
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo(Strings.Tools_SampleSizeColon, [(Strings.Tools_PointSample, 1), (string.Format(Strings.Tools_SampleAverage, 3), 3), (string.Format(Strings.Tools_SampleAverage, 5), 5), (string.Format(Strings.Tools_SampleAverage, 11), 11), (string.Format(Strings.Tools_SampleAverage, 31), 31), (string.Format(Strings.Tools_SampleAverage, 51), 51), (string.Format(Strings.Tools_SampleAverage, 101), 101)],
            () => SampleSize, v => SampleSize = v, 140);
        options.Combo(Strings.SamplingTools_SampleColon, [(Strings.SamplingTools_CurrentLayer, false), (Strings.EditorCommands_AllLayers, true)], () => AllLayers, v => AllLayers = v, 130);
        options.Check(Strings.SamplingTools_ShowSamplingRing, () => ShowRing, v => ShowRing = v);
    }

    /// <summary>Simple arc used for the sampling ring.</summary>
    private sealed class ArcGeometry : StreamGeometry
    {
        public ArcGeometry(Point center, double radius, double startDegrees, double endDegrees)
        {
            using StreamGeometryContext ctx = Open();
            const int steps = 40;
            for (int i = 0; i <= steps; i++)
            {
                double a = (startDegrees + (endDegrees - startDegrees) * i / steps) * Math.PI / 180;
                Point p = new(center.X + Math.Cos(a) * radius, center.Y + Math.Sin(a) * radius);
                if (i == 0) ctx.BeginFigure(p, false);
                else ctx.LineTo(p);
            }
            ctx.EndFigure(false);
        }
    }
}

public sealed class ColorSamplerTool : ToolBase
{
    private ColorSamplerPoint? _dragging;

    public ColorSamplerTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.ColorSampler;

    public override string Name => Strings.SamplingTools_ColorSamplerTool;

    public override string Hint => Strings.SamplingTools_ColorSamplerHint;

    public int SampleSize { get; set; } = 1;

    private ColorSamplerPoint? Hit(SKPoint p) =>
        Doc?.Samplers.FirstOrDefault(s => SKPoint.Distance(s.Position, p) * Host.View!.Zoom < 8);

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        ColorSamplerPoint? hit = Hit(input.Doc);
        if (hit != null && input.Alt)
        {
            Doc.Samplers.Remove(hit);
            PushHistory(Strings.SamplingTools_DeleteColorSampler);
            Host.InvalidateOverlay();
            return;
        }
        if (hit != null)
        {
            _dragging = hit;
            return;
        }
        if (Doc.Samplers.Count >= 10)
        {
            Host.ShowMessage(Strings.SamplingTools_MaxSamplers);
            return;
        }
        Doc.Samplers.Add(new ColorSamplerPoint { Position = new SKPoint(MathF.Floor(input.Doc.X) + 0.5f, MathF.Floor(input.Doc.Y) + 0.5f) });
        PushHistory(Strings.SamplingTools_ColorSampler);
        Host.InvalidateOverlay();
    }

    public override void PointerMove(ToolInput input)
    {
        if (_dragging == null) return;
        _dragging.Position = new SKPoint(MathF.Floor(input.Doc.X) + 0.5f, MathF.Floor(input.Doc.Y) + 0.5f);
        Host.InvalidateOverlay();
        Host.RefreshPanels();
    }

    public override void PointerUp(ToolInput input)
    {
        if (_dragging == null) return;
        _dragging = null;
        PushHistory(Strings.SamplingTools_MoveColorSampler);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo(Strings.Tools_SampleSizeColon, [(Strings.Tools_PointSample, 1), (string.Format(Strings.Tools_SampleAverage, 3), 3), (string.Format(Strings.Tools_SampleAverage, 5), 5)], () => SampleSize, v => SampleSize = v, 130);
        options.Button(Strings.AnnotationTools_ClearAll, () =>
        {
            if (Doc == null || Doc.Samplers.Count == 0) return;
            Doc.Samplers.Clear();
            PushHistory(Strings.SamplingTools_ClearColorSamplers);
            Host.InvalidateOverlay();
        });
    }
}
