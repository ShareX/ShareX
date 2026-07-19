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

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.IO;
using System.Linq;

namespace ShareX;

public partial class ApplicationSettingsWindow : Window
{
    private ApplicationSettingsViewModel ViewModel => (ApplicationSettingsViewModel)DataContext!;

    public ApplicationSettingsWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        DataContext = new ApplicationSettingsViewModel();
        Opened += (_, _) => Activate();
        Closed += (_, _) => ViewModel.Dispose();
    }

    private void OnRestartClick(object? sender, RoutedEventArgs e) => ViewModel.Restart();
    private void OnEditQuickTaskMenuClick(object? sender, RoutedEventArgs e) => ViewModel.EditQuickTaskMenu();
    private async void OnCheckDevBuildClick(object? sender, RoutedEventArgs e) => await ViewModel.CheckDevBuildAsync();
    private void OnOpenChromeExtensionClick(object? sender, RoutedEventArgs e) => ViewModel.OpenChromeExtensionPage();
    private void OnOpenFirefoxAddonClick(object? sender, RoutedEventArgs e) => ViewModel.OpenFirefoxAddonPage();
    private void OnOpenPersonalFolderClick(object? sender, RoutedEventArgs e) => ViewModel.OpenPersonalFolder();
    private void OnOpenScreenshotsFolderClick(object? sender, RoutedEventArgs e) => ViewModel.OpenScreenshotsFolder();
    private void OnResetThumbnailSizeClick(object? sender, RoutedEventArgs e) => ViewModel.ResetThumbnailSize();
    private void OnAddClipboardFormatClick(object? sender, RoutedEventArgs e) => ViewModel.AddClipboardFormat();
    private void OnRemoveClipboardFormatClick(object? sender, RoutedEventArgs e) => ViewModel.RemoveSelectedClipboardFormat();
    private void OnMoveImageUploaderUpClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedImageUploader(-1);
    private void OnMoveImageUploaderDownClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedImageUploader(1);
    private void OnMoveTextUploaderUpClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedTextUploader(-1);
    private void OnMoveTextUploaderDownClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedTextUploader(1);
    private void OnMoveFileUploaderUpClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedFileUploader(-1);
    private void OnMoveFileUploaderDownClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedFileUploader(1);
    private void OnImagePrintSettingsClick(object? sender, RoutedEventArgs e) => ViewModel.ShowImagePrintSettings();
    private async void OnResetSettingsClick(object? sender, RoutedEventArgs e) => await ViewModel.ResetAsync();

    private async void OnBrowsePersonalFolderClick(object? sender, RoutedEventArgs e)
    {
        string? path = await PickFolderAsync("Choose ShareX personal folder path");
        if (!string.IsNullOrEmpty(path))
        {
            ViewModel.PersonalFolderPath = path;
        }
    }

    private async void OnBrowseScreenshotsFolderClick(object? sender, RoutedEventArgs e)
    {
        string? path = await PickFolderAsync("Choose screenshots folder path");
        if (!string.IsNullOrEmpty(path))
        {
            ViewModel.CustomScreenshotsPath = path;
        }
    }

    private async System.Threading.Tasks.Task<string?> PickFolderAsync(string title)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });
        return folders.FirstOrDefault()?.TryGetLocalPath();
    }

    private async void OnExportClick(object? sender, RoutedEventArgs e)
    {
        string machineName = FileHelpers.SanitizeFileName(Environment.MachineName.ToLowerInvariant());
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export ShareX backup",
            SuggestedFileName = $"ShareX-{Helpers.GetApplicationVersion()}-{machineName}-backup.sxb",
            DefaultExtension = "sxb",
            FileTypeChoices =
            [
                new FilePickerFileType("ShareX backup") { Patterns = ["*.sxb"] },
                FilePickerFileTypes.All
            ]
        });

        string? path = file?.TryGetLocalPath();
        if (!string.IsNullOrEmpty(path))
        {
            await ViewModel.ExportAsync(path);
        }
    }

    private async void OnImportClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import ShareX backup",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("ShareX backup") { Patterns = ["*.sxb"] }]
        });

        string? path = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            await ViewModel.ImportAsync(path);
        }
    }
}
