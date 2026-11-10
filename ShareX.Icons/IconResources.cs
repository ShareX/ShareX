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

using System.Collections.Concurrent;
using System.Reflection;

namespace ShareX.Icons;

/// <summary>The SVG files embedded in this assembly, from the Modern and Legacy folders.</summary>
internal static class IconResources
{
    private static readonly Assembly Assembly = typeof(IconResources).Assembly;
    private static readonly HashSet<string> Names = new(Assembly.GetManifestResourceNames(), StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, SvgIconDrawing?> Drawings = new(StringComparer.Ordinal);

    /// <summary>
    /// The drawing of the file <paramref name="name"/>.svg in <paramref name="folder"/>, read once and kept, or null
    /// if there is no such file.
    /// </summary>
    public static SvgIconDrawing? Find(string folder, string name) => Drawings.GetOrAdd($"ShareX.Icons.{folder}.{name}.svg", static resource =>
    {
        if (!Names.Contains(resource))
        {
            return null;
        }

        using Stream stream = Assembly.GetManifestResourceStream(resource)!;
        return SvgIconDrawing.Load(stream);
    });
}
