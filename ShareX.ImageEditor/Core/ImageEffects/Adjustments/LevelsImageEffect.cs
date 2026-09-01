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

public sealed class LevelsImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "levels";
    public override string Name => "Levels";
    public override string IconKey => LucideIcons.sliders_vertical;
    public override string Description => "Adjusts image color levels.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<LevelsImageEffect>("input_black", "Input black", 0, 255, 0, (effect, value) => effect.InputBlack = value),
        EffectParameters.IntSlider<LevelsImageEffect>("input_white", "Input white", 0, 255, 255, (effect, value) => effect.InputWhite = value),
        EffectParameters.FloatSlider<LevelsImageEffect>("gamma", "Gamma", 0.1, 5, 1, (effect, value) => effect.Gamma = value, tickFrequency: 0.1, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.0}"),
        EffectParameters.IntSlider<LevelsImageEffect>("output_black", "Output black", 0, 255, 0, (effect, value) => effect.OutputBlack = value),
        EffectParameters.IntSlider<LevelsImageEffect>("output_white", "Output white", 0, 255, 255, (effect, value) => effect.OutputWhite = value)
    ];

    public int InputBlack { get; set; }
    public int InputWhite { get; set; } = 255;
    public float Gamma { get; set; } = 1f;
    public int OutputBlack { get; set; }
    public int OutputWhite { get; set; } = 255;

    public override SKBitmap Apply(SKBitmap source)
    {
        int inBlack = Math.Clamp(InputBlack, 0, 255);
        int inWhite = Math.Clamp(InputWhite, 0, 255);
        int outBlack = Math.Clamp(OutputBlack, 0, 255);
        int outWhite = Math.Clamp(OutputWhite, 0, 255);
        float gamma = Math.Clamp(Gamma, 0.1f, 5f);

        if (inWhite <= inBlack)
        {
            inWhite = Math.Min(255, inBlack + 1);
        }

        if (outWhite < outBlack)
        {
            (outBlack, outWhite) = (outWhite, outBlack);
        }

        if (inBlack == 0 && inWhite == 255 && Math.Abs(gamma - 1f) < 0.0001f &&
            outBlack == 0 && outWhite == 255)
        {
            return source.Copy();
        }

        float inRange = inWhite - inBlack;
        float outRange = outWhite - outBlack;

        return ApplyPixelOperation(source, c =>
        {
            byte r = Map(c.Red, inBlack, inRange, gamma, outBlack, outRange);
            byte g = Map(c.Green, inBlack, inRange, gamma, outBlack, outRange);
            byte b = Map(c.Blue, inBlack, inRange, gamma, outBlack, outRange);
            return new SKColor(r, g, b, c.Alpha);
        });
    }

    private static byte Map(byte value, int inBlack, float inRange, float gamma, int outBlack, float outRange)
    {
        float normalized = (value - inBlack) / inRange;
        normalized = Math.Clamp(normalized, 0f, 1f);
        float corrected = MathF.Pow(normalized, gamma);
        float output = outBlack + corrected * outRange;

        if (output <= 0f) return 0;
        if (output >= 255f) return 255;
        return (byte)MathF.Round(output);
    }
}