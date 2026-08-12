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

public static class BeforeUploadWindowIntegration
{
    public static bool Show(TaskInfo info)
    {
        TaskCompletionSource<bool> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                BeforeUploadWindow window = new(info);
                window.Closed += (_, _) => completion.TrySetResult(window.Accepted);
                window.Show();
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                completion.TrySetResult(false);
            }
        });

        return completion.Task.GetAwaiter().GetResult();
    }
}
