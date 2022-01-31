#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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

using System.Text.RegularExpressions;

namespace ShareX.UploadersLib
{
    internal class CustomUploaderFunctionRegex : CustomUploaderFunction
    {
        public override string Name { get; } = "regex";

        public override string Call(CustomUploaderSyntaxParser parser, string[] parameters)
        {
            if (parameters.Length > 0)
            {
                string regexIndex = parameters[0];

                if (!string.IsNullOrEmpty(regexIndex) && int.TryParse(regexIndex, out int regexIndexNumber))
                {
                    string pattern = parser.RegexList[regexIndexNumber - 1];
                    Match match = Regex.Match(parser.ResponseInfo.ResponseText, pattern);

                    if (parameters.Length > 1)
                    {
                        string regexGroup = parameters[1];

                        if (!string.IsNullOrEmpty(regexGroup) && int.TryParse(regexGroup, out int regexGroupNumber))
                        {
                            return match.Groups[regexGroupNumber].Value;
                        }

                        return match.Groups[regexGroup].Value;
                    }

                    return match.Value;
                }
            }

            return null;
        }
    }
}