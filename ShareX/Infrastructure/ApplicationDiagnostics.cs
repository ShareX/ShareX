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

using Avalonia.Threading;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Windows.Forms;

namespace ShareX;

internal static class ApplicationDiagnostics
{
    private static bool _exceptionHandlersEnabled;

    internal static void Initialize()
    {
        AssemblyLoadContext.Default.Resolving += ResolveLocalizedAssembly;

#if DEBUG
        if (Debugger.IsAttached)
        {
            return;
        }
#endif

        _exceptionHandlersEnabled = true;
        Application.ThreadException += OnWinFormsThreadException;
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    }

    internal static void InitializeAvalonia()
    {
        if (_exceptionHandlersEnabled)
        {
            Dispatcher.UIThread.UnhandledException += OnAvaloniaDispatcherException;
        }
    }

    internal static void WriteStartupFlags()
    {
        List<string> flags = [];
        if (ApplicationInfo.Dev) flags.Add(nameof(ApplicationInfo.Dev));
        if (StartupOptions.MultiInstance) flags.Add(nameof(StartupOptions.MultiInstance));
        if (StartupOptions.Portable) flags.Add(nameof(StartupOptions.Portable));
        if (StartupOptions.SilentRun) flags.Add(nameof(StartupOptions.SilentRun));
        if (StartupOptions.Sandbox) flags.Add(nameof(StartupOptions.Sandbox));
        if (StartupOptions.IgnoreHotkeyWarning) flags.Add(nameof(StartupOptions.IgnoreHotkeyWarning));
        if (SystemOptions.DisableUpdateCheck) flags.Add(nameof(SystemOptions.DisableUpdateCheck));
        if (SystemOptions.DisableUpload) flags.Add(nameof(SystemOptions.DisableUpload));
        if (SystemOptions.DisableLogging) flags.Add(nameof(SystemOptions.DisableLogging));

        if (flags.Count > 0)
        {
            DebugHelper.WriteLine("Flags: " + string.Join(", ", flags));
        }
    }

    private static Assembly? ResolveLocalizedAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        if (string.IsNullOrEmpty(assemblyName.CultureName) ||
            assemblyName.Name?.EndsWith(".resources", StringComparison.OrdinalIgnoreCase) != true)
        {
            return null;
        }

        string path = Path.Combine(AppContext.BaseDirectory, "Languages", assemblyName.CultureName, assemblyName.Name + ".dll");
        return File.Exists(path) ? context.LoadFromAssemblyPath(path) : null;
    }

    private static void OnWinFormsThreadException(object sender, ThreadExceptionEventArgs e) => ShowError(e.Exception);

    private static void OnAvaloniaDispatcherException(object? sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ShowError(e.Exception);
        e.Handled = true;
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception exception = e.ExceptionObject as Exception ??
            new Exception($"Unhandled exception object: {e.ExceptionObject}");
        ShowError(exception);
    }

    private static void ShowError(Exception exception)
    {
        ErrorWindowIntegration.Show(exception.Message, $"{exception}\r\n\r\n{ApplicationInfo.Title}",
            AppPaths.LogsFilePath, Links.GitHubIssues);
    }
}
