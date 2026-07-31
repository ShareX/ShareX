#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Controls;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib;
using ShareX.Tools;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DrawingRectangle = System.Drawing.Rectangle;
using DrawingSize = System.Drawing.Size;
using WinForms = System.Windows.Forms;

namespace ShareX;

internal sealed class TaskSettingsPageBuilder
{
    private readonly TaskSettingsWindow _window;
    private readonly TaskSettings _settings;
    private readonly bool _isDefault;
    private readonly TaskSettingsGeneral _generalSettings;
    private readonly TaskSettingsImage _imageSettings;
    private readonly TaskSettingsCapture _captureSettings;
    private readonly TaskSettingsUpload _uploadSettings;
    private readonly TaskSettingsTools _toolsSettings;
    private readonly TaskSettingsAdvanced _advancedSettings;
    private readonly List<ExternalProgram> _externalPrograms;

    private readonly BoundValue<bool> _generalOverride;
    private readonly BoundValue<bool> _imageOverride;
    private readonly BoundValue<bool> _captureOverride;
    private readonly BoundValue<bool> _uploadOverride;
    private readonly BoundValue<bool> _toolsOverride;
    private readonly BoundValue<bool> _actionsOverride;
    private readonly BoundValue<bool> _advancedOverride;

    public TaskSettingsPageBuilder(TaskSettingsWindow window, TaskSettings settings, bool isDefault)
    {
        _window = window;
        _settings = settings;
        _isDefault = isDefault;

        _generalSettings = settings.GeneralSettings ?? new TaskSettingsGeneral();
        _imageSettings = settings.ImageSettings ?? new TaskSettingsImage();
        _captureSettings = settings.CaptureSettings ?? new TaskSettingsCapture();
        _uploadSettings = settings.UploadSettings ?? new TaskSettingsUpload();
        _toolsSettings = settings.ToolsSettings ?? new TaskSettingsTools();
        _advancedSettings = settings.AdvancedSettings ?? new TaskSettingsAdvanced();
        _externalPrograms = settings.ExternalPrograms ?? [];

        _generalOverride = OverrideValue(() => !_settings.UseDefaultGeneralSettings, value =>
        {
            _settings.UseDefaultGeneralSettings = !value;
            if (value) _settings.GeneralSettings = _generalSettings;
        });
        _imageOverride = OverrideValue(() => !_settings.UseDefaultImageSettings, value =>
        {
            _settings.UseDefaultImageSettings = !value;
            if (value) _settings.ImageSettings = _imageSettings;
        });
        _captureOverride = OverrideValue(() => !_settings.UseDefaultCaptureSettings, value =>
        {
            _settings.UseDefaultCaptureSettings = !value;
            if (value) _settings.CaptureSettings = _captureSettings;
        });
        _uploadOverride = OverrideValue(() => !_settings.UseDefaultUploadSettings, value =>
        {
            _settings.UseDefaultUploadSettings = !value;
            if (value) _settings.UploadSettings = _uploadSettings;
        });
        _toolsOverride = OverrideValue(() => !_settings.UseDefaultToolsSettings, value =>
        {
            _settings.UseDefaultToolsSettings = !value;
            if (value) _settings.ToolsSettings = _toolsSettings;
        });
        _actionsOverride = OverrideValue(() => !_settings.UseDefaultActions, value =>
        {
            _settings.UseDefaultActions = !value;
            if (value) _settings.ExternalPrograms = _externalPrograms;
        });
        _advancedOverride = OverrideValue(() => !_settings.UseDefaultAdvancedSettings, value =>
        {
            _settings.UseDefaultAdvancedSettings = !value;
            if (value) _settings.AdvancedSettings = _advancedSettings;
        });
    }

    public IReadOnlyDictionary<string, Control> BuildPages()
    {
        Dictionary<string, Control> pages = [];

        if (!_isDefault)
        {
            pages.Add("task", BuildTaskPage());
        }

        pages.Add("general", ParentPage("general", "General", LucideIcons.settings, _generalOverride, "Override general settings"));
        pages.Add("general-notifications", BuildNotificationsPage());
        pages.Add("image", BuildImagePage());
        pages.Add("image-effects", BuildImageEffectsPage());
        pages.Add("image-thumbnail", BuildThumbnailPage());
        pages.Add("capture", BuildCapturePage());
        pages.Add("capture-region", BuildRegionCapturePage());
        pages.Add("capture-screen-recorder", BuildScreenRecorderPage());
        pages.Add("capture-ocr", BuildOcrPage());
        pages.Add("upload", ParentPage("upload", "Upload", LucideIcons.upload, _uploadOverride, "Override upload settings"));
        pages.Add("upload-file-naming", BuildFileNamingPage());
        pages.Add("upload-clipboard", BuildClipboardUploadPage());
        pages.Add("upload-filters", BuildUploaderFiltersPage());
        pages.Add("tools", BuildToolsPage());
        pages.Add("actions", BuildActionsPage());
        pages.Add("watch-folders", BuildWatchFoldersPage());
        pages.Add("advanced", BuildAdvancedPage());

        return pages;
    }

