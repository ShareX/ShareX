#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;

namespace ShareX;

public static class CustomUploaderSettingsIntegration
{
    private static CustomUploaderSettingsWindow? _window;

    public static void Show()
    {
        SettingManager.WaitUploadersConfig();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window == null)
            {
                _window = new CustomUploaderSettingsWindow(Program.UploadersConfig);
                _window.Closed += (_, _) =>
                {
                    _window = null;
                    SettingManager.SaveUploadersConfigAsync();
                    MainWindowIntegration.RefreshMenus();
                };
                _window.Show();
            }

            _window.Activate();
        });
    }

    public static void Refresh(bool selectLast = false)
    {
        Dispatcher.UIThread.Post(() => _window?.Refresh(selectLast));
    }
}
