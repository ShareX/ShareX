using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareX.Platform.Linux;

/// <summary>Launch at sign in through an XDG autostart entry in $XDG_CONFIG_HOME/autostart.</summary>
/// <remarks>Works on GNOME, KDE Plasma, Xfce, Cinnamon, MATE and LXQt. Hyprland and sway need an autostart helper such as dex or systemd-xdg-autostart-generator.</remarks>
public sealed class XdgAutostartService : IStartupService
{
    private readonly string autostartDirectory;

    public XdgAutostartService(XdgPathService paths)
        : this(Path.Combine(paths.ConfigHome, "autostart"))
    {
    }

    public XdgAutostartService(string autostartDirectory)
    {
        this.autostartDirectory = autostartDirectory;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    public string GetEntryPath(StartupRegistration registration) => Path.Combine(autostartDirectory, registration.Name + ".desktop");

    public StartupRegistrationState GetState(StartupRegistration registration)
    {
        string path = GetEntryPath(registration);

        if (!File.Exists(path))
        {
            return StartupRegistrationState.Disabled;
        }

        Dictionary<string, string> entry = DesktopEntry.Parse(File.ReadAllText(path));

        if (!entry.TryGetValue("Exec", out string? exec))
        {
            return StartupRegistrationState.Disabled;
        }

        List<string> arguments = DesktopEntry.ParseExec(exec);

        if (arguments.Count == 0 || !string.Equals(arguments[0], registration.ExecutablePath, StringComparison.Ordinal))
        {
            return StartupRegistrationState.Disabled;
        }

        // Desktop session settings (GNOME Tweaks, KDE System Settings) disable an entry rather than delete it.
        if (IsTrue(entry, "Hidden") || (entry.TryGetValue("X-GNOME-Autostart-enabled", out string? enabled) && enabled == "false"))
        {
            return StartupRegistrationState.DisabledByUser;
        }

        return StartupRegistrationState.Enabled;
    }

    public void SetEnabled(StartupRegistration registration, bool enabled)
    {
        string path = GetEntryPath(registration);

        if (enabled)
        {
            Directory.CreateDirectory(autostartDirectory);
            File.WriteAllText(path, CreateEntry(registration), new UTF8Encoding(false));
        }
        else if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    internal static string CreateEntry(StartupRegistration registration)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[Desktop Entry]\n");
        builder.Append("Type=Application\n");
        builder.Append("Version=1.5\n");
        builder.Append("Name=").Append(DesktopEntry.EscapeValue(registration.DisplayName)).Append('\n');
        builder.Append("Exec=").Append(DesktopEntry.BuildExec(registration.ExecutablePath, registration.Arguments)).Append('\n');

        if (!string.IsNullOrEmpty(registration.IconName))
        {
            builder.Append("Icon=").Append(DesktopEntry.EscapeValue(registration.IconName)).Append('\n');
        }

        builder.Append("Terminal=false\n");
        builder.Append("X-GNOME-Autostart-enabled=true\n");
        builder.Append("X-KDE-autostart-after=panel\n");
        return builder.ToString();
    }

    private static bool IsTrue(Dictionary<string, string> entry, string key) =>
        entry.TryGetValue(key, out string? value) && value.Equals("true", StringComparison.OrdinalIgnoreCase);
}
