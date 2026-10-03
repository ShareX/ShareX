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
using ShareX.Platform;
using ShareX.Platform.Linux;
using ShareX.Platform.MacOS;
using ShareX.Platform.Windows;
using System;

namespace ShareX.ImageEditor.App
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            PlatformServices.Initialize(CreatePlatformServices());

            try
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            finally
            {
                PlatformServices.Shutdown();
            }
        }

        // Platform selection is the sole startup OS check; editor code uses PlatformServices.Current.
        private static IPlatformServices CreatePlatformServices()
        {
            if (OperatingSystem.IsWindows()) return new WindowsPlatformServices();
            if (OperatingSystem.IsMacOS()) return new MacPlatformServices();
            if (OperatingSystem.IsLinux()) return new LinuxPlatformServices();
            throw new PlatformNotSupportedException("ShareX runs on Windows, macOS and Linux.");
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            AppBuilder builder = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont();

#if DEBUG
            builder = builder.LogToTrace().WithDeveloperTools();
#endif

            return builder;
        }
    }
}
