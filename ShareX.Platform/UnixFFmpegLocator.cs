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
using System.Collections.Generic;
using System.IO;

namespace ShareX.Platform;

/// <summary>Finds the FFmpeg program on Linux and macOS.</summary>
public static class UnixFFmpegLocator
{
    /// <summary>
    /// An ffmpeg next to ShareX first, so a packaged build can ship its own, then PATH, then the usual install folders, which
    /// applications started from a launcher may not have on PATH (Homebrew on macOS).
    /// </summary>
    public static string Find(string applicationDirectory, IEnumerable<string> extraDirectories)
    {
        string bundled = Path.Combine(applicationDirectory, "ffmpeg");

        if (File.Exists(bundled))
        {
            return bundled;
        }

        if (CommandRunner.FindOnPath("ffmpeg") is string onPath)
        {
            return onPath;
        }

        foreach (string directory in extraDirectories)
        {
            string candidate = Path.Combine(directory, "ffmpeg");

            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        // Shown in FFmpeg options as the program that was looked for.
        return "/usr/bin/ffmpeg";
    }
}
