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

using ShareX.Tools.Localization;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace ShareX.Tools;

/// <summary>Cancellable FFmpeg exports. Preview, metadata and playback use Windows Media Foundation.</summary>
internal sealed class VideoTrimmerService(string ffmpegPath)
{
    public string FFmpegPath { get; set; } = ffmpegPath;
    internal static string Timestamp(double seconds) => seconds.ToString("0.######", CultureInfo.InvariantCulture);

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

    private async Task RunAsync(IEnumerable<string> arguments, CancellationToken token,
        IProgress<double> progress, double duration)
    {
        token.ThrowIfCancellationRequested();
        using Process process = new();
        process.StartInfo = new ProcessStartInfo(FFmpegPath)
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

        async Task ReadOutputAsync()
        {
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
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(log.ToString().Trim());
        }

    }
}
