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

using SkiaSharp;
using System.Drawing;

namespace ShareX.Tools;

// Read on the UI thread; the state references keep the backend's existing ownership.
internal readonly record struct MouseHighlighterFrame(MouseHighlighterOptions Options, Point CursorPosition,
    IReadOnlyList<MouseHighlight> Highlights, double Time);

internal static class MouseHighlighterRenderer
{
    public static Rectangle GetEffectBounds(Rectangle screenBounds, MouseHighlighterFrame frame)
    {
        MouseHighlighterOptions options = frame.Options;
        Rectangle bounds = Rectangle.Empty;
        if (options.Mode == MouseHighlightMode.Circle && options.AlwaysColor.A > 0)
        {
            bounds = Rectangle.Intersect(Around(frame.CursorPosition, options.Radius + 2), screenBounds);
        }
        foreach (MouseHighlight highlight in frame.Highlights)
        {
            Color color = options.GetColor(highlight.Button);
            if (color.A == 0) continue;
            int radius = options.Mode == MouseHighlightMode.Ripple ? options.RippleSize / 2 + 12 : options.Radius + 2;
            Rectangle effect = Rectangle.Intersect(Around(highlight.Position, radius), screenBounds);
            if (effect.Width > 0 && effect.Height > 0)
            {
                bounds = bounds.Width <= 0 || bounds.Height <= 0 ? effect : Rectangle.Union(bounds, effect);
            }
        }
        return Rectangle.Intersect(bounds, screenBounds);
    }

    private static Rectangle Around(System.Drawing.Point center, int radius) =>
        new(center.X - radius, center.Y - radius, radius * 2 + 1, radius * 2 + 1);

    public static void Draw(SKCanvas canvas, MouseHighlighterFrame frame)
    {
        MouseHighlighterOptions options = frame.Options;
        double time = frame.Time;
        SKPoint cursor = new(frame.CursorPosition.X, frame.CursorPosition.Y);
        if (options.Mode == MouseHighlightMode.Circle)
        {
            using SKPaint always = Paint(options.AlwaysColor);
            canvas.DrawCircle(cursor, options.Radius, always);
        }
        foreach (MouseHighlight highlight in frame.Highlights)
        {
            Color color = options.GetColor(highlight.Button);
            SKPoint center = new(highlight.Position.X, highlight.Position.Y);
            if (options.Mode == MouseHighlightMode.Circle)
            {
                using SKPaint fill = Paint(color, FadeOpacity(highlight, options, time));
                canvas.DrawCircle(center, options.Radius, fill);
            }
            else
            {
                double progress = Math.Clamp((time - highlight.Started) / options.RippleDuration, 0, 1);
                double fade = highlight.Released.HasValue
                    ? 1 - Math.Clamp((time - highlight.Released.Value) / options.RippleDuration, 0, 1) : 1;
                float radius = (float)(options.RippleSize / 2d * (0.35 + 0.65 * progress));
                using SKPaint ring = Paint(color, fade * options.RippleIntensity);
                ring.Style = SKPaintStyle.Stroke;
                ring.StrokeWidth = (float)(3 * options.RippleIntensity);
                if (highlight.Crosshairs)
                {
                    float gap = radius * 0.4f;
                    canvas.DrawLine(center.X - radius, center.Y, center.X - gap, center.Y, ring);
                    canvas.DrawLine(center.X + gap, center.Y, center.X + radius, center.Y, ring);
                    canvas.DrawLine(center.X, center.Y - radius, center.X, center.Y - gap, ring);
                    canvas.DrawLine(center.X, center.Y + gap, center.X, center.Y + radius, ring);
                }
                else
                {
                    using SKPaint glow = Paint(color, fade * options.RippleIntensity * 0.3);
                    using SKMaskFilter blur = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 3);
                    glow.MaskFilter = blur;
                    canvas.DrawCircle(center, radius, glow);
                    canvas.DrawCircle(center, radius, ring);
                }
            }
        }
    }

    private static double FadeOpacity(MouseHighlight highlight, MouseHighlighterOptions options, double time)
    {
        if (!highlight.Released.HasValue) return 1;
        double age = time - highlight.Released.Value - options.FadeDelay;
        if (age < 0) return 1;
        return options.FadeDuration == 0 ? 0 : Math.Clamp(1 - age / options.FadeDuration, 0, 1);
    }

    private static SKPaint Paint(Color color, double opacity = 1) => new()
    {
        IsAntialias = true,
        Color = new SKColor(color.R, color.G, color.B, (byte)Math.Clamp(Math.Round(color.A * opacity), 0, 255))
    };

}
