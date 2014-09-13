#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

namespace HelpersLib
{
    public class ActionsCodeMenuEntry : CodeMenuEntry
    {
        public ActionsCodeMenuEntry(string value, string description)
            : base(value, description)
        {
        }

        public override String ToPrefixString()
        {
            return '%' + _value;
        }

        public static readonly ActionsCodeMenuEntry FilePath = new ActionsCodeMenuEntry("input", "File path");
        public static readonly ActionsCodeMenuEntry OutputFilePath = new ActionsCodeMenuEntry("output", "File path without extension + \"Output file name extension\"");
    }
}