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
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormsDialogResult = System.Windows.Forms.DialogResult;
using FormsMessageBox = System.Windows.Forms.MessageBox;
using FormsMessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using FormsMessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

namespace ShareX;

public partial class CustomUploaderSettingsWindow : Window
{
    private static readonly (string Token, string Description)[] InputTokens =
    [
        ("{input}", "Text or URL input"),
        ("{filename}", "Uploaded file name"),
        ("{random:input1|input2}", "Random selection"),
        ("{select:input1|input2}", "User selection"),
        ("{inputbox:title|default_value}", "User text input"),
        ("{base64:input}", "Base64 encoding")
    ];

    private static readonly (string Token, string Description)[] OutputTokens =
    [
        ("{response}", "Response text"),
        ("{responseurl}", "Response or redirection URL"),
        ("{header:header_name}", "Response header"),
        ("{json:path}", "JSONPath response value"),
        ("{xml:path}", "XPath response value"),
        ("{regex:pattern|group}", "Regular expression result"),
        ("{filename}", "Uploaded file name"),
        ("{random:input1|input2}", "Random selection"),
        ("{select:input1|input2}", "User selection"),
        ("{inputbox:title|default_value}", "User text input"),
        ("{outputbox:title|text}", "Display output text"),
        ("{base64:input}", "Base64 encoding")
    ];

    private CustomUploaderSettingsViewModel? _viewModel;

