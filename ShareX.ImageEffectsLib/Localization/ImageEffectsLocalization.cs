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
