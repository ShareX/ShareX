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

using System;

namespace ShareX.Platform;

/// <summary>A toolkit neutral point in physical pixels on the virtual desktop.</summary>
public readonly record struct PlatformPoint(int X, int Y);

/// <summary>A toolkit neutral size in physical pixels.</summary>
public readonly record struct PlatformSize(int Width, int Height);

/// <summary>A toolkit neutral rectangle in physical pixels on the virtual desktop.</summary>
public readonly record struct PlatformRectangle(int X, int Y, int Width, int Height)
{
    public static PlatformRectangle Empty => default;

    public int Left => X;

    public int Top => Y;

    public int Right => X + Width;

    public int Bottom => Y + Height;

    public bool IsEmpty => Width <= 0 || Height <= 0;

    public PlatformSize Size => new PlatformSize(Width, Height);

    public static PlatformRectangle FromLTRB(int left, int top, int right, int bottom) => new PlatformRectangle(left, top, right - left, bottom - top);

    public bool Contains(PlatformPoint point) => point.X >= X && point.X < Right && point.Y >= Y && point.Y < Bottom;

    public PlatformRectangle Intersect(PlatformRectangle other)
    {
        int left = Math.Max(X, other.X);
        int top = Math.Max(Y, other.Y);
        int right = Math.Min(Right, other.Right);
        int bottom = Math.Min(Bottom, other.Bottom);

        return right > left && bottom > top ? FromLTRB(left, top, right, bottom) : Empty;
    }

    public PlatformRectangle Union(PlatformRectangle other)
    {
        if (IsEmpty) return other;
        if (other.IsEmpty) return this;

        return FromLTRB(Math.Min(X, other.X), Math.Min(Y, other.Y), Math.Max(Right, other.Right), Math.Max(Bottom, other.Bottom));
    }

    public PlatformRectangle Offset(int dx, int dy) => new PlatformRectangle(X + dx, Y + dy, Width, Height);
}
