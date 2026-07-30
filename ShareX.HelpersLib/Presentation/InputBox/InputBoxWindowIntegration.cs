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
using System.Threading.Tasks;

namespace ShareX.HelpersLib;

public static class InputBoxWindowIntegration
{
    public static string? Show(
        string title,
        string? inputText = null,
        string? okText = null,
        string? cancelText = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            string? result = null;
            DispatcherFrame frame = new();
            ShowCore(title, inputText, okText, cancelText, value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<string?> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
            ShowCore(title, inputText, okText, cancelText, value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(
        string title,
        string? inputText,
        string? okText,
        string? cancelText,
        Action<string?> completed)
    {
        try
        {
            InputBoxWindow window = new(title, inputText, okText, cancelText);
            window.Closed += (_, _) => completed(window.SubmittedText);
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(null);
        }
    }
}
