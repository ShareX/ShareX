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
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.IO;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingImageFormat = System.Drawing.Imaging.ImageFormat;

namespace ShareX;

public partial class BeforeUploadWindow : Window
{
    private readonly TaskInfo _info;
    private readonly DrawingBitmap? _sourceImage;
    private readonly DrawingBitmap? _ownedSourceImage;
    private Bitmap? _previewBitmap;

    public IReadOnlyList<BeforeUploadDestinationOption> DestinationOptions { get; }
    public bool Accepted { get; private set; }

    public BeforeUploadWindow() : this(new TaskInfo(global::ShareX.TaskSettings.GetDefaultTaskSettings()))
    {
    }

    public BeforeUploadWindow(TaskInfo info)
    {
        _info = info;
        DestinationOptions = CreateDestinationOptions(info);

        InitializeComponent();
        DataContext = this;
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        if (info.Metadata?.Image != null)
        {
            _sourceImage = info.Metadata.Image;
        }
        else if (!string.IsNullOrEmpty(info.FilePath) && File.Exists(info.FilePath) && FileHelpers.IsImageFile(info.FilePath))
        {
            _ownedSourceImage = ImageHelpers.LoadImage(info.FilePath);
            _sourceImage = _ownedSourceImage;
        }

        string? selectedDestination = null;
        foreach (BeforeUploadDestinationOption option in DestinationOptions)
        {
            if (option.IsSelected)
            {
                selectedDestination = option.Label;
                break;
            }
        }

        UpdatePrompt(selectedDestination);
        LoadPreview();

        Opened += (_, _) => Activate();
        Closed += OnClosed;
    }

    private IReadOnlyList<BeforeUploadDestinationOption> CreateDestinationOptions(TaskInfo info)
    {
        List<BeforeUploadDestinationOption> options = new();
        TaskSettings taskSettings = info.TaskSettings;

        switch (info.DataType)
        {
            case EDataType.Image:
                foreach (ImageDestination destination in Helpers.GetEnums<ImageDestination>())
                {
                    if (destination == ImageDestination.FileUploader ||
                        !UploadersConfigValidator.Validate<ImageDestination>((int)destination, Program.UploadersConfig))
                    {
                        continue;
                    }

                    string label = GetDestinationLabel(destination, taskSettings);
                    options.Add(CreateOption(label, taskSettings.ImageDestination == destination,
                        () => taskSettings.ImageDestination = destination));
                }

                foreach (FileDestination destination in Helpers.GetEnums<FileDestination>())
                {
                    if (!UploadersConfigValidator.Validate<FileDestination>((int)destination, Program.UploadersConfig))
                    {
                        continue;
                    }

                    string label = GetDestinationLabel(destination, taskSettings);
                    bool selected = taskSettings.ImageDestination == ImageDestination.FileUploader &&
                        taskSettings.ImageFileDestination == destination;
                    options.Add(CreateOption(label, selected, () =>
                    {
                        taskSettings.ImageDestination = ImageDestination.FileUploader;
                        taskSettings.ImageFileDestination = destination;
                    }));
                }
                break;
            case EDataType.Text:
                foreach (TextDestination destination in Helpers.GetEnums<TextDestination>())
                {
                    if (destination == TextDestination.FileUploader ||
                        !UploadersConfigValidator.Validate<TextDestination>((int)destination, Program.UploadersConfig))
                    {
                        continue;
                    }

                    string label = GetDestinationLabel(destination, taskSettings);
                    options.Add(CreateOption(label, taskSettings.TextDestination == destination,
                        () => taskSettings.TextDestination = destination));
                }

                foreach (FileDestination destination in Helpers.GetEnums<FileDestination>())
                {
                    if (!UploadersConfigValidator.Validate<FileDestination>((int)destination, Program.UploadersConfig))
                    {
                        continue;
                    }

                    string label = GetDestinationLabel(destination, taskSettings);
                    bool selected = taskSettings.TextDestination == TextDestination.FileUploader &&
                        taskSettings.TextFileDestination == destination;
                    options.Add(CreateOption(label, selected, () =>
                    {
                        taskSettings.TextDestination = TextDestination.FileUploader;
                        taskSettings.TextFileDestination = destination;
                    }));
                }
                break;
            case EDataType.File:
                foreach (FileDestination destination in Helpers.GetEnums<FileDestination>())
                {
                    if (!UploadersConfigValidator.Validate<FileDestination>((int)destination, Program.UploadersConfig))
                    {
                        continue;
                    }

                    string label = GetDestinationLabel(destination, taskSettings);
                    options.Add(CreateOption(label, taskSettings.FileDestination == destination, () =>
                    {
                        taskSettings.ImageDestination = ImageDestination.FileUploader;
                        taskSettings.TextDestination = TextDestination.FileUploader;
                        taskSettings.ImageFileDestination = destination;
                        taskSettings.TextFileDestination = destination;
                        taskSettings.FileDestination = destination;
                    }));
                }
                break;
            case EDataType.URL:
                foreach (UrlShortenerType destination in Helpers.GetEnums<UrlShortenerType>())
                {
                    if (!UploadersConfigValidator.Validate<UrlShortenerType>((int)destination, Program.UploadersConfig))
                    {
                        continue;
                    }

                    string label = GetDestinationLabel(destination, taskSettings);
                    options.Add(CreateOption(label, taskSettings.URLShortenerDestination == destination,
                        () => taskSettings.URLShortenerDestination = destination));
                }
                break;
        }

        return options;
    }

