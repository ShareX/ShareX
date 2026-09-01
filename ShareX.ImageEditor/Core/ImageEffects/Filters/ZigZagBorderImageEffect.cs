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

using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class ZigZagBorderImageEffect : ImageEffectBase
{
    public override string Id => "zigzag_border";
    public override string Name => "Zigzag border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.waves;
    public override string Description => "Adds a border with decorative jagged zigzag inner teeth that bite into the image edges.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<ZigZagBorderImageEffect>("border_size", "Border size", 8, 200, 36, (e, v) => e.BorderSize = v),
        EffectParameters.FloatSlider<ZigZagBorderImageEffect>("teeth_height", "Teeth height", 5, 80, 40, (e, v) => e.TeethHeight = v),
        EffectParameters.FloatSlider<ZigZagBorderImageEffect>("teeth_width", "Teeth width", 4, 80, 24, (e, v) => e.TeethWidth = v),
        EffectParameters.Color<ZigZagBorderImageEffect>("frame_color", "Frame color", new SKColor(40, 40, 50), (e, v) => e.FrameColor = v),
        EffectParameters.Color<ZigZagBorderImageEffect>("accent_color", "Accent color", new SKColor(220, 180, 60), (e, v) => e.AccentColor = v),
        EffectParameters.Bool<ZigZagBorderImageEffect>("accent_stripe", "Accent stripe", true, (e, v) => e.AccentStripe = v)
    ];

    public int BorderSize { get; set; } = 36;
    public float TeethHeight { get; set; } = 40f;
    public float TeethWidth { get; set; } = 24f;
    public SKColor FrameColor { get; set; } = new SKColor(40, 40, 50);
    public SKColor AccentColor { get; set; } = new SKColor(220, 180, 60);
    public bool AccentStripe { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 8, 200);
        float teethH = Math.Clamp(TeethHeight, 5f, 80f);
        float teethW = Math.Clamp(TeethWidth, 4f, 80f);

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        // Full canvas filled with frame color
        canvas.Clear(FrameColor);

        // Clip to zigzag inner shape and draw source image
        using SKPath innerPath = BuildZigZagInnerPath(newWidth, newHeight, border, teethW, teethH);
        canvas.Save();
        canvas.ClipPath(innerPath, SKClipOperation.Intersect, antialias: true);
        canvas.DrawBitmap(source, border, border);
        canvas.Restore();

        // Accent stripe along inner edge of frame
        if (AccentStripe)
        {
            float stripeW = Math.Max(2f, border * 0.1f);
            using SKPath accentPath = BuildZigZagInnerPath(newWidth, newHeight, border, teethW, teethH);
            using SKPaint accentPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = stripeW,
                Color = AccentColor
            };
            canvas.DrawPath(accentPath, accentPaint);
        }

        return result;
    }

    private static SKPath BuildZigZagInnerPath(int canvasW, int canvasH, int border, float teethW, float teethH)
    {
        var path = new SKPath();

        float imgL = border;
        float imgT = border;
        float imgR = canvasW - border;
        float imgB = canvasH - border;

        // We walk around the "inner image boundary" but with zigzag teeth pointing INTO the image
        // A tooth = going inward from the boundary line by teethH, then back
        // Start at top-left, walk clockwise

        // Top edge: left to right, teeth point downward (into image)
        float x = imgL;
        path.MoveTo(imgL, imgT);

        while (x < imgR)
        {
            float nextX = Math.Min(x + teethW, imgR);
            float midX = x + teethW * 0.5f;
            if (midX < imgR)
            {
                path.LineTo(midX, imgT + teethH);
                path.LineTo(nextX, imgT);
            }
            else
            {
                path.LineTo(nextX, imgT);
            }
            x = nextX;
        }

        // Right edge: top to bottom, teeth point leftward (into image)
        float y = imgT;
        while (y < imgB)
        {
            float nextY = Math.Min(y + teethW, imgB);
            float midY = y + teethW * 0.5f;
            if (midY < imgB)
            {
                path.LineTo(imgR - teethH, midY);
                path.LineTo(imgR, nextY);
            }
            else
            {
                path.LineTo(imgR, nextY);
            }
            y = nextY;
        }

        // Bottom edge: right to left, teeth point upward (into image)
        x = imgR;
        while (x > imgL)
        {
            float nextX = Math.Max(x - teethW, imgL);
            float midX = x - teethW * 0.5f;
            if (midX > imgL)
            {
                path.LineTo(midX, imgB - teethH);
                path.LineTo(nextX, imgB);
            }
            else
            {
                path.LineTo(nextX, imgB);
            }
            x = nextX;
        }

        // Left edge: bottom to top, teeth point rightward (into image)
        y = imgB;
        while (y > imgT)
        {
            float nextY = Math.Max(y - teethW, imgT);
            float midY = y - teethW * 0.5f;
            if (midY > imgT)
            {
                path.LineTo(imgL + teethH, midY);
                path.LineTo(imgL, nextY);
            }
            else
            {
                path.LineTo(imgL, nextY);
            }
            y = nextY;
        }

        path.Close();
        return path;
    }
}