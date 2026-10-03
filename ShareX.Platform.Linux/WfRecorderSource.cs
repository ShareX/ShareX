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

using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux;

/// <summary>
/// Records a Wayland screen area with wf-recorder (wlr-screencopy, which Hyprland, sway and other wlroots compositors provide)
/// into a named pipe that FFmpeg reads, so ShareX's encoding settings, GIF conversion and stop handling stay the same.
/// </summary>
/// <remarks>
/// wf-recorder stores a lossless stream; FFmpeg encodes it. When ShareX stops FFmpeg, wf-recorder loses its reader and exits.
/// </remarks>
public sealed class WfRecorderSource : IScreenRecordingSource
{
    private readonly IReadOnlyList<string> arguments;
    private Process? process;
    private bool disposed;

    public WfRecorderSource(string pipePath, PlatformRectangle region, int frameRate)
    {
        PipePath = pipePath;
        arguments = CreateArguments(pipePath, region, frameRate);
    }

    public string PipePath { get; }

    /// <summary>Lossless H.264 in Matroska, which FFmpeg can read from a pipe while it is being written.</summary>
    internal static IReadOnlyList<string> CreateArguments(string pipePath, PlatformRectangle region, int frameRate) =>
    [
        "-g", LinuxScreenCaptureService.FormatGeometry(region),
        "-r", Math.Max(1, frameRate).ToString(System.Globalization.CultureInfo.InvariantCulture),
        "-c", "libx264", "-p", "preset=ultrafast", "-p", "qp=0", "-x", "yuv444p",
        "-m", "matroska", "-y", "-f", pipePath
    ];

    public void Start()
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        if (File.Exists(PipePath))
        {
            File.Delete(PipePath);
        }

        if (LibC.MakeFifo(PipePath, Convert.ToUInt32("600", 8)) != 0)
        {
            throw new Win32Exception(System.Runtime.InteropServices.Marshal.GetLastPInvokeError(), "Cannot create the recording pipe.");
        }

        ProcessStartInfo startInfo = new ProcessStartInfo("wf-recorder")
        {
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        process = Process.Start(startInfo) ?? throw new InvalidOperationException("wf-recorder could not be started.");
        process.OutputDataReceived += (_, _) => { };
        process.ErrorDataReceived += (_, _) => { };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.EnableRaisingEvents = true;
        process.Exited += OnExited;
    }

    /// <summary>
    /// If wf-recorder ends before FFmpeg opened the pipe, FFmpeg would wait for a writer forever. Opening and closing the write end
    /// gives it an end of file, so it fails and ShareX reports the error.
    /// </summary>
    private void OnExited(object? sender, EventArgs e)
    {
        if (disposed)
        {
            return;
        }

        _ = Task.Run(() =>
        {
            try
            {
                using FileStream unblock = new FileStream(PipePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }).WaitAsync(TimeSpan.FromSeconds(5));
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        if (process != null)
        {
            try
            {
                if (!process.HasExited)
                {
                    // wf-recorder finishes its file on SIGINT, as when Ctrl+C is pressed.
                    LibC.Kill(process.Id, LibC.SIGINT);

                    if (!process.WaitForExit(3000))
                    {
                        process.Kill();
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }

            process.Dispose();
        }

        try
        {
            File.Delete(PipePath);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
