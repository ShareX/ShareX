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

using ShareX.Desktop.Settings;
using ShareX.HelpersLib;
using ShareX.UploadersLib;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Desktop.Workflows;

/// <summary>
/// Uploads through ShareX.UploadersLib, the same uploaders and UploadersConfig.json the Windows application uses,
/// so accounts configured there work here.
/// </summary>
public sealed class UploadersUploadService : IUploadService
{
    private Func<DesktopSettings> getSettings;
    private readonly string configPath;
    private readonly object sync = new object();
    private UploadersConfig? config;
    private DateTime configWriteTime;

    // Read on every use so DesktopSettings.json edits apply without a restart.
    private DesktopSettings settings => getSettings();

    public UploadersUploadService(DesktopSettingsStore store, string personalFolder)
        : this(store.Current, personalFolder)
    {
        getSettings = () => store.Current;
    }

    public UploadersUploadService(DesktopSettings settings, string personalFolder)
    {
        getSettings = () => settings;
        configPath = Path.Combine(personalFolder, "UploadersConfig.json");
    }

    private UploadersConfig Config
    {
        get
        {
            lock (sync)
            {
                DateTime writeTime = File.Exists(configPath) ? File.GetLastWriteTimeUtc(configPath) : DateTime.MinValue;

                // Reload when the file changes, for example after the user adds an account, so a restart is not needed.
                if (config == null || writeTime != configWriteTime)
                {
                    configWriteTime = writeTime;
                    config = UploadersConfig.Load(configPath);

                    // Same as the Windows application: tokens that uploaders refresh are written back encrypted, never in plain text.
                    config.SupportDPAPIEncryption = true;
                }

                return config;
            }
        }
    }

    public bool IsConfigured(bool isImage, out string? reason)
    {
        IGenericUploaderService? service = GetService(isImage, out string name);

        if (service == null)
        {
            string key = isImage ? "ImageUploader" : "FileUploader";
            reason = string.IsNullOrWhiteSpace(name)
                ? $"No upload destination is set. Set {key} in {DesktopSettings.FileName} (for example \"Imgur\" or \"CustomImageUploader\")."
                : $"'{name}' in {DesktopSettings.FileName} is not a known uploader.";
            return false;
        }

        if (!service.CheckConfig(Config))
        {
            reason = HasSecretsFromAnotherComputer()
                ? $"{name} is not set up. {configPath} holds passwords or tokens encrypted by Windows, which only that Windows account can read. Sign in to {name} again on this computer."
                : $"{name} is not set up. Add its account to {configPath}.";
            return false;
        }

        reason = null;
        return true;
    }

    /// <summary>
    /// True when the file has values encrypted for another user or computer, for example UploadersConfig.json copied from Windows,
    /// where DPAPI encrypts them. Those values load as empty.
    /// </summary>
    private bool HasSecretsFromAnotherComputer()
    {
        try
        {
            return File.Exists(configPath) && File.ReadAllText(configPath).Contains(DPAPIEncryptedStringValueProvider.EncryptedTag, StringComparison.Ordinal);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    public async Task<UploadOutcome> UploadAsync(string fileName, byte[] data, bool isImage, CancellationToken cancellationToken)
    {
        IGenericUploaderService? service = GetService(isImage, out string name);

        if (service == null)
        {
            return UploadOutcome.Failed($"Unknown uploader '{name}'.");
        }

        try
        {
            GenericUploader uploader = service.CreateUploader(Config, new TaskReferenceHelper { DataType = isImage ? EDataType.Image : EDataType.File });

            using MemoryStream stream = new MemoryStream(data);
            UploadResult result = await uploader.UploadAsync(stream, fileName, cancellationToken).ConfigureAwait(false);

            string? url = !string.IsNullOrEmpty(result.ShortenedURL) ? result.ShortenedURL : result.URL;

            if (result.IsSuccess && !string.IsNullOrEmpty(url))
            {
                return new UploadOutcome(true, url, null);
            }

            string errors = string.Join(Environment.NewLine, new[] { result.Errors?.ToString(), uploader.Errors?.ToString() }.Where(e => !string.IsNullOrWhiteSpace(e)));

            if (string.IsNullOrWhiteSpace(errors))
            {
                string response = result.Response ?? "";
                errors = $"{name} returned no URL." + (response.Length > 0 ? " Response: " + (response.Length > 200 ? response[..200] + "..." : response) : "");
            }

            return UploadOutcome.Failed(errors.Trim());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return UploadOutcome.Failed(ex.Message);
        }
    }

    public string GetDestinationName(bool isImage)
    {
        string name = isImage ? settings.ImageUploader : settings.FileUploader;
        return string.IsNullOrWhiteSpace(name) ? "" : DestinationCatalog.GetUploaderName(isImage ? ShareX.Destinations.UploaderCategory.Image : ShareX.Destinations.UploaderCategory.File, name);
    }

    private IGenericUploaderService? GetService(bool isImage, out string name)
    {
        name = isImage ? settings.ImageUploader : settings.FileUploader;

        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return DestinationCatalog.GetService(isImage ? ShareX.Destinations.UploaderCategory.Image : ShareX.Destinations.UploaderCategory.File, name);
    }
}
