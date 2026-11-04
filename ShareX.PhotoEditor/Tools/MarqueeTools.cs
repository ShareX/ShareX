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

public sealed class MarqueeTool : SelectionToolBase
{
    private readonly bool _ellipse;
    private SKPoint _start;
    private SKPoint _current;
    private bool _dragging;
    private SelectionMode _mode;
    private bool _square;
    private bool _fromCenter;

    public MarqueeTool(IEditorHost host, bool ellipse) : base(host)
    {
        _ellipse = ellipse;
    }

    public override ToolId Id => _ellipse ? ToolId.EllipticalMarquee : ToolId.RectangularMarquee;

    public override string Name => _ellipse ? Strings.MarqueeTools_EllipticalMarqueeTool : Strings.MarqueeTools_RectangularMarqueeTool;

    public override string Hint => Strings.MarqueeTools_Hint;

    public MarqueeStyle Style { get; set; }

    public float FixedWidth { get; set; } = 64;

    public float FixedHeight { get; set; } = 64;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        if (TryBeginMove(input)) return;
        _mode = ResolveMode(input);
        _start = _current = input.Doc;
        _dragging = true;
        if (Style == MarqueeStyle.FixedSize) _current = _start + new SKPoint(FixedWidth, FixedHeight);
    }

    public override void PointerMove(ToolInput input)
    {
        if (IsMovingSelection)
        {
            UpdateMove(input);
            return;
        }
        if (!_dragging) return;
        _square = input.Shift && _mode != SelectionMode.Add || Style == MarqueeStyle.FixedRatio;
        _fromCenter = input.Alt && _mode != SelectionMode.Subtract;
        _current = Style == MarqueeStyle.FixedSize ? input.Doc : input.Doc;
        Host.InvalidateOverlay();
        SKRect r = GetRect();
        Host.SetStatus(string.Format(Strings.Tools_SizeInfo, r.Width, r.Height));
    }

    private SKRect GetRect()
    {
        if (Style == MarqueeStyle.FixedSize)
        {
            return _fromCenter ? SKRect.Create(_current.X - FixedWidth / 2, _current.Y - FixedHeight / 2, FixedWidth, FixedHeight) : SKRect.Create(_current.X, _current.Y, FixedWidth, FixedHeight);
        }
        SKPoint end = _current;
        if (Style == MarqueeStyle.FixedRatio && FixedHeight > 0)
        {
            float ratio = FixedWidth / FixedHeight;
            float dx = end.X - _start.X, dy = end.Y - _start.Y;
            float h = MathF.Max(MathF.Abs(dy), MathF.Abs(dx) / ratio);
            end = new SKPoint(_start.X + MathF.Sign(dx == 0 ? 1 : dx) * h * ratio, _start.Y + MathF.Sign(dy == 0 ? 1 : dy) * h);
            return DragRect(_start, end, false, _fromCenter);
        }
        SKRect rect = DragRect(_start, end, _square, _fromCenter);
        return _ellipse ? rect : new SKRect(MathF.Round(rect.Left), MathF.Round(rect.Top), MathF.Round(rect.Right), MathF.Round(rect.Bottom));
    }

    public override void PointerUp(ToolInput input)
    {
        if (IsMovingSelection)
        {
            EndMove();
            return;
        }
        if (!_dragging || Doc == null) return;
        _dragging = false;
        SKRect rect = GetRect();
        Host.InvalidateOverlay();
        if (rect.Width < 1 || rect.Height < 1)
        {
            DeselectOnClick(_mode);
            return;
        }
        using SKPath path = new();
        if (_ellipse) path.AddOval(rect);
        else path.AddRect(rect);
        bool aa = AntiAlias;
        if (!_ellipse) AntiAlias = false;
        ApplyPath(path, _mode, _ellipse ? Strings.MarqueeTools_EllipticalMarquee : Strings.MarqueeTools_RectangularMarquee);
        AntiAlias = aa;
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging) return;
        SKRect rect = GetRect();
        if (_ellipse) context.Ellipse(rect);
        else context.Rect(rect);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        BuildCommonOptions(options, true, _ellipse);
        options.Separator();
        options.EnumCombo(Strings.MarqueeTools_StyleColon, () => Style, v => { Style = v; Host.RefreshOptions(); }, 110);
        bool fixedStyle = Style != MarqueeStyle.Normal;
        options.Number(Strings.CropTools_WColon, 1, 30000, () => FixedWidth, v => FixedWidth = (float)v, "0", null, 1, 64).IsEnabled = fixedStyle;
        options.Number(Strings.CropTools_HColon, 1, 30000, () => FixedHeight, v => FixedHeight = (float)v, "0", null, 1, 64).IsEnabled = fixedStyle;
        options.Separator();
        options.Button(Strings.EditorCommands_SelectAndMaskEllipsis, () => UI.Commands.EditorCommands.SelectAndMask(Host), Strings.MarqueeTools_RefineEdgeTip);
    }
}

/// <summary>Single row or single column marquee: selects a 1 pixel line across the document.</summary>
public sealed class SingleLineMarqueeTool : SelectionToolBase
{
    private readonly bool _row;
    private SKPoint? _hover;

    public SingleLineMarqueeTool(IEditorHost host, bool row) : base(host)
    {
        _row = row;
    }

    public override ToolId Id => _row ? ToolId.SingleRowMarquee : ToolId.SingleColumnMarquee;

    public override string Name => _row ? Strings.MarqueeTools_SingleRowMarqueeTool : Strings.MarqueeTools_SingleColumnMarqueeTool;

    public override string Hint => _row ? Strings.MarqueeTools_SingleRowHint : Strings.MarqueeTools_SingleColumnHint;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        SKPointI p = input.DocPixel;
        SKRectI rect = _row ? new SKRectI(0, p.Y, Doc.Width, p.Y + 1) : new SKRectI(p.X, 0, p.X + 1, Doc.Height);
        ApplySelection(MaskOps.FromRect(rect, Doc.Width, Doc.Height), ResolveMode(input), Name.Replace(" Tool", string.Empty));
    }

    public override void PointerMove(ToolInput input)
    {
        _hover = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_hover is not SKPoint p || Doc == null) return;
        if (_row) context.Line(new SKPoint(0, MathF.Floor(p.Y) + 0.5f), new SKPoint(Doc.Width, MathF.Floor(p.Y) + 0.5f));
        else context.Line(new SKPoint(MathF.Floor(p.X) + 0.5f, 0), new SKPoint(MathF.Floor(p.X) + 0.5f, Doc.Height));
    }

    public override void BuildOptions(OptionsBuilder options) => BuildCommonOptions(options, false, false);
}
