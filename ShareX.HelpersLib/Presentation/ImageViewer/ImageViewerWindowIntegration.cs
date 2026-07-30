#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using DrawingImage = System.Drawing.Image;

namespace ShareX.HelpersLib;

public static class ImageViewerWindowIntegration
{
    public static void ShowImage() => ShowWindow(() => new ImageViewerWindow());

    public static void ShowImage(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            ShowImage();
            return;
        }

        ShowWindow(() => new ImageViewerWindow(filePath));
    }

    public static void ShowImage(IReadOnlyList<string> filePaths, int selectedIndex = 0)
    {
        if (filePaths.Count > 0)
        {
            string[] files = new string[filePaths.Count];
            for (int index = 0; index < filePaths.Count; index++)
            {
                files[index] = filePaths[index];
            }

            ShowWindow(() => new ImageViewerWindow(files, selectedIndex));
        }
    }

    public static void ShowImage(
        byte[]? imageData,
        string? displayName = null,
        Window? owner = null)
    {
        if (imageData is not { Length: > 0 })
        {
            return;
        }

        byte[] data = (byte[])imageData.Clone();
        ShowWindow(() => new ImageViewerWindow(data, displayName), owner);
    }

    public static void ShowImage(DrawingImage? image, Window? owner = null)
    {
        if (image == null)
        {
            return;
        }

        try
        {
            using DrawingImage? clonedImage = image.CloneSafe();
            if (clonedImage == null)
            {
                return;
            }

            using MemoryStream stream = new();
            clonedImage.Save(stream, ImageFormat.Png);
            ShowImage(stream.ToArray(), owner: owner);
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception, "Failed to prepare image viewer data.");
        }
    }

    private static void ShowWindow(Func<Window> windowFactory, Window? owner = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        void ShowCore()
        {
            try
            {
                Window window = windowFactory();
                if (owner is { IsVisible: true })
                {
                    window.Show(owner);
                }
                else
                {
                    window.Show();
                }
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
            }
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            ShowCore();
        }
        else
        {
            Dispatcher.UIThread.Post(ShowCore);
        }
    }
}
