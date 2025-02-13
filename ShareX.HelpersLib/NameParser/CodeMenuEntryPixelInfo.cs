#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Collections.Generic;
using System.Drawing;

namespace ShareX.HelpersLib
{
    public class CodeMenuEntryPixelInfo : CodeMenuEntry
    {
        protected override string Prefix { get; } = "$";

        // This shouldn't show up in the list of options, but will continue to work for backwards compatibility's sake.
        private static readonly CodeMenuEntryPixelInfo r = new CodeMenuEntryPixelInfo("r", "Red color (0-255)");
        private static readonly CodeMenuEntryPixelInfo g = new CodeMenuEntryPixelInfo("g", "Green color (0-255)");
        private static readonly CodeMenuEntryPixelInfo b = new CodeMenuEntryPixelInfo("b", "Blue color (0-255)");

        public static readonly CodeMenuEntryPixelInfo r255 = new CodeMenuEntryPixelInfo("r255", "Red color (0-255)");
        public static readonly CodeMenuEntryPixelInfo g255 = new CodeMenuEntryPixelInfo("g255", "Green color (0-255)");
        public static readonly CodeMenuEntryPixelInfo b255 = new CodeMenuEntryPixelInfo("b255", "Blue color (0-255)");
        public static readonly CodeMenuEntryPixelInfo r1 = new CodeMenuEntryPixelInfo("r1", "Red color (0-1). Specify decimal precision with {n}, defaults to 3.");
        public static readonly CodeMenuEntryPixelInfo g1 = new CodeMenuEntryPixelInfo("g1", "Green color (0-1). Specify decimal precision with {n}, defaults to 3.");
        public static readonly CodeMenuEntryPixelInfo b1 = new CodeMenuEntryPixelInfo("b1", "Blue color (0-1). Specify decimal precision with {n}, defaults to 3.");
        public static readonly CodeMenuEntryPixelInfo hex = new CodeMenuEntryPixelInfo("hex", "Hex color value (Lowercase)");
        public static readonly CodeMenuEntryPixelInfo rhex = new CodeMenuEntryPixelInfo("rhex", "Red hex color value (00-ff)");
        public static readonly CodeMenuEntryPixelInfo ghex = new CodeMenuEntryPixelInfo("ghex", "Green hex color value (00-ff)");
        public static readonly CodeMenuEntryPixelInfo bhex = new CodeMenuEntryPixelInfo("bhex", "Blue hex color value (00-ff)");
        public static readonly CodeMenuEntryPixelInfo HEX = new CodeMenuEntryPixelInfo("HEX", "Hex color value (Uppercase)");
        public static readonly CodeMenuEntryPixelInfo rHEX = new CodeMenuEntryPixelInfo("rHEX", "Red hex color value (00-FF)");
        public static readonly CodeMenuEntryPixelInfo gHEX = new CodeMenuEntryPixelInfo("gHEX", "Green hex color value (00-FF)");
        public static readonly CodeMenuEntryPixelInfo bHEX = new CodeMenuEntryPixelInfo("bHEX", "Blue hex color value (00-FF)");
        public static readonly CodeMenuEntryPixelInfo c100 = new CodeMenuEntryPixelInfo("c100", "Cyan color (0-100)");
        public static readonly CodeMenuEntryPixelInfo m100 = new CodeMenuEntryPixelInfo("m100", "Magenta color (0-100)");
        public static readonly CodeMenuEntryPixelInfo y100 = new CodeMenuEntryPixelInfo("y100", "Yellow color (0-100)");
        public static readonly CodeMenuEntryPixelInfo k100 = new CodeMenuEntryPixelInfo("k100", "Key color (0-100)");
        public static readonly CodeMenuEntryPixelInfo name = new CodeMenuEntryPixelInfo("name", "Color name");
        public static readonly CodeMenuEntryPixelInfo x = new CodeMenuEntryPixelInfo("x", "X position");
        public static readonly CodeMenuEntryPixelInfo y = new CodeMenuEntryPixelInfo("y", "Y position");
        public static readonly CodeMenuEntryPixelInfo n = new CodeMenuEntryPixelInfo("n", "New line");

        public CodeMenuEntryPixelInfo(string value, string description) : base(value, description)
        {
        }

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
}