#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia.Threading;
using System.Drawing;

namespace ShareX.Tools;

public static class PinToScreenManager
{
    private static readonly HashSet<PinToScreenWindow> Windows = [];

    public static void Pin(byte[] imageData, PinToScreenOptions options, Action<byte[]> copyImage, Point? location = null)
    {
        if (imageData.Length == 0)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            PinToScreenWindow window = new(imageData, options, copyImage, location);
            Windows.Add(window);
            window.Closed += (_, _) => Windows.Remove(window);
            window.Show();
        });
    }

    public static void CloseAll()
    {
        Dispatcher.UIThread.Post(() =>
        {
            foreach (PinToScreenWindow window in Windows.ToArray())
            {
                window.Close();
            }
        });
    }
}
