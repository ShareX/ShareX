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

using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareX.Localization;

namespace ShareX;

internal sealed class MainMenuBuilder
{
    private readonly MainForm _host;
    private readonly bool _trayMenu;

    public MainMenuBuilder(MainForm host, bool trayMenu = false)
    {
        _host = host;
        _trayMenu = trayMenu;
    }

    public IReadOnlyList<MainNavigationSection> BuildNavigation()
    {
        bool uploadsEnabled = !SystemOptions.DisableUpload;

        return new List<MainNavigationSection>
        {
            new(Strings.MainMenuBuilder_Capture, LucideIcons.camera, BuildCaptureMenu),
            new(Strings.MainMenuBuilder_Upload, LucideIcons.upload, BuildUploadMenu, uploadsEnabled),
            new(Strings.MainMenuBuilder_Workflows, LucideIcons.list_checks, BuildWorkflowsMenu),
            new(Strings.MainMenuBuilder_Tools, LucideIcons.wrench, BuildToolsMenu),
            new(Strings.MainMenuBuilder_AfterCaptureTasks, LucideIcons.image_up, BuildAfterCaptureMenu),
            new(Strings.MainMenuBuilder_AfterUploadTasks, LucideIcons.cloud_upload, BuildAfterUploadMenu, uploadsEnabled),
            new(Strings.MainMenuBuilder_Destinations, LucideIcons.server, BuildDestinationsMenu, uploadsEnabled),
            new(Strings.MainMenuBuilder_ApplicationSettings, LucideIcons.settings, () => Run(MainFormCommand.ApplicationSettings)),
            new(Strings.MainMenuBuilder_TaskSettings, LucideIcons.sliders_horizontal, () => Run(MainFormCommand.TaskSettings)),
            new(Strings.MainMenuBuilder_HotkeySettings, LucideIcons.keyboard, () => Run(MainFormCommand.HotkeySettings)),
            new(Strings.MainMenuBuilder_DestinationSettings, LucideIcons.cloud_cog, () => Run(MainFormCommand.DestinationSettings), uploadsEnabled),
            new(Strings.MainMenuBuilder_CustomUploaderSettings, LucideIcons.cloud, () => Run(MainFormCommand.CustomUploaderSettings), uploadsEnabled),
            new(Strings.MainMenuBuilder_ScreenshotsFolder, LucideIcons.folder_open, () => Run(MainFormCommand.ScreenshotsFolder)),
            new(Strings.MainMenuBuilder_History, LucideIcons.history, () => Run(MainFormCommand.History)),
            new(Strings.MainMenuBuilder_ImageHistory, LucideIcons.images, () => Run(MainFormCommand.ImageHistory)),
            new(Strings.MainMenuBuilder_Debug, LucideIcons.bug, BuildDebugMenu),
            new(Strings.MainMenuBuilder_Donate, LucideIcons.heart, () => Run(MainFormCommand.Donate)),
            new(Strings.MainMenuBuilder_FollowShareX, LucideIcons.external_link, () => Run(MainFormCommand.X)),
            new(Strings.MainMenuBuilder_Discord, LucideIcons.message_circle, () => Run(MainFormCommand.Discord)),
            new(Strings.MainMenuBuilder_About, LucideIcons.info, () => Run(MainFormCommand.About))
        };
    }

