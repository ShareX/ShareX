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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShareX.Tools;

public sealed partial class PinToScreenStartupViewModel : ViewModelBase
{
    private readonly PinToScreenServices _services;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    public bool IsIdle => !IsBusy;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public Action<PinToScreenSource>? SourceSelected { get; set; }
    public Action? RegionCaptureStarted { get; set; }
    public Action? RegionCaptureFinished { get; set; }

    public PinToScreenStartupViewModel(PinToScreenServices services)
    {
        _services = services;
    }

    [RelayCommand]
    private async Task CaptureRegionAsync()
    {
        RegionCaptureStarted?.Invoke();
        try
        {
            await Task.Delay(200);
            await SelectAsync(_services.CaptureRegionAsync, "No region was selected.");
        }
        finally
        {
            RegionCaptureFinished?.Invoke();
        }
    }

    [RelayCommand]
    private Task FromClipboardAsync() => SelectAsync(_services.GetClipboardImageAsync, "The clipboard does not contain an image.");

    [RelayCommand]
    private Task FromFileAsync() => SelectAsync(_services.SelectImageFileAsync, "No image was selected.");

    private async Task SelectAsync(Func<Task<PinToScreenSource?>> selector, string emptyMessage)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            PinToScreenSource? source = await selector();
            if (source == null || source.ImageData.Length == 0)
            {
                ErrorMessage = emptyMessage;
                return;
            }

            SourceSelected?.Invoke(source);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unable to load the image. {ex.Message}";
            ToolsDiagnostics.ReportWarning(nameof(PinToScreenStartupViewModel), "Unable to select an image to pin.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