    private Control BuildTaskPage()
    {
        BoundValue<bool> afterCaptureOverride = OverrideValue(
            () => !_settings.UseDefaultAfterCaptureJob,
            value => _settings.UseDefaultAfterCaptureJob = !value);
        BoundValue<bool> afterUploadOverride = OverrideValue(
            () => !_settings.UseDefaultAfterUploadJob,
            value => _settings.UseDefaultAfterUploadJob = !value);
        BoundValue<bool> destinationsOverride = OverrideValue(
            () => !_settings.UseDefaultDestinations,
            value => _settings.UseDefaultDestinations = !value);

        Control afterCapture = FlagsEditor(
            () => _settings.AfterCaptureJob,
            value => _settings.AfterCaptureJob = value,
            Enum.GetValues<AfterCaptureTasks>().Where(x => x != AfterCaptureTasks.None));
        BindEnabled(afterCapture, afterCaptureOverride);

        Control afterUpload = FlagsEditor(
            () => _settings.AfterUploadJob,
            value => _settings.AfterUploadJob = value,
            Enum.GetValues<AfterUploadTasks>().Where(x => x != AfterUploadTasks.None));
        BindEnabled(afterUpload, afterUploadOverride);

        StackPanel destinations = new() { Spacing = 4 };
        destinations.Children.Add(Row("Image uploader:", EnumCombo(() => _settings.ImageDestination, value => _settings.ImageDestination = value)));
        destinations.Children.Add(Row("Image file uploader:", EnumCombo(() => _settings.ImageFileDestination, value => _settings.ImageFileDestination = value)));
        destinations.Children.Add(Row("Text uploader:", EnumCombo(() => _settings.TextDestination, value => _settings.TextDestination = value)));
        destinations.Children.Add(Row("Text file uploader:", EnumCombo(() => _settings.TextFileDestination, value => _settings.TextFileDestination = value)));
        destinations.Children.Add(Row("File uploader:", EnumCombo(() => _settings.FileDestination, value => _settings.FileDestination = value)));
        destinations.Children.Add(Row("URL shortener:", EnumCombo(() => _settings.URLShortenerDestination, value => _settings.URLShortenerDestination = value)));
        destinations.Children.Add(Row("URL sharing service:", EnumCombo(() => _settings.URLSharingServiceDestination, value => _settings.URLSharingServiceDestination = value)));
        BindEnabled(destinations, destinationsOverride);

        List<Control> accountControls = [];

        if (Program.UploadersConfig?.FTPAccountList.Count > 0)
        {
            BoundValue<bool> ftpOverride = new(_settings.OverrideFTP, value => _settings.OverrideFTP = value);
            ComboBox ftp = ObjectCombo(
                Program.UploadersConfig.FTPAccountList,
                () => Program.UploadersConfig.FTPAccountList[_settings.FTPIndex.BetweenOrDefault(0, Program.UploadersConfig.FTPAccountList.Count - 1)],
                value => _settings.FTPIndex = Program.UploadersConfig.FTPAccountList.IndexOf(value));
            BindEnabled(ftp, ftpOverride);
            accountControls.Add(Check("Override default FTP account", ftpOverride));
            accountControls.Add(Row("FTP account:", ftp));
        }

        if (Program.UploadersConfig?.CustomUploadersList.Count > 0)
        {
            BoundValue<bool> customOverride = new(_settings.OverrideCustomUploader, value => _settings.OverrideCustomUploader = value);
            ComboBox custom = ObjectCombo(
                Program.UploadersConfig.CustomUploadersList,
                () => Program.UploadersConfig.CustomUploadersList[_settings.CustomUploaderIndex.BetweenOrDefault(0, Program.UploadersConfig.CustomUploadersList.Count - 1)],
                value => _settings.CustomUploaderIndex = Program.UploadersConfig.CustomUploadersList.IndexOf(value));
            BindEnabled(custom, customOverride);
            accountControls.Add(Check("Override default custom uploader", customOverride));
            accountControls.Add(Row("Custom uploader:", custom));
        }

        BoundValue<bool> folderOverride = new(_settings.OverrideScreenshotsFolder, value => _settings.OverrideScreenshotsFolder = value);
        TextBox folderText = Text(() => _settings.ScreenshotsFolder, value => _settings.ScreenshotsFolder = value);
        Button browseFolder = Button("Browse...", async () =>
        {
            string? path = await PickFolderAsync("Choose screenshots folder");
            if (!string.IsNullOrEmpty(path))
            {
                ((BoundValue<string>)folderText.DataContext!).Value = path;
            }
        });
        BindEnabled(folderText, folderOverride);
        BindEnabled(browseFolder, folderOverride);
        Grid folderRow = InputWithButton(folderText, browseFolder);

        return Page("task", "Task", LucideIcons.keyboard,
            Card("Task", Row("Task:", EnumCombo(() => _settings.Job, value => _settings.Job = value)),
                Row("Description:", Text(() => _settings.Description, value => _settings.Description = value))),
            Card("After capture tasks", Check("Override after capture tasks", afterCaptureOverride), afterCapture),
            Card("After upload tasks", Check("Override after upload tasks", afterUploadOverride), afterUpload),
            Card("Destinations", Check("Override destinations", destinationsOverride), destinations),
            Card("Uploader accounts", accountControls.ToArray()),
            Card("Screenshots folder", Check("Override screenshots folder", folderOverride), folderRow));
    }

    private Control BuildNotificationsPage()
    {
        TaskSettingsGeneral general = _settings.GeneralSettings;
        BoundValue<bool> showToast = new(general.ShowToastNotificationAfterTaskCompleted, value => general.ShowToastNotificationAfterTaskCompleted = value);

        StackPanel toastOptions = new() { Spacing = 4 };
        toastOptions.Children.Add(Row("Duration (seconds):", Number(() => (decimal)general.ToastWindowDuration, value => general.ToastWindowDuration = (float)value, 0, 60, 0.1m)));
        toastOptions.Children.Add(Row("Fade duration (seconds):", Number(() => (decimal)general.ToastWindowFadeDuration, value => general.ToastWindowFadeDuration = (float)value, 0, 10, 0.1m)));
        toastOptions.Children.Add(Row("Placement:", EnumCombo(() => general.ToastWindowPlacement, value => general.ToastWindowPlacement = value)));
        toastOptions.Children.Add(Row("Width:", Number(() => general.ToastWindowSize.Width, value => general.ToastWindowSize = new DrawingSize((int)value, general.ToastWindowSize.Height), 100, 2000)));
        toastOptions.Children.Add(Row("Height:", Number(() => general.ToastWindowSize.Height, value => general.ToastWindowSize = new DrawingSize(general.ToastWindowSize.Width, (int)value), 50, 2000)));
        toastOptions.Children.Add(Row("Left click action:", EnumCombo(() => general.ToastWindowLeftClickAction, value => general.ToastWindowLeftClickAction = value)));
        toastOptions.Children.Add(Row("Right click action:", EnumCombo(() => general.ToastWindowRightClickAction, value => general.ToastWindowRightClickAction = value)));
        toastOptions.Children.Add(Row("Middle click action:", EnumCombo(() => general.ToastWindowMiddleClickAction, value => general.ToastWindowMiddleClickAction = value)));
        toastOptions.Children.Add(Row("Notification button size:", Number(() => general.ToastWindowButtonSize,
            value => general.ToastWindowButtonSize = (int)value, 16, 128)));
        toastOptions.Children.Add(Row("Notification buttons:", Button("Configure...", () =>
            _window.ShowNotificationButtonsEditor(general.ToastWindowButtons, buttons => general.ToastWindowButtons = buttons))));
        toastOptions.Children.Add(Check("Automatically hide on screen capture", () => general.ToastWindowAutoHide, value => general.ToastWindowAutoHide = value));
        toastOptions.Children.Add(Check("Disable toast notifications on fullscreen", () => general.DisableNotificationsOnFullscreen, value => general.DisableNotificationsOnFullscreen = value));
        BindEnabled(toastOptions, showToast);

        return Page("general-notifications", "Notifications", LucideIcons.bell,
            EnabledCard(_generalOverride, "Sounds",
                Check("Play sound after capture is made", () => general.PlaySoundAfterCapture, value => general.PlaySoundAfterCapture = value),
                Check("Play sound after task is completed", () => general.PlaySoundAfterUpload, value => general.PlaySoundAfterUpload = value),
                Check("Play sound after action is completed", () => general.PlaySoundAfterAction, value => general.PlaySoundAfterAction = value)),
            EnabledCard(_generalOverride, "Toast notification",
                Check("Show toast notification after task is completed", showToast), toastOptions),
            EnabledCard(_generalOverride, "Custom sounds",
                SoundPath("Use custom capture sound", () => general.UseCustomCaptureSound, value => general.UseCustomCaptureSound = value,
                    () => general.CustomCaptureSoundPath, value => general.CustomCaptureSoundPath = value),
                SoundPath("Use custom task completed sound", () => general.UseCustomTaskCompletedSound, value => general.UseCustomTaskCompletedSound = value,
                    () => general.CustomTaskCompletedSoundPath, value => general.CustomTaskCompletedSoundPath = value),
                SoundPath("Use custom action completed sound", () => general.UseCustomActionCompletedSound, value => general.UseCustomActionCompletedSound = value,
                    () => general.CustomActionCompletedSoundPath, value => general.CustomActionCompletedSoundPath = value),
                SoundPath("Use custom error sound", () => general.UseCustomErrorSound, value => general.UseCustomErrorSound = value,
                    () => general.CustomErrorSoundPath, value => general.CustomErrorSoundPath = value)));
    }

