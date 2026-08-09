#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

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
