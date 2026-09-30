using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

/// <summary>Secret storage: Credential Manager on Windows, Keychain on macOS, Secret Service (GNOME Keyring or KWallet) on Linux.</summary>
public interface ICredentialService
{
    FeatureSupport Support { get; }

    /// <param name="service">Groups secrets, for example "ShareX".</param>
    /// <param name="account">Identifies the secret within the service, for example "Imgur".</param>
    Task<bool> StoreAsync(string service, string account, string secret, CancellationToken cancellationToken = default);

    Task<string?> GetAsync(string service, string account, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string service, string account, CancellationToken cancellationToken = default);
}
