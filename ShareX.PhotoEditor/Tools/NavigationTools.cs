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
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

public sealed class HandTool : ToolBase
{
    private Point? _last;

    public HandTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Hand;

    public override string Name => "Hand Tool";

    public override string Hint => "Drag to scroll the view. Hold Space with any tool to use the Hand tool temporarily. Double-click to fit on screen.";

    public override ToolCursor Cursor => _last != null ? ToolCursor.Grab : ToolCursor.Hand;

    public override void PointerDown(ToolInput input)
    {
        if (input.ClickCount >= 2)
        {
            Host.View?.FitOnScreen();
            Host.InvalidateOverlay();
            return;
        }
        _last = input.Screen;
    }

    public override void PointerMove(ToolInput input)
    {
        if (_last is not Point last || Host.View == null) return;
        Host.View.Pan += input.Screen - last;
        _last = input.Screen;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input) => _last = null;

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Button("100%", () => { Host.View?.ActualPixels(); Host.InvalidateOverlay(); });
        options.Button("Fit Screen", () => { Host.View?.FitOnScreen(); Host.InvalidateOverlay(); });
        options.Button("Fill Screen", () => { Host.View?.FillScreen(); Host.InvalidateOverlay(); });
    }
}

public sealed class RotateViewTool : ToolBase
{
    private double _startAngle;
    private double _startRotation;
    private bool _dragging;

    public RotateViewTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.RotateView;

    public override string Name => "Rotate View Tool";

    public override string Hint => "Drag to rotate the canvas view without changing pixels. Shift snaps to 15 degrees. Esc or Reset View restores it.";

    public override ToolCursor Cursor => ToolCursor.Rotate;

    private double AngleOf(Point p)
    {
        Size viewport = Host.View!.Viewport;
        Vector center = new Vector(viewport.Width / 2, viewport.Height / 2) + Host.View.Pan;
        return Math.Atan2(p.Y - center.Y, p.X - center.X) * 180 / Math.PI;
    }

    public override void PointerDown(ToolInput input)
    {
        if (Host.View == null) return;
        _dragging = true;
        _startAngle = AngleOf(input.Screen);
        _startRotation = Host.View.Rotation;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging || Host.View == null) return;
        double rotation = _startRotation + AngleOf(input.Screen) - _startAngle;
        if (input.Shift) rotation = Math.Round(rotation / 15) * 15;
        Host.View.Rotation = ((rotation % 360) + 360) % 360;
        Host.InvalidateOverlay();
        Host.RefreshOptions();
    }

    public override void PointerUp(ToolInput input) => _dragging = false;

    public override bool KeyDown(Avalonia.Input.Key key, Avalonia.Input.KeyModifiers modifiers)
    {
        if (key == Avalonia.Input.Key.Escape && Host.View != null)
        {
            Host.View.Rotation = 0;
            Host.InvalidateOverlay();
            Host.RefreshOptions();
            return true;
        }
        return base.KeyDown(key, modifiers);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Number("Rotation Angle:", -360, 360, () => Host.View?.Rotation ?? 0, v =>
        {
            if (Host.View == null) return;
            Host.View.Rotation = v;
            Host.InvalidateOverlay();
        }, "0", "°");
        options.Button("Reset View", () =>
        {
            if (Host.View == null) return;
            Host.View.Rotation = 0;
            Host.InvalidateOverlay();
            Host.RefreshOptions();
        });
    }
}

public sealed class ZoomTool : ToolBase
{
    private Point _start;
    private Point _current;
    private bool _dragging;
    private bool _zoomOut;
    private double _startZoom;
    private Vector _startPan;

    public ZoomTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Zoom;

    public override string Name => "Zoom Tool";

    public override string Hint => "Click to zoom in, Alt-click to zoom out. Drag left/right for scrubby zoom, or drag a rectangle when scrubby zoom is off.";

    public override ToolCursor Cursor => _zoomOut ? ToolCursor.ZoomOut : ToolCursor.ZoomIn;

    public bool ScrubbyZoom { get; set; } = true;

    public bool ZoomOutDefault { get; set; }

    public override void PointerDown(ToolInput input)
    {
        if (Host.View == null) return;
        _zoomOut = input.Alt != ZoomOutDefault;
        _start = _current = input.Screen;
        _startZoom = Host.View.Zoom;
        _startPan = Host.View.Pan;
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        _zoomOut = input.Alt != ZoomOutDefault;
        if (!_dragging || Host.View == null) return;
        _current = input.Screen;
        if (ScrubbyZoom)
        {
            double dx = _current.X - _start.X;
            // Always zoom from the state at the press so the point under the pointer stays fixed.
            Host.View.Zoom = _startZoom;
            Host.View.Pan = _startPan;
            Host.View.ZoomAt(_startZoom * Math.Pow(1.01, dx), _start);
            Host.RefreshPanels();
        }
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_dragging || Host.View == null) return;
        _dragging = false;
        Vector delta = _current - _start;
        if (Math.Abs(delta.X) < 3 && Math.Abs(delta.Y) < 3)
        {
            Host.View.Zoom = _startZoom;
            Host.View.Pan = _startPan;
            Host.View.ZoomStep(!_zoomOut, input.Screen);
        }
        else if (!ScrubbyZoom)
        {
            // Zoom to the dragged rectangle.
            SKPoint a = Host.View.ScreenToDoc(_start), b = Host.View.ScreenToDoc(_current);
            SKRect rect = DragRect(a, b, false, false);
            double zoom = Math.Min(Host.View.Viewport.Width / Math.Max(1, rect.Width), Host.View.Viewport.Height / Math.Max(1, rect.Height));
            Host.View.Zoom = Math.Clamp(zoom, 0.01, 64);
            Point center = Host.View.DocToScreen(new SKPoint(rect.MidX, rect.MidY));
            Host.View.Pan += new Point(Host.View.Viewport.Width / 2, Host.View.Viewport.Height / 2) - center;
        }
        Host.InvalidateOverlay();
        Host.RefreshPanels();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging || ScrubbyZoom || Host.View == null) return;
        context.Rect(DragRect(Host.View.ScreenToDoc(_start), Host.View.ScreenToDoc(_current), false, false));
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.IconButton("zoom-in", "Zoom in", () => { Host.View?.ZoomStep(true, Center()); Host.InvalidateOverlay(); });
        options.IconButton("zoom-out", "Zoom out", () => { Host.View?.ZoomStep(false, Center()); Host.InvalidateOverlay(); });
        options.Check("Scrubby Zoom", () => ScrubbyZoom, v => ScrubbyZoom = v);
        options.Button("100%", () => { Host.View?.ActualPixels(); Host.InvalidateOverlay(); });
        options.Button("Fit Screen", () => { Host.View?.FitOnScreen(); Host.InvalidateOverlay(); });
        options.Button("Fill Screen", () => { Host.View?.FillScreen(); Host.InvalidateOverlay(); });
    }

    private Point Center() => Host.View == null ? default : new Point(Host.View.Viewport.Width / 2, Host.View.Viewport.Height / 2);
}
