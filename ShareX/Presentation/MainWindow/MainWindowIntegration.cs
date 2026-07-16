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
using System.IO;

namespace ShareX;

public static class MainWindowIntegration
{
    private static MainWindow? _window;
    private static bool _isVisible;

    public static bool IsInitialized => _window != null;
    public static bool IsVisible => _isVisible;

    public static void Initialize(MainForm host, bool show)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        RunOnUiThread(() =>
        {
            if (_window == null)
            {
                _window = new MainWindow(host);
                _window.Closed += (_, _) =>
                {
                    _window = null;
                    _isVisible = false;
                };
            }

            if (show)
            {
                _window.ShowAndActivate();
                _isVisible = true;
            }
        });
    }

    public static void Activate()
    {
        RunOnUiThread(() =>
        {
            _window?.ShowAndActivate();
            _isVisible = _window?.IsVisible == true;
        });
    }

    public static void Hide()
    {
        RunOnUiThread(() =>
        {
            _window?.HideToTray();
            _isVisible = false;
        });
    }

    public static void Close()
    {
        RunOnUiThread(() =>
        {
            _window?.CloseFromHost();
            _isVisible = false;
        });
    }

    public static void SetTitle(string title) => RunOnUiThread(() => _window?.SetTitle(title));

    public static void SetTrayVisible(bool visible) => RunOnUiThread(() => _window?.SetTrayVisible(visible));

    public static void SetTrayIcon(System.Drawing.Icon icon)
    {
        using MemoryStream stream = new();
        icon.Save(stream);
        byte[] iconBytes = stream.ToArray();
        RunOnUiThread(() => _window?.SetTrayIcon(iconBytes));
    }

    public static void ShowTrayMenu() => RunOnUiThread(() => _window?.ShowTrayMenu());

    public static void RefreshMenus() => RunOnUiThread(() => _window?.RefreshMenus());

    internal static void ReportVisibility(bool visible) => _isVisible = visible;

    private static void RunOnUiThread(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.UIThread.Post(action);
        }
    }
}
