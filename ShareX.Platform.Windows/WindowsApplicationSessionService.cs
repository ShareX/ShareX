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
using System.Runtime.InteropServices;

namespace ShareX.Platform.Windows;

/// <summary>Receives Windows session broadcasts on the application's UI thread.</summary>
public sealed class WindowsApplicationSessionService : IApplicationSessionService
{
    private const uint QueryEndSession = 0x0011;
    private const uint EndSession = 0x0016;
    private const long CloseApp = 0x00000001;
    private readonly Action<string> registerRestart;
    private WindowsMessageWindow? window;
    private bool disposed;

    public WindowsApplicationSessionService() : this(arguments => RegisterApplicationRestart(arguments, 0))
    {
    }

    internal WindowsApplicationSessionService(Action<string> registerRestart) => this.registerRestart = registerRestart;

    internal IntPtr WindowHandle => window?.Handle ?? IntPtr.Zero;

    public FeatureSupport RestartSupport => FeatureSupport.Supported;

    public event EventHandler? RestartRequested;
    public event EventHandler<SessionEndingEventArgs>? SessionEnding;

    public void Initialize(Action<Exception> onUnhandledException)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentNullException.ThrowIfNull(onUnhandledException);
        if (window != null)
        {
            window.VerifyAccess();
            return;
        }

        // A message-only window does not receive session broadcasts. Keep this top-level window hidden.
        window = new WindowsMessageWindow("ShareX - Session", OnMessage, onUnhandledException);
    }

    public void RegisterRestart(string arguments)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentNullException.ThrowIfNull(arguments);
        if (window == null) throw new InvalidOperationException("Initialize session notifications on the UI thread first.");
        window.VerifyAccess();
        registerRestart(arguments);
    }

    private IntPtr? OnMessage(uint message, IntPtr wParam, IntPtr lParam)
    {
        if (message == QueryEndSession)
        {
            if ((lParam.ToInt64() & CloseApp) != 0) RestartRequested?.Invoke(this, EventArgs.Empty);
            return new IntPtr(1);
        }

        if (message == EndSession)
        {
            if (wParam != IntPtr.Zero)
            {
                SessionEnding?.Invoke(this, new SessionEndingEventArgs((lParam.ToInt64() & CloseApp) != 0));
            }
            return IntPtr.Zero;
        }

        return null;
    }

    public void Dispose()
    {
        if (disposed) return;
        window?.Dispose();
        window = null;
        disposed = true;
        RestartRequested = null;
        SessionEnding = null;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
    private static extern void RegisterApplicationRestart(string commandLine, uint flags);
}
