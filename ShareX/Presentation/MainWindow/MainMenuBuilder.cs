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

#nullable enable

using ShareX.HelpersLib;
using ShareX.Icons;
using ShareX.Localization;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bitmap = SkiaSharp.SKBitmap;
using ImageFormat = SkiaSharp.SKEncodedImageFormat;

namespace ShareX;

internal sealed class MainMenuBuilder
{
    private readonly bool _trayMenu;

    public MainMenuBuilder(bool trayMenu = false)
    {
        _trayMenu = trayMenu;
    }

    public IReadOnlyList<MainNavigationSection> BuildNavigation()
    {
        bool uploadsEnabled = !SystemOptions.DisableUpload;

        return new List<MainNavigationSection>
        {
            new(Strings.MainMenuBuilder_Capture, AppIcons.Camera, BuildCaptureMenu),
            new(Strings.MainMenuBuilder_Upload, AppIcons.Upload, BuildUploadMenu, uploadsEnabled),
            new(Strings.MainMenuBuilder_Workflows, AppIcons.Workflows, BuildWorkflowsMenu),
            new(Strings.MainMenuBuilder_Tools, AppIcons.Wrench, BuildToolsMenu,
                createCategories: BuildToolCategories),
            new(Strings.MainMenuBuilder_AfterCaptureTasks, AppIcons.AfterCaptureTasks, BuildAfterCaptureMenu),
            new(Strings.MainMenuBuilder_AfterUploadTasks, AppIcons.AfterUploadTasks, BuildAfterUploadMenu, uploadsEnabled),
            new(Strings.MainMenuBuilder_Destinations, AppIcons.Destinations, BuildDestinationsMenu, uploadsEnabled),
            new(Strings.MainMenuBuilder_ApplicationSettings, AppIcons.Settings, () => Run(MainFormCommand.ApplicationSettings)),
            new(Strings.MainMenuBuilder_TaskSettings, AppIcons.Sliders, () => Run(MainFormCommand.TaskSettings)),
            new(Strings.MainMenuBuilder_HotkeySettings, AppIcons.Keyboard, () => Run(MainFormCommand.HotkeySettings)),
            new(Strings.MainMenuBuilder_DestinationSettings, AppIcons.DestinationSettings, () => Run(MainFormCommand.DestinationSettings), uploadsEnabled),
            new(Strings.MainMenuBuilder_CustomUploaderSettings, AppIcons.CustomUploaderSettings, () => Run(MainFormCommand.CustomUploaderSettings), uploadsEnabled),
            new(Strings.MainMenuBuilder_ScreenshotsFolder, AppIcons.ScreenshotsFolder, () => Run(MainFormCommand.ScreenshotsFolder)),
            new(Strings.MainMenuBuilder_History, AppIcons.History, () => Run(MainFormCommand.History)),
            new(Strings.MainMenuBuilder_ImageHistory, AppIcons.ImageHistory, () => Run(MainFormCommand.ImageHistory)),
            new(Strings.MainMenuBuilder_Stats, AppIcons.BarChart, () => Run(MainFormCommand.Stats)),
            new(Strings.MainMenuBuilder_Debug, AppIcons.Bug, BuildDebugMenu),
            new(Strings.MainMenuBuilder_Donate, AppIcons.Heart, () => Run(MainFormCommand.Donate)),
            new(Strings.MainMenuBuilder_FollowShareX, AppIcons.Follow, () => Run(MainFormCommand.X)),
            new(Strings.MainMenuBuilder_Discord, AppIcons.Discord, () => Run(MainFormCommand.Discord)),
            new(Strings.MainMenuBuilder_About, AppIcons.Info, () => Run(MainFormCommand.About))
        };
    }

