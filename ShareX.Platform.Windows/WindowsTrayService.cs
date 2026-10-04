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

using ShareX.Platform.Windows.Native;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShareX.Platform.Windows;

/// <summary>Windows notification-area transport, preserving every v22 mouse-button event.</summary>
public sealed class WindowsTrayService : ITrayService
{
    public FeatureSupport Support => FeatureSupport.Supported;
    public FeatureSupport IconAreaSupport => FeatureSupport.Supported;
    public FeatureSupport MiddleClickSupport => FeatureSupport.Supported;
    public FeatureSupport RightButtonSupport => FeatureSupport.Supported;
    public ITraySession CreateSession(Action<Exception> onUnhandledException) => new WindowsTraySession(onUnhandledException);
}

internal sealed class WindowsTraySession : ITraySession
{
    internal const uint CallbackMessage = 0x8000 + 107;
    internal delegate bool NotifyIcon(uint message, ref NotifyIconData data);
    private readonly WindowsMessageWindow window;
    private readonly NotifyIcon notifyIcon;
    private readonly uint taskbarCreated;
    private IntPtr icon;
    private string toolTipText = string.Empty;
    private bool visible, added, disposed;

    internal IntPtr WindowHandle => window.Handle;
    internal IntPtr IconHandle => icon;
    public event Action<TrayMouseButton>? MouseDown;
    public event Action<TrayMouseButton>? MouseUp;
    public event Action? CloseRequested;
    public TimeSpan DoubleClickTime { get; } = TimeSpan.FromMilliseconds(GetDoubleClickTime());

    internal WindowsTraySession(Action<Exception> onUnhandledException, NotifyIcon? notifyIcon = null)
    {
        ArgumentNullException.ThrowIfNull(onUnhandledException);
        this.notifyIcon = notifyIcon ?? ShellNotifyIcon;
        taskbarCreated = RegisterWindowMessage("TaskbarCreated");
        if (taskbarCreated == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
        window = new WindowsMessageWindow("ShareX - Tray", OnMessage, onUnhandledException);
    }

    public bool Visible
    {
        get => visible;
        set { VerifyAccess(); visible = value; Update(); }
    }

    public string ToolTipText
    {
        get => toolTipText;
        set
        {
            VerifyAccess();
            ArgumentNullException.ThrowIfNull(value);
            toolTipText = value.Length > 63 ? value[..63] : value;
            Update();
        }
    }

    public void SetIcon(byte[] png)
    {
        VerifyAccess();
        ArgumentNullException.ThrowIfNull(png);
        IntPtr replacement = CreateIconFromResourceEx(png, (uint)png.Length, true, 0x30000,
            Win32.GetSystemMetrics(49), Win32.GetSystemMetrics(50), 0);
        if (replacement == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
        IntPtr previous = icon;
        icon = replacement;
        try { Update(); }
        finally { if (previous != IntPtr.Zero) DestroyIcon(previous); }
    }

    private void VerifyAccess()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        window.VerifyAccess();
    }

    private void Update()
    {
        if (!visible && !added) return;
        NotifyIconData data = new()
        {
            Size = (uint)Marshal.SizeOf<NotifyIconData>(),
            Window = window.Handle,
            Id = 1,
            Flags = 1 | 2 | 4, // NIF_MESSAGE | NIF_ICON | NIF_TIP
            CallbackMessage = CallbackMessage,
            Icon = icon,
            ToolTip = toolTipText,
            Info = string.Empty,
            InfoTitle = string.Empty
        };
        if (!visible)
        {
            notifyIcon(2, ref data); // NIM_DELETE, before the receiver is destroyed.
            added = false;
        }
        else if (icon != IntPtr.Zero)
        {
            added = notifyIcon(added ? 1u : 0u, ref data); // NIM_MODIFY / NIM_ADD, retaining the legacy protocol.
            if (!added) Trace.WriteLine("Unable to update the Windows tray icon.");
        }
    }

    private IntPtr? OnMessage(uint message, IntPtr wParam, IntPtr lParam)
    {
        if (message == 0x0010) // WM_CLOSE, previously handled by the application host.
        {
            CloseRequested?.Invoke();
            return IntPtr.Zero;
        }
        if (message == taskbarCreated)
        {
            added = false;
            Update();
            return IntPtr.Zero;
        }
        if (message != CallbackMessage || wParam.ToInt64() != 1) return null;
        int mouseMessage = (int)lParam.ToInt64();
        TrayMouseButton? button = mouseMessage switch
        {
            0x201 or 0x202 or 0x203 => TrayMouseButton.Left,
            0x204 or 0x205 or 0x206 => TrayMouseButton.Right,
            0x207 or 0x208 or 0x209 => TrayMouseButton.Middle,
            _ => null
        };
        if (button.HasValue)
        {
            if (mouseMessage is 0x201 or 0x203 or 0x204 or 0x206 or 0x207 or 0x209) MouseDown?.Invoke(button.Value);
            if (mouseMessage is 0x202 or 0x205 or 0x208) MouseUp?.Invoke(button.Value);
        }
        return IntPtr.Zero;
    }

    public void Dispose()
    {
        if (disposed) return;
        window.VerifyAccess();
        try
        {
            visible = false;
            Update();
        }
        finally
        {
            if (icon != IntPtr.Zero) DestroyIcon(icon);
            icon = IntPtr.Zero;
            window.Dispose();
            disposed = true;
            MouseDown = null;
            MouseUp = null;
            CloseRequested = null;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct NotifyIconData
    {
        public uint Size;
        public IntPtr Window;
        public uint Id, Flags, CallbackMessage;
        public IntPtr Icon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string ToolTip;
        public uint State, StateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Info;
        public uint Version;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string InfoTitle;
        public uint InfoFlags;
        public Guid Guid;
        public IntPtr BalloonIcon;
    }

    [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShellNotifyIcon(uint message, ref NotifyIconData data);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint RegisterWindowMessage(string message);
    [DllImport("user32.dll")]
    private static extern uint GetDoubleClickTime();
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CreateIconFromResourceEx(byte[] bits, uint size, [MarshalAs(UnmanagedType.Bool)] bool icon,
        uint version, int width, int height, uint flags);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(IntPtr icon);
}
