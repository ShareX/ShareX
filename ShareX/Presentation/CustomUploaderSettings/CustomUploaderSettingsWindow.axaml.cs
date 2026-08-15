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
using ShareX.Localization;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;
using MessageBoxResult = ShareX.AvaloniaUI.DialogResult;

namespace ShareX;

public partial class CustomUploaderSettingsWindow : Window
{
    private static readonly (string Token, string Description)[] InputTokens =
    [
        ("{input}", Strings.CustomUploaderSettingsWindow_TextOrURLInput),
        ("{filename}", Strings.CustomUploaderSettingsWindow_UploadedFileName),
        ("{random:input1|input2}", Strings.CustomUploaderSettingsWindow_RandomSelection),
        ("{select:input1|input2}", Strings.CustomUploaderSettingsWindow_UserSelection),
        ("{inputbox:title|default_value}", Strings.CustomUploaderSettingsWindow_UserTextInput),
        ("{base64:input}", Strings.CustomUploaderSettingsWindow_Base64Encoding)
    ];

    private static readonly (string Token, string Description)[] OutputTokens =
    [
        ("{response}", Strings.CustomUploaderSettingsWindow_ResponseText),
        ("{responseurl}", Strings.CustomUploaderSettingsWindow_ResponseOrRedirectionURL),
        ("{header:header_name}", Strings.CustomUploaderSettingsWindow_ResponseHeader),
        ("{json:path}", Strings.CustomUploaderSettingsWindow_JSONPathResponseValue),
        ("{xml:path}", Strings.CustomUploaderSettingsWindow_XPathResponseValue),
        ("{regex:pattern|group}", Strings.CustomUploaderSettingsWindow_RegularExpressionResult),
        ("{filename}", Strings.CustomUploaderSettingsWindow_UploadedFileName),
        ("{random:input1|input2}", Strings.CustomUploaderSettingsWindow_RandomSelection),
        ("{select:input1|input2}", Strings.CustomUploaderSettingsWindow_UserSelection),
        ("{inputbox:title|default_value}", Strings.CustomUploaderSettingsWindow_UserTextInput),
        ("{outputbox:title|text}", Strings.CustomUploaderSettingsWindow_DisplayOutputText),
        ("{base64:input}", Strings.CustomUploaderSettingsWindow_Base64Encoding)
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
            Title = Strings.CustomUploaderSettingsWindow_ImportDialogTitle,
            AllowMultiple = true,
            FileTypeFilter = [new FilePickerFileType(Strings.CustomUploaderSettingsWindow_CustomUploaderFileType) { Patterns = ["*.sxcu"] }]
        });
        ImportPaths(files.Select(x => x.TryGetLocalPath()).Where(x => !string.IsNullOrEmpty(x)).Cast<string>());
    }

    private async void OnExportClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel?.SelectedUploader == null) return;
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Strings.CustomUploaderSettingsWindow_ExportDialogTitle,
            SuggestedFileName = _viewModel.SelectedUploader.Model.GetFileName(),
            DefaultExtension = "sxcu",
            FileTypeChoices = [new FilePickerFileType(Strings.CustomUploaderSettingsWindow_CustomUploaderFileType) { Patterns = ["*.sxcu"] }]
        });
        string? path = file?.TryGetLocalPath();
        if (string.IsNullOrEmpty(path)) return;
        string? error = _viewModel.ExportSelected(path);
        if (!string.IsNullOrEmpty(error)) _viewModel.StatusMessage = error;
    }

    private async void OnExportAllClick(object? sender, RoutedEventArgs e)
    {
        string? folder = await PickFolderAsync(Strings.CustomUploaderSettingsWindow_ExportAllDialogTitle);
        if (!string.IsNullOrEmpty(folder)) _viewModel?.ExportAll(folder);
    }

    private async void OnUpdateFolderClick(object? sender, RoutedEventArgs e)
    {
        string? folder = await PickFolderAsync(Strings.CustomUploaderSettingsWindow_UpdateFolderDialogTitle);
        if (!string.IsNullOrEmpty(folder)) _viewModel?.UpdateFolder(folder);
    }

    private void OnClearClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel == null) return;
        MessageBoxResult result = MessageBox.Show(
            Strings.CustomUploaderSettingsWindow_ClearConfirmation,
            Strings.CustomUploaderSettingsWindow_ConfirmationTitle,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (result == MessageBoxResult.Yes) _viewModel.Clear();
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
        _viewModel.OpenSyntaxTest();
        SyntaxTestExpressionBox.CaretIndex = SyntaxTestExpressionBox.Text?.Length ?? 0;
        SyntaxTestExpressionBox.Focus();
    }

    private void OnCloseSyntaxTestClick(object? sender, RoutedEventArgs e) => _viewModel?.CloseSyntaxTest();

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
        if (result != null) ResponseWindow.ShowInstance(result);
    }

    private async Task<string?> PromptForTextAsync()
    {
        TextBox input = new()
        {
            AcceptsReturn = true,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            MinHeight = 130,
            Text = Strings.CustomUploaderSettingsWindow_TextUploadSample
        };
        Window dialog = new()
        {
            Title = Strings.CustomUploaderSettingsWindow_TextUploadTestTitle,
            Width = 520,
            Height = 250,
            MinWidth = 420,
            MinHeight = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            RequestedThemeVariant = ThemeManager.GetCurrentTheme()
        };
        Button upload = new() { Content = Strings.CustomUploaderSettingsWindow_Upload, MinWidth = 80 };
        Button cancel = new() { Content = Strings.CustomUploaderSettingsWindow_Cancel, MinWidth = 80 };
        upload.Click += (_, _) => dialog.Close(input.Text ?? string.Empty);
        cancel.Click += (_, _) => dialog.Close(null);
        dialog.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(12),
            Spacing = 8,
            Children =
            {
                new TextBlock { Text = Strings.CustomUploaderSettingsWindow_TextToUpload },
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

        foreach (TextBox textBox in new[] { ResultURLBox, ThumbnailURLBox, DeletionURLBox, ErrorMessageBox, SyntaxTestExpressionBox })
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
