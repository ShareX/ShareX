#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;

namespace ShareX.HistoryLib;

public static class HistoryIntegration
{
    public static void ShowHistoryWindow(HistoryManagerSQLite historyManager, HistorySettings settings,
        HistoryWindowServices services)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() => new HistoryWindow(historyManager, settings, services).Show());
    }

    public static void ShowImageHistoryWindow(HistoryManagerSQLite historyManager, ImageHistorySettings settings,
        HistoryWindowServices services)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() => new ImageHistoryWindow(historyManager, settings, services).Show());
    }
}
