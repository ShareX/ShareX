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

using ShareX.HelpersLib.Properties;

namespace ShareX.HelpersLib
{
    public class CodeMenuEntryActions : CodeMenuEntry
    {
        protected override string Prefix { get; } = "$";

        public static readonly CodeMenuEntryActions input = new CodeMenuEntryActions("input", Resources.ActionsCodeMenuEntry_FilePath_File_path);
        public static readonly CodeMenuEntryActions output = new CodeMenuEntryActions("output", Resources.ActionsCodeMenuEntry_OutputFilePath_File_path_without_extension____Output_file_name_extension_);

        public CodeMenuEntryActions(string value, string description) : base(value, description)
        {
        }

        public static string Parse(string pattern, string inputPath, string outputPath)
        {
            string result = pattern;

            if (inputPath != null)
            {
                result = result.Replace(input.ToPrefixString("%"), '"' + inputPath + '"');
                result = result.Replace(input.ToPrefixString(), inputPath);
            }

            if (outputPath != null)
            {
                result = result.Replace(output.ToPrefixString("%"), '"' + outputPath + '"');
                result = result.Replace(output.ToPrefixString(), outputPath);
            }

            return result;
        }
    }
}