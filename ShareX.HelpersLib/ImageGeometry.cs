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
using System.ComponentModel;
using System.Globalization;

namespace ShareX.HelpersLib;

[Flags]
public enum ImageSides
{
    None = 0,
    Top = 1,
    Bottom = 2,
    Left = 4,
    Right = 8
}

public enum ImageOrientation
{
    Horizontal = 0,
    Vertical = 1
}

// Integer margins retain the serialized representation of existing effect presets.
[TypeConverter(typeof(ImageMarginsConverter))]
public struct ImageMargins : IEquatable<ImageMargins>
{
    public static readonly ImageMargins Empty = new(0);
    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
    [Browsable(false)] public readonly int Horizontal => Left + Right;
    [Browsable(false)] public readonly int Vertical => Top + Bottom;
    [Browsable(false)] public readonly int All => Left == Top && Top == Right && Right == Bottom ? Left : -1;

    public ImageMargins(int all) : this(all, all, all, all) { }
    public ImageMargins(int left, int top, int right, int bottom) =>
        (Left, Top, Right, Bottom) = (left, top, right, bottom);
    public readonly bool Equals(ImageMargins other) =>
        Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
    public override readonly bool Equals(object obj) => obj is ImageMargins other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
    public static bool operator ==(ImageMargins left, ImageMargins right) => left.Equals(right);
    public static bool operator !=(ImageMargins left, ImageMargins right) => !left.Equals(right);
}

public sealed class ImageMarginsConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
        destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string text)
        {
            culture ??= CultureInfo.CurrentCulture;
            string[] values = text.Split(culture.TextInfo.ListSeparator, StringSplitOptions.TrimEntries);
            if (values.Length == 1) return new ImageMargins(int.Parse(values[0], culture));
            if (values.Length == 4) return new ImageMargins(int.Parse(values[0], culture), int.Parse(values[1], culture),
                int.Parse(values[2], culture), int.Parse(values[3], culture));
            throw new FormatException("Margins require one or four integer values.");
        }
        return base.ConvertFrom(context, culture, value);
    }
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is ImageMargins margins)
        {
            culture ??= CultureInfo.CurrentCulture;
            return string.Join(culture.TextInfo.ListSeparator + " ", margins.Left.ToString(culture),
                margins.Top.ToString(culture), margins.Right.ToString(culture), margins.Bottom.ToString(culture));
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
