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

namespace ShareX.HelpersLib;

[Flags]
public enum ImageFontStyle { Regular = 0, Bold = 1, Italic = 2, Underline = 4, Strikeout = 8 }
public enum ImageFontUnit { World, Display, Pixel, Point, Inch, Document, Millimeter }
public enum ImageTextRenderingMode { SystemDefault, SingleBitPerPixelGridFit, SingleBitPerPixel, AntiAliasGridFit, AntiAlias, ClearTypeGridFit }

/// <summary>Serializable font settings; native Skia fonts are created only for measuring and drawing.</summary>
[TypeConverter(typeof(ImageFontConverter))]
public sealed class ImageFont : IDisposable
{
    public string Name { get; }
    public float Size { get; }
    public ImageFontStyle Style { get; }
    public ImageFontUnit Unit { get; }
    public float SizeInPoints => PixelSize * 72 / 96;
    public float PixelSize => Unit switch
    {
        ImageFontUnit.Point => Size * 96 / 72,
        ImageFontUnit.Inch => Size * 96,
        ImageFontUnit.Document => Size * 96 / 300,
        ImageFontUnit.Millimeter => Size * 96 / 25.4f,
        _ => Size
    };

    public ImageFont(string name, float size, ImageFontStyle style = ImageFontStyle.Regular, ImageFontUnit unit = ImageFontUnit.Point)
    {
        Name = name; Size = size; Style = style; Unit = unit;
    }

    public SKFont CreateFont()
    {
        using SKTypeface typeface = SKTypeface.FromFamilyName(Name,
            Style.HasFlag(ImageFontStyle.Bold) ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
            SKFontStyleWidth.Normal, Style.HasFlag(ImageFontStyle.Italic) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
        return new SKFont(typeface ?? SKTypeface.Default, PixelSize);
    }
    internal static string GetUnitSuffix(ImageFontUnit unit) => unit switch
    {
        ImageFontUnit.World => "world",
        ImageFontUnit.Display => "display",
        ImageFontUnit.Pixel => "px",
        ImageFontUnit.Inch => "in",
        ImageFontUnit.Document => "doc",
        ImageFontUnit.Millimeter => "mm",
        _ => "pt"
    };

    public override string ToString() => $"{Name}, {Size.ToString(CultureInfo.InvariantCulture)}{GetUnitSuffix(Unit)}" +
        (Style == ImageFontStyle.Regular ? "" : $", style={Style}");
    public void Dispose() { }
}

public sealed class ImageFontConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is not string text) return base.ConvertFrom(context, culture, value);
        string[] pieces = text.Split(',');
        string sizeText = pieces.Length > 1 ? pieces[1].Trim() : "8.25pt";
        ImageFontUnit unit = ImageFontUnit.Point;
        foreach (ImageFontUnit candidate in Enum.GetValues<ImageFontUnit>())
        {
            string suffix = ImageFont.GetUnitSuffix(candidate);
            if (!sizeText.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) continue;
            unit = candidate;
            sizeText = sizeText[..^suffix.Length].TrimEnd();
            break;
        }
        float size = float.Parse(sizeText, CultureInfo.InvariantCulture);
        ImageFontStyle style = ImageFontStyle.Regular;
        if (pieces.Length > 2) Enum.TryParse(string.Join(",", pieces.Skip(2)).Replace("style=", "").Trim(), true, out style);
        return new ImageFont(pieces[0].Trim(), size, style, unit);
    }
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        => destinationType == typeof(string) && value is ImageFont font ? font.ToString() : base.ConvertTo(context, culture, value, destinationType);
}
