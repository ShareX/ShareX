#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using System.Drawing;

namespace ShareX;

public static class DragDropUploadWindowIntegration
{
    private static DragDropUploadWindow? _window;

    public static void Show(
        int size,
        int offset,
        ContentAlignment alignment,
        int opacity,
        int hoverOpacity,
        TaskSettings? taskSettings = null)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_window == null)
            {
                _window = new DragDropUploadWindow(size, offset, alignment, opacity, hoverOpacity);
                _window.Closed += (_, _) => _window = null;
                _window.Show();
            }

            _window.TaskSettings = taskSettings;
            _window.Activate();
        });
    }
}
