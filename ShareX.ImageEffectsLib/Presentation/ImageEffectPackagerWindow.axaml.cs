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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

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
            new FolderPickerOpenOptions { AllowMultiple = false, Title = Localization.Strings.ImageEffectPackagerWindow_Select_assets_folder });
        if (folders.Count > 0) _assetsFolder.Text = folders[0].Path.LocalPath;
    }

    private async void OnBrowsePackageClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Localization.Strings.ImageEffectPackagerWindow_Save_package,
            DefaultExtension = "sxie",
            SuggestedFileName = Path.GetFileName(_packageFile.Text),
            FileTypeChoices = [new FilePickerFileType(Localization.Strings.ImageEffectPackagerWindow_File_type) { Patterns = ["*.sxie"] }]
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
                _status.Text = Localization.Strings.ImageEffectPackagerWindow_Assets_must_be_inside;
                return;
            }
            if (string.IsNullOrWhiteSpace(output))
            {
                _status.Text = Localization.Strings.ImageEffectPackagerWindow_Choose_package_path;
                return;
            }
            if (File.Exists(output) && _overwrite.IsChecked != true)
            {
                _status.Text = Localization.Strings.ImageEffectPackagerWindow_Already_exists;
                return;
            }

            string result = ImageEffectPackager.Package(output, _json, assets);
            if (!string.IsNullOrEmpty(result) && File.Exists(result))
            {
                FileHelpers.OpenFolderWithFile(result);
                _status.Text = Localization.Strings.ImageEffectPackagerWindow_Created_successfully;
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
