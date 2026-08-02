#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingImageFormat = System.Drawing.Imaging.ImageFormat;

namespace ShareX;

public readonly record struct AfterCaptureWindowResult(bool Accepted, string? FileName);

public partial class AfterCaptureWindow : Window
{
    private readonly DrawingBitmap? _sourceImage;
    private readonly DrawingBitmap? _ownedSourceImage;
    private Bitmap? _previewBitmap;

    public TaskSettings TaskSettings { get; }
    public IReadOnlyList<AfterCaptureTaskOption> AfterCaptureOptions { get; }
    public IReadOnlyList<AfterCaptureTaskOption> AfterUploadOptions { get; }
    public IReadOnlyList<AfterCaptureDestinationOption> DestinationOptions { get; }
    public AfterCaptureWindowResult Result { get; private set; }

    public AfterCaptureWindow() : this(global::ShareX.TaskSettings.GetDefaultTaskSettings(), null, null)
    {
    }

    public AfterCaptureWindow(TaskSettings taskSettings, TaskMetadata? metadata, string? filePath)
    {
        TaskSettings = taskSettings;
        AfterCaptureOptions = CreateAfterCaptureOptions(taskSettings.AfterCaptureJob);
        AfterUploadOptions = CreateAfterUploadOptions(taskSettings.AfterUploadJob);
        DestinationOptions = CreateDestinationOptions(taskSettings);

        InitializeComponent();
        DataContext = this;
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        string fileName;
        if (!string.IsNullOrEmpty(filePath))
        {
            fileName = Path.GetFileNameWithoutExtension(filePath);
            if (FileHelpers.IsImageFile(filePath))
            {
                _ownedSourceImage = ImageHelpers.LoadImage(filePath);
                _sourceImage = _ownedSourceImage;
            }
        }
        else
        {
            _sourceImage = metadata?.Image;
            fileName = TaskHelpers.GetFileName(taskSettings, null, metadata);
        }

        FileNameTextBox.Text = fileName;
        CopyButton.IsEnabled = metadata?.Image != null;
        LoadPreview(filePath);

        Opened += (_, _) => Activate();
        Closed += OnClosed;
    }

    private void LoadPreview(string? filePath)
    {
        if (_sourceImage != null)
        {
            using MemoryStream stream = new();
            _sourceImage.Save(stream, DrawingImageFormat.Png);
            stream.Position = 0;
            _previewBitmap = new Bitmap(stream);
            PreviewImage.Source = _previewBitmap;
            PreviewImage.IsVisible = true;
            EmptyPreview.IsVisible = false;
            ImageSizeText.Text = $"{_sourceImage.Width} × {_sourceImage.Height}";
            ImageSizeBadge.IsVisible = true;
            return;
        }

        PreviewImage.IsVisible = false;
        ImageSizeBadge.IsVisible = false;
        EmptyPreview.IsVisible = true;
        EmptyPreviewText.Text = string.IsNullOrEmpty(filePath)
            ? Strings.AfterCaptureWindow_NoPreview
            : Path.GetFileName(filePath);
    }

    private static IReadOnlyList<AfterCaptureTaskOption> CreateAfterCaptureOptions(AfterCaptureTasks selected)
    {
        AfterCaptureTasks[] ignored =
        [
            AfterCaptureTasks.None,
            AfterCaptureTasks.ShowQuickTaskMenu,
            AfterCaptureTasks.ShowAfterCaptureWindow
        ];

        return Helpers.GetEnums<AfterCaptureTasks>()
            .Where(task => !ignored.Contains(task))
            .Select(task => new AfterCaptureTaskOption(task, task.GetLocalizedDescription(), selected.HasFlag(task)))
            .ToArray();
    }

    private static IReadOnlyList<AfterCaptureTaskOption> CreateAfterUploadOptions(AfterUploadTasks selected)
    {
        return Helpers.GetEnums<AfterUploadTasks>()
            .Where(task => task != AfterUploadTasks.None)
            .Select(task => new AfterCaptureTaskOption(task, task.GetLocalizedDescription(), selected.HasFlag(task)))
            .ToArray();
    }

