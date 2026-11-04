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

public sealed class ArtboardTool : ToolBase
{
    private SKPoint _start;
    private SKPoint _current;
    private bool _creating;
    private GroupLayer? _moving;
    private SKPoint _applied;

    public ArtboardTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Artboard;

    public override string Name => "Artboard Tool";

    public override string Hint => "Drag on empty canvas to create an artboard. Drag an artboard's title to move it with its contents.";

    public SKColor BackgroundColor { get; set; } = SKColors.White;

    public SKSizeI PresetSize { get; set; }

    private GroupLayer? HitArtboardTitle(ToolInput input)
    {
        if (Doc == null || Host.View == null) return null;
        float titleHeight = 20 / (float)Host.View.Zoom;
        return Doc.AllLayers.OfType<GroupLayer>().LastOrDefault(g => g.ArtboardBounds is SKRectI b &&
            input.Doc.X >= b.Left && input.Doc.X <= b.Right && input.Doc.Y >= b.Top - titleHeight && input.Doc.Y <= b.Bottom);
    }

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _moving = HitArtboardTitle(input);
        _start = _current = input.Doc;
        _applied = default;
        if (_moving != null)
        {
            Doc.ActiveLayer = _moving;
            Host.RefreshPanels();
            return;
        }
        _creating = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (Doc == null) return;
        if (_moving != null)
        {
            SKPoint delta = new(MathF.Round(input.Doc.X - _start.X), MathF.Round(input.Doc.Y - _start.Y));
            SKPoint step = delta - _applied;
            _moving.Translate(step.X, step.Y);
            _applied = delta;
            Host.InvalidateCanvas();
            return;
        }
        if (!_creating) return;
        _current = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (Doc == null) return;
        if (_moving != null)
        {
            _moving = null;
            if (_applied != default) PushHistory("Move Artboard");
            return;
        }
        if (!_creating) return;
        _creating = false;
        SKRect r = DragRect(_start, _current, input.Shift, false);
        if (r.Width < 4 || r.Height < 4)
        {
            if (PresetSize.Width <= 0) return;
            r = SKRect.Create(_start.X, _start.Y, PresetSize.Width, PresetSize.Height);
        }
        CreateArtboard(SKRectI.Round(r));
    }

    public void CreateArtboard(SKRectI bounds)
    {
        if (Doc == null) return;
        int number = Doc.AllLayers.OfType<GroupLayer>().Count(g => g.IsArtboard) + 1;
        GroupLayer artboard = new($"Artboard {number}") { ArtboardBounds = bounds, ArtboardBackground = BackgroundColor, BlendMode = LayerBlendMode.PassThrough };
        Layer? active = Doc.ActiveLayer;
        Doc.Root.Add(artboard);
        if (active != null && active is not GroupLayer { IsArtboard: true } && !(active is RasterLayer { IsBackground: true }) && active.Parent == Doc.Root &&
            SKRect.Intersect(active.GetContentBounds(), SKRect.Create(bounds.Left, bounds.Top, bounds.Width, bounds.Height)) != SKRect.Empty)
        {
            artboard.Add(active);
        }
        Doc.ActiveLayer = artboard;
        Doc.NotifyStructureChanged();
        // Grow the canvas if the artboard extends beyond it.
        SKRectI all = DocumentTransforms.ContentBounds(Doc);
        if (all != Doc.Bounds && (all.Left < 0 || all.Top < 0 || all.Right > Doc.Width || all.Bottom > Doc.Height))
        {
            DocumentTransforms.ApplyMatrix(Doc, SKMatrix.CreateTranslation(-Math.Min(0, all.Left), -Math.Min(0, all.Top)),
                Math.Max(Doc.Width, all.Right) - Math.Min(0, all.Left), Math.Max(Doc.Height, all.Bottom) - Math.Min(0, all.Top),
                ResampleMethod.NearestNeighbor, Host.Background, true);
        }
        PushHistory("New Artboard");
        Host.InvalidateCanvas();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_creating) return;
        context.Rect(DragRect(_start, _current, false, false), OverlayContext.AccentPen);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo("Size:", [("Custom", new SKSizeI(0, 0)), ("1920 × 1080 (HD)", new SKSizeI(1920, 1080)), ("1280 × 720", new SKSizeI(1280, 720)),
            ("1080 × 1080 (Square)", new SKSizeI(1080, 1080)), ("390 × 844 (Phone)", new SKSizeI(390, 844)), ("820 × 1180 (Tablet)", new SKSizeI(820, 1180)), ("595 × 842 (A4)", new SKSizeI(595, 842))],
            () => PresetSize, v => PresetSize = v, 170);
        options.ColorButton("Artboard background", () => BackgroundColor, c => BackgroundColor = c);
        options.Button("Add New Artboard", () =>
        {
            if (Doc == null) return;
            SKSizeI size = PresetSize.Width > 0 ? PresetSize : new SKSizeI(Math.Min(1000, Doc.Width), Math.Min(1000, Doc.Height));
            int right = Doc.AllLayers.OfType<GroupLayer>().Where(g => g.IsArtboard).Select(g => g.ArtboardBounds!.Value.Right).DefaultIfEmpty(-100).Max();
            CreateArtboard(SKRectI.Create(right + 100, 0, size.Width, size.Height));
        });
    }
}
