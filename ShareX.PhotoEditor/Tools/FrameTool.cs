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

public sealed class FrameTool : ToolBase
{
    private SKPoint _start;
    private SKPoint _current;
    private bool _dragging;

    public FrameTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Frame;

    public override string Name => "Frame Tool";

    public override string Hint => "Drag to create a frame placeholder. Place or paste an image to fill the active frame.";

    public bool Ellipse { get; set; }

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _start = _current = input.Doc;
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        _current = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_dragging || Doc == null) return;
        _dragging = false;
        SKRect rect = DragRect(_start, input.Doc, input.Shift, input.Alt);
        Host.InvalidateOverlay();
        if (rect.Width < 4 || rect.Height < 4)
        {
            FrameLayer? hit = Doc.AllLayers.OfType<FrameLayer>().LastOrDefault(f => f.Frame.Contains(input.Doc));
            if (hit != null)
            {
                Doc.ActiveLayer = hit;
                Host.RefreshPanels();
            }
            return;
        }
        FrameLayer frame = new($"Frame {Doc.AllLayers.OfType<FrameLayer>().Count() + 1}", rect, Ellipse);
        Layer? active = Doc.ActiveLayer;
        if (active is RasterLayer raster && !raster.IsBackground && input.Ctrl)
        {
            // Ctrl-drag converts the active layer into the frame's content.
            SKRectI bounds = raster.GetOpaqueBounds();
            if (!bounds.IsEmpty)
            {
                frame.Content = new PixelSurface(ImageOps.Crop(raster.Surface.Bitmap, SKRectI.Create(bounds.Left - raster.Offset.X, bounds.Top - raster.Offset.Y, bounds.Width, bounds.Height)));
                frame.ContentRect = bounds;
                Doc.RemoveLayer(raster);
            }
        }
        Doc.AddLayer(frame);
        PushHistory("Frame");
        Host.InvalidateCanvas();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging) return;
        SKRect rect = DragRect(_start, _current, false, false);
        if (Ellipse) context.Ellipse(rect, OverlayContext.AccentPen);
        else context.Rect(rect, OverlayContext.AccentPen);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.IconButton("frame-rect", "Rectangular frame", () => { Ellipse = false; Host.RefreshOptions(); }, !Ellipse);
        options.IconButton("frame-ellipse", "Elliptical frame", () => { Ellipse = true; Host.RefreshOptions(); }, Ellipse);
        options.Separator();
        options.Button("Place Image...", () => UI.Commands.EditorCommands.PlaceIntoFrame(Host), "Place an image file into the active frame");
        options.Button("Clear Frame Content", () =>
        {
            if (Doc?.ActiveLayer is FrameLayer frame && frame.Content != null)
            {
                frame.Content = null;
                PushHistory("Clear Frame");
                Host.InvalidateCanvas();
            }
        });
    }
}
