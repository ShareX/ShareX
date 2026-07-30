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

internal sealed record LargeFileUploadWarningResult(bool ShouldContinue, bool DontShowAgain);

internal static class LargeFileUploadWarningWindowIntegration
{
    public static LargeFileUploadWarningResult Show()
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            LargeFileUploadWarningResult result = new(false, false);
            DispatcherFrame frame = new();
            ShowCore(value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<LargeFileUploadWarningResult> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() => ShowCore(value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(Action<LargeFileUploadWarningResult> completed)
    {
        try
        {
            LargeFileUploadWarningWindow window = new();
            window.Closed += (_, _) =>
                completed(new LargeFileUploadWarningResult(window.ShouldContinue, window.DontShowAgain));
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(new LargeFileUploadWarningResult(false, false));
        }
    }
}
