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
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public enum AnaglyphMode
{
    RedCyan,
    AmberBlue,
    GreenMagenta
}

public sealed class Anaglyph3DImageEffect : ImageEffectBase
{
    public override string Id => "anaglyph_3d";
    public override string Name => "Anaglyph 3D";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.glasses;
    public override string Description => "Simulates a retro anaglyph 3D stereoscopic effect.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<Anaglyph3DImageEffect, AnaglyphMode>(
            "mode",
            "Mode",
            AnaglyphMode.RedCyan,
            (e, v) => e.Mode = v,
            new (string Label, AnaglyphMode Value)[]
            {
                ("Red / Cyan", AnaglyphMode.RedCyan),
                ("Amber / Blue", AnaglyphMode.AmberBlue),
                ("Green / Magenta", AnaglyphMode.GreenMagenta)
            }),
        EffectParameters.IntSlider<Anaglyph3DImageEffect>("separation_x", "Separation X", -100, 100, 10, (e, v) => e.SeparationX = v),
        EffectParameters.IntSlider<Anaglyph3DImageEffect>("separation_y", "Separation Y", -100, 100, 0, (e, v) => e.SeparationY = v),
        EffectParameters.FloatSlider<Anaglyph3DImageEffect>("ghost_reduction", "Ghost reduction", 0, 100, 45, (e, v) => e.GhostReduction = v)
    ];

    public AnaglyphMode Mode { get; set; } = AnaglyphMode.RedCyan;
    public int SeparationX { get; set; } = 10;
    public int SeparationY { get; set; }
    public float GhostReduction { get; set; } = 45f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float ghostReduction = Math.Clamp(GhostReduction, 0f, 100f) / 100f;
        if (SeparationX == 0 && SeparationY == 0)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float halfX = SeparationX * 0.5f;
        float halfY = SeparationY * 0.5f;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor left = AnalogEffectHelper.Sample(srcPixels, width, height, x - halfX, y - halfY);
                SKColor right = AnalogEffectHelper.Sample(srcPixels, width, height, x + halfX, y + halfY);

                float leftGray = AnalogEffectHelper.Luminance01(left);
                float rightGray = AnalogEffectHelper.Luminance01(right);
                float r;
                float g;
                float b;

                switch (Mode)
                {
                    case AnaglyphMode.AmberBlue:
                        r = ProceduralEffectHelper.Lerp(left.Red / 255f, leftGray, ghostReduction * 0.85f);
                        g = ProceduralEffectHelper.Lerp(left.Green / 255f, leftGray, ghostReduction * 0.50f);
                        b = ProceduralEffectHelper.Lerp(right.Blue / 255f, rightGray, ghostReduction);
                        break;
                    case AnaglyphMode.GreenMagenta:
                        r = ProceduralEffectHelper.Lerp(right.Red / 255f, rightGray, ghostReduction * 0.85f);
                        g = ProceduralEffectHelper.Lerp(left.Green / 255f, leftGray, ghostReduction);
                        b = ProceduralEffectHelper.Lerp(right.Blue / 255f, rightGray, ghostReduction * 0.85f);
                        break;
                    default:
                        r = ProceduralEffectHelper.Lerp(left.Red / 255f, leftGray, ghostReduction);
                        g = ProceduralEffectHelper.Lerp(right.Green / 255f, rightGray, ghostReduction * 0.90f);
                        b = ProceduralEffectHelper.Lerp(right.Blue / 255f, rightGray, ghostReduction);
                        break;
                }

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    (byte)((left.Alpha + right.Alpha) / 2));
            }
        });

        return AnalogEffectHelper.CreateBitmap(source, dstPixels);
    }
}