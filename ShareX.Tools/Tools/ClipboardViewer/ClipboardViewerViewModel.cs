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
    private string _previewDetails = Localization.Strings.ClipboardViewerViewModel_Select_format;

    [ObservableProperty]
    private string _emptyMessage = Localization.Strings.ClipboardViewerViewModel_Clipboard_empty;

    public bool HasFormats => Formats.Count > 0;
    public bool HasImage => PreviewImage != null;
    public bool HasText => !string.IsNullOrEmpty(PreviewText);
    public bool HasPreview => HasImage || HasText;
    public string PreviewTitle => SelectedFormat ?? Localization.Strings.ClipboardViewerViewModel_Clipboard_content;
    public string FormatCountText => Formats.Count == 1 ? Localization.Strings.ClipboardViewerViewModel_One_format : string.Format(Localization.Strings.ClipboardViewerViewModel_Format_count, Formats.Count);

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
            EmptyMessage = Localization.Strings.ClipboardViewerViewModel_Clipboard_empty;
            SelectedFormat = Formats.FirstOrDefault(x => x.Equals(previousSelection, StringComparison.Ordinal))
                ?? Formats.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _clipboardData = null;
            Formats.Clear();
            SelectedFormat = null;
            EmptyMessage = string.Format(Localization.Strings.ClipboardViewerViewModel_Unable_read_clipboard, ex.Message);
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

        EmptyMessage = Localization.Strings.ClipboardViewerViewModel_No_previewable_content;

        try
        {
            ClipboardViewerPreview preview = _clipboardData.GetPreview(value);
            if (preview.IsImage && preview.ImageData != null)
            {
                PreviewImageData = preview.ImageData;
                PreviewImage = new Bitmap(new MemoryStream(preview.ImageData, writable: false));
                PreviewDetails = string.Format(Localization.Strings.ClipboardViewerViewModel_Image_details, preview.ImageWidth, preview.ImageHeight);
            }
            else
            {
                PreviewText = preview.Text;
                PreviewDetails = string.Format(Localization.Strings.ClipboardViewerViewModel_Text_details, preview.Text.Length);
            }
        }
        catch (Exception ex)
        {
            PreviewText = string.Format(Localization.Strings.ClipboardViewerViewModel_Preview_failed, ex.Message);
            PreviewDetails = Localization.Strings.ClipboardViewerViewModel_Preview_unavailable;
            ToolsDiagnostics.ReportWarning(nameof(ClipboardViewerViewModel), $"Failed to preview clipboard format '{value}'.", ex);
        }
    }

    private void ClearPreview()
    {
        PreviewImage?.Dispose();
        PreviewImage = null;
        PreviewImageData = null;
        PreviewText = string.Empty;
        PreviewDetails = Localization.Strings.ClipboardViewerViewModel_Select_format;
    }

    public void Dispose()
    {
        PreviewImage?.Dispose();
    }
}
