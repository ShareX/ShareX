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

using ShareX.Platform.Windows;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Xunit;

namespace ShareX.ImageEditor.Tests;

[SupportedOSPlatform("windows")]
[Collection("Windows session notifications")]
public sealed class WindowsApplicationSessionTests
{
    [WindowsSessionFact]
    public void RestartQueriesAndConfirmedShutdownHaveSeparateSynchronousTiming() => RunSta(() =>
    {
        IntPtr foreground = GetForegroundWindow();
        int uiThread = Environment.CurrentManagedThreadId;
        List<string> calls = new();
        List<Exception> errors = new();
        bool messageReturned = false;
        using WindowsApplicationSessionService service = new(arguments =>
        {
            Assert.False(messageReturned);
            Assert.Equal(uiThread, Environment.CurrentManagedThreadId);
            calls.Add(arguments);
        });
        service.RestartRequested += (_, _) =>
        {
            Assert.False(messageReturned);
            Assert.Equal(uiThread, Environment.CurrentManagedThreadId);
            calls.Add("query");
            service.RegisterRestart("-silent");
        };
        service.SessionEnding += (_, args) =>
        {
            Assert.False(messageReturned);
            Assert.Equal(uiThread, Environment.CurrentManagedThreadId);
            calls.Add(args.Restarting ? "save-restart" : "save");
        };

        Assert.Equal(IntPtr.Zero, service.WindowHandle);
        service.Initialize(errors.Add);
        IntPtr handle = service.WindowHandle;
        service.Initialize(errors.Add);
        Assert.Equal(handle, service.WindowHandle);
        Assert.True(service.RestartSupport.IsSupported);
        Assert.NotEqual(IntPtr.Zero, handle);
        Assert.False(IsWindowVisible(handle));
        Assert.Equal(handle, GetAncestor(handle, 2)); // GA_ROOT: broadcasts require a top-level window.
        Assert.Equal(GetCurrentThreadId(), GetWindowThreadProcessId(handle, out _));

        Assert.Equal(new IntPtr(1), SendMessage(handle, 0x0011, IntPtr.Zero, IntPtr.Zero));
        Assert.Empty(calls);
        foreach (long reason in new[] { 0x80000001L, 0x40000001L })
        {
            messageReturned = false;
            Assert.Equal(new IntPtr(1), SendMessage(handle, 0x0011, IntPtr.Zero, new IntPtr(reason)));
            messageReturned = true;
            Assert.Equal(new[] { "query", "-silent" }, calls);

            // Even a restart query can be cancelled by another application.
            Assert.Equal(IntPtr.Zero, SendMessage(handle, 0x0016, IntPtr.Zero, new IntPtr(reason)));
            Assert.Equal(new[] { "query", "-silent" }, calls);
            Assert.True(IsWindow(handle));
            calls.Clear();
        }

        messageReturned = false;
        Assert.Equal(IntPtr.Zero, SendMessage(handle, 0x0016, new IntPtr(1), new IntPtr(1)));
        messageReturned = true;
        Assert.Equal(new[] { "save-restart" }, calls);
        calls.Clear();

        messageReturned = false;
        Assert.Equal(IntPtr.Zero, SendMessage(handle, 0x0016, new IntPtr(7), new IntPtr(0x80000000L)));
        messageReturned = true;
        Assert.Equal(new[] { "save" }, calls);
        Assert.Empty(errors);

        Task.Run(() =>
        {
            Assert.Throws<InvalidOperationException>(() => service.Initialize(errors.Add));
            Assert.Throws<InvalidOperationException>(() => service.RegisterRestart("-silent"));
            Assert.Throws<InvalidOperationException>(service.Dispose);
        }).GetAwaiter().GetResult();
        Assert.True(IsWindow(handle));

        service.Dispose();
        service.Dispose();
        Assert.Equal(IntPtr.Zero, service.WindowHandle);
        Assert.False(IsWindow(handle));
        Assert.Throws<ObjectDisposedException>(() => service.Initialize(errors.Add));
        Assert.Equal(foreground, GetForegroundWindow());
    });

