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
using System;
using System.Drawing.Imaging;
using System.IO;
using DrawingBitmap = System.Drawing.Bitmap;

namespace ShareX;

public partial class ClipboardUploadWindow : Window
{
    private readonly TaskSettings _taskSettings;
    private object? _clipboardContent;
    private Bitmap? _previewBitmap;
    private bool _keepClipboardContent;

    public bool DontShowAgain => DontShowAgainCheckBox.IsChecked == true;

    public ClipboardUploadWindow() : this(TaskSettings.GetDefaultTaskSettings())
    {
    }

    public ClipboardUploadWindow(TaskSettings taskSettings, bool showDontShowAgain = false)
    {
        _taskSettings = taskSettings;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Title = $"ShareX - {Properties.Resources.ClipboardUpload}";
        HeaderTitle.Text = Properties.Resources.ClipboardUpload;
        DontShowAgainCheckBox.IsVisible = showDontShowAgain;

        UploadButton.IsEnabled = LoadClipboardContent();

        Opened += (_, _) => Activate();
        Closed += OnClosed;
    }

    private bool LoadClipboardContent()
    {
        ImagePreviewContainer.IsVisible = false;
        TextPreview.IsVisible = false;
        FilePreview.IsVisible = false;
        EmptyPreview.IsVisible = false;

        if (ClipboardHelpers.ContainsImage())
        {
            using DrawingBitmap? clipboardImage = ClipboardHelpers.GetImage();
            if (clipboardImage != null)
            {
                DrawingBitmap image = (DrawingBitmap)clipboardImage.Clone();
                _clipboardContent = image;
                _previewBitmap = CreatePreviewBitmap(image);
                ImagePreview.Source = _previewBitmap;
                ImagePreviewContainer.IsVisible = true;
                ClipboardSummary.Text = string.Format(
                    Properties.Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_content__Image__Size___0_x_1__,
                    image.Width,
                    image.Height);
                return true;
            }
        }
        else if (ClipboardHelpers.ContainsText())
        {
            string text = ClipboardHelpers.GetText();
            if (!string.IsNullOrEmpty(text))
            {
                _clipboardContent = text;
                TextPreview.Text = text;
                TextPreview.IsVisible = true;
                ClipboardSummary.Text = string.Format(
                    Properties.Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_content__Text__Length___0__,
                    text.Length);
                return true;
            }
        }
        else if (ClipboardHelpers.ContainsFileDropList())
        {
            string[]? files = ClipboardHelpers.GetFileDropList();
            if (files is { Length: > 0 })
            {
                _clipboardContent = files;
                FilePreview.ItemsSource = files;
                FilePreview.IsVisible = true;
                ClipboardSummary.Text = string.Format(
                    Properties.Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_content__File__Count___0__,
                    files.Length);
                return true;
            }
        }

        ClipboardSummary.Text = Properties.Resources.ClipboardContentViewer_ClipboardContentViewer_Load_Clipboard_is_empty_or_contains_unknown_data_;
        EmptyPreview.Text = ClipboardSummary.Text;
        EmptyPreview.IsVisible = true;
        return false;
    }

    private static Bitmap CreatePreviewBitmap(DrawingBitmap image)
    {
        using MemoryStream stream = new();
        image.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        return new Bitmap(stream);
    }

    private void UploadClipboardContent()
    {
        switch (_clipboardContent)
        {
            case DrawingBitmap image:
                _keepClipboardContent = true;
                UploadManager.ProcessImageUpload(image, _taskSettings);
                break;
            case string text:
                UploadManager.ProcessTextUpload(text, _taskSettings);
                break;
            case string[] files:
                UploadManager.ProcessFilesUpload(files, _taskSettings);
                break;
        }
    }

    private void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        UploadClipboardContent();
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();

    private void OnImagePreviewPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(ImagePreviewContainer).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed &&
            _clipboardContent is DrawingBitmap image)
        {
            ImageViewerWindowIntegration.ShowImage(image);
            e.Handled = true;
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _previewBitmap?.Dispose();
        _previewBitmap = null;

        if (!_keepClipboardContent && _clipboardContent is DrawingBitmap image)
        {
            image.Dispose();
        }

        _clipboardContent = null;
    }
}
