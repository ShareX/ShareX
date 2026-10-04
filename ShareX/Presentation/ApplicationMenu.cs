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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using ShareX.Localization;

namespace ShareX;

/// <summary>
/// The application menu macOS shows in the menu bar next to the Apple menu: About, Settings (Command+Comma) and Quit (Command+Q),
/// instead of Avalonia's default "About Avalonia". Other systems do not show an application-level menu, so it is harmless there.
/// </summary>
internal static class ApplicationMenu
{
    internal static void Install()
    {
        if (Application.Current is not Application application)
        {
            return;
        }

        NativeMenuItem about = new NativeMenuItem(Strings.MainMenuBuilder_About);
        about.Click += (_, _) => AboutWindowIntegration.Show();

        NativeMenuItem settings = new NativeMenuItem(Strings.MainMenuBuilder_ApplicationSettings)
        {
            Gesture = new KeyGesture(Key.OemComma, KeyModifiers.Meta)
        };
        settings.Click += (_, _) => ApplicationSettingsIntegration.Show();

        NativeMenuItem quit = new NativeMenuItem(Strings.MainMenuBuilder_Exit)
        {
            Gesture = new KeyGesture(Key.Q, KeyModifiers.Meta)
        };
        quit.Click += (_, _) => ApplicationLifecycle.Exit();

        NativeMenu menu = new NativeMenu();
        menu.Items.Add(about);
        menu.Items.Add(new NativeMenuItemSeparator());
        menu.Items.Add(settings);
        menu.Items.Add(new NativeMenuItemSeparator());
        menu.Items.Add(quit);
        NativeMenu.SetMenu(application, menu);
    }
}
