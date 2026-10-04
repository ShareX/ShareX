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
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShareX;

/// <summary>
/// Requests the desktop sends to the running application rather than on its command line: on macOS, files opened from Finder or
/// dropped on the Dock icon, and a click on the Dock icon. Each becomes the arguments a second ShareX start would forward, so the
/// usual routing applies. Desktops that pass files as arguments never raise these.
/// </summary>
internal static class ApplicationActivation
{
    internal static void Install()
    {
        if (Application.Current?.TryGetFeature<IActivatableLifetime>() is not IActivatableLifetime lifetime)
        {
            return;
        }

        lifetime.Activated += (_, e) =>
        {
            try
            {
                switch (e)
                {
                    case FileActivatedEventArgs files:
                        string[] arguments = ToArguments(files.Files.Select(x => x.TryGetLocalPath()));

                        if (arguments.Length > 0)
                        {
                            SingleInstanceCommandRouter.ArgumentsReceived(arguments);
                        }

                        break;
                    case { Kind: ActivationKind.Reopen }:
                        // No arguments: show the main window, as starting ShareX again does.
                        SingleInstanceCommandRouter.ArgumentsReceived([]);
                        break;
                }
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
            }
        };
    }

    /// <summary>Custom uploaders and image effects are imported, as their file associations do; other files are uploaded.</summary>
    internal static string[] ToArguments(IEnumerable<string?> paths)
    {
        List<string> arguments = new();

        foreach (string? path in paths)
        {
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            string extension = Path.GetExtension(path);

            if (extension.Equals(".sxcu", StringComparison.OrdinalIgnoreCase))
            {
                arguments.Add("-CustomUploader");
            }
            else if (extension.Equals(".sxie", StringComparison.OrdinalIgnoreCase))
            {
                arguments.Add("-ImageEffect");
            }

            arguments.Add(path);
        }

        return arguments.ToArray();
    }
}
