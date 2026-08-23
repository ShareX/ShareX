#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using ShareX.UploadersLib.ImageUploaders;
using ShareX.UploadersLib.TextUploaders;

namespace ShareX.UploadersLib;

public static class UploaderConfigurationClient
{
    public static Task<bool> LoginImageShackAsync(ImageShackOptions config, CancellationToken cancellationToken = default) =>
        new ImageShackUploader(APIKeys.ImageShackKey, config).GetAccessTokenAsync(cancellationToken);

    public static Task<bool> LoginPastebinAsync(PastebinSettings settings, CancellationToken cancellationToken = default) =>
        new Pastebin(APIKeys.PastebinKey, settings).LoginAsync(cancellationToken);

    public static string GetPasteEeAuthorizationURL() =>
        $"https://paste.ee/account/api/authorize/{APIKeys.Paste_eeApplicationKey}";
}
