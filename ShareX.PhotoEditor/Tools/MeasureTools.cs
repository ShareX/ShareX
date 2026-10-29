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
using Avalonia.Input;
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

    public override string Name => "Eyedropper Tool";

    public override string Hint => "Click to sample the foreground color. Alt-click samples the background color.";

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
        options.Combo("Sample Size:", [("Point Sample", 1), ("3 by 3 Average", 3), ("5 by 5 Average", 5), ("11 by 11 Average", 11), ("31 by 31 Average", 31), ("51 by 51 Average", 51), ("101 by 101 Average", 101)],
            () => SampleSize, v => SampleSize = v, 140);
        options.Combo("Sample:", [("Current Layer", false), ("All Layers", true)], () => AllLayers, v => AllLayers = v, 110);
        options.Check("Show Sampling Ring", () => ShowRing, v => ShowRing = v);
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

    public override string Name => "Color Sampler Tool";

    public override string Hint => "Click to place up to 10 color samplers (shown in the Info panel). Drag to move, Alt-click to delete.";

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
            PushHistory("Delete Color Sampler");
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
            Host.ShowMessage("A maximum of 10 color samplers is allowed.");
            return;
        }
        Doc.Samplers.Add(new ColorSamplerPoint { Position = new SKPoint(MathF.Floor(input.Doc.X) + 0.5f, MathF.Floor(input.Doc.Y) + 0.5f) });
        PushHistory("Color Sampler");
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
        PushHistory("Move Color Sampler");
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo("Sample Size:", [("Point Sample", 1), ("3 by 3 Average", 3), ("5 by 5 Average", 5)], () => SampleSize, v => SampleSize = v, 130);
        options.Button("Clear All", () =>
        {
            if (Doc == null || Doc.Samplers.Count == 0) return;
            Doc.Samplers.Clear();
            PushHistory("Clear Color Samplers");
            Host.InvalidateOverlay();
        });
    }
}

/// <summary>Measures distance and angle; can straighten the active layer along the measured line.</summary>
public sealed class RulerTool : ToolBase
{
    private SKPoint? _a;
    private SKPoint? _b;
    private int _dragging;

    public RulerTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Ruler;

    public override string Name => "Ruler Tool";

    public override string Hint => "Drag to measure distance and angle. Drag an end point to adjust it.";

    public bool UseMeasurementScale { get; set; }

    public float Angle => _a is SKPoint a && _b is SKPoint b ? -MathF.Atan2(b.Y - a.Y, b.X - a.X) * 180 / MathF.PI : 0;

    public float Length => _a is SKPoint a && _b is SKPoint b ? SKPoint.Distance(a, b) : 0;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        float tolerance = 8 / (float)Host.View!.Zoom;
        if (_a is SKPoint a && SKPoint.Distance(a, input.Doc) < tolerance) _dragging = 1;
        else if (_b is SKPoint b && SKPoint.Distance(b, input.Doc) < tolerance) _dragging = 2;
        else
        {
            _a = _b = input.Doc;
            _dragging = 2;
        }
    }

    public override void PointerMove(ToolInput input)
    {
        if (_dragging == 0) return;
        SKPoint other = _dragging == 1 ? _b!.Value : _a!.Value;
        SKPoint p = input.Shift ? Constrain45(other, input.Doc) : input.Doc;
        if (_dragging == 1) _a = p;
        else _b = p;
        Host.RefreshOptions();
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input) => _dragging = 0;

    public override void DrawOverlay(OverlayContext context)
    {
        if (_a is not SKPoint a || _b is not SKPoint b) return;
        context.Line(a, b, new Pen(Brushes.White, 1));
        context.Handle(a, false, 6);
        context.Handle(b, false, 6);
        context.Text(b, $"{Length:0.0} px  {Angle:0.0}°");
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        SKPoint a = _a ?? default, b = _b ?? default;
        options.Label($"X: {a.X:0.0}   Y: {a.Y:0.0}   W: {b.X - a.X:0.0}   H: {b.Y - a.Y:0.0}   A: {Angle:0.0}°   L1: {Length:0.0}");
        options.Separator();
        options.Check("Use Measurement Scale", () => UseMeasurementScale, v => UseMeasurementScale = v);
        options.Button("Straighten Layer", Straighten, "Rotate the active layer so that the ruler line becomes level");
        options.Button("Clear", () =>
        {
            _a = _b = null;
            Host.RefreshOptions();
            Host.InvalidateOverlay();
        });
    }

    private void Straighten()
    {
        if (Doc == null || _a == null || _b == null || Length < 1) return;
        float angle = Angle;
        // Rotate by the smallest amount that levels the line horizontally or vertically.
        float correction = angle;
        while (correction > 45) correction -= 90;
        while (correction < -45) correction += 90;
        UI.Commands.EditorCommands.RotateCanvasOrLayer(Host, correction, Doc.ActiveLayer is RasterLayer { IsBackground: true });
        _a = _b = null;
        Host.RefreshOptions();
    }
}

public sealed class NoteTool : ToolBase
{
    private NoteItem? _dragging;
    private SKPoint _dragOffset;
    private bool _moved;

    public NoteTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Note;

    public override string Name => "Note Tool";

    public override string Hint => "Click to add a note. Click a note to edit it, drag to move it, Alt-click to delete it.";

