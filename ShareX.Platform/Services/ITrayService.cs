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

namespace ShareX.Platform;

/// <summary>Optional native tray transport; the application uses its Avalonia tray when no session is supplied.</summary>
public interface ITrayService
{
    FeatureSupport Support { get; }
    FeatureSupport MiddleClickSupport { get; }
    FeatureSupport RightButtonSupport { get; }

    /// <summary>
    /// Creates a hidden tray receiver on the UI thread, or returns null for the Avalonia fallback.
    /// Dispose the returned session on that thread. The host routes callback errors without throwing
    /// through native callbacks, using <paramref name="onUnhandledException"/>.
    /// </summary>
    ITraySession? CreateSession(Action<Exception> onUnhandledException);
}

public enum TrayMouseButton
{
    Left,
    Middle,
    Right
}

/// <summary>A notification-area icon and its message receiver, owned by the creating UI thread.</summary>
public interface ITraySession : IDisposable
{
    event Action<TrayMouseButton>? MouseDown;
    event Action<TrayMouseButton>? MouseUp;
    event Action? CloseRequested;
    bool Visible { get; set; }
    string ToolTipText { get; set; }
    TimeSpan DoubleClickTime { get; }

    /// <summary>Replaces the icon using an encoded PNG. The session owns and releases its native icon resource.</summary>
    void SetIcon(byte[] png);
}

/// <summary>Uses the existing Avalonia tray, whose click/menu APIs do not expose these native gestures.</summary>
public sealed class UnsupportedTrayService : ITrayService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported("Native tray transport is not available; ShareX uses the desktop tray.");
    public FeatureSupport MiddleClickSupport { get; } = FeatureSupport.NotSupported("The desktop tray does not expose middle-click events.");
    public FeatureSupport RightButtonSupport { get; } = FeatureSupport.NotSupported("The desktop tray does not expose separate right-button press and release events.");
    public ITraySession? CreateSession(Action<Exception> onUnhandledException) => null;
}
