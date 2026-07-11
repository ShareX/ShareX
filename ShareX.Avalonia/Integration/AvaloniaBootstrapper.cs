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

namespace ShareX.AvaloniaUI.Integration;

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
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        base.OnFrameworkInitializationCompleted();
    }
}

public static class AvaloniaBootstrapper
{
    private static readonly object SyncRoot = new();
    private static bool _initialized;

    public static void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        lock (SyncRoot)
        {
            if (_initialized)
            {
                return;
            }

            if (Application.Current == null)
            {
                AppBuilder builder = AppBuilder.Configure<ShareXAvaloniaApplication>()
                    .UsePlatformDetect()
                    .WithInterFont();

#if DEBUG
                builder = builder.LogToTrace();
#endif

                builder.SetupWithoutStarting();
            }

            _initialized = true;
        }
    }
}
