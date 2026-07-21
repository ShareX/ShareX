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

namespace ShareX;

public static class ClipboardUploadWindowIntegration
{
    private static ClipboardUploadWindow? _window;

    public static void Show(TaskSettings taskSettings, bool showDontShowAgain = false, Action<bool>? closed = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window != null)
            {
                _window.Activate();
                return;
            }

            ClipboardUploadWindow window = new(taskSettings, showDontShowAgain);
            _window = window;
            window.Closed += (_, _) =>
            {
                bool dontShowAgain = window.DontShowAgain;
                _window = null;
                closed?.Invoke(dontShowAgain);
            };
            window.Show();
        });
    }
}
