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

using System.Collections.Generic;
using System.Text;

namespace ShareX.HelpersLib
{
    public class StringLineReader
    {
        public string Text { get; private set; }
        public int Position { get; private set; }
        public int Length { get; private set; }

        public StringLineReader(string text)
        {
            Text = text;
            Length = Text.Length;
        }

        public string ReadLine()
        {
            StringBuilder builder = new StringBuilder();

            while (!string.IsNullOrEmpty(Text) && Position < Length)
            {
                char ch = Text[Position];
                builder.Append(ch);
                Position++;

                if (ch == '\r' || ch == '\n' || Position == Length)
                {
                    if (ch == '\r' && Position < Length && Text[Position] == '\n')
                    {
                        continue;
                    }

                    return builder.ToString();
                }
            }

            return null;
        }

        public string[] ReadAllLines(bool autoTrim = true)
        {
            List<string> lines = new List<string>();

            string line;

            while ((line = ReadLine()) != null)
            {
                if (autoTrim) line = line.Trim();
                lines.Add(line);
            }

            return lines.ToArray();
        }

        public void Reset()
        {
            Position = 0;
        }
    }
}