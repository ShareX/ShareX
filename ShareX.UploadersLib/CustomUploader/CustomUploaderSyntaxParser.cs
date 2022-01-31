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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;

namespace ShareX.UploadersLib
{
    public class CustomUploaderSyntaxParser : ShareXSyntaxParser
    {
        private static IEnumerable<CustomUploaderFunction> Functions = Helpers.GetInstances<CustomUploaderFunction>();

        public string FileName { get; set; }
        public string Input { get; set; }
        public ResponseInfo ResponseInfo { get; set; }
        public List<string> RegexList { get; set; }
        public bool URLEncode { get; set; } // Only URL encodes file name and input

        protected override string CallFunction(string functionName, string[] parameters)
        {
            foreach (CustomUploaderFunction function in Functions)
            {
                if (function.Name.Equals(functionName, StringComparison.OrdinalIgnoreCase))
                {
                    return function.Call(this, parameters);
                }
            }

            throw new Exception("Invalid function name: " + functionName);
        }
    }
}