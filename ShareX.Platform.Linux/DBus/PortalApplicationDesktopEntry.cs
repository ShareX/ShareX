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

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ShareX.Platform.Linux.DBus;

/// <summary>Provides the portal identity for ShareX runs that did not go through the Linux installer.</summary>
internal static class PortalApplicationDesktopEntry
{
    private const string Header = "[Desktop Entry]\nType=Application\nName=ShareX\nExec=";
    private const string Footer = "\nIcon=sharex\nTerminal=false\nNoDisplay=true\nX-ShareX-PortalIdentity=true\n";
    internal static void EnsureExists()
    {
        Assembly? application = Assembly.GetEntryAssembly();

        // The platform library is also used by tools and test hosts. Only the real application
        // owns sharex.desktop; never install an identity for dotnet, an editor or a test runner.
        if (application?.GetName().Name != "ShareX")
        {
            return;
        }

        EnsureExists(new XdgPathService().DataHome, Environment.GetEnvironmentVariable("XDG_DATA_DIRS"),
            Environment.ProcessPath, application.Location);
    }

    private static bool IsGeneratedEntry(string content) =>
        content.StartsWith(Header, StringComparison.Ordinal) && content.EndsWith(Footer, StringComparison.Ordinal) &&
        !content[Header.Length..^Footer.Length].Contains('\n');

    internal static bool EnsureExists(string dataHome, string? dataDirectories, string? processPath, string applicationPath)
    {
        string? temporaryPath = null;

        try
        {
            if (!Path.IsPathFullyQualified(dataHome))
            {
                return false;
            }

            string entryName = DBusSession.ApplicationId + ".desktop";
            string target = Path.Combine(dataHome, "applications", entryName);
            string directories = string.IsNullOrEmpty(dataDirectories) ? "/usr/local/share:/usr/share" : dataDirectories;

            string? previous = null;

            if (File.Exists(target))
            {
                previous = File.ReadAllText(target);

                // Refresh only our exact generated template. Package/installer entries,
                // user customizations, hidden entries and symlinks remain untouched.
                if (!IsGeneratedEntry(previous) || new FileInfo(target).LinkTarget != null)
                {
                    return true;
                }
            }
            else if (directories.Split(Path.PathSeparator).Where(Path.IsPathFullyQualified)
                .Any(directory => File.Exists(Path.Combine(directory, "applications", entryName))))
            {
                return true;
            }

            if (string.IsNullOrEmpty(processPath) || !Path.IsPathFullyQualified(processPath) ||
                !File.Exists(processPath) || processPath.Contains('='))
            {
                return false;
            }

            bool frameworkHost = Path.GetFileName(processPath) == "dotnet";

            if (frameworkHost && (!Path.IsPathFullyQualified(applicationPath) || !File.Exists(applicationPath)))
            {
                return false;
            }

            string[] arguments = frameworkHost ? [applicationPath] : [];
            string content = Header + DesktopEntry.EscapeValue(DesktopEntry.BuildExec(processPath, arguments)) + Footer;

            if (previous == content)
            {
                return true;
            }

            string directoryPath = Path.GetDirectoryName(target)!;
            Directory.CreateDirectory(directoryPath);
            temporaryPath = Path.Combine(directoryPath, ".sharex-" + Guid.NewGuid().ToString("N") + ".tmp");
            File.WriteAllText(temporaryPath, content, new UTF8Encoding(false));
            // Publish a complete entry before Registry.Register. Preserve an entry installed or
            // customized while preparing this one; only a matching generated entry is replaced.
            if (previous != null && File.ReadAllText(target) != previous)
            {
                return true;
            }

            File.Move(temporaryPath, target, overwrite: previous != null);
            return true;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            // Registration still runs and reports the portal's reason if identity is unavailable.
            return false;
        }
        finally
        {
            if (temporaryPath != null)
            {
                try
                {
                    File.Delete(temporaryPath);
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException)
                {
                }
            }
        }
    }
}
