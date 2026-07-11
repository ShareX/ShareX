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

public sealed class GrayscaleImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "grayscale";
    public override string Name => "Grayscale";
    public override string IconKey => LucideIcons.eclipse;
    public override string Description => "Converts the image to grayscale.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<GrayscaleImageEffect>("strength", "Strength", 0, 100, 100, (effect, value) => effect.Strength = value)
    ];

    public float Strength { get; set; } = 100f;

    public override SKBitmap Apply(SKBitmap source)
    {
        float strength = Strength;
        if (strength >= 100)
        {
            float[] matrix = {
                0.2126f, 0.7152f, 0.0722f, 0, 0,
                0.2126f, 0.7152f, 0.0722f, 0, 0,
                0.2126f, 0.7152f, 0.0722f, 0, 0,
                0,       0,       0,       1, 0
            };
            return ApplyColorMatrix(source, matrix);
        }
        else if (strength <= 0)
        {
            return source.Copy();
        }
        else
        {
            float s = strength / 100f;
            float invS = 1f - s;

            float[] matrix = {
                0.2126f * s + invS, 0.7152f * s,        0.0722f * s,        0, 0,
                0.2126f * s,        0.7152f * s + invS, 0.0722f * s,        0, 0,
                0.2126f * s,        0.7152f * s,        0.0722f * s + invS, 0, 0,
                0,                  0,                  0,                  1, 0
            };
            return ApplyColorMatrix(source, matrix);
        }
    }
}