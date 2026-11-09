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

using DrawingColor = System.Drawing.Color;

namespace ShareX.ImageEditor.Integration;

/// <summary>
/// Recently used colors, newest first, shown in the color picker panel.
/// Hosts can set <see cref="Colors"/> to their own saved list, so it is shared with their color picker window and kept between sessions.
/// </summary>
public static class EditorRecentColors
{
    /// <summary>Same limit as the ShareX color picker window, which shares this list.</summary>
    public const int MaxCount = 32;

    public static List<DrawingColor> Colors { get; set; } = new();

    public static void Add(DrawingColor color)
    {
        List<DrawingColor> colors = Colors;
        int argb = color.ToArgb();
        colors.RemoveAll(x => x.ToArgb() == argb);
        colors.Insert(0, DrawingColor.FromArgb(argb));

        if (colors.Count > MaxCount)
        {
            colors.RemoveRange(MaxCount, colors.Count - MaxCount);
        }
    }
}
