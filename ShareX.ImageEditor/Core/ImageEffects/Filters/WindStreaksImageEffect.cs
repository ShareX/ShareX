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

using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public enum WindDirection
{
    Left,
    Right
}

public sealed class WindStreaksImageEffect : ImageEffectBase
{
    public override string Id => "wind_streaks";
    public override string Name => "Wind streaks";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.wind;
    public override string Description => "Adds directional wind streak trails to bright areas.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<WindStreaksImageEffect>("length", "Streak length", 5, 100, 30, (e, v) => e.Length = v),
        EffectParameters.IntSlider<WindStreaksImageEffect>("threshold", "Brightness threshold", 100, 254, 200, (e, v) => e.Threshold = v),
        EffectParameters.FloatSlider<WindStreaksImageEffect>("opacity", "Opacity", 0, 100, 40, (e, v) => e.Opacity = v),
        EffectParameters.Enum<WindStreaksImageEffect, WindDirection>(
            "direction", "Direction", WindDirection.Right, (e, v) => e.Direction = v,
            new (string, WindDirection)[] { ("Left", WindDirection.Left), ("Right", WindDirection.Right) })
    ];

    public int Length { get; set; } = 30;
    public int Threshold { get; set; } = 200;
    public float Opacity { get; set; } = 40f;
    public WindDirection Direction { get; set; } = WindDirection.Right;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int w = source.Width, h = source.Height;
        int len = Math.Max(1, Length);
        float alpha = Math.Clamp(Opacity / 100f, 0f, 1f);
        SKColor[] src = source.Pixels;
        SKColor[] dst = (SKColor[])src.Clone();
        int dir = Direction == WindDirection.Right ? 1 : -1;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                SKColor c = src[y * w + x];
                float lum = 0.2126f * c.Red + 0.7152f * c.Green + 0.0722f * c.Blue;

                if (lum < Threshold) continue;

                float streakAlpha = alpha * ((lum - Threshold) / (255f - Threshold));

                for (int s = 1; s <= len; s++)
                {
                    int tx = x + s * dir;
                    if (tx < 0 || tx >= w) break;

                    float fade = 1f - (float)s / len;
                    float blend = streakAlpha * fade;

                    SKColor existing = dst[y * w + tx];
                    byte r = (byte)(existing.Red + (c.Red - existing.Red) * blend);
                    byte g = (byte)(existing.Green + (c.Green - existing.Green) * blend);
                    byte b = (byte)(existing.Blue + (c.Blue - existing.Blue) * blend);
                    dst[y * w + tx] = new SKColor(r, g, b, existing.Alpha);
                }
            }
        }

        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }
}