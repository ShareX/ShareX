#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.Steam
{
    public static class Launcher
    {
        private static string ContentFolderPath = Helpers.GetAbsolutePath("ShareX");
        private static string ContentExecutablePath = Path.Combine(ContentFolderPath, "ShareX.exe");
        private static string ContentSteamFilePath = Path.Combine(ContentFolderPath, "Steam");
        private static string UpdatingTempFilePath = Path.Combine(ContentFolderPath, "Updating");

        private static string UpdateFolderPath = Helpers.GetAbsolutePath("Updates");
        private static string UpdateExecutablePath = Path.Combine(UpdateFolderPath, "ShareX.exe");

        private static bool IsFirstTimeRunning, IsStartupRun, ShowInApp, IsSteamInit;
        private static Stopwatch SteamInitStopwatch;

        public static void Run(string[] args)
        {
            if (Helpers.IsCommandExist(args, "-uninstall"))
            {
                UninstallShareX();
                return;
            }

            IsStartupRun = Helpers.IsCommandExist(args, "-silent");

            ShowInApp = File.Exists(ContentSteamFilePath);

            if (!IsShareXRunning())
            {
                // If running on startup and need to show "In-app" then wait for Steam to run.
                if (IsStartupRun && ShowInApp)
                {
                    for (int i = 0; i < 30 && !SteamAPI.IsSteamRunning(); i++)
                    {
                        Thread.Sleep(1000);
                    }
                }

                if (SteamAPI.IsSteamRunning())
                {
                    // Even "IsSteamRunning" is true still Steam API init can fail, therefore need to give more time for Steam to launch.
                    for (int i = 0; i < 10; i++)
                    {
                        IsSteamInit = SteamAPI.Init();

                        if (IsSteamInit)
                        {
                            SteamInitStopwatch = Stopwatch.StartNew();
                            break;
                        }

                        Thread.Sleep(1000);
                    }
                }

                if (IsUpdateRequired())
                {
                    UpdateShareX();
                }

                if (IsSteamInit)
                {
                    SteamAPI.Shutdown();
                }
            }

            if (File.Exists(ContentExecutablePath))
            {
                string arguments = "";

                if (IsFirstTimeRunning)
                {
                    // Show first time config window.
                    arguments = "-SteamConfig";
                }
                else if (IsStartupRun)
                {
                    // Don't show ShareX main window.
                    arguments = "-silent";
                }

                RunShareX(arguments);

                if (IsSteamInit)
                {
                    // Reason for this workaround is because Steam only allows writing review if user is played the game at least 5 minutes.
                    // For this reason ShareX launcher will stay on for at least 10 seconds to let users eventually reach 5 minutes play time.
                    int waitTime = 10000;

                    if (SteamInitStopwatch != null)
                    {
                        waitTime -= (int)SteamInitStopwatch.ElapsedMilliseconds;
                    }

                    if (waitTime > 0)
                    {
                        Thread.Sleep(waitTime);
                    }
                }
            }
        }

        private static bool IsShareXRunning()
        {
            // Check ShareX mutex.
            return Helpers.IsRunning("82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC");
        }

        private static bool IsUpdateRequired()
        {
            try
            {
                // Update not exists?
                if (!File.Exists(UpdateExecutablePath))
                {
                    return false;
                }

                // First time running?
                if (!File.Exists(ContentExecutablePath))
                {
                    IsFirstTimeRunning = true;
                    return true;
                }

                // Need repair?
                if (File.Exists(UpdatingTempFilePath))
                {
                    return true;
                }

                // Need update?
                FileVersionInfo contentVersionInfo = FileVersionInfo.GetVersionInfo(ContentExecutablePath);
                FileVersionInfo updateVersionInfo = FileVersionInfo.GetVersionInfo(UpdateExecutablePath);

                return Helpers.CompareVersion(contentVersionInfo.FileVersion, updateVersionInfo.FileVersion) < 0;
            }
            catch (Exception e)
            {
                Helpers.ShowError(e);
            }

            return false;
        }

        private static void UpdateShareX()
        {
            try
            {
                if (!Directory.Exists(ContentFolderPath))
                {
                    Directory.CreateDirectory(ContentFolderPath);
                }

                // In case updating process terminate middle of it, allow launcher to repair ShareX.
                Helpers.CreateEmptyFile(UpdatingTempFilePath);
                Helpers.CopyAll(UpdateFolderPath, ContentFolderPath);
                File.Delete(UpdatingTempFilePath);

                if (IsFirstTimeRunning)
                {
                    Helpers.CreateEmptyFile(ContentSteamFilePath);
                }
            }
            catch (Exception e)
            {
                Helpers.ShowError(e);
            }
        }

        private static void RunShareX(string arguments = "")
        {
            try
            {
                if (!ShowInApp)
                {
                    // Workarounds to not show "In-Game" on Steam.

                    // Workaround 1.
                    try
                    {
                        using (ManagementClass managementClass = new ManagementClass("Win32_Process"))
                        {
                            ManagementClass processInfo = new ManagementClass("Win32_ProcessStartup");
                            processInfo.Properties["CreateFlags"].Value = 0x00000008;

                            ManagementBaseObject inParameters = managementClass.GetMethodParameters("Create");
                            inParameters["CommandLine"] = $"\"{ContentExecutablePath}\" {arguments}";
                            inParameters["ProcessStartupInformation"] = processInfo;

                            ManagementBaseObject result = managementClass.InvokeMethod("Create", inParameters, null);
                            // Returns a value of 0 (zero) if the process was successfully created, and any other number to indicate an error.
                            if (result != null && (uint)result.Properties["ReturnValue"].Value == 0)
                            {
                                return;
                            }
                        }
                    }
                    catch
                    {
                    }

                    // Workaround 2.
                    try
                    {
                        uint result = Helpers.WinExec($"\"{ContentExecutablePath}\" {arguments}", 5);

                        // If the function succeeds, the return value is greater than 31.
                        if (result > 31)
                        {
                            return;
                        }
                    }
                    catch
                    {
                    }

                    // Workaround 3.
                    try
                    {
                        string path = Path.Combine(Environment.SystemDirectory, "cmd.exe");

                        if (!File.Exists(path))
                        {
                            path = "cmd.exe";
                        }

                        using (Process process = new Process())
                        {
                            ProcessStartInfo psi = new ProcessStartInfo()
                            {
                                FileName = path,
                                Arguments = $"/C start \"\" \"{ContentExecutablePath}\" {arguments}",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };

                            process.StartInfo = psi;
                            bool result = process.Start();

                            if (result)
                            {
                                return;
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                using (Process process = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = ContentExecutablePath,
                        Arguments = arguments,
                        UseShellExecute = true
                    };

                    process.StartInfo = psi;
                    process.Start();
                }
            }
            catch (Exception e)
            {
                Helpers.ShowError(e);
            }
        }

        private static void UninstallShareX()
        {
            try
            {
                while (IsShareXRunning())
                {
                    if (MessageBox.Show("ShareX is currently running.\r\n\r\nPlease close ShareX and press \"Retry\" button after it is closed.", "ShareX - Uninstaller",
                        MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                if (Directory.Exists(ContentFolderPath))
                {
                    if (File.Exists(ContentExecutablePath))
                    {
                        using (Process process = new Process())
                        {
                            ProcessStartInfo psi = new ProcessStartInfo()
                            {
                                FileName = ContentExecutablePath,
                                Arguments = "-uninstall"
                            };

                            process.StartInfo = psi;
                            process.Start();
                            process.WaitForExit();
                        }
                    }

                    Directory.Delete(ContentFolderPath, true);
                }
            }
            catch (Exception e)
            {
                Helpers.ShowError(e);
            }
        }
    }
}