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

public enum DatamoshDirection
{
    Horizontal,
    Vertical
}

public sealed class DatamoshSmearImageEffect : ImageEffectBase
{
    public override string Id => "datamosh_smear";
    public override string Name => "Datamosh smear";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.cpu;
    public override string Description => "Simulates digital datamoshing with smeared pixel blocks and channel splitting.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<DatamoshSmearImageEffect, DatamoshDirection>("direction", "Direction", DatamoshDirection.Horizontal, (e, v) => e.Direction = v, new (string, DatamoshDirection)[] { ("Horizontal", DatamoshDirection.Horizontal), ("Vertical", DatamoshDirection.Vertical) }),
        EffectParameters.FloatSlider<DatamoshSmearImageEffect>("smear_amount", "Smear amount", 0, 100, 58, (e, v) => e.SmearAmount = v),
        EffectParameters.FloatSlider<DatamoshSmearImageEffect>("corruption", "Corruption", 0, 100, 36, (e, v) => e.Corruption = v),
        EffectParameters.IntSlider<DatamoshSmearImageEffect>("block_size", "Block size", 4, 64, 12, (e, v) => e.BlockSize = v),
        EffectParameters.FloatSlider<DatamoshSmearImageEffect>("drift", "Drift", -100, 100, 24, (e, v) => e.Drift = v),
        EffectParameters.FloatSlider<DatamoshSmearImageEffect>("channel_split", "Channel split", 0, 100, 25, (e, v) => e.ChannelSplit = v)
    ];

    public DatamoshDirection Direction { get; set; } = DatamoshDirection.Horizontal;
    public float SmearAmount { get; set; } = 58f;
    public float Corruption { get; set; } = 36f;
    public int BlockSize { get; set; } = 12;
    public float Drift { get; set; } = 24f;
    public float ChannelSplit { get; set; } = 25f;
    public int Seed { get; set; } = 9011;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float smear = Math.Clamp(SmearAmount, 0f, 100f) / 100f;
        float corruption = Math.Clamp(Corruption, 0f, 100f) / 100f;
        int blockSize = Math.Clamp(BlockSize, 4, 64);
        float drift = Math.Clamp(Drift, -100f, 100f);
        float channelSplit = Math.Clamp(ChannelSplit, 0f, 100f) / 100f * 7f;

        if (smear <= 0.0001f && corruption <= 0.0001f && Math.Abs(drift) <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float smearDistance = smear * blockSize * 5.5f;

        if (Direction == DatamoshDirection.Horizontal)
        {
            Parallel.For(0, height, y =>
            {
                int row = y * width;
                float rowNoise = (ProceduralEffectHelper.Hash01(y / Math.Max(1, blockSize), 7, Seed) * 2f) - 1f;
                float baseShift = (MathF.Sin((y * 0.09f) + 0.4f) + (rowNoise * 1.15f)) * drift * 0.34f;
                float carry = 0f;

                for (int blockX = 0; blockX < width; blockX += blockSize)
                {
                    int blockIndex = blockX / blockSize;
                    float trigger = ProceduralEffectHelper.Hash01(blockIndex, y / Math.Max(1, blockSize), Seed ^ 313);

                    if (trigger > 1f - (corruption * 0.78f))
                    {
                        float sign = (ProceduralEffectHelper.Hash01(blockIndex, y, Seed ^ 743) * 2f) - 1f;
                        carry += sign * smearDistance * (0.35f + trigger);
                    }
                    else
                    {
                        carry *= 0.82f;
                    }

                    int x1 = Math.Min(width, blockX + blockSize);
                    for (int x = blockX; x < x1; x++)
                    {
                        float fx = x - carry - baseShift;
                        SKColor sr = AnalogEffectHelper.Sample(srcPixels, width, height, fx + channelSplit, y);
                        SKColor sg = AnalogEffectHelper.Sample(srcPixels, width, height, fx, y);
                        SKColor sb = AnalogEffectHelper.Sample(srcPixels, width, height, fx - channelSplit, y);

                        dstPixels[row + x] = new SKColor(sr.Red, sg.Green, sb.Blue, sg.Alpha);
                    }
                }
            });
        }
        else
        {
            Parallel.For(0, width, x =>
            {
                float columnNoise = (ProceduralEffectHelper.Hash01(x / Math.Max(1, blockSize), 11, Seed) * 2f) - 1f;
                float baseShift = (MathF.Cos((x * 0.07f) + 1.1f) + (columnNoise * 1.20f)) * drift * 0.34f;
                float carry = 0f;

                for (int blockY = 0; blockY < height; blockY += blockSize)
                {
                    int blockIndex = blockY / blockSize;
                    float trigger = ProceduralEffectHelper.Hash01(x / Math.Max(1, blockSize), blockIndex, Seed ^ 919);

                    if (trigger > 1f - (corruption * 0.78f))
                    {
                        float sign = (ProceduralEffectHelper.Hash01(x, blockIndex, Seed ^ 1597) * 2f) - 1f;
                        carry += sign * smearDistance * (0.35f + trigger);
                    }
                    else
                    {
                        carry *= 0.82f;
                    }

                    int y1 = Math.Min(height, blockY + blockSize);
                    for (int y = blockY; y < y1; y++)
                    {
                        float fy = y - carry - baseShift;
                        SKColor sr = AnalogEffectHelper.Sample(srcPixels, width, height, x, fy + channelSplit);
                        SKColor sg = AnalogEffectHelper.Sample(srcPixels, width, height, x, fy);
                        SKColor sb = AnalogEffectHelper.Sample(srcPixels, width, height, x, fy - channelSplit);

                        dstPixels[(y * width) + x] = new SKColor(sr.Red, sg.Green, sb.Blue, sg.Alpha);
                    }
                }
            });
        }

        return AnalogEffectHelper.CreateBitmap(source, dstPixels);
    }
}