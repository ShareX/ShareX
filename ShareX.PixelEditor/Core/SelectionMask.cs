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

namespace ShareX.PixelEditor.Core;

public enum SelectionMode
{
    Replace,
    Add,
    Subtract,
    Intersect
}

/// <summary>A hard-edged selection the size of the sprite. Masks are immutable; every operation returns a new one.</summary>
public sealed class SelectionMask
{
    private List<(int X0, int Y0, int X1, int Y1)>? _outline;

    public SelectionMask(int width, int height, bool[] data)
    {
        Width = width;
        Height = height;
        Data = data;
        Bounds = ComputeBounds();
    }

    public int Width { get; }

    public int Height { get; }

    public bool[] Data { get; }

    /// <summary>The smallest rectangle holding every selected pixel; empty when nothing is selected.</summary>
    public SKRectI Bounds { get; }

    public bool IsEmpty => Bounds.IsEmpty;

    public bool this[int x, int y] => (uint)x < (uint)Width && (uint)y < (uint)Height && Data[y * Width + x];

    private SKRectI ComputeBounds()
    {
        int minX = Width, minY = Height, maxX = -1, maxY = -1;
        for (int y = 0; y < Height; y++)
        {
            int row = y * Width;
            for (int x = 0; x < Width; x++)
            {
                if (!Data[row + x]) continue;
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                maxY = y;
            }
        }
        return maxX < 0 ? SKRectI.Empty : new SKRectI(minX, minY, maxX + 1, maxY + 1);
    }

    public static SelectionMask All(int width, int height)
    {
        bool[] data = new bool[width * height];
        Array.Fill(data, true);
        return new SelectionMask(width, height, data);
    }

