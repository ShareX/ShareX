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
                items.Add(Item(title, IconForName(title),
                    () => new CaptureWindow(selectedWindow.Handle).Capture(!_trayMenu)));
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
        IEnumerable<AfterCaptureTasks> values = Helpers.GetEnums<AfterCaptureTasks>().Skip(1);

        if (SystemOptions.DisableUpload)
        {
            values = values.Except(new[] { AfterCaptureTasks.ShowBeforeUploadWindow, AfterCaptureTasks.UploadImageToHost });
        }

        return values.Select(task => new MainMenuEntry(
            task.GetLocalizedDescription(),
            IconForName(task.ToString()),
            () => Program.DefaultTaskSettings.AfterCaptureJob = Program.DefaultTaskSettings.AfterCaptureJob.Swap(task),
            createChildren: task == AfterCaptureTasks.AddImageEffects ? BuildImageEffectPresetMenu : null,
            isChecked: value.HasFlag(task),
            toggleType: MainMenuToggleType.CheckBox)).ToArray();
    }

    private IReadOnlyList<MainMenuEntry> BuildImageEffectPresetMenu()
    {
        List<MainMenuEntry> items = new()
        {
            new MainMenuEntry(Strings.MainMenuBuilder_EnableAddImageEffects, LucideIcons.wand_sparkles,
                () => Program.DefaultTaskSettings.AfterCaptureJob =
                    Program.DefaultTaskSettings.AfterCaptureJob.Swap(AfterCaptureTasks.AddImageEffects),
                isChecked: Program.DefaultTaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AddImageEffects),
                toggleType: MainMenuToggleType.CheckBox),
            MainMenuEntry.Separator()
        };
        List<ImageEffectsLib.ImageEffectPreset>? presets = Program.DefaultTaskSettings.ImageSettings.ImageEffectPresets;

        if (presets != null)
        {
            for (int i = 0; i < presets.Count; i++)
            {
                int index = i;
                ImageEffectsLib.ImageEffectPreset? preset = presets[i];
                if (preset != null)
                {
                    items.Add(new MainMenuEntry(preset.ToString(), LucideIcons.wand_sparkles,
                        () => Program.DefaultTaskSettings.ImageSettings.SelectedImageEffectPreset = index,
                        isChecked: index == Program.DefaultTaskSettings.ImageSettings.SelectedImageEffectPreset,
                        toggleType: MainMenuToggleType.Radio));
                }
            }
        }

        if (items.Count == 2)
        {
            items.Add(new MainMenuEntry(Strings.MainMenuBuilder_NoImageEffectPresets, LucideIcons.wand_sparkles, isEnabled: false));
        }

        return items;
    }

    private static IReadOnlyList<MainMenuEntry> BuildAfterUploadMenu()
    {
        AfterUploadTasks value = Program.DefaultTaskSettings.AfterUploadJob;
        return Helpers.GetEnums<AfterUploadTasks>().Skip(1).Select(task => new MainMenuEntry(
            task.GetLocalizedDescription(),
            IconForName(task.ToString()),
            () => Program.DefaultTaskSettings.AfterUploadJob = Program.DefaultTaskSettings.AfterUploadJob.Swap(task),
            isChecked: value.HasFlag(task),
            toggleType: MainMenuToggleType.CheckBox)).ToArray();
    }

    private static IReadOnlyList<MainMenuEntry> BuildDestinationsMenu()
    {
        return new List<MainMenuEntry>
        {
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_Image_uploader___0_,
                GetImageUploaderName()), LucideIcons.image, () => BuildImageDestinations()),
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_Text_uploader___0_,
                GetTextUploaderName()), LucideIcons.file_text, () => BuildTextDestinations()),
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_File_uploader___0_,
                Program.DefaultTaskSettings.FileDestination.GetLocalizedDescription()), LucideIcons.file_up, () => BuildEnumDestinations(
                Program.DefaultTaskSettings.FileDestination,
                value => Program.DefaultTaskSettings.FileDestination = value)),
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_URL_shortener___0_,
                Program.DefaultTaskSettings.URLShortenerDestination.GetLocalizedDescription()), LucideIcons.link_2, () => BuildEnumDestinations(
                Program.DefaultTaskSettings.URLShortenerDestination,
                value => Program.DefaultTaskSettings.URLShortenerDestination = value)),
            Parent(string.Format(Strings.TaskSettingsForm_UpdateUploaderMenuNames_URL_sharing_service___0_,
                Program.DefaultTaskSettings.URLSharingServiceDestination.GetLocalizedDescription()), LucideIcons.globe_2, () => BuildEnumDestinations(
                Program.DefaultTaskSettings.URLSharingServiceDestination,
                value => Program.DefaultTaskSettings.URLSharingServiceDestination = value))
        };
    }

    private static string GetImageUploaderName()
    {
        return Program.DefaultTaskSettings.ImageDestination == ImageDestination.FileUploader
            ? Program.DefaultTaskSettings.ImageFileDestination.GetLocalizedDescription()
            : Program.DefaultTaskSettings.ImageDestination.GetLocalizedDescription();
    }

    private static string GetTextUploaderName()
    {
        return Program.DefaultTaskSettings.TextDestination == TextDestination.FileUploader
            ? Program.DefaultTaskSettings.TextFileDestination.GetLocalizedDescription()
            : Program.DefaultTaskSettings.TextDestination.GetLocalizedDescription();
    }

    private static IReadOnlyList<MainMenuEntry> BuildImageDestinations()
    {
        return Helpers.GetEnums<ImageDestination>().Select(value => new MainMenuEntry(
            value.GetLocalizedDescription(),
            string.Empty,
            () => Program.DefaultTaskSettings.ImageDestination = value,
            createChildren: value == ImageDestination.FileUploader
                ? () => BuildEnumDestinations(Program.DefaultTaskSettings.ImageFileDestination,
                    selected =>
                    {
                        Program.DefaultTaskSettings.ImageDestination = ImageDestination.FileUploader;
                        Program.DefaultTaskSettings.ImageFileDestination = selected;
                    })
                : null,
            isChecked: Program.DefaultTaskSettings.ImageDestination == value,
            toggleType: MainMenuToggleType.Radio)).ToArray();
    }

    private static IReadOnlyList<MainMenuEntry> BuildTextDestinations()
    {
        return Helpers.GetEnums<TextDestination>().Select(value => new MainMenuEntry(
            value.GetLocalizedDescription(),
            string.Empty,
            () => Program.DefaultTaskSettings.TextDestination = value,
            createChildren: value == TextDestination.FileUploader
                ? () => BuildEnumDestinations(Program.DefaultTaskSettings.TextFileDestination,
                    selected =>
                    {
                        Program.DefaultTaskSettings.TextDestination = TextDestination.FileUploader;
                        Program.DefaultTaskSettings.TextFileDestination = selected;
                    })
                : null,
            isChecked: Program.DefaultTaskSettings.TextDestination == value,
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
            Item(Strings.MainMenuBuilder_TestUrlSharing, LucideIcons.globe_2, () => Run(MainFormCommand.TestUrlSharing), uploadsEnabled)
        };
    }

    private static IReadOnlyList<MainMenuEntry> BuildRecentItemsMenu()
    {
        IEnumerable<RecentTask> tasks = TaskManager.RecentManager.Tasks;
        if (Program.Settings.RecentTasksTrayMenuMostRecentFirst)
        {
            tasks = tasks.Reverse();
        }

        return tasks.Select(task => Parent(task.TrayMenuText, IconForName(task.TrayMenuText), () => new List<MainMenuEntry>
        {
            Item(Strings.MainMenuBuilder_Copy, LucideIcons.copy, task.Copy),
            Item(Strings.MainMenuBuilder_Open, LucideIcons.external_link, task.Open)
        })).ToArray();
    }

    private void Run(MainFormCommand command) => _host.ExecuteAvaloniaMainFormCommand(command);

    private static MainMenuEntry Item(string header, string icon, Action execute, bool isVisible = true) =>
        new(header, icon, execute, isVisible: isVisible);

    private static MainMenuEntry Item(string header, string icon, Func<Task> execute, bool isVisible = true) =>
        new(header, icon, execute, isVisible: isVisible);

    private static MainMenuEntry Parent(string header, string icon, Func<IReadOnlyList<MainMenuEntry>> children, bool isVisible = true) =>
        new(header, icon, createChildren: children, isVisible: isVisible);

    private static string IconForName(string name)
    {
        int hash = StringComparer.Ordinal.GetHashCode(name) & int.MaxValue;
        string[] icons =
        {
            LucideIcons.circle_check, LucideIcons.copy, LucideIcons.file, LucideIcons.folder,
            LucideIcons.image, LucideIcons.link, LucideIcons.cloud, LucideIcons.sparkles,
            LucideIcons.clipboard, LucideIcons.database, LucideIcons.external_link, LucideIcons.settings_2
        };
        return icons[hash % icons.Length];
    }
}