    public IReadOnlyList<MainMenuEntry> BuildTrayMenu()
    {
        bool uploadsEnabled = !SystemOptions.DisableUpload;
        List<MainMenuEntry> items = new()
        {
            Parent(Strings.MainMenuBuilder_Capture, LucideIcons.camera, BuildCaptureMenu),
            Parent(Strings.MainMenuBuilder_Upload, LucideIcons.upload, BuildUploadMenu, uploadsEnabled),
            Parent(Strings.MainMenuBuilder_Workflows, LucideIcons.list_checks, BuildWorkflowsMenu),
            Parent(Strings.MainMenuBuilder_Tools, LucideIcons.wrench, BuildToolsMenu),
            MainMenuEntry.Separator(),
            Parent(Strings.MainMenuBuilder_AfterCaptureTasks, LucideIcons.image_up, BuildAfterCaptureMenu),
            Parent(Strings.MainMenuBuilder_AfterUploadTasks, LucideIcons.cloud_upload, BuildAfterUploadMenu, uploadsEnabled),
            Parent(Strings.MainMenuBuilder_Destinations, LucideIcons.server, BuildDestinationsMenu, uploadsEnabled),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_ApplicationSettings, LucideIcons.settings, () => Run(MainFormCommand.ApplicationSettings)),
            Item(Strings.MainMenuBuilder_TaskSettings, LucideIcons.sliders_horizontal, () => Run(MainFormCommand.TaskSettings)),
            Item(Strings.MainMenuBuilder_HotkeySettings, LucideIcons.keyboard, () => Run(MainFormCommand.HotkeySettings)),
            Item(Program.Settings.DisableHotkeys ? Strings.MainMenuBuilder_EnableHotkeys : Strings.MainMenuBuilder_DisableHotkeys,
                Program.Settings.DisableHotkeys ? LucideIcons.keyboard : LucideIcons.keyboard_off,
                () => TaskHelpers.ToggleHotkeys()),
            Item(Strings.MainMenuBuilder_DestinationSettings, LucideIcons.cloud_cog, () => Run(MainFormCommand.DestinationSettings), uploadsEnabled),
            Item(Strings.MainMenuBuilder_CustomUploaderSettings, LucideIcons.cloud, () => Run(MainFormCommand.CustomUploaderSettings), uploadsEnabled),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_ScreenshotsFolder, LucideIcons.folder_open, () => Run(MainFormCommand.ScreenshotsFolder)),
            Item(Strings.MainMenuBuilder_History, LucideIcons.history, () => Run(MainFormCommand.History)),
            Item(Strings.MainMenuBuilder_ImageHistory, LucideIcons.images, () => Run(MainFormCommand.ImageHistory)),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_RestartAsAdministrator, LucideIcons.shield, () => Program.Restart(true)),
            Parent(Strings.MainMenuBuilder_RecentItems, LucideIcons.clipboard_list, BuildRecentItemsMenu,
                Program.Settings.RecentTasksSave && Program.Settings.RecentTasksShowInTrayMenu && TaskManager.RecentManager.Tasks.Count > 0),
            Item(Strings.MainMenuBuilder_ActionsToolbar, LucideIcons.panel_top, () => TaskHelpers.ToggleActionsToolbar()),
            Item(Strings.MainMenuBuilder_ShowShareX, LucideIcons.maximize, MainWindowIntegration.Activate),
            Item(Strings.MainMenuBuilder_Exit, LucideIcons.log_out, _host.ForceClose)
        };

        return items;
    }

    private IReadOnlyList<MainMenuEntry> BuildCaptureMenu()
    {
        bool autoHide = !_trayMenu;
        return new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_Fullscreen, LucideIcons.maximize, () => new CaptureFullscreen().Capture(autoHide)),
            Parent(Strings.MainMenuBuilder_Window, LucideIcons.app_window, BuildWindowMenu),
            Parent(Strings.MainMenuBuilder_Monitor, LucideIcons.monitor, BuildMonitorMenu),
            Item(Strings.MainMenuBuilder_Region, LucideIcons.scan, () => new CaptureRegion().Capture(autoHide)),
            Item(Strings.MainMenuBuilder_RegionLight, LucideIcons.square, () => new CaptureRegion(RegionCaptureType.Light).Capture(autoHide)),
            Item(Strings.MainMenuBuilder_RegionTransparent, LucideIcons.square_dashed, () => new CaptureRegion(RegionCaptureType.Transparent).Capture(autoHide)),
            Item(Strings.MainMenuBuilder_LastRegion, LucideIcons.layers, () => new CaptureLastRegion().Capture(autoHide)),
            Item(Strings.MainMenuBuilder_ScreenRecording, LucideIcons.video,
                () => TaskHelpers.StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.Region)),
            Item(Strings.MainMenuBuilder_ScreenRecordingGif, LucideIcons.film,
                () => TaskHelpers.StartScreenRecording(ScreenRecordOutput.GIF, ScreenRecordStartMethod.Region)),
            Item(Strings.MainMenuBuilder_ScrollingCapture, LucideIcons.scroll_text, async () => await TaskHelpers.OpenScrollingCapture()),
            Item(Strings.MainMenuBuilder_AutoCapture, LucideIcons.clock, () => TaskHelpers.OpenAutoCapture()),
            MainMenuEntry.Separator(),
            new MainMenuEntry(Strings.MainMenuBuilder_ShowCursor, LucideIcons.mouse_pointer_2,
                () => Program.DefaultTaskSettings.CaptureSettings.ShowCursor = !Program.DefaultTaskSettings.CaptureSettings.ShowCursor,
                isChecked: Program.DefaultTaskSettings.CaptureSettings.ShowCursor,
                toggleType: MainMenuToggleType.CheckBox),
            Parent(string.Format(Strings.ScreenshotDelay0S, Program.DefaultTaskSettings.CaptureSettings.ScreenshotDelay.ToString("0.#")),
                LucideIcons.timer, BuildScreenshotDelayMenu)
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
            items.Add(new MainMenuEntry(Strings.MainMenuBuilder_NoWindowsFound, LucideIcons.app_window, isEnabled: false));
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

        using Bitmap bitmap = icon.ToBitmap();
        using MemoryStream stream = new();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    private IReadOnlyList<MainMenuEntry> BuildMonitorMenu()
    {
        List<MainMenuEntry> items = new();
        Screen[] screens = Screen.AllScreens;

        for (int i = 0; i < screens.Length; i++)
        {
            Rectangle bounds = screens[i].Bounds;
            string label = $"{i + 1}. {bounds.Width}x{bounds.Height}";
            items.Add(Item(label, i == 0 ? LucideIcons.monitor : LucideIcons.monitor_up,
                () => new CaptureMonitor(bounds).Capture(!_trayMenu)));
        }

        return items;
    }

    private IReadOnlyList<MainMenuEntry> BuildScreenshotDelayMenu()
    {
        decimal current = Program.DefaultTaskSettings.CaptureSettings.ScreenshotDelay;
        return Enumerable.Range(0, 6)
            .Select(delay => new MainMenuEntry(
                string.Format(Strings.ScreenshotDelay0S, delay),
                delay == 0 ? LucideIcons.timer_off : LucideIcons.timer,
                () => _host.SetAvaloniaScreenshotDelay(delay),
                isChecked: Math.Abs(current - delay) < 0.01m,
                toggleType: MainMenuToggleType.Radio))
            .ToArray();
    }

    private static IReadOnlyList<MainMenuEntry> BuildUploadMenu()
    {
        return new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_UploadFile, LucideIcons.file_up, () => UploadManager.UploadFile()),
            Item(Strings.MainMenuBuilder_UploadFolder, LucideIcons.folder_up, () => UploadManager.UploadFolder()),
            Item(Strings.MainMenuBuilder_UploadClipboard, LucideIcons.clipboard, () => UploadManager.ClipboardUploadMainWindow()),
            Item(Strings.MainMenuBuilder_UploadText, LucideIcons.file_text, async () => await UploadManager.ShowTextUploadDialog()),
            Item(Strings.MainMenuBuilder_UploadUrl, LucideIcons.link, async () => await UploadManager.UploadURL()),
            Item(Strings.MainMenuBuilder_DragAndDropUpload, LucideIcons.mouse_pointer_2, () => TaskHelpers.OpenDropWindow()),
            Item(Strings.MainMenuBuilder_ShortenUrl, LucideIcons.link_2, async () => await UploadManager.ShowShortenURLDialog())
        };
    }

    private static IReadOnlyList<MainMenuEntry> BuildToolsMenu()
    {
        return new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_ColorPicker, LucideIcons.palette, () => TaskHelpers.ShowScreenColorPickerDialog()),
            Item(Strings.MainMenuBuilder_ScreenColorPicker, LucideIcons.pipette, () => TaskHelpers.OpenScreenColorPicker()),
            Item(Strings.MainMenuBuilder_Ruler, LucideIcons.ruler, () => TaskHelpers.OpenRuler()),
            Item(Strings.MainMenuBuilder_PinToScreenDialog, LucideIcons.pin, () => TaskHelpers.PinToScreen()),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_ImageEditor, LucideIcons.image, () => TaskHelpers.OpenImageEditor()),
            Item(Strings.MainMenuBuilder_ImageBeautifier, LucideIcons.sparkles, () => TaskHelpers.OpenImageBeautifier()),
            Item(Strings.MainMenuBuilder_ImageEffects, LucideIcons.wand_sparkles, () => TaskHelpers.OpenImageEffects()),
            Item(Strings.MainMenuBuilder_ImageViewer, LucideIcons.eye, () => TaskHelpers.OpenImageViewer()),
            Item(Strings.MainMenuBuilder_BackgroundRemover, LucideIcons.eraser, () => TaskHelpers.OpenBackgroundRemover()),
            Item(Strings.MainMenuBuilder_ImageComparer, LucideIcons.images, () => TaskHelpers.OpenImageComparer()),
            Item(Strings.MainMenuBuilder_IconConverter, LucideIcons.file_image, () => TaskHelpers.OpenIconConverter()),
            Item(Strings.MainMenuBuilder_ImageCombiner, LucideIcons.combine, () => TaskHelpers.OpenImageCombiner()),
            Item(Strings.MainMenuBuilder_ImageSplitter, LucideIcons.split, () => TaskHelpers.OpenImageSplitter()),
            Item(Strings.MainMenuBuilder_ImageThumbnailer, LucideIcons.shrink, () => TaskHelpers.OpenImageThumbnailer()),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_VideoConverter, LucideIcons.file_video, () => TaskHelpers.OpenVideoConverter()),
            Item(Strings.MainMenuBuilder_VideoThumbnailer, LucideIcons.clapperboard, () => TaskHelpers.OpenVideoThumbnailer()),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_AnalyzeImage, LucideIcons.bot, () => TaskHelpers.AnalyzeImage()),
            Item(Strings.MainMenuBuilder_OCR, LucideIcons.scan_text, async () => await TaskHelpers.OCRImage()),
            Item(Strings.MainMenuBuilder_QRCode, LucideIcons.qr_code, () => TaskHelpers.OpenQRCode()),
            Item(Strings.MainMenuBuilder_HashChecker, LucideIcons.hash, () => TaskHelpers.OpenHashCheck()),
            Item(Strings.MainMenuBuilder_Metadata, LucideIcons.tags, () => TaskHelpers.OpenMetadataWindow()),
            Item(Strings.MainMenuBuilder_IndexFolder, LucideIcons.folder_tree, () => TaskHelpers.OpenDirectoryIndexer()),
            MainMenuEntry.Separator(),
            Item(Strings.MainMenuBuilder_ClipboardViewer, LucideIcons.clipboard_list, () => TaskHelpers.OpenClipboardViewer()),
            Item(Strings.MainMenuBuilder_BorderlessWindow, LucideIcons.frame, () => TaskHelpers.OpenBorderlessWindow()),
            Item(Strings.MainMenuBuilder_InspectWindow, LucideIcons.scan_search, () => TaskHelpers.OpenInspectWindow()),
            Item(Strings.MainMenuBuilder_MonitorTest, LucideIcons.monitor, () => TaskHelpers.OpenMonitorTest())
        };
    }

    private IReadOnlyList<MainMenuEntry> BuildWorkflowsMenu()
    {
        List<MainMenuEntry> items = new();

        if (Program.HotkeysConfig?.Hotkeys != null)
        {
            foreach (HotkeySettings hotkey in Program.HotkeysConfig.Hotkeys)
            {
                if (hotkey.TaskSettings.Job == HotkeyType.None ||
                    (Program.Settings.WorkflowsOnlyShowEdited && hotkey.TaskSettings.IsUsingDefaultSettings))
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

            items.Add(Item(Strings.MainMenuBuilder_AddWorkflowsFromHotkeySettings, LucideIcons.keyboard,
                () => Run(MainFormCommand.HotkeySettings)));
        }

        return items;
    }

    private IReadOnlyList<MainMenuEntry> BuildAfterCaptureMenu()
    {
        AfterCaptureTasks value = Program.DefaultTaskSettings.AfterCaptureJob;
        return GetAfterCaptureTaskMenuOptions(!SystemOptions.DisableUpload).Select(option => new MainMenuEntry(
            option.Header,
            option.Icon,
            () => Program.DefaultTaskSettings.AfterCaptureJob = Program.DefaultTaskSettings.AfterCaptureJob.Swap(option.Task),
            createChildren: option.Task == AfterCaptureTasks.AddImageEffects ? BuildImageEffectPresetMenu : null,
            isChecked: value.HasFlag(option.Task),
            toggleType: MainMenuToggleType.CheckBox,
            staysOpenOnClick: true)).ToArray();
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
        List<ImageEffectsLib.ImageEffectPreset>? presets = Program.DefaultTaskSettings.ImageSettings.ImageEffectPresets;

        if (presets != null)
        {
            for (int i = 0; i < presets.Count; i++)
            {
                int index = i;
                ImageEffectsLib.ImageEffectPreset? preset = presets[i];
                if (preset != null)
                {
                    items.Add(new MainMenuEntry(preset.ToString(), string.Empty,
                        () => Program.DefaultTaskSettings.ImageSettings.SelectedImageEffectPreset = index,
                        isChecked: index == Program.DefaultTaskSettings.ImageSettings.SelectedImageEffectPreset,
                        toggleType: MainMenuToggleType.Radio));
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
        AfterUploadTasks value = Program.DefaultTaskSettings.AfterUploadJob;
        return GetAfterUploadTaskMenuOptions().Select(option => new MainMenuEntry(
            option.Header,
            option.Icon,
            () => Program.DefaultTaskSettings.AfterUploadJob = Program.DefaultTaskSettings.AfterUploadJob.Swap(option.Task),
            isChecked: value.HasFlag(option.Task),
            toggleType: MainMenuToggleType.CheckBox,
            staysOpenOnClick: true)).ToArray();
    }

    internal static IReadOnlyList<(AfterUploadTasks Task, string Header, string Icon)> GetAfterUploadTaskMenuOptions() =>
        Helpers.GetEnums<AfterUploadTasks>().Skip(1)
            .Select(task => (task, task.GetLocalizedDescription(), GetAfterUploadTaskIcon(task)))
            .ToArray();

    private static string GetAfterCaptureTaskIcon(AfterCaptureTasks task) => task switch
    {
        AfterCaptureTasks.ShowQuickTaskMenu => LucideIcons.menu,
        AfterCaptureTasks.ShowAfterCaptureWindow => LucideIcons.app_window,
        AfterCaptureTasks.BeautifyImage => LucideIcons.sparkles,
        AfterCaptureTasks.AddImageEffects => LucideIcons.wand_sparkles,
        AfterCaptureTasks.AnnotateImage => LucideIcons.pen_line,
        AfterCaptureTasks.CopyImageToClipboard => LucideIcons.clipboard_copy,
        AfterCaptureTasks.PinToScreen => LucideIcons.pin,
        AfterCaptureTasks.SendImageToPrinter => LucideIcons.printer,
        AfterCaptureTasks.SaveImageToFile => LucideIcons.save,
        AfterCaptureTasks.SaveImageToFileWithDialog => LucideIcons.save_pen,
        AfterCaptureTasks.SaveThumbnailImageToFile => LucideIcons.image_down,
        AfterCaptureTasks.PerformActions => LucideIcons.terminal,
        AfterCaptureTasks.CopyFileToClipboard => LucideIcons.clipboard_copy,
        AfterCaptureTasks.CopyFilePathToClipboard => LucideIcons.clipboard_list,
        AfterCaptureTasks.CopyFolderPathToClipboard => LucideIcons.folder_bookmark,
        AfterCaptureTasks.ShowInExplorer => LucideIcons.folder_open,
        AfterCaptureTasks.AnalyzeImage => LucideIcons.bot,
        AfterCaptureTasks.ScanQRCode => LucideIcons.qr_code,
        AfterCaptureTasks.DoOCR => LucideIcons.scan_text,
        AfterCaptureTasks.ShowBeforeUploadWindow => LucideIcons.app_window,
        AfterCaptureTasks.UploadImageToHost => LucideIcons.upload_cloud,
        AfterCaptureTasks.DeleteFile => LucideIcons.trash_2,
        _ => LucideIcons.circle
    };

    private static string GetAfterUploadTaskIcon(AfterUploadTasks task) => task switch
    {
        AfterUploadTasks.ShowAfterUploadWindow => LucideIcons.app_window,
        AfterUploadTasks.UseURLShortener => LucideIcons.link_2,
        AfterUploadTasks.ShareURL => LucideIcons.share_2,
        AfterUploadTasks.CopyURLToClipboard => LucideIcons.clipboard_copy,
        AfterUploadTasks.OpenURL => LucideIcons.external_link,
        AfterUploadTasks.ShowQRCode => LucideIcons.qr_code,
        _ => LucideIcons.circle
    };

    private static IReadOnlyList<MainMenuEntry> BuildDestinationsMenu() =>
        BuildDestinationsMenu(Program.DefaultTaskSettings);

    internal static IReadOnlyList<MainMenuEntry> BuildDestinationsMenu(TaskSettings settings)
    {
        return new List<MainMenuEntry>
        {
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_Image_uploader___0_,
                GetImageUploaderName(settings)), LucideIcons.image, () => BuildImageDestinations(settings)),
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_Text_uploader___0_,
                GetTextUploaderName(settings)), LucideIcons.file_text, () => BuildTextDestinations(settings)),
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_File_uploader___0_,
                settings.FileDestination.GetLocalizedDescription()), LucideIcons.file_up, () => BuildEnumDestinations(
                settings.FileDestination,
                value => settings.FileDestination = value)),
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_URL_shortener___0_,
                settings.URLShortenerDestination.GetLocalizedDescription()), LucideIcons.link_2, () => BuildEnumDestinations(
                settings.URLShortenerDestination,
                value => settings.URLShortenerDestination = value)),
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_URL_sharing_service___0_,
                settings.URLSharingServiceDestination.GetLocalizedDescription()), LucideIcons.share_2, () => BuildEnumDestinations(
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
            toggleType: MainMenuToggleType.Radio)).ToArray();
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
            toggleType: MainMenuToggleType.Radio)).ToArray();
    }

    private static IReadOnlyList<MainMenuEntry> BuildEnumDestinations<T>(T selected, Action<T> setValue) where T : struct, Enum
    {
        return Helpers.GetEnums<T>().Select(value => new MainMenuEntry(
            value.GetLocalizedDescription(),
            string.Empty,
            () => setValue(value),
            isChecked: EqualityComparer<T>.Default.Equals(selected, value),
            toggleType: MainMenuToggleType.Radio)).ToArray();
    }

    private IReadOnlyList<MainMenuEntry> BuildDebugMenu()
    {
        bool uploadsEnabled = !SystemOptions.DisableUpload;
        return new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_ShowDebugLog, LucideIcons.file_text, () => Run(MainFormCommand.DebugLog)),
            Item(Strings.MainMenuBuilder_TestImageUpload, LucideIcons.image_up, () => Run(MainFormCommand.TestImageUpload), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestTextUpload, LucideIcons.file_up, () => Run(MainFormCommand.TestTextUpload), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestFileUpload, LucideIcons.upload, () => Run(MainFormCommand.TestFileUpload), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestUrlShortener, LucideIcons.link_2, () => Run(MainFormCommand.TestUrlShortener), uploadsEnabled),
            Item(Strings.MainMenuBuilder_TestUrlSharing, LucideIcons.share_2, () => Run(MainFormCommand.TestUrlSharing), uploadsEnabled)
        };
    }

    private static IReadOnlyList<MainMenuEntry> BuildRecentItemsMenu()
    {
        IEnumerable<RecentTask> tasks = TaskManager.RecentManager.Tasks;
        if (Program.Settings.RecentTasksTrayMenuMostRecentFirst)
        {
            tasks = tasks.Reverse();
        }

        return tasks.Select(task => Parent(task.TrayMenuText, GetRecentTaskIcon(task), () => new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_Copy, LucideIcons.copy, task.Copy),
            Item(Strings.MainMenuBuilder_Open, LucideIcons.external_link, task.Open)
        })).ToArray();
    }

    private static string GetRecentTaskIcon(RecentTask task)
    {
        if (!string.IsNullOrEmpty(task.ShortenedURL) || !string.IsNullOrEmpty(task.URL)) return LucideIcons.link;
        if (FileHelpers.IsVideoFile(task.FilePath)) return LucideIcons.file_video;
        if (FileHelpers.IsTextFile(task.FilePath)) return LucideIcons.file_text;
        if (FileHelpers.IsImageFile(task.FilePath)) return LucideIcons.file_image;
        return LucideIcons.file;
    }

    private void Run(MainFormCommand command) => _host.ExecuteAvaloniaMainFormCommand(command);

    private static MainMenuEntry Item(string header, string icon, Action execute, bool isVisible = true, byte[]? bitmapIcon = null) =>
        new(header, icon, execute, isVisible: isVisible, bitmapIcon: bitmapIcon);

    private static MainMenuEntry Item(string header, string icon, Func<Task> execute, bool isVisible = true) =>
        new(header, icon, execute, isVisible: isVisible);

    private static MainMenuEntry Parent(string header, string icon, Func<IReadOnlyList<MainMenuEntry>> children, bool isVisible = true) =>
        new(header, icon, createChildren: children, isVisible: isVisible);

}
