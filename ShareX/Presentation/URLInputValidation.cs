#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using System;

namespace ShareX;

internal static class URLInputValidation
{
    public static bool IsSupported(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) &&
            (uri.Scheme == Uri.UriSchemeHttp ||
             uri.Scheme == Uri.UriSchemeHttps ||
             uri.Scheme == Uri.UriSchemeFtp);
    }
}