    [WindowsSessionFact]
    public void CallbackFailuresReturnToHostWithoutEscapingTheNativeProcedure() => RunSta(() =>
    {
        List<Exception> errors = new();
        InvalidOperationException failure = new("Synthetic session handler failure.");
        using WindowsApplicationSessionService service = new(_ => { });
        service.RestartRequested += (_, _) => throw failure;
        service.SessionEnding += (_, _) => throw failure;
        service.Initialize(errors.Add);

        Assert.Equal(IntPtr.Zero, SendMessage(service.WindowHandle, 0x0011, IntPtr.Zero, new IntPtr(1)));
        Assert.Same(failure, Assert.Single(errors));
        Assert.Equal(IntPtr.Zero, SendMessage(service.WindowHandle, 0x0016, IntPtr.Zero, new IntPtr(1)));
        Assert.Single(errors);
        Assert.Equal(IntPtr.Zero, SendMessage(service.WindowHandle, 0x0016, new IntPtr(1), new IntPtr(1)));
        Assert.Equal(2, errors.Count);
        Assert.Same(failure, errors[1]);
        Assert.True(IsWindow(service.WindowHandle));
    });

    [WindowsSessionFact]
    public void NativeRestartRegistrationKeepsUnicodeArgumentsAndZeroFlags() => RunSta(() =>
    {
        using Process process = Process.GetCurrentProcess();
        StringBuilder previous = new(1024);
        uint previousLength = (uint)previous.Capacity;
        int previousResult = GetApplicationRestartSettings(process.Handle, previous, ref previousLength, out uint previousFlags);
        Assert.True(previousResult == 0 || previousResult == unchecked((int)0x80070490), $"Unexpected restart-settings HRESULT: {previousResult:X8}");
        List<Exception> errors = new();
        using WindowsApplicationSessionService service = new();
        service.Initialize(errors.Add);
        service.RestartRequested += (_, _) => service.RegisterRestart("-silent");

        try
        {
            Assert.Equal(new IntPtr(1), SendMessage(service.WindowHandle, 0x0011, IntPtr.Zero, new IntPtr(1)));
            Assert.Empty(errors);
            VerifyRestartSettings(process.Handle, "-silent");
            service.RegisterRestart("-silent \"Unicode İstanbul 日本語\"");
            VerifyRestartSettings(process.Handle, "-silent \"Unicode İstanbul 日本語\"");
        }
        finally
        {
            // This test modifies only its own test process, then restores its previous registration.
            if (previousResult == 0) Marshal.ThrowExceptionForHR(RegisterApplicationRestart(previous.ToString(), previousFlags));
            else Marshal.ThrowExceptionForHR(UnregisterApplicationRestart());
        }
    });

    private static void VerifyRestartSettings(IntPtr process, string expected)
    {
        StringBuilder arguments = new(1024);
        uint length = (uint)arguments.Capacity;
        Assert.Equal(0, GetApplicationRestartSettings(process, arguments, ref length, out uint flags));
        Assert.Equal(expected, arguments.ToString());
        Assert.Equal(0u, flags);
    }

    private static void RunSta(Action action)
    {
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try { action(); }
            catch (Exception exception) { failure = exception; }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "Native session fixture timed out.");
        if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr window);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindow(IntPtr window);
    [DllImport("user32.dll")]
    private static extern IntPtr GetAncestor(IntPtr window, uint flags);
    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern int GetApplicationRestartSettings(IntPtr process, StringBuilder arguments, ref uint length, out uint flags);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern int RegisterApplicationRestart(string arguments, uint flags);
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern int UnregisterApplicationRestart();
}

public sealed class WindowsSessionFactAttribute : FactAttribute
{
    public WindowsSessionFactAttribute()
    {
        if (!OperatingSystem.IsWindows()) Skip = "Requires Windows session message delivery.";
    }
}

[CollectionDefinition("Windows session notifications", DisableParallelization = true)]
public sealed class WindowsSessionNotificationCollection;
