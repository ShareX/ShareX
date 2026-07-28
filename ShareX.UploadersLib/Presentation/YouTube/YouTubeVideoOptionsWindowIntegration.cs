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

public static class YouTubeVideoOptionsWindowIntegration
{
    public static YouTubeVideoOptionsWindowResult? Show(
        string? title,
        string? description,
        YouTubeVideoPrivacy visibility)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            YouTubeVideoOptionsWindowResult? result = null;
            DispatcherFrame frame = new();
            ShowCore(title, description, visibility, value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<YouTubeVideoOptionsWindowResult?> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
            ShowCore(title, description, visibility, value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(
        string? title,
        string? description,
        YouTubeVideoPrivacy visibility,
        Action<YouTubeVideoOptionsWindowResult?> completed)
    {
        try
        {
            YouTubeVideoOptionsWindow window = new(title, description, visibility);
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
