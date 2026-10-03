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

using ShareX.Platform;
using ShareX.Platform.Windows;
using SkiaSharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Xunit;

namespace ShareX.ImageEditor.Tests;

[Collection("Windows session notifications")]
public sealed class WindowsTrayTests
{
    [Fact]
    public void AvaloniaFallbackExplainsUnsupportedGesturesWithoutCreatingANativeSession()
    {
        UnsupportedTrayService service = new();
        Assert.False(service.Support.IsSupported);
        Assert.False(service.MiddleClickSupport.IsSupported);
        Assert.False(service.RightButtonSupport.IsSupported);
        Assert.False(string.IsNullOrWhiteSpace(service.MiddleClickSupport.Reason));
        Assert.False(string.IsNullOrWhiteSpace(service.RightButtonSupport.Reason));
        Assert.Null(service.CreateSession(_ => throw new InvalidOperationException()));
    }

    [WindowsTrayFact]
    [SupportedOSPlatform("windows")]
    public void NativeMessagesPreserveButtonPhasesAndRouteCloseAndHandlerFailures() => RunSta(() =>
    {
        WindowsTrayService service = new();
        Assert.True(service.Support.IsSupported);
        Assert.True(service.MiddleClickSupport.IsSupported);
        Assert.True(service.RightButtonSupport.IsSupported);
        List<string> events = new();
        List<Exception> errors = new();
        int uiThread = Environment.CurrentManagedThreadId;
        using WindowsTraySession session = new(errors.Add, (uint _, ref WindowsTraySession.NotifyIconData _) => true);
        session.MouseDown += button => { Assert.Equal(uiThread, Environment.CurrentManagedThreadId); events.Add($"down-{button}"); };
        session.MouseUp += button => { Assert.Equal(uiThread, Environment.CurrentManagedThreadId); events.Add($"up-{button}"); };
        session.CloseRequested += () => events.Add("close");
        Assert.Equal(TimeSpan.FromMilliseconds(GetDoubleClickTime()), session.DoubleClickTime);

        for (int message = 0x201; message <= 0x209; message++)
        {
            SendMessage(session.WindowHandle, WindowsTraySession.CallbackMessage, new IntPtr(1), new IntPtr(message));
        }
        Assert.Equal(new[] { "down-Left", "up-Left", "down-Left", "down-Right", "up-Right", "down-Right", "down-Middle", "up-Middle", "down-Middle" }, events);
        events.Clear();
        SendMessage(session.WindowHandle, WindowsTraySession.CallbackMessage, new IntPtr(2), new IntPtr(0x202));
        SendMessage(session.WindowHandle, WindowsTraySession.CallbackMessage, new IntPtr(1), new IntPtr(0x200));
        Assert.Empty(events);
        SendMessage(session.WindowHandle, 0x10, IntPtr.Zero, IntPtr.Zero);
        Assert.Equal(new[] { "close" }, events);
        Assert.True(IsWindow(session.WindowHandle)); // The host decides when to close and dispose.

        InvalidOperationException failure = new("Synthetic tray handler failure.");
        session.MouseUp += _ => throw failure;
        Assert.Equal(IntPtr.Zero, SendMessage(session.WindowHandle, WindowsTraySession.CallbackMessage, new IntPtr(1), new IntPtr(0x208)));
        Assert.Same(failure, Assert.Single(errors));
    });

    [WindowsTrayFact]
    [SupportedOSPlatform("windows")]
    public void VisibilityIconReplacementAndExplorerRecoveryRetainTheLegacyProtocol() => RunSta(() =>
    {
        IntPtr foreground = GetForegroundWindow();
        List<(uint Message, IntPtr Icon, string ToolTip)> notifications = new();
        List<Exception> errors = new();
        bool succeed = true;
        using WindowsTraySession session = new(errors.Add, (uint message, ref WindowsTraySession.NotifyIconData data) =>
        {
            Assert.True(IsWindow(data.Window)); // Delete happens before destroying the receiver.
            Assert.Equal(1u, data.Id);
            Assert.Equal(7u, data.Flags);
            Assert.Equal(WindowsTraySession.CallbackMessage, data.CallbackMessage);
            Assert.Equal((uint)Marshal.SizeOf<WindowsTraySession.NotifyIconData>(), data.Size);
            notifications.Add((message, data.Icon, data.ToolTip));
            return succeed;
        });
        Assert.False(IsWindowVisible(session.WindowHandle));
        byte[] png = CreatePng();
        session.ToolTipText = new string('x', 80);
        session.SetIcon(png);
        IntPtr firstIcon = session.IconHandle;
        Assert.Empty(notifications);
        VerifyIcon(firstIcon);
        session.Visible = true;
        Assert.Equal(0u, Assert.Single(notifications).Message);
        Assert.Equal(new string('x', 63), session.ToolTipText);
        Assert.Equal(session.ToolTipText, notifications[0].ToolTip);

        session.SetIcon(png);
        Assert.NotEqual(firstIcon, session.IconHandle);
        Assert.Equal(1u, notifications[^1].Message);
        Assert.False(GetIconInfo(firstIcon, out _));
        VerifyIcon(session.IconHandle);
        session.ToolTipText = "Generated fixture";
        Assert.Equal(1u, notifications[^1].Message);
        Assert.Equal("Generated fixture", notifications[^1].ToolTip);

        uint taskbarCreated = RegisterWindowMessage("TaskbarCreated");
        SendMessage(session.WindowHandle, taskbarCreated, IntPtr.Zero, IntPtr.Zero);
        Assert.Equal(0u, notifications[^1].Message);
        succeed = false;
        session.ToolTipText = "Failed modify";
        Assert.Equal(1u, notifications[^1].Message);
        succeed = true;
        session.ToolTipText = "Retry add";
        Assert.Equal(0u, notifications[^1].Message);
        session.Visible = false;
        Assert.Equal(2u, notifications[^1].Message);
        int count = notifications.Count;
        SendMessage(session.WindowHandle, taskbarCreated, IntPtr.Zero, IntPtr.Zero);
        Assert.Equal(count, notifications.Count);

        session.Visible = true;
        Assert.Equal(0u, notifications[^1].Message);
        IntPtr window = session.WindowHandle;
        IntPtr lastIcon = session.IconHandle;
        session.Dispose();
        session.Dispose();
        Assert.Equal(2u, notifications[^1].Message);
        Assert.False(GetIconInfo(lastIcon, out _));
        Assert.False(IsWindow(window));
        Assert.Empty(errors);
        Assert.Equal(foreground, GetForegroundWindow());
    });

