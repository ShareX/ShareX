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

namespace ShareX.PhotoEditor.Imaging;

/// <summary>Match Color: moves an image's Lab color statistics onto those of a source image.</summary>
public static class ColorMatch
{
    private const int MaxSamples = 400_000;

    /// <summary>Mean and standard deviation of the Lab channels of an image.</summary>
    public readonly record struct Statistics(float MeanL, float MeanA, float MeanB, float DeviationL, float DeviationA, float DeviationB);

    /// <summary>Measures an image, weighted by alpha and the optional weight. Returns null when no pixel counts.</summary>
    public static unsafe Statistics? Measure(SKBitmap bitmap, Func<int, int, float>? weight = null)
    {
        int step = Math.Max(1, (int)MathF.Sqrt(bitmap.Width * (float)bitmap.Height / MaxSamples));
        uint* pixels = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        double total = 0, sumL = 0, sumA = 0, sumB = 0, squareL = 0, squareA = 0, squareB = 0;
        for (int y = 0; y < bitmap.Height; y += step)
        {
            for (int x = 0; x < bitmap.Width; x += step)
            {
                uint p = pixels[y * stride + x];
                if ((p >> 24) == 0) continue;
                float w = weight?.Invoke(x, y) ?? 1;
                if (w <= 0) continue;
                ColorMath.Unpack(p, out float r, out float g, out float b, out float alpha);
                ColorMath.RgbToLab(r, g, b, out float labL, out float labA, out float labB);
                w *= alpha;
                total += w;
                sumL += labL * w;
                sumA += labA * w;
                sumB += labB * w;
                squareL += labL * labL * w;
                squareA += labA * labA * w;
                squareB += labB * labB * w;
            }
        }
        if (total <= 0) return null;
        double meanL = sumL / total, meanA = sumA / total, meanB = sumB / total;
        return new Statistics((float)meanL, (float)meanA, (float)meanB,
            Deviation(squareL / total, meanL), Deviation(squareA / total, meanA), Deviation(squareB / total, meanB));
    }

    private static float Deviation(double meanSquare, double mean) => (float)Math.Sqrt(Math.Max(0, meanSquare - mean * mean));

    public static void Store(AdjustmentSettings settings, string prefix, Statistics statistics)
    {
        settings.Set($"{prefix}.meanL", statistics.MeanL);
        settings.Set($"{prefix}.meanA", statistics.MeanA);
        settings.Set($"{prefix}.meanB", statistics.MeanB);
        settings.Set($"{prefix}.deviationL", statistics.DeviationL);
        settings.Set($"{prefix}.deviationA", statistics.DeviationA);
        settings.Set($"{prefix}.deviationB", statistics.DeviationB);
    }

    private static Statistics Read(AdjustmentSettings settings, string prefix) => new(
        settings.Get($"{prefix}.meanL"), settings.Get($"{prefix}.meanA"), settings.Get($"{prefix}.meanB"),
        settings.Get($"{prefix}.deviationL"), settings.Get($"{prefix}.deviationA"), settings.Get($"{prefix}.deviationB"));

    private static float Ratio(float source, float target) => target < 0.01f ? 1 : Math.Clamp(source / target, 0.05f, 20);

    public static Adjustments.PixelFunction CreateFunction(AdjustmentSettings s)
    {
        Statistics target = Read(s, "target");
        Statistics source = s.Get("hasSource") > 0 ? Read(s, "source") : target;
        bool neutralize = s.Get("neutralize") > 0;
        float luminance = s.Get("luminance", 100) / 100f;
        float intensity = s.Get("intensity", 100) / 100f;
        float fade = s.Get("fade") / 100f;
        float scaleL = Ratio(source.DeviationL, target.DeviationL);
        float scaleA = Ratio(source.DeviationA, target.DeviationA);
        float scaleB = Ratio(source.DeviationB, target.DeviationB);
        float centerA = neutralize ? 0 : source.MeanA;
        float centerB = neutralize ? 0 : source.MeanB;
        return (ref float r, ref float g, ref float b) =>
        {
            ColorMath.RgbToLab(r, g, b, out float labL, out float labA, out float labB);
            labL = (source.MeanL + (labL - target.MeanL) * scaleL) * luminance;
            labA = (centerA + (labA - target.MeanA) * scaleA) * intensity;
            labB = (centerB + (labB - target.MeanB) * scaleB) * intensity;
            ColorMath.LabToRgb(labL, labA, labB, out float matchedR, out float matchedG, out float matchedB);
            r = matchedR + (r - matchedR) * fade;
            g = matchedG + (g - matchedG) * fade;
            b = matchedB + (b - matchedB) * fade;
        };
    }
}
