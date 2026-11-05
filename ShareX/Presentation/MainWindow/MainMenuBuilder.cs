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
            new(Strings.MainMenuBuilder_Capture, MainWindowIcons.Camera, BuildCaptureMenu),
            new(Strings.MainMenuBuilder_Upload, MainWindowIcons.Upload, BuildUploadMenu, uploadsEnabled),
            new(Strings.MainMenuBuilder_Workflows, MainWindowIcons.Workflows, BuildWorkflowsMenu),
            new(Strings.MainMenuBuilder_Tools, MainWindowIcons.Wrench, BuildToolsMenu,
                createCategories: BuildToolCategories),
            new(Strings.MainMenuBuilder_AfterCaptureTasks, MainWindowIcons.ImageUpload, BuildAfterCaptureMenu),
            new(Strings.MainMenuBuilder_AfterUploadTasks, MainWindowIcons.CloudUpload, BuildAfterUploadMenu, uploadsEnabled),
            new(Strings.MainMenuBuilder_Destinations, MainWindowIcons.Server, BuildDestinationsMenu, uploadsEnabled),
            new(Strings.MainMenuBuilder_ApplicationSettings, MainWindowIcons.Settings, () => Run(MainFormCommand.ApplicationSettings)),
            new(Strings.MainMenuBuilder_TaskSettings, MainWindowIcons.Sliders, () => Run(MainFormCommand.TaskSettings)),
            new(Strings.MainMenuBuilder_HotkeySettings, MainWindowIcons.Keyboard, () => Run(MainFormCommand.HotkeySettings)),
            new(Strings.MainMenuBuilder_DestinationSettings, MainWindowIcons.CloudSettings, () => Run(MainFormCommand.DestinationSettings), uploadsEnabled),
            new(Strings.MainMenuBuilder_CustomUploaderSettings, MainWindowIcons.Cloud, () => Run(MainFormCommand.CustomUploaderSettings), uploadsEnabled),
            new(Strings.MainMenuBuilder_ScreenshotsFolder, MainWindowIcons.FolderOpen, () => Run(MainFormCommand.ScreenshotsFolder)),
            new(Strings.MainMenuBuilder_History, MainWindowIcons.History, () => Run(MainFormCommand.History)),
            new(Strings.MainMenuBuilder_ImageHistory, MainWindowIcons.Images, () => Run(MainFormCommand.ImageHistory)),
            new(Strings.MainMenuBuilder_Stats, MainWindowIcons.BarChart, () => Run(MainFormCommand.Stats)),
            new(Strings.MainMenuBuilder_Debug, MainWindowIcons.Bug, BuildDebugMenu),
            new(Strings.MainMenuBuilder_Donate, MainWindowIcons.Heart, () => Run(MainFormCommand.Donate)),
            new(Strings.MainMenuBuilder_FollowShareX, MainWindowIcons.ExternalLink, () => Run(MainFormCommand.X)),
            new(Strings.MainMenuBuilder_Discord, MainWindowIcons.ChatBubble, () => Run(MainFormCommand.Discord)),
            new(Strings.MainMenuBuilder_About, MainWindowIcons.Info, () => Run(MainFormCommand.About))
        };
    }

    public IReadOnlyList<MainMenuEntry> BuildTrayMenu()
    {
        bool uploadsEnabled = !SystemOptions.DisableUpload;
        List<MainMenuEntry> items = new()
        {
            Parent(Strings.MainMenuBuilder_Capture, MainWindowIcons.Camera, BuildCaptureMenu),
            Parent(Strings.MainMenuBuilder_Upload, MainWindowIcons.Upload, BuildUploadMenu, uploadsEnabled),
            Parent(Strings.MainMenuBuilder_Workflows, MainWindowIcons.Workflows, BuildWorkflowsMenu),
            CategoryParent(Strings.MainMenuBuilder_Tools, MainWindowIcons.Wrench, BuildToolCategories),
            MainMenuEntry.Separator(),
            Parent(Strings.MainMenuBuilder_AfterCaptureTasks, MainWindowIcons.ImageUpload, BuildAfterCaptureMenu),
            Parent(Strings.MainMenuBuilder_AfterUploadTasks, MainWindowIcons.CloudUpload, BuildAfterUploadMenu, uploadsEnabled),
            Parent(Strings.MainMenuBuilder_Destinations, MainWindowIcons.Server, BuildDestinationsMenu, uploadsEnabled),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_ApplicationSettings, MainWindowIcons.Settings, () => Run(MainFormCommand.ApplicationSettings)),
            Item(Strings.MainMenuBuilder_TaskSettings, MainWindowIcons.Sliders, () => Run(MainFormCommand.TaskSettings)),
            Item(Strings.MainMenuBuilder_HotkeySettings, MainWindowIcons.Keyboard, () => Run(MainFormCommand.HotkeySettings)),
            Item(ApplicationState.Settings.DisableHotkeys ? Strings.MainMenuBuilder_EnableHotkeys : Strings.MainMenuBuilder_DisableHotkeys,
                ApplicationState.Settings.DisableHotkeys ? MainWindowIcons.Keyboard : MainWindowIcons.KeyboardOff,
                () => TaskHelpers.ToggleHotkeys()),
            Item(Strings.MainMenuBuilder_DestinationSettings, MainWindowIcons.CloudSettings, () => Run(MainFormCommand.DestinationSettings), uploadsEnabled),
            Item(Strings.MainMenuBuilder_CustomUploaderSettings, MainWindowIcons.Cloud, () => Run(MainFormCommand.CustomUploaderSettings), uploadsEnabled),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_ScreenshotsFolder, MainWindowIcons.FolderOpen, () => Run(MainFormCommand.ScreenshotsFolder)),
            Item(Strings.MainMenuBuilder_History, MainWindowIcons.History, () => Run(MainFormCommand.History)),
            Item(Strings.MainMenuBuilder_ImageHistory, MainWindowIcons.Images, () => Run(MainFormCommand.ImageHistory)),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_RestartAsAdministrator, MainWindowIcons.Shield, () => ApplicationLifecycle.Restart(true)),
            Parent(Strings.MainMenuBuilder_RecentItems, MainWindowIcons.ClipboardList, BuildRecentItemsMenu,
                ApplicationState.Settings.RecentTasksSave && ApplicationState.Settings.RecentTasksShowInTrayMenu && TaskManager.RecentManager.Tasks.Count > 0),
            Item(Strings.MainMenuBuilder_ActionsToolbar, MainWindowIcons.Toolbar, () => TaskHelpers.ToggleActionsToolbar()),
            Item(Strings.MainMenuBuilder_ShowShareX, MainWindowIcons.Window, MainWindowIntegration.Activate),
            Item(Strings.MainMenuBuilder_Exit, MainWindowIcons.Exit, ApplicationLifecycle.ForceClose)
        };

        return items;
    }

    private IReadOnlyList<MainMenuEntry> BuildCaptureMenu()
    {
        bool autoHide = !_trayMenu;
        return new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_Fullscreen, MainWindowIcons.Fullscreen, () => new CaptureFullscreen().Capture(autoHide)),
            Parent(Strings.MainMenuBuilder_Window, MainWindowIcons.Window, BuildWindowMenu),
            Parent(Strings.MainMenuBuilder_Monitor, MainWindowIcons.Monitor, BuildMonitorMenu),
            Item(Strings.MainMenuBuilder_Region, MainWindowIcons.Region, () => new CaptureRegion().Capture(autoHide)),
            Item(Strings.MainMenuBuilder_LastRegion, MainWindowIcons.LastRegion, () => new CaptureLastRegion().Capture(autoHide)),
            Item(Strings.MainMenuBuilder_ScreenRecording, MainWindowIcons.Video,
                () => TaskHelpers.StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.Region)),
            Item(Strings.MainMenuBuilder_ScreenRecordingGif, MainWindowIcons.Film,
                () => TaskHelpers.StartScreenRecording(ScreenRecordOutput.GIF, ScreenRecordStartMethod.Region)),
            Item(Strings.MainMenuBuilder_ScrollingCapture, MainWindowIcons.ScrollingCapture, async () => await TaskHelpers.OpenScrollingCapture()),
            Item(Strings.MainMenuBuilder_AutoCapture, MainWindowIcons.Clock, () => TaskHelpers.OpenAutoCapture()),
            MainMenuEntry.Separator(),
            new MainMenuEntry(Strings.MainMenuBuilder_ShowCursor, MainWindowIcons.Pointer,
                () => ApplicationState.DefaultTaskSettings.CaptureSettings.ShowCursor = !ApplicationState.DefaultTaskSettings.CaptureSettings.ShowCursor,
                isChecked: ApplicationState.DefaultTaskSettings.CaptureSettings.ShowCursor,
                toggleType: MainMenuToggleType.CheckBox),
            Parent(string.Format(Strings.ScreenshotDelay0S, ApplicationState.DefaultTaskSettings.CaptureSettings.ScreenshotDelay.ToString("0.#")),
                MainWindowIcons.Timer, BuildScreenshotDelayMenu)
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
            items.Add(new MainMenuEntry(Strings.MainMenuBuilder_NoWindowsFound, MainWindowIcons.Window, isEnabled: false));
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
            items.Add(Item(label, i == 0 ? MainWindowIcons.Monitor : MainWindowIcons.Monitor,
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
                delay == 0 ? MainWindowIcons.TimerOff : MainWindowIcons.Timer,
                () => MainWindowIntegration.SetScreenshotDelay(delay),
                isChecked: Math.Abs(current - delay) < 0.01m,
                toggleType: MainMenuToggleType.Radio))
            .ToArray();
    }

    private static IReadOnlyList<MainMenuEntry> BuildUploadMenu()
    {
        return new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_UploadFile, MainWindowIcons.FileUpload, () => UploadManager.UploadFile()),
            Item(Strings.MainMenuBuilder_UploadFolder, MainWindowIcons.FolderUpload, () => UploadManager.UploadFolder()),
            Item(Strings.MainMenuBuilder_UploadClipboard, MainWindowIcons.Clipboard, () => UploadManager.ClipboardUploadMainWindow()),
            Item(Strings.MainMenuBuilder_UploadText, MainWindowIcons.FileText, async () => await UploadManager.ShowTextUploadDialog()),
            Item(Strings.MainMenuBuilder_UploadUrl, MainWindowIcons.Link, async () => await UploadManager.UploadURL()),
            Item(Strings.MainMenuBuilder_DragAndDropUpload, MainWindowIcons.Pointer, () => TaskHelpers.OpenDropWindow()),
            Item(Strings.MainMenuBuilder_ShortenUrl, MainWindowIcons.ShortLink, async () => await UploadManager.ShowShortenURLDialog())
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
            Tool(nameof(Strings.MainMenuBuilder_ColorPicker), Strings.MainMenuBuilder_ColorPicker, MainWindowIcons.Palette, () => TaskHelpers.ShowColorPickerDialog()),
            Tool(nameof(Strings.MainMenuBuilder_MouseHighlighter), Strings.MainMenuBuilder_MouseHighlighter, MainWindowIcons.PointerClick, () => TaskHelpers.OpenMouseHighlighter()),
            Tool(nameof(Strings.MainMenuBuilder_PinToScreenDialog), Strings.MainMenuBuilder_PinToScreenDialog, MainWindowIcons.Pin, () => TaskHelpers.PinToScreen()),
            Tool(nameof(Strings.MainMenuBuilder_Ruler), Strings.MainMenuBuilder_Ruler, MainWindowIcons.Ruler, () => TaskHelpers.OpenRuler()),
            Tool(nameof(Strings.MainMenuBuilder_ScreenColorPicker), Strings.MainMenuBuilder_ScreenColorPicker, MainWindowIcons.Eyedropper, () => TaskHelpers.OpenScreenColorPicker())
        ]),
        new(Strings.MainMenuBuilder_ToolCategoryImages,
        [
            Tool(nameof(Strings.MainMenuBuilder_BackgroundRemover), Strings.MainMenuBuilder_BackgroundRemover, MainWindowIcons.Eraser, () => TaskHelpers.OpenBackgroundRemover()),
            Tool(nameof(Strings.MainMenuBuilder_IconConverter), Strings.MainMenuBuilder_IconConverter, MainWindowIcons.FileImage, () => TaskHelpers.OpenIconConverter()),
            Tool(nameof(Strings.MainMenuBuilder_ImageBeautifier), Strings.MainMenuBuilder_ImageBeautifier, MainWindowIcons.Sparkles, () => TaskHelpers.OpenImageBeautifier()),
            Tool(nameof(Strings.MainMenuBuilder_ImageCombiner), Strings.MainMenuBuilder_ImageCombiner, MainWindowIcons.Combine, () => TaskHelpers.OpenImageCombiner()),
            Tool(nameof(Strings.MainMenuBuilder_ImageComparer), Strings.MainMenuBuilder_ImageComparer, MainWindowIcons.Images, () => TaskHelpers.OpenImageComparer()),
            Tool(nameof(Strings.MainMenuBuilder_ImageConverter), Strings.MainMenuBuilder_ImageConverter, MainWindowIcons.Convert, () => TaskHelpers.OpenImageConverter()),
            Tool(nameof(Strings.MainMenuBuilder_ImageEditor), Strings.MainMenuBuilder_ImageEditor, MainWindowIcons.Image, () => TaskHelpers.OpenImageEditor()),
            Tool(nameof(Strings.MainMenuBuilder_ImageEffects), Strings.MainMenuBuilder_ImageEffects, MainWindowIcons.MagicWand, () => TaskHelpers.OpenImageEffects()),
            Tool(nameof(Strings.MainMenuBuilder_ImageResizer), Strings.MainMenuBuilder_ImageResizer, MainWindowIcons.Resize, () => TaskHelpers.OpenImageResizer()),
            Tool(nameof(Strings.MainMenuBuilder_ImageSplitter), Strings.MainMenuBuilder_ImageSplitter, MainWindowIcons.Split, () => TaskHelpers.OpenImageSplitter()),
            Tool(nameof(Strings.MainMenuBuilder_ImageThumbnailer), Strings.MainMenuBuilder_ImageThumbnailer, MainWindowIcons.Shrink, () => TaskHelpers.OpenImageThumbnailer()),
            Tool(nameof(Strings.MainMenuBuilder_PhotoEditor), Strings.MainMenuBuilder_PhotoEditor, MainWindowIcons.Layers, () => TaskHelpers.OpenPhotoEditor(),
                badgeText: Strings.MainMenuBuilder_NewBadge),
            Tool(nameof(Strings.MainMenuBuilder_ImageViewer), Strings.MainMenuBuilder_ImageViewer, MainWindowIcons.Eye, () => TaskHelpers.OpenImageViewer()),
            Tool(nameof(Strings.MainMenuBuilder_ImageWatermark), Strings.MainMenuBuilder_ImageWatermark, MainWindowIcons.Stamp, () => TaskHelpers.OpenImageWatermark())
        ]),
        new(Strings.MainMenuBuilder_ToolCategoryVideo,
        [
            Tool(nameof(Strings.MainMenuBuilder_AnimatedGifMaker), Strings.MainMenuBuilder_AnimatedGifMaker, MainWindowIcons.Film, () => TaskHelpers.OpenAnimatedGifMaker()),
            Tool(nameof(Strings.MainMenuBuilder_AnimatedGifTrimmer), Strings.MainMenuBuilder_AnimatedGifTrimmer, MainWindowIcons.ScissorsFrame, () => TaskHelpers.OpenAnimatedGifTrimmer()),
            Tool(nameof(Strings.MainMenuBuilder_VideoConverter), Strings.MainMenuBuilder_VideoConverter, MainWindowIcons.FileVideo, () => TaskHelpers.OpenVideoConverter()),
            Tool(nameof(Strings.MainMenuBuilder_VideoPlayer), Strings.MainMenuBuilder_VideoPlayer, MainWindowIcons.CirclePlay, () => TaskHelpers.OpenVideoPlayer()),
            Tool(nameof(Strings.MainMenuBuilder_VideoThumbnailer), Strings.MainMenuBuilder_VideoThumbnailer, MainWindowIcons.Clapperboard, () => TaskHelpers.OpenVideoThumbnailer()),
            Tool(nameof(Strings.MainMenuBuilder_VideoTrimmer), Strings.MainMenuBuilder_VideoTrimmer, MainWindowIcons.Scissors, () => TaskHelpers.OpenVideoTrimmer())
        ]),
        new(Strings.MainMenuBuilder_ToolCategoryData,
        [
            Tool(nameof(Strings.MainMenuBuilder_AnalyzeImage), Strings.MainMenuBuilder_AnalyzeImage, MainWindowIcons.Robot, () => TaskHelpers.AnalyzeImage()),
            Tool(nameof(Strings.MainMenuBuilder_HashChecker), Strings.MainMenuBuilder_HashChecker, MainWindowIcons.Hash, () => TaskHelpers.OpenHashCheck()),
            Tool(nameof(Strings.MainMenuBuilder_IndexFolder), Strings.MainMenuBuilder_IndexFolder, MainWindowIcons.FolderTree, () => TaskHelpers.OpenDirectoryIndexer()),
            Tool(nameof(Strings.MainMenuBuilder_Metadata), Strings.MainMenuBuilder_Metadata, MainWindowIcons.Tag, () => TaskHelpers.OpenMetadataWindow()),
            Tool(nameof(Strings.MainMenuBuilder_OCR), Strings.MainMenuBuilder_OCR, MainWindowIcons.TextRecognition, async () => await TaskHelpers.OCRImage()),
            Tool(nameof(Strings.MainMenuBuilder_QRCode), Strings.MainMenuBuilder_QRCode, MainWindowIcons.QrCode, () => TaskHelpers.OpenQRCode()),
            Tool(nameof(Strings.MainMenuBuilder_RemoteStorageBrowser), Strings.MainMenuBuilder_RemoteStorageBrowser, MainWindowIcons.Cloud, () => TaskHelpers.OpenRemoteStorageBrowser())
        ]),
        new(Strings.MainMenuBuilder_ToolCategorySystem,
        [
            Tool(nameof(Strings.MainMenuBuilder_BorderlessWindow), Strings.MainMenuBuilder_BorderlessWindow, MainWindowIcons.BorderlessWindow, () => TaskHelpers.OpenBorderlessWindow()),
            Tool(nameof(Strings.MainMenuBuilder_ClipboardViewer), Strings.MainMenuBuilder_ClipboardViewer, MainWindowIcons.ClipboardList, () => TaskHelpers.OpenClipboardViewer()),
            Tool(nameof(Strings.MainMenuBuilder_InspectWindow), Strings.MainMenuBuilder_InspectWindow, MainWindowIcons.InspectWindow, () => TaskHelpers.OpenInspectWindow()),
            Tool(nameof(Strings.MainMenuBuilder_MonitorTest), Strings.MainMenuBuilder_MonitorTest, MainWindowIcons.Monitor, () => TaskHelpers.OpenMonitorTest()),
            Tool(nameof(Strings.MainMenuBuilder_NetworkMonitor), Strings.MainMenuBuilder_NetworkMonitor, MainWindowIcons.Activity, () => TaskHelpers.OpenNetworkMonitor())
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

                items.Add(Item(title, TaskHelpers.FindMenuLucideIcon(workflow.TaskSettings.Job),
                    async () => await TaskHelpers.ExecuteJob(workflow.TaskSettings)));
            }
        }

        if (!_trayMenu)
        {
            if (items.Count > 0)
            {
                items.Add(MainMenuEntry.Separator());
            }

            items.Add(Item(Strings.MainMenuBuilder_AddWorkflowsFromHotkeySettings, MainWindowIcons.Keyboard,
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
        AfterCaptureTasks.ShowQuickTaskMenu => MainWindowIcons.Menu,
        AfterCaptureTasks.ShowAfterCaptureWindow => MainWindowIcons.Window,
        AfterCaptureTasks.BeautifyImage => MainWindowIcons.Sparkles,
        AfterCaptureTasks.AddImageEffects => MainWindowIcons.MagicWand,
        AfterCaptureTasks.AnnotateImage => MainWindowIcons.Annotate,
        AfterCaptureTasks.OpenInPhotoEditor => MainWindowIcons.Layers,
        AfterCaptureTasks.CopyImageToClipboard => MainWindowIcons.ClipboardCopy,
        AfterCaptureTasks.PinToScreen => MainWindowIcons.Pin,
        AfterCaptureTasks.SendImageToPrinter => MainWindowIcons.Printer,
        AfterCaptureTasks.SaveImageToFile => MainWindowIcons.Save,
        AfterCaptureTasks.SaveImageToFileWithDialog => MainWindowIcons.SaveAs,
        AfterCaptureTasks.SaveThumbnailImageToFile => MainWindowIcons.ImageDownload,
        AfterCaptureTasks.PerformActions => MainWindowIcons.Terminal,
        AfterCaptureTasks.CopyFileToClipboard => MainWindowIcons.ClipboardCopy,
        AfterCaptureTasks.CopyFilePathToClipboard => MainWindowIcons.ClipboardList,
        AfterCaptureTasks.CopyFolderPathToClipboard => MainWindowIcons.FolderBookmark,
        AfterCaptureTasks.ShowInExplorer => MainWindowIcons.FolderOpen,
        AfterCaptureTasks.AnalyzeImage => MainWindowIcons.Robot,
        AfterCaptureTasks.ScanQRCode => MainWindowIcons.QrCode,
        AfterCaptureTasks.DoOCR => MainWindowIcons.TextRecognition,
        AfterCaptureTasks.ShowBeforeUploadWindow => MainWindowIcons.Window,
        AfterCaptureTasks.UploadImageToHost => MainWindowIcons.CloudUpload,
        AfterCaptureTasks.DeleteFile => MainWindowIcons.Trash,
        _ => MainWindowIcons.Circle
    };

    private static string GetAfterUploadTaskIcon(AfterUploadTasks task) => task switch
    {
        AfterUploadTasks.ShowAfterUploadWindow => MainWindowIcons.Window,
        AfterUploadTasks.UseURLShortener => MainWindowIcons.ShortLink,
        AfterUploadTasks.ShareURL => MainWindowIcons.Share,
        AfterUploadTasks.CopyURLToClipboard => MainWindowIcons.ClipboardCopy,
        AfterUploadTasks.OpenURL => MainWindowIcons.ExternalLink,
        AfterUploadTasks.ShowQRCode => MainWindowIcons.QrCode,
        _ => MainWindowIcons.Circle
    };

    private static IReadOnlyList<MainMenuEntry> BuildDestinationsMenu() =>
        BuildDestinationsMenu(ApplicationState.DefaultTaskSettings);

    internal static IReadOnlyList<MainMenuEntry> BuildDestinationsMenu(TaskSettings settings)
    {
        return new List<MainMenuEntry>
        {
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_Image_uploader___0_,
                () => GetImageUploaderName(settings), MainWindowIcons.Image, () => BuildImageDestinations(settings)),
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_Text_uploader___0_,
                () => GetTextUploaderName(settings), MainWindowIcons.FileText, () => BuildTextDestinations(settings)),
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_File_uploader___0_,
                () => settings.FileDestination.GetLocalizedDescription(), MainWindowIcons.FileUpload, () => BuildEnumDestinations(
                settings.FileDestination,
                value => settings.FileDestination = value)),
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_URL_shortener___0_,
                () => settings.URLShortenerDestination.GetLocalizedDescription(), MainWindowIcons.ShortLink, () => BuildEnumDestinations(
                settings.URLShortenerDestination,
                value => settings.URLShortenerDestination = value)),
            Parent(Strings.TaskSettingsForm_UpdateUploaderMenuNames_URL_sharing_service___0_,
                () => settings.URLSharingServiceDestination.GetLocalizedDescription(), MainWindowIcons.Share, () => BuildEnumDestinations(
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
            Item(Strings.MainMenuBuilder_ShowDebugLog, MainWindowIcons.FileText, () => Run(MainFormCommand.DebugLog)),
            Item(Strings.MainMenuBuilder_TestImageUpload, MainWindowIcons.ImageUpload, () => Run(MainFormCommand.TestImageUpload), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestTextUpload, MainWindowIcons.FileUpload, () => Run(MainFormCommand.TestTextUpload), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestFileUpload, MainWindowIcons.Upload, () => Run(MainFormCommand.TestFileUpload), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestUrlShortener, MainWindowIcons.ShortLink, () => Run(MainFormCommand.TestUrlShortener), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestUrlSharing, MainWindowIcons.Share, () => Run(MainFormCommand.TestUrlSharing), uploadsEnabled)
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
            Item(Strings.MainMenuBuilder_Copy, MainWindowIcons.Copy, task.Copy),
            Item(Strings.MainMenuBuilder_Open, MainWindowIcons.ExternalLink, task.Open)
        })).ToArray();
    }

    private static string GetRecentTaskIcon(RecentTask task)
    {
        if (!string.IsNullOrEmpty(task.ShortenedURL) || !string.IsNullOrEmpty(task.URL)) return MainWindowIcons.Link;
        if (FileHelpers.IsVideoFile(task.FilePath)) return MainWindowIcons.FileVideo;
        if (FileHelpers.IsTextFile(task.FilePath)) return MainWindowIcons.FileText;
        if (FileHelpers.IsImageFile(task.FilePath)) return MainWindowIcons.FileImage;
        return MainWindowIcons.File;
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
