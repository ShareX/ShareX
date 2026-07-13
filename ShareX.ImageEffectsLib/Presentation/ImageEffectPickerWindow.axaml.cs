#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.ImageEffectsLib;

public partial class ImageEffectPickerWindow : Window
{
    private TextBox _searchBox = null!;
    private ListBox _effectsList = null!;
    public ImageEffectDefinition? SelectedDefinition { get; private set; }

    public ImageEffectPickerWindow()
    {
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _searchBox = this.FindControl<TextBox>("SearchBox")!;
        _effectsList = this.FindControl<ListBox>("EffectsList")!;
        ApplyFilter();
        Opened += (_, _) => _searchBox.Focus();
        KeyUp += (_, e) => { if (e.Key == Key.Escape) Close(false); };
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs e) => ApplyFilter();

    private void ApplyFilter()
    {
        string query = _searchBox?.Text?.Trim() ?? string.Empty;
        _effectsList.ItemsSource = ImageEffectCatalog.All.Where(x => string.IsNullOrEmpty(query) ||
            x.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
            x.Category.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToArray();
        _effectsList.SelectedIndex = _effectsList.ItemCount > 0 ? 0 : -1;
    }

    private void OnEffectDoubleTapped(object? sender, TappedEventArgs e) => Accept();
    private void OnAddClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Accept();
    private void OnCancelClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(false);

    private void Accept()
    {
        if (_effectsList.SelectedItem is ImageEffectDefinition definition)
        {
            SelectedDefinition = definition;
            Close(true);
        }
    }
}
