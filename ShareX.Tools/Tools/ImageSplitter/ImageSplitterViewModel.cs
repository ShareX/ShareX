#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.
*/

#endregion License Information (GPL v3)

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.HelpersLib;
using ShareX.Tools.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;

namespace ShareX.Tools;

public sealed partial class ImageSplitterViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _imageFilePath = string.Empty;

    [ObservableProperty]
    private string _outputFolderPath = string.Empty;

    [ObservableProperty]
    private decimal _rowCount = 1;

    [ObservableProperty]
    private decimal _columnCount = 1;

    [ObservableProperty]
    private bool _isBusy;

    public Func<Task<string?>>? SelectImageRequested { get; set; }
    public Func<Task<string?>>? SelectOutputFolderRequested { get; set; }

    public bool CanProcess => !IsBusy && File.Exists(ImageFilePath) && Directory.Exists(OutputFolderPath) &&
        (RowCount > 1 || ColumnCount > 1);

    public string GridSizeText => string.Format(Localization.Strings.ImageSplitterViewModel_Grid_size, (int)ColumnCount, (int)RowCount);

    [RelayCommand]
    private async Task BrowseImageAsync()
    {
        if (SelectImageRequested == null)
        {
            return;
        }

        string? filePath = await SelectImageRequested();
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            SetImageFile(filePath);
        }
    }

    [RelayCommand]
    private async Task BrowseOutputFolderAsync()
    {
        if (SelectOutputFolderRequested == null)
        {
            return;
        }

        string? folder = await SelectOutputFolderRequested();
        if (!string.IsNullOrWhiteSpace(folder))
        {
            OutputFolderPath = folder;
        }
    }

    public void SetImageFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        ImageFilePath = filePath;
        if (string.IsNullOrWhiteSpace(OutputFolderPath))
        {
            OutputFolderPath = Path.GetDirectoryName(filePath) ?? string.Empty;
        }
    }

    [RelayCommand]
    private async Task SplitImageAsync()
    {
        if (!CanProcess)
        {
            return;
        }

        IsBusy = true;
        try
        {
            List<string> files = await Task.Run(() => SplitImage(ImageFilePath, (int)RowCount, (int)ColumnCount, OutputFolderPath));
            if (files.Count > 0)
            {
                FileHelpers.OpenFolderWithFile(files[0]);
            }
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(ImageSplitterViewModel), "Failed to split image.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnImageFilePathChanged(string value) => NotifyStateChanged();
    partial void OnOutputFolderPathChanged(string value) => NotifyStateChanged();
    partial void OnRowCountChanged(decimal value) => NotifyStateChanged();
    partial void OnColumnCountChanged(decimal value) => NotifyStateChanged();
    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(CanProcess));

    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(CanProcess));
        OnPropertyChanged(nameof(GridSizeText));
    }

    private static List<string> SplitImage(string filePath, int rowCount, int columnCount, string outputFolder)
    {
        List<string> filePaths = [];
        using Bitmap? source = ImageHelpers.LoadImage(filePath);
        if (source == null)
        {
            return filePaths;
        }

        List<Bitmap> images = ImageHelpers.SplitImage(source, rowCount, columnCount);
        try
        {
            string originalFileName = Path.GetFileNameWithoutExtension(filePath);
            for (int i = 0; i < images.Count; i++)
            {
                string outputPath = Path.Combine(outputFolder, $"{originalFileName}{i + 1}.png");
                images[i].Save(outputPath, ImageFormat.Png);
                filePaths.Add(outputPath);
            }
        }
        finally
        {
            foreach (Bitmap image in images)
            {
                image.Dispose();
            }
        }

        return filePaths;
    }
}
