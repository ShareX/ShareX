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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Diagnostics;

public sealed record CommandResult(int ExitCode, byte[] StandardOutput, string StandardError)
{
    public bool Success => ExitCode == 0;

    public string StandardOutputText => Encoding.UTF8.GetString(StandardOutput);
}

/// <summary>Runs external helper programs such as wl-copy, grim, screencapture or secret-tool.</summary>
public interface ICommandRunner
{
    /// <summary>Returns true when <paramref name="command"/> can be found on PATH.</summary>
    bool Exists(string command);

    Task<CommandResult> RunAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs a helper that forks a background server, for example wl-copy or xclip which keep serving the clipboard.
    /// Output is not captured because the forked child would hold the pipes open. Returns the exit code of the launched process.
    /// </summary>
    Task<int> RunForkingAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}

public sealed class CommandRunner : ICommandRunner
{
    public static CommandRunner Default { get; } = new CommandRunner();

    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    public bool Exists(string command) => FindOnPath(command) != null;

    public static string? FindOnPath(string command)
    {
        if (Path.IsPathRooted(command))
        {
            return File.Exists(command) ? command : null;
        }

        string? pathVariable = Environment.GetEnvironmentVariable("PATH");

        if (string.IsNullOrEmpty(pathVariable))
        {
            return null;
        }

        string[] extensions = OperatingSystem.IsWindows() ? [".exe", ".cmd", ".bat", ""] : [""];

        foreach (string directory in pathVariable.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (string extension in extensions)
            {
                string candidate = Path.Combine(directory, command + extension);

                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        return null;
    }

    public async Task<CommandResult> RunAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        using Process process = new Process { StartInfo = CreateStartInfo(command, arguments, standardInput != null) };
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.Start();

        using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(timeout ?? DefaultTimeout);

        try
        {
            Task writeInput = Task.CompletedTask;

            if (standardInput != null)
            {
                writeInput = WriteInputAsync(process, standardInput, timeoutSource.Token);
            }

            using MemoryStream output = new MemoryStream();
            Task readOutput = process.StandardOutput.BaseStream.CopyToAsync(output, timeoutSource.Token);
            Task<string> readError = process.StandardError.ReadToEndAsync(timeoutSource.Token);

            await Task.WhenAll(writeInput, readOutput, readError).ConfigureAwait(false);
            await process.WaitForExitAsync(timeoutSource.Token).ConfigureAwait(false);

            return new CommandResult(process.ExitCode, output.ToArray(), readError.Result);
        }
        catch (OperationCanceledException)
        {
            TryKill(process);

            if (cancellationToken.IsCancellationRequested)
            {
                throw;
            }

            throw new TimeoutException($"'{command}' did not finish within {(timeout ?? DefaultTimeout).TotalSeconds:0} seconds.");
        }
    }

    public async Task<int> RunForkingAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        using Process process = new Process { StartInfo = CreateStartInfo(command, arguments, standardInput != null) };
        process.Start();

        using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(timeout ?? DefaultTimeout);

        try
        {
            if (standardInput != null)
            {
                await WriteInputAsync(process, standardInput, timeoutSource.Token).ConfigureAwait(false);
            }

            await process.WaitForExitAsync(timeoutSource.Token).ConfigureAwait(false);
            return process.ExitCode;
        }
        catch (OperationCanceledException)
        {
            TryKill(process);

            if (cancellationToken.IsCancellationRequested)
            {
                throw;
            }

            throw new TimeoutException($"'{command}' did not finish within {(timeout ?? DefaultTimeout).TotalSeconds:0} seconds.");
        }
    }

    private static ProcessStartInfo CreateStartInfo(string command, IReadOnlyList<string> arguments, bool redirectInput)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo(command)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = redirectInput
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        return startInfo;
    }

    private static async Task WriteInputAsync(Process process, byte[] input, CancellationToken cancellationToken)
    {
        try
        {
            await process.StandardInput.BaseStream.WriteAsync(input, cancellationToken).ConfigureAwait(false);
        }
        catch (IOException)
        {
            // The helper exited without reading everything. Its exit code reports the failure.
        }
        finally
        {
            process.StandardInput.Close();
        }
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(true);
            }
        }
        catch (InvalidOperationException)
        {
        }
    }

    /// <summary>Formats arguments for logging. Not used to build command lines.</summary>
    public static string Describe(string command, IEnumerable<string> arguments) =>
        command + " " + string.Join(" ", arguments.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));
}
