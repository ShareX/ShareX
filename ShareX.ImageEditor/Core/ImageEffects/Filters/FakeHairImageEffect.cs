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

public sealed class FakeHairImageEffect : ImageEffectBase
{
    public override string Id => "fake_hair";
    public override string Name => "Fake Hair";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.scissors;
    public override string Description => "Scatters realistic hair strands over the image to annoy your friends.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<FakeHairImageEffect>("count", "Hair Count", 1, 100, 1, (e, v) => e.Count = v),
        EffectParameters.FloatSlider<FakeHairImageEffect>("length", "Length", 20, 600, 180, (e, v) => e.Length = v),
        EffectParameters.FloatSlider<FakeHairImageEffect>("thickness", "Thickness", 0.5, 4, 1, (e, v) => e.Thickness = v,
            tickFrequency: 0.1, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.0}"),
        EffectParameters.FloatSlider<FakeHairImageEffect>("curliness", "Curliness", 0, 100, 30, (e, v) => e.Curliness = v),
        EffectParameters.Color<FakeHairImageEffect>("hair_color", "Hair Color", new SKColor(20, 15, 10, 220), (e, v) => e.HairColor = v)
    ];

    public int Count { get; set; } = 1;
    public float Length { get; set; } = 180f;
    public float Thickness { get; set; } = 1.5f;
    public float Curliness { get; set; } = 30f;
    public SKColor HairColor { get; set; } = new SKColor(20, 15, 10, 220);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int count = Math.Clamp(Count, 0, 1000);
        if (count == 0) return source.Copy();

        float length = Math.Clamp(Length, 5f, 2000f);
        float thickness = Math.Clamp(Thickness, 0.2f, 20f);
        float curliness = Math.Clamp(Curliness, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);

        int rng = Environment.TickCount;
        if (rng == 0) rng = 1;

        for (int i = 0; i < count; i++)
        {
            // Random start position — ensure the strand starts within the visible image.
            float startX = NextFloat(ref rng) * width;
            float startY = NextFloat(ref rng) * height;

            // Random base angle.
            float baseAngle = NextFloat(ref rng) * MathF.PI * 2f;

            // Per-hair variation in color (slight lightness/darkness shift).
            float colorVariation = (NextFloat(ref rng) - 0.5f) * 0.3f;
            byte hR = ClampToByte(HairColor.Red + HairColor.Red * colorVariation);
            byte hG = ClampToByte(HairColor.Green + HairColor.Green * colorVariation);
            byte hB = ClampToByte(HairColor.Blue + HairColor.Blue * colorVariation);
            // Slight alpha variation for realism.
            byte hA = ClampToByte(180 + NextFloat(ref rng) * 75f);

            // Per-hair thickness variation.
            float hairThickness = thickness * (0.6f + NextFloat(ref rng) * 0.8f);

            // Per-hair length variation.
            float hairLength = length * (0.5f + NextFloat(ref rng) * 0.7f);

            // Build a curved path with multiple segments.
            int segments = Math.Max(8, (int)(hairLength / 6f));
            float segmentLen = hairLength / segments;

            // Max random angle change per segment due to curliness.
            float maxTurn = curliness * 0.35f;

            using SKPath path = new();
            float px = startX;
            float py = startY;
            path.MoveTo(px, py);

            float angle = baseAngle;
            for (int s = 1; s <= segments; s++)
            {
                // Random direction change at each segment — more curliness
                // means bigger random turns, giving natural irregular curls.
                angle += (NextFloat(ref rng) - 0.5f) * 2f * maxTurn;

                px += MathF.Cos(angle) * segmentLen;
                py += MathF.Sin(angle) * segmentLen;

                path.LineTo(px, py);
            }

            // Draw the hair strand.
            using SKPaint paint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = new SKColor(hR, hG, hB, hA),
                StrokeWidth = hairThickness,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

            canvas.DrawPath(path, paint);

            // Optionally draw a slightly thinner, lighter highlight strand
            // offset by a sub-pixel for a realistic sheen.
            if (hairThickness > 0.8f)
            {
                using SKPaint sheen = new()
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = new SKColor(
                        ClampToByte(hR + 60),
                        ClampToByte(hG + 50),
                        ClampToByte(hB + 40),
                        (byte)(hA / 4)),
                    StrokeWidth = hairThickness * 0.3f,
                    StrokeCap = SKStrokeCap.Round,
                    StrokeJoin = SKStrokeJoin.Round
                };

                canvas.Save();
                canvas.Translate(0.3f, -0.3f);
                canvas.DrawPath(path, sheen);
                canvas.Restore();
            }
        }

        return result;
    }

    private static float NextFloat(ref int state)
    {
        state ^= state << 13;
        state ^= state >> 17;
        state ^= state << 5;
        return (state & 0x7FFFFFFF) / (float)int.MaxValue;
    }

    private static byte ClampToByte(float value)
    {
        if (value <= 0f) return 0;
        if (value >= 255f) return 255;
        return (byte)MathF.Round(value);
    }
}