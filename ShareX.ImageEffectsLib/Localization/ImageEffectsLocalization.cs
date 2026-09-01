using ShareX.HelpersLib;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ShareX.ImageEffectsLib.Localization;

internal static class ImageEffectsLocalization
{
    public static string GetEffectName(Type effectType)
    {
        string fallback = effectType.GetCustomAttribute<DescriptionAttribute>()?.Description ?? Helpers.GetProperName(effectType.Name);
        return Get("ImageEffect_" + effectType.Name, fallback);
    }

    public static string GetPropertyName(PropertyDescriptor property)
    {
        return Get("ImageEffectProperty_" + property.Name, Helpers.GetProperName(property.DisplayName));
    }

    public static string GetPropertyDescription(Type effectType, PropertyDescriptor property)
    {
        if (string.IsNullOrWhiteSpace(property.Description))
        {
            return string.Empty;
        }

        return Get($"ImageEffectPropertyDescription_{effectType.Name}_{property.Name}", property.Description);
    }

    public static string GetEnumValue(Type enumType, object value)
    {
        string name = Enum.GetName(enumType, value) ?? value.ToString() ?? string.Empty;
        FieldInfo? field = enumType.GetField(name);
        string fallback = field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? Helpers.GetProperName(name);
        return Get($"ImageEffectEnum_{Sanitize(enumType.Name)}_{name}", fallback);
    }

    private static string Get(string key, string fallback)
    {
        return Strings.ResourceManager.GetString(key, Strings.Culture) ?? fallback;
    }

    private static string Sanitize(string value)
    {
        return Regex.Replace(value, "[^A-Za-z0-9]+", "_").Trim('_');
    }
}
