#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Windows;
using System;
using System.Threading.Tasks;
using DrawingColor = System.Drawing.Color;

namespace ShareX.HelpersLib;

public static class ColorPickerWindowIntegration
{
    public static void Show(
        ColorPickerOptions? options = null,
        ScreenColorPickerOptions? screenColorPickerOptions = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                new ColorPickerWindow(options, screenColorPickerOptions).Show();
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
            }
        });
    }

    public static bool PickColor(
        DrawingColor currentColor,
        out DrawingColor selectedColor,
        ColorPickerOptions? options = null,
        Func<PointInfo>? openScreenColorPicker = null,
        ScreenColorPickerOptions? screenColorPickerOptions = null)
    {
        NativeMethods.ReleaseCapture();
        AvaloniaBootstrapper.EnsureInitialized();

        DrawingColor? result;
        if (Dispatcher.UIThread.CheckAccess())
        {
            result = null;
            DispatcherFrame frame = new();
            ShowPicker(currentColor, options, screenColorPickerOptions, openScreenColorPicker, value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
        }
        else
        {
            TaskCompletionSource<DrawingColor?> completion =
                new(TaskCreationOptions.RunContinuationsAsynchronously);
            Dispatcher.UIThread.Post(() =>
                ShowPicker(currentColor, options, screenColorPickerOptions, openScreenColorPicker,
                    value => completion.TrySetResult(value)));
            result = completion.Task.GetAwaiter().GetResult();
        }

        selectedColor = result ?? currentColor;
        return result.HasValue;
    }

    private static void ShowPicker(
        DrawingColor currentColor,
        ColorPickerOptions? options,
        ScreenColorPickerOptions? screenColorPickerOptions,
        Func<PointInfo>? openScreenColorPicker,
        Action<DrawingColor?> completed)
    {
        try
        {
            NativeMethods.ReleaseCapture();
            ColorPickerWindow window =
                new(currentColor, options, screenColorPickerOptions, openScreenColorPicker);
            window.Closed += (_, _) => completed(window.SelectedColor);
            window.Show();
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            completed(null);
        }
    }
}
