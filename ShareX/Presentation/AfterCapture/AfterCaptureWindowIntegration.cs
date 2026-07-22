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

public static class AfterCaptureWindowIntegration
{
    public static void Show(
        TaskSettings taskSettings,
        TaskMetadata? metadata,
        string? filePath,
        Action<AfterCaptureWindowResult> completed)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                AfterCaptureWindow window = new(taskSettings, metadata, filePath);
                window.Closed += (_, _) => completed(window.Result);
                window.Show();
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                completed(new AfterCaptureWindowResult(false, null));
            }
        });
    }
}
