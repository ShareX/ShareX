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

public sealed class ColorMatrixImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "color_matrix";
    public override string Name => "Color matrix";
    public override string IconKey => LucideIcons.grid_3x3;
    public override string Description => "Applies a color matrix transformation.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("rr", "Rr", -5, 5, 1, (effect, value) => effect.Rr = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("rg", "Rg", -5, 5, 0, (effect, value) => effect.Rg = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("rb", "Rb", -5, 5, 0, (effect, value) => effect.Rb = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("ra", "Ra", -5, 5, 0, (effect, value) => effect.Ra = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("ro", "Ro", -255, 255, 0, (effect, value) => effect.Ro = (float)value, increment: 1, formatString: "0"),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("gr", "Gr", -5, 5, 0, (effect, value) => effect.Gr = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("gg", "Gg", -5, 5, 1, (effect, value) => effect.Gg = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("gb", "Gb", -5, 5, 0, (effect, value) => effect.Gb = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("ga", "Ga", -5, 5, 0, (effect, value) => effect.Ga = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("go", "Go", -255, 255, 0, (effect, value) => effect.Go = (float)value, increment: 1, formatString: "0"),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("br", "Br", -5, 5, 0, (effect, value) => effect.Br = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("bg", "Bg", -5, 5, 0, (effect, value) => effect.Bg = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("bb", "Bb", -5, 5, 1, (effect, value) => effect.Bb = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("ba", "Ba", -5, 5, 0, (effect, value) => effect.Ba = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("bo", "Bo", -255, 255, 0, (effect, value) => effect.Bo = (float)value, increment: 1, formatString: "0"),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("ar", "Ar", -5, 5, 0, (effect, value) => effect.Ar = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("ag", "Ag", -5, 5, 0, (effect, value) => effect.Ag = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("ab", "Ab", -5, 5, 0, (effect, value) => effect.Ab = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("aa", "Aa", -5, 5, 1, (effect, value) => effect.Aa = (float)value, increment: 0.1),
        EffectParameters.DoubleNumeric<ColorMatrixImageEffect>("ao", "Ao", -255, 255, 0, (effect, value) => effect.Ao = (float)value, increment: 1, formatString: "0")
    ];

    // Red output
    public float Rr { get; set; } = 1f;
    public float Rg { get; set; }
    public float Rb { get; set; }
    public float Ra { get; set; }
    public float Ro { get; set; }

    // Green output
    public float Gr { get; set; }
    public float Gg { get; set; } = 1f;
    public float Gb { get; set; }
    public float Ga { get; set; }
    public float Go { get; set; }

    // Blue output
    public float Br { get; set; }
    public float Bg { get; set; }
    public float Bb { get; set; } = 1f;
    public float Ba { get; set; }
    public float Bo { get; set; }

    // Alpha output
    public float Ar { get; set; }
    public float Ag { get; set; }
    public float Ab { get; set; }
    public float Aa { get; set; } = 1f;
    public float Ao { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        float[] matrix =
        {
            Rr, Rg, Rb, Ra, Ro,
            Gr, Gg, Gb, Ga, Go,
            Br, Bg, Bb, Ba, Bo,
            Ar, Ag, Ab, Aa, Ao
        };

        return ApplyColorMatrix(source, matrix);
    }
}