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

using System;
using System.Threading.Tasks;

namespace ShareX.AvaloniaUI.Integration;

/// <summary>Coordinates welcome and main-window presentation with command readiness and application shutdown.</summary>
public static class ApplicationStartupPresentation
{
    public static async Task RunAsync(bool welcomePending, bool hasInitialActions,
        Func<Task> initializeRuntimeAsync, Func<Task> showWelcomeAsync, Action initializeMainWindow,
        Action markReady, Func<Task> executeInitialAsync, Action finishPresentation,
        Func<bool> isClosing, Action completeStartup)
    {
        try
        {
            await initializeRuntimeAsync();
            if (isClosing()) return;

            if (welcomePending && !hasInitialActions)
            {
                // Show welcome before releasing received commands. Main-window construction waits for the chosen language.
                Task welcome = showWelcomeAsync();
                markReady();
                await welcome;
                welcomePending = false;
                if (isClosing()) return;
            }

            initializeMainWindow();
            markReady();
            if (isClosing()) return;

            await executeInitialAsync();
            if (isClosing()) return;

            if (welcomePending)
            {
                await showWelcomeAsync();
                if (isClosing()) return;
            }

            finishPresentation();
        }
        finally
        {
            completeStartup();
        }
    }
}
