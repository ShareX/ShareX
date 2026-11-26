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
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;

namespace ShareX.AvaloniaUI.Integration;

using ShareX.AvaloniaUI.Theming;

public sealed class ShareXAvaloniaApplication : Application
{
    public override void Initialize()
    {
        Uri baseUri = new Uri("avares://ShareX.Avalonia/");
        Resources.MergedDictionaries.Add(new ResourceInclude(baseUri)
        {
            Source = new Uri("avares://ShareX.Avalonia/Theming/ShareXTheme.axaml")
        });
        Styles.Add(new FluentTheme());
        Styles.Add(new StyleInclude(baseUri)
        {
            Source = new Uri("avares://ShareX.Avalonia/Theming/ControlStyles.axaml")
        });
        Styles.Add(new StyleInclude(baseUri)
        {
            Source = new Uri("avares://ShareX.Avalonia/Theming/ToolStyles.axaml")
        });

        MenuItem.SubmenuOpenedEvent.AddClassHandler<MenuItem>((item, _) => item.Resources["FlyoutThemeMaxWidth"] = double.PositiveInfinity);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        ThemeManager.Refresh();
        base.OnFrameworkInitializationCompleted();
    }
}

public static class AvaloniaBootstrapper
{
    private static readonly object SyncRoot = new();
    private static ClassicDesktopStyleApplicationLifetime? _desktopLifetime;
    private static string[]? _args;
    private static int _shutdownStarted;

    public static void Initialize(string[] args, Func<Task> startup, Action shutdown)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(startup);
        ArgumentNullException.ThrowIfNull(shutdown);

        lock (SyncRoot)
        {
            if (_args != null)
            {
                throw new InvalidOperationException("Avalonia is already initialized.");
            }

            // A message box shown before startup (for example when the personal folder can't be created)
            // already set up Avalonia through EnsureInitialized, so reuse the lifetime it created.
            ClassicDesktopStyleApplicationLifetime desktop = _desktopLifetime ?? Setup();

            Interlocked.Exchange(ref _shutdownStarted, 0);

            desktop.Args = args;
            desktop.Startup += (_, _) => Dispatcher.UIThread.Post(async () => await startup());
            desktop.Exit += (_, _) =>
            {
                Interlocked.Exchange(ref _shutdownStarted, 1);
                shutdown();
            };

            _args = args;
        }
    }

    public static int Run()
    {
        if (_desktopLifetime == null || _args == null)
        {
            throw new InvalidOperationException("Avalonia must be initialized before its application lifetime is started.");
        }

        return _desktopLifetime.Start(_args);
    }

    /// <summary>
    /// Sets up Avalonia if it isn't set up yet, so windows can be shown before <see cref="Initialize"/> is called
    /// or from a host that owns its own message loop.
    /// </summary>
    public static void EnsureInitialized()
    {
        if (Application.Current != null)
        {
            return;
        }

        lock (SyncRoot)
        {
            if (Application.Current == null)
            {
                Setup();
            }
        }
    }

    private static ClassicDesktopStyleApplicationLifetime Setup()
    {
        if (Application.Current != null)
        {
            throw new InvalidOperationException("Avalonia is already initialized.");
        }

        ClassicDesktopStyleApplicationLifetime desktop = new()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown
        };

        BuildAvaloniaApp().SetupWithLifetime(desktop);
        _desktopLifetime = desktop;
        return desktop;
    }

    public static void Shutdown()
    {
        if (Interlocked.Exchange(ref _shutdownStarted, 1) != 0)
        {
            return;
        }

        void ShutdownCore()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            ShutdownCore();
        }
        else
        {
            Dispatcher.UIThread.Post(ShutdownCore);
        }
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        AppBuilder builder = AppBuilder.Configure<ShareXAvaloniaApplication>()
            .UsePlatformDetect()
            .WithInterFont();

#if DEBUG
        builder = builder.LogToTrace();
#endif

        return builder;
    }
}
