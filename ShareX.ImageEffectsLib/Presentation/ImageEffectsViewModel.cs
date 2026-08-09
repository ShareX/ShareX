#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Serialization;
using ShareX.AvaloniaUI.Theming;
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
    private const string WindowTitlePrefix = "ShareX - Image effects";
    private readonly List<ImageEffectPreset> _presets;
    private readonly ImageEffectsCallbacks _callbacks;
    private readonly ISerializationBinder _serializationBinder = new ImageEffectsSerializationBinder();
    private readonly DispatcherTimer _previewTimer;
    private System.Drawing.Bitmap? _sourceImage;
    private byte[]? _previewImageData;
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
    private string _windowTitle = WindowTitlePrefix;

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
        _previewTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _previewTimer.Tick += async (_, _) =>
        {
            _previewTimer.Stop();
            await UpdatePreviewAsync();
        };

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

    public async Task InitializeAsync()
    {
        _previewTimer.Stop();
        await UpdatePreviewAsync();
    }

    private void QueuePreview()
    {
        _previewVersion++;
        _previewTimer.Stop();
        _previewTimer.Start();
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
            ImageEffectPreset preset = SelectedPreset.Preset.Copy();
            System.Drawing.Bitmap? result = await Task.Run(() => preset.ApplyEffects(source));
            using (result)
            {
                if (result == null || version != _previewVersion || _disposed) return;
                using MemoryStream stream = new();
                result.Save(stream, ImageFormat.Png);
                byte[] previewImageData = stream.ToArray();
                using MemoryStream previewStream = new(previewImageData);
                Avalonia.Media.Imaging.Bitmap preview = new(previewStream);
                Preview?.Dispose();
                Preview = preview;
                _previewImageData = previewImageData;
                WindowTitle = $"{WindowTitlePrefix} - {result.Width} × {result.Height} - {timer.ElapsedMilliseconds} ms";
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
        ImageEffectPreset preset = SelectedPreset.Preset.Copy();
        return await Task.Run(() => preset.ApplyEffects(source));
    }

    public byte[]? GetPreviewImageData() => _previewImageData == null ? null : (byte[])_previewImageData.Clone();

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

        System.Drawing.Color backgroundMain = GetThemeColor("ShareX.Color.Background.Main", System.Drawing.Color.FromArgb(39, 39, 39));
        System.Drawing.Color backgroundPanel = GetThemeColor("ShareX.Color.Background.Panel", System.Drawing.Color.FromArgb(36, 36, 36));
        System.Drawing.Color accentStart = GetThemeColor("ShareX.Color.Accent.Start", System.Drawing.Color.FromArgb(62, 131, 242));
        System.Drawing.Color accentEnd = GetThemeColor("ShareX.Color.Accent.End", System.Drawing.Color.FromArgb(57, 117, 213));
        System.Drawing.Color accentForeground = GetThemeColor("ShareX.Color.Accent.Foreground", System.Drawing.Color.FromArgb(216, 218, 219));

        using LinearGradientBrush background = new(new Rectangle(0, 0, bitmap.Width, bitmap.Height), backgroundMain, backgroundPanel, 35f);
        graphics.FillRectangle(background, 0, 0, bitmap.Width, bitmap.Height);

        const float shapeSize = 200f;
        RectangleF shapeBounds = new(
            (bitmap.Width - shapeSize) / 2f,
            (bitmap.Height - shapeSize) / 2f,
            shapeSize,
            shapeSize);
        PointF[] outerHexagon = CreateHexagon(shapeBounds, 0f);
        float shapeCenterX = shapeBounds.Left + shapeBounds.Width / 2f;
        float shapeCenterY = shapeBounds.Top + shapeBounds.Height / 2f;
        PointF shapeCenter = new(shapeCenterX, shapeCenterY);
        PointF[] topFace = [outerHexagon[0], outerHexagon[1], shapeCenter, outerHexagon[5]];
        PointF[] leftFace = [outerHexagon[5], shapeCenter, outerHexagon[3], outerHexagon[4]];
        PointF[] rightFace = [outerHexagon[1], outerHexagon[2], outerHexagon[3], shapeCenter];
        using SolidBrush topFaceBrush = new(BlendColors(accentStart, accentForeground, 0.16f));
        using SolidBrush leftFaceBrush = new(accentStart);
        using SolidBrush rightFaceBrush = new(accentEnd);
        using Pen faceOutline = new(System.Drawing.Color.FromArgb(100, accentForeground), 2f);
        graphics.FillPolygon(topFaceBrush, topFace);
        graphics.FillPolygon(leftFaceBrush, leftFace);
        graphics.FillPolygon(rightFaceBrush, rightFace);
        graphics.DrawPolygon(faceOutline, outerHexagon);
        graphics.DrawLine(faceOutline, outerHexagon[3], shapeCenter);
        graphics.DrawLine(faceOutline, outerHexagon[1], shapeCenter);
        graphics.DrawLine(faceOutline, outerHexagon[5], shapeCenter);

        return bitmap;
    }

    private static System.Drawing.Color BlendColors(System.Drawing.Color first, System.Drawing.Color second, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        float inverse = 1f - amount;
        return System.Drawing.Color.FromArgb(
            (int)(first.A * inverse + second.A * amount),
            (int)(first.R * inverse + second.R * amount),
            (int)(first.G * inverse + second.G * amount),
            (int)(first.B * inverse + second.B * amount));
    }

    private static PointF[] CreateHexagon(RectangleF bounds, float inset)
    {
        bounds.Inflate(-inset, -inset);
        float quarterHeight = bounds.Height / 4f;
        float centerX = bounds.Left + bounds.Width / 2f;

        return
        [
            new(centerX, bounds.Top),
            new(bounds.Right, bounds.Top + quarterHeight),
            new(bounds.Right, bounds.Bottom - quarterHeight),
            new(centerX, bounds.Bottom),
            new(bounds.Left, bounds.Bottom - quarterHeight),
            new(bounds.Left, bounds.Top + quarterHeight)
        ];
    }

    private static System.Drawing.Color GetThemeColor(string resourceKey, System.Drawing.Color fallback)
    {
        if (Application.Current?.TryFindResource(resourceKey, ThemeManager.GetCurrentTheme(), out object? resource) == true &&
            resource is Avalonia.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        return fallback;
    }

    public void Dispose()
    {
        _disposed = true;
        _previewTimer.Stop();
        _sourceImage?.Dispose();
        Preview?.Dispose();
        _previewImageData = null;
    }
}
