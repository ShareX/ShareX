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

using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.Tools.Infrastructure;
using System.Text;

namespace ShareX.Tools.Features.QrCode;

public sealed partial class QrCodeViewModel : ViewModelBase, IDisposable
{
    private const int MaximumContentBytes = 2952;
    private readonly QrCodeServices _services;
    private readonly QrCodeWindowOptions _options;
    private int _availableSize = 400;
    private int _generationVersion;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasValidContent))]
    private string _text = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EffectiveSize))]
    private decimal _qrCodeSize;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasQrCode))]
    private Bitmap? _previewImage;

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public QrCodeViewModel(QrCodeServices services, QrCodeWindowOptions options)
    {
        _services = services;
        _options = options;
        _text = options.InitialText ?? string.Empty;
    }

    public Func<QrCodeScanMode, Task<string[]?>>? CaptureScanRequested { get; set; }
    public Func<Task<string[]?>>? ImageFileScanRequested { get; set; }
    public Func<string, int, Task<bool>>? SaveRequested { get; set; }

    public bool HasQrCode => PreviewImage != null;
    public bool HasValidContent => !string.IsNullOrEmpty(Text) && Encoding.UTF8.GetByteCount(Text) <= MaximumContentBytes;
    public int EffectiveSize => Math.Max(QrCodeSize > 0 ? (int)QrCodeSize : _availableSize, 64);
    public async Task InitializeAsync()
    {
        await RegenerateAsync();

        if (!string.IsNullOrWhiteSpace(_options.InitialImageFilePath))
        {
            await ScanAsync(QrCodeScanMode.ImageFile, _options.InitialImageFilePath);
        }
        else if (_options.InitialScanMode.HasValue)
        {
            await ScanCaptureAsync(_options.InitialScanMode.Value);
        }
    }

    public void SetAvailableSize(double width, double height)
    {
        int size = Math.Max((int)Math.Floor(Math.Min(width, height)), 64);
        if (_availableSize != size)
        {
            _availableSize = size;
            OnPropertyChanged(nameof(EffectiveSize));
            if (QrCodeSize == 0)
            {
                _ = RegenerateAsync();
            }
        }
    }

    partial void OnTextChanged(string value)
    {
        _ = RegenerateAsync();
    }

    partial void OnQrCodeSizeChanged(decimal value)
    {
        _ = RegenerateAsync();
    }

    private async Task RegenerateAsync()
    {
        int version = ++_generationVersion;

        if (!HasValidContent)
        {
            PreviewImage?.Dispose();
            PreviewImage = null;
            StatusText = string.IsNullOrEmpty(Text) || Encoding.UTF8.GetByteCount(Text) <= MaximumContentBytes
                ? string.Empty
                : "The content is too large for a QR code.";
            return;
        }

        try
        {
            byte[]? png = await _services.GeneratePreviewAsync(Text, EffectiveSize);
            if (version != _generationVersion)
            {
                return;
            }

            if (png == null)
            {
                StatusText = "Unable to generate a QR code from this content.";
                return;
            }

            using MemoryStream stream = new(png);
            Bitmap bitmap = new(stream);
            PreviewImage?.Dispose();
            PreviewImage = bitmap;
            StatusText = string.Empty;
        }
        catch (Exception ex)
        {
            if (version == _generationVersion)
            {
                StatusText = $"QR code generation failed: {ex.Message}";
            }
        }
    }

    [RelayCommand]
    private void CopyImage()
    {
        if (HasValidContent)
        {
            _services.CopyImage(Text, EffectiveSize);
        }
    }

    [RelayCommand]
    private async Task SaveImageAsync()
    {
        if (HasValidContent && SaveRequested != null && await SaveRequested(Text, EffectiveSize))
        {
            StatusText = "QR code saved.";
        }
    }

    [RelayCommand]
    private void UploadImage()
    {
        if (HasValidContent)
        {
            _services.UploadImage(Text, EffectiveSize);
        }
    }

    [RelayCommand]
    private Task ScanScreenAsync() => ScanCaptureAsync(QrCodeScanMode.Screen);

    [RelayCommand]
    private Task ScanRegionAsync() => ScanCaptureAsync(QrCodeScanMode.Region);

    [RelayCommand]
    private async Task ScanImageFileAsync()
    {
        if (ImageFileScanRequested == null || IsScanning)
        {
            return;
        }

        IsScanning = true;
        try
        {
            await ApplyScanResultsAsync(await ImageFileScanRequested());
        }
        finally
        {
            IsScanning = false;
        }
    }

    private async Task ScanCaptureAsync(QrCodeScanMode mode)
    {
        if (CaptureScanRequested == null || IsScanning)
        {
            return;
        }

        IsScanning = true;
        try
        {
            await ApplyScanResultsAsync(await CaptureScanRequested(mode));
        }
        finally
        {
            IsScanning = false;
        }
    }

    private async Task ScanAsync(QrCodeScanMode mode, string? filePath)
    {
        IsScanning = true;
        try
        {
            await ApplyScanResultsAsync(await _services.ScanAsync(mode, filePath));
        }
        finally
        {
            IsScanning = false;
        }
    }

    private Task ApplyScanResultsAsync(string[]? results)
    {
        Text = results is { Length: > 0 }
            ? string.Join(Environment.NewLine + Environment.NewLine, results)
            : string.Empty;
        StatusText = results is { Length: > 0 }
            ? $"Decoded {results.Length} value{(results.Length == 1 ? string.Empty : "s")}."
            : "No QR code was found.";
        _services.PlayNotificationSound?.Invoke();
        return Task.CompletedTask;
    }

    public async Task<string[]?> ScanWithServiceAsync(QrCodeScanMode mode, string? filePath = null)
    {
        return await _services.ScanAsync(mode, filePath);
    }

    public async Task SaveWithServiceAsync(string text, int size, string filePath)
    {
        await _services.SaveAsync(text, size, filePath);
    }

    public void Dispose()
    {
        PreviewImage?.Dispose();
    }
}
