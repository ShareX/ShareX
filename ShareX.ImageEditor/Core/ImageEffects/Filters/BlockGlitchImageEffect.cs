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

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class BlockGlitchImageEffect : ImageEffectBase
{
    public override string Id => "block_glitch";
    public override string Name => "Block glitch / Databending";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.cpu;
    public override string Description => "Simulates digital glitch artifacts with displaced color blocks.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<BlockGlitchImageEffect>("block_count", "Block count", 1, 240, 36, (e, v) => e.BlockCount = v),
        EffectParameters.IntSlider<BlockGlitchImageEffect>("min_block_width", "Min block width", 4, 400, 24, (e, v) => e.MinBlockWidth = v),
        EffectParameters.IntSlider<BlockGlitchImageEffect>("max_block_width", "Max block width", 4, 900, 200, (e, v) => e.MaxBlockWidth = v),
        EffectParameters.IntSlider<BlockGlitchImageEffect>("min_block_height", "Min block height", 2, 200, 6, (e, v) => e.MinBlockHeight = v),
        EffectParameters.IntSlider<BlockGlitchImageEffect>("max_block_height", "Max block height", 2, 500, 50, (e, v) => e.MaxBlockHeight = v),
        EffectParameters.IntSlider<BlockGlitchImageEffect>("max_displacement", "Max displacement", 0, 500, 50, (e, v) => e.MaxDisplacement = v),
        EffectParameters.IntSlider<BlockGlitchImageEffect>("channel_shift", "Channel shift", 0, 64, 4, (e, v) => e.ChannelShift = v),
        EffectParameters.FloatSlider<BlockGlitchImageEffect>("noise_amount", "Noise amount", 0, 100, 10, (e, v) => e.NoiseAmount = v)
    ];

    public int BlockCount { get; set; } = 36; // 1..240
    public int MinBlockWidth { get; set; } = 24; // 4..400
    public int MaxBlockWidth { get; set; } = 200; // 4..900
    public int MinBlockHeight { get; set; } = 6; // 2..200
    public int MaxBlockHeight { get; set; } = 50; // 2..500
    public int MaxDisplacement { get; set; } = 50; // 0..500
    public int ChannelShift { get; set; } = 4; // 0..64
    public float NoiseAmount { get; set; } = 10f; // 0..100
    public int Seed { get; set; } = 1945;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        int blockCount = Math.Clamp(BlockCount, 1, 240);
        int minW = Math.Clamp(MinBlockWidth, 4, 400);
        int maxW = Math.Clamp(MaxBlockWidth, minW, 900);
        int minH = Math.Clamp(MinBlockHeight, 2, 200);
        int maxH = Math.Clamp(MaxBlockHeight, minH, 500);
        int maxShift = Math.Clamp(MaxDisplacement, 0, 500);
        int channelShift = Math.Clamp(ChannelShift, 0, 64);
        float noise = Math.Clamp(NoiseAmount, 0f, 100f) / 100f;

        SKColor[] src = source.Pixels;
        SKColor[] dst = new SKColor[src.Length];
        Array.Copy(src, dst, src.Length);

        Random random = new Random(Seed);

        for (int i = 0; i < blockCount; i++)
        {
            int bw = random.Next(minW, maxW + 1);
            int bh = random.Next(minH, maxH + 1);

            int x = random.Next(0, Math.Max(1, width - 1));
            int y = random.Next(0, Math.Max(1, height - 1));

            int right = Math.Min(width, x + bw);
            int bottom = Math.Min(height, y + bh);
            if (right <= x || bottom <= y)
            {
                continue;
            }

            int dx = random.Next(-maxShift, maxShift + 1);
            int dy = random.Next(-Math.Max(1, maxShift / 6), Math.Max(2, (maxShift / 6) + 1));

            int redShift = random.Next(-channelShift, channelShift + 1);
            int blueShift = random.Next(-channelShift, channelShift + 1);

            for (int py = y; py < bottom; py++)
            {
                for (int px = x; px < right; px++)
                {
                    int sx = Clamp(px + dx, 0, width - 1);
                    int sy = Clamp(py + dy, 0, height - 1);

                    int srX = Clamp(sx + redShift, 0, width - 1);
                    int sbX = Clamp(sx - blueShift, 0, width - 1);

                    SKColor baseColor = src[(sy * width) + sx];
                    SKColor rColor = src[(sy * width) + srX];
                    SKColor bColor = src[(sy * width) + sbX];

                    float nr = noise > 0f ? ((float)random.NextDouble() * 2f - 1f) * noise * 48f : 0f;
                    float ng = noise > 0f ? ((float)random.NextDouble() * 2f - 1f) * noise * 48f : 0f;
                    float nb = noise > 0f ? ((float)random.NextDouble() * 2f - 1f) * noise * 48f : 0f;

                    dst[(py * width) + px] = new SKColor(
                        ProceduralEffectHelper.ClampToByte(rColor.Red + nr),
                        ProceduralEffectHelper.ClampToByte(baseColor.Green + ng),
                        ProceduralEffectHelper.ClampToByte(bColor.Blue + nb),
                        baseColor.Alpha);
                }
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dst
        };
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}