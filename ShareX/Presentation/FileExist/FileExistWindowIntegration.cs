#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using ShareX.HelpersLib;
using System;
using System.Threading.Tasks;

namespace ShareX;

public static class FileExistWindowIntegration
{
    public static string Show(string filePath)
    {

        if (Dispatcher.UIThread.CheckAccess())
        {
            string result = filePath;
            DispatcherFrame frame = new();
            ShowCore(filePath, value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<string> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() => ShowCore(filePath, value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(string filePath, Action<string> completed)
    {
        try
        {
            FileExistWindow window = new(filePath);
            window.Closed += (_, _) => completed(window.FilePath);
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(filePath);
        }
    }
}
