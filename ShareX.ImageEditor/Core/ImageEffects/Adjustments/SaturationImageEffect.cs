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

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public sealed class SaturationImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "saturation";
    public override string Name => "Saturation";
    public override string IconKey => LucideIcons.droplets;
    public override string Description => "Adjusts the color saturation.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<SaturationImageEffect>("amount", "Amount", -100, 100, 0, (effect, value) => effect.Amount = value)
    ];

    public float Amount { get; set; } = 0; // -100 to 100

    public override SKBitmap Apply(SKBitmap source)
    {
        float x = 1f + (Amount / 100f);
        float lumR = 0.3086f;
        float lumG = 0.6094f;
        float lumB = 0.0820f;

        float invSat = 1f - x;

        float r = (invSat * lumR);
        float g = (invSat * lumG);
        float b = (invSat * lumB);

        float[] matrix = {
            r + x, g,     b,     0, 0,
            r,     g + x, b,     0, 0,
            r,     g,     b + x, 0, 0,
            0,     0,     0,     1, 0
        };
        return ApplyColorMatrix(source, matrix);
    }
}