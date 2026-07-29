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

public static class ParserSelectWindowIntegration
{
    public static string? Show(string[] texts)
    {
        if (texts.Length == 0)
        {
            return null;
        }

        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            string? selectedText = texts[0];
            DispatcherFrame frame = new();
            ShowCore(texts, value =>
            {
                selectedText = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return selectedText;
        }

        TaskCompletionSource<string?> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() => ShowCore(texts, value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(string[] texts, Action<string?> completed)
    {
        try
        {
            ParserSelectWindow window = new(texts);
            window.Closed += (_, _) => completed(window.SelectedText);
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(texts.Length > 0 ? texts[0] : null);
        }
    }
}
