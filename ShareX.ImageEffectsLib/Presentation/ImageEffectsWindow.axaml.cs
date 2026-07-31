#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Newtonsoft.Json.Serialization;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;

namespace ShareX.ImageEffectsLib;

public partial class ImageEffectsWindow : Window
{
    private readonly ISerializationBinder _serializationBinder = new ImageEffectsSerializationBinder();
    private ImageEffectOptionsPanel _effectOptionsPanel = null!;
    public ImageEffectsViewModel ViewModel { get; }
    public bool Accepted { get; private set; }

    public ImageEffectsWindow() : this(null, [], 0, ImageEffectsWindowMode.Presets)
    {
    }

    public ImageEffectsWindow(System.Drawing.Bitmap? sourceImage, List<ImageEffectPreset> presets, int selectedPresetIndex,
        ImageEffectsWindowMode mode, ImageEffectsCallbacks? callbacks = null, string? filePath = null)
    {
        ViewModel = new ImageEffectsViewModel(sourceImage, presets, selectedPresetIndex, mode, callbacks, filePath);
        DataContext = ViewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _effectOptionsPanel = this.FindControl<ImageEffectOptionsPanel>("EffectOptionsPanel")!;
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);

        ViewModel.AddEffectRequested = AddEffectAsync;
        ViewModel.PackagePresetRequested = PackagePresetAsync;
        ViewModel.CloseRequested = accepted => { Accepted = accepted; Close(); };
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        UpdateEffectOptions();
        Opened += async (_, _) => await ViewModel.InitializeAsync();
        Closed += (_, _) =>
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            ViewModel.Dispose();
        };
        KeyDown += OnWindowKeyDown;
    }

    public void ImportPreset(string json) => ViewModel.ImportPreset(json);

    private async void AddEffectAsync()
    {
        ImageEffectPickerWindow picker = new();
        if (await picker.ShowDialog<bool>(this) && picker.SelectedDefinition != null)
        {
            ViewModel.AddEffect(picker.SelectedDefinition.Create());
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImageEffectsViewModel.SelectedEffect))
        {
            UpdateEffectOptions();
        }
    }

    private void UpdateEffectOptions()
    {
        ImageEffectItemViewModel? item = ViewModel.SelectedEffect;
        _effectOptionsPanel.SetEffect(item?.Effect, item == null ? null : () => ViewModel.EffectOptionsChanged(item));
    }

    private async void PackagePresetAsync(ImageEffectPreset preset)
    {
        if (string.IsNullOrWhiteSpace(preset.Name))
        {
            return;
        }

        try
        {
            string json = JsonHelpers.SerializeToString(preset, serializationBinder: _serializationBinder);
            string effectsFolder = HelpersOptions.ShareXSpecialFolders["ShareXImageEffects"];
            await new ImageEffectPackagerWindow(json, preset.Name, effectsFolder).ShowDialog(this);
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
        }
    }

    private async void OnClearEffectsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (await new ImageEffectsConfirmationWindow("Clear all effects from the selected preset?").ShowDialog<bool>(this))
        {
            ViewModel.ClearEffectsCommand.Execute(null);
        }
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete && ViewModel.HasSelectedEffect)
        {
            ViewModel.RemoveEffectCommand.Execute(null);
            e.Handled = true;
        }
        else if (e.Key == Key.F5)
        {
            ViewModel.RefreshPreviewCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        bool supported = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().Any(x =>
            x.Path.LocalPath.EndsWith(".sxie", StringComparison.OrdinalIgnoreCase) ||
            (ViewModel.IsToolMode && FileHelpers.IsImageFile(x.Path.LocalPath))) == true;
        e.DragEffects = supported ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        foreach (IStorageFile file in e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>() ?? [])
        {
            string path = file.Path.LocalPath;
            if (ViewModel.IsToolMode && FileHelpers.IsImageFile(path))
            {
                ViewModel.LoadImageFile(path);
                break;
            }
            if (!path.EndsWith(".sxie", StringComparison.OrdinalIgnoreCase)) continue;
            try
            {
                string json = ImageEffectPackager.ExtractPackage(path, HelpersOptions.ShareXSpecialFolders["ShareXImageEffects"]);
                if (!string.IsNullOrWhiteSpace(json)) ViewModel.ImportPreset(json);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
        }
        e.Handled = true;
    }
}
