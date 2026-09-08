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

using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class HighlightAnnotation
{
    /// <summary>
    /// Creates the Avalonia visual for this annotation.
    /// </summary>
    public Control CreateVisual()
    {
        var baseColor = Color.Parse(FillColor ?? "#FFFF00");
        var highlightColor = Color.FromArgb(0x55, baseColor.R, baseColor.G, baseColor.B);

        return new Avalonia.Controls.Shapes.Rectangle
        {
            Fill = new SolidColorBrush(highlightColor),
            Stroke = Brushes.Transparent,
            StrokeThickness = 0,
            Tag = this
        };
    }

    internal Control CreatePreviewVisual()
    {
        Color baseColor = Color.Parse(StrokeColor);
        Color highlightColor = Color.FromArgb(0x55, baseColor.R, baseColor.G, baseColor.B);
        return new Rectangle
        {
            Fill = new SolidColorBrush(highlightColor),
            Stroke = Brushes.Transparent,
            StrokeThickness = 0,
            Tag = this
        };
    }
}