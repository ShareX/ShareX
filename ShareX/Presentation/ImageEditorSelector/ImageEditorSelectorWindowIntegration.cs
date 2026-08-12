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

public static class ImageEditorSelectorWindowIntegration
{
    public static bool? Show()
    {

        if (Dispatcher.UIThread.CheckAccess())
        {
            bool? result = null;
            DispatcherFrame frame = new();
            ShowCore(value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<bool?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() => ShowCore(value => completion.TrySetResult(value)));
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(Action<bool?> completed)
    {
        try
        {
            ImageEditorSelectorWindow window = new();
            window.Closed += (_, _) => completed(window.UseLegacyImageEditor);

            if (MainWindowIntegration.Instance is { IsVisible: true } owner)
            {
                _ = window.ShowDialog(owner);
            }
            else
            {
                window.WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen;
                window.Show();
            }
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(null);
        }
    }
}