    private BeforeUploadDestinationOption CreateOption(string label, bool selected, Action select)
    {
        return new BeforeUploadDestinationOption(label, selected, () =>
        {
            select();
            UpdatePrompt(label);
        });
    }

    private static string GetDestinationLabel(Enum destination, TaskSettings taskSettings)
    {
        int customUploaderIndex = -1;

        if (destination is ImageDestination.CustomImageUploader)
        {
            customUploaderIndex = Program.UploadersConfig.CustomImageUploaderSelected;
        }
        else if (destination is TextDestination.CustomTextUploader)
        {
            customUploaderIndex = Program.UploadersConfig.CustomTextUploaderSelected;
        }
        else if (destination is FileDestination.CustomFileUploader)
        {
            customUploaderIndex = Program.UploadersConfig.CustomFileUploaderSelected;
        }
        else if (destination is UrlShortenerType.CustomURLShortener)
        {
            customUploaderIndex = Program.UploadersConfig.CustomURLShortenerSelected;
        }

        if (customUploaderIndex >= 0)
        {
            if (taskSettings.OverrideCustomUploader)
            {
                customUploaderIndex = taskSettings.CustomUploaderIndex.BetweenOrDefault(
                    0, Program.UploadersConfig.CustomUploadersList.Count - 1);
            }

            CustomUploaderItem? uploader = Program.UploadersConfig.CustomUploadersList.ReturnIfValidIndex(customUploaderIndex);
            if (uploader != null)
            {
                return string.Format("{0} [{1}]", Properties.Resources.BeforeUploadControl_AddDestination_Custom, uploader);
            }
        }

        return destination.GetLocalizedDescription();
    }

    private void UpdatePrompt(string? destination)
    {
        if (PromptText == null)
        {
            return;
        }

        PromptText.Text = string.IsNullOrEmpty(destination)
            ? Properties.Resources.BeforeUploadForm_BeforeUploadForm_Please_choose_a_destination_
            : string.Format(
                Properties.Resources.BeforeUploadForm_BeforeUploadForm__0__is_about_to_be_uploaded_to__1___You_may_choose_a_different_destination_,
                _info.FileName,
                destination);
    }

    private void LoadPreview()
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
        EmptyPreviewText.Text = string.IsNullOrEmpty(_info.FileName) ? "No preview available" : _info.FileName;
    }

    private void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        Accepted = true;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();

    private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_sourceImage != null &&
            e.GetCurrentPoint(PreviewSurface).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            ImageViewer.ShowImage(_sourceImage);
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

public sealed class BeforeUploadDestinationOption
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

    public BeforeUploadDestinationOption(string label, bool isSelected, Action select)
    {
        Label = label;
        _isSelected = isSelected;
        _select = select;
    }
}
