#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using System;

namespace ShareX.UploadersLib;

public static class DestinationSettingsIntegration
{
    private static DestinationSettingsWindow? _window;

    public static void Show(UploadersConfig config, IUploaderService? service = null, Action? onClosed = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window == null)
            {
                _window = new DestinationSettingsWindow(config);
                _window.Closed += (_, _) =>
                {
                    _window = null;
                    onClosed?.Invoke();
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
