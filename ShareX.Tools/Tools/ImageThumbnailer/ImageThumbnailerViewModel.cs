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
using ShareX.HelpersLib;
using ShareX.Tools.Infrastructure;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ShareX.Tools;

public sealed partial class ImageThumbnailerViewModel : ViewModelBase
{
    private readonly HashSet<string> _selectedImages = [];

    public ObservableCollection<string> Images { get; } = [];

    [ObservableProperty]
    private string? _selectedImage;

    [ObservableProperty]
    private decimal _width = 150;

    [ObservableProperty]
    private decimal _height = 150;

    [ObservableProperty]
    private decimal _quality = 90;

    [ObservableProperty]
    private string _outputFolderPath = string.Empty;

    [ObservableProperty]
    private string _outputFileName = "$filename_th";

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMessage))]
    private string _message = string.Empty;

    public Func<Task<IReadOnlyList<string>?>>? SelectFilesRequested { get; set; }
    public Func<Task<string?>>? SelectOutputFolderRequested { get; set; }

    public bool HasImages => Images.Count > 0;
    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);
    public bool CanRemove => _selectedImages.Count > 0 || SelectedImage != null;
    public bool CanGenerate => !IsBusy && HasImages && Width > 0 && Height > 0 &&
        Directory.Exists(OutputFolderPath) && !string.IsNullOrWhiteSpace(OutputFileName);
    public string ImageCountText => string.Format(Images.Count == 1 ? Localization.Strings.ImageThumbnailerViewModel_One_image : Localization.Strings.ImageThumbnailerViewModel_Image_count, Images.Count);

    [RelayCommand]
    private async Task AddAsync()
    {
        if (SelectFilesRequested != null)
        {
            AddFiles(await SelectFilesRequested());
        }
    }

    public void AddFiles(IEnumerable<string>? files)
    {
        if (files == null)
        {
            return;
        }

        foreach (string file in files.Where(File.Exists))
        {
            Images.Add(file);
            if (string.IsNullOrWhiteSpace(OutputFolderPath))
            {
                OutputFolderPath = Path.GetDirectoryName(file) ?? string.Empty;
            }
        }

        NotifyCollectionChanged();
    }

    public void SetSelectedImages(IEnumerable<string> files)
    {
        _selectedImages.Clear();
        foreach (string file in files)
        {
            _selectedImages.Add(file);
        }
        OnPropertyChanged(nameof(CanRemove));
    }

    [RelayCommand]
    private void Remove()
    {
        string[] files = _selectedImages.Count > 0
            ? _selectedImages.ToArray()
            : SelectedImage == null ? [] : [SelectedImage];

        foreach (string file in files)
        {
            Images.Remove(file);
        }

        _selectedImages.Clear();
        SelectedImage = null;
        NotifyCollectionChanged();
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

    [RelayCommand]
    private async Task GenerateAsync()
    {
        if (!CanGenerate)
        {
            return;
        }

        IsBusy = true;
        Message = Localization.Strings.ImageThumbnailerViewModel_Generating;
        try
        {
            List<string> outputFiles = await Task.Run(GenerateThumbnails);
            Message = string.Format(outputFiles.Count == 1 ? Localization.Strings.ImageThumbnailerViewModel_Generated_one : Localization.Strings.ImageThumbnailerViewModel_Generated_many, outputFiles.Count);
            if (outputFiles.Count > 0)
            {
                FileHelpers.OpenFolderWithFile(outputFiles[0]);
            }
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(ImageThumbnailerViewModel), "Failed to generate thumbnails.", ex);
            Message = string.Format(Localization.Strings.ImageThumbnailerViewModel_Generation_failed, ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedImageChanged(string? value) => OnPropertyChanged(nameof(CanRemove));
    partial void OnWidthChanged(decimal value) => NotifyStateChanged();
    partial void OnHeightChanged(decimal value) => NotifyStateChanged();
    partial void OnQualityChanged(decimal value) => NotifyStateChanged();
    partial void OnOutputFolderPathChanged(string value) => NotifyStateChanged();
    partial void OnOutputFileNameChanged(string value) => NotifyStateChanged();
    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(CanGenerate));

    private void NotifyCollectionChanged()
    {
        OnPropertyChanged(nameof(HasImages));
        OnPropertyChanged(nameof(ImageCountText));
        OnPropertyChanged(nameof(CanRemove));
        OnPropertyChanged(nameof(CanGenerate));
        Message = string.Empty;
    }

    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(CanGenerate));
        Message = string.Empty;
    }

    private List<string> GenerateThumbnails()
    {
        List<string> outputFiles = [];
        foreach (string filePath in Images.ToArray())
        {
            if (!File.Exists(filePath))
            {
                continue;
            }

            using Bitmap? source = ImageHelpers.LoadImage(filePath);
            if (source == null)
            {
                continue;
            }

            using Bitmap thumbnail = ImageHelpers.CreateThumbnail(source, (int)Width, (int)Height);
            using Bitmap output = ImageHelpers.FillBackground(thumbnail, Color.White);
            string sourceName = Path.GetFileNameWithoutExtension(filePath);
            string outputPath = Path.Combine(OutputFolderPath, OutputFileName.Replace("$filename", sourceName));
            outputPath = Path.ChangeExtension(outputPath, "jpg");
            ImageHelpers.SaveJPEG(output, outputPath, (int)Quality);
            outputFiles.Add(outputPath);
        }

        return outputFiles;
    }
}
