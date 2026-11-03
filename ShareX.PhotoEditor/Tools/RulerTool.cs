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

using Avalonia.Media;
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

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
