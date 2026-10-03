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
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace ShareX.Platform.Windows;

[SupportedOSPlatform("windows")]
public sealed class WindowsApplicationLaunchService : IApplicationLaunchService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public string GetExecutablePath(string directory, string applicationName)
    {
        ArgumentException.ThrowIfNullOrEmpty(directory);
        ArgumentException.ThrowIfNullOrEmpty(applicationName);
        if (!Path.IsPathFullyQualified(directory))
        {
            throw new ArgumentException("The installation directory must be absolute.", nameof(directory));
        }
        if (applicationName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || applicationName is "." or "..")
        {
            throw new ArgumentException("The application name must be a file name without a directory.", nameof(applicationName));
        }
        return Path.Combine(directory, applicationName + ".exe");
    }

    public int LaunchDetached(string executablePath, IReadOnlyList<string> arguments)
    {
        ArgumentException.ThrowIfNullOrEmpty(executablePath);
        ArgumentNullException.ThrowIfNull(arguments);
        if (!Path.IsPathFullyQualified(executablePath) || executablePath.Contains('\0'))
        {
            throw new ArgumentException("The executable path must be absolute and contain no null characters.", nameof(executablePath));
        }

        StringBuilder commandLine = new(WindowsStartupService.QuoteArgument(executablePath));
        foreach (string argument in arguments)
        {
            if (argument is null || argument.Contains('\0'))
            {
                throw new ArgumentException("Arguments must contain no null values or characters.", nameof(arguments));
            }
            commandLine.Append(' ').Append(WindowsStartupService.QuoteArgument(argument));
        }
        // CreateProcessW's limit includes its terminating null character.
        if (commandLine.Length >= 32767)
        {
            throw new ArgumentException("The application command line exceeds the Windows limit.", nameof(arguments));
        }

        StartupInfo startup = new() { Size = (uint)Marshal.SizeOf<StartupInfo>() };
        // Preserve native messaging's CREATE_BREAKAWAY_FROM_JOB, with default security and no handle inheritance.
        if (!CreateProcess(executablePath, commandLine, IntPtr.Zero, IntPtr.Zero, false, 0x01000000,
            IntPtr.Zero, null, ref startup, out ProcessInformation process))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            return unchecked((int)process.ProcessId);
        }
        finally
        {
            CloseHandle(process.Thread);
            CloseHandle(process.Process);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct StartupInfo
    {
        public uint Size;
        public IntPtr Reserved, Desktop, Title;
        public uint X, Y, XSize, YSize, XCountChars, YCountChars, FillAttribute, Flags;
        public ushort ShowWindow, ReservedSize;
        public IntPtr ReservedBytes, StandardInput, StandardOutput, StandardError;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ProcessInformation
    {
        public IntPtr Process, Thread;
        public uint ProcessId, ThreadId;
    }

    [DllImport("kernel32.dll", EntryPoint = "CreateProcessW", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateProcess(string applicationName, StringBuilder commandLine, IntPtr processAttributes,
        IntPtr threadAttributes, [MarshalAs(UnmanagedType.Bool)] bool inheritHandles, uint creationFlags,
        IntPtr environment, string? currentDirectory, ref StartupInfo startupInfo, out ProcessInformation processInformation);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr handle);
}
