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
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingImageFormat = System.Drawing.Imaging.ImageFormat;

namespace ShareX;

public partial class AfterUploadWindow : Window
{
    private readonly TaskInfo _info;
    private readonly UploadInfoParser _parser = new();
    private readonly DispatcherTimer _closeTimer;
    private DrawingBitmap? _sourceImage;
    private DrawingBitmap? _ownedSourceImage;
    private Bitmap? _previewBitmap;
    private bool _closed;

    public IReadOnlyList<AfterUploadFormatItem> Formats { get; }
    public string ResultText { get; }

    public AfterUploadWindow() : this(new TaskInfo(global::ShareX.TaskSettings.GetDefaultTaskSettings()))
    {
    }

    public AfterUploadWindow(TaskInfo info)
    {
        _info = info;
        Formats = CreateFormats();
        ResultText = !string.IsNullOrEmpty(info.Result?.URL) ? info.Result.URL : info.FileName;

        InitializeComponent();
        DataContext = this;
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Title = "ShareX - " + (!string.IsNullOrEmpty(info.FilePath) && File.Exists(info.FilePath) ? info.FilePath : info.FileName);

        LoadInitialPreview();
        UpdateActionStates();

        _closeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(60) };
        _closeTimer.Tick += (_, _) => Close();

        Opened += OnOpened;
        Closed += OnClosed;
    }

    private IReadOnlyList<AfterUploadFormatItem> CreateFormats()
    {
        List<AfterUploadFormatItem> formats = new();
        bool isImageURL = FileHelpers.IsImageFile(_info.Result.URL);

        foreach (LinkFormatEnum type in Helpers.GetEnums<LinkFormatEnum>())
        {
            if (!isImageURL && IsImageFormat(type))
            {
                continue;
            }

            AddFormat(formats, GetFormatGroup(type), type.GetLocalizedDescription(), GetURLByType(type));
        }

        if (isImageURL)
        {
            foreach (ClipboardFormat format in Program.Settings.ClipboardContentFormats)
            {
                AddFormat(formats, "Custom", format.Description, _parser.Parse(_info, format.Format));
            }
        }

        return formats;
    }

    private static void AddFormat(List<AfterUploadFormatItem> formats, string group, string? description, string? text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            formats.Add(new AfterUploadFormatItem(group, description ?? string.Empty, text));
        }
    }

    private static bool IsImageFormat(LinkFormatEnum type) => type is
        LinkFormatEnum.HTMLImage or LinkFormatEnum.HTMLLinkedImage or
        LinkFormatEnum.ForumImage or LinkFormatEnum.ForumLinkedImage or
        LinkFormatEnum.WikiImage or LinkFormatEnum.WikiLinkedImage;

    private static string GetFormatGroup(LinkFormatEnum type) => type switch
    {
        LinkFormatEnum.ForumImage or LinkFormatEnum.ForumLinkedImage => "Forums",
        LinkFormatEnum.HTMLImage or LinkFormatEnum.HTMLLinkedImage => "HTML",
        LinkFormatEnum.WikiImage or LinkFormatEnum.WikiLinkedImage => "Wiki",
        LinkFormatEnum.LocalFilePath or LinkFormatEnum.LocalFilePathUri => "Local",
        _ => "Links"
    };

    private string GetURLByType(LinkFormatEnum type) => type switch
    {
        LinkFormatEnum.URL => _info.Result.URL,
        LinkFormatEnum.ShortenedURL => _info.Result.ShortenedURL,
        LinkFormatEnum.ForumImage => _parser.Parse(_info, UploadInfoParser.ForumImage),
        LinkFormatEnum.HTMLImage => _parser.Parse(_info, UploadInfoParser.HTMLImage),
        LinkFormatEnum.WikiImage => _parser.Parse(_info, UploadInfoParser.WikiImage),
        LinkFormatEnum.ForumLinkedImage => _parser.Parse(_info, UploadInfoParser.ForumLinkedImage),
        LinkFormatEnum.HTMLLinkedImage => _parser.Parse(_info, UploadInfoParser.HTMLLinkedImage),
        LinkFormatEnum.WikiLinkedImage => _parser.Parse(_info, UploadInfoParser.WikiLinkedImage),
        LinkFormatEnum.ThumbnailURL => _info.Result.ThumbnailURL,
        LinkFormatEnum.LocalFilePath => _info.FilePath,
        LinkFormatEnum.LocalFilePathUri => GetLocalFilePathAsUri(_info.FilePath),
        _ => _info.Result.URL
    };

    private static string GetLocalFilePathAsUri(string? filePath)
    {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            try
            {
                return new Uri(filePath).AbsoluteUri;
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
            }
        }

        return string.Empty;
    }

    private void LoadInitialPreview()
    {
        if (_info.DataType == EDataType.Image)
        {
            if (!string.IsNullOrEmpty(_info.FilePath) && File.Exists(_info.FilePath) && FileHelpers.IsImageFile(_info.FilePath))
            {
                _ownedSourceImage = ImageHelpers.LoadImage(_info.FilePath);
            }
            else if (_info.Metadata?.Image != null)
            {
                _ownedSourceImage = _info.Metadata.Image.CloneSafe();
            }

            _sourceImage = _ownedSourceImage;
        }

        ApplyPreview();
    }

    private void ApplyPreview()
    {
        _previewBitmap?.Dispose();
        _previewBitmap = null;

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
        }
        else
        {
            PreviewImage.Source = null;
            PreviewImage.IsVisible = false;
            EmptyPreview.IsVisible = true;
            ImageSizeBadge.IsVisible = false;
        }
    }

    private void UpdateActionStates()
    {
        bool fileExists = !string.IsNullOrEmpty(_info.FilePath) && File.Exists(_info.FilePath);
        CopyImageButton.IsEnabled = _sourceImage != null;
        CopyLinkButton.IsEnabled = Formats.Count > 0;
        OpenLinkButton.IsEnabled = !string.IsNullOrEmpty(_info.Result.URL);
        OpenFileButton.IsEnabled = fileExists;
        OpenFolderButton.IsEnabled = fileExists;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        if (_info.TaskSettings.AdvancedSettings.AutoCloseAfterUploadForm)
        {
            _closeTimer.Start();
        }

        if (_sourceImage == null && _info.DataType == EDataType.Image && !string.IsNullOrEmpty(_info.Result.URL))
        {
            await LoadRemotePreviewAsync(_info.Result.URL);
        }
    }

    private async Task LoadRemotePreviewAsync(string url)
    {
        try
        {
            DrawingBitmap? image = await WebHelpers.DownloadImageAsync(url);
            if (image == null)
            {
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (_closed)
                {
                    image.Dispose();
                    return;
                }

                _ownedSourceImage?.Dispose();
                _ownedSourceImage = image;
                _sourceImage = image;
                ApplyPreview();
                UpdateActionStates();
            });
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
        }
    }

    private void CopySelectedFormat()
    {
        AfterUploadFormatItem? selected = FormatList.SelectedItem as AfterUploadFormatItem;
        selected ??= Formats.Count > 0 ? Formats[0] : null;

        if (!string.IsNullOrEmpty(selected?.Text))
        {
            ClipboardHelpers.CopyText(selected.Text);
        }
    }

    private void OnCopyImageClick(object? sender, RoutedEventArgs e)
    {
        if (_sourceImage != null)
        {
            ClipboardHelpers.CopyImage(_sourceImage);
        }
    }

    private void OnCopyLinkClick(object? sender, RoutedEventArgs e) => CopySelectedFormat();

    private void OnFormatDoubleTapped(object? sender, TappedEventArgs e) => CopySelectedFormat();

    private void OnOpenLinkClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_info.Result.URL))
        {
            URLHelpers.OpenURL(_info.Result.URL);
        }
    }

    private void OnOpenFileClick(object? sender, RoutedEventArgs e) => FileHelpers.OpenFile(_info.FilePath);

    private void OnOpenFolderClick(object? sender, RoutedEventArgs e) => FileHelpers.OpenFolderWithFile(_info.FilePath);

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

    private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_sourceImage != null &&
            e.GetCurrentPoint(PreviewSurface).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            ImageViewerWindowIntegration.ShowImage(_sourceImage);
            e.Handled = true;
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _closed = true;
        _closeTimer.Stop();
        _previewBitmap?.Dispose();
        _previewBitmap = null;
        _ownedSourceImage?.Dispose();
        _ownedSourceImage = null;
        _sourceImage = null;
    }
}

public sealed record AfterUploadFormatItem(string Group, string Description, string Text);
