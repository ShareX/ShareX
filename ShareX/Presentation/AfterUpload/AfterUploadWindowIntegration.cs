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

namespace ShareX;

public static class AfterUploadWindowIntegration
{
    public static void Show(TaskInfo info)
    {
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                AfterUploadWindow window = new(info);
                window.Show();
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
            }
        });
    }
}
