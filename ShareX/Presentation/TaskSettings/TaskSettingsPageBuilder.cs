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
using ShareX.Localization;
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
    private ContextMenu? _activeNamePatternMenu;

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

        pages.Add("general", ParentPage("general", Strings.TaskSettingsWindow_General, LucideIcons.settings, _generalOverride, Strings.TaskSettingsWindow_OverrideGeneralSettings));
        pages.Add("general-notifications", BuildNotificationsPage());
        pages.Add("image", BuildImagePage());
        pages.Add("image-effects", BuildImageEffectsPage());
        pages.Add("image-thumbnail", BuildThumbnailPage());
        pages.Add("capture", BuildCapturePage());
        pages.Add("capture-region", BuildRegionCapturePage());
        pages.Add("capture-screen-recorder", BuildScreenRecorderPage());
        pages.Add("capture-ocr", BuildOcrPage());
        pages.Add("upload", ParentPage("upload", Strings.TaskSettingsWindow_Upload, LucideIcons.upload, _uploadOverride, Strings.TaskSettingsWindow_OverrideUploadSettings));
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

        Control afterCapture = TaskFlagsMenu(
            () => _settings.AfterCaptureJob,
            value => _settings.AfterCaptureJob = value,
            MainMenuBuilder.GetAfterCaptureTaskMenuOptions(),
            LucideIcons.image_up);
        BindEnabled(afterCapture, afterCaptureOverride);

        Control afterUpload = TaskFlagsMenu(
            () => _settings.AfterUploadJob,
            value => _settings.AfterUploadJob = value,
            MainMenuBuilder.GetAfterUploadTaskMenuOptions(),
            LucideIcons.cloud_upload);
        BindEnabled(afterUpload, afterUploadOverride);

        Control destinations = DestinationMenu(_settings);
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
            accountControls.Add(Check(Strings.TaskSettingsWindow_OverrideDefaultFTPAccount, ftpOverride));
            accountControls.Add(Row(Strings.TaskSettingsWindow_FTPAccount, ftp));
        }

        if (Program.UploadersConfig?.CustomUploadersList.Count > 0)
        {
            BoundValue<bool> customOverride = new(_settings.OverrideCustomUploader, value => _settings.OverrideCustomUploader = value);
            ComboBox custom = ObjectCombo(
                Program.UploadersConfig.CustomUploadersList,
                () => Program.UploadersConfig.CustomUploadersList[_settings.CustomUploaderIndex.BetweenOrDefault(0, Program.UploadersConfig.CustomUploadersList.Count - 1)],
                value => _settings.CustomUploaderIndex = Program.UploadersConfig.CustomUploadersList.IndexOf(value));
            BindEnabled(custom, customOverride);
            accountControls.Add(Check(Strings.TaskSettingsWindow_OverrideDefaultCustomUploader, customOverride));
            accountControls.Add(Row(Strings.TaskSettingsWindow_CustomUploader, custom));
        }

        BoundValue<bool> folderOverride = new(_settings.OverrideScreenshotsFolder, value => _settings.OverrideScreenshotsFolder = value);
        TextBox folderText = Text(() => _settings.ScreenshotsFolder, value => _settings.ScreenshotsFolder = value);
        Button browseFolder = Button(Strings.TaskSettingsWindow_BrowseWithEllipsis, async () =>
        {
            string? path = await PickFolderAsync(Strings.TaskSettingsWindow_ChooseScreenshotsFolder);
            if (!string.IsNullOrEmpty(path))
            {
                ((BoundValue<string>)folderText.DataContext!).Value = path;
            }
        });
        BindEnabled(folderText, folderOverride);
        BindEnabled(browseFolder, folderOverride);
        Grid folderRow = InputWithButton(folderText, browseFolder);

        return Page("task", Strings.TaskSettingsWindow_Task, LucideIcons.keyboard,
            Card(Strings.TaskSettingsWindow_Task, Row(Strings.TaskSettingsWindow_TaskLabel, TaskMenu(() => _settings.Job, value => _settings.Job = value)),
                Row(Strings.TaskSettingsWindow_Description, Text(() => _settings.Description, value => _settings.Description = value))),
            Card(Strings.TaskSettingsWindow_AfterCaptureTasks, Check(Strings.TaskSettingsWindow_OverrideAfterCaptureTasks, afterCaptureOverride), afterCapture),
            Card(Strings.TaskSettingsWindow_AfterUploadTasks, Check(Strings.TaskSettingsWindow_OverrideAfterUploadTasks, afterUploadOverride), afterUpload),
            Card(Strings.TaskSettingsWindow_Destinations, Check(Strings.TaskSettingsWindow_OverrideDestinations, destinationsOverride), destinations),
            Card(Strings.TaskSettingsWindow_UploaderAccounts, accountControls.ToArray()),
            Card(Strings.TaskSettingsWindow_ScreenshotsFolder, Check(Strings.TaskSettingsWindow_OverrideScreenshotsFolder, folderOverride), folderRow));
    }

    private Control BuildNotificationsPage()
    {
        TaskSettingsGeneral general = _settings.GeneralSettings;
        BoundValue<bool> showToast = new(general.ShowToastNotificationAfterTaskCompleted, value => general.ShowToastNotificationAfterTaskCompleted = value);

        StackPanel toastOptions = new() { Spacing = 4 };
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_DurationSeconds, Number(() => (decimal)general.ToastWindowDuration, value => general.ToastWindowDuration = (float)value, 0, 60, 0.1m)));
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_FadeDurationSeconds, Number(() => (decimal)general.ToastWindowFadeDuration, value => general.ToastWindowFadeDuration = (float)value, 0, 10, 0.1m)));
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_Placement, EnumCombo(() => general.ToastWindowPlacement, value => general.ToastWindowPlacement = value)));
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_Width, Number(() => general.ToastWindowSize.Width, value => general.ToastWindowSize = new DrawingSize((int)value, general.ToastWindowSize.Height), 100, 2000)));
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_Height, Number(() => general.ToastWindowSize.Height, value => general.ToastWindowSize = new DrawingSize(general.ToastWindowSize.Width, (int)value), 50, 2000)));
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_LeftClickAction, EnumCombo(() => general.ToastWindowLeftClickAction, value => general.ToastWindowLeftClickAction = value)));
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_RightClickAction, EnumCombo(() => general.ToastWindowRightClickAction, value => general.ToastWindowRightClickAction = value)));
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_MiddleClickAction, EnumCombo(() => general.ToastWindowMiddleClickAction, value => general.ToastWindowMiddleClickAction = value)));
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_NotificationButtonSize, Number(() => general.ToastWindowButtonSize,
            value => general.ToastWindowButtonSize = (int)value, 16, 128)));
        toastOptions.Children.Add(Row(Strings.TaskSettingsWindow_NotificationButtonsLabel, Button(Strings.TaskSettingsWindow_ConfigureWithEllipsis, () =>
            _window.ShowNotificationButtonsEditor(general.ToastWindowButtons, buttons => general.ToastWindowButtons = buttons))));
        toastOptions.Children.Add(Check(Strings.TaskSettingsWindow_AutomaticallyHideOnScreenCapture, () => general.ToastWindowAutoHide, value => general.ToastWindowAutoHide = value));
        toastOptions.Children.Add(Check(Strings.TaskSettingsWindow_DisableToastNotificationsOnFullscreen, () => general.DisableNotificationsOnFullscreen, value => general.DisableNotificationsOnFullscreen = value));
        BindEnabled(toastOptions, showToast);

        return Page("general-notifications", Strings.TaskSettingsWindow_Notifications, LucideIcons.bell,
            EnabledCard(_generalOverride, Strings.TaskSettingsWindow_Sounds,
                Check(Strings.TaskSettingsWindow_PlaySoundAfterCaptureIsMade, () => general.PlaySoundAfterCapture, value => general.PlaySoundAfterCapture = value),
                Check(Strings.TaskSettingsWindow_PlaySoundAfterTaskIsCompleted, () => general.PlaySoundAfterUpload, value => general.PlaySoundAfterUpload = value),
                Check(Strings.TaskSettingsWindow_PlaySoundAfterActionIsCompleted, () => general.PlaySoundAfterAction, value => general.PlaySoundAfterAction = value)),
            EnabledCard(_generalOverride, Strings.TaskSettingsWindow_ToastNotification,
                Check(Strings.TaskSettingsWindow_ShowToastNotificationAfterTaskIsCompleted, showToast), toastOptions),
            EnabledCard(_generalOverride, Strings.TaskSettingsWindow_CustomSounds,
                SoundPath(Strings.TaskSettingsWindow_UseCustomCaptureSound, () => general.UseCustomCaptureSound, value => general.UseCustomCaptureSound = value,
                    () => general.CustomCaptureSoundPath, value => general.CustomCaptureSoundPath = value),
                SoundPath(Strings.TaskSettingsWindow_UseCustomTaskCompletedSound, () => general.UseCustomTaskCompletedSound, value => general.UseCustomTaskCompletedSound = value,
                    () => general.CustomTaskCompletedSoundPath, value => general.CustomTaskCompletedSoundPath = value),
                SoundPath(Strings.TaskSettingsWindow_UseCustomActionCompletedSound, () => general.UseCustomActionCompletedSound, value => general.UseCustomActionCompletedSound = value,
                    () => general.CustomActionCompletedSoundPath, value => general.CustomActionCompletedSoundPath = value),
                SoundPath(Strings.TaskSettingsWindow_UseCustomErrorSound, () => general.UseCustomErrorSound, value => general.UseCustomErrorSound = value,
                    () => general.CustomErrorSoundPath, value => general.CustomErrorSoundPath = value)));
    }

    private Control BuildImagePage()
    {
        TaskSettingsImage image = _settings.ImageSettings;
        BoundValue<bool> autoJpeg = new(image.ImageAutoUseJPEG, value => image.ImageAutoUseJPEG = value);
        NumericUpDown autoJpegSize = Number(() => image.ImageAutoUseJPEGSize, value => image.ImageAutoUseJPEGSize = (int)value, 0, int.MaxValue);
        CheckBox autoQuality = Check(Strings.TaskSettingsWindow_AdjustJPEGQualityAutomaticallyToKeepImageSizeNearTheSpecifiedSize, () => image.ImageAutoJPEGQuality, value => image.ImageAutoJPEGQuality = value);
        BindEnabled(autoJpegSize, autoJpeg);
        BindEnabled(autoQuality, autoJpeg);

        return Page("image", Strings.TaskSettingsWindow_Image, LucideIcons.image,
            OverrideCard(_imageOverride, Strings.TaskSettingsWindow_OverrideImageSettings),
            EnabledCard(_imageOverride, Strings.TaskSettingsWindow_ImageQuality,
                Row(Strings.TaskSettingsWindow_ImageFormat, EnumCombo(() => image.ImageFormat, value => image.ImageFormat = value)),
                Row(Strings.TaskSettingsWindow_PNGBitDepth, EnumCombo(() => image.ImagePNGBitDepth, value => image.ImagePNGBitDepth = value)),
                Row(Strings.TaskSettingsWindow_JPEGQuality, Number(() => image.ImageJPEGQuality, value => image.ImageJPEGQuality = (int)value, 0, 100)),
                Row(Strings.TaskSettingsWindow_GIFQuality, EnumCombo(() => image.ImageGIFQuality, value => image.ImageGIFQuality = value)),
                Row(Strings.TaskSettingsWindow_IfFileExists, EnumCombo(() => image.FileExistAction, value => image.FileExistAction = value))),
            EnabledCard(_imageOverride, Strings.TaskSettingsWindow_AutomaticJPEG,
                Check(Strings.TaskSettingsWindow_UseJPEGIfImageSizeIsBiggerThanTheSpecifiedSize, autoJpeg),
                Row(Strings.TaskSettingsWindow_SizeLimitKB, autoJpegSize), autoQuality));
    }

    private Control BuildImageEffectsPage()
    {
        TaskSettingsImage image = _settings.ImageSettings;
        return Page("image-effects", Strings.TaskSettingsWindow_Effects, LucideIcons.wand_sparkles,
            EnabledCard(_imageOverride, Strings.TaskSettingsWindow_ImageEffects,
                Check(Strings.TaskSettingsWindow_ShowImageEffectsWindowAfterCapture, () => image.ShowImageEffectsWindowAfterCapture, value => image.ShowImageEffectsWindowAfterCapture = value),
                Check(Strings.TaskSettingsWindow_OnlyApplyEffectsToRegionCapture, () => image.ImageEffectOnlyRegionCapture, value => image.ImageEffectOnlyRegionCapture = value),
                Check(Strings.TaskSettingsWindow_UseRandomImageEffect, () => image.UseRandomImageEffect, value => image.UseRandomImageEffect = value),
                Button(Strings.TaskSettingsWindow_ImageEffectsConfigurationWithEllipsis, () => TaskHelpers.OpenImageEffectsSingleton(_settings)),
                Hint(Strings.TaskSettingsWindow_YouCanEnableOrDisableImageEffectsFromAfterCaptureTasksAddImageEffects)));
    }

    private Control BuildThumbnailPage()
    {
        TaskSettingsImage image = _settings.ImageSettings;
        BoundValue<string> name = new(image.ThumbnailName, value => image.ThumbnailName = value);
        TextBlock preview = Hint(Strings.TaskSettingsWindow_ImageNameSample + image.ThumbnailName + ".jpg");
        name.PropertyChanged += (_, _) => preview.Text = Strings.TaskSettingsWindow_ImageNameSample + name.Value + ".jpg";

        return Page("image-thumbnail", Strings.TaskSettingsWindow_Thumbnail, LucideIcons.images,
            EnabledCard(_imageOverride, Strings.TaskSettingsWindow_Thumbnail,
                Row(Strings.TaskSettingsWindow_Width, Number(() => image.ThumbnailWidth, value => image.ThumbnailWidth = (int)value, 0, 10000)),
                Row(Strings.TaskSettingsWindow_Height, Number(() => image.ThumbnailHeight, value => image.ThumbnailHeight = (int)value, 0, 10000)),
                Row(Strings.TaskSettingsWindow_ThumbnailName, Text(name)),
                Row(Strings.TaskSettingsWindow_Preview, preview),
                Check(Strings.TaskSettingsWindow_CreateThumbnailOnlyIfImageIsBiggerThanThumbnailSize, () => image.ThumbnailCheckSize, value => image.ThumbnailCheckSize = value)));
    }

    private Control BuildCapturePage()
    {
        TaskSettingsCapture capture = _settings.CaptureSettings;
        BoundValue<bool> transparent = new(capture.CaptureTransparent, value => capture.CaptureTransparent = value);
        CheckBox shadow = Check(Strings.TaskSettingsWindow_CaptureWindowWithShadow, () => capture.CaptureShadow, value => capture.CaptureShadow = value);
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

        Button selectRegion = Button(Strings.TaskSettingsWindow_SelectRegionWithEllipsis, () =>
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
        AddGridLabel(regionGrid, Strings.TaskSettingsWindow_X, 0, 0);
        AddGridControl(regionGrid, Number(regionX, -100000, 100000), 0, 1);
        AddGridLabel(regionGrid, Strings.TaskSettingsWindow_Y, 0, 2);
        AddGridControl(regionGrid, Number(regionY, -100000, 100000), 0, 3);
        AddGridLabel(regionGrid, Strings.TaskSettingsWindow_Width, 1, 0);
        AddGridControl(regionGrid, Number(regionWidth, 0, 100000), 1, 1);
        AddGridLabel(regionGrid, Strings.TaskSettingsWindow_Height, 1, 2);
        AddGridControl(regionGrid, Number(regionHeight, 0, 100000), 1, 3);

        return Page("capture", Strings.TaskSettingsWindow_Capture, LucideIcons.camera,
            OverrideCard(_captureOverride, Strings.TaskSettingsWindow_OverrideCaptureSettings),
            EnabledCard(_captureOverride, Strings.TaskSettingsWindow_Screenshots,
                Check(Strings.TaskSettingsWindow_ShowCursorInScreenshots, () => capture.ShowCursor, value => capture.ShowCursor = value),
                Row(Strings.TaskSettingsWindow_ScreenshotDelaySeconds, Number(() => capture.ScreenshotDelay, value => capture.ScreenshotDelay = value, 0, 60, 0.1m)),
                Check(Strings.TaskSettingsWindow_CaptureWindowWithTransparency, transparent), shadow,
                Row(Strings.TaskSettingsWindow_ShadowOffset, shadowOffset),
                Check(Strings.TaskSettingsWindow_CaptureClientAreaForWindowCaptures, () => capture.CaptureClientArea, value => capture.CaptureClientArea = value),
                Check(Strings.TaskSettingsWindow_HideTaskbarWhenItIntersectsACapturedWindow, () => capture.CaptureAutoHideTaskbar, value => capture.CaptureAutoHideTaskbar = value),
                Check(Strings.TaskSettingsWindow_AutomaticallyHideDesktopIcons, () => capture.CaptureAutoHideDesktopIcons, value => capture.CaptureAutoHideDesktopIcons = value),
                Check(Strings.TaskSettingsWindow_HDRScreenshotColorCorrector, () => capture.HDRScreenshotColorCorrection, value => capture.HDRScreenshotColorCorrection = value)),
            EnabledCard(_captureOverride, Strings.TaskSettingsWindow_PreconfiguredRegion, regionGrid, selectRegion),
            EnabledCard(_captureOverride, Strings.TaskSettingsWindow_PreconfiguredWindow,
                Row(Strings.TaskSettingsWindow_WindowTitle, Text(() => capture.CaptureCustomWindow, value => capture.CaptureCustomWindow = value))));
    }

    private Control BuildRegionCapturePage()
    {
        RegionCaptureOptions options = _settings.CaptureSettings.SurfaceOptions;
        BoundValue<bool> detectWindows = new(options.DetectWindows, value => options.DetectWindows = value);
        CheckBox detectControls = Check(Strings.TaskSettingsWindow_AlsoDetectControlsInsideWindows, () => options.DetectControls, value => options.DetectControls = value);
        BindEnabled(detectControls, detectWindows);

        BoundValue<bool> customInfo = new(options.UseCustomInfoText, value => options.UseCustomInfoText = value);
        TextBox customInfoText = Text(() => options.CustomInfoText, value => options.CustomInfoText = value.Replace("\r\n", "$n").Replace("\n", "$n"));
        BindEnabled(customInfoText, customInfo);

        BoundValue<bool> magnifier = new(options.ShowMagnifier, value => options.ShowMagnifier = value);
        CheckBox squareMagnifier = Check(Strings.TaskSettingsWindow_UseSquareMagnifier, () => options.UseSquareMagnifier, value => options.UseSquareMagnifier = value);
        NumericUpDown pixelCount = Number(() => options.MagnifierPixelCount, value => options.MagnifierPixelCount = (int)value,
            RegionCaptureOptions.MagnifierPixelCountMinimum, RegionCaptureOptions.MagnifierPixelCountMaximum);
        NumericUpDown pixelSize = Number(() => options.MagnifierPixelSize, value => options.MagnifierPixelSize = (int)value,
            RegionCaptureOptions.MagnifierPixelSizeMinimum, RegionCaptureOptions.MagnifierPixelSizeMaximum);
        BindEnabled(squareMagnifier, magnifier);
        BindEnabled(pixelCount, magnifier);
        BindEnabled(pixelSize, magnifier);

        return Page("capture-region", Strings.TaskSettingsWindow_RegionCapture, LucideIcons.crop,
            EnabledCard(_captureOverride, Strings.TaskSettingsWindow_Selection,
                Check(Strings.TaskSettingsWindow_UseMultiRegionMode, () => !options.QuickCrop, value => options.QuickCrop = !value),
                Check(Strings.TaskSettingsWindow_DetectWindowRegions, detectWindows), detectControls,
                Check(Strings.TaskSettingsWindow_RestrictCaptureAndCursorToTheActiveMonitor, () => options.ActiveMonitorMode, value => options.ActiveMonitorMode = value),
                Row(Strings.TaskSettingsWindow_BackgroundDimStrengthPercent, Number(() => options.BackgroundDimStrength, value => options.BackgroundDimStrength = (int)value, 0, 100))),
            EnabledCard(_captureOverride, Strings.TaskSettingsWindow_MouseActions,
                Row(Strings.TaskSettingsWindow_RightClick, EnumCombo(() => options.RegionCaptureActionRightClick, value => options.RegionCaptureActionRightClick = value)),
                Row(Strings.TaskSettingsWindow_MiddleClick, EnumCombo(() => options.RegionCaptureActionMiddleClick, value => options.RegionCaptureActionMiddleClick = value)),
                Row(Strings.TaskSettingsWindow_Mouse4Click, EnumCombo(() => options.RegionCaptureActionX1Click, value => options.RegionCaptureActionX1Click = value)),
                Row(Strings.TaskSettingsWindow_Mouse5Click, EnumCombo(() => options.RegionCaptureActionX2Click, value => options.RegionCaptureActionX2Click = value))),
            EnabledCard(_captureOverride, Strings.TaskSettingsWindow_InformationAndMagnifier,
                Check(Strings.TaskSettingsWindow_ShowPositionAndSizeInfo, () => options.ShowInfo, value => options.ShowInfo = value),
                Check(Strings.TaskSettingsWindow_UseCustomInfoText, customInfo), customInfoText,
                Check(Strings.TaskSettingsWindow_ShowMagnifierNearCursor, magnifier), squareMagnifier,
                Row(Strings.TaskSettingsWindow_MagnifierPixelCount, pixelCount),
                Row(Strings.TaskSettingsWindow_MagnifierPixelSize, pixelSize),
                Check(Strings.TaskSettingsWindow_ShowCenterCrosshair, () => options.ShowCenterCrosshair, value => options.ShowCenterCrosshair = value),
                Check(Strings.TaskSettingsWindow_ShowScreenWideCrosshair, () => options.ShowCrosshair, value => options.ShowCrosshair = value)));
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

        return Page("capture-screen-recorder", Strings.TaskSettingsWindow_ScreenRecorder, LucideIcons.video,
            EnabledCard(_captureOverride, Strings.TaskSettingsWindow_Recording,
                Row(Strings.TaskSettingsWindow_ScreenRecordingFPS, Number(() => capture.ScreenRecordFPS, value => capture.ScreenRecordFPS = (int)value, 1, HelpersOptions.DevMode ? 300 : 60)),
                Row(Strings.TaskSettingsWindow_GIFFPS, Number(() => capture.GIFFPS, value => capture.GIFFPS = (int)value, 1, HelpersOptions.DevMode ? 60 : 30)),
                Check(Strings.TaskSettingsWindow_ShowCursorInRecording, () => capture.ScreenRecordShowCursor, value => capture.ScreenRecordShowCursor = value),
                Check(Strings.TaskSettingsWindow_ShowRecordingTimer, () => capture.ScreenRecordShowTimer, value => capture.ScreenRecordShowTimer = value),
                Check(Strings.TaskSettingsWindow_ShowRecordingButtonLabels, () => capture.ScreenRecordShowButtonLabels, value => capture.ScreenRecordShowButtonLabels = value),
                Check(Strings.TaskSettingsWindow_StartRecordingAfterADelay, autoStart), Row(Strings.TaskSettingsWindow_StartDelaySeconds, startDelay),
                Check(Strings.TaskSettingsWindow_UseFixedDuration, fixedDuration), Row(Strings.TaskSettingsWindow_DurationSeconds, duration)),
            EnabledCard(_captureOverride, Strings.TaskSettingsWindow_EncodingAndCapture,
                Check(Strings.TaskSettingsWindow_RecordLosslesslyFirstThenApplyEncodingOptions, () => capture.ScreenRecordTwoPassEncoding, value => capture.ScreenRecordTwoPassEncoding = value),
                Check(Strings.TaskSettingsWindow_AskForConfirmationWhenAborting, () => capture.ScreenRecordAskConfirmationOnAbort, value => capture.ScreenRecordAskConfirmationOnAbort = value),
                Check(Strings.TaskSettingsWindow_UseTransparentRegionSelection, () => capture.ScreenRecordTransparentRegion, value => capture.ScreenRecordTransparentRegion = value),
                Button(Strings.TaskSettingsWindow_ScreenRecordingOptionsWithEllipsis, ShowScreenRecordingOptions)));
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
            language = new ComboBox { IsEnabled = false, PlaceholderText = Strings.TaskSettingsWindow_OCRLanguagesAreUnavailable };
            language.Classes.Add("form-control");
        }

        BoundValue<bool> silent = new(options.Silent, value => options.Silent = value);
        CheckBox autoCopy = Check(Strings.TaskSettingsWindow_AutomaticallyCopyResultsToClipboard, () => options.AutoCopy, value => options.AutoCopy = value);
        BindEnabled(autoCopy, silent, invert: true);

        return Page("capture-ocr", Strings.TaskSettingsWindow_OCR, LucideIcons.scan_text,
            EnabledCard(_captureOverride, Strings.TaskSettingsWindow_OpticalCharacterRecognition,
                Row(Strings.TaskSettingsWindow_DefaultLanguage, language),
                Check(Strings.TaskSettingsWindow_ProcessOCRSilently, silent), autoCopy,
                Check(Strings.TaskSettingsWindow_CloseOCRWindowAfterOpeningServiceLink, () => options.CloseWindowAfterOpeningServiceLink, value => options.CloseWindowAfterOpeningServiceLink = value),
                Button(Strings.TaskSettingsWindow_OCRHelp, () => URLHelpers.OpenURL(Links.DocsOCR))));
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
        TextBox capturePatternTextBox = NamePatternText(capturePattern, CodeMenuEntryFilename.n, CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn);
        TextBox windowPatternTextBox = NamePatternText(windowPattern, CodeMenuEntryFilename.n);
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

        return Page("upload-file-naming", Strings.TaskSettingsWindow_FileNaming, LucideIcons.file_pen,
            EnabledCard(_uploadOverride, Strings.TaskSettingsWindow_NamePatterns,
                Row(Strings.TaskSettingsWindow_CaptureOrClipboardUpload, capturePatternTextBox), Row(Strings.TaskSettingsWindow_Preview, capturePreview),
                Row(Strings.TaskSettingsWindow_WindowCapture, windowPatternTextBox), Row(Strings.TaskSettingsWindow_Preview, windowPreview),
                Check(Strings.TaskSettingsWindow_UseNamePatternForFileUploads, () => upload.FileUploadUseNamePattern, value => upload.FileUploadUseNamePattern = value),
                Check(Strings.TaskSettingsWindow_ReplaceURLProblematicCharactersWithUnderscores, () => upload.FileUploadReplaceProblematicCharacters, value => upload.FileUploadReplaceProblematicCharacters = value),
                Row(Strings.TaskSettingsWindow_AutoIncrementNumber, autoIncrement)),
            EnabledCard(_uploadOverride, Strings.TaskSettingsWindow_TimeZone,
                Check(Strings.TaskSettingsWindow_UseCustomTimeZone, customTimeZone), Row(Strings.TaskSettingsWindow_TimeZoneLabel, timeZone)),
            EnabledCard(_uploadOverride, Strings.TaskSettingsWindow_URLReplacement,
                Check(Strings.TaskSettingsWindow_ReplaceResultURLUsingARegularExpression, regexReplace),
                Row(Strings.TaskSettingsWindow_Pattern, regexPattern), Row(Strings.TaskSettingsWindow_Replacement, regexReplacement)));
    }

    private Control BuildClipboardUploadPage()
    {
        TaskSettingsUpload upload = _settings.UploadSettings;
        return Page("upload-clipboard", Strings.TaskSettingsWindow_ClipboardUpload, LucideIcons.clipboard,
            EnabledCard(_uploadOverride, Strings.TaskSettingsWindow_ClipboardContent,
                Check(Strings.TaskSettingsWindow_IfClipboardContainsAFileURLDownloadAndUploadIt, () => upload.ClipboardUploadURLContents, value => upload.ClipboardUploadURLContents = value),
                Check(Strings.TaskSettingsWindow_IfClipboardContainsAURLUseURLShortener, () => upload.ClipboardUploadShortenURL, value => upload.ClipboardUploadShortenURL = value),
                Check(Strings.TaskSettingsWindow_IfClipboardContainsAURLShareItUsingURLSharingService, () => upload.ClipboardUploadShareURL, value => upload.ClipboardUploadShareURL = value),
                Check(Strings.TaskSettingsWindow_IfClipboardContainsAFolderPathIndexAndUploadIt, () => upload.ClipboardUploadAutoIndexFolder, value => upload.ClipboardUploadAutoIndexFolder = value)));
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

        Button add = Button(Strings.TaskSettingsWindow_Add, () =>
        {
            if (CreateFilter() is { } filter)
            {
                upload.UploaderFilters.Add(filter);
                rows.Add(FilterTitle(filter));
                list.SelectedIndex = rows.Count - 1;
            }
        });
        Button update = Button(Strings.TaskSettingsWindow_Update, () =>
        {
            int index = list.SelectedIndex;
            if (index >= 0 && CreateFilter() is { } filter)
            {
                upload.UploaderFilters[index] = filter;
                rows[index] = FilterTitle(filter);
            }
        });
        Button remove = Button(Strings.TaskSettingsWindow_Remove, () =>
        {
            int index = list.SelectedIndex;
            if (index >= 0)
            {
                upload.UploaderFilters.RemoveAt(index);
                rows.RemoveAt(index);
            }
        });

        return Page("upload-filters", Strings.TaskSettingsWindow_UploaderFilters, LucideIcons.filter,
            EnabledCard(_uploadOverride, Strings.TaskSettingsWindow_Filters, list,
                Row(Strings.TaskSettingsWindow_Uploader, uploader),
                Row(Strings.TaskSettingsWindow_Extensions, extensions),
                Hint(Strings.TaskSettingsWindow_SeparateExtensionsWithCommasForExamplePngJpgJpeg),
                ButtonRow(add, update, remove)));
    }

    private static string FilterTitle(UploaderFilter filter) => $"{filter.Uploader} — {filter.GetExtensions()}";

    private Control BuildToolsPage()
    {
        TaskSettingsTools tools = _settings.ToolsSettings;
        var picker = tools.ScreenColorPickerOptions;
        return Page("tools", Strings.TaskSettingsWindow_Tools, LucideIcons.wrench,
            OverrideCard(_toolsOverride, Strings.TaskSettingsWindow_OverrideToolsSettings),
            EnabledCard(_toolsOverride, Strings.TaskSettingsWindow_ImageEditor,
                Check(Strings.TaskSettingsWindow_UseLegacyImageEditor, () => tools.UseLegacyImageEditor, value => tools.UseLegacyImageEditor = value)),
            EnabledCard(_toolsOverride, Strings.TaskSettingsWindow_ScreenColorPicker,
                Row(Strings.TaskSettingsWindow_Format, Text(() => picker.Format, value => picker.Format = value)),
                Row(Strings.TaskSettingsWindow_FormatCtrlPlusClick, Text(() => picker.FormatCtrl, value => picker.FormatCtrl = value)),
                Row(Strings.TaskSettingsWindow_InfoText, Text(() => picker.InfoText, value => picker.InfoText = value)),
                Check(Strings.TaskSettingsWindow_ShowMagnifier, () => picker.ShowMagnifier, value => picker.ShowMagnifier = value)));
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
                ToolTip.SetTip(enabled, Strings.TaskSettingsWindow_EnableAction);

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

        Button add = Button(Strings.TaskSettingsWindow_AddWithEllipsis, () =>
        {
            _window.ShowActionEditor(null, action =>
            {
                _externalPrograms.Add(action);
                Refresh(action);
            });
        });
        Button edit = Button(Strings.TaskSettingsWindow_EditWithEllipsis, () =>
        {
            if (Selected() is { } action)
            {
                _window.ShowActionEditor(action, editedAction =>
                {
                    Refresh(editedAction);
                });
            }
        });
        Button duplicate = Button(Strings.TaskSettingsWindow_Duplicate, () =>
        {
            if (Selected() is { } action)
            {
                ExternalProgram copy = action.Copy();
                _externalPrograms.Add(copy);
                Refresh(copy);
            }
        });
        Button remove = Button(Strings.TaskSettingsWindow_Remove, () =>
        {
            if (Selected() is { } action)
            {
                _externalPrograms.Remove(action);
                Refresh();
            }
        });

        return Page("actions", Strings.TaskSettingsWindow_Actions, LucideIcons.zap,
            OverrideCard(_actionsOverride, Strings.TaskSettingsWindow_OverrideActions),
            EnabledCard(_actionsOverride, Strings.TaskSettingsWindow_Actions, list, ButtonRow(add, edit, duplicate, remove),
                Hint(Strings.TaskSettingsWindow_YouCanEnableOrDisableActionsFromAfterCaptureTasksPerformActions)));
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

        Button add = Button(Strings.TaskSettingsWindow_AddWithEllipsis, () =>
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
        Button edit = Button(Strings.TaskSettingsWindow_EditWithEllipsis, () =>
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
        Button remove = Button(Strings.TaskSettingsWindow_Remove, () =>
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

        return Page("watch-folders", Strings.TaskSettingsWindow_WatchFolders, LucideIcons.folder_search,
            Card(Strings.TaskSettingsWindow_WatchFolders,
                Check(Strings.TaskSettingsWindow_WatchFoldersAndUploadNewlyCreatedFiles, enabled),
                list, ButtonRow(add, edit, remove)));
    }

    private static string WatchFolderTitle(WatchFolderSettings folder) => string.Format(
        Strings.TaskSettingsWindow_WatchFolderSummary,
        folder.FolderPath,
        folder.Filter,
        folder.IncludeSubdirectories ? Strings.TaskSettingsWindow_Yes : Strings.TaskSettingsWindow_No);

    private Control BuildAdvancedPage()
    {
        List<Control> controls = [];
        if (!_isDefault)
        {
            controls.Add(OverrideCard(_advancedOverride, Strings.TaskSettingsWindow_OverrideAdvancedSettings));
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
                string title = GetAdvancedSettingTitle(property);

                Control row = property.PropertyType == typeof(bool)
                    ? editor
                    : Row(title + ":", editor);
                settings.Add(row);
            }

            controls.Add(EnabledCard(_advancedOverride, GetAdvancedCategoryTitle(category.Key), settings.ToArray()));
        }

        return Page("advanced", Strings.TaskSettingsWindow_Advanced, LucideIcons.sliders_horizontal, controls.ToArray());
    }

    private Control AdvancedEditor(PropertyInfo property)
    {
        if (property.PropertyType == typeof(bool))
        {
            string title = GetAdvancedSettingTitle(property);
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

    private static string GetAdvancedCategoryTitle(string category) => category switch
    {
        "General" => Strings.TaskSettingsWindow_General,
        "Capture" => Strings.TaskSettingsWindow_Capture,
        "Upload" => Strings.TaskSettingsWindow_Upload,
        "Upload text" => Strings.TaskSettingsWindow_UploadText,
        "After upload" => Strings.TaskSettingsWindow_AfterUpload,
        "Name pattern" => Strings.TaskSettingsWindow_NamePattern,
        _ => category
    };

    private static string GetAdvancedSettingTitle(PropertyInfo property) => property.Name switch
    {
        nameof(TaskSettingsAdvanced.ProcessImagesDuringFileUpload) => Strings.TaskSettingsWindow_ProcessImagesDuringFileUpload,
        nameof(TaskSettingsAdvanced.ProcessImagesDuringClipboardUpload) => Strings.TaskSettingsWindow_ProcessImagesDuringClipboardUpload,
        nameof(TaskSettingsAdvanced.ProcessImagesDuringExtensionUpload) => Strings.TaskSettingsWindow_ProcessImagesDuringExtensionUpload,
        nameof(TaskSettingsAdvanced.UseAfterCaptureTasksDuringFileUpload) => Strings.TaskSettingsWindow_UseAfterCaptureTasksDuringFileUpload,
        nameof(TaskSettingsAdvanced.TextTaskSaveAsFile) => Strings.TaskSettingsWindow_TextTaskSaveAsFile,
        nameof(TaskSettingsAdvanced.AutoClearClipboard) => Strings.TaskSettingsWindow_AutoClearClipboard,
        nameof(TaskSettingsAdvanced.RegionCaptureDisableAnnotation) => Strings.TaskSettingsWindow_RegionCaptureDisableAnnotation,
        nameof(TaskSettingsAdvanced.ImageExtensions) => Strings.TaskSettingsWindow_ImageExtensions,
        nameof(TaskSettingsAdvanced.TextExtensions) => Strings.TaskSettingsWindow_TextExtensions,
        nameof(TaskSettingsAdvanced.EarlyCopyURL) => Strings.TaskSettingsWindow_EarlyCopyURL,
        nameof(TaskSettingsAdvanced.TextFileExtension) => Strings.TaskSettingsWindow_TextFileExtension,
        nameof(TaskSettingsAdvanced.TextFormat) => Strings.TaskSettingsWindow_TextFormat,
        nameof(TaskSettingsAdvanced.TextCustom) => Strings.TaskSettingsWindow_TextCustom,
        nameof(TaskSettingsAdvanced.TextCustomEncodeInput) => Strings.TaskSettingsWindow_TextCustomEncodeInput,
        nameof(TaskSettingsAdvanced.ResultForceHTTPS) => Strings.TaskSettingsWindow_ResultForceHTTPS,
        nameof(TaskSettingsAdvanced.ClipboardContentFormat) => Strings.TaskSettingsWindow_ClipboardContentFormat,
        nameof(TaskSettingsAdvanced.BalloonTipContentFormat) => Strings.TaskSettingsWindow_BalloonTipContentFormat,
        nameof(TaskSettingsAdvanced.OpenURLFormat) => Strings.TaskSettingsWindow_OpenURLFormat,
        nameof(TaskSettingsAdvanced.AutoShortenURLLength) => Strings.TaskSettingsWindow_AutoShortenURLLength,
        nameof(TaskSettingsAdvanced.AutoCloseAfterUploadForm) => Strings.TaskSettingsWindow_AutoCloseAfterUploadForm,
        nameof(TaskSettingsAdvanced.NamePatternMaxLength) => Strings.TaskSettingsWindow_NamePatternMaxLength,
        nameof(TaskSettingsAdvanced.NamePatternMaxTitleLength) => Strings.TaskSettingsWindow_NamePatternMaxTitleLength,
        _ => property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? SplitPascalCase(property.Name)
    };

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

    private TextBox NamePatternText(BoundValue<string> value, params CodeMenuEntryFilename[] ignoredEntries)
    {
        TextBox textBox = Text(value);
        HashSet<CodeMenuEntryFilename> ignored = ignoredEntries.ToHashSet();
        IEnumerable<CodeMenuEntryFilename> entries = typeof(CodeMenuEntryFilename)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(CodeMenuEntryFilename))
            .Select(field => field.GetValue(null))
            .OfType<CodeMenuEntryFilename>()
            .Where(entry => !ignored.Contains(entry));

        List<MenuItem> rootItems = [];

        foreach (IGrouping<string?, CodeMenuEntryFilename> group in entries.GroupBy(entry => entry.Category))
        {
            List<MenuItem> items = group.Select(entry =>
            {
                string pattern = entry.ToPrefixString();
                MenuItem item = new()
                {
                    Header = $"{pattern} - {entry.Description}",
                    Focusable = false
                };
                item.Click += (_, _) => InsertText(textBox, pattern);
                return item;
            }).ToList();

            if (string.IsNullOrWhiteSpace(group.Key))
            {
                rootItems.AddRange(items);
            }
            else
            {
                rootItems.Add(new MenuItem { Header = group.Key, ItemsSource = items, Focusable = false });
            }
        }

        ContextMenu menu = new()
        {
            Focusable = false,
            Placement = PlacementMode.RightEdgeAlignedTop,
            PlacementTarget = textBox,
            ItemsSource = rootItems
        };
        textBox.ContextMenu = menu;

        void OpenMenu()
        {
            if (_activeNamePatternMenu != null && !ReferenceEquals(_activeNamePatternMenu, menu))
            {
                _activeNamePatternMenu.Close();
            }

            if (!menu.IsOpen)
            {
                _activeNamePatternMenu = menu;
                menu.Open(textBox);
            }
        }

        menu.Closed += (_, _) =>
        {
            if (ReferenceEquals(_activeNamePatternMenu, menu))
            {
                _activeNamePatternMenu = null;
            }
        };
        textBox.GotFocus += (_, _) => OpenMenu();
        textBox.PointerReleased += (_, _) => OpenMenu();

        return textBox;
    }

    private static void InsertText(TextBox textBox, string textToInsert)
    {
        string text = textBox.Text ?? string.Empty;
        int start = Math.Clamp(Math.Min(textBox.SelectionStart, textBox.SelectionEnd), 0, text.Length);
        textBox.Text = text.Insert(start, textToInsert);
        textBox.CaretIndex = start + textToInsert.Length;
        textBox.Focus();
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

    private static Button TaskMenu(Func<HotkeyType> getter, Action<HotkeyType> setter)
    {
        TextBlock selectedIcon = CreateTaskMenuIcon(getter());
        TextBlock selectedTitle = new()
        {
            FontWeight = FontWeight.Normal,
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center
        };
        TextBlock chevron = new()
        {
            Text = LucideIcons.chevron_down,
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        };
        chevron.Classes.Add("icon");

        Grid content = new()
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
            ColumnSpacing = 7
        };
        Grid.SetColumn(selectedTitle, 1);
        Grid.SetColumn(chevron, 2);
        content.Children.Add(selectedIcon);
        content.Children.Add(selectedTitle);
        content.Children.Add(chevron);

        Button button = new()
        {
            Content = content,
            MinWidth = 250,
            MinHeight = 32,
            Padding = new Thickness(8, 4),
            HorizontalAlignment = HorizontalAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Stretch
        };

        void UpdateSelectedTask(HotkeyType task)
        {
            selectedIcon.Text = TaskHelpers.FindMenuLucideIcon(task);
            selectedTitle.Text = task.GetLocalizedDescription();
        }

        void SelectTask(HotkeyType task)
        {
            setter(task);
            UpdateSelectedTask(task);
        }

        UpdateSelectedTask(getter());

        button.Click += (_, _) =>
        {
            List<HotkeyType> tasks = Helpers.GetEnums<HotkeyType>().ToList();
            List<MenuItem> rootItems = [CreateTaskMenuItem(HotkeyType.None, getter(), SelectTask)];

            foreach (IGrouping<string, HotkeyType> category in tasks
                .Where(task => task != HotkeyType.None)
                .Select(task => (Task: task, Category: new EnumInfo(task).Category ?? string.Empty))
                .Where(item => !string.IsNullOrWhiteSpace(item.Category))
                .GroupBy(item => item.Category, item => item.Task))
            {
                rootItems.Add(new MenuItem
                {
                    Header = category.Key,
                    ItemsSource = category.Select(task => CreateTaskMenuItem(task, getter(), SelectTask)).ToList()
                });
            }

            ContextMenu menu = new()
            {
                Placement = PlacementMode.BottomEdgeAlignedLeft,
                PlacementTarget = button,
                ItemsSource = rootItems
            };
            menu.Open(button);
        };

        return button;
    }

    private static Button TaskFlagsMenu<T>(
        Func<T> getter,
        Action<T> setter,
        IReadOnlyList<(T Task, string Header, string Icon)> options,
        string emptyIcon) where T : struct, Enum
    {
        IReadOnlyDictionary<T, (string Header, string Icon)> taskOptions = options
            .ToDictionary(option => option.Task, option => (option.Header, option.Icon));
        TextBlock selectedIcon = CreateAccentMenuIcon(emptyIcon);
        T lastSelectedTask = taskOptions.Keys.LastOrDefault(task => HasFlag(getter(), task));
        TextBlock selectedTasks = new()
        {
            FontWeight = FontWeight.Normal,
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center
        };
        TextBlock chevron = new()
        {
            Text = LucideIcons.chevron_down,
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        };
        chevron.Classes.Add("icon");

        Grid content = new()
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
            ColumnSpacing = 7
        };
        Grid.SetColumn(selectedTasks, 1);
        Grid.SetColumn(chevron, 2);
        content.Children.Add(selectedIcon);
        content.Children.Add(selectedTasks);
        content.Children.Add(chevron);

        Button button = new()
        {
            Content = content,
            MinWidth = 300,
            MinHeight = 32,
            Padding = new Thickness(8, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch
        };

        void UpdateSelectedTasks()
        {
            T selected = getter();
            if (!EqualityComparer<T>.Default.Equals(lastSelectedTask, default) && !HasFlag(selected, lastSelectedTask))
            {
                lastSelectedTask = taskOptions.Keys.LastOrDefault(task => HasFlag(selected, task));
            }

            selectedIcon.Text = EqualityComparer<T>.Default.Equals(lastSelectedTask, default)
                ? emptyIcon
                : taskOptions[lastSelectedTask].Icon;

            string[] titles = taskOptions
                .Where(option => HasFlag(selected, option.Key))
                .Select(option => option.Value.Header)
                .ToArray();
            selectedTasks.Text = titles.Length > 0
                ? string.Join(", ", titles)
                : ((Enum)(object)default(T)).GetLocalizedDescription();
        }

        UpdateSelectedTasks();

        button.Click += (_, _) =>
        {
            T selected = getter();
            List<MenuItem> items = [];

            foreach ((T task, (string header, string icon)) in taskOptions)
            {
                MenuItem item = new()
                {
                    Header = header,
                    Icon = CreateAccentMenuIcon(icon),
                    ToggleType = MenuItemToggleType.CheckBox,
                    IsChecked = HasFlag(selected, task)
                };
                item.Classes.Add("compact-menu-item");
                item.Click += (_, _) =>
                {
                    T updated = ToggleFlag(getter(), task);
                    setter(updated);
                    if (HasFlag(updated, task))
                    {
                        lastSelectedTask = task;
                    }
                    UpdateSelectedTasks();
                };
                items.Add(item);
            }

            ContextMenu menu = new()
            {
                Placement = PlacementMode.BottomEdgeAlignedLeft,
                PlacementTarget = button,
                ItemsSource = items
            };
            menu.Open(button);
        };

        return button;
    }

    private static Button DestinationMenu(TaskSettings settings)
    {
        TextBlock icon = CreateAccentMenuIcon(LucideIcons.server);
        TextBlock title = new()
        {
            Text = Strings.TaskSettingsWindow_Destinations + "...",
            FontWeight = FontWeight.Normal,
            VerticalAlignment = VerticalAlignment.Center
        };
        TextBlock chevron = new()
        {
            Text = LucideIcons.chevron_down,
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        };
        chevron.Classes.Add("icon");

        Grid content = new()
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
            ColumnSpacing = 7
        };
        Grid.SetColumn(title, 1);
        Grid.SetColumn(chevron, 2);
        content.Children.Add(icon);
        content.Children.Add(title);
        content.Children.Add(chevron);

        Button button = new()
        {
            Content = content,
            MinWidth = 300,
            MinHeight = 32,
            Padding = new Thickness(8, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch
        };

        button.Click += (_, _) =>
        {
            ContextMenu menu = new()
            {
                Placement = PlacementMode.BottomEdgeAlignedLeft,
                PlacementTarget = button,
                ItemsSource = MainMenuBuilder.BuildDestinationsMenu(settings)
                    .Select(CreateDestinationMenuItem)
                    .ToList()
            };
            menu.Open(button);
        };

        return button;
    }

    private static MenuItem CreateDestinationMenuItem(MainMenuEntry entry)
    {
        MenuItem item = new()
        {
            Header = entry.Header,
            IsChecked = entry.IsChecked,
            ToggleType = entry.ToggleType switch
            {
                MainMenuToggleType.CheckBox => MenuItemToggleType.CheckBox,
                MainMenuToggleType.Radio => MenuItemToggleType.Radio,
                _ => MenuItemToggleType.None
            }
        };
        item.Classes.Add("compact-menu-item");

        if (!string.IsNullOrEmpty(entry.Icon))
        {
            item.Icon = CreateAccentMenuIcon(entry.Icon);
        }

        if (entry.CreateChildren != null)
        {
            item.ItemsSource = entry.CreateChildren().Select(CreateDestinationMenuItem).ToList();
        }

        if (entry.ExecuteAsync != null)
        {
            item.Click += async (_, _) => await entry.ExecuteAsync();
        }

        return item;
    }

    private static bool HasFlag<T>(T value, T flag) where T : struct, Enum =>
        (Convert.ToUInt64(value) & Convert.ToUInt64(flag)) == Convert.ToUInt64(flag);

    private static T ToggleFlag<T>(T value, T flag) where T : struct, Enum =>
        (T)Enum.ToObject(typeof(T), Convert.ToUInt64(value) ^ Convert.ToUInt64(flag));

    private static MenuItem CreateTaskMenuItem(HotkeyType task, HotkeyType selectedTask, Action<HotkeyType> selectTask)
    {
        MenuItem item = new()
        {
            Header = task.GetLocalizedDescription(),
            Icon = CreateTaskMenuIcon(task),
            ToggleType = MenuItemToggleType.Radio,
            IsChecked = task == selectedTask
        };
        item.Click += (_, _) => selectTask(task);
        return item;
    }

    private static TextBlock CreateTaskMenuIcon(HotkeyType task)
        => CreateAccentMenuIcon(TaskHelpers.FindMenuLucideIcon(task));

    private static TextBlock CreateAccentMenuIcon(string glyph)
    {
        TextBlock icon = new()
        {
            Text = glyph,
            FontSize = 16,
            Width = 18,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        icon.Classes.Add("icon");
        icon.Classes.Add("accent-menu-icon");
        return icon;
    }

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

    private Control SoundPath(string title, Func<bool> enabledGetter, Action<bool> enabledSetter, Func<string> pathGetter, Action<string> pathSetter)
    {
        BoundValue<bool> enabled = new(enabledGetter(), enabledSetter);
        BoundValue<string> path = new(pathGetter(), pathSetter);
        TextBox text = Text(path);
        Button browse = Button("...", async () =>
        {
            string? selected = await PickFileAsync(Strings.TaskSettingsWindow_ChooseAudioFile, Strings.TaskSettingsWindow_WaveAudio, "*.wav");
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