    public IReadOnlyList<MainMenuEntry> BuildTrayMenu()
    {
        bool uploadsEnabled = !SystemOptions.DisableUpload;
        List<MainMenuEntry> items = new()
        {
            Parent(Strings.MainMenuBuilder_Capture, AppIcons.Camera, BuildCaptureMenu),
            Parent(Strings.MainMenuBuilder_Upload, AppIcons.Upload, BuildUploadMenu, uploadsEnabled),
            Parent(Strings.MainMenuBuilder_Workflows, AppIcons.Workflows, BuildWorkflowsMenu),
            CategoryParent(Strings.MainMenuBuilder_Tools, AppIcons.Wrench, BuildToolCategories),
            MainMenuEntry.Separator(),
            Parent(Strings.MainMenuBuilder_AfterCaptureTasks, AppIcons.AfterCaptureTasks, BuildAfterCaptureMenu),
            Parent(Strings.MainMenuBuilder_AfterUploadTasks, AppIcons.AfterUploadTasks, BuildAfterUploadMenu, uploadsEnabled),
            Parent(Strings.MainMenuBuilder_Destinations, AppIcons.Destinations, BuildDestinationsMenu, uploadsEnabled),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_ApplicationSettings, AppIcons.Settings, () => Run(MainFormCommand.ApplicationSettings)),
            Item(Strings.MainMenuBuilder_TaskSettings, AppIcons.Sliders, () => Run(MainFormCommand.TaskSettings)),
            Item(Strings.MainMenuBuilder_HotkeySettings, AppIcons.Keyboard, () => Run(MainFormCommand.HotkeySettings)),
            Item(ApplicationState.Settings.DisableHotkeys ? Strings.MainMenuBuilder_EnableHotkeys : Strings.MainMenuBuilder_DisableHotkeys,
                ApplicationState.Settings.DisableHotkeys ? AppIcons.Keyboard : AppIcons.KeyboardOff,
                () => TaskHelpers.ToggleHotkeys()),
            Item(Strings.MainMenuBuilder_DestinationSettings, AppIcons.DestinationSettings, () => Run(MainFormCommand.DestinationSettings), uploadsEnabled),
            Item(Strings.MainMenuBuilder_CustomUploaderSettings, AppIcons.CustomUploaderSettings, () => Run(MainFormCommand.CustomUploaderSettings), uploadsEnabled),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_ScreenshotsFolder, AppIcons.ScreenshotsFolder, () => Run(MainFormCommand.ScreenshotsFolder)),
            Item(Strings.MainMenuBuilder_History, AppIcons.History, () => Run(MainFormCommand.History)),
            Item(Strings.MainMenuBuilder_ImageHistory, AppIcons.ImageHistory, () => Run(MainFormCommand.ImageHistory)),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_RestartAsAdministrator, AppIcons.RestartAsAdmin, () => ApplicationLifecycle.Restart(true)),
            Parent(Strings.MainMenuBuilder_RecentItems, AppIcons.RecentItems, BuildRecentItemsMenu,
                ApplicationState.Settings.RecentTasksSave && ApplicationState.Settings.RecentTasksShowInTrayMenu && TaskManager.RecentManager.Tasks.Count > 0),
            Item(Strings.MainMenuBuilder_ActionsToolbar, AppIcons.Toolbar, () => TaskHelpers.ToggleActionsToolbar()),
            Item(Strings.MainMenuBuilder_ShowShareX, AppIcons.Window, MainWindowIntegration.Activate),
            Item(Strings.MainMenuBuilder_Exit, AppIcons.Exit, ApplicationLifecycle.ForceClose)
        };

        return items;
    }

    private IReadOnlyList<MainMenuEntry> BuildCaptureMenu()
    {
        bool autoHide = !_trayMenu;
        return new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_Fullscreen, AppIcons.Fullscreen, () => new CaptureFullscreen().Capture(autoHide)),
            Parent(Strings.MainMenuBuilder_Window, AppIcons.Window, BuildWindowMenu),
            Parent(Strings.MainMenuBuilder_Monitor, AppIcons.Monitor, BuildMonitorMenu),
            Item(Strings.MainMenuBuilder_Region, AppIcons.Region, () => new CaptureRegion().Capture(autoHide)),
            Item(Strings.MainMenuBuilder_LastRegion, AppIcons.LastRegion, () => new CaptureLastRegion().Capture(autoHide)),
            Item(Strings.MainMenuBuilder_ScreenRecording, AppIcons.ScreenRecording,
                () => TaskHelpers.StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.Region)),
            Item(Strings.MainMenuBuilder_ScreenRecordingGif, AppIcons.Film,
                () => TaskHelpers.StartScreenRecording(ScreenRecordOutput.GIF, ScreenRecordStartMethod.Region)),
            Item(Strings.MainMenuBuilder_ScrollingCapture, AppIcons.ScrollingCapture, async () => await TaskHelpers.OpenScrollingCapture()),
            Item(Strings.MainMenuBuilder_AutoCapture, AppIcons.AutoCapture, () => TaskHelpers.OpenAutoCapture()),
            MainMenuEntry.Separator(),
            new MainMenuEntry(Strings.MainMenuBuilder_ShowCursor, AppIcons.Pointer,
                () => ApplicationState.DefaultTaskSettings.CaptureSettings.ShowCursor = !ApplicationState.DefaultTaskSettings.CaptureSettings.ShowCursor,
                isChecked: ApplicationState.DefaultTaskSettings.CaptureSettings.ShowCursor,
                toggleType: MainMenuToggleType.CheckBox),
            Parent(string.Format(Strings.ScreenshotDelay0S, ApplicationState.DefaultTaskSettings.CaptureSettings.ScreenshotDelay.ToString("0.#")),
                AppIcons.Timer, BuildScreenshotDelayMenu)
        };
    }

    private IReadOnlyList<MainMenuEntry> BuildWindowMenu()
    {
        List<MainMenuEntry> items = new();

        try
        {
            foreach (WindowInfo window in new WindowsList().GetVisibleWindowsList())
            {
                WindowInfo selectedWindow = window;
                string title = selectedWindow.Text.Truncate(50, "...");
                items.Add(Item(title, string.Empty,
                    () => new CaptureWindow(selectedWindow.Handle).Capture(!_trayMenu),
                    bitmapIcon: GetWindowIcon(selectedWindow)));
            }
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
        }

        if (items.Count == 0)
        {
            items.Add(new MainMenuEntry(Strings.MainMenuBuilder_NoWindowsFound, AppIcons.Window, isEnabled: false));
        }

        return items;
    }

    private static byte[]? GetWindowIcon(WindowInfo window)
    {
        using Icon? icon = window.Icon;
        if (icon == null)
        {
            return null;
        }

        using Bitmap bitmap = WindowsImageInterop.FromIcon(icon.Handle);
        using MemoryStream stream = new();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    private IReadOnlyList<MainMenuEntry> BuildMonitorMenu()
    {
        List<MainMenuEntry> items = new();
        DesktopScreen[] screens = DesktopScreen.AllScreens;

        for (int i = 0; i < screens.Length; i++)
        {
            Rectangle bounds = screens[i].Bounds;
            string label = $"{i + 1}. {bounds.Width}x{bounds.Height}";
            items.Add(Item(label, i == 0 ? AppIcons.Monitor : AppIcons.Monitor,
                () => new CaptureMonitor(bounds).Capture(!_trayMenu)));
        }

        return items;
    }

    private IReadOnlyList<MainMenuEntry> BuildScreenshotDelayMenu()
    {
        decimal current = ApplicationState.DefaultTaskSettings.CaptureSettings.ScreenshotDelay;
        return Enumerable.Range(0, 6)
            .Select(delay => new MainMenuEntry(
                string.Format(Strings.ScreenshotDelay0S, delay),
                delay == 0 ? AppIcons.TimerOff : AppIcons.Timer,
                () => MainWindowIntegration.SetScreenshotDelay(delay),
                isChecked: Math.Abs(current - delay) < 0.01m,
                toggleType: MainMenuToggleType.Radio))
            .ToArray();
    }

    private static IReadOnlyList<MainMenuEntry> BuildUploadMenu()
    {
        return new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_UploadFile, AppIcons.FileUpload, () => UploadManager.UploadFile()),
            Item(Strings.MainMenuBuilder_UploadFolder, AppIcons.FolderUpload, () => UploadManager.UploadFolder()),
            Item(Strings.MainMenuBuilder_UploadClipboard, AppIcons.UploadClipboard, () => UploadManager.ClipboardUploadMainWindow()),
            Item(Strings.MainMenuBuilder_UploadText, AppIcons.FileText, async () => await UploadManager.ShowTextUploadDialog()),
            Item(Strings.MainMenuBuilder_UploadUrl, AppIcons.Link, async () => await UploadManager.UploadURL()),
            Item(Strings.MainMenuBuilder_DragAndDropUpload, AppIcons.DragDropUpload, () => TaskHelpers.OpenDropWindow()),
            Item(Strings.MainMenuBuilder_ShortenUrl, AppIcons.ShortLink, async () => await UploadManager.ShowShortenURLDialog())
        };
    }

    private static IReadOnlyList<MainMenuEntry> BuildToolsMenu()
    {
        List<MainMenuEntry> items = new();

        foreach (MainMenuCategory category in BuildToolCategories())
        {
            if (items.Count > 0)
            {
                items.Add(MainMenuEntry.Separator());
            }

            items.AddRange(category.Entries);
        }

        return items;
    }

    private static IReadOnlyList<MainMenuCategory> BuildToolCategories() =>
    [
        new(Strings.MainMenuBuilder_ToolCategoryScreen,
        [
            Tool(nameof(Strings.MainMenuBuilder_ColorPicker), Strings.MainMenuBuilder_ColorPicker, AppIcons.Palette, () => TaskHelpers.ShowColorPickerDialog()),
            Tool(nameof(Strings.MainMenuBuilder_MouseHighlighter), Strings.MainMenuBuilder_MouseHighlighter, AppIcons.MouseHighlighter, () => TaskHelpers.OpenMouseHighlighter()),
            Tool(nameof(Strings.MainMenuBuilder_PinToScreenDialog), Strings.MainMenuBuilder_PinToScreenDialog, AppIcons.Pin, () => TaskHelpers.PinToScreen()),
            Tool(nameof(Strings.MainMenuBuilder_Ruler), Strings.MainMenuBuilder_Ruler, AppIcons.Ruler, () => TaskHelpers.OpenRuler()),
            Tool(nameof(Strings.MainMenuBuilder_ScreenColorPicker), Strings.MainMenuBuilder_ScreenColorPicker, AppIcons.ScreenColorPicker, () => TaskHelpers.OpenScreenColorPicker())
        ]),
        new(Strings.MainMenuBuilder_ToolCategoryImages,
        [
            Tool(nameof(Strings.MainMenuBuilder_BackgroundRemover), Strings.MainMenuBuilder_BackgroundRemover, AppIcons.BackgroundRemover, () => TaskHelpers.OpenBackgroundRemover()),
            Tool(nameof(Strings.MainMenuBuilder_IconConverter), Strings.MainMenuBuilder_IconConverter, AppIcons.IconConverter, () => TaskHelpers.OpenIconConverter()),
            Tool(nameof(Strings.MainMenuBuilder_ImageBeautifier), Strings.MainMenuBuilder_ImageBeautifier, AppIcons.ImageBeautifier, () => TaskHelpers.OpenImageBeautifier()),
            Tool(nameof(Strings.MainMenuBuilder_ImageCombiner), Strings.MainMenuBuilder_ImageCombiner, AppIcons.Combine, () => TaskHelpers.OpenImageCombiner()),
            Tool(nameof(Strings.MainMenuBuilder_ImageComparer), Strings.MainMenuBuilder_ImageComparer, AppIcons.ImageComparer, () => TaskHelpers.OpenImageComparer()),
            Tool(nameof(Strings.MainMenuBuilder_ImageConverter), Strings.MainMenuBuilder_ImageConverter, AppIcons.ImageConverter, () => TaskHelpers.OpenImageConverter()),
            Tool(nameof(Strings.MainMenuBuilder_ImageEditor), Strings.MainMenuBuilder_ImageEditor, AppIcons.ImageEditor, () => TaskHelpers.OpenImageEditor()),
            Tool(nameof(Strings.MainMenuBuilder_ImageEffects), Strings.MainMenuBuilder_ImageEffects, AppIcons.MagicWand, () => TaskHelpers.OpenImageEffects()),
            Tool(nameof(Strings.MainMenuBuilder_ImageResizer), Strings.MainMenuBuilder_ImageResizer, AppIcons.Resize, () => TaskHelpers.OpenImageResizer()),
            Tool(nameof(Strings.MainMenuBuilder_ImageSplitter), Strings.MainMenuBuilder_ImageSplitter, AppIcons.Split, () => TaskHelpers.OpenImageSplitter()),
            Tool(nameof(Strings.MainMenuBuilder_ImageThumbnailer), Strings.MainMenuBuilder_ImageThumbnailer, AppIcons.ImageThumbnailer, () => TaskHelpers.OpenImageThumbnailer()),
            Tool(nameof(Strings.MainMenuBuilder_PhotoEditor), Strings.MainMenuBuilder_PhotoEditor, AppIcons.Layers, () => TaskHelpers.OpenPhotoEditor(),
                badgeText: Strings.MainMenuBuilder_NewBadge),
            Tool(nameof(Strings.MainMenuBuilder_ImageViewer), Strings.MainMenuBuilder_ImageViewer, AppIcons.Eye, () => TaskHelpers.OpenImageViewer()),
            Tool(nameof(Strings.MainMenuBuilder_ImageWatermark), Strings.MainMenuBuilder_ImageWatermark, AppIcons.Stamp, () => TaskHelpers.OpenImageWatermark())
        ]),
        new(Strings.MainMenuBuilder_ToolCategoryVideo,
        [
            Tool(nameof(Strings.MainMenuBuilder_AnimatedGifMaker), Strings.MainMenuBuilder_AnimatedGifMaker, AppIcons.AnimatedGifMaker, () => TaskHelpers.OpenAnimatedGifMaker()),
            Tool(nameof(Strings.MainMenuBuilder_AnimatedGifTrimmer), Strings.MainMenuBuilder_AnimatedGifTrimmer, AppIcons.ScissorsFrame, () => TaskHelpers.OpenAnimatedGifTrimmer()),
            Tool(nameof(Strings.MainMenuBuilder_VideoConverter), Strings.MainMenuBuilder_VideoConverter, AppIcons.VideoConverter, () => TaskHelpers.OpenVideoConverter()),
            Tool(nameof(Strings.MainMenuBuilder_VideoPlayer), Strings.MainMenuBuilder_VideoPlayer, AppIcons.CirclePlay, () => TaskHelpers.OpenVideoPlayer()),
            Tool(nameof(Strings.MainMenuBuilder_VideoThumbnailer), Strings.MainMenuBuilder_VideoThumbnailer, AppIcons.Clapperboard, () => TaskHelpers.OpenVideoThumbnailer()),
            Tool(nameof(Strings.MainMenuBuilder_VideoTrimmer), Strings.MainMenuBuilder_VideoTrimmer, AppIcons.Scissors, () => TaskHelpers.OpenVideoTrimmer())
        ]),
        new(Strings.MainMenuBuilder_ToolCategoryData,
        [
            Tool(nameof(Strings.MainMenuBuilder_AnalyzeImage), Strings.MainMenuBuilder_AnalyzeImage, AppIcons.Robot, () => TaskHelpers.AnalyzeImage()),
            Tool(nameof(Strings.MainMenuBuilder_HashChecker), Strings.MainMenuBuilder_HashChecker, AppIcons.Hash, () => TaskHelpers.OpenHashCheck()),
            Tool(nameof(Strings.MainMenuBuilder_IndexFolder), Strings.MainMenuBuilder_IndexFolder, AppIcons.FolderTree, () => TaskHelpers.OpenDirectoryIndexer()),
            Tool(nameof(Strings.MainMenuBuilder_Metadata), Strings.MainMenuBuilder_Metadata, AppIcons.Tag, () => TaskHelpers.OpenMetadataWindow()),
            Tool(nameof(Strings.MainMenuBuilder_OCR), Strings.MainMenuBuilder_OCR, AppIcons.TextRecognition, async () => await TaskHelpers.OCRImage()),
            Tool(nameof(Strings.MainMenuBuilder_QRCode), Strings.MainMenuBuilder_QRCode, AppIcons.QrCode, () => TaskHelpers.OpenQRCode()),
            Tool(nameof(Strings.MainMenuBuilder_RemoteStorageBrowser), Strings.MainMenuBuilder_RemoteStorageBrowser, AppIcons.RemoteStorageBrowser, () => TaskHelpers.OpenRemoteStorageBrowser())
        ]),
        new(Strings.MainMenuBuilder_ToolCategorySystem,
        [
            Tool(nameof(Strings.MainMenuBuilder_BorderlessWindow), Strings.MainMenuBuilder_BorderlessWindow, AppIcons.BorderlessWindow, () => TaskHelpers.OpenBorderlessWindow()),
            Tool(nameof(Strings.MainMenuBuilder_ClipboardViewer), Strings.MainMenuBuilder_ClipboardViewer, AppIcons.ClipboardViewer, () => TaskHelpers.OpenClipboardViewer()),
            Tool(nameof(Strings.MainMenuBuilder_InspectWindow), Strings.MainMenuBuilder_InspectWindow, AppIcons.InspectWindow, () => TaskHelpers.OpenInspectWindow()),
            Tool(nameof(Strings.MainMenuBuilder_MonitorTest), Strings.MainMenuBuilder_MonitorTest, AppIcons.MonitorTest, () => TaskHelpers.OpenMonitorTest()),
            Tool(nameof(Strings.MainMenuBuilder_NetworkMonitor), Strings.MainMenuBuilder_NetworkMonitor, AppIcons.Activity, () => TaskHelpers.OpenNetworkMonitor())
        ])
    ];

    private IReadOnlyList<MainMenuEntry> BuildWorkflowsMenu()
    {
        List<MainMenuEntry> items = new();

        if (ApplicationState.HotkeysConfigOrNull?.Hotkeys != null)
        {
            foreach (HotkeySettings hotkey in ApplicationState.HotkeysConfig.Hotkeys)
            {
                if (hotkey.TaskSettings.Job == HotkeyType.None ||
                    (ApplicationState.Settings.WorkflowsOnlyShowEdited && hotkey.TaskSettings.IsUsingDefaultSettings))
                {
                    continue;
                }

                HotkeySettings workflow = hotkey;
                string title = workflow.TaskSettings + (workflow.TaskSettings.IsUsingDefaultSettings ? string.Empty : "*");
                if (workflow.HotkeyInfo.IsValidHotkey)
                {
                    title += $"    {workflow.HotkeyInfo}";
                }

                items.Add(Item(title, TaskHelpers.FindMenuIcon(workflow.TaskSettings.Job),
                    async () => await TaskHelpers.ExecuteJob(workflow.TaskSettings)));
            }
        }

        if (!_trayMenu)
        {
            if (items.Count > 0)
            {
                items.Add(MainMenuEntry.Separator());
            }

            items.Add(Item(Strings.MainMenuBuilder_AddWorkflowsFromHotkeySettings, AppIcons.Keyboard,
                () => Run(MainFormCommand.HotkeySettings)));
        }

        return items;
    }

    private IReadOnlyList<MainMenuEntry> BuildAfterCaptureMenu()
    {
        AfterCaptureTasks value = ApplicationState.DefaultTaskSettings.AfterCaptureJob;
        return GetAfterCaptureTaskMenuOptions(!SystemOptions.DisableUpload).Select(option => new MainMenuEntry(
            option.Header,
            option.Icon,
            () => ApplicationState.DefaultTaskSettings.AfterCaptureJob = ApplicationState.DefaultTaskSettings.AfterCaptureJob.Swap(option.Task),
            createChildren: option.Task == AfterCaptureTasks.AddImageEffects ? BuildImageEffectPresetMenu : null,
            isChecked: value.HasFlag(option.Task),
            toggleType: MainMenuToggleType.CheckBox,
            staysOpenOnClick: true,
            boldWhenChecked: true)).ToArray();
    }

    internal static IReadOnlyList<(AfterCaptureTasks Task, string Header, string Icon)> GetAfterCaptureTaskMenuOptions(bool includeUploadTasks = true)
    {
        IEnumerable<AfterCaptureTasks> tasks = Helpers.GetEnums<AfterCaptureTasks>().Skip(1);
        if (!includeUploadTasks)
        {
            tasks = tasks.Except(new[] { AfterCaptureTasks.ShowBeforeUploadWindow, AfterCaptureTasks.UploadImageToHost });
        }

        return tasks.Select(task => (task, task.GetLocalizedDescription(), GetAfterCaptureTaskIcon(task))).ToArray();
    }

    private IReadOnlyList<MainMenuEntry> BuildImageEffectPresetMenu()
    {
        List<MainMenuEntry> items = new();
        List<ImageEffectsLib.ImageEffectPreset>? presets = ApplicationState.DefaultTaskSettings.ImageSettings.ImageEffectPresets;

        if (presets != null)
        {
            for (int i = 0; i < presets.Count; i++)
            {
                int index = i;
                ImageEffectsLib.ImageEffectPreset? preset = presets[i];
                if (preset != null)
                {
                    items.Add(new MainMenuEntry(preset.ToString(), string.Empty,
                        () => ApplicationState.DefaultTaskSettings.ImageSettings.SelectedImageEffectPreset = index,
                        isChecked: index == ApplicationState.DefaultTaskSettings.ImageSettings.SelectedImageEffectPreset,
                        toggleType: MainMenuToggleType.Radio,
                        staysOpenOnClick: true,
                        boldWhenChecked: true));
                }
            }
        }

        if (items.Count == 0)
        {
            items.Add(new MainMenuEntry(Strings.MainMenuBuilder_NoImageEffectPresets, string.Empty, isEnabled: false));
        }

        return items;
    }

    private IReadOnlyList<MainMenuEntry> BuildAfterUploadMenu()
    {
        AfterUploadTasks value = ApplicationState.DefaultTaskSettings.AfterUploadJob;
        return GetAfterUploadTaskMenuOptions().Select(option => new MainMenuEntry(
            option.Header,
            option.Icon,
            () => ApplicationState.DefaultTaskSettings.AfterUploadJob = ApplicationState.DefaultTaskSettings.AfterUploadJob.Swap(option.Task),
            isChecked: value.HasFlag(option.Task),
            toggleType: MainMenuToggleType.CheckBox,
            staysOpenOnClick: true,
            boldWhenChecked: true)).ToArray();
    }

    internal static IReadOnlyList<(AfterUploadTasks Task, string Header, string Icon)> GetAfterUploadTaskMenuOptions() =>
        Helpers.GetEnums<AfterUploadTasks>().Skip(1)
            .Select(task => (task, task.GetLocalizedDescription(), GetAfterUploadTaskIcon(task)))
            .ToArray();

    internal static string GetAfterCaptureTaskIcon(AfterCaptureTasks task) => task switch
    {
        AfterCaptureTasks.ShowQuickTaskMenu => AppIcons.Menu,
        AfterCaptureTasks.ShowAfterCaptureWindow => AppIcons.Window,
        AfterCaptureTasks.BeautifyImage => AppIcons.ImageBeautifier,
        AfterCaptureTasks.AddImageEffects => AppIcons.MagicWand,
        AfterCaptureTasks.AnnotateImage => AppIcons.ImageEditor,
        AfterCaptureTasks.OpenInPhotoEditor => AppIcons.Layers,
        AfterCaptureTasks.CopyImageToClipboard => AppIcons.ClipboardImage,
        AfterCaptureTasks.PinToScreen => AppIcons.Pin,
        AfterCaptureTasks.SendImageToPrinter => AppIcons.Printer,
        AfterCaptureTasks.SaveImageToFile => AppIcons.Save,
        AfterCaptureTasks.SaveImageToFileWithDialog => AppIcons.SaveAs,
        AfterCaptureTasks.SaveThumbnailImageToFile => AppIcons.ImageDownload,
        AfterCaptureTasks.PerformActions => AppIcons.Terminal,
        AfterCaptureTasks.CopyFileToClipboard => AppIcons.ClipboardCopy,
        AfterCaptureTasks.CopyFilePathToClipboard => AppIcons.ClipboardList,
        AfterCaptureTasks.CopyFolderPathToClipboard => AppIcons.FolderBookmark,
        AfterCaptureTasks.ShowInExplorer => AppIcons.FolderOpen,
        AfterCaptureTasks.AnalyzeImage => AppIcons.Robot,
        AfterCaptureTasks.ScanQRCode => AppIcons.QrCode,
        AfterCaptureTasks.DoOCR => AppIcons.TextRecognition,
        AfterCaptureTasks.ShowBeforeUploadWindow => AppIcons.Window,
        AfterCaptureTasks.UploadImageToHost => AppIcons.CloudUpload,
        AfterCaptureTasks.DeleteFile => AppIcons.Trash,
        _ => AppIcons.Circle
    };

    private static string GetAfterUploadTaskIcon(AfterUploadTasks task) => task switch
    {
        AfterUploadTasks.ShowAfterUploadWindow => AppIcons.Window,
        AfterUploadTasks.UseURLShortener => AppIcons.ShortLink,
        AfterUploadTasks.ShareURL => AppIcons.Share,
        AfterUploadTasks.CopyURLToClipboard => AppIcons.ClipboardLink,
        AfterUploadTasks.OpenURL => AppIcons.ExternalLink,
        AfterUploadTasks.ShowQRCode => AppIcons.QrCode,
        _ => AppIcons.Circle
    };

    private static IReadOnlyList<MainMenuEntry> BuildDestinationsMenu() =>
        BuildDestinationsMenu(ApplicationState.DefaultTaskSettings);

    internal static IReadOnlyList<MainMenuEntry> BuildDestinationsMenu(TaskSettings settings)
    {
        return new List<MainMenuEntry>
        {
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_Image_uploader___0_,
                () => GetImageUploaderName(settings), AppIcons.Image, () => BuildImageDestinations(settings)),
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_Text_uploader___0_,
                () => GetTextUploaderName(settings), AppIcons.FileText, () => BuildTextDestinations(settings)),
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_File_uploader___0_,
                () => settings.FileDestination.GetLocalizedDescription(), AppIcons.FileUpload, () => BuildEnumDestinations(
                settings.FileDestination,
                value => settings.FileDestination = value)),
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_URL_shortener___0_,
                () => settings.URLShortenerDestination.GetLocalizedDescription(), AppIcons.ShortLink, () => BuildEnumDestinations(
                settings.URLShortenerDestination,
                value => settings.URLShortenerDestination = value)),
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_URL_sharing_service___0_,
                () => settings.URLSharingServiceDestination.GetLocalizedDescription(), AppIcons.Share, () => BuildEnumDestinations(
                settings.URLSharingServiceDestination,
                value => settings.URLSharingServiceDestination = value))
        };
    }

    private static string GetImageUploaderName(TaskSettings settings)
    {
        return settings.ImageDestination == ImageDestination.FileUploader
            ? settings.ImageFileDestination.GetLocalizedDescription()
            : settings.ImageDestination.GetLocalizedDescription();
    }

    private static string GetTextUploaderName(TaskSettings settings)
    {
        return settings.TextDestination == TextDestination.FileUploader
            ? settings.TextFileDestination.GetLocalizedDescription()
            : settings.TextDestination.GetLocalizedDescription();
    }

    private static IReadOnlyList<MainMenuEntry> BuildImageDestinations(TaskSettings settings)
    {
        return Helpers.GetEnums<ImageDestination>().Select(value => new MainMenuEntry(
            value.GetLocalizedDescription(),
            string.Empty,
            () => settings.ImageDestination = value,
            createChildren: value == ImageDestination.FileUploader
                ? () => BuildEnumDestinations(settings.ImageFileDestination,
                    selected =>
                    {
                        settings.ImageDestination = ImageDestination.FileUploader;
                        settings.ImageFileDestination = selected;
                    })
                : null,
            isChecked: settings.ImageDestination == value,
            toggleType: MainMenuToggleType.Radio,
            staysOpenOnClick: true,
            boldWhenChecked: true)).ToArray();
    }

    private static IReadOnlyList<MainMenuEntry> BuildTextDestinations(TaskSettings settings)
    {
        return Helpers.GetEnums<TextDestination>().Select(value => new MainMenuEntry(
            value.GetLocalizedDescription(),
            string.Empty,
            () => settings.TextDestination = value,
            createChildren: value == TextDestination.FileUploader
                ? () => BuildEnumDestinations(settings.TextFileDestination,
                    selected =>
                    {
                        settings.TextDestination = TextDestination.FileUploader;
                        settings.TextFileDestination = selected;
                    })
                : null,
            isChecked: settings.TextDestination == value,
            toggleType: MainMenuToggleType.Radio,
            staysOpenOnClick: true,
            boldWhenChecked: true)).ToArray();
    }

    private static IReadOnlyList<MainMenuEntry> BuildEnumDestinations<T>(T selected, Action<T> setValue) where T : struct, Enum
    {
        return Helpers.GetEnums<T>().Select(value => new MainMenuEntry(
            value.GetLocalizedDescription(),
            string.Empty,
            () => setValue(value),
            isChecked: EqualityComparer<T>.Default.Equals(selected, value),
            toggleType: MainMenuToggleType.Radio,
            staysOpenOnClick: true,
            boldWhenChecked: true)).ToArray();
    }

    private IReadOnlyList<MainMenuEntry> BuildDebugMenu()
    {
        bool uploadsEnabled = !SystemOptions.DisableUpload;
        return new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_ShowDebugLog, AppIcons.FileText, () => Run(MainFormCommand.DebugLog)),
            Item(Strings.MainMenuBuilder_TestImageUpload, AppIcons.ImageUpload, () => Run(MainFormCommand.TestImageUpload), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestTextUpload, AppIcons.FileUpload, () => Run(MainFormCommand.TestTextUpload), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestFileUpload, AppIcons.Upload, () => Run(MainFormCommand.TestFileUpload), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestUrlShortener, AppIcons.ShortLink, () => Run(MainFormCommand.TestUrlShortener), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestUrlSharing, AppIcons.Share, () => Run(MainFormCommand.TestUrlSharing), uploadsEnabled)
        };
    }

    private static IReadOnlyList<MainMenuEntry> BuildRecentItemsMenu()
    {
        IEnumerable<RecentTask> tasks = TaskManager.RecentManager.Tasks;
        if (ApplicationState.Settings.RecentTasksTrayMenuMostRecentFirst)
        {
            tasks = tasks.Reverse();
        }

        return tasks.Select(task => Parent(task.TrayMenuText, GetRecentTaskIcon(task), () => new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_Copy, AppIcons.Copy, task.Copy),
            Item(Strings.MainMenuBuilder_Open, AppIcons.ExternalLink, task.Open)
        })).ToArray();
    }

    private static string GetRecentTaskIcon(RecentTask task)
    {
        if (!string.IsNullOrEmpty(task.ShortenedURL) || !string.IsNullOrEmpty(task.URL)) return AppIcons.Link;
        if (FileHelpers.IsVideoFile(task.FilePath)) return AppIcons.FileVideo;
        if (FileHelpers.IsTextFile(task.FilePath)) return AppIcons.FileText;
        if (FileHelpers.IsImageFile(task.FilePath)) return AppIcons.FileImage;
        return AppIcons.File;
    }

    private static void Run(MainFormCommand command) => MainWindowIntegration.ExecuteCommand(command);

    private static MainMenuEntry Item(string header, string icon, Action execute, bool isVisible = true, byte[]? bitmapIcon = null) =>
        new(header, icon, execute, isVisible: isVisible, bitmapIcon: bitmapIcon);

    private static MainMenuEntry Item(string header, string icon, Func<Task> execute, bool isVisible = true) =>
        new(header, icon, execute, isVisible: isVisible);

    private static MainMenuEntry Tool(string id, string header, string icon, Action execute, string? badgeText = null) =>
        new(header, icon, execute, id: id, badgeText: badgeText);

    private static MainMenuEntry Tool(string id, string header, string icon, Func<Task> execute) =>
        new(header, icon, execute, id: id);

    private static MainMenuEntry Parent(string header, string icon, Func<IReadOnlyList<MainMenuEntry>> children, bool isVisible = true) =>
        new(header, icon, createChildren: children, isVisible: isVisible);

    private static MainMenuEntry CategoryParent(string header, string icon,
        Func<IReadOnlyList<MainMenuCategory>> categories, bool isVisible = true) =>
        new(header, icon, createCategories: categories, isVisible: isVisible);

    private static MainMenuEntry Parent(string headerFormat, Func<string> createAccentText, string icon,
        Func<IReadOnlyList<MainMenuEntry>> children, bool isVisible = true)
    {
        string CreateHeader() => string.Format(headerFormat, createAccentText());
        return new MainMenuEntry(CreateHeader(), icon, createChildren: children, isVisible: isVisible,
            createHeader: CreateHeader, createAccentText: createAccentText);
    }

}