    private Control BuildImagePage()
    {
        TaskSettingsImage image = _settings.ImageSettings;
        BoundValue<bool> autoJpeg = new(image.ImageAutoUseJPEG, value => image.ImageAutoUseJPEG = value);
        NumericUpDown autoJpegSize = Number(() => image.ImageAutoUseJPEGSize, value => image.ImageAutoUseJPEGSize = (int)value, 0, int.MaxValue);
        CheckBox autoQuality = Check("Adjust JPEG quality automatically to keep image size near the specified size", () => image.ImageAutoJPEGQuality, value => image.ImageAutoJPEGQuality = value);
        BindEnabled(autoJpegSize, autoJpeg);
        BindEnabled(autoQuality, autoJpeg);

        return Page("image", "Image", LucideIcons.image,
            OverrideCard(_imageOverride, "Override image settings"),
            EnabledCard(_imageOverride, "Image quality",
                Row("Image format:", EnumCombo(() => image.ImageFormat, value => image.ImageFormat = value)),
                Row("PNG bit depth:", EnumCombo(() => image.ImagePNGBitDepth, value => image.ImagePNGBitDepth = value)),
                Row("JPEG quality:", Number(() => image.ImageJPEGQuality, value => image.ImageJPEGQuality = (int)value, 0, 100)),
                Row("GIF quality:", EnumCombo(() => image.ImageGIFQuality, value => image.ImageGIFQuality = value)),
                Row("If file exists:", EnumCombo(() => image.FileExistAction, value => image.FileExistAction = value))),
            EnabledCard(_imageOverride, "Automatic JPEG",
                Check("Use JPEG if image size is bigger than the specified size", autoJpeg),
                Row("Size limit (kB):", autoJpegSize), autoQuality));
    }

    private Control BuildImageEffectsPage()
    {
        TaskSettingsImage image = _settings.ImageSettings;
        return Page("image-effects", "Effects", LucideIcons.wand_sparkles,
            EnabledCard(_imageOverride, "Image effects",
                Check("Show image effects window after capture", () => image.ShowImageEffectsWindowAfterCapture, value => image.ShowImageEffectsWindowAfterCapture = value),
                Check("Only apply effects to region capture", () => image.ImageEffectOnlyRegionCapture, value => image.ImageEffectOnlyRegionCapture = value),
                Check("Use random image effect", () => image.UseRandomImageEffect, value => image.UseRandomImageEffect = value),
                Button("Image effects configuration...", () => TaskHelpers.OpenImageEffectsSingleton(_settings)),
                Hint("You can enable or disable image effects from After capture tasks → Add image effects.")));
    }

    private Control BuildThumbnailPage()
    {
        TaskSettingsImage image = _settings.ImageSettings;
        BoundValue<string> name = new(image.ThumbnailName, value => image.ThumbnailName = value);
        TextBlock preview = Hint("ImageName" + image.ThumbnailName + ".jpg");
        name.PropertyChanged += (_, _) => preview.Text = "ImageName" + name.Value + ".jpg";

        return Page("image-thumbnail", "Thumbnail", LucideIcons.images,
            EnabledCard(_imageOverride, "Thumbnail",
                Row("Width:", Number(() => image.ThumbnailWidth, value => image.ThumbnailWidth = (int)value, 0, 10000)),
                Row("Height:", Number(() => image.ThumbnailHeight, value => image.ThumbnailHeight = (int)value, 0, 10000)),
                Row("Thumbnail name:", Text(name)),
                Row("Preview:", preview),
                Check("Create thumbnail only if image is bigger than thumbnail size", () => image.ThumbnailCheckSize, value => image.ThumbnailCheckSize = value)));
    }

    private Control BuildCapturePage()
    {
        TaskSettingsCapture capture = _settings.CaptureSettings;
        BoundValue<bool> transparent = new(capture.CaptureTransparent, value => capture.CaptureTransparent = value);
        CheckBox shadow = Check("Capture window with shadow", () => capture.CaptureShadow, value => capture.CaptureShadow = value);
        NumericUpDown shadowOffset = Number(() => capture.CaptureShadowOffset, value => capture.CaptureShadowOffset = (int)value, 0, 1000);
        BindEnabled(shadow, transparent);
        BindEnabled(shadowOffset, transparent);

        BoundValue<decimal?> regionX = NumericValue(capture.CaptureCustomRegion.X, value =>
            capture.CaptureCustomRegion = new DrawingRectangle((int)(value ?? 0), capture.CaptureCustomRegion.Y, capture.CaptureCustomRegion.Width, capture.CaptureCustomRegion.Height));
        BoundValue<decimal?> regionY = NumericValue(capture.CaptureCustomRegion.Y, value =>
            capture.CaptureCustomRegion = new DrawingRectangle(capture.CaptureCustomRegion.X, (int)(value ?? 0), capture.CaptureCustomRegion.Width, capture.CaptureCustomRegion.Height));
        BoundValue<decimal?> regionWidth = NumericValue(capture.CaptureCustomRegion.Width, value =>
            capture.CaptureCustomRegion = new DrawingRectangle(capture.CaptureCustomRegion.X, capture.CaptureCustomRegion.Y, (int)(value ?? 0), capture.CaptureCustomRegion.Height));
        BoundValue<decimal?> regionHeight = NumericValue(capture.CaptureCustomRegion.Height, value =>
            capture.CaptureCustomRegion = new DrawingRectangle(capture.CaptureCustomRegion.X, capture.CaptureCustomRegion.Y, capture.CaptureCustomRegion.Width, (int)(value ?? 0)));

        Button selectRegion = Button("Select region...", () =>
        {
            if (RegionCaptureTasks.GetRectangleRegion(out DrawingRectangle rectangle, capture.SurfaceOptions))
            {
                regionX.Value = rectangle.X;
                regionY.Value = rectangle.Y;
                regionWidth.Value = rectangle.Width;
                regionHeight.Value = rectangle.Height;
            }
        });

        Grid regionGrid = new()
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,130,Auto,130"),
            RowDefinitions = new RowDefinitions("Auto,Auto"),
            ColumnSpacing = 8,
            RowSpacing = 4
        };
        AddGridLabel(regionGrid, "X:", 0, 0);
        AddGridControl(regionGrid, Number(regionX, -100000, 100000), 0, 1);
        AddGridLabel(regionGrid, "Y:", 0, 2);
        AddGridControl(regionGrid, Number(regionY, -100000, 100000), 0, 3);
        AddGridLabel(regionGrid, "Width:", 1, 0);
        AddGridControl(regionGrid, Number(regionWidth, 0, 100000), 1, 1);
        AddGridLabel(regionGrid, "Height:", 1, 2);
        AddGridControl(regionGrid, Number(regionHeight, 0, 100000), 1, 3);

