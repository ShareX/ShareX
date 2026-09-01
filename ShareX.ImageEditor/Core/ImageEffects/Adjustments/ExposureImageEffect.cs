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

public sealed class ExposureImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "exposure";
    public override string Name => "Exposure";
    public override string IconKey => LucideIcons.aperture;
    public override string Description => "Adjusts the exposure level.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<ExposureImageEffect>("amount", "Exposure", -100, 100, 0, (effect, value) => effect.Amount = value)
    ];

    // Exposure in stops. Typical range: -5..5
    public float Amount { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        float amount = Math.Clamp(Amount, -10f, 10f);
        if (Math.Abs(amount) < 0.0001f)
        {
            return source.Copy();
        }

        float gain = MathF.Pow(2f, amount);

        return ApplyPixelOperation(source, c =>
        {
            byte r = ClampToByte(c.Red * gain);
            byte g = ClampToByte(c.Green * gain);
            byte b = ClampToByte(c.Blue * gain);
            return new SKColor(r, g, b, c.Alpha);
        });
    }

    private static byte ClampToByte(float value)
    {
        if (value <= 0f) return 0;
        if (value >= 255f) return 255;
        return (byte)MathF.Round(value);
    }
}