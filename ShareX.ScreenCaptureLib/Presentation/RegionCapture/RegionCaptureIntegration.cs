#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.ImageEditor.Integration;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>UI-thread boundary for the Avalonia region capture experience.</summary>
public static class RegionCaptureIntegration
{
    public static Rectangle LastRegionRectangle { get; internal set; }

    public static Task<AvaloniaRegionCaptureResult?> CaptureAsync(AvaloniaRegionCaptureRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            AvaloniaBootstrapper.EnsureInitialized();
        }
        catch (Exception ex)
        {
            request.Screenshot.Dispose();
            request.CursorBitmap?.Dispose();
            return Task.FromException<AvaloniaRegionCaptureResult?>(ex);
        }

        TaskCompletionSource<AvaloniaRegionCaptureResult?> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(async () =>
        {
            RegionCaptureWindow? window = null;

            try
            {
                ImageEditorIntegration.Initialize();
                window = new RegionCaptureWindow(request);
                completion.TrySetResult(await window.CaptureAsync());
            }
            catch (Exception ex)
            {
                if (window == null || !window.IsVisible)
                {
                    request.Screenshot.Dispose();
                    request.CursorBitmap?.Dispose();
                }

                completion.TrySetException(ex);
            }
        });

        return completion.Task;
    }
}
