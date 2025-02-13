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

namespace ShareX
{
    public class OCROptions
    {
        public string Language { get; set; } = "en";
        public float ScaleFactor { get; set; } = 2f;
        public bool SingleLine { get; set; } = false;
        public bool Silent { get; set; } = false;
        public bool AutoCopy { get; set; } = false;
        public List<ServiceLink> ServiceLinks { get; set; } = DefaultServiceLinks;
        public bool CloseWindowAfterOpeningServiceLink { get; set; } = false;
        public int SelectedServiceLink { get; set; } = 0;

        public static List<ServiceLink> DefaultServiceLinks => new List<ServiceLink>()
        {
            new ServiceLink("Google Translate", "https://translate.google.com/?sl=auto&tl=en&text={0}&op=translate"),
            new ServiceLink("Google Search", "https://www.google.com/search?q={0}"),
            new ServiceLink("Google Images", "https://www.google.com/search?q={0}&tbm=isch"),
            new ServiceLink("Bing", "https://www.bing.com/search?q={0}"),
            new ServiceLink("DuckDuckGo", "https://duckduckgo.com/?q={0}"),
            new ServiceLink("DeepL", "https://www.deepl.com/translator#auto/en/{0}")
        };
    }
}