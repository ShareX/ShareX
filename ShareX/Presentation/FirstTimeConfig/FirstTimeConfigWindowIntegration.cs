#region License Information (GPL v3)

/*
    ShareX - A program developed by ShareX Team
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using ShareX.HelpersLib;
using System;
using System.Threading.Tasks;

namespace ShareX;

public static class FirstTimeConfigWindowIntegration
{
    public static Task ShowAsync()
    {
        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() => ShowCore(() => completion.TrySetResult()));
        return completion.Task;
    }

    private static void ShowCore(Action completed)
    {
        try
        {
            FirstTimeConfigWindow window = new();
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
