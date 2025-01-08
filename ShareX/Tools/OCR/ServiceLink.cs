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

using ShareX.HelpersLib;

namespace ShareX
{
    public class ServiceLink
    {
        public string Name { get; set; }
        public string URL { get; set; }

        public ServiceLink(string name, string url)
        {
            Name = name;
            URL = url;
        }

        public string GenerateLink(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string encodedInput = URLHelpers.URLEncode(input);
                return string.Format(URL, encodedInput);
            }

            return null;
        }

        public void OpenLink(string input)
        {
            string link = GenerateLink(input);

            if (!string.IsNullOrEmpty(link))
            {
                URLHelpers.OpenURL(link);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}