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

#nullable enable

using Avalonia.Media;
using Avalonia.Platform;
using ShareX.HelpersLib;
using ShareX.Icons;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace ShareX;

/// <summary>
/// The flag shown next to each language, read from the 4:3 SVG files in Resources/Flags so it is drawn sharp at
/// every display scale.
/// </summary>
public static class LanguageFlags
{
    private static readonly ConcurrentDictionary<SupportedLanguage, IImage?> Flags = new();

    public static IImage? Get(SupportedLanguage language) => Flags.GetOrAdd(language, static language =>
    {
        if (language == SupportedLanguage.Automatic)
        {
            return null;
        }

        string countryCode = LanguageHelper.GetCultureName(language).Split('-')[1].ToLowerInvariant();

        try
        {
            using Stream stream = AssetLoader.Open(new Uri($"avares://ShareX/Resources/Flags/{countryCode}.svg"));
            return SvgImage.Load(stream);
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
            return null;
        }
    });
}
