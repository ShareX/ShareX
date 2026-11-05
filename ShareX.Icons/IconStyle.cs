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

namespace ShareX.Icons;

/// <summary>How the strokes of an icon group are drawn: the default width and the shape of line ends and corners.</summary>
internal sealed class IconStyle(double strokeWidth, PenLineCap lineCap, PenLineJoin lineJoin)
{
    /// <summary>Line art for the interface icons: 2px strokes with round caps and joins.</summary>
    public static readonly IconStyle Rounded = new(2, PenLineCap.Round, PenLineJoin.Round);

    /// <summary>Pixel line art: 1px strokes with square ends and corners, sharp at the grid's own size.</summary>
    public static readonly IconStyle Pixel = new(1, PenLineCap.Flat, PenLineJoin.Miter);

    /// <summary>Fine line art for small icons shown at their grid size: 1px strokes with square ends and round corners.</summary>
    public static readonly IconStyle Fine = new(1, PenLineCap.Flat, PenLineJoin.Round);

    public double StrokeWidth { get; } = strokeWidth;

    public PenLineCap LineCap { get; } = lineCap;

    public PenLineJoin LineJoin { get; } = lineJoin;
}
