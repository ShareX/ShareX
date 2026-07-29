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
using System.Threading.Tasks;

namespace ShareX.UploadersLib;

public static class PuushLoginWindowIntegration
{
    public static string? Show()
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            string? apiKey = null;
            DispatcherFrame frame = new();
            ShowCore(value =>
            {
                apiKey = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return apiKey;
        }

        TaskCompletionSource<string?> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() => ShowCore(value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(Action<string?> completed)
    {
        try
        {
            PuushLoginWindow window = new();
            window.Closed += (_, _) => completed(window.SubmittedApiKey);
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(null);
        }
    }
}
