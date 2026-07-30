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

public static class OutputBoxWindowIntegration
{
    public static void Show(string text, string title, bool scrollToEnd = false)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            DispatcherFrame frame = new();
            ShowCore(text, title, scrollToEnd, () => frame.Continue = false);
            Dispatcher.UIThread.PushFrame(frame);
            return;
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
            ShowCore(text, title, scrollToEnd, () => completion.TrySetResult()));
        completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(string text, string title, bool scrollToEnd, Action completed)
    {
        try
        {
            OutputBoxWindow window = new(text, title, scrollToEnd);
            window.Closed += (_, _) => completed();
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed();
        }
    }
}
