#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using ShareX.HelpersLib;

namespace ShareX.ImageEffectsLib;

public sealed record ImageEffectDefinition(string Category, string Name, Type EffectType)
{
    public ImageEffect Create() => (ImageEffect)Activator.CreateInstance(EffectType)!;
}

public static class ImageEffectCatalog
{
    public static IReadOnlyList<ImageEffectDefinition> All { get; } = Build();

    private static IReadOnlyList<ImageEffectDefinition> Build()
    {
        List<(string Category, Type[] Types)> groups =
        [
            ("Drawings", [typeof(DrawBackground), typeof(DrawBackgroundImage), typeof(DrawBorder), typeof(DrawCheckerboard),
                typeof(DrawImage), typeof(DrawParticles), typeof(DrawTextEx), typeof(DrawText)]),
            ("Manipulations", [typeof(AutoCrop), typeof(Canvas), typeof(Crop), typeof(Flip), typeof(ForceProportions),
                typeof(Resize), typeof(Rotate), typeof(RoundedCorners), typeof(Scale), typeof(Skew)]),
            ("Adjustments", [typeof(Alpha), typeof(BlackWhite), typeof(Brightness), typeof(MatrixColor), typeof(Colorize),
                typeof(Contrast), typeof(Gamma), typeof(Grayscale), typeof(Hue), typeof(Inverse), typeof(Polaroid),
                typeof(ReplaceColor), typeof(Saturation), typeof(SelectiveColor), typeof(Sepia)]),
            ("Filters", [typeof(Blur), typeof(ColorDepth), typeof(MatrixConvolution), typeof(EdgeDetect), typeof(Emboss),
                typeof(GaussianBlur), typeof(Glow), typeof(MeanRemoval), typeof(Outline), typeof(Pixelate), typeof(Reflection),
                typeof(RGBSplit), typeof(Shadow), typeof(Sharpen), typeof(Slice), typeof(Smooth), typeof(TornEdge), typeof(WaveEdge)])
        ];

        return groups.SelectMany(group => group.Types.Select(type =>
            new ImageEffectDefinition(group.Category, type.GetDescription(), type))).ToArray();
    }
}
