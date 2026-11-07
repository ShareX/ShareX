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
using Avalonia;
using Avalonia.Media;

namespace ShareX.Icons;

/// <summary>
/// The icons of the <see cref="IconPack.Legacy"/> pack: the ShareX 21 icons, mostly from the Fugue icon set by
/// Yusuke Kamiyamane (CC BY 3.0), redrawn as SVG files in the Legacy folder and embedded in this assembly.
/// </summary>
internal static class LegacyIcons
{
    private const string ResourcePrefix = "ShareX.Icons.Legacy.";
    private const string DarkSuffix = "-white";

    private static readonly Assembly Assembly = typeof(LegacyIcons).Assembly;
    private static readonly HashSet<string> Resources = new(Assembly.GetManifestResourceNames(), StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, SvgIconDrawing?> Drawings = new(StringComparer.Ordinal);

    /// <summary>
    /// The legacy version of the icon with key <paramref name="key"/>, laid out like <paramref name="modern"/>, or
    /// null if ShareX 21 had no icon for it.
    /// </summary>
    public static VectorIcon? Find(string key, VectorIcon modern)
    {
        if (!LegacyIconMap.Names.TryGetValue(key, out string? name) || GetDrawing(name) is not SvgIconDrawing light)
        {
            return null;
        }

        return new VectorIcon(modern.GridSize, new LegacyIcon(light, GetDrawing(name + DarkSuffix)));
    }

    private static SvgIconDrawing? GetDrawing(string name) => Drawings.GetOrAdd(name, static name =>
    {
        string resource = ResourcePrefix + name + ".svg";

        if (!Resources.Contains(resource))
        {
            return null;
        }

        using Stream stream = Assembly.GetManifestResourceStream(resource)!;
        return SvgIconDrawing.Load(stream);
    });
}

/// <summary>
/// A legacy icon, with the version ShareX 21 used on dark backgrounds when it had one, chosen when the icon is
/// shown in a dark theme, or, when the theme is not known, drawn in a light Foreground.
/// </summary>
internal sealed class LegacyIcon(SvgIconDrawing light, SvgIconDrawing? dark)
{
    public void Draw(DrawingContext context, Rect bounds, IBrush foreground, bool? darkTheme)
    {
        SvgIconDrawing drawing = dark != null && (darkTheme ?? IsLight(foreground)) ? dark : light;
        drawing.Draw(context, bounds, foreground);
    }

    private static bool IsLight(IBrush brush)
    {
        if (brush is not ISolidColorBrush { Color: Color color })
        {
            return false;
        }

        double luminance = (0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B) / 255;
        return luminance > 0.5;
    }
}
