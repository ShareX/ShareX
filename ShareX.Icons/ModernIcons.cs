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

namespace ShareX.Icons;

/// <summary>
/// The icons of the <see cref="IconPack.Modern"/> pack: line art drawn in the Foreground, as SVG files in the Modern
/// folder named after their key (app/camera is app.camera.svg), embedded in this assembly.
/// </summary>
/// <remarks>
/// The interface icons (<see cref="AppIcons"/>, <see cref="ImageEditorIcons"/>, <see cref="ImageEffectIcons"/>) are
/// drawn on a 24x24 grid with 2px strokes with round caps and joins, content within 2..22. Related icons share their
/// parts (page, folder, cloud, image, frame, brackets), and variants add a glyph inside the part or an overlay, such
/// as a badge in the bottom-right corner or a slash, which a mask cuts a gap for in the shapes under it. Line work is
/// <c>currentColor</c>; shading is <c>currentColor</c> at partial opacity, so it becomes accent shades on the image
/// editor toolbar. A fixed color is only for what a tool puts on the image, such as the highlighter ink.
/// <para>
/// The <see cref="PhotoEditorIcons"/> are pixel line art drawn on the pixel grid each group is shown at (tools 20px,
/// interface icons 16px, badges 12px and controls 10px) with 1px strokes on pixel centers, so they are sharp at their
/// native size. The title bar caption buttons are fine line art on a 16px grid.
/// </para>
/// <para>
/// The <see cref="PixelEditorIcons"/> are pixel art on the same grids (tools 20px, interface icons 16px, small
/// buttons 12px and controls 10px): every icon is built from whole grid pixels, solid in <c>currentColor</c> with
/// shading at partial opacity, and has no curves or strokes.
/// </para>
/// </remarks>
internal static class ModernIcons
{
    private static readonly ConcurrentDictionary<string, VectorIcon?> Icons = new(StringComparer.Ordinal);

    /// <summary>The icon with key <paramref name="key"/>, or null if it is not a ShareX icon key.</summary>
    public static VectorIcon? Find(string key) => Icons.GetOrAdd(key, static key =>
        IconResources.Find("Modern", key.Replace('/', '.')) is SvgIconDrawing drawing ? new VectorIcon(drawing) : null);
}
