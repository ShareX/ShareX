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

using System.Drawing;

namespace ShareX.Tools;

public enum KeyHighlighterPosition
{
    BottomLeft,
    BottomCenter,
    BottomRight,
    TopLeft,
    TopRight
}

/// <summary>How pressed keys are shown in recordings.</summary>
/// <param name="ShowAllKeys">Also show typed text. Off shows only shortcuts and keys such as Enter, Esc or F1.</param>
public sealed record KeyHighlighterSettings(bool ShowAllKeys, KeyHighlighterPosition Position, int FontSize, Color TextColor,
    double DisplaySeconds);

/// <summary>Premultiplied BGRA pixels, top-down, placed at X, Y in a frame. Version changes when the pixels change.</summary>
public sealed record KeyHighlighterImage(byte[] Pixels, int Width, int Height, int X, int Y, int Version);
