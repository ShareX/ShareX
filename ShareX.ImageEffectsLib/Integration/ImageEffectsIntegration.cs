#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

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
