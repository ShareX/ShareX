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

using SkiaSharp;

namespace ShareX.AvaloniaUI.Imaging;

/// <summary>Resolves saved font names to installed families without changing the saved settings.</summary>
public static class FontFamilyResolver
{
    private static readonly Lazy<HashSet<string>> InstalledFamilies = new(() =>
        new HashSet<string>(SKFontManager.Default.FontFamilies, StringComparer.OrdinalIgnoreCase));

    private static readonly string[] SansFamilies =
        ["Noto Sans", "DejaVu Sans", "Liberation Sans", "Helvetica Neue", "Helvetica", "Arial"];
    private static readonly string[] ArialFamilies =
        ["Liberation Sans", "Helvetica", "Helvetica Neue", "Noto Sans", "DejaVu Sans"];
    private static readonly string[] MonospaceFamilies =
        ["Noto Sans Mono", "DejaVu Sans Mono", "Liberation Mono", "Menlo", "Monaco", "Courier New"];

    public static string Resolve(string? requestedFamily) =>
        Resolve(requestedFamily, InstalledFamilies.Value, SKTypeface.Default.FamilyName);

    internal static string Resolve(string? requestedFamily, HashSet<string> installedFamilies, string defaultFamily)
    {
        string family = string.IsNullOrWhiteSpace(requestedFamily) ? "Segoe UI" : requestedFamily.Trim();
        if (installedFamilies.TryGetValue(family, out string? installedFamily))
        {
            return installedFamily;
        }

        string[] fallbacks = family.Equals("Arial", StringComparison.OrdinalIgnoreCase) ? ArialFamilies :
            family.Equals("Consolas", StringComparison.OrdinalIgnoreCase) ||
            family.Equals("Courier New", StringComparison.OrdinalIgnoreCase) ? MonospaceFamilies : SansFamilies;

        foreach (string fallback in fallbacks)
        {
            if (installedFamilies.TryGetValue(fallback, out installedFamily))
            {
                return installedFamily;
            }
        }

        return defaultFamily;
    }
}
