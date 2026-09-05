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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Strings = ShareX.Tools.Localization.Strings;

namespace ShareX.Tools;

/// <summary>Small, cancellable FFmpeg jobs. No player, ffprobe, or persistent media files are required.</summary>
internal sealed class VideoTrimmerService(string ffmpegPath)
{
    internal static string Timestamp(double seconds) => seconds.ToString("0.######", CultureInfo.InvariantCulture);

    public async Task<double> GetDurationAsync(string input, CancellationToken token)
    {
        var result = await RunAsync(["-i", input], token, allowFailure: true);
        Match match = Regex.Match(result.Log, @"Duration:\s*(\d+):(\d+):(\d+(?:\.\d+)?)", RegexOptions.CultureInvariant);
        if (!match.Success || !Regex.IsMatch(result.Log, @"Stream #.*Video:"))
        {
            throw new InvalidOperationException(Strings.VideoTrimmer_InvalidVideo);
        }

        double duration = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) * 3600 +
            double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture) * 60 +
            double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
        if (!double.IsFinite(duration) || duration <= 0)
        {
            throw new InvalidOperationException(Strings.VideoTrimmer_InvalidVideo);
        }

        return duration;
    }

    public async Task<byte[]> GetFrameAsync(string input, double position, CancellationToken token)
    {
        var result = await RunAsync([
            "-v", "error", "-ss", Timestamp(position), "-threads", "2", "-i", input,
            "-map", "0:V:0", "-frames:v", "1", "-an", "-sn",
            "-vf", "scale=640:360:force_original_aspect_ratio=decrease,pad=640:360:(ow-iw)/2:(oh-ih)/2,setsar=1",
            "-threads", "1", "-c:v", "bmp", "-f", "image2pipe", "pipe:1"
        ], token);
        if (result.Data.Length == 0)
        {
            throw new InvalidOperationException(Strings.VideoTrimmer_NoFrame);
        }

        return result.Data;
    }

    internal static string[] BuildTrimArguments(string input, string output, double start, double end, bool precise)
    {
        List<string> args = ["-v", "error", "-ss", Timestamp(start), "-i", input, "-t", Timestamp(end - start),
            "-map", "0:V:0", "-map", "0:a?", "-map_chapters", "-1"];
        if (precise)
        {
            args.AddRange(["-c:v", "libx264", "-preset", "fast", "-crf", "18", "-vf", "pad=ceil(iw/2)*2:ceil(ih/2)*2",
                "-pix_fmt", "yuv420p", "-c:a", "aac", "-b:a", "192k", "-movflags", "+faststart"]);
        }
        else
        {
            args.AddRange(["-map", "0:s?", "-c", "copy", "-avoid_negative_ts", "make_zero"]);
        }

        args.AddRange(["-progress", "pipe:1", "-nostats", "-n", output]);
        return args.ToArray();
    }

    public async Task TrimAsync(string input, string output, double start, double end, double duration,
        bool precise, IProgress<double> progress, CancellationToken token)
    {
        if (!double.IsFinite(start) || !double.IsFinite(end) || start < 0 || end <= start || end > duration)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        if (string.Equals(Path.GetFullPath(input), Path.GetFullPath(output), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(Strings.VideoTrimmer_SourceOverwrite);
        }

        string extension = precise ? ".mp4" : Path.GetExtension(input);
        if (!string.Equals(Path.GetExtension(output), extension, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(string.Format(Strings.VideoTrimmer_OutputExtension, extension));
        }

        // Publish only after success; cancellation or a muxer failure leaves an existing destination intact.
        string temporary = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(output))!, $".sharex-trim-{Guid.NewGuid():N}{extension}");
        try
        {
            await RunAsync(BuildTrimArguments(input, temporary, start, end, precise), token, progress: progress, duration: end - start);
            token.ThrowIfCancellationRequested();
            if (!File.Exists(temporary) || new FileInfo(temporary).Length == 0)
            {
                throw new InvalidOperationException(Strings.VideoTrimmer_EmptyOutput);
            }

            File.Move(temporary, output, true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }

    private async Task<(byte[] Data, string Log)> RunAsync(IEnumerable<string> arguments, CancellationToken token,
        bool allowFailure = false, IProgress<double>? progress = null, double duration = 0)
    {
        token.ThrowIfCancellationRequested();
        using Process process = new();
        process.StartInfo = new ProcessStartInfo(ffmpegPath)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        foreach (string arg in new[] { "-hide_banner", "-nostdin" }.Concat(arguments))
        {
            process.StartInfo.ArgumentList.Add(arg);
        }

        process.Start();
        using CancellationTokenRegistration registration = token.Register(() =>
        {
            try { process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { }
            catch (System.ComponentModel.Win32Exception) { }
        });
        StringBuilder log = new();
        async Task ReadErrorsAsync()
        {
            while (await process.StandardError.ReadLineAsync() is { } line)
            {
                log.AppendLine(line);
                if (log.Length > 32768) log.Remove(0, log.Length - 32768);
            }
        }

        using MemoryStream data = new();
        async Task ReadOutputAsync()
        {
            if (progress == null)
            {
                await process.StandardOutput.BaseStream.CopyToAsync(data);
                return;
            }

            while (await process.StandardOutput.ReadLineAsync() is { } line)
            {
                if (line.StartsWith("out_time_us=", StringComparison.Ordinal) &&
                    long.TryParse(line.AsSpan(12), CultureInfo.InvariantCulture, out long microseconds))
                {
                    progress.Report(Math.Clamp(microseconds / 1000000d / duration * 100, 0, 100));
                }
            }
        }

        await Task.WhenAll(ReadErrorsAsync(), ReadOutputAsync(), process.WaitForExitAsync());
        token.ThrowIfCancellationRequested();
        if (!allowFailure && process.ExitCode != 0)
        {
            throw new InvalidOperationException(log.ToString().Trim());
        }

        return (data.ToArray(), log.ToString());
    }
}
