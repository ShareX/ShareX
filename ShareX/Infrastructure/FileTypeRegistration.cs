#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

namespace ShareX;

internal static class FileTypeRegistration
{
    internal static void RegisterMissingExtensions()
    {
#if !MicrosoftStore
        if (StartupOptions.Portable)
        {
            return;
        }

        if (!IntegrationHelpers.CheckCustomUploaderExtension())
        {
            IntegrationHelpers.CreateCustomUploaderExtension(true);
        }

        if (!IntegrationHelpers.CheckImageEffectExtension())
        {
            IntegrationHelpers.CreateImageEffectExtension(true);
        }
#endif
    }
}
