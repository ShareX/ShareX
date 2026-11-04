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

/// <summary>A ShareX icon: shapes drawn on a square grid, scaled to whatever size it is shown at.</summary>
public sealed class VectorIcon
{
    private readonly IconStyle _style;
    private readonly IconShape[] _shapes;

    internal VectorIcon(double gridSize, IconStyle style, IconShape[] shapes)
    {
        GridSize = gridSize;
        _style = style;
        _shapes = shapes;
    }

    /// <summary>The size of the grid the icon is drawn on, which is also the size it is sharpest at.</summary>
    public double GridSize { get; }

    /// <summary>Draws the icon in <paramref name="foreground"/>, centered in the largest square that fits <paramref name="bounds"/>.</summary>
    public void Draw(DrawingContext context, Rect bounds, IBrush foreground)
    {
        double size = Math.Min(bounds.Width, bounds.Height);
        double scale = size / GridSize;
        Matrix transform = Matrix.CreateScale(scale, scale) *
            Matrix.CreateTranslation(bounds.X + (bounds.Width - size) / 2, bounds.Y + (bounds.Height - size) / 2);

        using (context.PushTransform(transform))
        {
            foreach (IconShape shape in _shapes)
            {
                shape.Draw(context, _style, foreground);
            }
        }
    }
}
