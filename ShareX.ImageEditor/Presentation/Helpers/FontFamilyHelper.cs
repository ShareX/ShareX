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
