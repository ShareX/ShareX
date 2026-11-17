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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Threading;
using ShareX.Platform;

namespace ShareX.AvaloniaUI.Integration;

/// <summary>Access to desktop services for commands that do not own a visible window.</summary>
public static class DesktopServices
{
    private static Window? _serviceWindow;

    // Call on the UI thread. A hidden top level supplies clipboard, screens and pickers
    // while ShareX is running only in the notification area.
    public static Window GetWindow(Window? owner = null)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (owner != null) return owner;
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Window? window = desktop.Windows.FirstOrDefault(x => x.IsActive) ??
                desktop.Windows.FirstOrDefault(x => x.IsVisible);
            if (window != null) return window;
        }
        if (_serviceWindow == null)
        {
            _serviceWindow = new Window { ShowInTaskbar = false, Title = "ShareX" };
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                lifetime.Exit += (_, _) => { _serviceWindow?.Close(); _serviceWindow = null; };
        }
        return _serviceWindow;
    }

    /// <summary>
    /// Call before the window is first shown. When the ShareX main window is visible, the window opens
    /// centered on the main window's screen, otherwise on the screen under the mouse cursor, unless its
    /// saved position is already on that screen.
    /// </summary>
    public static void PlaceOnMainWindowScreen(Window window)
    {
        Dispatcher.UIThread.VerifyAccess();
        Window? mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (ReferenceEquals(mainWindow, window)) return;
        Screen? screen = mainWindow is { IsVisible: true } ? mainWindow.Screens.ScreenFromTopLevel(mainWindow) : GetCursorScreen(window);
        PlaceOnScreen(window, screen);
    }

    /// <summary>
    /// Call before the window is first shown. The window opens centered on the screen under the mouse cursor,
    /// which after a capture is the screen the capture was taken on, unless its saved position is already on
    /// that screen. ShareX 21 opened its windows there.
    /// </summary>
    public static void PlaceOnCursorScreen(Window window)
    {
        Dispatcher.UIThread.VerifyAccess();
        PlaceOnScreen(window, GetCursorScreen(window));
    }

    private static void PlaceOnScreen(Window window, Screen? screen)
    {
        Screen? savedScreen = window.WindowStartupLocation == WindowStartupLocation.Manual ? window.Screens.ScreenFromPoint(window.Position) : null;
        if (screen == null)
        {
            // A saved position on a screen that is no longer connected would open the window out of sight.
            if (window.WindowStartupLocation == WindowStartupLocation.Manual && savedScreen == null)
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return;
        }
        if (savedScreen?.Bounds == screen.Bounds) return;
        // Without an owner, CenterScreen centers on the screen that contains the window's position, which for a
        // new window is the primary screen. Windows Forms, which ShareX 21 used, centered on the cursor's screen.
        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        window.Position = screen.WorkingArea.Position;
    }

    private static Screen? GetCursorScreen(Window window)
    {
        if (!PlatformServices.IsInitialized || PlatformServices.Current.Windows.GetCursorPosition() is not PlatformPoint point) return null;
        return window.Screens.ScreenFromPoint(new PixelPoint(point.X, point.Y));
    }

    /// <summary>
    /// Attaches a dynamic tray icon to the application. Disposing the registration
    /// removes it from the application collection and disposes its native resources.
    /// </summary>
    public static IDisposable RegisterTrayIcon(TrayIcon icon)
    {
        Dispatcher.UIThread.VerifyAccess();
        ArgumentNullException.ThrowIfNull(icon);
        Application application = Application.Current ??
            throw new InvalidOperationException("Avalonia must be initialized before registering a tray icon.");
        TrayIcons? icons = TrayIcon.GetIcons(application);
        if (icons == null)
        {
            icons = new TrayIcons();
            TrayIcon.SetIcons(application, icons);
        }
        icons.Add(icon);
        return new TrayIconRegistration(icons, icon);
    }

    private sealed class TrayIconRegistration(TrayIcons icons, TrayIcon icon) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispatcher.UIThread.VerifyAccess();
            if (_disposed) return;
            _disposed = true;
            icons.Remove(icon);
            icon.Dispose();
        }
    }

    public static T Run<T>(Func<Task<T>> operation)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        if (!Dispatcher.UIThread.CheckAccess())
            return Dispatcher.UIThread.Invoke(() => Wait(operation()));
        return Wait(operation());
    }

    public static T Run<T>(Func<T> operation) => Run(() => Task.FromResult(operation()));

    private static T Wait<T>(Task<T> task)
    {
        if (!task.IsCompleted)
        {
            DispatcherFrame frame = new();
            _ = task.ContinueWith(_ => Dispatcher.UIThread.Post(() => frame.Continue = false,
                DispatcherPriority.Send), TaskScheduler.Default);
            Dispatcher.UIThread.PushFrame(frame);

            // The frame also ends when the application shuts down. Blocking on the unfinished task here would stop the UI thread
            // from ever finishing the shutdown (for example a file dialog that is open when the session ends).
            if (!task.IsCompleted)
            {
                throw new OperationCanceledException("The application is closing.");
            }
        }
        return task.GetAwaiter().GetResult();
    }
}