    public override ToolCursor Cursor => ToolCursor.Arrow;

    public string Author { get; set; } = Environment.UserName;

    public SKColor Color { get; set; } = new(0xFF, 0xD7, 0x40);

    private NoteItem? Hit(SKPoint p) => Doc?.Notes.LastOrDefault(n =>
    {
        SKPoint d = p - n.Position;
        float size = 18 / (float)Host.View!.Zoom;
        return d.X >= 0 && d.Y >= 0 && d.X <= size && d.Y <= size;
    });

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        NoteItem? hit = Hit(input.Doc);
        if (hit != null && input.Alt)
        {
            Doc.Notes.Remove(hit);
            PushHistory("Delete Note");
            Host.InvalidateOverlay();
            return;
        }
        if (hit != null)
        {
            _dragging = hit;
            _dragOffset = input.Doc - hit.Position;
            _moved = false;
            return;
        }
        _ = AddNoteAsync(input.Doc);
    }

    private async Task AddNoteAsync(SKPoint position)
    {
        string? text = await Host.PromptTextAsync("Note", $"Author: {Author}", string.Empty);
        if (text == null || Doc == null) return;
        Doc.Notes.Add(new NoteItem { Position = position, Text = text, Author = Author, Color = Color });
        PushHistory("Note");
        Host.InvalidateOverlay();
    }

    public override void PointerMove(ToolInput input)
    {
        if (_dragging == null) return;
        _dragging.Position = input.Doc - _dragOffset;
        _moved = true;
        Host.InvalidateOverlay();
    }

    public override async void PointerUp(ToolInput input)
    {
        NoteItem? note = _dragging;
        _dragging = null;
        if (note == null) return;
        if (_moved)
        {
            PushHistory("Move Note");
            return;
        }
        string? text = await Host.PromptTextAsync("Note", $"Author: {note.Author}", note.Text);
        if (text == null) return;
        note.Text = text;
        PushHistory("Edit Note");
        Host.InvalidateOverlay();
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Text("Author:", () => Author, v => Author = v, 140);
        options.ColorButton("Note color", () => Color, c => Color = c);
        options.Button("Clear All", () =>
        {
            if (Doc == null || Doc.Notes.Count == 0) return;
            Doc.Notes.Clear();
            PushHistory("Clear Notes");
            Host.InvalidateOverlay();
        });
    }
}

public sealed class CountTool : ToolBase
{
    private CountItem? _dragging;

    public CountTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Count;

    public override string Name => "Count Tool";

    public override string Hint => "Click to place numbered count markers. Drag to move, Alt-click to remove.";

    private CountItem? Hit(SKPoint p) => Doc?.Counts.LastOrDefault(c => SKPoint.Distance(c.Position, p) * Host.View!.Zoom < 8);

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        CountItem? hit = Hit(input.Doc);
        if (hit != null && input.Alt)
        {
            Doc.Counts.Remove(hit);
            PushHistory("Delete Count");
            Host.RefreshOptions();
            Host.InvalidateOverlay();
            return;
        }
        if (hit != null)
        {
            _dragging = hit;
            return;
        }
        Doc.Counts.Add(new CountItem { Position = input.Doc, Group = Doc.ActiveCountGroup });
        PushHistory("Count");
        Host.RefreshOptions();
        Host.InvalidateOverlay();
    }

    public override void PointerMove(ToolInput input)
    {
        if (_dragging == null) return;
        _dragging.Position = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (_dragging == null) return;
        _dragging = null;
        PushHistory("Move Count");
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        if (Doc == null) return;
        CountGroup group = Doc.CountGroups[Math.Clamp(Doc.ActiveCountGroup, 0, Doc.CountGroups.Count - 1)];
        options.Label($"Count: {Doc.Counts.Count(c => c.Group == Doc.ActiveCountGroup)}");
        options.Combo("Group:", Doc.CountGroups.Select((g, i) => (g.Name, i)), () => Doc.ActiveCountGroup, v => { Doc.ActiveCountGroup = v; Host.RefreshOptions(); }, 140);
        options.Button("+", () =>
        {
            Doc.CountGroups.Add(new CountGroup { Name = $"Count Group {Doc.CountGroups.Count + 1}", Color = ColorMath.FromHsv(Doc.CountGroups.Count * 67 % 360, 0.8f, 0.95f) });
            Doc.ActiveCountGroup = Doc.CountGroups.Count - 1;
            Host.RefreshOptions();
        }, "Create a new count group");
        options.ColorButton("Group color", () => group.Color, c => { group.Color = c; Host.InvalidateOverlay(); });
        options.Number("Marker Size:", 1, 10, () => group.MarkerSize, v => { group.MarkerSize = (float)v; Host.InvalidateOverlay(); }, "0", "px");
        options.Number("Label Size:", 8, 72, () => group.LabelSize, v => { group.LabelSize = (float)v; Host.InvalidateOverlay(); }, "0", "pt");
        options.Button("Clear", () =>
        {
            Doc.Counts.RemoveAll(c => c.Group == Doc.ActiveCountGroup);
            PushHistory("Clear Count");
            Host.RefreshOptions();
            Host.InvalidateOverlay();
        });
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers) => base.KeyDown(key, modifiers);
}
