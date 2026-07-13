#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;

namespace ShareX.ImageEffectsLib;

public partial class ImageEffectPackagerWindow : Window
{
    private readonly string _json;
    private readonly string _effectsFolder;
    private TextBox _assetsFolder = null!;
    private TextBox _packageFile = null!;
    private ToggleSwitch _overwrite = null!;
    private TextBlock _status = null!;

    public ImageEffectPackagerWindow() : this("{}", "Preset", Environment.CurrentDirectory)
    {
    }

    public ImageEffectPackagerWindow(string json, string name, string effectsFolder)
    {
        _json = json;
        _effectsFolder = effectsFolder;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _assetsFolder = this.FindControl<TextBox>("AssetsFolderTextBox")!;
        _packageFile = this.FindControl<TextBox>("PackageFileTextBox")!;
        _overwrite = this.FindControl<ToggleSwitch>("OverwriteToggle")!;
        _status = this.FindControl<TextBlock>("StatusText")!;
        _assetsFolder.Text = Path.Combine(effectsFolder, name);
        _packageFile.Text = Path.Combine(effectsFolder, name + ".sxie");
    }

    private async void OnBrowseAssetsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions { AllowMultiple = false, Title = "Select effect assets folder" });
        if (folders.Count > 0) _assetsFolder.Text = folders[0].Path.LocalPath;
    }

    private async void OnBrowsePackageClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save image effect package",
            DefaultExtension = "sxie",
            SuggestedFileName = Path.GetFileName(_packageFile.Text),
            FileTypeChoices = [new FilePickerFileType("ShareX image effect") { Patterns = ["*.sxie"] }]
        });
        if (file != null) _packageFile.Text = file.Path.LocalPath;
    }

    private void OnOpenFolderClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => FileHelpers.OpenFolder(_effectsFolder);

    private void OnPackageClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            string assets = _assetsFolder.Text?.Trim() ?? string.Empty;
            string output = _packageFile.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(assets) && !assets.StartsWith(_effectsFolder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                _status.Text = "The assets folder must be inside the ShareX image effects folder.";
                return;
            }
            if (string.IsNullOrWhiteSpace(output))
            {
                _status.Text = "Choose a package file path.";
                return;
            }
            if (File.Exists(output) && _overwrite.IsChecked != true)
            {
                _status.Text = "A package already exists at this path. Enable overwrite to replace it.";
                return;
            }

            string result = ImageEffectPackager.Package(output, _json, assets);
            if (!string.IsNullOrEmpty(result) && File.Exists(result))
            {
                FileHelpers.OpenFolderWithFile(result);
                _status.Text = "Package created successfully.";
            }
        }
        catch (Exception ex)
        {
            _status.Text = ex.Message;
            DebugHelper.WriteException(ex);
        }
    }

    private void OnCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();
}
