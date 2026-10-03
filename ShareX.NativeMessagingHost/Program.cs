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

using ShareX.HelpersLib;
using ShareX.Platform;
using ShareX.Platform.Linux;
using ShareX.Platform.MacOS;
using ShareX.Platform.Windows;
using System;
using System.IO;
using System.Text;
using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;

namespace ShareX.NativeMessagingHost
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    PlatformServices.Initialize(CreateForCurrentOS());
                    HelpersLib.NativeMessagingHost host = new HelpersLib.NativeMessagingHost();
                    string input = host.Read();

                    if (!string.IsNullOrEmpty(input))
                    {
                        host.Write(input);

                        IApplicationLaunchService launch = PlatformServices.Current.ApplicationLaunch;
                        if (!launch.Support.IsSupported)
                        {
                            throw new PlatformNotSupportedException(launch.Support.Reason);
                        }

                        string filePath = launch.GetExecutablePath(AppContext.BaseDirectory, "ShareX");
                        string tempFilePath = Path.Combine(Path.GetTempPath(), "ShareX-native-" + Guid.NewGuid().ToString("N") + ".json");
                        try
                        {
                            File.WriteAllText(tempFilePath, input, Encoding.UTF8);
                            launch.LaunchDetached(filePath, ["-NativeMessagingInput", tempFilePath]);
                        }
                        catch
                        {
                            File.Delete(tempFilePath);
                            throw;
                        }
                    }
                }
                catch (Exception e)
                {
                    e.ShowError();
                }
                finally
                {
                    PlatformServices.Shutdown();
                }
            }
            else
            {
                MessageBox.Show("This executable is used to receive data from browser addon and send it to ShareX.",
                    "ShareX NativeMessagingHost", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // The only OS branch: select the implementation at process startup.
        private static IPlatformServices CreateForCurrentOS()
        {
            if (OperatingSystem.IsWindows()) return new WindowsPlatformServices();
            if (OperatingSystem.IsLinux()) return new LinuxPlatformServices();
            if (OperatingSystem.IsMacOS()) return new MacPlatformServices();
            throw new PlatformNotSupportedException("The native messaging host is unavailable on this operating system.");
        }
    }
}
