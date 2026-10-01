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

using SkiaSharp;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib;

/// <summary>Windows notification-area transport with all mouse buttons preserved.</summary>
public sealed class WindowsTrayIcon : IDisposable
{
    private const int CallbackMessage = 0x8000 + 107;
    private readonly IHotkeyHost host;
    private readonly uint taskbarCreated;
    private IntPtr icon;
    private string toolTipText;
    private bool visible, added;
    public event Action<InputMouseButton> MouseDown, MouseUp;

    public WindowsTrayIcon(IHotkeyHost host)
    {
        this.host = host;
        taskbarCreated = RegisterWindowMessage("TaskbarCreated");
        host.NativeMessageReceived += OnMessage;
    }

    public bool Visible
    {
        get => visible;
        set { visible = value; Update(); }
    }

    public string ToolTipText
    {
        get => toolTipText;
        set { toolTipText = value.Truncate(63); Update(); }
    }

    public static TimeSpan DoubleClickTime => TimeSpan.FromMilliseconds(GetDoubleClickTime());

    public void SetIcon(byte[] bytes)
    {
        using SKBitmap bitmap = SkiaImageHelpers.ByteArrayToBitmap(bytes);
        using MemoryStream stream = new();
        bitmap.Save(stream, SKEncodedImageFormat.Png);
        byte[] png = stream.ToArray();
        IntPtr replacement = CreateIconFromResourceEx(png, (uint)png.Length, true, 0x30000,
            NativeMethods.GetSystemMetrics(SystemMetric.SM_CXSMICON),
            NativeMethods.GetSystemMetrics(SystemMetric.SM_CYSMICON), 0);
        if (replacement == IntPtr.Zero) throw new Win32Exception();
        IntPtr previous = icon;
        icon = replacement;
        Update();
        if (previous != IntPtr.Zero) NativeMethods.DestroyIcon(previous);
    }

    private void Update()
    {
        if (!visible && !added) return;
        NotifyIconData data = new()
        {
            Size = (uint)Marshal.SizeOf<NotifyIconData>(),
            Window = host.Handle,
            Id = 1,
            Flags = 1 | 2 | 4, // NIF_MESSAGE | NIF_ICON | NIF_TIP
            CallbackMessage = CallbackMessage,
            Icon = icon,
            ToolTip = toolTipText ?? string.Empty
        };
        if (!visible)
        {
            ShellNotifyIcon(2, ref data); // NIM_DELETE
            added = false;
        }
        else if (icon != IntPtr.Zero)
        {
            added = ShellNotifyIcon(added ? 1u : 0u, ref data); // NIM_MODIFY / NIM_ADD
            if (!added) DebugHelper.WriteLine("Unable to update the Windows tray icon.");
        }
    }

    private void OnMessage(object sender, NativeWindowMessageEventArgs message)
    {
        if ((uint)message.Message == taskbarCreated) { added = false; Update(); return; }
        if (message.Message != CallbackMessage || message.WParam.ToInt64() != 1) return;
        int mouseMessage = (int)message.LParam.ToInt64();
        InputMouseButton button = mouseMessage switch
        {
            0x201 or 0x202 or 0x203 => InputMouseButton.Left,
            0x204 or 0x205 or 0x206 => InputMouseButton.Right,
            0x207 or 0x208 or 0x209 => InputMouseButton.Middle,
            _ => InputMouseButton.None
        };
        if (mouseMessage is 0x201 or 0x203 or 0x204 or 0x206 or 0x207 or 0x209) MouseDown?.Invoke(button);
        if (mouseMessage is 0x202 or 0x205 or 0x208) MouseUp?.Invoke(button);
        message.Handled = true;
    }

    public void Dispose()
    {
        Visible = false;
        host.NativeMessageReceived -= OnMessage;
        if (icon != IntPtr.Zero) NativeMethods.DestroyIcon(icon);
        icon = IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NotifyIconData
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
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern uint RegisterWindowMessage(string message);
    [DllImport("user32.dll")]
    private static extern uint GetDoubleClickTime();
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CreateIconFromResourceEx(byte[] bits, uint size, [MarshalAs(UnmanagedType.Bool)] bool icon,
        uint version, int width, int height, uint flags);
}
