#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;

namespace ShareX;

public static class AutoCaptureWindowIntegration
{
    private static AutoCaptureWindow? _window;

    public static bool IsRunning => _window?.IsRunning == true;

    public static void Show(TaskSettings taskSettings)
    {
        Dispatch(window =>
        {
            window.TaskSettings = taskSettings;
            window.ShowAndActivate();
        });
    }

    public static void Start(TaskSettings taskSettings)
    {
        Dispatch(window =>
        {
            if (!window.IsRunning)
            {
                window.TaskSettings = taskSettings;
                window.ShowAndActivate();
                window.Execute();
            }
        });
    }

    public static void Stop()
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_window?.IsRunning == true)
            {
                _window.Execute();
            }
        });
    }

    private static void Dispatch(System.Action<AutoCaptureWindow> action)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() => action(GetOrCreateWindow()));
    }

    private static AutoCaptureWindow GetOrCreateWindow()
    {
        if (_window == null)
        {
            _window = new AutoCaptureWindow();
            _window.Closed += (_, _) => _window = null;
        }

        return _window;
    }
}
