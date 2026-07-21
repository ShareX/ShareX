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
            new("Capture", LucideIcons.camera, BuildCaptureMenu),
            new("Upload", LucideIcons.upload, BuildUploadMenu, uploadsEnabled),
            new("Workflows", LucideIcons.list_checks, BuildWorkflowsMenu),
            new("Tools", LucideIcons.wrench, BuildToolsMenu),
            new("After capture tasks", LucideIcons.image_up, BuildAfterCaptureMenu),
            new("After upload tasks", LucideIcons.cloud_upload, BuildAfterUploadMenu, uploadsEnabled),
            new("Destinations", LucideIcons.server, BuildDestinationsMenu, uploadsEnabled),
            new("Application settings", LucideIcons.settings, () => Run(MainFormCommand.ApplicationSettings)),
            new("Task settings", LucideIcons.sliders_horizontal, () => Run(MainFormCommand.TaskSettings)),
            new("Hotkey settings", LucideIcons.keyboard, () => Run(MainFormCommand.HotkeySettings)),
            new("Destination settings", LucideIcons.cloud_cog, () => Run(MainFormCommand.DestinationSettings), uploadsEnabled),
            new("Custom uploader settings", LucideIcons.cloud, () => Run(MainFormCommand.CustomUploaderSettings), uploadsEnabled),
            new("Screenshots folder", LucideIcons.folder_open, () => Run(MainFormCommand.ScreenshotsFolder)),
            new("History", LucideIcons.history, () => Run(MainFormCommand.History)),
            new("Image history", LucideIcons.images, () => Run(MainFormCommand.ImageHistory)),
            new("Debug", LucideIcons.bug, BuildDebugMenu),
            new("Donate", LucideIcons.heart, () => Run(MainFormCommand.Donate)),
            new("Follow ShareX", LucideIcons.external_link, () => Run(MainFormCommand.X)),
            new("Discord", LucideIcons.message_circle, () => Run(MainFormCommand.Discord)),
            new("About", LucideIcons.info, () => Run(MainFormCommand.About))
        };
    }

    public IReadOnlyList<MainMenuEntry> BuildTrayMenu()
    {
        bool uploadsEnabled = !SystemOptions.DisableUpload;
        List<MainMenuEntry> items = new()
        {
            Parent("Capture", LucideIcons.camera, BuildCaptureMenu),
            Parent("Upload", LucideIcons.upload, BuildUploadMenu, uploadsEnabled),
            Parent("Workflows", LucideIcons.list_checks, BuildWorkflowsMenu),
            Parent("Tools", LucideIcons.wrench, BuildToolsMenu),
            MainMenuEntry.Separator(),
            Parent("After capture tasks", LucideIcons.image_up, BuildAfterCaptureMenu),
            Parent("After upload tasks", LucideIcons.cloud_upload, BuildAfterUploadMenu, uploadsEnabled),
            Parent("Destinations", LucideIcons.server, BuildDestinationsMenu, uploadsEnabled),
            MainMenuEntry.Separator(),
            Item("Application settings", LucideIcons.settings, () => Run(MainFormCommand.ApplicationSettings)),
            Item("Task settings", LucideIcons.sliders_horizontal, () => Run(MainFormCommand.TaskSettings)),
            Item("Hotkey settings", LucideIcons.keyboard, () => Run(MainFormCommand.HotkeySettings)),
            Item(Program.Settings.DisableHotkeys ? "Enable hotkeys" : "Disable hotkeys",
                Program.Settings.DisableHotkeys ? LucideIcons.keyboard : LucideIcons.keyboard_off,
                () => TaskHelpers.ToggleHotkeys()),
            Item("Destination settings", LucideIcons.cloud_cog, () => Run(MainFormCommand.DestinationSettings), uploadsEnabled),
            Item("Custom uploader settings", LucideIcons.cloud, () => Run(MainFormCommand.CustomUploaderSettings), uploadsEnabled),
            MainMenuEntry.Separator(),
            Item("Screenshots folder", LucideIcons.folder_open, () => Run(MainFormCommand.ScreenshotsFolder)),
            Item("History", LucideIcons.history, () => Run(MainFormCommand.History)),
            Item("Image history", LucideIcons.images, () => Run(MainFormCommand.ImageHistory)),
            MainMenuEntry.Separator(),
            Item("Restart as administrator", LucideIcons.shield, () => Program.Restart(true)),
            Parent("Recent items", LucideIcons.clipboard_list, BuildRecentItemsMenu,
                Program.Settings.RecentTasksSave && Program.Settings.RecentTasksShowInTrayMenu && TaskManager.RecentManager.Tasks.Count > 0),
            Item("Actions toolbar", LucideIcons.panel_top, () => TaskHelpers.ToggleActionsToolbar()),
            Item("Show ShareX", LucideIcons.maximize, MainWindowIntegration.Activate),
            Item("Exit", LucideIcons.log_out, _host.ForceClose)
        };

        return items;
    }

    private IReadOnlyList<MainMenuEntry> BuildCaptureMenu()
    {
        bool autoHide = !_trayMenu;
        return new List<MainMenuEntry>
        {
            Item("Fullscreen", LucideIcons.maximize, () => new CaptureFullscreen().Capture(autoHide)),
            Parent("Window", LucideIcons.app_window, BuildWindowMenu),
            Parent("Monitor", LucideIcons.monitor, BuildMonitorMenu),
            Item("Region", LucideIcons.scan, () => new CaptureRegion().Capture(autoHide)),
            Item("Region (light)", LucideIcons.square, () => new CaptureRegion(RegionCaptureType.Light).Capture(autoHide)),
            Item("Region (transparent)", LucideIcons.square_dashed, () => new CaptureRegion(RegionCaptureType.Transparent).Capture(autoHide)),
            Item("Last region", LucideIcons.layers, () => new CaptureLastRegion().Capture(autoHide)),
            Item("Screen recording", LucideIcons.video,
                () => TaskHelpers.StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.Region)),
            Item("Screen recording (GIF)", LucideIcons.film,
                () => TaskHelpers.StartScreenRecording(ScreenRecordOutput.GIF, ScreenRecordStartMethod.Region)),
            Item("Scrolling capture", LucideIcons.scroll_text, async () => await TaskHelpers.OpenScrollingCapture()),
            Item("Auto capture", LucideIcons.clock, () => TaskHelpers.OpenAutoCapture()),
            MainMenuEntry.Separator(),
            new MainMenuEntry("Show cursor", LucideIcons.mouse_pointer_2,
                () => Program.DefaultTaskSettings.CaptureSettings.ShowCursor = !Program.DefaultTaskSettings.CaptureSettings.ShowCursor,
                isChecked: Program.DefaultTaskSettings.CaptureSettings.ShowCursor,
                toggleType: MainMenuToggleType.CheckBox),
            Parent(string.Format(Resources.ScreenshotDelay0S, Program.DefaultTaskSettings.CaptureSettings.ScreenshotDelay.ToString("0.#")),
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
            items.Add(new MainMenuEntry("No windows found", LucideIcons.app_window, isEnabled: false));
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
                string.Format(Resources.ScreenshotDelay0S, delay),
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
            Item("Upload file", LucideIcons.file_up, () => UploadManager.UploadFile()),
            Item("Upload folder", LucideIcons.folder_up, () => UploadManager.UploadFolder()),
            Item("Upload clipboard", LucideIcons.clipboard, () => UploadManager.ClipboardUploadMainWindow()),
            Item("Upload text", LucideIcons.file_text, async () => await UploadManager.ShowTextUploadDialog()),
            Item("Upload URL", LucideIcons.link, async () => await UploadManager.UploadURL()),
            Item("Drag and drop upload", LucideIcons.mouse_pointer_2, () => TaskHelpers.OpenDropWindow()),
            Item("Shorten URL", LucideIcons.link_2, async () => await UploadManager.ShowShortenURLDialog())
        };
    }

    private static IReadOnlyList<MainMenuEntry> BuildToolsMenu()
    {
        return new List<MainMenuEntry>
        {
            Item("Color picker", LucideIcons.palette, () => TaskHelpers.ShowScreenColorPickerDialog()),
            Item("Screen color picker", LucideIcons.pipette, () => TaskHelpers.OpenScreenColorPicker()),
            Item("Ruler", LucideIcons.ruler, () => TaskHelpers.OpenRuler()),
            Item("Pin to screen", LucideIcons.pin, () => TaskHelpers.PinToScreen()),
            MainMenuEntry.Separator(),
            Item("Image editor", LucideIcons.image, () => TaskHelpers.OpenImageEditor()),
            Item("Image beautifier", LucideIcons.sparkles, () => TaskHelpers.OpenImageBeautifier()),
            Item("Image effects", LucideIcons.wand_sparkles, () => TaskHelpers.OpenImageEffects()),
            Item("Image viewer", LucideIcons.eye, () => TaskHelpers.OpenImageViewer()),
            Item("Background remover", LucideIcons.eraser, () => TaskHelpers.OpenBackgroundRemover()),
            Item("Image comparer", LucideIcons.images, () => TaskHelpers.OpenImageComparer()),
            Item("Icon converter", LucideIcons.file_image, () => TaskHelpers.OpenIconConverter()),
            Item("Image combiner", LucideIcons.combine, () => TaskHelpers.OpenImageCombiner()),
            Item("Image splitter", LucideIcons.split, () => TaskHelpers.OpenImageSplitter()),
            Item("Image thumbnailer", LucideIcons.shrink, () => TaskHelpers.OpenImageThumbnailer()),
            MainMenuEntry.Separator(),
            Item("Video converter", LucideIcons.file_video, () => TaskHelpers.OpenVideoConverter()),
            Item("Video thumbnailer", LucideIcons.clapperboard, () => TaskHelpers.OpenVideoThumbnailer()),
            MainMenuEntry.Separator(),
            Item("Analyze image", LucideIcons.bot, () => TaskHelpers.AnalyzeImage()),
            Item("OCR", LucideIcons.scan_text, async () => await TaskHelpers.OCRImage()),
            Item("QR code", LucideIcons.qr_code, () => TaskHelpers.OpenQRCode()),
            Item("Hash checker", LucideIcons.hash, () => TaskHelpers.OpenHashCheck()),
            Item("Metadata", LucideIcons.tags, () => TaskHelpers.OpenMetadataWindow()),
            Item("Index folder", LucideIcons.folder_tree, () => TaskHelpers.OpenDirectoryIndexer()),
            MainMenuEntry.Separator(),
            Item("Clipboard viewer", LucideIcons.clipboard_list, () => TaskHelpers.OpenClipboardViewer()),
            Item("Borderless window", LucideIcons.frame, () => TaskHelpers.OpenBorderlessWindow()),
            Item("Inspect window", LucideIcons.scan_search, () => TaskHelpers.OpenInspectWindow()),
            Item("Monitor test", LucideIcons.test_tube, () => TaskHelpers.OpenMonitorTest())
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

            items.Add(Item("Add workflows from Hotkey settings...", LucideIcons.keyboard,
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
            new MainMenuEntry("Enable add image effects", LucideIcons.wand_sparkles,
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
            items.Add(new MainMenuEntry("No image effect presets", LucideIcons.wand_sparkles, isEnabled: false));
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
            Parent("Image uploader", LucideIcons.image, () => BuildImageDestinations()),
            Parent("Text uploader", LucideIcons.file_text, () => BuildTextDestinations()),
            Parent("File uploader", LucideIcons.file_up, () => BuildEnumDestinations(
                Program.DefaultTaskSettings.FileDestination,
                value => Program.DefaultTaskSettings.FileDestination = value)),
            Parent("URL shortener", LucideIcons.link_2, () => BuildEnumDestinations(
                Program.DefaultTaskSettings.URLShortenerDestination,
                value => Program.DefaultTaskSettings.URLShortenerDestination = value)),
            Parent("URL sharing service", LucideIcons.globe_2, () => BuildEnumDestinations(
                Program.DefaultTaskSettings.URLSharingServiceDestination,
                value => Program.DefaultTaskSettings.URLSharingServiceDestination = value))
        };
    }

    private static IReadOnlyList<MainMenuEntry> BuildImageDestinations()
    {
        return Helpers.GetEnums<ImageDestination>().Select(value => new MainMenuEntry(
            value.GetLocalizedDescription(),
            value == ImageDestination.FileUploader ? LucideIcons.file_up : LucideIcons.image,
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
            value == TextDestination.FileUploader ? LucideIcons.file_up : LucideIcons.file_text,
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
            IconForName(value.ToString()),
            () => setValue(value),
            isChecked: EqualityComparer<T>.Default.Equals(selected, value),
            toggleType: MainMenuToggleType.Radio)).ToArray();
    }

    private IReadOnlyList<MainMenuEntry> BuildDebugMenu()
    {
        bool uploadsEnabled = !SystemOptions.DisableUpload;
        return new List<MainMenuEntry>
        {
            Item("Show debug log", LucideIcons.file_text, () => Run(MainFormCommand.DebugLog)),
            Item("Test image upload", LucideIcons.image_up, () => Run(MainFormCommand.TestImageUpload), uploadsEnabled),
            Item("Test text upload", LucideIcons.file_up, () => Run(MainFormCommand.TestTextUpload), uploadsEnabled),
            Item("Test file upload", LucideIcons.upload, () => Run(MainFormCommand.TestFileUpload), uploadsEnabled),
            Item("Test URL shortener", LucideIcons.link_2, () => Run(MainFormCommand.TestUrlShortener), uploadsEnabled),
            Item("Test URL sharing", LucideIcons.globe_2, () => Run(MainFormCommand.TestUrlSharing), uploadsEnabled)
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
            Item("Copy", LucideIcons.copy, task.Copy),
            Item("Open", LucideIcons.external_link, task.Open)
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
