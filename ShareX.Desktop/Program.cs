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

using ShareX.Desktop.Commands;
using ShareX.Desktop.Diagnostics;
using ShareX.Desktop.Hosting;
using ShareX.Desktop.Ipc;
using ShareX.Desktop.Settings;
using ShareX.Desktop.Workflows;
using ShareX.Platform;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Desktop;

internal static class Program
{
    private const string ApplicationName = "ShareX";

    [STAThread]
    public static int Main(string[] args)
    {
        ParseResult parsed = CommandLineParser.Parse(args);

        if (!parsed.IsSuccess)
        {
            Console.Error.WriteLine(parsed.Error);
            Console.Error.WriteLine("Run 'sharex --help' for usage.");
            return 2;
        }

        // Relative paths mean the folder the user typed the command in, not the running instance's folder, so resolve them here.
        DesktopCommand command = WithAbsolutePaths(parsed.Command!);

        switch (command.Kind)
        {
            case CommandKind.Help:
                Console.Out.Write(CommandLineParser.Usage);
                return 0;
            case CommandKind.Version:
                Console.Out.WriteLine($"ShareX {GetVersion()}");
                return 0;
        }

        // Fast path for key bindings: hand the command to the instance that is already running and leave.
        if (command.Kind is not (CommandKind.Doctor or CommandKind.Hotkeys or CommandKind.Config))
        {
            CommandResponse? response = SendToRunningInstance(command);

            if (response != null)
            {
                Console.Out.WriteLine(response.Message);
                return response.Ok ? 0 : 1;
            }

            if (command.Kind is CommandKind.Quit or CommandKind.Status)
            {
                Console.Out.WriteLine("ShareX is not running.");
                return command.Kind == CommandKind.Quit ? 0 : 3;
            }
        }

        IPlatformServices platform = PlatformBootstrap.Create();
        PlatformServices.Initialize(platform);

        try
        {
            switch (command.Kind)
            {
                case CommandKind.Doctor:
                    Console.Out.Write(Doctor.Report(platform));
                    return 0;
                case CommandKind.Hotkeys:
                    Console.Out.Write(HotkeyBindings.Report(platform.Info, Environment.ProcessPath ?? "sharex"));
                    return 0;
                case CommandKind.Config:
                    string folder = platform.Paths.GetDefaultPersonalFolder(ApplicationName);
                    Console.Out.WriteLine($"Settings:         {Path.Combine(folder, DesktopSettings.FileName)}");
                    Console.Out.WriteLine($"Upload accounts:  {Path.Combine(folder, "UploadersConfig.json")}");
                    Console.Out.WriteLine($"History:          {Path.Combine(folder, "History.db")}");
                    return 0;
            }

            string personalFolder = platform.Paths.GetDefaultPersonalFolder(ApplicationName);
            string settingsPath = Path.Combine(personalFolder, DesktopSettings.FileName);
            DesktopSettingsStore settings = new DesktopSettingsStore(settingsPath);
            UploadersUploadService uploader = new UploadersUploadService(settings, personalFolder);
            SqliteHistoryRecorder history = new SqliteHistoryRecorder(Path.Combine(personalFolder, "History.db"));
            DesktopHost host = new DesktopHost(platform, settings, uploader, history);

            InstanceServer? server = InstanceChannel.TryStartServer(InstanceChannel.GetPipeName(), host.HandleAsync);

            if (server == null)
            {
                // Another process won the race to become the instance.
                CommandResponse? response = SendToRunningInstance(command);
                Console.Out.WriteLine(response?.Message ?? "ShareX could not start.");
                return response?.Ok == true ? 0 : 1;
            }

            return host.Run(args, command, () => server.DisposeAsync().AsTask().GetAwaiter().GetResult());
        }
        finally
        {
            PlatformServices.Shutdown();
        }
    }

    internal static DesktopCommand WithAbsolutePaths(DesktopCommand command) =>
        command.Files.Count == 0 ? command : command with { Files = command.Files.Select(file => string.IsNullOrEmpty(file) ? file : Path.GetFullPath(file)).ToArray() };

    private static CommandResponse? SendToRunningInstance(DesktopCommand command)
    {
        try
        {
            return InstanceChannel.TrySendAsync(InstanceChannel.GetPipeName(), command, TimeSpan.FromMilliseconds(400), TimeSpan.FromMinutes(10)).GetAwaiter().GetResult();
        }
        catch (Exception ex) when (ex is IOException or OperationCanceledException)
        {
            return null;
        }
    }

    private static string GetVersion() =>
        typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "unknown";
}
