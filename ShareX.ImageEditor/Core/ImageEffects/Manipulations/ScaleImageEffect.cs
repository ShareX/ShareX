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

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class ScaleImageEffect : ImageEffectBase
{
    public override string Id => "scale";
    public override string Name => "Scale";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.scaling;
    public override string Description => "Scales the image by width and height percentages.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<ScaleImageEffect>("width_percentage", "Width %", 0f, 500f, 100f, (e, v) => e.WidthPercentage = v),
        EffectParameters.FloatSlider<ScaleImageEffect>("height_percentage", "Height %", 0f, 500f, 0f, (e, v) => e.HeightPercentage = v)
    ];

    public float WidthPercentage { get; set; } = 100f;
    public float HeightPercentage { get; set; } = 0f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        if (WidthPercentage <= 0f && HeightPercentage <= 0f)
        {
            return source.Copy();
        }

        int width = (int)Math.Round(WidthPercentage / 100f * source.Width);
        int height = (int)Math.Round(HeightPercentage / 100f * source.Height);

        if (width == 0)
        {
            width = (int)Math.Round((float)height / source.Height * source.Width);
        }
        else if (height == 0)
        {
            height = (int)Math.Round((float)width / source.Width * source.Height);
        }

        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKImageInfo info = new SKImageInfo(width, height, source.ColorType, source.AlphaType, source.ColorSpace);
        return source.Resize(info, new SKSamplingOptions(SKCubicResampler.CatmullRom)) ?? source.Copy();
    }
}