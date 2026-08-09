using ShareX.ImageEditor.Core.ImageEffects;
using System;

namespace ShareX.ImageEditor.Localization;

internal static class EffectBrowserLocalization
{
    private const string CategoryPrefix = "EffectBrowserPanel_Category_";
    private const string EffectPrefix = "EffectBrowserPanel_Effect_";
    private const string DialogSuffix = "...";

    public static string GetCategoryName(ImageEffectCategory category)
    {
        string fallback = category.ToString();
        return Strings.ResourceManager.GetString(CategoryPrefix + fallback, Strings.Culture) ?? fallback;
    }

    public static string GetEffectBrowserLabel(string effectId, string fallbackBrowserLabel)
    {
        bool opensDialog = fallbackBrowserLabel.EndsWith(DialogSuffix, StringComparison.Ordinal);
        string fallbackName = opensDialog ? fallbackBrowserLabel[..^DialogSuffix.Length] : fallbackBrowserLabel;
        string localizedName = Strings.ResourceManager.GetString(EffectPrefix + effectId, Strings.Culture) ?? fallbackName;

        return opensDialog ? localizedName + DialogSuffix : localizedName;
    }
}
