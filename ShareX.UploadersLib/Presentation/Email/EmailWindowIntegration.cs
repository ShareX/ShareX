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

public static class EmailWindowIntegration
{
    public static EmailWindowResult? Show(string? toEmail, string? subject, string? body)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            EmailWindowResult? result = null;
            DispatcherFrame frame = new();
            ShowCore(toEmail, subject, body, value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<EmailWindowResult?> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
            ShowCore(toEmail, subject, body, value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(
        string? toEmail,
        string? subject,
        string? body,
        Action<EmailWindowResult?> completed)
    {
        try
        {
            EmailWindow window = new(toEmail, subject, body);
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
