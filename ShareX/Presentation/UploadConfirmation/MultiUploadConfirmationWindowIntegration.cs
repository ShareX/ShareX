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

namespace ShareX;

internal sealed record MultiUploadConfirmationResult(bool IsConfirmed, bool DontShowAgain);

internal static class MultiUploadConfirmationWindowIntegration
{
    public static MultiUploadConfirmationResult Show(int fileCount)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            MultiUploadConfirmationResult result = new(false, false);
            DispatcherFrame frame = new();
            ShowCore(fileCount, value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<MultiUploadConfirmationResult> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
            ShowCore(fileCount, value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(int fileCount, Action<MultiUploadConfirmationResult> completed)
    {
        try
        {
            MultiUploadConfirmationWindow window = new(fileCount);
            window.Closed += (_, _) =>
                completed(new MultiUploadConfirmationResult(window.IsConfirmed, window.DontShowAgain));
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(new MultiUploadConfirmationResult(false, false));
        }
    }
}
