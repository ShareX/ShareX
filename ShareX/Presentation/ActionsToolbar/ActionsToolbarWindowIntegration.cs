#region License Information (GPL v3)

/*
    ShareX - A program developed by ShareX Team
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;

namespace ShareX;

public static class ActionsToolbarWindowIntegration
{
    private static ActionsToolbarWindow? _window;

    public static void Show()
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window == null)
            {
                _window = new ActionsToolbarWindow();
                _window.Closed += (_, _) =>
                {
                    _window = null;
                    SettingManager.SaveApplicationConfigAsync();
                };
                _window.Show();
            }

            _window.Activate();
        });
    }

    public static void Toggle()
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window == null)
            {
                _window = new ActionsToolbarWindow();
                _window.Closed += (_, _) =>
                {
                    _window = null;
                    SettingManager.SaveApplicationConfigAsync();
                };
                _window.Show();
                _window.Activate();
            }
            else
            {
                _window.Close();
            }
        });
    }
}
