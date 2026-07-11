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

using System.Diagnostics;
using System.Text;

namespace ShareX.ImageEditor.Integration;

/// <summary>
/// Default macOS wallpaper resolver used by Avalonia hosts that do not provide a custom implementation.
/// </summary>
internal sealed class MacOSDesktopWallpaperService : IDesktopWallpaperService
{
    public bool IsSupported => OperatingSystem.IsMacOS();
    public bool RequiresDesktopWallpaperPrewarm => false;

    public bool TryGetDesktopWallpaper(out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        if (!OperatingSystem.IsMacOS() || !TryGetDesktopWallpaperPath(out string? path))
        {
            return false;
        }

        wallpaper = new DesktopWallpaperInfo
        {
            Path = path!,
            Layout = TryGetDesktopWallpaperLayout(out DesktopWallpaperLayout layout)
                ? layout
                : DesktopWallpaperLayout.Fill
        };

        return true;
    }

    public void PrewarmDesktopWallpaper()
    {
        // Apple exposes wallpaper URLs and presentation options directly, so no conversion cache is needed here.
    }

    private static bool TryGetDesktopWallpaperPath(out string? path)
    {
        path = null;

        const string appleScript = "tell application \"System Events\" to get POSIX path of (picture of current desktop as alias)";
        if (!TryRunProcess("osascript", $"-e \"{EscapeForDoubleQuotedArgument(appleScript)}\"", out string output))
        {
            return false;
        }

        string wallpaperPath = output.Trim();
        if (string.IsNullOrWhiteSpace(wallpaperPath) || !File.Exists(wallpaperPath))
        {
            return false;
        }

        path = wallpaperPath;
        return true;
    }

    private static bool TryGetDesktopWallpaperLayout(out DesktopWallpaperLayout layout)
    {
        layout = DesktopWallpaperLayout.Fill;

        if (!TryRunProcess("defaults", "read com.apple.spaces", out string spacesOutput) ||
            !TryGetCurrentSpaceUuid(spacesOutput, out string? currentSpaceUuid) ||
            string.IsNullOrWhiteSpace(currentSpaceUuid))
        {
            return false;
        }

        if (!TryRunProcess("defaults", "read com.apple.desktop Background", out string desktopOutput) ||
            !TryExtractDesktopSettingsBlock(desktopOutput, currentSpaceUuid!, out string settingsBlock))
        {
            return false;
        }

        string? placement = TryReadAssignedValue(settingsBlock, "Placement");
        string? scaling = TryReadAssignedValue(settingsBlock, "ImageScaling");
        layout = MapDesktopWallpaperLayout(placement, scaling);
        return true;
    }

    private static DesktopWallpaperLayout MapDesktopWallpaperLayout(string? placement, string? scaling)
    {
        if (!string.IsNullOrWhiteSpace(placement))
        {
            switch (placement.Trim())
            {
                case "Center":
                    return DesktopWallpaperLayout.Center;
                case "Tile":
                    return DesktopWallpaperLayout.Tile;
                case "StretchToFillScreen":
                    return DesktopWallpaperLayout.Stretch;
                case "FitToScreen":
                    return DesktopWallpaperLayout.Fit;
                case "FillScreen":
                    return DesktopWallpaperLayout.Fill;
            }
        }

        if (!string.IsNullOrWhiteSpace(scaling))
        {
            switch (scaling.Trim())
            {
                case "none":
                    return DesktopWallpaperLayout.Center;
                case "proportionally":
                case "proportionallyDown":
                    return DesktopWallpaperLayout.Fit;
                case "axesIndependently":
                    return DesktopWallpaperLayout.Stretch;
                case "proportionallyUpOrDown":
                    return DesktopWallpaperLayout.Fill;
            }
        }

        return DesktopWallpaperLayout.Fill;
    }

    private static bool TryGetCurrentSpaceUuid(string spacesOutput, out string? uuid)
    {
        uuid = null;
        bool inCurrentSpaceBlock = false;

        foreach (string rawLine in spacesOutput.Split('\n'))
        {
            string line = rawLine.Trim();
            if (line.StartsWith("\"Current Space\"", StringComparison.Ordinal) ||
                line.StartsWith("Current Space", StringComparison.Ordinal))
            {
                inCurrentSpaceBlock = true;
                continue;
            }

            if (!inCurrentSpaceBlock)
            {
                continue;
            }

            if (line.StartsWith("uuid =", StringComparison.Ordinal))
            {
                int firstQuote = line.IndexOf('"');
                int lastQuote = line.LastIndexOf('"');
                if (firstQuote >= 0 && lastQuote > firstQuote)
                {
                    uuid = line.Substring(firstQuote + 1, lastQuote - firstQuote - 1);
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryExtractDesktopSettingsBlock(string desktopOutput, string uuid, out string settingsBlock)
    {
        settingsBlock = string.Empty;
        string[] lines = desktopOutput.Split('\n');
        StringBuilder blockBuilder = new();
        bool inUuidBlock = false;
        bool inDefaultBlock = false;
        int braceDepth = 0;
        int defaultBraceDepth = 0;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (!inUuidBlock)
            {
                if (line.StartsWith($"\"{uuid}\"", StringComparison.Ordinal))
                {
                    inUuidBlock = true;
                    braceDepth += CountChar(line, '{') - CountChar(line, '}');
                }

                continue;
            }

            if (!inDefaultBlock && line.StartsWith("default = {", StringComparison.Ordinal))
            {
                inDefaultBlock = true;
                defaultBraceDepth = braceDepth + CountChar(line, '{') - CountChar(line, '}');
                blockBuilder.AppendLine(line);
                braceDepth = defaultBraceDepth;
                continue;
            }

            int delta = CountChar(line, '{') - CountChar(line, '}');
            braceDepth += delta;

            if (inDefaultBlock)
            {
                blockBuilder.AppendLine(line);
                if (braceDepth < defaultBraceDepth)
                {
                    settingsBlock = blockBuilder.ToString();
                    return true;
                }
            }

            if (braceDepth <= 0)
            {
                break;
            }
        }

        return false;
    }

    private static string? TryReadAssignedValue(string block, string key)
    {
        string prefix = key + " = ";
        foreach (string rawLine in block.Split('\n'))
        {
            string line = rawLine.Trim();
            if (!line.StartsWith(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            string value = line.Substring(prefix.Length).Trim().TrimEnd(';').Trim();
            return value.Trim('"');
        }

        return null;
    }

    private static int CountChar(string value, char target)
    {
        int count = 0;

        foreach (char character in value)
        {
            if (character == target)
            {
                count++;
            }
        }

        return count;
    }

    private static bool TryRunProcess(string fileName, string arguments, out string output)
    {
        output = string.Empty;

        try
        {
            using Process? process = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            if (process == null)
            {
                return false;
            }

            output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit(2000);
            return process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output);
        }
        catch
        {
            return false;
        }
    }

    private static string EscapeForDoubleQuotedArgument(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}