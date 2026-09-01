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

public sealed class CrossProcessImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "cross_process";
    public override string Name => "Cross process";
    public override string IconKey => LucideIcons.camera;
    public override string Description => "Simulates film cross-processing with shifted colors and high contrast.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<CrossProcessImageEffect>("strength", "Strength", 0, 100, 100, (e, v) => e.Strength = v)
    ];

    public float Strength { get; set; } = 100f;

    public override SKBitmap Apply(SKBitmap source)
    {
        float s = Math.Clamp(Strength / 100f, 0f, 1f);
        if (s <= 0f) return source.Copy();

        // Cross-process color matrix: boost greens/yellows, shift blues to cyan, warm shadows
        float[] crossMatrix =
        {
            1.2f,  0.1f, -0.1f, 0, 0.05f,
           -0.1f,  1.3f,  0.0f, 0, 0.02f,
           -0.1f,  0.0f,  0.8f, 0, 0.1f,
            0,     0,     0,    1, 0
        };

        float[] identity =
        {
            1, 0, 0, 0, 0,
            0, 1, 0, 0, 0,
            0, 0, 1, 0, 0,
            0, 0, 0, 1, 0
        };

        float[] matrix = new float[20];
        for (int i = 0; i < 20; i++)
            matrix[i] = identity[i] * (1f - s) + crossMatrix[i] * s;

        return ApplyColorMatrix(source, matrix);
    }
}