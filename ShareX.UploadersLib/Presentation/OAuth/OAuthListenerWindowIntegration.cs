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

public static class OAuthListenerWindowIntegration
{
    public static OAuthListenerWindowResult? Show(IOAuth2Loopback oauth)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            OAuthListenerWindowResult? result = null;
            DispatcherFrame frame = new();
            ShowCore(oauth, value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<OAuthListenerWindowResult?> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
            ShowCore(oauth, value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(
        IOAuth2Loopback oauth,
        Action<OAuthListenerWindowResult?> completed)
    {
        try
        {
            OAuthListenerWindow window = new(oauth);
            window.Closed += (_, _) => completed(window.SubmittedResult);
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(null);
        }
    }
}
