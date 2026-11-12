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

/// <summary>A ShareX icon: an SVG drawing on a square grid, scaled to whatever size it is shown at.</summary>
public sealed class VectorIcon
{
    private readonly SvgIconDrawing? _drawing;
    private readonly LegacyIcon? _legacy;

    /// <summary>A modern icon, drawn in the Foreground; the grid is the drawing's viewBox.</summary>
    internal VectorIcon(SvgIconDrawing drawing)
    {
        GridSize = drawing.Width;
        _drawing = drawing;
    }

    /// <summary>A full-colour legacy icon, laid out on the grid of the modern icon it stands in for.</summary>
    internal VectorIcon(double gridSize, LegacyIcon legacy)
    {
        GridSize = gridSize;
        _legacy = legacy;
    }

    /// <summary>The size of the grid the icon is drawn on, which is also the size it is sharpest at.</summary>
    public double GridSize { get; }

    /// <summary>
    /// Draws the icon in <paramref name="foreground"/>, centered in the largest square that fits <paramref name="bounds"/>.
    /// <paramref name="darkTheme"/> picks the variant of a legacy icon made for dark backgrounds; when null, it is
    /// picked by how light <paramref name="foreground"/> is.
    /// </summary>
    public void Draw(DrawingContext context, Rect bounds, IBrush foreground, bool? darkTheme = null)
    {
        if (_legacy != null)
        {
            _legacy.Draw(context, bounds, foreground, darkTheme);
        }
        else
        {
            _drawing!.Draw(context, bounds, foreground);
        }
    }
}
