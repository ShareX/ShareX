#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

namespace ShareX.Tools;

public sealed class OCROptions
{
    public bool SingleLine { get; set; }
    // Puts an empty line between paragraphs, which are found from the spacing and size of the lines in the image.
    public bool SeparateParagraphs { get; set; } = true;
    public bool Silent { get; set; }
    public bool AutoCopy { get; set; }
    public List<OCRServiceLinkOption> ServiceLinks { get; set; } = DefaultServiceLinks;
    public bool CloseWindowAfterOpeningServiceLink { get; set; }
    public int SelectedServiceLink { get; set; }
    public WindowState WindowState { get; set; } = new();
    // Width of the result panel beside the image preview; 0 uses the default.
    public double ResultPanelWidth { get; set; }

    public static List<OCRServiceLinkOption> DefaultServiceLinks =>
    [
            new(Localization.Strings.OCROptions_Google_Translate, "https://translate.google.com/?sl=auto&tl=en&text={0}&op=translate"),
            new(Localization.Strings.OCROptions_Google_Search, "https://www.google.com/search?q={0}"),
            new(Localization.Strings.OCROptions_Google_Images, "https://www.google.com/search?q={0}&tbm=isch"),
            new(Localization.Strings.OCROptions_Bing, "https://www.bing.com/search?q={0}"),
            new(Localization.Strings.OCROptions_DuckDuckGo, "https://duckduckgo.com/?q={0}"),
            new(Localization.Strings.OCROptions_DeepL, "https://www.deepl.com/translator#auto/en/{0}")
    ];
}
