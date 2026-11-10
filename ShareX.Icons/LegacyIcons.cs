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

using Avalonia;
using Avalonia.Media;

namespace ShareX.Icons;

/// <summary>
/// The icons of the <see cref="IconPack.Legacy"/> pack: full-colour icons, as SVG files in the Legacy folder named
/// after their key like the <see cref="ModernIcons"/> (app/camera is app.camera.svg), embedded in this assembly. A
/// file with the -white suffix (app.camera-white.svg) is the version for dark backgrounds. Every key has a legacy
/// icon except the <see cref="IsModernOnly">modern-only</see> ones; see README.md in this project.
/// </summary>
internal static class LegacyIcons
{
    private const string DarkSuffix = "-white";

    // The main window's title bar buttons match the window chrome in both packs.
    private static readonly HashSet<string> ModernOnlyKeys = new(StringComparer.Ordinal)
    {
        AppIcons.CaptionMoon,
        AppIcons.CaptionSun,
        AppIcons.CaptionPin,
        AppIcons.CaptionUnpin,
        AppIcons.CaptionMinimize,
        AppIcons.CaptionMaximize,
        AppIcons.CaptionRestore,
        AppIcons.CaptionClose
    };

    /// <summary>
    /// Whether the icon with key <paramref name="key"/> always uses its modern icon, so the Legacy folder has no
    /// file for it, and one added there is not used: the main window's title bar buttons, and all of the photo
    /// editor, which keeps its own look in both packs.
    /// </summary>
    public static bool IsModernOnly(string key) =>
        ModernOnlyKeys.Contains(key) || key.StartsWith(PhotoEditorIcons.Prefix, StringComparison.Ordinal);

    /// <summary>
    /// The legacy version of the icon with key <paramref name="key"/>, laid out like <paramref name="modern"/>, or
    /// null if the icon is modern-only or the Legacy folder has no icon for it.
    /// </summary>
    public static VectorIcon? Find(string key, VectorIcon modern)
    {
        string name = key.Replace('/', '.');

        if (IsModernOnly(key) || GetDrawing(name) is not SvgIconDrawing light)
        {
            return null;
        }

        return new VectorIcon(modern.GridSize, new LegacyIcon(light, GetDrawing(name + DarkSuffix)));
    }

    private static SvgIconDrawing? GetDrawing(string name) => IconResources.Find("Legacy", name);
}

/// <summary>
/// A legacy icon, with its version for dark backgrounds when it has one, chosen when the icon is shown in a dark
/// theme, or, when the theme is not known, drawn in a light Foreground.
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
