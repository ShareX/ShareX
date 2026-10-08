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

using ShareX.Tools;
using Xunit;

namespace ShareX.Tools.Tests;

public sealed class OCRLanguageTests
{
    private static readonly OCRLanguageOption[] VisionLanguages =
    [
        new("allemand (Allemagne)", "de-DE"),
        new("anglais (États-Unis)", "en-US"),
        new("français (France)", "fr-FR")
    ];

    [Fact]
    public void SavedLanguageWinsWhenTheEngineHasIt()
    {
        Assert.Equal("en-US", OCRViewModel.SelectLanguage(VisionLanguages, "en-US", "fr-FR")?.LanguageTag);
    }

    [Fact]
    public void InexactSavedTagFallsBackToTheSystemLanguageNotTheFirstInTheList()
    {
        Assert.Equal("fr-FR", OCRViewModel.SelectLanguage(VisionLanguages, "en", "fr-FR")?.LanguageTag);
        Assert.Equal("fr-FR", OCRViewModel.SelectLanguage(VisionLanguages, "en", "fr-CA")?.LanguageTag);
    }

    [Fact]
    public void WithoutTheSystemLanguageAVariantOfTheSavedOneIsUsed()
    {
        Assert.Equal("en-US", OCRViewModel.SelectLanguage(VisionLanguages, "en", "ja-JP")?.LanguageTag);
        Assert.Equal("de-DE", OCRViewModel.SelectLanguage(VisionLanguages, "it", "ja-JP")?.LanguageTag);
        Assert.Null(OCRViewModel.SelectLanguage([], "en", "fr-FR"));
    }
}