        return Page("capture", "Capture", LucideIcons.camera,
            OverrideCard(_captureOverride, "Override capture settings"),
            EnabledCard(_captureOverride, "Screenshots",
                Check("Show cursor in screenshots", () => capture.ShowCursor, value => capture.ShowCursor = value),
                Row("Screenshot delay (seconds):", Number(() => capture.ScreenshotDelay, value => capture.ScreenshotDelay = value, 0, 60, 0.1m)),
                Check("Capture window with transparency", transparent), shadow,
                Row("Shadow offset:", shadowOffset),
                Check("Capture client area for window captures", () => capture.CaptureClientArea, value => capture.CaptureClientArea = value),
                Check("Hide taskbar when it intersects a captured window", () => capture.CaptureAutoHideTaskbar, value => capture.CaptureAutoHideTaskbar = value),
                Check("Automatically hide desktop icons", () => capture.CaptureAutoHideDesktopIcons, value => capture.CaptureAutoHideDesktopIcons = value),
                Check("HDR screenshot color corrector", () => capture.HDRScreenshotColorCorrection, value => capture.HDRScreenshotColorCorrection = value)),
            EnabledCard(_captureOverride, "Preconfigured region", regionGrid, selectRegion),
            EnabledCard(_captureOverride, "Preconfigured window",
                Row("Window title:", Text(() => capture.CaptureCustomWindow, value => capture.CaptureCustomWindow = value))));
    }

    private Control BuildRegionCapturePage()
    {
        RegionCaptureOptions options = _settings.CaptureSettings.SurfaceOptions;
        BoundValue<bool> detectWindows = new(options.DetectWindows, value => options.DetectWindows = value);
        CheckBox detectControls = Check("Also detect controls inside windows", () => options.DetectControls, value => options.DetectControls = value);
        BindEnabled(detectControls, detectWindows);

        BoundValue<bool> customInfo = new(options.UseCustomInfoText, value => options.UseCustomInfoText = value);
        TextBox customInfoText = Text(() => options.CustomInfoText, value => options.CustomInfoText = value.Replace("\r\n", "$n").Replace("\n", "$n"));
        BindEnabled(customInfoText, customInfo);

        BoundValue<bool> magnifier = new(options.ShowMagnifier, value => options.ShowMagnifier = value);
        CheckBox squareMagnifier = Check("Use square magnifier", () => options.UseSquareMagnifier, value => options.UseSquareMagnifier = value);
        NumericUpDown pixelCount = Number(() => options.MagnifierPixelCount, value => options.MagnifierPixelCount = (int)value,
            RegionCaptureOptions.MagnifierPixelCountMinimum, RegionCaptureOptions.MagnifierPixelCountMaximum);
        NumericUpDown pixelSize = Number(() => options.MagnifierPixelSize, value => options.MagnifierPixelSize = (int)value,
            RegionCaptureOptions.MagnifierPixelSizeMinimum, RegionCaptureOptions.MagnifierPixelSizeMaximum);
        BindEnabled(squareMagnifier, magnifier);
        BindEnabled(pixelCount, magnifier);
        BindEnabled(pixelSize, magnifier);

        BoundValue<bool> fixedSize = new(options.IsFixedSize, value => options.IsFixedSize = value);
        NumericUpDown fixedWidth = Number(() => options.FixedSize.Width, value => options.FixedSize = new DrawingSize((int)value, options.FixedSize.Height), 1, 100000);
        NumericUpDown fixedHeight = Number(() => options.FixedSize.Height, value => options.FixedSize = new DrawingSize(options.FixedSize.Width, (int)value), 1, 100000);
        BindEnabled(fixedWidth, fixedSize);
        BindEnabled(fixedHeight, fixedSize);

        ObservableCollection<SnapSize> snapSizes = new(options.SnapSizes);
        ListBox snapList = new() { ItemsSource = snapSizes, MaxHeight = 125 };
        snapList.Classes.Add("settings-list");
        NumericUpDown snapWidth = Number(() => 640, _ => { }, 1, 100000);
        NumericUpDown snapHeight = Number(() => 360, _ => { }, 1, 100000);
        Button addSnap = Button("Add", () =>
        {
            SnapSize size = new((int)(snapWidth.Value ?? 640), (int)(snapHeight.Value ?? 360));
            options.SnapSizes.Add(size);
            snapSizes.Add(size);
            snapList.SelectedItem = size;
        });
        Button removeSnap = Button("Remove", () =>
        {
            if (snapList.SelectedItem is SnapSize size)
            {
                options.SnapSizes.Remove(size);
                snapSizes.Remove(size);
            }
        });

        StackPanel snapEditor = new() { Spacing = 4 };
        snapEditor.Children.Add(snapList);
        snapEditor.Children.Add(new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Children = { Label("Width:"), snapWidth, Label("Height:"), snapHeight, addSnap, removeSnap }
        });

        return Page("capture-region", "Region capture", LucideIcons.crop,
            EnabledCard(_captureOverride, "Selection",
                Check("Use multi region mode", () => !options.QuickCrop, value => options.QuickCrop = !value),
                Check("Detect window regions", detectWindows), detectControls,
                Check("Restrict capture and cursor to the active monitor", () => options.ActiveMonitorMode, value => options.ActiveMonitorMode = value),
                Row("Background dim strength (%):", Number(() => options.BackgroundDimStrength, value => options.BackgroundDimStrength = (int)value, 0, 100))),
            EnabledCard(_captureOverride, "Mouse actions",
                Row("Right click:", EnumCombo(() => options.RegionCaptureActionRightClick, value => options.RegionCaptureActionRightClick = value)),
                Row("Middle click:", EnumCombo(() => options.RegionCaptureActionMiddleClick, value => options.RegionCaptureActionMiddleClick = value)),
                Row("Mouse 4 click:", EnumCombo(() => options.RegionCaptureActionX1Click, value => options.RegionCaptureActionX1Click = value)),
                Row("Mouse 5 click:", EnumCombo(() => options.RegionCaptureActionX2Click, value => options.RegionCaptureActionX2Click = value))),
            EnabledCard(_captureOverride, "Information and magnifier",
                Check("Show position and size info", () => options.ShowInfo, value => options.ShowInfo = value),
                Check("Use custom info text", customInfo), customInfoText,
                Check("Show magnifier near cursor", magnifier), squareMagnifier,
                Row("Magnifier pixel count:", pixelCount),
                Row("Magnifier pixel size:", pixelSize),
                Check("Show center crosshair", () => options.ShowCenterCrosshair, value => options.ShowCenterCrosshair = value),
                Check("Show screen-wide crosshair", () => options.ShowCrosshair, value => options.ShowCrosshair = value)),
            EnabledCard(_captureOverride, "Fixed size and performance",
                Check("Fixed size region mode", fixedSize),
                Row("Fixed width:", fixedWidth), Row("Fixed height:", fixedHeight),
                Check("Show FPS", () => options.ShowFPS, value => options.ShowFPS = value),
                Row("FPS limit:", Number(() => options.FPSLimit, value => options.FPSLimit = (int)value, 1, 1000))),
            EnabledCard(_captureOverride, "Snap sizes", snapEditor));
    }

    private Control BuildScreenRecorderPage()
    {
        TaskSettingsCapture capture = _settings.CaptureSettings;
        BoundValue<bool> fixedDuration = new(capture.ScreenRecordFixedDuration, value => capture.ScreenRecordFixedDuration = value);
        NumericUpDown duration = Number(() => (decimal)capture.ScreenRecordDuration, value => capture.ScreenRecordDuration = (float)value, 0, 86400, 0.1m);
        BindEnabled(duration, fixedDuration);

        BoundValue<bool> autoStart = new(capture.ScreenRecordAutoStart, value => capture.ScreenRecordAutoStart = value);
        NumericUpDown startDelay = Number(() => (decimal)capture.ScreenRecordStartDelay, value => capture.ScreenRecordStartDelay = (float)value, 0, 3600, 0.1m);
        BindEnabled(startDelay, autoStart);

        return Page("capture-screen-recorder", "Screen recorder", LucideIcons.video,
            EnabledCard(_captureOverride, "Recording",
                Row("Screen recording FPS:", Number(() => capture.ScreenRecordFPS, value => capture.ScreenRecordFPS = (int)value, 1, HelpersOptions.DevMode ? 300 : 60)),
                Row("GIF FPS:", Number(() => capture.GIFFPS, value => capture.GIFFPS = (int)value, 1, HelpersOptions.DevMode ? 60 : 30)),
                Check("Show cursor in recording", () => capture.ScreenRecordShowCursor, value => capture.ScreenRecordShowCursor = value),
                Check("Start recording after a delay", autoStart), Row("Start delay (seconds):", startDelay),
                Check("Use fixed duration", fixedDuration), Row("Duration (seconds):", duration)),
            EnabledCard(_captureOverride, "Encoding and capture",
                Check("Record losslessly first, then apply encoding options", () => capture.ScreenRecordTwoPassEncoding, value => capture.ScreenRecordTwoPassEncoding = value),
                Check("Ask for confirmation when aborting", () => capture.ScreenRecordAskConfirmationOnAbort, value => capture.ScreenRecordAskConfirmationOnAbort = value),
                Check("Use transparent region selection", () => capture.ScreenRecordTransparentRegion, value => capture.ScreenRecordTransparentRegion = value),
                Button("Screen recording options...", ShowScreenRecordingOptions)));
    }

    private async Task ShowScreenRecordingOptions()
    {
        TaskSettingsCapture capture = _settings.CaptureSettings;
        ScreenRecordingOptions options = new()
        {
            IsRecording = true,
            FFmpeg = capture.FFmpegOptions,
            FPS = capture.ScreenRecordFPS,
            Duration = capture.ScreenRecordFixedDuration ? capture.ScreenRecordDuration : 0,
            OutputPath = "output.mp4",
            CaptureArea = WinForms.Screen.PrimaryScreen?.Bounds ?? DrawingRectangle.Empty,
            DrawCursor = capture.ScreenRecordShowCursor
        };

        FFmpegOptionsWindow window = new(options);
        await window.ShowDialog(_window);
        capture.FFmpegOptions = window.Options.FFmpeg;
    }

    private Control BuildOcrPage()
    {
        OCROptions options = _settings.CaptureSettings.OCROptions;
        ComboBox language;

        try
        {
            OCRLanguageOption[] languages = OCRHelper.AvailableLanguages.OrderBy(x => x.DisplayName).ToArray();
            OCRLanguageOption selected = languages.FirstOrDefault(x => x.LanguageTag.Equals(options.Language, StringComparison.OrdinalIgnoreCase)) ?? languages.First();
            options.Language = selected.LanguageTag;
            language = ObjectCombo(languages, () => selected, value => options.Language = value.LanguageTag, value => value.DisplayName);
        }
        catch
        {
            language = new ComboBox { IsEnabled = false, PlaceholderText = "OCR languages are unavailable" };
            language.Classes.Add("form-control");
        }

        BoundValue<bool> silent = new(options.Silent, value => options.Silent = value);
        CheckBox autoCopy = Check("Automatically copy results to clipboard", () => options.AutoCopy, value => options.AutoCopy = value);
        BindEnabled(autoCopy, silent, invert: true);

        return Page("capture-ocr", "OCR", LucideIcons.scan_text,
            EnabledCard(_captureOverride, "Optical character recognition",
                Row("Default language:", language),
                Check("Process OCR silently", silent), autoCopy,
                Check("Close OCR window after opening service link", () => options.CloseWindowAfterOpeningServiceLink, value => options.CloseWindowAfterOpeningServiceLink = value),
                Button("OCR help", () => URLHelpers.OpenURL(Links.DocsOCR))));
    }

    private Control BuildFileNamingPage()
    {
        TaskSettingsUpload upload = _settings.UploadSettings;
        TextBlock capturePreview = Hint(string.Empty);
        TextBlock windowPreview = Hint(string.Empty);

        void UpdatePreviews()
        {
            NameParser parser = new(NameParserType.FileName)
            {
                AutoIncrementNumber = Program.Settings.NameParserAutoIncrementNumber,
                ImageWidth = 1920,
                ImageHeight = 1080,
                MaxNameLength = _settings.AdvancedSettings.NamePatternMaxLength,
                MaxTitleLength = _settings.AdvancedSettings.NamePatternMaxTitleLength,
                CustomTimeZone = upload.UseCustomTimeZone ? upload.CustomTimeZone : null,
                IsPreviewMode = true
            };

            capturePreview.Text = parser.Parse(upload.NameFormatPattern);
            parser.WindowText = _window.Title;
            parser.ProcessName = "ShareX";
            windowPreview.Text = parser.Parse(upload.NameFormatPatternActiveWindow);
        }

        BoundValue<string> capturePattern = new(upload.NameFormatPattern, value => { upload.NameFormatPattern = value; UpdatePreviews(); });
        BoundValue<string> windowPattern = new(upload.NameFormatPatternActiveWindow, value => { upload.NameFormatPatternActiveWindow = value; UpdatePreviews(); });
        BoundValue<bool> customTimeZone = new(upload.UseCustomTimeZone, value => { upload.UseCustomTimeZone = value; UpdatePreviews(); });
        TimeZoneInfo[] timeZones = TimeZoneInfo.GetSystemTimeZones().ToArray();
        ComboBox timeZone = ObjectCombo(timeZones, () => upload.CustomTimeZone, value => { upload.CustomTimeZone = value; UpdatePreviews(); }, value => value.DisplayName);
        BindEnabled(timeZone, customTimeZone);

        BoundValue<bool> regexReplace = new(upload.URLRegexReplace, value => upload.URLRegexReplace = value);
        TextBox regexPattern = Text(() => upload.URLRegexReplacePattern, value => upload.URLRegexReplacePattern = value);
        TextBox regexReplacement = Text(() => upload.URLRegexReplaceReplacement, value => upload.URLRegexReplaceReplacement = value);
        BindEnabled(regexPattern, regexReplace);
        BindEnabled(regexReplacement, regexReplace);

        NumericUpDown autoIncrement = Number(() => Program.Settings.NameParserAutoIncrementNumber, value => Program.Settings.NameParserAutoIncrementNumber = (int)value, 0, int.MaxValue);
        if (autoIncrement.DataContext is BoundValue<decimal?> autoIncrementValue)
        {
            autoIncrementValue.PropertyChanged += (_, _) => UpdatePreviews();
        }

        UpdatePreviews();

        return Page("upload-file-naming", "File naming", LucideIcons.file_pen,
            EnabledCard(_uploadOverride, "Name patterns",
                Row("Capture or clipboard upload:", Text(capturePattern)), Row("Preview:", capturePreview),
                Row("Window capture:", Text(windowPattern)), Row("Preview:", windowPreview),
                Check("Use name pattern for file uploads", () => upload.FileUploadUseNamePattern, value => upload.FileUploadUseNamePattern = value),
                Check("Replace URL-problematic characters with underscores", () => upload.FileUploadReplaceProblematicCharacters, value => upload.FileUploadReplaceProblematicCharacters = value),
                Row("Auto increment number:", autoIncrement)),
            EnabledCard(_uploadOverride, "Time zone",
                Check("Use custom time zone", customTimeZone), Row("Time zone:", timeZone)),
            EnabledCard(_uploadOverride, "URL replacement",
                Check("Replace result URL using a regular expression", regexReplace),
                Row("Pattern:", regexPattern), Row("Replacement:", regexReplacement)));
    }

    private Control BuildClipboardUploadPage()
    {
        TaskSettingsUpload upload = _settings.UploadSettings;
        return Page("upload-clipboard", "Clipboard upload", LucideIcons.clipboard,
            EnabledCard(_uploadOverride, "Clipboard content",
                Check("If clipboard contains a file URL, download and upload it", () => upload.ClipboardUploadURLContents, value => upload.ClipboardUploadURLContents = value),
                Check("If clipboard contains a URL, use URL shortener", () => upload.ClipboardUploadShortenURL, value => upload.ClipboardUploadShortenURL = value),
                Check("If clipboard contains a URL, share it using URL sharing service", () => upload.ClipboardUploadShareURL, value => upload.ClipboardUploadShareURL = value),
                Check("If clipboard contains a folder path, index and upload it", () => upload.ClipboardUploadAutoIndexFolder, value => upload.ClipboardUploadAutoIndexFolder = value)));
    }

    private Control BuildUploaderFiltersPage()
    {
        TaskSettingsUpload upload = _settings.UploadSettings;
        upload.UploaderFilters ??= [];

        ObservableCollection<string> rows = new(upload.UploaderFilters.Select(FilterTitle));
        ListBox list = new() { ItemsSource = rows, MinHeight = 180 };
        list.Classes.Add("settings-list");

        IGenericUploaderService[] services = UploaderFactory.AllGenericUploaderServices.OrderBy(x => x.ServiceName).ToArray();
        IGenericUploaderService initialService = services.FirstOrDefault()!;
        ComboBox uploader = services.Length > 0
            ? ObjectCombo(services, () => initialService, _ => { }, value => value.ServiceName)
            : new ComboBox { IsEnabled = false };
        TextBox extensions = Text(() => string.Empty, _ => { });

        void LoadSelection()
        {
            int index = list.SelectedIndex;
            if (index < 0 || index >= upload.UploaderFilters.Count)
            {
                return;
            }

            UploaderFilter filter = upload.UploaderFilters[index];
            if (uploader.DataContext is BoundValue<ChoiceOption<IGenericUploaderService>> uploaderValue)
            {
                ChoiceOption<IGenericUploaderService>? match = ((IEnumerable<ChoiceOption<IGenericUploaderService>>)uploader.ItemsSource!)
                    .FirstOrDefault(x => x.Value.ServiceIdentifier.Equals(filter.Uploader, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    uploaderValue.Value = match;
                }
            }

            ((BoundValue<string>)extensions.DataContext!).Value = filter.GetExtensions();
        }

        list.SelectionChanged += (_, _) => LoadSelection();

        UploaderFilter? CreateFilter()
        {
            if (uploader.DataContext is not BoundValue<ChoiceOption<IGenericUploaderService>> selected)
            {
                return null;
            }

            UploaderFilter filter = new() { Uploader = selected.Value.Value.ServiceIdentifier };
            filter.SetExtensions(((BoundValue<string>)extensions.DataContext!).Value);
            return filter;
        }

        Button add = Button("Add", () =>
        {
            if (CreateFilter() is { } filter)
            {
                upload.UploaderFilters.Add(filter);
                rows.Add(FilterTitle(filter));
                list.SelectedIndex = rows.Count - 1;
            }
        });
        Button update = Button("Update", () =>
        {
            int index = list.SelectedIndex;
            if (index >= 0 && CreateFilter() is { } filter)
            {
                upload.UploaderFilters[index] = filter;
                rows[index] = FilterTitle(filter);
            }
        });
        Button remove = Button("Remove", () =>
        {
            int index = list.SelectedIndex;
            if (index >= 0)
            {
                upload.UploaderFilters.RemoveAt(index);
                rows.RemoveAt(index);
            }
        });

        return Page("upload-filters", "Uploader filters", LucideIcons.filter,
            EnabledCard(_uploadOverride, "Filters", list,
                Row("Uploader:", uploader),
                Row("Extensions:", extensions),
                Hint("Separate extensions with commas, for example: png, jpg, jpeg"),
                ButtonRow(add, update, remove)));
    }

    private static string FilterTitle(UploaderFilter filter) => $"{filter.Uploader} — {filter.GetExtensions()}";

    private Control BuildToolsPage()
    {
        TaskSettingsTools tools = _settings.ToolsSettings;
        var picker = tools.ScreenColorPickerOptions;
        return Page("tools", "Tools", LucideIcons.wrench,
            OverrideCard(_toolsOverride, "Override tools settings"),
            EnabledCard(_toolsOverride, "Image editor",
                Check("Use legacy image editor", () => tools.UseLegacyImageEditor, value => tools.UseLegacyImageEditor = value)),
            EnabledCard(_toolsOverride, "Screen color picker",
                Row("Format:", Text(() => picker.Format, value => picker.Format = value)),
                Row("Format (Ctrl + click):", Text(() => picker.FormatCtrl, value => picker.FormatCtrl = value)),
                Row("Info text:", Text(() => picker.InfoText, value => picker.InfoText = value)),
                Check("Show magnifier", () => picker.ShowMagnifier, value => picker.ShowMagnifier = value)));
    }

    private Control BuildActionsPage()
    {
        _settings.ExternalPrograms = _externalPrograms;
        TaskHelpers.AddDefaultExternalPrograms(_settings);

        ListBox list = new() { MinHeight = 230 };
        list.Classes.Add("settings-list");
        list.Classes.Add("action-list");
        Dictionary<Control, ExternalProgram> entries = [];

        void Refresh(ExternalProgram? selected = null)
        {
            list.Items.Clear();
            entries.Clear();
            foreach (ExternalProgram action in _externalPrograms)
            {
                CheckBox enabled = Check(string.Empty, () => action.IsActive, value => action.IsActive = value);
                enabled.Content = null;
                enabled.Classes.Remove("setting");
                enabled.Classes.Add("action-list-toggle");
                ToolTip.SetTip(enabled, "Enable action");

                Grid item = new()
                {
                    ColumnDefinitions = new ColumnDefinitions("Auto,Auto,*"),
                    ColumnSpacing = 6
                };
                item.Classes.Add("action-list-item");
                item.Children.Add(enabled);

                TextBlock name = new()
                {
                    Text = action.Name,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(name, 1);
                item.Children.Add(name);

                TextBlock path = new()
                {
                    Text = $"— {action.Path}",
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    VerticalAlignment = VerticalAlignment.Center
                };
                path.Classes.Add("action-list-path");
                Grid.SetColumn(path, 2);
                item.Children.Add(path);

                item.PointerPressed += (_, _) => list.SelectedItem = item;
                enabled.Click += (_, _) => list.SelectedItem = item;
                entries.Add(item, action);
                list.Items.Add(item);
                if (ReferenceEquals(action, selected))
                {
                    list.SelectedItem = item;
                }
            }
        }

        ExternalProgram? Selected() => list.SelectedItem is Control item && entries.TryGetValue(item, out ExternalProgram? action) ? action : null;
        Refresh();

        Button add = Button("Add...", () =>
        {
            _window.ShowActionEditor(null, action =>
            {
                _externalPrograms.Add(action);
                Refresh(action);
            });
        });
        Button edit = Button("Edit...", () =>
        {
            if (Selected() is { } action)
            {
                _window.ShowActionEditor(action, editedAction =>
                {
                    Refresh(editedAction);
                });
            }
        });
        Button duplicate = Button("Duplicate", () =>
        {
            if (Selected() is { } action)
            {
                ExternalProgram copy = action.Copy();
                _externalPrograms.Add(copy);
                Refresh(copy);
            }
        });
        Button remove = Button("Remove", () =>
        {
            if (Selected() is { } action)
            {
                _externalPrograms.Remove(action);
                Refresh();
            }
        });

        return Page("actions", "Actions", LucideIcons.zap,
            OverrideCard(_actionsOverride, "Override actions"),
            EnabledCard(_actionsOverride, "Actions", list, ButtonRow(add, edit, duplicate, remove),
                Hint("You can enable or disable actions from After capture tasks → Perform actions.")));
    }

    private Control BuildWatchFoldersPage()
    {
        _settings.WatchFolderList ??= [];

        foreach (WatchFolderSettings folder in _settings.WatchFolderList)
        {
            Program.WatchFolderManager?.AddWatchFolder(folder, _settings);
        }

        ObservableCollection<string> rows = new(_settings.WatchFolderList.Select(WatchFolderTitle));
        ListBox list = new() { ItemsSource = rows, MinHeight = 230 };
        list.Classes.Add("settings-list");

        void UpdateState(WatchFolderSettings folder) => Program.WatchFolderManager?.UpdateWatchFolderState(folder);

        Button add = Button("Add...", () =>
        {
            _window.ShowWatchFolderEditor(null, folder =>
            {
                Program.WatchFolderManager?.AddWatchFolder(folder, _settings);
                if (!_settings.WatchFolderList.Contains(folder))
                {
                    _settings.WatchFolderList.Add(folder);
                }
                rows.Add(WatchFolderTitle(folder));
                list.SelectedIndex = rows.Count - 1;
            });
        });
        Button edit = Button("Edit...", () =>
        {
            int index = list.SelectedIndex;
            if (index >= 0)
            {
                WatchFolderSettings folder = _settings.WatchFolderList[index];
                _window.ShowWatchFolderEditor(folder, editedFolder =>
                {
                    rows[index] = WatchFolderTitle(editedFolder);
                    UpdateState(editedFolder);
                });
            }
        });
        Button remove = Button("Remove", () =>
        {
            int index = list.SelectedIndex;
            if (index >= 0)
            {
                WatchFolderSettings folder = _settings.WatchFolderList[index];
                Program.WatchFolderManager?.RemoveWatchFolder(folder);
                _settings.WatchFolderList.Remove(folder);
                rows.RemoveAt(index);
            }
        });

        BoundValue<bool> enabled = new(_settings.WatchFolderEnabled, value =>
        {
            _settings.WatchFolderEnabled = value;
            foreach (WatchFolderSettings folder in _settings.WatchFolderList)
            {
                UpdateState(folder);
            }
        });

        return Page("watch-folders", "Watch folders", LucideIcons.folder_search,
            Card("Watch folders",
                Check("Watch folders and upload newly created files", enabled),
                list, ButtonRow(add, edit, remove)));
    }

    private static string WatchFolderTitle(WatchFolderSettings folder) =>
        $"{folder.FolderPath} — {folder.Filter} — Include subfolders: {folder.IncludeSubdirectories}";

    private Control BuildAdvancedPage()
    {
        List<Control> controls = [];
        if (!_isDefault)
        {
            controls.Add(OverrideCard(_advancedOverride, "Override advanced settings"));
        }

        IEnumerable<IGrouping<string, PropertyInfo>> categories = typeof(TaskSettingsAdvanced)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.CanRead && x.CanWrite && x.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false)
            .GroupBy(x => x.GetCustomAttribute<CategoryAttribute>()?.Category ?? "General");

        foreach (IGrouping<string, PropertyInfo> category in categories)
        {
            List<Control> settings = [];
            foreach (PropertyInfo property in category)
            {
                Control editor = AdvancedEditor(property);
                string title = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? SplitPascalCase(property.Name);
                string? description = property.GetCustomAttribute<DescriptionAttribute>()?.Description;

                Control row = property.PropertyType == typeof(bool)
                    ? editor
                    : Row(title + ":", editor);
                if (!string.IsNullOrWhiteSpace(description))
                {
                    ToolTip.SetTip(row, description);
                }
                settings.Add(row);
            }

            controls.Add(EnabledCard(_advancedOverride, category.Key, settings.ToArray()));
        }

        return Page("advanced", "Advanced", LucideIcons.sliders_horizontal, controls.ToArray());
    }

    private Control AdvancedEditor(PropertyInfo property)
    {
        if (property.PropertyType == typeof(bool))
        {
            string title = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? SplitPascalCase(property.Name);
            return Check(title, () => (bool)property.GetValue(_settings.AdvancedSettings)!, value => property.SetValue(_settings.AdvancedSettings, value));
        }

        if (property.PropertyType == typeof(int))
        {
            return Number(() => (int)property.GetValue(_settings.AdvancedSettings)!, value => property.SetValue(_settings.AdvancedSettings, (int)value), int.MinValue, int.MaxValue);
        }

        if (property.PropertyType == typeof(string))
        {
            TextBox text = Text(() => (string?)property.GetValue(_settings.AdvancedSettings) ?? string.Empty, value => property.SetValue(_settings.AdvancedSettings, value));
            if (property.Name == nameof(TaskSettingsAdvanced.TextCustom))
            {
                text.AcceptsReturn = true;
                text.TextWrapping = TextWrapping.Wrap;
                text.MinHeight = 80;
            }
            return text;
        }

        if (property.PropertyType == typeof(List<string>))
        {
            return Text(
                () => string.Join(", ", (List<string>?)property.GetValue(_settings.AdvancedSettings) ?? []),
                value => property.SetValue(_settings.AdvancedSettings, value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()));
        }

        return Hint(property.GetValue(_settings.AdvancedSettings)?.ToString() ?? string.Empty);
    }

    private BoundValue<bool> OverrideValue(Func<bool> getter, Action<bool> setter) =>
        _isDefault ? new BoundValue<bool>(true, _ => { }) : new BoundValue<bool>(getter(), setter);

    private Control ParentPage(string id, string title, string icon, BoundValue<bool> overrideValue, string overrideText) =>
        Page(id, title, icon, OverrideCard(overrideValue, overrideText));

    private ScrollViewer Page(string id, string title, string icon, params Control[] controls)
    {
        StackPanel content = new()
        {
            Margin = new Thickness(28, 24, 28, 32),
            MaxWidth = 780,
            HorizontalAlignment = HorizontalAlignment.Left,
            Spacing = 0
        };

        Grid header = new()
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*"),
            ColumnSpacing = 9
        };
        header.Classes.Add("page-title");
        SettingsSearch.SetIsPageTitle(header, true);

        TextBlock iconBlock = new() { Text = icon };
        iconBlock.Classes.Add("icon");
        iconBlock.Classes.Add("page-title-icon");
        TextBlock titleBlock = new() { Text = title };
        titleBlock.Classes.Add("page-title-text");
        Grid.SetColumn(titleBlock, 1);
        header.Children.Add(iconBlock);
        header.Children.Add(titleBlock);
        content.Children.Add(header);

        foreach (Control control in controls)
        {
            content.Children.Add(control);
        }

        ScrollViewer page = new() { Content = content, IsVisible = false };
        SettingsSearch.SetPageId(page, id);
        return page;
    }

    private Border Card(string title, params Control[] controls)
    {
        StackPanel panel = new() { Spacing = 4 };
        if (!string.IsNullOrWhiteSpace(title))
        {
            TextBlock heading = new() { Text = title };
            heading.Classes.Add("section-title");
            panel.Children.Add(heading);
        }

        foreach (Control control in controls)
        {
            panel.Children.Add(control);
        }

        Border card = new() { Child = panel };
        card.Classes.Add("section-card");
        SettingsSearch.SetIsPanel(card, true);
        return card;
    }

    private Border EnabledCard(BoundValue<bool> enabled, string title, params Control[] controls)
    {
        Border card = Card(title, controls);
        BindEnabled(card, enabled);
        return card;
    }

    private Border OverrideCard(BoundValue<bool> value, string text)
    {
        Border card = new()
        {
            Child = Check(text, value)
        };
        card.Classes.Add("override-card");
        SettingsSearch.SetIsPanel(card, true);

        Border availability = new()
        {
            Child = card,
            IsVisible = !_isDefault
        };
        SettingsSearch.SetIsAvailabilityContainer(availability, true);
        return availability;
    }

    private static Grid Row(string label, Control editor)
    {
        Grid row = new()
        {
            ColumnDefinitions = new ColumnDefinitions("210,*"),
            ColumnSpacing = 8
        };
        row.Children.Add(Label(label));
        Grid.SetColumn(editor, 1);
        row.Children.Add(editor);
        return row;
    }

    private static TextBlock Label(string text) => new()
    {
        Text = text,
        FontWeight = FontWeight.Normal,
        VerticalAlignment = VerticalAlignment.Center
    };

    private static TextBlock Hint(string text)
    {
        TextBlock hint = new() { Text = text };
        hint.Classes.Add("hint");
        return hint;
    }

    private static CheckBox Check(string text, Func<bool> getter, Action<bool> setter) =>
        Check(text, new BoundValue<bool>(getter(), setter));

    private static CheckBox Check(string text, BoundValue<bool> value)
    {
        CheckBox checkBox = new() { Content = text, DataContext = value };
        checkBox.Classes.Add("setting");
        checkBox.Bind(ToggleButton.IsCheckedProperty, new Binding(nameof(BoundValue<bool>.Value))
        {
            Source = value,
            Mode = BindingMode.TwoWay
        });
        return checkBox;
    }

    private static TextBox Text(Func<string> getter, Action<string> setter) => Text(new BoundValue<string>(getter(), setter));

    private static TextBox Text(BoundValue<string> value)
    {
        TextBox textBox = new() { DataContext = value };
        textBox.Classes.Add("form-control");
        textBox.Bind(TextBox.TextProperty, new Binding(nameof(BoundValue<string>.Value))
        {
            Source = value,
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        return textBox;
    }

    private static NumericUpDown Number(Func<decimal> getter, Action<decimal> setter, decimal minimum, decimal maximum, decimal increment = 1) =>
        Number(NumericValue(getter(), value => setter(value ?? 0)), minimum, maximum, increment);

    private static NumericUpDown Number(Func<int> getter, Action<decimal> setter, decimal minimum, decimal maximum, decimal increment = 1) =>
        Number(NumericValue(getter(), value => setter(value ?? 0)), minimum, maximum, increment);

    private static NumericUpDown Number(BoundValue<decimal?> value, decimal minimum, decimal maximum, decimal increment = 1)
    {
        NumericUpDown number = new()
        {
            DataContext = value,
            Minimum = minimum,
            Maximum = maximum,
            Increment = increment
        };
        number.Classes.Add("form-control");
        number.Bind(NumericUpDown.ValueProperty, new Binding(nameof(BoundValue<decimal?>.Value))
        {
            Source = value,
            Mode = BindingMode.TwoWay
        });
        return number;
    }

    private static BoundValue<decimal?> NumericValue(decimal initial, Action<decimal?> setter) => new(initial, setter);

    private static ComboBox EnumCombo<T>(Func<T> getter, Action<T> setter) where T : struct, Enum =>
        ObjectCombo(Enum.GetValues<T>(), getter, setter, value => ((Enum)(object)value).GetLocalizedDescription());

    private static ComboBox ObjectCombo<T>(IEnumerable<T> values, Func<T> getter, Action<T> setter, Func<T, string>? title = null)
    {
        ChoiceOption<T>[] options = values.Select(value => new ChoiceOption<T>(value, title?.Invoke(value) ?? value?.ToString() ?? string.Empty)).ToArray();
        T current = getter();
        ChoiceOption<T> selected = options.FirstOrDefault(x => EqualityComparer<T>.Default.Equals(x.Value, current)) ?? options.First();
        BoundValue<ChoiceOption<T>> binding = new(selected, value => setter(value.Value));
        ComboBox comboBox = new()
        {
            ItemsSource = options,
            DataContext = binding
        };
        comboBox.Classes.Add("form-control");
        comboBox.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(BoundValue<ChoiceOption<T>>.Value))
        {
            Source = binding,
            Mode = BindingMode.TwoWay
        });
        return comboBox;
    }

    private static Control FlagsEditor<T>(Func<T> getter, Action<T> setter, IEnumerable<T> values) where T : struct, Enum
    {
        WrapPanel flags = new() { Orientation = Orientation.Horizontal };
        foreach (T value in values)
        {
            ulong mask = Convert.ToUInt64(value);
            BoundValue<bool> selected = new(
                (Convert.ToUInt64(getter()) & mask) == mask,
                isChecked =>
                {
                    ulong current = Convert.ToUInt64(getter());
                    ulong updated = isChecked ? current | mask : current & ~mask;
                    setter((T)Enum.ToObject(typeof(T), updated));
                });
            CheckBox checkBox = Check(((Enum)(object)value).GetLocalizedDescription(), selected);
            checkBox.Margin = new Thickness(0, 1, 12, 1);
            flags.Children.Add(checkBox);
        }

        return new Expander
        {
            Header = "Select tasks",
            Content = flags,
            IsExpanded = false
        };
    }

    private Control SoundPath(string title, Func<bool> enabledGetter, Action<bool> enabledSetter, Func<string> pathGetter, Action<string> pathSetter)
    {
        BoundValue<bool> enabled = new(enabledGetter(), enabledSetter);
        BoundValue<string> path = new(pathGetter(), pathSetter);
        TextBox text = Text(path);
        Button browse = Button("...", async () =>
        {
            string? selected = await PickFileAsync("Choose audio file", "Wave audio", "*.wav");
            if (!string.IsNullOrEmpty(selected))
            {
                path.Value = selected;
            }
        });
        BindEnabled(text, enabled);
        BindEnabled(browse, enabled);

        StackPanel panel = new() { Spacing = 3 };
        panel.Children.Add(Check(title, enabled));
        panel.Children.Add(InputWithButton(text, browse));
        return panel;
    }

    private static Grid InputWithButton(Control input, Button button)
    {
        Grid grid = new()
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 6
        };
        grid.Children.Add(input);
        Grid.SetColumn(button, 1);
        grid.Children.Add(button);
        return grid;
    }

    private static StackPanel ButtonRow(params Button[] buttons)
    {
        StackPanel row = new() { Orientation = Orientation.Horizontal, Spacing = 6 };
        foreach (Button button in buttons)
        {
            row.Children.Add(button);
        }
        return row;
    }

    private static Button Button(string text, Action action)
    {
        Button button = new() { Content = text };
        button.Classes.Add("compact");
        button.Click += (_, _) => action();
        return button;
    }

    private static Button Button(string text, Func<Task> action)
    {
        Button button = new() { Content = text };
        button.Classes.Add("compact");
        button.Click += async (_, _) => await action();
        return button;
    }

    private static void BindEnabled(Control control, BoundValue<bool> value, bool invert = false)
    {
        control.Bind(Control.IsEnabledProperty, new Binding(nameof(BoundValue<bool>.Value))
        {
            Source = value,
            Converter = invert ? InverseBooleanConverter.Instance : null
        });
    }

    private async Task<string?> PickFolderAsync(string title)
    {
        IReadOnlyList<IStorageFolder> folders = await _window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });
        return folders.FirstOrDefault()?.TryGetLocalPath();
    }

    private async Task<string?> PickFileAsync(string title, string typeName, string pattern)
    {
        IReadOnlyList<IStorageFile> files = await _window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType(typeName) { Patterns = [pattern] }]
        });
        return files.FirstOrDefault()?.TryGetLocalPath();
    }

    private static void AddGridLabel(Grid grid, string text, int row, int column) => AddGridControl(grid, Label(text), row, column);

    private static void AddGridControl(Grid grid, Control control, int row, int column)
    {
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        grid.Children.Add(control);
    }

    private static string SplitPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        string result = System.Text.RegularExpressions.Regex.Replace(value, "([A-Z]+)([A-Z][a-z])", "$1 $2");
        return System.Text.RegularExpressions.Regex.Replace(result, "([a-z0-9])([A-Z])", "$1 $2");
    }
}

internal sealed class BoundValue<T> : INotifyPropertyChanged
{
    private T _value;
    private readonly Action<T> _setter;

    public T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
            {
                return;
            }

            _value = value;
            _setter(value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public BoundValue(T value, Action<T> setter)
    {
        _value = value;
        _setter = setter;
    }
}

internal sealed record ChoiceOption<T>(T Value, string Title)
{
    public override string ToString() => Title;
}

internal sealed class InverseBooleanConverter : Avalonia.Data.Converters.IValueConverter
{
    public static InverseBooleanConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => value is bool boolean && !boolean;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => value is bool boolean && !boolean;
}