    [WindowsTrayFact]
    [SupportedOSPlatform("windows")]
    public void FailedIconsThreadOwnershipAndRepeatedDisposalReleaseNativeResources() => RunSta(() =>
    {
        byte[] png = CreatePng();
        using Process process = Process.GetCurrentProcess();
        // Warm up the one process-wide window class before measuring owned resources.
        using (WindowsTraySession warmup = new(_ => { }, (uint _, ref WindowsTraySession.NotifyIconData _) => true)) warmup.SetIcon(png);
        uint beforeUser = GetGuiResources(process.Handle, 1);
        uint beforeGdi = GetGuiResources(process.Handle, 0);
        for (int iteration = 0; iteration < 24; iteration++)
        {
            using WindowsTraySession session = new(_ => { }, (uint _, ref WindowsTraySession.NotifyIconData _) => true);
            session.SetIcon(png);
            IntPtr icon = session.IconHandle;
            Assert.Throws<Win32Exception>(() => session.SetIcon(new byte[128]));
            Assert.Equal(icon, session.IconHandle);
            VerifyIcon(icon);
            Task.Run(() =>
            {
                Assert.Throws<InvalidOperationException>(() => session.SetIcon(png));
                Assert.Throws<InvalidOperationException>(() => session.Visible = true);
                Assert.Throws<InvalidOperationException>(() => session.ToolTipText = "Wrong thread");
                Assert.Throws<InvalidOperationException>(session.Dispose);
            }).GetAwaiter().GetResult();
            session.Dispose();
            Assert.Equal(IntPtr.Zero, session.IconHandle);
            Assert.Throws<ObjectDisposedException>(() => session.SetIcon(png));
        }
        Assert.Equal(beforeUser, GetGuiResources(process.Handle, 1));
        Assert.Equal(beforeGdi, GetGuiResources(process.Handle, 0));
    });

    private static byte[] CreatePng()
    {
        using SKBitmap bitmap = new(16, 16);
        bitmap.Erase(SKColors.Transparent);
        using SKCanvas canvas = new(bitmap);
        using SKPaint paint = new() { Color = SKColors.Red };
        canvas.DrawRect(3, 3, 10, 10, paint);
        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }

    [SupportedOSPlatform("windows")]
    private static void VerifyIcon(IntPtr icon)
    {
        Assert.True(GetIconInfo(icon, out IconInfo info));
        try
        {
            Assert.True(info.IsIcon);
            Assert.NotEqual(IntPtr.Zero, info.Color);
            Assert.NotEqual(IntPtr.Zero, info.Mask);
        }
        finally
        {
            DeleteObject(info.Color);
            DeleteObject(info.Mask);
        }
    }

    [SupportedOSPlatform("windows")]
    private static void RunSta(Action action)
    {
        Exception? failure = null;
        Thread thread = new(() => { try { action(); } catch (Exception exception) { failure = exception; } }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "Native tray fixture timed out.");
        if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct IconInfo
    {
        [MarshalAs(UnmanagedType.Bool)] public bool IsIcon;
        public uint XHotspot, YHotspot;
        public IntPtr Mask, Color;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern uint GetDoubleClickTime();
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern uint RegisterWindowMessage(string message);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindow(IntPtr window);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr window);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetIconInfo(IntPtr icon, out IconInfo info);
    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(IntPtr value);
    [DllImport("user32.dll")]
    private static extern uint GetGuiResources(IntPtr process, uint flags);
}

public sealed class WindowsTrayFactAttribute : FactAttribute
{
    public WindowsTrayFactAttribute()
    {
        if (!OperatingSystem.IsWindows()) Skip = "Requires Windows notification-area messages and icon resources.";
    }
}
