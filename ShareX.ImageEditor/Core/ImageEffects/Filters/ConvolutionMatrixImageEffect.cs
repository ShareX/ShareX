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

using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class ConvolutionMatrixImageEffect : ImageEffectBase
{
    public override string Id => "convolution_matrix";
    public override string Name => "Convolution matrix";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.grid_3x3;
    public override string Description => "Applies a custom 3x3 convolution kernel to the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("x0y0", "X0 Y0", -99, 99, 0, (e, v) => e.X0Y0 = v),
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("x1y0", "X1 Y0", -99, 99, 0, (e, v) => e.X1Y0 = v),
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("x2y0", "X2 Y0", -99, 99, 0, (e, v) => e.X2Y0 = v),
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("x0y1", "X0 Y1", -99, 99, 0, (e, v) => e.X0Y1 = v),
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("x1y1", "X1 Y1", -99, 99, 1, (e, v) => e.X1Y1 = v),
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("x2y1", "X2 Y1", -99, 99, 0, (e, v) => e.X2Y1 = v),
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("x0y2", "X0 Y2", -99, 99, 0, (e, v) => e.X0Y2 = v),
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("x1y2", "X1 Y2", -99, 99, 0, (e, v) => e.X1Y2 = v),
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("x2y2", "X2 Y2", -99, 99, 0, (e, v) => e.X2Y2 = v),
        EffectParameters.FloatSlider<ConvolutionMatrixImageEffect>("factor", "Factor", 0.01f, 10f, 1f, (e, v) => e.Factor = v),
        EffectParameters.IntNumeric<ConvolutionMatrixImageEffect>("offset", "Offset", -255, 255, 0, (e, v) => e.Offset = v),
    ];

    public int X0Y0 { get; set; }
    public int X1Y0 { get; set; }
    public int X2Y0 { get; set; }

    public int X0Y1 { get; set; }
    public int X1Y1 { get; set; } = 1;
    public int X2Y1 { get; set; }

    public int X0Y2 { get; set; }
    public int X1Y2 { get; set; }
    public int X2Y2 { get; set; }

    public double Factor { get; set; } = 1d;
    public int Offset { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float factor = (float)Math.Max(0.01d, Factor);
        float gain = 1f / factor;
        float bias = Offset;

        float[] kernel =
        {
            X0Y0, X1Y0, X2Y0,
            X0Y1, X1Y1, X2Y1,
            X0Y2, X1Y2, X2Y2
        };

        return ConvolutionHelper.Apply3x3(source, kernel, gain, bias);
    }
}