    public static SelectionMask FromRect(int width, int height, SKRectI rect)
    {
        bool[] data = new bool[width * height];
        int x0 = Math.Max(0, rect.Left), y0 = Math.Max(0, rect.Top), x1 = Math.Min(width, rect.Right), y1 = Math.Min(height, rect.Bottom);
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++)
                data[y * width + x] = true;
        return new SelectionMask(width, height, data);
    }

    public static SelectionMask FromEllipse(int width, int height, SKRectI rect)
    {
        bool[] data = new bool[width * height];
        if (rect.Width > 0 && rect.Height > 0)
        {
            Raster.Ellipse(rect.Left, rect.Top, rect.Right - 1, rect.Bottom - 1, true, (x, y) =>
            {
                if ((uint)x < (uint)width && (uint)y < (uint)height) data[y * width + x] = true;
            });
        }
        return new SelectionMask(width, height, data);
    }

    public static SelectionMask FromPolygon(int width, int height, IReadOnlyList<SKPointI> points)
    {
        bool[] data = new bool[width * height];
        Raster.FillPolygon(points, (x, y) =>
        {
            if ((uint)x < (uint)width && (uint)y < (uint)height) data[y * width + x] = true;
        });
        return new SelectionMask(width, height, data);
    }

    /// <summary>Combines a new selection with an existing one. Returns null when the result selects nothing.</summary>
    public static SelectionMask? Combine(SelectionMask? current, SelectionMask added, SelectionMode mode)
    {
        if (current == null || mode == SelectionMode.Replace)
        {
            if (mode is SelectionMode.Subtract or SelectionMode.Intersect) return current == null ? null : Combine(current, added, mode, true);
            return added.IsEmpty ? null : added;
        }
        return Combine(current, added, mode, true);
    }

    private static SelectionMask? Combine(SelectionMask current, SelectionMask added, SelectionMode mode, bool _)
    {
        bool[] data = new bool[current.Data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            bool a = current.Data[i], b = added.Data[i];
            data[i] = mode switch
            {
                SelectionMode.Add => a || b,
                SelectionMode.Subtract => a && !b,
                SelectionMode.Intersect => a && b,
                _ => b
            };
        }
        SelectionMask result = new(current.Width, current.Height, data);
        return result.IsEmpty ? null : result;
    }

    public SelectionMask? Invert()
    {
        bool[] data = new bool[Data.Length];
        for (int i = 0; i < data.Length; i++) data[i] = !Data[i];
        SelectionMask result = new(Width, Height, data);
        return result.IsEmpty ? null : result;
    }

    public SelectionMask? Offset(int dx, int dy)
    {
        bool[] data = new bool[Data.Length];
        for (int y = Bounds.Top; y < Bounds.Bottom; y++)
        {
            int ny = y + dy;
            if ((uint)ny >= (uint)Height) continue;
            for (int x = Bounds.Left; x < Bounds.Right; x++)
            {
                int nx = x + dx;
                if ((uint)nx < (uint)Width && Data[y * Width + x]) data[ny * Width + nx] = true;
            }
        }
        SelectionMask result = new(Width, Height, data);
        return result.IsEmpty ? null : result;
    }

    /// <summary>Expands (positive) or contracts (negative) the selection by a number of pixels.</summary>
    public SelectionMask? Grow(int amount, bool diagonal = false)
    {
        bool[] data = (bool[])Data.Clone();
        bool grow = amount > 0;
        for (int step = 0; step < Math.Abs(amount); step++)
        {
            bool[] next = (bool[])data.Clone();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int i = y * Width + x;
                    if (data[i] == grow) continue;
                    bool hit = false;
                    for (int oy = -1; oy <= 1 && !hit; oy++)
                    {
                        for (int ox = -1; ox <= 1; ox++)
                        {
                            if (ox == 0 && oy == 0 || !diagonal && ox != 0 && oy != 0) continue;
                            int nx = x + ox, ny = y + oy;
                            // Outside the sprite counts as unselected, so contracting also eats in from the edges.
                            bool value = (uint)nx < (uint)Width && (uint)ny < (uint)Height && data[ny * Width + nx];
                            if (value == grow)
                            {
                                hit = true;
                                break;
                            }
                        }
                    }
                    if (hit) next[i] = grow;
                }
            }
            data = next;
        }
        SelectionMask result = new(Width, Height, data);
        return result.IsEmpty ? null : result;
    }

    /// <summary>Keeps only a band of the given width along the inside of the selection edge.</summary>
    public SelectionMask? Border(int width)
    {
        SelectionMask? inner = Grow(-width);
        if (inner == null) return this;
        return Combine(this, inner, SelectionMode.Subtract);
    }

    /// <summary>The same selection on a sprite of another size, shifted by an offset.</summary>
    public SelectionMask? Resized(int width, int height, int dx, int dy)
    {
        bool[] data = new bool[width * height];
        for (int y = Bounds.Top; y < Bounds.Bottom; y++)
        {
            int ny = y + dy;
            if ((uint)ny >= (uint)height) continue;
            for (int x = Bounds.Left; x < Bounds.Right; x++)
            {
                int nx = x + dx;
                if ((uint)nx < (uint)width && Data[y * Width + x]) data[ny * width + nx] = true;
            }
        }
        SelectionMask result = new(width, height, data);
        return result.IsEmpty ? null : result;
    }

    /// <summary>The selection edge as merged horizontal and vertical segments on pixel boundaries.</summary>
    public IReadOnlyList<(int X0, int Y0, int X1, int Y1)> Outline()
    {
        if (_outline != null) return _outline;
        List<(int, int, int, int)> segments = [];
        // Horizontal edges: between row y-1 and row y.
        for (int y = Bounds.Top; y <= Bounds.Bottom; y++)
        {
            int start = -1;
            for (int x = Bounds.Left; x <= Bounds.Right; x++)
            {
                bool edge = x < Bounds.Right && this[x, y - 1] != this[x, y];
                if (edge && start < 0) start = x;
                else if (!edge && start >= 0)
                {
                    segments.Add((start, y, x, y));
                    start = -1;
                }
            }
        }
        for (int x = Bounds.Left; x <= Bounds.Right; x++)
        {
            int start = -1;
            for (int y = Bounds.Top; y <= Bounds.Bottom; y++)
            {
                bool edge = y < Bounds.Bottom && this[x - 1, y] != this[x, y];
                if (edge && start < 0) start = y;
                else if (!edge && start >= 0)
                {
                    segments.Add((x, start, x, y));
                    start = -1;
                }
            }
        }
        _outline = segments;
        return segments;
    }
}