    private static IReadOnlyList<AfterCaptureDestinationOption> CreateDestinationOptions(TaskSettings taskSettings)
    {
        List<AfterCaptureDestinationOption> options = new();

        foreach (ImageDestination destination in Helpers.GetEnums<ImageDestination>())
        {
            if (destination == ImageDestination.FileUploader ||
                !UploadersConfigValidator.Validate<ImageDestination>((int)destination, Program.UploadersConfig))
            {
                continue;
            }

            string label = destination.GetLocalizedDescription();
            if (destination == ImageDestination.CustomImageUploader)
            {
                label = GetCustomDestinationLabel(Program.UploadersConfig.CustomImageUploaderSelected, taskSettings, label);
            }

            bool selected = taskSettings.ImageDestination == destination;
            options.Add(new AfterCaptureDestinationOption(label, selected, () => taskSettings.ImageDestination = destination));
        }

        foreach (FileDestination destination in Helpers.GetEnums<FileDestination>())
        {
            if (!UploadersConfigValidator.Validate<FileDestination>((int)destination, Program.UploadersConfig))
            {
                continue;
            }

            string label = destination.GetLocalizedDescription();
            if (destination == FileDestination.CustomFileUploader)
            {
                label = GetCustomDestinationLabel(Program.UploadersConfig.CustomFileUploaderSelected, taskSettings, label);
            }

            bool selected = taskSettings.ImageDestination == ImageDestination.FileUploader &&
                taskSettings.ImageFileDestination == destination;
            options.Add(new AfterCaptureDestinationOption(label, selected, () =>
            {
                taskSettings.ImageDestination = ImageDestination.FileUploader;
                taskSettings.ImageFileDestination = destination;
            }));
        }

        return options;
    }

    private static string GetCustomDestinationLabel(int index, TaskSettings taskSettings, string fallback)
    {
        if (taskSettings.OverrideCustomUploader)
        {
            index = taskSettings.CustomUploaderIndex.BetweenOrDefault(0, Program.UploadersConfig.CustomUploadersList.Count - 1);
        }

        CustomUploaderItem? uploader = Program.UploadersConfig.CustomUploadersList.ReturnIfValidIndex(index);
        return uploader == null
            ? fallback
            : string.Format(Strings.AfterCaptureWindow_CustomUploader, uploader);
    }

    private void OnContinueClick(object? sender, RoutedEventArgs e)
    {
        AfterCaptureTasks afterCaptureTasks = AfterCaptureTasks.None;
        foreach (AfterCaptureTaskOption option in AfterCaptureOptions.Where(option => option.IsChecked))
        {
            afterCaptureTasks |= (AfterCaptureTasks)option.Value;
        }

        AfterUploadTasks afterUploadTasks = AfterUploadTasks.None;
        foreach (AfterCaptureTaskOption option in AfterUploadOptions.Where(option => option.IsChecked))
        {
            afterUploadTasks |= (AfterUploadTasks)option.Value;
        }

        TaskSettings.AfterCaptureJob = afterCaptureTasks;
        TaskSettings.AfterUploadJob = afterUploadTasks;
        AcceptAndClose();
    }

    private void OnSectionSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (AfterCaptureOptionsPanel == null || DestinationOptionsPanel == null || AfterUploadOptionsPanel == null)
        {
            return;
        }

        AfterCaptureOptionsPanel.IsVisible = SectionNavigation.SelectedIndex == 0;
        DestinationOptionsPanel.IsVisible = SectionNavigation.SelectedIndex == 1;
        AfterUploadOptionsPanel.IsVisible = SectionNavigation.SelectedIndex == 2;
    }

    private void OnCopyClick(object? sender, RoutedEventArgs e)
    {
        TaskSettings.AfterCaptureJob = AfterCaptureTasks.CopyImageToClipboard;
        AcceptAndClose();
    }

    private void AcceptAndClose()
    {
        Result = new AfterCaptureWindowResult(true, FileNameTextBox.Text ?? string.Empty);
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();

    private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_sourceImage != null &&
            e.GetCurrentPoint(PreviewSurface).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            ImageViewerWindowIntegration.ShowImage(_sourceImage);
            e.Handled = true;
        }
    }

    private void OnCopyPreviewClick(object? sender, RoutedEventArgs e)
    {
        if (_sourceImage != null)
        {
            ClipboardHelpers.CopyImage(_sourceImage);
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _previewBitmap?.Dispose();
        _previewBitmap = null;
        _ownedSourceImage?.Dispose();
    }
}

public sealed class AfterCaptureTaskOption
{
    public Enum Value { get; }
    public string Label { get; }
    public bool IsChecked { get; set; }

    public AfterCaptureTaskOption(Enum value, string label, bool isChecked)
    {
        Value = value;
        Label = label;
        IsChecked = isChecked;
    }
}

public sealed class AfterCaptureDestinationOption
{
    private readonly Action _select;
    private bool _isSelected;

    public string Label { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            if (value)
            {
                _select();
            }
        }
    }

    public AfterCaptureDestinationOption(string label, bool isSelected, Action select)
    {
        Label = label;
        _isSelected = isSelected;
        _select = select;
    }
}
