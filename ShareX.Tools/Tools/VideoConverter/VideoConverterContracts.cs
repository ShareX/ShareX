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

using System.Globalization;
using System.Text.RegularExpressions;

namespace ShareX.Tools;

public enum VideoConverterCodec
{
    X264,
    X265,
    H264Nvenc,
    HevcNvenc,
    H264Amf,
    HevcAmf,
    H264Qsv,
    HevcQsv,
    Vp8,
    Vp9,
    Av1,
    Xvid,
    Gif,
    Webp,
    Apng
}

public sealed record VideoConversionRequest(string Arguments, string OutputFilePath, bool AutoOpenFolder);

public sealed record VideoConversionResult(bool Succeeded, bool WasCancelled, string? ErrorMessage = null, VideoConversionOutputInfo? OutputInfo = null);

public sealed record VideoConversionOutputInfo(long FileSize, double? AverageBitrateKbps, int? Width, int? Height)
{
    private static readonly Regex DurationRegex = new(@"Duration:\s*(\d+):(\d+):(\d+(?:\.\d+)?)", RegexOptions.CultureInvariant);
    private static readonly Regex BitrateRegex = new(@"Duration:.*?bitrate:\s*(\d+(?:\.\d+)?)\s*kb/s", RegexOptions.CultureInvariant);
    private static readonly Regex ResolutionRegex = new(@"Stream #.*?: Video: .*?, (\d+)x(\d+)", RegexOptions.CultureInvariant);

    // Parses the stream summary that "ffmpeg -i <file>" prints for the converted file.
    public static VideoConversionOutputInfo FromFFmpegOutput(string ffmpegOutput, long fileSize)
    {
        double? bitrate = null;
        Match bitrateMatch = BitrateRegex.Match(ffmpegOutput);

        if (bitrateMatch.Success && double.TryParse(bitrateMatch.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double reported) && reported > 0)
        {
            bitrate = reported;
        }
        else
        {
            // Some containers (GIF, APNG) report "bitrate: N/A", so derive it from the file size and duration.
            Match durationMatch = DurationRegex.Match(ffmpegOutput);

            if (durationMatch.Success)
            {
                double seconds = double.Parse(durationMatch.Groups[1].Value, CultureInfo.InvariantCulture) * 3600
                    + double.Parse(durationMatch.Groups[2].Value, CultureInfo.InvariantCulture) * 60
                    + double.Parse(durationMatch.Groups[3].Value, CultureInfo.InvariantCulture);

                if (seconds > 0 && fileSize > 0)
                {
                    bitrate = fileSize * 8d / 1000 / seconds;
                }
            }
        }

        int? width = null, height = null;
        Match resolutionMatch = ResolutionRegex.Match(ffmpegOutput);

        if (resolutionMatch.Success
            && int.TryParse(resolutionMatch.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out int parsedWidth)
            && int.TryParse(resolutionMatch.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out int parsedHeight)
            && parsedWidth > 0 && parsedHeight > 0)
        {
            width = parsedWidth;
            height = parsedHeight;
        }

        return new VideoConversionOutputInfo(fileSize, bitrate, width, height);
    }
}

public delegate Task<VideoConversionResult> VideoConversionHandler(
    VideoConversionRequest request,
    IProgress<double> progress,
    CancellationToken cancellationToken);
