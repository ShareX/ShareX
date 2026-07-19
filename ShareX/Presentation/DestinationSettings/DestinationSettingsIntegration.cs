#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.UploadersLib;

namespace ShareX;

public static class DestinationSettingsIntegration
{
    private static DestinationSettingsWindow? _window;

    public static void Show(IUploaderService? service = null)
    {
        SettingManager.WaitUploadersConfig();
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window == null)
            {
                _window = new DestinationSettingsWindow(Program.UploadersConfig);
                _window.Closed += (_, _) =>
                {
                    _window = null;
                    SettingManager.SaveUploadersConfigAsync();
                };
                _window.Show();
            }

            if (service != null)
            {
                _window.NavigateToService(service);
            }

            _window.Activate();
        });
    }
}
