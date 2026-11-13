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

using Avalonia.Media;
using System.Diagnostics.CodeAnalysis;

namespace ShareX.ImageEditor.Presentation.Helpers;

internal static class FontFamilyHelper
{
    private static string? _cachedDefaultFontFamily;

    public static string GetDefaultSystemFontFamily()
    {
        if (_cachedDefaultFontFamily != null)
        {
            return _cachedDefaultFontFamily;
        }

        if (OperatingSystem.IsWindows())
        {
            _cachedDefaultFontFamily = "Segoe UI";
            return _cachedDefaultFontFamily;
        }

        if (OperatingSystem.IsMacOS())
        {
            _cachedDefaultFontFamily = "San Francisco";
            return _cachedDefaultFontFamily;
        }

        // Linux preferred fonts
        string[] preferred = ["Ubuntu", "Ubuntu Sans", "Noto Sans", "DejaVu Sans", "Liberation Sans", "Cantarell"];
        try
        {
            var systemFonts = FontManager.Current.SystemFonts;
            foreach (var pref in preferred)
            {
                if (systemFonts.Any(f => string.Equals(f.Name, pref, StringComparison.OrdinalIgnoreCase)))
                {
                    _cachedDefaultFontFamily = pref;
                    return _cachedDefaultFontFamily;
                }
            }

            var first = systemFonts.FirstOrDefault()?.Name;
            if (!string.IsNullOrWhiteSpace(first))
            {
                _cachedDefaultFontFamily = first;
                return _cachedDefaultFontFamily;
            }
        }
        catch
        {
        }

        _cachedDefaultFontFamily = "Ubuntu";
        return _cachedDefaultFontFamily;
    }

    public static string ResolveFontFamily(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return GetDefaultSystemFontFamily();
        }

        if (OperatingSystem.IsLinux() && string.Equals(name, "Segoe UI", StringComparison.OrdinalIgnoreCase))
        {
            return GetDefaultSystemFontFamily();
        }

        return name;
    }

    public static bool TryCreateFontFamily(string? name, [NotNullWhen(true)] out FontFamily? fontFamily)
    {
        fontFamily = null;
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        try
        {
            var candidate = new FontFamily(name);

            // Avalonia defers system font name normalization until text is measured.
            // Check the names without loading any glyphs or font files.
            new Typeface(candidate).Normalize(out _);
            for (int i = 1; i < candidate.FamilyNames.Count; i++)
            {
                new Typeface(candidate.FamilyNames[i]).Normalize(out _);
            }

            fontFamily = candidate;
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException)
        {
            return false;
        }
    }
}
