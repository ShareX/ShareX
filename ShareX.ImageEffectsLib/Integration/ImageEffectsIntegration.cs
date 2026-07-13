#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using System.Drawing;

namespace ShareX.ImageEffectsLib;

public static class ImageEffectsIntegration
{
    private static ImageEffectsWindow? _singletonWindow;

    public static ImageEffectsDialogResult ShowDialog(Bitmap? sourceImage, List<ImageEffectPreset> presets,
        int selectedPresetIndex, ImageEffectsWindowMode mode, ImageEffectsCallbacks? callbacks = null, string? filePath = null)
    {
        return ShowDialogAsync(sourceImage, presets, selectedPresetIndex, mode, callbacks, filePath)
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public static Task<ImageEffectsDialogResult> ShowDialogAsync(Bitmap? sourceImage, List<ImageEffectPreset> presets,
        int selectedPresetIndex, ImageEffectsWindowMode mode, ImageEffectsCallbacks? callbacks = null, string? filePath = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        TaskCompletionSource<ImageEffectsDialogResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            ImageEffectsWindow window = new(sourceImage, presets, selectedPresetIndex, mode, callbacks, filePath);
            window.Closed += (_, _) => completion.TrySetResult(
                new ImageEffectsDialogResult(window.Accepted, window.ViewModel.SelectedPresetIndex));
            window.Show();
        });

        return completion.Task;
    }

    public static void ShowToolWindow(Bitmap sourceImage, List<ImageEffectPreset> presets, int selectedPresetIndex,
        ImageEffectsCallbacks? callbacks = null, string? filePath = null, Action<int>? selectedPresetChanged = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Bitmap sourceCopy = (Bitmap)sourceImage.Clone();

        Dispatcher.UIThread.Post(() =>
        {
            ImageEffectsWindow window;
            using (sourceCopy)
            {
                window = new ImageEffectsWindow(sourceCopy, presets, selectedPresetIndex,
                    ImageEffectsWindowMode.Tool, callbacks, filePath);
            }
            window.Closed += (_, _) => selectedPresetChanged?.Invoke(window.ViewModel.SelectedPresetIndex);
            window.Show();
        });
    }

    public static void ShowPresetWindow(List<ImageEffectPreset> presets, int selectedPresetIndex,
        Action<int>? selectedPresetChanged = null, string? importJson = null, ImageEffectsCallbacks? callbacks = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_singletonWindow != null)
            {
                if (!string.IsNullOrWhiteSpace(importJson)) _singletonWindow.ImportPreset(importJson);
                _singletonWindow.Activate();
                return;
            }

            _singletonWindow = new ImageEffectsWindow(null, presets, selectedPresetIndex,
                ImageEffectsWindowMode.Presets, callbacks);
            if (!string.IsNullOrWhiteSpace(importJson)) _singletonWindow.ImportPreset(importJson);
            _singletonWindow.Closed += (_, _) =>
            {
                selectedPresetChanged?.Invoke(_singletonWindow.ViewModel.SelectedPresetIndex);
                _singletonWindow = null;
            };
            _singletonWindow.Show();
        });
    }
}
