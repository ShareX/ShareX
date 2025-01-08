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

using System.Linq;

namespace ShareX.UploadersLib
{
    // Example: {select:domain1.com|domain2.com}
    internal class CustomUploaderFunctionSelect : CustomUploaderFunction
    {
        public override string Name { get; } = "select";

        public override int MinParameterCount { get; } = 1;

        public override string Call(ShareXCustomUploaderSyntaxParser parser, string[] parameters)
        {
            string[] values = parameters.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (values.Length > 0)
            {
                using (ParserSelectForm form = new ParserSelectForm(values))
                {
                    form.ShowDialog();
                    return form.SelectedText;
                }
            }

            return null;
        }
    }
}