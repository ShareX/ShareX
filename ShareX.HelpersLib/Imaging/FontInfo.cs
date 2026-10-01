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

using SkiaSharp;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// A font description that works on every OS. It reads and writes the same text as GDI+'s FontConverter
    /// ("Arial, 11.25pt" or "Arial, 36pt, style=Bold, Italic"), so fonts saved by older versions keep loading.
    /// </summary>
    [TypeConverter(typeof(FontInfoConverter))]
    public sealed class FontInfo : IEquatable<FontInfo>
    {
        public FontInfo()
        {
        }

        public FontInfo(string family, float sizeInPoints, bool bold = false, bool italic = false, bool underline = false, bool strikeout = false)
        {
            Family = family;
            SizeInPoints = sizeInPoints;
            Bold = bold;
            Italic = italic;
            Underline = underline;
            Strikeout = strikeout;
        }

        public string Family { get; set; } = "Arial";

        public float SizeInPoints { get; set; } = 11.25f;

        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public bool Underline { get; set; }

        public bool Strikeout { get; set; }

        /// <summary>Size in pixels at 96 DPI, which is what GDI+ drew into bitmaps with.</summary>
        public float SizeInPixels => SizeInPoints * 96f / 72f;

        public SKTypeface CreateTypeface() =>
            SKTypeface.FromFamilyName(Family, Bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal, SKFontStyleWidth.Normal,
                Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright) ?? SKTypeface.Default;

        public SKFont CreateFont() => new SKFont(CreateTypeface(), SizeInPixels) { Edging = SKFontEdging.Antialias, Subpixel = true };

        public override string ToString()
        {
            string text = FormattableString.Invariant($"{Family}, {SizeInPoints}pt");
            string[] styles = new[] { Bold ? "Bold" : null, Italic ? "Italic" : null, Underline ? "Underline" : null, Strikeout ? "Strikeout" : null }
                .Where(x => x != null).ToArray();

            return styles.Length > 0 ? text + ", style=" + string.Join(", ", styles) : text;
        }

        public static FontInfo Parse(string text)
        {
            FontInfo font = new FontInfo();
            string[] parts = text.Split(',', StringSplitOptions.TrimEntries);

            if (parts.Length > 0 && parts[0].Length > 0)
            {
                font.Family = parts[0];
            }

            if (parts.Length > 1)
            {
                string size = parts[1];
                string number = new string(size.TakeWhile(c => char.IsDigit(c) || c == '.').ToArray());
                string unit = size.Substring(number.Length).Trim().ToLowerInvariant();

                if (float.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                {
                    font.SizeInPoints = unit switch
                    {
                        "px" => value * 72f / 96f,
                        "in" => value * 72f,
                        "mm" => value * 72f / 25.4f,
                        "doc" or "document" => value * 72f / 300f,
                        "world" or "display" => value * 72f / 96f,
                        _ => value
                    };
                }
            }

            foreach (string part in parts.Skip(2))
            {
                string style = part.StartsWith("style=", StringComparison.OrdinalIgnoreCase) ? part.Substring(6) : part;

                switch (style.Trim().ToLowerInvariant())
                {
                    case "bold": font.Bold = true; break;
                    case "italic": font.Italic = true; break;
                    case "underline": font.Underline = true; break;
                    case "strikeout": font.Strikeout = true; break;
                }
            }

            return font;
        }

        public bool Equals(FontInfo other) => other != null && ToString() == other.ToString();

        public override bool Equals(object obj) => Equals(obj as FontInfo);

        public override int GetHashCode() => ToString().GetHashCode(StringComparison.Ordinal);
    }

    public sealed class FontInfoConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            value is string text ? FontInfo.Parse(text) : base.ConvertFrom(context, culture, value);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            destinationType == typeof(string) && value is FontInfo font ? font.ToString() : base.ConvertTo(context, culture, value, destinationType);
    }
}
