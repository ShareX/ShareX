using ShareX.Platform.Diagnostics;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux;

/// <summary>
/// Secrets through the freedesktop Secret Service (GNOME Keyring, KWallet 5.97+, KeePassXC) using libsecret's secret-tool.
/// </summary>
/// <remarks>The secret is passed on standard input so it never appears in the process list.</remarks>
public sealed class SecretServiceCredentialService : ICredentialService
{
    private readonly ICommandRunner runner;
    private readonly LinuxDistribution distribution;

    public SecretServiceCredentialService(PlatformInfo info, ICommandRunner runner)
    {
        this.runner = runner;
        distribution = info.Distribution ?? LinuxDistribution.Unknown;
    }

    public FeatureSupport Support => runner.Exists("secret-tool") ? FeatureSupport.Supported : LinuxPackages.Missing(distribution, LinuxTool.SecretTool);

    public async Task<bool> StoreAsync(string service, string account, string secret, CancellationToken cancellationToken = default)
    {
        CommandResult result = await RunAsync(["store", "--label", $"{service}: {account}", "service", service, "account", account],
            Encoding.UTF8.GetBytes(secret), cancellationToken).ConfigureAwait(false);
        return result.Success;
    }

    public async Task<string?> GetAsync(string service, string account, CancellationToken cancellationToken = default)
    {
        CommandResult result = await RunAsync(["lookup", "service", service, "account", account], null, cancellationToken).ConfigureAwait(false);
        return result.Success ? result.StandardOutputText : null;
    }

    public async Task<bool> DeleteAsync(string service, string account, CancellationToken cancellationToken = default)
    {
        CommandResult result = await RunAsync(["clear", "service", service, "account", account], null, cancellationToken).ConfigureAwait(false);
        return result.Success;
    }

    private async Task<CommandResult> RunAsync(string[] arguments, byte[]? input, CancellationToken cancellationToken)
    {
        if (!runner.Exists("secret-tool"))
        {
            throw new PlatformNotSupportedException(Support.Reason);
        }

        // Unlocking the keyring can show a password prompt, so allow time for the user.
        return await runner.RunAsync("secret-tool", arguments, input, TimeSpan.FromMinutes(2), cancellationToken).ConfigureAwait(false);
    }
}
