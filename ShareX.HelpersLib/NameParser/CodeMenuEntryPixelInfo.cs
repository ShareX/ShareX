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

namespace ShareX.HelpersLib
{
    public class CodeMenuEntryPixelInfo : CodeMenuEntry
    {
        protected override string Prefix { get; } = "$";

        public static readonly CodeMenuEntryPixelInfo x = new CodeMenuEntryPixelInfo("x", "X position");
        public static readonly CodeMenuEntryPixelInfo y = new CodeMenuEntryPixelInfo("y", "Y position");
        public static readonly CodeMenuEntryPixelInfo r = new CodeMenuEntryPixelInfo("r", "Red color (0-255)");
        public static readonly CodeMenuEntryPixelInfo g = new CodeMenuEntryPixelInfo("g", "Green color (0-255)");
        public static readonly CodeMenuEntryPixelInfo b = new CodeMenuEntryPixelInfo("b", "Blue color (0-255)");
        public static readonly CodeMenuEntryPixelInfo hex = new CodeMenuEntryPixelInfo("hex", "Hex color value");
        public static readonly CodeMenuEntryPixelInfo n = new CodeMenuEntryPixelInfo("n", "New line");

        public CodeMenuEntryPixelInfo(string value, string description) : base(value, description)
        {
        }
    }
}