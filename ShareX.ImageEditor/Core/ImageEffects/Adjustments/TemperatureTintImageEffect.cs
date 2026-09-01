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

public sealed class TemperatureTintImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "temperature_tint";
    public override string Name => "Temperature / Tint";
    public override string IconKey => LucideIcons.thermometer;
    public override string Description => "Adjusts the color temperature and tint.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<TemperatureTintImageEffect>("temperature", "Temperature", -100, 100, 0, (effect, value) => effect.Temperature = value),
        EffectParameters.FloatSlider<TemperatureTintImageEffect>("tint", "Tint", -100, 100, 0, (effect, value) => effect.Tint = value)
    ];

    public float Temperature { get; set; } // -100..100
    public float Tint { get; set; } // -100..100

    public override SKBitmap Apply(SKBitmap source)
    {
        float temperature = Math.Clamp(Temperature, -100f, 100f);
        float tint = Math.Clamp(Tint, -100f, 100f);

        if (Math.Abs(temperature) < 0.0001f && Math.Abs(tint) < 0.0001f)
        {
            return source.Copy();
        }

        float tempDelta = temperature / 100f * 64f;
        float tintDelta = tint / 100f * 64f;

        return ApplyPixelOperation(source, c =>
        {
            float r = c.Red + tempDelta - tintDelta * 0.25f;
            float g = c.Green + tintDelta;
            float b = c.Blue - tempDelta - tintDelta * 0.25f;

            return new SKColor(ClampToByte(r), ClampToByte(g), ClampToByte(b), c.Alpha);
        });
    }

    private static byte ClampToByte(float value)
    {
        if (value <= 0f) return 0;
        if (value >= 255f) return 255;
        return (byte)MathF.Round(value);
    }
}