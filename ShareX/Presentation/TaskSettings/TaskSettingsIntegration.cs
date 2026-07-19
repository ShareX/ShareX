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

public static class TaskSettingsIntegration
{
    private static TaskSettingsWindow? _window;

    public static void Show(TaskSettings settings, bool isDefault, Action? closed = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window != null)
            {
                _window.Activate();
                return;
            }

            if (!isDefault)
            {
                settings.SetDefaultSettings();
            }

            _window = new TaskSettingsWindow(settings, isDefault);
            _window.Closed += (_, _) =>
            {
                _window = null;
                closed?.Invoke();
            };
            _window.Show();
        });
    }
}
