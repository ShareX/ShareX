#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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

using ShareX.HelpersLib.Helpers;

using System.Drawing;

namespace ShareX.HelpersLib.Colors;

public struct RGBA
{
    private int red, green, blue, alpha;

    public int Red
    {
        readonly get => red;
        set => red = ColorHelpers.ValidColor(value);
    }

    public int Green
    {
        readonly get => green;
        set => green = ColorHelpers.ValidColor(value);
    }

    public int Blue
    {
        readonly get => blue;
        set => blue = ColorHelpers.ValidColor(value);
    }

    public int Alpha
    {
        readonly get => alpha;
        set => alpha = ColorHelpers.ValidColor(value);
    }

    public RGBA(int red, int green, int blue, int alpha = 255) : this()
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public RGBA(Color color) : this(color.R, color.G, color.B, color.A)
    {
    }

    public static implicit operator RGBA(Color color) => new(color);

    public static implicit operator Color(RGBA color) => color.ToColor();

    public static implicit operator HSB(RGBA color) => color.ToHSB();

    public static implicit operator CMYK(RGBA color) => color.ToCMYK();

    public static bool operator ==(RGBA left, RGBA right) => left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue && left.Alpha == right.Alpha;

    public static bool operator !=(RGBA left, RGBA right) => !(left == right);

    public override string ToString() => $"R: {Red}, G: {Green}, B: {Blue}, A: {Alpha}";

    public readonly Color ToColor() => Color.FromArgb(Alpha, Red, Green, Blue);

    public readonly string ToHex(ColorFormat format = ColorFormat.RGB) => ColorHelpers.ColorToHex(this, format);

    public readonly int ToDecimal(ColorFormat format = ColorFormat.RGB) => ColorHelpers.ColorToDecimal(this, format);

    public readonly HSB ToHSB() => ColorHelpers.ColorToHSB(this);

    public readonly CMYK ToCMYK() => ColorHelpers.ColorToCMYK(this);

    public override readonly int GetHashCode() => base.GetHashCode();

    public override readonly bool Equals(object obj) => base.Equals(obj);
}