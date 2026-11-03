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

namespace ShareX.PhotoEditor.Core;

/// <summary>A placeholder frame that crops placed content to a rectangle or ellipse.</summary>
public sealed class FrameLayer : Layer
{
    public FrameLayer(string name, SKRect frame, bool ellipse) : base(name)
    {
        Frame = frame;
        Ellipse = ellipse;
    }

    public override LayerKind Kind => LayerKind.Frame;

    public SKRect Frame { get; set; }

    public bool Ellipse { get; set; }

    public PixelSurface? Content { get; set; }

    /// <summary>Content placement relative to the frame, in document coordinates.</summary>
    public SKRect ContentRect { get; set; }

    public SKColor StrokeColor { get; set; } = SKColors.Transparent;

    public float StrokeWidth { get; set; }

    public override IEnumerable<PixelSurface> Surfaces => Content != null ? base.Surfaces.Append(Content) : base.Surfaces;

    protected override Layer CreateShallowCopy() => new FrameLayer(Name, Frame, Ellipse)
    {
        Content = Content,
        ContentRect = ContentRect,
        StrokeColor = StrokeColor,
        StrokeWidth = StrokeWidth
    };

    protected override void DetachSurfaces()
    {
        base.DetachSurfaces();
        Content = Content?.Clone();
    }

    public override void Translate(float dx, float dy)
    {
        SKRect frame = Frame;
        frame.Offset(dx, dy);
        Frame = frame;
        SKRect content = ContentRect;
        content.Offset(dx, dy);
        ContentRect = content;
    }

    public void PlaceContent(PixelSurface content)
    {
        Content = content;
        float scale = Math.Max(Frame.Width / content.Width, Frame.Height / content.Height);
        float w = content.Width * scale, h = content.Height * scale;
        ContentRect = SKRect.Create(Frame.MidX - w / 2, Frame.MidY - h / 2, w, h);
    }

    public SKPath GetFramePath()
    {
        SKPath path = new();
        if (Ellipse) path.AddOval(Frame);
        else path.AddRect(Frame);
        return path;
    }

    public override SKRect GetContentBounds() => Frame;

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        using SKPath path = GetFramePath();
        canvas.Save();
        canvas.ClipPath(path, SKClipOperation.Intersect, true);
        if (Content != null)
        {
            using SKImage image = SKImage.FromBitmap(Content.Bitmap);
            canvas.DrawImage(image, SKRect.Create(0, 0, Content.Width, Content.Height), ContentRect, context.Sampling);
        }
        canvas.Restore();
        if (StrokeWidth > 0 && StrokeColor.Alpha > 0)
        {
            using SKPaint stroke = new() { Color = StrokeColor, StrokeWidth = StrokeWidth, Style = SKPaintStyle.Stroke, IsAntialias = true };
            canvas.DrawPath(path, stroke);
        }
    }
}