    public CustomUploaderSettingsWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
    }

    public CustomUploaderSettingsWindow(UploadersConfig config) : this()
    {
        _viewModel = new CustomUploaderSettingsViewModel(config);
        DataContext = _viewModel;
        Opened += (_, _) => AttachSyntaxMenus();
    }

    public void Refresh(bool selectLast) => _viewModel?.Reload(selectLast);

    private void OnNewClick(object? sender, RoutedEventArgs e) => _viewModel?.NewUploader();
    private void OnDuplicateClick(object? sender, RoutedEventArgs e) => _viewModel?.DuplicateSelected();
    private void OnRemoveClick(object? sender, RoutedEventArgs e) => _viewModel?.RemoveSelected();

    private void OnGuideClick(object? sender, RoutedEventArgs e) => URLHelpers.OpenURL(Links.DocsCustomUploader);

    private async void OnImportClick(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import custom uploaders",
            AllowMultiple = true,
            FileTypeFilter = [new FilePickerFileType("ShareX custom uploader") { Patterns = ["*.sxcu"] }]
        });
        ImportPaths(files.Select(x => x.TryGetLocalPath()).Where(x => !string.IsNullOrEmpty(x)).Cast<string>());
    }

    private async void OnExportClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel?.SelectedUploader == null) return;
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export custom uploader",
            SuggestedFileName = _viewModel.SelectedUploader.Model.GetFileName(),
            DefaultExtension = "sxcu",
            FileTypeChoices = [new FilePickerFileType("ShareX custom uploader") { Patterns = ["*.sxcu"] }]
        });
        string? path = file?.TryGetLocalPath();
        if (string.IsNullOrEmpty(path)) return;
        string? error = _viewModel.ExportSelected(path);
        if (!string.IsNullOrEmpty(error)) _viewModel.StatusMessage = error;
    }

    private async void OnExportAllClick(object? sender, RoutedEventArgs e)
    {
        string? folder = await PickFolderAsync("Export all custom uploaders");
        if (!string.IsNullOrEmpty(folder)) _viewModel?.ExportAll(folder);
    }

    private async void OnUpdateFolderClick(object? sender, RoutedEventArgs e)
    {
        string? folder = await PickFolderAsync("Update custom uploader folder");
        if (!string.IsNullOrEmpty(folder)) _viewModel?.UpdateFolder(folder);
    }

    private void OnClearClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel == null) return;
        FormsDialogResult result = FormsMessageBox.Show(
            "Remove all custom uploaders?",
            "ShareX - Confirmation",
            FormsMessageBoxButtons.YesNo,
            FormsMessageBoxIcon.Question);
        if (result == FormsDialogResult.Yes) _viewModel.Clear();
    }

    private void OnBeautifyClick(object? sender, RoutedEventArgs e) => FormatData(Newtonsoft.Json.Formatting.Indented);
    private void OnMinifyClick(object? sender, RoutedEventArgs e) => FormatData(Newtonsoft.Json.Formatting.None);

    private void FormatData(Newtonsoft.Json.Formatting formatting)
    {
        if (_viewModel?.SelectedUploader == null) return;
        try
        {
            _viewModel.SelectedUploader.FormatData(formatting);
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            _viewModel.StatusMessage = exception.Message;
        }
    }

    private void OnSyntaxTestClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel?.SelectedUploader == null) return;
        using CustomUploaderSyntaxTestForm form = new(_viewModel.LastResult?.ResponseInfo, _viewModel.SelectedUploader.URL);
        form.ShowDialog(Program.MainForm);
    }

    private async void OnTestImageClick(object? sender, RoutedEventArgs e) =>
        await RunTestAsync(CustomUploaderDestinationType.ImageUploader, _viewModel?.CustomImageUploaderSelected ?? -1);

    private async void OnTestTextClick(object? sender, RoutedEventArgs e)
    {
        string? text = await PromptForTextAsync();
        if (text != null) await RunTestAsync(CustomUploaderDestinationType.TextUploader, _viewModel?.CustomTextUploaderSelected ?? -1, text);
    }

    private async void OnTestFileClick(object? sender, RoutedEventArgs e) =>
        await RunTestAsync(CustomUploaderDestinationType.FileUploader, _viewModel?.CustomFileUploaderSelected ?? -1);

    private async void OnTestURLShortenerClick(object? sender, RoutedEventArgs e) =>
        await RunTestAsync(CustomUploaderDestinationType.URLShortener, _viewModel?.CustomURLShortenerSelected ?? -1);

    private async void OnTestURLSharingClick(object? sender, RoutedEventArgs e) =>
        await RunTestAsync(CustomUploaderDestinationType.URLSharingService, _viewModel?.CustomURLSharingServiceSelected ?? -1);

    private async Task RunTestAsync(CustomUploaderDestinationType type, int index, string? text = null)
    {
        if (_viewModel == null) return;
        UploadResult? result = await _viewModel.TestAsync(type, index, text);
        if (result != null) ResponseForm.ShowInstance(result);
    }

    private async Task<string?> PromptForTextAsync()
    {
        TextBox input = new()
        {
            AcceptsReturn = true,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            MinHeight = 130,
            Text = "ShareX text upload test"
        };
        Window dialog = new()
        {
            Title = "ShareX - Text upload test",
            Width = 520,
            Height = 250,
            MinWidth = 420,
            MinHeight = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            RequestedThemeVariant = ThemeManager.GetCurrentTheme()
        };
        Button upload = new() { Content = "Upload", MinWidth = 80 };
        Button cancel = new() { Content = "Cancel", MinWidth = 80 };
        upload.Click += (_, _) => dialog.Close(input.Text ?? string.Empty);
        cancel.Click += (_, _) => dialog.Close(null);
        dialog.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(12),
            Spacing = 8,
            Children =
            {
                new TextBlock { Text = "Text to upload:" },
                input,
                new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    Spacing = 6,
                    Children = { upload, cancel }
                }
            }
        };
        return await dialog.ShowDialog<string?>(this);
    }

    private async Task<string?> PickFolderAsync(string title)
    {
        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });
        return folders.FirstOrDefault()?.TryGetLocalPath();
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        bool supported = e.DataTransfer.TryGetFiles()?.Any(x =>
            x.TryGetLocalPath()?.EndsWith(".sxcu", StringComparison.OrdinalIgnoreCase) == true) == true;
        e.DragEffects = supported ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        string[] paths = e.DataTransfer.TryGetFiles()?
            .Select(x => x.TryGetLocalPath())
            .Where(x => !string.IsNullOrEmpty(x) && x.EndsWith(".sxcu", StringComparison.OrdinalIgnoreCase))
            .Cast<string>()
            .ToArray() ?? [];
        ImportPaths(paths);
        e.Handled = true;
    }

    private void ImportPaths(IEnumerable<string> paths)
    {
        if (_viewModel?.ImportFiles(paths) > 0) AttachSyntaxMenus();
    }

    private void AttachSyntaxMenus()
    {
        foreach (TextBox textBox in this.GetLogicalDescendants().OfType<TextBox>().Where(x => x.Classes.Contains("syntax-input")))
        {
            AttachSyntaxMenu(textBox, InputTokens);
        }

        foreach (TextBox textBox in new[] { ResultURLBox, ThumbnailURLBox, DeletionURLBox, ErrorMessageBox })
        {
            AttachSyntaxMenu(textBox, OutputTokens);
        }
    }

    private static void AttachSyntaxMenu(TextBox textBox, IEnumerable<(string Token, string Description)> tokens)
    {
        ContextMenu menu = new();
        foreach ((string token, string description) in tokens)
        {
            MenuItem item = new() { Header = token };
            ToolTip.SetTip(item, description);
            item.Click += (_, _) => InsertToken(textBox, token);
            menu.Items.Add(item);
        }
        textBox.ContextMenu = menu;
    }

    private static void InsertToken(TextBox textBox, string token)
    {
        string text = textBox.Text ?? string.Empty;
        int start = Math.Min(textBox.SelectionStart, textBox.SelectionEnd).Clamp(0, text.Length);
        int end = Math.Max(textBox.SelectionStart, textBox.SelectionEnd).Clamp(start, text.Length);
        textBox.Text = text.Remove(start, end - start).Insert(start, token);
        textBox.CaretIndex = start + token.Length;
        textBox.Focus();
    }
}
