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

namespace ShareX.Icons;

/// <summary>Finds a ShareX icon by its key, a constant of one of the icon sets, such as <see cref="AppIcons"/>.</summary>
public static class IconCatalog
{
    private static Dictionary<string, VectorIcon>? _icons;

    /// <summary>Returns the icon for <paramref name="key"/>, or null if it is not a ShareX icon key, such as a font glyph.</summary>
    public static VectorIcon? Find(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        // Built on first use: geometries need the platform's render interface.
        _icons ??= CreateIcons();
        return _icons.GetValueOrDefault(key);
    }

    private static Dictionary<string, VectorIcon> CreateIcons()
    {
        Dictionary<string, VectorIcon> icons = new(StringComparer.Ordinal);

        IEnumerable<IconGroup> groups =
        [
            .. ImageEditorIconDefinitions.CreateGroups(),
            .. PhotoEditorIconDefinitions.CreateGroups(),
            .. AppIconDefinitions.CreateGroups(),
            .. ImageEffectIconDefinitions.CreateGroups()
        ];

        foreach (IconGroup group in groups)
        {
            foreach ((string key, VectorIcon icon) in group)
            {
                icons.Add(key, icon);
            }
        }

        return icons;
    }
}
