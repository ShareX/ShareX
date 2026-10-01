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

namespace ShareX.HelpersLib
{
    /// <summary>
    /// Space on each side of a rectangle. Replaces System.Windows.Forms.Padding in portable code and serializes the same way
    /// ("left, top, right, bottom"), so settings and image effect presets saved by older versions still load.
    /// </summary>
    [TypeConverter(typeof(InsetsConverter))]
    public struct Insets : IEquatable<Insets>
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public Insets(int all) : this(all, all, all, all)
        {
        }

        public Insets(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>The common value of every side, or -1 when they differ, like Padding.All.</summary>
        public int All
        {
            get => Left == Top && Top == Right && Right == Bottom ? Left : -1;
            set => Left = Top = Right = Bottom = value;
        }

        public int Horizontal => Left + Right;

        public int Vertical => Top + Bottom;

        public static Insets Empty => default;

        public bool Equals(Insets other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;

        public override bool Equals(object obj) => obj is Insets other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

        public static bool operator ==(Insets a, Insets b) => a.Equals(b);

        public static bool operator !=(Insets a, Insets b) => !a.Equals(b);

        public override string ToString() => FormattableString.Invariant($"{Left}, {Top}, {Right}, {Bottom}");

        public static Insets Parse(string text)
        {
            string[] parts = text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return new Insets(int.Parse(parts[0], CultureInfo.InvariantCulture));
            }

            if (parts.Length != 4)
            {
                throw new FormatException($"'{text}' is not four comma separated numbers.");
            }

            return new Insets(int.Parse(parts[0], CultureInfo.InvariantCulture), int.Parse(parts[1], CultureInfo.InvariantCulture),
                int.Parse(parts[2], CultureInfo.InvariantCulture), int.Parse(parts[3], CultureInfo.InvariantCulture));
        }
    }

    public sealed class InsetsConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            value is string text ? Insets.Parse(text) : base.ConvertFrom(context, culture, value);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            destinationType == typeof(string) && value is Insets insets ? insets.ToString() : base.ConvertTo(context, culture, value, destinationType);
    }
}
