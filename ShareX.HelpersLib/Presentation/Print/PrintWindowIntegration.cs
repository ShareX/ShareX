#region License Information (GPL v3)

/*
    ShareX - A program developed by ShareX Team
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using System;
using System.Threading.Tasks;
using DrawingImage = System.Drawing.Image;

namespace ShareX.HelpersLib;

public static class PrintWindowIntegration
{
    public static void Show(DrawingImage image, PrintSettings settings, bool previewOnly = false, Window? owner = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            DispatcherFrame frame = new();
            ShowCore(image, settings, previewOnly, owner, () => frame.Continue = false);
            Dispatcher.UIThread.PushFrame(frame);
            return;
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
            ShowCore(image, settings, previewOnly, owner, () => completion.TrySetResult()));
        completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(
        DrawingImage image,
        PrintSettings settings,
        bool previewOnly,
        Window? owner,
        Action completed)
    {
        try
        {
            PrintWindow window = new(image, settings, previewOnly);
            window.Closed += (_, _) => completed();

            if (owner is { IsVisible: true })
            {
                _ = window.ShowDialog(owner);
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Show();
            }
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed();
        }
    }
}
