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
using ShareX.AvaloniaUI.Windows;
using ShareX.HelpersLib;
using ShareX.Tools;

namespace ShareX.Tools.Integration;

public static class ToolsIntegration
{
    public static void ShowDirectoryIndexerWindow(IndexerSettings settings,
        Func<string, IndexerOutput, Task>? uploadRequested = null)
    {
        Show(() => new DirectoryIndexerWindow(settings, uploadRequested));
    }

    public static void ShowAnalyzeImageWindow(
        string? imagePath,
        AIOptions options,
        AnalyzeImageRegionCaptureHandler captureRegion,
        Action? playNotificationSound = null)
    {
        Show(() => new AnalyzeImageWindow(imagePath, options, captureRegion, playNotificationSound));
    }

    public static void ShowBackgroundRemoverWindow(string? modelsFolder, BackgroundRemoverOptions options)
    {
        Show(() => new BackgroundRemoverWindow(modelsFolder, options));
    }

    public static void ShowBorderlessWindow(
        BorderlessWindowSettings options,
        Func<string, bool, bool> toggleWindow,
        Action<BorderlessWindowSettings>? settingsChanged = null,
        Action? playNotificationSound = null)
    {
        Show(() => new BorderlessWindowWindow(options, toggleWindow, settingsChanged, playNotificationSound));
    }

    public static void ShowClipboardViewerWindow()
    {
        Show(() => new ClipboardViewerWindow());
    }

    public static Task<ScreenColorPickerResult?> PickScreenColorAsync(ScreenColorPickerOptions options)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        TaskCompletionSource<ScreenColorPickerResult?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                ScreenColorPickerWindow picker = new(options);
                completion.TrySetResult(await picker.PickAsync());
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        });

        return completion.Task;
    }

    public static void ShowHashCheckerWindow(HashCalculationHandler handler, Action? playSound = null, string? filePath = null)
    {
        Show(() => new HashCheckerWindow(handler, playSound, filePath));
    }

    public static void ShowImageCombinerWindow(ImageCombinerOptions options, ImageCombinerServices services,
        IEnumerable<string>? imageFiles = null)
    {
        Show(() => new ImageCombinerWindow(options, services, imageFiles));
    }

    public static void ShowImageComparerWindow()
    {
        Show(() => new ImageComparerWindow());
    }

    public static void ShowInspectWindowWindow()
    {
        Show(() => new InspectWindowWindow());
    }

    public static void ShowMonitorTestWindow()
    {
        Show(() => new MonitorTestWindow());
    }

    public static void ShowPinToScreenWindow(PinToScreenServices services, PinToScreenOptions options)
    {
        Show(() => new PinToScreenStartupWindow(services, options));
    }

    public static void PinToScreen(byte[] imageData, PinToScreenOptions options, Action<byte[]> copyImage,
        System.Drawing.Point? location = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        PinToScreenManager.Pin(imageData, options, copyImage, location);
    }

    public static void CloseAllPinnedImages()
    {
        AvaloniaBootstrapper.EnsureInitialized();
        PinToScreenManager.CloseAll();
    }

    public static Task<string?> ShowOCRWindowAsync(
        byte[] imageData,
        IReadOnlyList<OCRLanguageOption> languages,
        OCROptions options,
        OCRRecognitionHandler recognize,
        OCRRegionCaptureHandler selectRegion,
        Action<OCROptions>? optionsChanged = null,
        Action? openHelp = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        TaskCompletionSource<string?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            OCRWindow window = new(imageData, languages, options, recognize, selectRegion, optionsChanged, openHelp);
            window.Closed += (_, _) => completion.TrySetResult(window.Result);
            window.Show();
        });

        return completion.Task;
    }

    public static void ShowMetadataWindow(string? filePath = null, Action? playNotificationSound = null)
    {
        Show(() => new MetadataWindow(filePath, playNotificationSound));
    }

    public static void ShowImageSplitterWindow()
    {
        Show(() => new ImageSplitterWindow());
    }

    public static void ShowImageThumbnailerWindow()
    {
        Show(() => new ImageThumbnailerWindow());
    }

    public static void ShowIconConverterWindow()
    {
        Show(() => new IconConverterWindow());
    }

    public static void ShowQRCodeWindow(QRCodeServices services, QRCodeWindowOptions options)
    {
        Show(() => new QRCodeWindow(services, options));
    }

    public static void ShowRulerWindow()
    {
        Show(() => new RulerWindow());
    }

    public static void ShowVideoConverterWindow(VideoConverterOptions options, VideoConversionHandler handler,
        string? inputFilePath = null)
    {
        Show(() => new VideoConverterWindow(options, handler, inputFilePath));
    }

    public static void ShowVideoThumbnailerWindow(string ffmpegPath, VideoThumbnailOptions options,
        Action<IReadOnlyList<VideoThumbnailInfo>>? thumbnailsTaken = null)
    {
        Show(() => new VideoThumbnailerWindow(ffmpegPath, options, thumbnailsTaken));
    }

    private static void Show(Func<Avalonia.Controls.Window> windowFactory)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() => windowFactory().Show());
    }
}
