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

using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.HelpersLib;
using System.Collections.ObjectModel;

namespace ShareX.Tools;

public sealed partial class ClipboardViewerViewModel : ViewModelBase, IDisposable
{
    private ClipboardViewerData? _clipboardData;

    public ObservableCollection<string> Formats { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewTitle))]
    private string? _selectedFormat;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private Bitmap? _previewImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasText))]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private string _previewText = string.Empty;

    [ObservableProperty]
    private byte[]? _previewImageData;

    [ObservableProperty]
    private string _previewDetails = "Select a format to inspect its contents";

    [ObservableProperty]
    private string _emptyMessage = "The clipboard is empty";

    public bool HasFormats => Formats.Count > 0;
    public bool HasImage => PreviewImage != null;
    public bool HasText => !string.IsNullOrEmpty(PreviewText);
    public bool HasPreview => HasImage || HasText;
    public string PreviewTitle => SelectedFormat ?? "Clipboard content";
    public string FormatCountText => Formats.Count == 1 ? "1 format" : $"{Formats.Count} formats";

    public ClipboardViewerViewModel()
    {
        Refresh();
    }

    [RelayCommand]
    private void Refresh()
    {
        string? previousSelection = SelectedFormat;
        SelectedFormat = null;
        ClearPreview();

        try
        {
            _clipboardData = ClipboardViewerData.Capture();
            Formats.Clear();
            foreach (string format in _clipboardData.Formats)
            {
                Formats.Add(format);
            }

            OnPropertyChanged(nameof(HasFormats));
            OnPropertyChanged(nameof(FormatCountText));
            EmptyMessage = "The clipboard is empty";
            SelectedFormat = Formats.FirstOrDefault(x => x.Equals(previousSelection, StringComparison.Ordinal))
                ?? Formats.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _clipboardData = null;
            Formats.Clear();
            SelectedFormat = null;
            EmptyMessage = $"Unable to read the clipboard\n{ex.Message}";
            OnPropertyChanged(nameof(HasFormats));
            OnPropertyChanged(nameof(FormatCountText));
            ToolsDiagnostics.ReportWarning(nameof(ClipboardViewerViewModel), "Failed to read clipboard contents.", ex);
        }
    }

    [RelayCommand]
    private void ClearClipboard()
    {
        ClipboardHelpers.Clear();
        Refresh();
    }

    partial void OnSelectedFormatChanged(string? value)
    {
        ClearPreview();

        if (string.IsNullOrWhiteSpace(value) || _clipboardData == null)
        {
            return;
        }

        EmptyMessage = "This format contains no previewable content";

        try
        {
            ClipboardViewerPreview preview = _clipboardData.GetPreview(value);
            if (preview.IsImage && preview.ImageData != null)
            {
                PreviewImageData = preview.ImageData;
                PreviewImage = new Bitmap(new MemoryStream(preview.ImageData, writable: false));
                PreviewDetails = $"Image  |  {preview.ImageWidth} × {preview.ImageHeight} px";
            }
            else
            {
                PreviewText = preview.Text;
                PreviewDetails = $"Text  |  {preview.Text.Length:N0} characters";
            }
        }
        catch (Exception ex)
        {
            PreviewText = $"This clipboard format could not be previewed.\n\n{ex.Message}";
            PreviewDetails = "Preview unavailable";
            ToolsDiagnostics.ReportWarning(nameof(ClipboardViewerViewModel), $"Failed to preview clipboard format '{value}'.", ex);
        }
    }

    private void ClearPreview()
    {
        PreviewImage?.Dispose();
        PreviewImage = null;
        PreviewImageData = null;
        PreviewText = string.Empty;
        PreviewDetails = "Select a format to inspect its contents";
    }

    public void Dispose()
    {
        PreviewImage?.Dispose();
    }
}
