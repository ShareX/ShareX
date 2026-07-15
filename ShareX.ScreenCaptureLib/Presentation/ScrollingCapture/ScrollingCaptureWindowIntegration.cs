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
using System.Drawing;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib;

public static class ScrollingCaptureWindowIntegration
{
    private static ScrollingCaptureWindow? _window;

    public static Task StartStopAsync(
        ScrollingCaptureOptions options,
        Action<Bitmap>? uploadRequested = null,
        Action? playNotificationSound = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                if (_window == null)
                {
                    _window = new ScrollingCaptureWindow(options, uploadRequested, playNotificationSound);
                    _window.Closed += (_, _) => _window = null;
                    _window.Show();
                }
                else
                {
                    await _window.StartStopAsync();
                }

                completion.SetResult();
            }
            catch (Exception ex)
            {
                completion.SetException(ex);
            }
        });

        return completion.Task;
    }
}
