#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Serialization;
using ShareX.HelpersLib;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ShareX.ImageEffectsLib;

public sealed partial class ImageEffectItemViewModel : ObservableObject
{
    public ImageEffect Effect { get; }
    private readonly Action _changed;

    [ObservableProperty]
    private bool _enabled;

    public string DisplayName => Effect.ToString();

    public ImageEffectItemViewModel(ImageEffect effect, Action changed)
    {
        Effect = effect;
        _changed = changed;
        _enabled = effect.Enabled;
    }

    partial void OnEnabledChanged(bool value)
    {
        Effect.Enabled = value;
        _changed();
    }

    public void RefreshName() => OnPropertyChanged(nameof(DisplayName));
}

public sealed partial class ImageEffectPresetItemViewModel : ObservableObject
{
    public ImageEffectPreset Preset { get; }
    public string DisplayName => string.IsNullOrWhiteSpace(Preset.Name) ? "Unnamed preset" : Preset.Name;

    public ImageEffectPresetItemViewModel(ImageEffectPreset preset) => Preset = preset;
    public void RefreshName() => OnPropertyChanged(nameof(DisplayName));
}

public sealed partial class ImageEffectsViewModel : ObservableObject, IDisposable
{
    private readonly List<ImageEffectPreset> _presets;
    private readonly ImageEffectsCallbacks _callbacks;
    private readonly ISerializationBinder _serializationBinder = new ImageEffectsSerializationBinder();
    private System.Drawing.Bitmap? _sourceImage;
    private int _previewVersion;
    private bool _disposed;

