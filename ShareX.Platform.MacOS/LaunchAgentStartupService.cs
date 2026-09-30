using ShareX.Platform.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml.Linq;

namespace ShareX.Platform.MacOS;

/// <summary>Launch at sign in with a per user launch agent in ~/Library/LaunchAgents.</summary>
/// <remarks>
/// A notarised app bundle can use SMAppService instead, which shows ShareX by name in System Settings. The launch agent
/// works for every distribution method, including a plain .app copied from a .dmg.
/// </remarks>
public sealed class LaunchAgentStartupService : IStartupService
{
    private const string DefaultLabel = "com.getsharex.ShareX";

    private readonly string launchAgentsDirectory;
    private readonly ICommandRunner runner;

    public LaunchAgentStartupService(MacPathService paths, ICommandRunner runner)
        : this(Path.Combine(paths.LibraryDirectory, "LaunchAgents"), runner)
    {
    }

    public LaunchAgentStartupService(string launchAgentsDirectory, ICommandRunner runner)
    {
        this.launchAgentsDirectory = launchAgentsDirectory;
        this.runner = runner;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    public static string GetLabel(StartupRegistration registration) => registration.BundleIdentifier ?? DefaultLabel;

    public string GetPlistPath(StartupRegistration registration) => Path.Combine(launchAgentsDirectory, GetLabel(registration) + ".plist");

    public StartupRegistrationState GetState(StartupRegistration registration)
    {
        string path = GetPlistPath(registration);

        if (!File.Exists(path))
        {
            return StartupRegistrationState.Disabled;
        }

        string? program = ReadProgram(File.ReadAllText(path));

        if (!string.Equals(program, registration.ExecutablePath, StringComparison.Ordinal))
        {
            return StartupRegistrationState.Disabled;
        }

        // Since macOS 13 users can switch background items off in System Settings > General > Login Items.
        return IsDisabledByUser(GetLabel(registration)) ? StartupRegistrationState.DisabledByUser : StartupRegistrationState.Enabled;
    }

    public void SetEnabled(StartupRegistration registration, bool enabled)
    {
        string path = GetPlistPath(registration);

        if (enabled)
        {
            Directory.CreateDirectory(launchAgentsDirectory);
            File.WriteAllText(path, CreatePlist(registration), new UTF8Encoding(false));
        }
        else if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    internal static string CreatePlist(StartupRegistration registration)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
        builder.Append("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n");
        builder.Append("<plist version=\"1.0\">\n<dict>\n");
        builder.Append("  <key>Label</key>\n  <string>").Append(SecurityElement.Escape(GetLabel(registration))).Append("</string>\n");
        builder.Append("  <key>ProgramArguments</key>\n  <array>\n");

        foreach (string argument in new[] { registration.ExecutablePath }.Concat(registration.Arguments))
        {
            builder.Append("    <string>").Append(SecurityElement.Escape(argument)).Append("</string>\n");
        }

        builder.Append("  </array>\n");
        builder.Append("  <key>RunAtLoad</key>\n  <true/>\n");
        builder.Append("  <key>ProcessType</key>\n  <string>Interactive</string>\n");
        builder.Append("  <key>LimitLoadToSessionType</key>\n  <string>Aqua</string>\n");
        builder.Append("</dict>\n</plist>\n");
        return builder.ToString();
    }

    internal static string? ReadProgram(string plist)
    {
        try
        {
            XDocument document = XDocument.Parse(plist, LoadOptions.None);
            XElement? dict = document.Root?.Element("dict");
            XElement? key = dict?.Elements("key").FirstOrDefault(k => k.Value == "ProgramArguments");
            XElement? array = key?.ElementsAfterSelf().FirstOrDefault();
            return array?.Elements("string").FirstOrDefault()?.Value;
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }
    }

    private bool IsDisabledByUser(string label)
    {
        try
        {
            // Lines look like "com.getsharex.ShareX" => disabled (macOS 13+) or => true (older releases).
            CommandResult result = runner.RunAsync("launchctl", ["print-disabled", $"gui/{GetUserId()}"], timeout: TimeSpan.FromSeconds(5))
                .GetAwaiter().GetResult();

            return result.Success && result.StandardOutputText.Split('\n')
                .Any(line => IsDisabledLine(line, label));
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception)
        {
            return false;
        }
    }

    internal static bool IsDisabledLine(string line, string label)
    {
        string trimmed = line.Trim();

        return trimmed.StartsWith($"\"{label}\"", StringComparison.Ordinal) &&
            (trimmed.EndsWith("=> disabled", StringComparison.Ordinal) || trimmed.EndsWith("=> true", StringComparison.Ordinal));
    }

    private static string GetUserId()
    {
        string? uid = Environment.GetEnvironmentVariable("UID");
        return !string.IsNullOrEmpty(uid) ? uid : Native.LibC.getuid().ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
