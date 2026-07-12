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
using ShareX.Tools;

namespace ShareX.Tools.Integration;

public static class ToolsIntegration
{
    public static void ShowAnalyzeImageWindow(
        string? imagePath,
        AnalyzeImageOptions options,
        AnalyzeImageHandler analyze,
        AnalyzeImageRegionCaptureHandler captureRegion,
        AnalyzeImageTestConnectionHandler testConnection,
        AnalyzeImageLoadModelsHandler loadModels,
        Action<AnalyzeImageOptions>? optionsChanged = null,
        Action? playNotificationSound = null)
    {
        Show(() => new AnalyzeImageWindow(imagePath, options, analyze, captureRegion, testConnection, loadModels,
            optionsChanged, playNotificationSound));
    }

    public static void ShowBackgroundRemoverWindow(string? modelsFolder, BackgroundRemoverOptions options)
    {
        Show(() => new BackgroundRemoverWindow(modelsFolder, options));
    }

    public static void ShowBorderlessWindow(
        BorderlessWindowOptions options,
        Func<string, bool, bool> toggleWindow,
        Action<BorderlessWindowOptions>? settingsChanged = null,
        Action? playNotificationSound = null)
    {
        Show(() => new BorderlessWindowWindow(options, toggleWindow, settingsChanged, playNotificationSound));
    }

    public static void ShowClipboardViewerWindow()
    {
        Show(() => new ClipboardViewerWindow());
    }

    public static void ShowHashCheckerWindow(HashCalculationHandler handler, Action? playSound = null, string? filePath = null)
    {
        Show(() => new HashCheckerWindow(handler, playSound, filePath));
    }

    public static void ShowImageCombinerWindow(ImageCombinerSettings settings, ImageCombinerServices services,
        Action<ImageCombinerSettings>? settingsChanged = null, IEnumerable<string>? imageFiles = null)
    {
        Show(() => new ImageCombinerWindow(settings, services, settingsChanged, imageFiles));
    }

    public static void ShowImageComparerWindow()
    {
        Show(() => new ImageComparerWindow());
    }

    public static void ShowImageViewerWindow(string? filePath = null)
    {
        Show(() => string.IsNullOrWhiteSpace(filePath)
            ? new ImageViewerWindow()
            : new ImageViewerWindow(filePath));
    }

    public static void ShowInspectWindowWindow()
    {
        Show(() => new InspectWindowWindow());
    }

    public static void ShowMonitorTestWindow()
    {
        Show(() => new MonitorTestWindow());
    }

    public static Task<string?> ShowOCRWindowAsync(
        byte[] imageData,
        IReadOnlyList<OCRLanguageOption> languages,
        OCRWindowOptions options,
        OCRRecognitionHandler recognize,
        OCRRegionCaptureHandler selectRegion,
        Action<OCRWindowOptions>? optionsChanged = null,
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

    public static void ShowVideoConverterWindow(VideoConverterSettings settings, VideoConversionHandler handler,
        Action<VideoConverterSettings>? settingsChanged = null, string? inputFilePath = null)
    {
        Show(() => new VideoConverterWindow(settings, handler, settingsChanged, inputFilePath));
    }

    private static void Show(Func<Avalonia.Controls.Window> windowFactory)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() => windowFactory().Show());
    }
}
