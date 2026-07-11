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

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public enum CylinderWrapOrientation
{
    Vertical,
    Horizontal
}

public sealed class CylinderWrapImageEffect : ImageEffectBase
{
    public override string Id => "cylinder_wrap";
    public override string Name => "Cylinder wrap";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.cylinder;
    public override string Description => "Wraps the image around a cylindrical surface.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<CylinderWrapImageEffect, CylinderWrapOrientation>("orientation", "Orientation", CylinderWrapOrientation.Vertical, (e, v) => e.Orientation = v,
            new (string, CylinderWrapOrientation)[] { ("Vertical", CylinderWrapOrientation.Vertical), ("Horizontal", CylinderWrapOrientation.Horizontal) }),
        EffectParameters.FloatSlider<CylinderWrapImageEffect>("curvature", "Curvature", 0f, 100f, 65f, (e, v) => e.Curvature = v),
        EffectParameters.FloatSlider<CylinderWrapImageEffect>("edge_shading", "Edge shading", 0f, 100f, 35f, (e, v) => e.EdgeShading = v)
    ];

    public CylinderWrapOrientation Orientation { get; set; } = CylinderWrapOrientation.Vertical;
    public float Curvature { get; set; } = 65f;
    public float EdgeShading { get; set; } = 35f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float curvature01 = Math.Clamp(Curvature, 0f, 100f) / 100f;
        if (curvature01 <= 0f)
        {
            return source.Copy();
        }

        float shading01 = Math.Clamp(EdgeShading, 0f, 100f) / 100f;
        float maxAngle = 0.12f + (curvature01 * 1.28f);
        float sinMax = MathF.Sin(maxAngle);

        int width = source.Width;
        int height = source.Height;
        float widthRange = Math.Max(1f, width - 1);
        float heightRange = Math.Max(1f, height - 1);
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                float sampleX;
                float sampleY;
                float shade;

                if (Orientation == CylinderWrapOrientation.Vertical)
                {
                    float projected = ((x / widthRange) * 2f) - 1f;
                    float angle = MathF.Asin(Math.Clamp(projected * sinMax, -1f, 1f));
                    float u = ((angle / maxAngle) + 1f) * 0.5f;
                    sampleX = u * widthRange;
                    sampleY = y;
                    shade = 1f - (shading01 * (1f - MathF.Cos(angle)) * 0.65f);
                }
                else
                {
                    float projected = ((y / heightRange) * 2f) - 1f;
                    float angle = MathF.Asin(Math.Clamp(projected * sinMax, -1f, 1f));
                    float v = ((angle / maxAngle) + 1f) * 0.5f;
                    sampleX = x;
                    sampleY = v * heightRange;
                    shade = 1f - (shading01 * (1f - MathF.Cos(angle)) * 0.65f);
                }

                SKColor sampled = DistortionEffectHelper.SampleClamped(srcPixels, width, height, sampleX, sampleY);
                dstPixels[row + x] = DistortionEffectHelper.MultiplyRgb(sampled, shade);
            }
        });

        return DistortionEffectHelper.CreateBitmap(source, width, height, dstPixels);
    }
}