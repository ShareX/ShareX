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

public sealed class AddNoiseImageEffect : ImageEffectBase
{
    public override string Id => "add_noise";
    public override string Name => "Add noise";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.spray_can;
    public override string Description => "Adds random noise to the image.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<AddNoiseImageEffect>("amount", "Amount", 0, 100, 8, (e, v) => e.Amount = v)
    ];

    public float Amount { get; set; } = 8f;
    public int Seed { get; set; } = 1337;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float amount = Math.Clamp(Amount, 0f, 100f);
        int amplitude = (int)MathF.Round(amount * 1.27f); // 0..127
        if (amplitude <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        uint seed = unchecked((uint)Seed * 747796405u);

        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor c = srcPixels[i];

            uint h = Hash((uint)i ^ seed);
            int nr = (((int)(h & 0xFF)) - 128) * amplitude / 128;
            h = Hash(h ^ 0x9E3779B9u);
            int ng = (((int)(h & 0xFF)) - 128) * amplitude / 128;
            h = Hash(h ^ 0x85EBCA6Bu);
            int nb = (((int)(h & 0xFF)) - 128) * amplitude / 128;

            byte r = ClampToByte(c.Red + nr);
            byte g = ClampToByte(c.Green + ng);
            byte b = ClampToByte(c.Blue + nb);

            dstPixels[i] = new SKColor(r, g, b, c.Alpha);
        }

        return new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static uint Hash(uint x)
    {
        x ^= x >> 16;
        x *= 0x7FEB352Du;
        x ^= x >> 15;
        x *= 0x846CA68Bu;
        x ^= x >> 16;
        return x;
    }

    private static byte ClampToByte(int value)
    {
        if (value <= 0) return 0;
        if (value >= 255) return 255;
        return (byte)value;
    }
}