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
using ShareX.HelpersLib.Properties;

using System.Drawing;

namespace ShareX.HelpersLib.Colors;

public struct CMYK
{
    private double cyan, magenta, yellow, key;
    private int alpha;

    public double Cyan
    {
        readonly get => cyan;
        set => cyan = ColorHelpers.ValidColor(value);
    }

    public double Cyan100
    {
        readonly get => cyan * 100;
        set => cyan = ColorHelpers.ValidColor(value / 100);
    }

    public double Magenta
    {
        readonly get => magenta;
        set => magenta = ColorHelpers.ValidColor(value);
    }

    public double Magenta100
    {
        readonly get => magenta * 100;
        set => magenta = ColorHelpers.ValidColor(value / 100);
    }

    public double Yellow
    {
        readonly get => yellow;
        set => yellow = ColorHelpers.ValidColor(value);
    }

    public double Yellow100
    {
        readonly get => yellow * 100;
        set => yellow = ColorHelpers.ValidColor(value / 100);
    }

    public double Key
    {
        readonly get => key;
        set => key = ColorHelpers.ValidColor(value);
    }

    public double Key100
    {
        readonly get => key * 100;
        set => key = ColorHelpers.ValidColor(value / 100);
    }

    public int Alpha
    {
        readonly get => alpha;
        set => alpha = ColorHelpers.ValidColor(value);
    }

    public CMYK(double cyan, double magenta, double yellow, double key, int alpha = 255) : this()
    {
        Cyan = cyan;
        Magenta = magenta;
        Yellow = yellow;
        Key = key;
        Alpha = alpha;
    }

    public CMYK(int cyan, int magenta, int yellow, int key, int alpha = 255) : this()
    {
        Cyan100 = cyan;
        Magenta100 = magenta;
        Yellow100 = yellow;
        Key100 = key;
        Alpha = alpha;
    }

    public CMYK(Color color) => this = ColorHelpers.ColorToCMYK(color);

    public static implicit operator CMYK(Color color) => ColorHelpers.ColorToCMYK(color);

    public static implicit operator Color(CMYK color) => color.ToColor();

    public static implicit operator RGBA(CMYK color) => color.ToColor();

    public static implicit operator HSB(CMYK color) => color.ToColor();

    public static bool operator ==(CMYK left, CMYK right) => left.Cyan == right.Cyan && left.Magenta == right.Magenta && left.Yellow == right.Yellow && left.Key == right.Key;

    public static bool operator !=(CMYK left, CMYK right) => !(left == right);

    public override readonly string ToString() => string.Format(Resources.CMYK_ToString_Cyan___0_0_0____Magenta___1_0_0____Yellow___2_0_0____Key___3_0_0__, Cyan100, Magenta100, Yellow100, Key100);

    public readonly Color ToColor() => ColorHelpers.CMYKToColor(this);

    public override readonly int GetHashCode() => base.GetHashCode();

    public override readonly bool Equals(object obj) => base.Equals(obj);
}