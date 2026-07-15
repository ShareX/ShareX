#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.HelpersLib;
using System;

namespace ShareX;

public static class DebugLogWindowIntegration
{
    private static DebugLogWindow? _window;

    public static void Show(Logger logger, Action<string>? uploadRequested, string uploadWarning)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window != null)
            {
                if (!_window.IsVisible)
                {
                    _window.Show();
                }

                _window.Activate();
                return;
            }

            _window = new DebugLogWindow(logger, uploadRequested, uploadWarning);
            _window.Closed += (_, _) => _window = null;
            _window.Show();
        });
    }
}
