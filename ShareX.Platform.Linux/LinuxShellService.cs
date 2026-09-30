using ShareX.Platform.Diagnostics;
using ShareX.Platform.Linux.DBus;
using System;
using System.IO;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux;

/// <summary>xdg-open for URLs and files, and org.freedesktop.FileManager1 to select a file in Nautilus, Dolphin, Nemo or Thunar.</summary>
public sealed class LinuxShellService : IShellService
{
    private readonly ICommandRunner runner;

    public LinuxShellService(ICommandRunner runner)
    {
        this.runner = runner;
    }

    public bool OpenUrl(string url) => Open(url);

    public bool OpenPath(string path) => Open(path);

    public bool RevealInFileManager(string path)
    {
        if (DBusSession.IsAvailable)
        {
            try
            {
                Task<bool> task = Task.Run(() => ShowItemsAsync(path));

                if (task.Wait(TimeSpan.FromSeconds(3)) && task.Result)
                {
                    return true;
                }
            }
            catch (AggregateException e) when (e.InnerException != null && DBusSession.IsExpectedFailure(e.InnerException))
            {
            }
        }

        // Without FileManager1 the best we can do is open the folder.
        string? folder = Directory.Exists(path) ? path : Path.GetDirectoryName(path);
        return folder != null && Open(folder);
    }

    private static async Task<bool> ShowItemsAsync(string path)
    {
        DBusConnection bus = await DBusSession.GetConnectionAsync().ConfigureAwait(false);
        string uri = new Uri(Path.GetFullPath(path)).AbsoluteUri;

        MessageBuffer call = DBusSession.CreateMethodCall(bus, "org.freedesktop.FileManager1", "/org/freedesktop/FileManager1",
            "org.freedesktop.FileManager1", "ShowItems", "ass",
            (ref MessageWriter writer) =>
            {
                writer.WriteArray(new[] { uri });
                writer.WriteString("");
            });

        await bus.CallMethodAsync(call).ConfigureAwait(false);
        return true;
    }

    private bool Open(string target)
    {
        // gio works inside Flatpak through the OpenURI portal, xdg-open covers everything else.
        foreach ((string command, string[] prefix) in new[] { ("xdg-open", Array.Empty<string>()), ("gio", new[] { "open" }) })
        {
            if (runner.Exists(command))
            {
                string[] arguments = [.. prefix, target];

                try
                {
                    if (runner.RunForkingAsync(command, arguments, timeout: TimeSpan.FromSeconds(10)).GetAwaiter().GetResult() == 0)
                    {
                        return true;
                    }
                }
                catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception)
                {
                }
            }
        }

        return false;
    }
}
