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
using System.IO;
using System.Runtime.Versioning;

namespace ShareX.Platform.MacOS;

/// <summary>File thumbnails from QuickLook through qlmanage, which ships with macOS.</summary>
[SupportedOSPlatform("macos")]
public sealed class QuickLookThumbnailService(ICommandRunner runner) : IThumbnailService
{
    public FeatureSupport Support => runner.Exists("qlmanage") ? FeatureSupport.Supported : FeatureSupport.NotSupported("QuickLook (qlmanage) is not available.");

    public byte[]? GetThumbnail(string path, int maxWidth, int maxHeight)
    {
        if (!File.Exists(path) || !Support.IsSupported)
        {
            return null;
        }

        string directory = Path.Combine(Path.GetTempPath(), "sharex-quicklook-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            // qlmanage writes <file name>.png into the output folder, scaled to fit the size.
            int size = Math.Max(16, Math.Max(maxWidth, maxHeight));
            CommandResult result = runner.RunAsync("qlmanage", ["-t", "-s", size.ToString(System.Globalization.CultureInfo.InvariantCulture), "-o", directory, path],
                timeout: TimeSpan.FromSeconds(15)).GetAwaiter().GetResult();
            string thumbnail = Path.Combine(directory, Path.GetFileName(path) + ".png");
            return result.Success && File.Exists(thumbnail) ? File.ReadAllBytes(thumbnail) : null;
        }
        finally
        {
            try
            {
                Directory.Delete(directory, true);
            }
            catch (IOException)
            {
            }
        }
    }
}
