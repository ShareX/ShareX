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

using ShareX.ImageEffectsLib.Localization;

namespace ShareX.ImageEffectsLib;

public sealed record ImageEffectDefinition(string CategoryResourceKey, Type EffectType)
{
    public string Category => Strings.ResourceManager.GetString(CategoryResourceKey, Strings.Culture) ?? CategoryResourceKey;
    public string Name => ImageEffectsLocalization.GetEffectName(EffectType);
    public ImageEffect Create() => (ImageEffect)Activator.CreateInstance(EffectType)!;
}

public static class ImageEffectCatalog
{
    public static IReadOnlyList<ImageEffectDefinition> All { get; } = Build();

    private static IReadOnlyList<ImageEffectDefinition> Build()
    {
        List<(string CategoryResourceKey, Type[] Types)> groups =
        [
            (nameof(Strings.ImageEffectCategory_Drawings), [typeof(DrawBackground), typeof(DrawBackgroundImage), typeof(DrawBorder), typeof(DrawCheckerboard),
                typeof(DrawImage), typeof(DrawParticles), typeof(DrawTextEx), typeof(DrawText)]),
            (nameof(Strings.ImageEffectCategory_Manipulations), [typeof(AutoCrop), typeof(Canvas), typeof(Crop), typeof(Flip), typeof(ForceProportions),
                typeof(Resize), typeof(Rotate), typeof(RoundedCorners), typeof(Scale), typeof(Skew)]),
            (nameof(Strings.ImageEffectCategory_Adjustments), [typeof(Alpha), typeof(BlackWhite), typeof(Brightness), typeof(MatrixColor), typeof(Colorize),
                typeof(Contrast), typeof(Gamma), typeof(Grayscale), typeof(Hue), typeof(Inverse), typeof(Polaroid),
                typeof(ReplaceColor), typeof(Saturation), typeof(SelectiveColor), typeof(Sepia)]),
            (nameof(Strings.ImageEffectCategory_Filters), [typeof(Blur), typeof(ColorDepth), typeof(MatrixConvolution), typeof(EdgeDetect), typeof(Emboss),
                typeof(GaussianBlur), typeof(Glow), typeof(MeanRemoval), typeof(Outline), typeof(Pixelate), typeof(Reflection),
                typeof(RGBSplit), typeof(Shadow), typeof(Sharpen), typeof(Slice), typeof(Smooth), typeof(TornEdge), typeof(WaveEdge)])
        ];

        return groups.SelectMany(group => group.Types.Select(type =>
            new ImageEffectDefinition(group.CategoryResourceKey, type))).ToArray();
    }
}
