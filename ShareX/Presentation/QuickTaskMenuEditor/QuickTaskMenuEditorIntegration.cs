#region License Information (GPL v3)

/*
    ShareX - A program developed by ShareX Team
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;

namespace ShareX;

public static class QuickTaskMenuEditorIntegration
{
    private static QuickTaskMenuEditorWindow? _window;

    public static void Show()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_window == null)
            {
                _window = new QuickTaskMenuEditorWindow();
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
}
