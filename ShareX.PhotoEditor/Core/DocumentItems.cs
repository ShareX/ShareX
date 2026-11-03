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

namespace ShareX.PhotoEditor.Core;

public sealed record Guide(bool Horizontal, float Position);

public sealed class NoteItem
{
    public SKPoint Position { get; set; }
    public string Author { get; set; } = Environment.UserName;
    public string Text { get; set; } = string.Empty;
    public SKColor Color { get; set; } = new(0xFF, 0xD7, 0x40);
    public NoteItem Clone() => (NoteItem)MemberwiseClone();
}

public sealed class CountItem
{
    public SKPoint Position { get; set; }
    public int Group { get; set; }
    public CountItem Clone() => (CountItem)MemberwiseClone();
}

public sealed class CountGroup
{
    public string Name { get; set; } = "Count Group 1";
    public SKColor Color { get; set; } = new(0x2E, 0x9B, 0xFF);
    public float MarkerSize { get; set; } = 6;
    public float LabelSize { get; set; } = 12;
    public bool Visible { get; set; } = true;
    public CountGroup Clone() => (CountGroup)MemberwiseClone();
}

public sealed class ColorSamplerPoint
{
    public SKPoint Position { get; set; }
    public ColorSamplerPoint Clone() => (ColorSamplerPoint)MemberwiseClone();
}

public sealed class SliceItem
{
    public SKRectI Bounds { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool UserSlice { get; set; } = true;
    public string Url { get; set; } = string.Empty;
    public SliceItem Clone() => (SliceItem)MemberwiseClone();
}

/// <summary>An alpha channel stored with the document (saved selection).</summary>
public sealed class AlphaChannel
{
    public AlphaChannel(string name, PixelSurface mask)
    {
        Name = name;
        Mask = mask;
    }

    public string Name { get; set; }
    public PixelSurface Mask { get; set; }
    public bool Visible { get; set; }
    public AlphaChannel Clone() => new(Name, Mask) { Visible = Visible };
}

public enum ChannelView
{
    Composite,
    Red,
    Green,
    Blue
}
