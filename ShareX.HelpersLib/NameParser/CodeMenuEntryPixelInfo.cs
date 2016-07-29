#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Drawing;

namespace ShareX.HelpersLib
{
    public class CodeMenuEntryPixelInfo : CodeMenuEntry
    {
        protected override string Prefix { get; } = "$";

        public static readonly CodeMenuEntryPixelInfo r = new CodeMenuEntryPixelInfo("r", "Red color (0-255)");
        public static readonly CodeMenuEntryPixelInfo g = new CodeMenuEntryPixelInfo("g", "Green color (0-255)");
        public static readonly CodeMenuEntryPixelInfo b = new CodeMenuEntryPixelInfo("b", "Blue color (0-255)");
        public static readonly CodeMenuEntryPixelInfo hex = new CodeMenuEntryPixelInfo("hex", "Hex color value (Lowercase)");
        public static readonly CodeMenuEntryPixelInfo HEX = new CodeMenuEntryPixelInfo("HEX", "Hex color value (Uppercase)");
        public static readonly CodeMenuEntryPixelInfo x = new CodeMenuEntryPixelInfo("x", "X position");
        public static readonly CodeMenuEntryPixelInfo y = new CodeMenuEntryPixelInfo("y", "Y position");
        public static readonly CodeMenuEntryPixelInfo n = new CodeMenuEntryPixelInfo("n", "New line");

        public CodeMenuEntryPixelInfo(string value, string description) : base(value, description)
        {
        }

        public static string Parse(string input, Color color, Point position)
        {
            return input.Replace(r.ToPrefixString(), color.R.ToString(), StringComparison.InvariantCultureIgnoreCase).
                Replace(g.ToPrefixString(), color.G.ToString(), StringComparison.InvariantCultureIgnoreCase).
                Replace(b.ToPrefixString(), color.B.ToString(), StringComparison.InvariantCultureIgnoreCase).
                Replace(HEX.ToPrefixString(), ColorHelpers.ColorToHex(color), StringComparison.InvariantCulture).
                Replace(hex.ToPrefixString(), ColorHelpers.ColorToHex(color).ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase).
                Replace(x.ToPrefixString(), position.X.ToString(), StringComparison.InvariantCultureIgnoreCase).
                Replace(y.ToPrefixString(), position.Y.ToString(), StringComparison.InvariantCultureIgnoreCase).
                Replace(n.ToPrefixString(), Environment.NewLine, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}