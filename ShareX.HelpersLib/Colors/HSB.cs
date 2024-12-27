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

public struct HSB
{
    private double hue,saturation,brightness;
    private int alpha;

    public double Hue
    {
        readonly get => hue;
        set => hue = ColorHelpers.ValidColor(value);
    }

    public double Hue360
    {
        readonly get => hue * 360;
        set => hue = ColorHelpers.ValidColor(value / 360);
    }

    public double Saturation
    {
        readonly get => saturation;
        set => saturation = ColorHelpers.ValidColor(value);
    }

    public double Saturation100
    {
        readonly get => saturation * 100;
        set => saturation = ColorHelpers.ValidColor(value / 100);
    }

    public double Brightness
    {
        readonly get => brightness;
        set => brightness = ColorHelpers.ValidColor(value);
    }

    public double Brightness100
    {
        readonly get => brightness * 100;
        set => brightness = ColorHelpers.ValidColor(value / 100);
    }

    public int Alpha
    {
        readonly get => alpha;
        set => alpha = ColorHelpers.ValidColor(value);
    }

    public HSB(double hue, double saturation, double brightness, int alpha = 255) : this()
    {
        Hue = hue;
        Saturation = saturation;
        Brightness = brightness;
        Alpha = alpha;
    }

    public HSB(int hue, int saturation, int brightness, int alpha = 255) : this()
    {
        Hue360 = hue;
        Saturation100 = saturation;
        Brightness100 = brightness;
        Alpha = alpha;
    }

    public HSB(Color color) => this = ColorHelpers.ColorToHSB(color);

    public static implicit operator HSB(Color color) => ColorHelpers.ColorToHSB(color);

    public static implicit operator Color(HSB color) => color.ToColor();

    public static implicit operator RGBA(HSB color) => color.ToColor();

    public static implicit operator CMYK(HSB color) => color.ToColor();

    public static bool operator ==(HSB left, HSB right) => left.Hue == right.Hue && left.Saturation == right.Saturation && left.Brightness == right.Brightness;

    public static bool operator !=(HSB left, HSB right) => !(left == right);

    public override readonly string ToString() => string.Format(Resources.HSB_ToString_, Hue360, Saturation100, Brightness100);

    public readonly Color ToColor() => ColorHelpers.HSBToColor(this);

    public override readonly int GetHashCode() => base.GetHashCode();

    public override readonly bool Equals(object obj) => base.Equals(obj);
}