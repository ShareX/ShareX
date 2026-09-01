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

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public sealed class AutoContrastImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "auto_contrast";
    public override string Name => "Auto contrast";
    public override string IconKey => LucideIcons.wand_sparkles;
    public override string Description => "Automatically adjusts the contrast.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<AutoContrastImageEffect>("clip_percent", "Clip percent", 0, 20, 0.5, (effect, value) => effect.ClipPercent = value, tickFrequency: 0.1, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.0}")
    ];

    // Histogram clipping amount per side in percent.
    public float ClipPercent { get; set; } = 0.5f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float clipPercent = Math.Clamp(ClipPercent, 0f, 20f);
        SKColor[] srcPixels = source.Pixels;
        int total = srcPixels.Length;
        if (total == 0)
        {
            return source.Copy();
        }

        int clipCount = (int)MathF.Round(total * (clipPercent / 100f));

        int[] histR = new int[256];
        int[] histG = new int[256];
        int[] histB = new int[256];

        for (int i = 0; i < total; i++)
        {
            SKColor c = srcPixels[i];
            histR[c.Red]++;
            histG[c.Green]++;
            histB[c.Blue]++;
        }

        FindRange(histR, clipCount, out int minR, out int maxR);
        FindRange(histG, clipCount, out int minG, out int maxG);
        FindRange(histB, clipCount, out int minB, out int maxB);

        if ((maxR <= minR) && (maxG <= minG) && (maxB <= minB))
        {
            return source.Copy();
        }

        SKColor[] dstPixels = new SKColor[total];
        for (int i = 0; i < total; i++)
        {
            SKColor c = srcPixels[i];
            byte r = Stretch(c.Red, minR, maxR);
            byte g = Stretch(c.Green, minG, maxG);
            byte b = Stretch(c.Blue, minB, maxB);
            dstPixels[i] = new SKColor(r, g, b, c.Alpha);
        }

        return new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static void FindRange(int[] histogram, int clipCount, out int min, out int max)
    {
        min = 0;
        int sum = 0;
        for (int i = 0; i < 256; i++)
        {
            sum += histogram[i];
            if (sum > clipCount)
            {
                min = i;
                break;
            }
        }

        max = 255;
        sum = 0;
        for (int i = 255; i >= 0; i--)
        {
            sum += histogram[i];
            if (sum > clipCount)
            {
                max = i;
                break;
            }
        }
    }

    private static byte Stretch(byte value, int min, int max)
    {
        if (max <= min)
        {
            return value;
        }

        if (value <= min) return 0;
        if (value >= max) return 255;

        float mapped = (value - min) * 255f / (max - min);
        return (byte)MathF.Round(mapped);
    }
}