    public ObservableCollection<ImageEffectPresetItemViewModel> Presets { get; } = [];
    public ObservableCollection<ImageEffectItemViewModel> Effects { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedPreset))]
    private ImageEffectPresetItemViewModel? _selectedPreset;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedEffect))]
    private ImageEffectItemViewModel? _selectedEffect;

    [ObservableProperty]
    private Avalonia.Media.Imaging.Bitmap? _preview;

    [ObservableProperty]
    private string _previewInfo = "Preparing preview...";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _filePath = string.Empty;

    public ImageEffectsWindowMode Mode { get; }
    public bool IsToolMode => Mode == ImageEffectsWindowMode.Tool;
    public bool IsEditorMode => Mode == ImageEffectsWindowMode.Editor;
    public bool IsPresetsMode => Mode == ImageEffectsWindowMode.Presets;
    public bool HasSelectedPreset => SelectedPreset != null;
    public bool HasSelectedEffect => SelectedEffect != null;
    public bool IsIdle => !IsBusy;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public string CloseButtonText => IsEditorMode ? "Cancel" : "Close";
    public int SelectedPresetIndex => Math.Max(0, SelectedPreset == null ? 0 : Presets.IndexOf(SelectedPreset));

    public Action? AddEffectRequested { get; set; }
    public Action<ImageEffectItemViewModel>? EditEffectRequested { get; set; }
    public Action<ImageEffectPreset>? PackagePresetRequested { get; set; }
    public Action<bool>? CloseRequested { get; set; }

    public string PresetName
    {
        get => SelectedPreset?.Preset.Name ?? string.Empty;
        set
        {
            if (SelectedPreset != null && SelectedPreset.Preset.Name != value)
            {
                SelectedPreset.Preset.Name = value ?? string.Empty;
                SelectedPreset.RefreshName();
                OnPropertyChanged();
            }
        }
    }

    public string EffectName
    {
        get => SelectedEffect?.Effect.Name ?? string.Empty;
        set
        {
            if (SelectedEffect != null && SelectedEffect.Effect.Name != value)
            {
                SelectedEffect.Effect.Name = value ?? string.Empty;
                SelectedEffect.RefreshName();
                OnPropertyChanged();
            }
        }
    }

    public ImageEffectsViewModel(System.Drawing.Bitmap? sourceImage, List<ImageEffectPreset> presets, int selectedPresetIndex,
        ImageEffectsWindowMode mode, ImageEffectsCallbacks? callbacks = null, string? filePath = null)
    {
        _sourceImage = sourceImage != null ? (System.Drawing.Bitmap)sourceImage.Clone() : CreateSampleImage();
        _presets = presets;
        _callbacks = callbacks ?? new ImageEffectsCallbacks();
        Mode = mode;
        FilePath = filePath ?? string.Empty;

        if (_presets.Count == 0)
        {
            _presets.Add(new ImageEffectPreset());
        }

        foreach (ImageEffectPreset preset in _presets)
        {
            Presets.Add(new ImageEffectPresetItemViewModel(preset));
        }

        SelectedPreset = Presets[Math.Clamp(selectedPresetIndex, 0, Presets.Count - 1)];
    }

    partial void OnSelectedPresetChanged(ImageEffectPresetItemViewModel? value)
    {
        Effects.Clear();
        if (value != null)
        {
            foreach (ImageEffect effect in value.Preset.Effects)
            {
                Effects.Add(new ImageEffectItemViewModel(effect, QueuePreview));
            }
        }
        SelectedEffect = Effects.FirstOrDefault();
        OnPropertyChanged(nameof(PresetName));
        QueuePreview();
    }

    partial void OnSelectedEffectChanged(ImageEffectItemViewModel? value)
    {
        OnPropertyChanged(nameof(EffectName));
    }

    [RelayCommand]
    private void NewPreset()
    {
        ImageEffectPreset preset = new();
        _presets.Add(preset);
        ImageEffectPresetItemViewModel item = new(preset);
        Presets.Add(item);
        SelectedPreset = item;
    }

    [RelayCommand]
    private void RemovePreset()
    {
        if (SelectedPreset == null) return;
        int index = Presets.IndexOf(SelectedPreset);
        _presets.RemoveAt(index);
        Presets.RemoveAt(index);
        if (Presets.Count == 0)
        {
            NewPreset();
        }
        else
        {
            SelectedPreset = Presets[Math.Min(index, Presets.Count - 1)];
        }
    }

    [RelayCommand]
    private void DuplicatePreset()
    {
        if (SelectedPreset == null) return;
        ImageEffectPreset copy = SelectedPreset.Preset.Copy();
        _presets.Add(copy);
        ImageEffectPresetItemViewModel item = new(copy);
        Presets.Add(item);
        SelectedPreset = item;
    }

    [RelayCommand] private void MovePresetUp() => MovePreset(-1);
    [RelayCommand] private void MovePresetDown() => MovePreset(1);

    private void MovePreset(int direction)
    {
        if (SelectedPreset == null) return;
        int oldIndex = Presets.IndexOf(SelectedPreset);
        int newIndex = oldIndex + direction;
        if (newIndex < 0 || newIndex >= Presets.Count) return;
        Presets.Move(oldIndex, newIndex);
        _presets.Move(oldIndex, newIndex);
    }

    [RelayCommand] private void AddEffect() => AddEffectRequested?.Invoke();

    public void AddEffect(ImageEffect effect)
    {
        if (SelectedPreset == null) return;
        int index = SelectedEffect == null ? Effects.Count : Effects.IndexOf(SelectedEffect) + 1;
        SelectedPreset.Preset.Effects.Insert(index, effect);
        ImageEffectItemViewModel item = new(effect, QueuePreview);
        Effects.Insert(index, item);
        SelectedEffect = item;
        QueuePreview();
    }

    [RelayCommand]
    private void RemoveEffect()
    {
        if (SelectedPreset == null || SelectedEffect == null) return;
        int index = Effects.IndexOf(SelectedEffect);
        SelectedPreset.Preset.Effects.RemoveAt(index);
        Effects.RemoveAt(index);
        SelectedEffect = Effects.Count == 0 ? null : Effects[Math.Min(index, Effects.Count - 1)];
        QueuePreview();
    }

    [RelayCommand]
    private void DuplicateEffect()
    {
        if (SelectedEffect != null) AddEffect(SelectedEffect.Effect.Copy());
    }

    [RelayCommand]
    private void ClearEffects()
    {
        if (SelectedPreset == null) return;
        SelectedPreset.Preset.Effects.Clear();
        Effects.Clear();
        SelectedEffect = null;
        QueuePreview();
    }

    [RelayCommand] private void MoveEffectUp() => MoveEffect(-1);
    [RelayCommand] private void MoveEffectDown() => MoveEffect(1);

    private void MoveEffect(int direction)
    {
        if (SelectedPreset == null || SelectedEffect == null) return;
        int oldIndex = Effects.IndexOf(SelectedEffect);
        int newIndex = oldIndex + direction;
        if (newIndex < 0 || newIndex >= Effects.Count) return;
        Effects.Move(oldIndex, newIndex);
        SelectedPreset.Preset.Effects.Move(oldIndex, newIndex);
        QueuePreview();
    }

    [RelayCommand]
    private void EditEffect()
    {
        if (SelectedEffect != null) EditEffectRequested?.Invoke(SelectedEffect);
    }

    [RelayCommand] private void RefreshPreview() => QueuePreview();

    [RelayCommand]
    private void LoadFile()
    {
        ReplaceSource(_callbacks.LoadImageFromFile?.Invoke());
    }

    [RelayCommand]
    private void LoadClipboard()
    {
        ReplaceSource(_callbacks.LoadImageFromClipboard?.Invoke());
    }

    private void ReplaceSource(ImageEffectsSource? source)
    {
        if (source == null) return;
        _sourceImage?.Dispose();
        _sourceImage = source.Image;
        FilePath = source.FilePath ?? string.Empty;
        QueuePreview();
    }

    public void LoadImageFile(string filePath)
    {
        System.Drawing.Bitmap image = ImageHelpers.LoadImage(filePath);
        if (image != null) ReplaceSource(new ImageEffectsSource(image, filePath));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (_callbacks.SaveImage == null) return;
        using System.Drawing.Bitmap? result = await ApplySelectedPresetAsync();
        if (result != null)
        {
            string? path = _callbacks.SaveImage(result, FilePath);
            if (!string.IsNullOrWhiteSpace(path)) FilePath = path;
        }
    }

    [RelayCommand]
    private async Task UploadAsync()
    {
        if (_callbacks.UploadImage == null) return;
        System.Drawing.Bitmap? result = await ApplySelectedPresetAsync();
        if (result != null) _callbacks.UploadImage(result);
    }

    [RelayCommand]
    private void PackagePreset()
    {
        if (SelectedPreset == null) return;
        if (string.IsNullOrWhiteSpace(SelectedPreset.Preset.Name))
        {
            ErrorMessage = "Enter a preset name before creating a package.";
            return;
        }
        ErrorMessage = string.Empty;
        PackagePresetRequested?.Invoke(SelectedPreset.Preset);
    }

    [RelayCommand] private void OpenHelp() => _callbacks.OpenImageEffectsPage?.Invoke();
    [RelayCommand] private void Accept() => CloseRequested?.Invoke(true);
    [RelayCommand] private void Cancel() => CloseRequested?.Invoke(false);

    public void EffectOptionsChanged(ImageEffectItemViewModel item)
    {
        item.RefreshName();
        QueuePreview();
    }

    public async Task InitializeAsync() => await UpdatePreviewAsync();

    private void QueuePreview()
    {
        int version = ++_previewVersion;
        Dispatcher.UIThread.Post(async () =>
        {
            if (version == _previewVersion) await UpdatePreviewAsync();
        });
    }

    private async Task UpdatePreviewAsync()
    {
        if (_sourceImage == null || SelectedPreset == null || _disposed) return;
        int version = ++_previewVersion;
        IsBusy = true;
        ErrorMessage = string.Empty;
        Stopwatch timer = Stopwatch.StartNew();

        try
        {
            using System.Drawing.Bitmap source = (System.Drawing.Bitmap)_sourceImage.Clone();
            System.Drawing.Bitmap? result = await Task.Run(() => SelectedPreset.Preset.ApplyEffects(source));
            using (result)
            {
                if (result == null || version != _previewVersion || _disposed) return;
                using MemoryStream stream = new();
                result.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                Avalonia.Media.Imaging.Bitmap preview = new(stream);
                Preview?.Dispose();
                Preview = preview;
                PreviewInfo = $"{result.Width} × {result.Height}  •  {timer.ElapsedMilliseconds} ms";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            DebugHelper.WriteException(ex);
        }
        finally
        {
            if (version == _previewVersion) IsBusy = false;
        }
    }

    private async Task<System.Drawing.Bitmap?> ApplySelectedPresetAsync()
    {
        if (_sourceImage == null || SelectedPreset == null) return null;
        using System.Drawing.Bitmap source = (System.Drawing.Bitmap)_sourceImage.Clone();
        return await Task.Run(() => SelectedPreset.Preset.ApplyEffects(source));
    }

    public void ImportPreset(string json)
    {
        try
        {
            ImageEffectPreset? preset = JsonHelpers.DeserializeFromString<ImageEffectPreset>(json, _serializationBinder);
            if (preset?.Effects == null || preset.Effects.Count == 0) return;
            SanitizeImportedPreset(preset);
            _presets.Add(preset);
            ImageEffectPresetItemViewModel item = new(preset);
            Presets.Add(item);
            SelectedPreset = item;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            DebugHelper.WriteException(ex);
        }
    }

    private static void SanitizeImportedPreset(ImageEffectPreset preset)
    {
        if (!HelpersOptions.ShareXSpecialFolders.TryGetValue("ShareXImageEffects", out string? folder)) return;
        foreach (ImageEffect effect in preset.Effects)
        {
            switch (effect)
            {
                case DrawImage draw when !IsAllowed(draw.ImageLocation, folder): draw.ImageLocation = string.Empty; break;
                case DrawBackgroundImage draw when !IsAllowed(draw.ImageFilePath, folder): draw.ImageFilePath = string.Empty; break;
                case DrawParticles draw when !IsAllowed(draw.ImageFolder, folder): draw.ImageFolder = string.Empty; break;
            }
        }
    }

    private static bool IsAllowed(string path, string folder) => string.IsNullOrEmpty(path) || ImageEffectPathHelpers.IsPathInFolder(path, folder);

    private static System.Drawing.Bitmap CreateSampleImage()
    {
        System.Drawing.Bitmap bitmap = new(720, 480);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        using LinearGradientBrush background = new(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            System.Drawing.Color.FromArgb(37, 44, 58), System.Drawing.Color.FromArgb(77, 107, 164), 35f);
        graphics.FillRectangle(background, 0, 0, bitmap.Width, bitmap.Height);
        using SolidBrush accent = new(System.Drawing.Color.FromArgb(220, 82, 160, 255));
        graphics.FillEllipse(accent, 90, 90, 220, 220);
        using Font font = new("Segoe UI", 42, FontStyle.Bold);
        graphics.DrawString("ShareX", font, Brushes.White, 350, 175);
        using Font caption = new("Segoe UI", 16);
        graphics.DrawString("Image effects preview", caption, Brushes.White, 354, 235);
        return bitmap;
    }

    public void Dispose()
    {
        _disposed = true;
        _sourceImage?.Dispose();
        Preview?.Dispose();
    }
}
