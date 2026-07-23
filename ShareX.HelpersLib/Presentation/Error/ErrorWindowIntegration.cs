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

public static class ErrorWindowIntegration
{
    public static void Show(Exception error, string? logFilePath, string? bugReportPath) =>
        Show(error.Message, error.ToString(), logFilePath, bugReportPath, true);

    public static void Show(string errorTitle, string errorMessage, string? logFilePath, string? bugReportPath,
        bool unhandledException = true)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            DispatcherFrame frame = new();
            ShowCore(errorTitle, errorMessage, logFilePath, bugReportPath, unhandledException,
                () => frame.Continue = false);
            Dispatcher.UIThread.PushFrame(frame);
            return;
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() => ShowCore(errorTitle, errorMessage, logFilePath, bugReportPath,
            unhandledException, () => completion.TrySetResult()));
        completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(string errorTitle, string errorMessage, string? logFilePath, string? bugReportPath,
        bool unhandledException, Action completed)
    {
        try
        {
            ErrorWindow window = new(errorTitle, errorMessage, logFilePath, bugReportPath, unhandledException);
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
