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
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public enum FoldCreaseOrientation
{
    Vertical,
    Horizontal
}

public sealed class FoldCreaseWarpImageEffect : ImageEffectBase
{
    public override string Id => "fold_crease_warp";
    public override string Name => "Fold crease warp";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.newspaper;
    public override string Description => "Simulates a folded paper crease distortion.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<FoldCreaseWarpImageEffect, FoldCreaseOrientation>("orientation", "Orientation", FoldCreaseOrientation.Vertical, (e, v) => e.Orientation = v,
            new (string, FoldCreaseOrientation)[] { ("Vertical", FoldCreaseOrientation.Vertical), ("Horizontal", FoldCreaseOrientation.Horizontal) }),
        EffectParameters.IntSlider<FoldCreaseWarpImageEffect>("fold_count", "Fold count", 1, 10, 3, (e, v) => e.FoldCount = v),
        EffectParameters.FloatSlider<FoldCreaseWarpImageEffect>("fold_depth", "Fold depth", 0f, 100f, 30f, (e, v) => e.FoldDepth = v),
        EffectParameters.FloatSlider<FoldCreaseWarpImageEffect>("shadow_strength", "Shadow strength", 0f, 100f, 40f, (e, v) => e.ShadowStrength = v)
    ];

    public FoldCreaseOrientation Orientation { get; set; } = FoldCreaseOrientation.Vertical;
    public int FoldCount { get; set; } = 3;
    public float FoldDepth { get; set; } = 30f;
    public float ShadowStrength { get; set; } = 40f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int foldCount = Math.Clamp(FoldCount, 1, 10);
        float depth01 = Math.Clamp(FoldDepth, 0f, 100f) / 100f;
        if (depth01 <= 0f)
        {
            return source.Copy();
        }

        float shadow01 = Math.Clamp(ShadowStrength, 0f, 100f) / 100f;
        int width = source.Width;
        int height = source.Height;
        float span = (Orientation == FoldCreaseOrientation.Vertical ? Math.Max(1f, width - 1) : Math.Max(1f, height - 1)) / foldCount;
        float displacement = span * (0.06f + (depth01 * 0.22f));

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                float primary = Orientation == FoldCreaseOrientation.Vertical ? x : y;
                float phase = (primary / Math.Max(1f, span)) * MathF.PI;
                float offset = MathF.Sin(phase) * displacement;

                float sampleX = Orientation == FoldCreaseOrientation.Vertical ? x + offset : x;
                float sampleY = Orientation == FoldCreaseOrientation.Vertical ? y : y + offset;

                SKColor sampled = DistortionEffectHelper.SampleClamped(srcPixels, width, height, sampleX, sampleY);

                float curvature = Math.Abs(MathF.Sin(phase));
                float slope = MathF.Cos(phase);
                float shade = 1f - (shadow01 * curvature * 0.22f) + (shadow01 * slope * 0.08f);

                dstPixels[row + x] = DistortionEffectHelper.MultiplyRgb(sampled, shade);
            }
        });

        return DistortionEffectHelper.CreateBitmap(source, width, height, dstPixels);
    }
}