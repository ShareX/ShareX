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

using ShareX.HelpersLib.Extensions;
using ShareX.HelpersLib.Helpers;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace ShareX.HelpersLib.NameParser;

public class CodeMenuEntryPixelInfo(string value, string description) : CodeMenuEntry(value, description)
{
    protected override string Prefix { get; } = "$";

    // This shouldn't show up in the list of options, but will continue to work for backwards compatibility's sake.
    private static readonly CodeMenuEntryPixelInfo
        r = new("r", "Red color (0-255)"),
        g = new("g", "Green color (0-255)"),
        b = new("b", "Blue color (0-255)");

    public static readonly CodeMenuEntryPixelInfo
        r255 = new("r255", "Red color (0-255)"),
        g255 = new("g255", "Green color (0-255)"),
        b255 = new("b255", "Blue color (0-255)"),
        r1 = new("r1", "Red color (0-1). Specify decimal precision with {n}, defaults to 3."),
        g1 = new("g1", "Green color (0-1). Specify decimal precision with {n}, defaults to 3."),
        b1 = new("b1", "Blue color (0-1). Specify decimal precision with {n}, defaults to 3."),
        hex = new("hex", "Hex color value (Lowercase)"),
        rhex = new("rhex", "Red hex color value (00-ff)"),
        ghex = new("ghex", "Green hex color value (00-ff)"),
        bhex = new("bhex", "Blue hex color value (00-ff)"),
        HEX = new("HEX", "Hex color value (Uppercase)"),
        rHEX = new("rHEX", "Red hex color value (00-FF)"),
        gHEX = new("gHEX", "Green hex color value (00-FF)"),
        bHEX = new("bHEX", "Blue hex color value (00-FF)"),
        c100 = new("c100", "Cyan color (0-100)"),
        m100 = new("m100", "Magenta color (0-100)"),
        y100 = new("y100", "Yellow color (0-100)"),
        k100 = new("k100", "Key color (0-100)"),
        name = new("name", "Color name"),
        x = new("x", "X position"),
        y = new("y", "Y position"),
        n = new("n", "New line");

    public static string Parse(string input, Color color, Point position)
    {
        input = input.Replace(r255.ToPrefixString(), color.R.ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(g255.ToPrefixString(), color.G.ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(b255.ToPrefixString(), color.B.ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(rHEX.ToPrefixString(), color.R.ToString("X2"), StringComparison.InvariantCulture).
            Replace(gHEX.ToPrefixString(), color.G.ToString("X2"), StringComparison.InvariantCulture).
            Replace(bHEX.ToPrefixString(), color.B.ToString("X2"), StringComparison.InvariantCulture).
            Replace(rhex.ToPrefixString(), color.R.ToString("X2").ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase).
            Replace(ghex.ToPrefixString(), color.G.ToString("X2").ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase).
            Replace(bhex.ToPrefixString(), color.B.ToString("X2").ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase).
            Replace(HEX.ToPrefixString(), ColorHelpers.ColorToHex(color), StringComparison.InvariantCulture).
            Replace(hex.ToPrefixString(), ColorHelpers.ColorToHex(color).ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase).
            Replace(c100.ToPrefixString(), Math.Round(ColorHelpers.ColorToCMYK(color).Cyan100, 2, MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(m100.ToPrefixString(), Math.Round(ColorHelpers.ColorToCMYK(color).Magenta100, 2, MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(y100.ToPrefixString(), Math.Round(ColorHelpers.ColorToCMYK(color).Yellow100, 2, MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(k100.ToPrefixString(), Math.Round(ColorHelpers.ColorToCMYK(color).Key100, 2, MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(name.ToPrefixString(), ColorHelpers.GetColorName(color), StringComparison.InvariantCultureIgnoreCase).
            Replace(x.ToPrefixString(), position.X.ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(y.ToPrefixString(), position.Y.ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(n.ToPrefixString(), Environment.NewLine, StringComparison.InvariantCultureIgnoreCase);

        foreach (Tuple<string, int> entry in ListEntryWithValue(input, r1.ToPrefixString()))
        {
            input = input.Replace(entry.Item1, Math.Round(color.R / 255d, MathHelpers.Clamp(entry.Item2, 0, 15), MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        foreach (Tuple<string, int> entry in ListEntryWithValue(input, g1.ToPrefixString()))
        {
            input = input.Replace(entry.Item1, Math.Round(color.G / 255d, MathHelpers.Clamp(entry.Item2, 0, 15), MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        foreach (Tuple<string, int> entry in ListEntryWithValue(input, b1.ToPrefixString()))
        {
            input = input.Replace(entry.Item1, Math.Round(color.B / 255d, MathHelpers.Clamp(entry.Item2, 0, 15), MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        input = input.Replace(r1.ToPrefixString(), Math.Round(color.R / 255d, 3, MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(g1.ToPrefixString(), Math.Round(color.G / 255d, 3, MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(b1.ToPrefixString(), Math.Round(color.B / 255d, 3, MidpointRounding.AwayFromZero).ToString(), StringComparison.InvariantCultureIgnoreCase);

        input = input.Replace(r.ToPrefixString(), color.R.ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(g.ToPrefixString(), color.G.ToString(), StringComparison.InvariantCultureIgnoreCase).
            Replace(b.ToPrefixString(), color.B.ToString(), StringComparison.InvariantCultureIgnoreCase);

        return input;
    }

    private static IEnumerable<Tuple<string, string[]>> ListEntryWithArguments(string text, string entry, int elements)
    {
        foreach (Tuple<string, string> o in text.ForEachBetween(entry + "{", "}"))
        {
            string[] s = o.Item2.Split(',');
            if (elements > s.Length)
            {
                Array.Resize(ref s, elements);
            }
            yield return new Tuple<string, string[]>(o.Item1, s);
        }
    }

    private static IEnumerable<Tuple<string, int[]>> ListEntryWithValues(string text, string entry, int elements)
    {
        foreach (Tuple<string, string[]> o in ListEntryWithArguments(text, entry, elements))
        {
            int[] a = new int[o.Item2.Length];
            for (int i = o.Item2.Length - 1; i >= 0; --i)
            {
                if (int.TryParse(o.Item2[i], out int n))
                {
                    a[i] = n;
                }
            }
            yield return new Tuple<string, int[]>(o.Item1, a);
        }
    }

    private static IEnumerable<Tuple<string, int>> ListEntryWithValue(string text, string entry)
    {
        foreach (Tuple<string, int[]> o in ListEntryWithValues(text, entry, 1))
        {
            yield return new Tuple<string, int>(o.Item1, o.Item2[0]);
        }
    }
}