#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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

using Steamworks;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.Steam
{
    public static class Launcher
    {
        private static string ContentFolderPath => Helpers.GetAbsolutePath("ShareX");
        private static string ContentExecutablePath => Path.Combine(ContentFolderPath, "ShareX.exe");
        private static string ContentSteamFilePath => Path.Combine(ContentFolderPath, "Steam");
        private static string UpdateFolderPath => Helpers.GetAbsolutePath("Updates");
        private static string UpdateExecutablePath => Path.Combine(UpdateFolderPath, "ShareX.exe");
        private static string UpdatingTempFilePath => Path.Combine(ContentFolderPath, "Updating");

        private static bool IsFirstTimeRunning { get; set; }
        private static bool IsStartupRun { get; set; }
        private static bool ShowInApp => File.Exists(ContentSteamFilePath);

        public static void Run(string[] args)
        {
            Stopwatch startTimer = Stopwatch.StartNew();

            if (Helpers.IsCommandExist(args, "-uninstall"))
            {
                UninstallShareX();
                return;
            }

            bool isSteamInit = false;

            IsStartupRun = Helpers.IsCommandExist(args, "-silent");

            if (!IsShareXRunning())
            {
                // If running on startup and need to show "In-app" then wait until Steam is open
                if (IsStartupRun && ShowInApp)
                {
                    for (int i = 0; i < 10 && !SteamAPI.IsSteamRunning(); i++)
                    {
                        Thread.Sleep(1000);
                    }
                }

                if (SteamAPI.IsSteamRunning())
                {
                    isSteamInit = SteamAPI.Init();
                }

                if (IsUpdateRequired())
                {
                    DoUpdate();
                }

                if (isSteamInit)
                {
                    SteamAPI.Shutdown();
                }
            }

            if (File.Exists(ContentExecutablePath))
            {
                string arguments = "";

                if (IsFirstTimeRunning)
                {
                    // Show first time config window
                    arguments = "-SteamConfig";
                }
                else if (IsStartupRun)
                {
                    // Don't show ShareX main window
                    arguments = "-silent";
                }

                RunShareX(arguments);

                if (isSteamInit)
                {
                    // Reason for this workaround because Steam only allows writing review if you played game at least 5 minutes
                    // So launcher will stay on for 10 seconds and eventually users can reach 5 minutes that way (It will require 30 times opening)
                    // Otherwise nobody can write review
                    int waitTime = 10000 - (int)startTimer.ElapsedMilliseconds;

                    if (waitTime > 0)
                    {
                        Thread.Sleep(waitTime);
                    }
                }
            }
        }

        private static bool IsShareXRunning()
        {
            // Check ShareX mutex
            return Helpers.IsRunning("82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC");
        }

        private static bool IsUpdateRequired()
        {
            try
            {
                // First time running?
                if (!Directory.Exists(ContentFolderPath) || !File.Exists(ContentExecutablePath))
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

        private static void DoUpdate()
        {
            try
            {
                if (!Directory.Exists(ContentFolderPath))
                {
                    Directory.CreateDirectory(ContentFolderPath);
                }

                // In case updating terminate middle of it, in next Launcher start it can repair
                File.Create(UpdatingTempFilePath).Dispose();
                Helpers.CopyAll(UpdateFolderPath, ContentFolderPath);
                File.Delete(UpdatingTempFilePath);
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
                if (ShowInApp)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        Arguments = arguments,
                        FileName = ContentExecutablePath,
                        UseShellExecute = true
                    };

                    Process.Start(startInfo);
                }
                else
                {
                    try
                    {
                        // Workaround for don't show "In-app"
                        uint result = Helpers.WinExec($"\"{ContentExecutablePath}\" {arguments}", 5);

                        // If the function succeeds, the return value is greater than 31
                        if (result > 31)
                        {
                            return;
                        }
                    }
                    catch { }

                    // Workaround 2
                    string path = Path.Combine(Environment.SystemDirectory, "cmd.exe");

                    if (!File.Exists(path))
                    {
                        path = "cmd.exe";
                    }

                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        Arguments = $"/C start \"\" \"{ContentExecutablePath}\" {arguments}",
                        CreateNoWindow = true,
                        FileName = path,
                        UseShellExecute = false
                    };

                    Process.Start(startInfo);
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
                        Process process = Process.Start(ContentExecutablePath, "-uninstall");

                        if (process != null)
                        {
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