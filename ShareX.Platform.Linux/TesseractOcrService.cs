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

using ShareX.Platform.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux;

/// <summary>OCR with the tesseract command line program, reading the image from standard input.</summary>
public sealed class TesseractOcrService : IOcrService
{
    private readonly PlatformInfo info;
    private readonly ICommandRunner runner;
    private IReadOnlyList<OcrLanguage>? languages;

    public TesseractOcrService(PlatformInfo info, ICommandRunner runner)
    {
        this.info = info;
        this.runner = runner;
    }

    public FeatureSupport Support => runner.Exists("tesseract")
        ? FeatureSupport.Supported
        : LinuxPackages.Missing(info.Distribution ?? LinuxDistribution.Unknown, LinuxTool.Tesseract);

    public IReadOnlyList<OcrLanguage> GetLanguages()
    {
        if (languages != null)
        {
            return languages;
        }

        if (!Support.IsSupported)
        {
            return Array.Empty<OcrLanguage>();
        }

        CommandResult result = runner.RunAsync("tesseract", ["--list-langs"], timeout: TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
        languages = ParseLanguages(Encoding.UTF8.GetString(result.StandardOutput));
        return languages;
    }

    public async Task<string> RecognizeAsync(byte[] png, string languageTag, bool singleLine, CancellationToken cancellationToken = default)
    {
        if (!Support.IsSupported)
        {
            throw new PlatformNotSupportedException(Support.Reason);
        }

        IReadOnlyList<OcrLanguage> available = GetLanguages();
        string language = MatchLanguage(languageTag, available)
            ?? throw new InvalidOperationException($"Tesseract has no data for \"{languageTag}\". Installed: {string.Join(", ", available.Select(l => l.Tag))}.");

        CommandResult result = await runner.RunAsync("tesseract", ["stdin", "stdout", "-l", language], png,
            TimeSpan.FromMinutes(2), cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            throw new InvalidOperationException($"tesseract failed: {result.StandardError.Trim()}");
        }

        return FormatText(Encoding.UTF8.GetString(result.StandardOutput), singleLine);
    }

    /// <summary>tesseract --list-langs prints a header line, then one code per line. "osd" is orientation detection, not a language.</summary>
    internal static IReadOnlyList<OcrLanguage> ParseLanguages(string output)
    {
        return output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => !line.StartsWith("List of available languages", StringComparison.OrdinalIgnoreCase) && line != "osd" && !line.Contains(' '))
            .Select(code => new OcrLanguage(code, GetDisplayName(code)))
            .OrderBy(language => language.DisplayName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// The installed Tesseract code for a tag: the code itself, or the ISO 639-2 code of a BCP 47 tag that settings carried over from
    /// Windows ("en-US" → "eng", "zh-Hans" → "chi_sim").
    /// </summary>
    internal static string? MatchLanguage(string tag, IReadOnlyList<OcrLanguage> available)
    {
        if (available.Any(l => l.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase)))
        {
            return available.First(l => l.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase)).Tag;
        }

        string? code = ToTesseractCode(tag);
        return code != null && available.Any(l => l.Tag == code) ? code : null;
    }

    internal static string? ToTesseractCode(string tag)
    {
        if (tag.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
        {
            return tag.Contains("Hant", StringComparison.OrdinalIgnoreCase) || tag.EndsWith("TW", StringComparison.OrdinalIgnoreCase) ||
                tag.EndsWith("HK", StringComparison.OrdinalIgnoreCase) ? "chi_tra" : "chi_sim";
        }

        try
        {
            string threeLetter = CultureInfo.GetCultureInfo(tag).ThreeLetterISOLanguageName;
            return string.IsNullOrEmpty(threeLetter) || threeLetter == "ivl" ? null : threeLetter;
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    internal static string FormatText(string text, bool singleLine)
    {
        IEnumerable<string> lines = text.Replace("\f", "").Split('\n').Select(line => line.TrimEnd()).Where(line => line.Length > 0);
        return string.Join(singleLine ? " " : Environment.NewLine, lines);
    }

    private static string GetDisplayName(string code)
    {
        string baseCode = code.Split('_')[0];

        foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
        {
            if (culture.ThreeLetterISOLanguageName == baseCode)
            {
                return code.Contains('_') ? $"{culture.DisplayName} ({code})" : culture.DisplayName;
            }
        }

        return code;
    }
}
