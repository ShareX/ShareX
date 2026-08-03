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

namespace ShareX.Tools;

public sealed partial class QRCodeViewModel : ViewModelBase, IDisposable
{
    private const int MaximumContentBytes = 2952;
    private readonly QRCodeServices _services;
    private readonly QRCodeWindowOptions _options;
    private int _availableSize = 400;
    private int _generationVersion;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasValidContent))]
    private string _text = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EffectiveSize))]
    private decimal _qRCodeSize;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasQRCode))]
    private Bitmap? _previewImage;

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public QRCodeViewModel(QRCodeServices services, QRCodeWindowOptions options)
    {
        _services = services;
        _options = options;
        _text = options.InitialText ?? string.Empty;
    }

    public Func<QRCodeScanMode, Task<string[]?>>? CaptureScanRequested { get; set; }
    public Func<Task<string[]?>>? ImageFileScanRequested { get; set; }
    public Func<string, int, Task<bool>>? SaveRequested { get; set; }

    public bool HasQRCode => PreviewImage != null;
    public bool HasValidContent => !string.IsNullOrEmpty(Text) && Encoding.UTF8.GetByteCount(Text) <= MaximumContentBytes;
    public int EffectiveSize => Math.Max(QRCodeSize > 0 ? (int)QRCodeSize : _availableSize, 64);
    public async Task InitializeAsync()
    {
        await RegenerateAsync();

        if (!string.IsNullOrWhiteSpace(_options.InitialImageFilePath))
        {
            await ScanAsync(QRCodeScanMode.ImageFile, _options.InitialImageFilePath);
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
            if (QRCodeSize == 0)
            {
                _ = RegenerateAsync();
            }
        }
    }

    partial void OnTextChanged(string value)
    {
        _ = RegenerateAsync();
    }

    partial void OnQRCodeSizeChanged(decimal value)
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
                : Localization.Strings.QRCodeViewModel_Content_too_large;
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
                StatusText = Localization.Strings.QRCodeViewModel_Unable_generate;
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
                StatusText = string.Format(Localization.Strings.QRCodeViewModel_Generation_failed, ex.Message);
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
            StatusText = Localization.Strings.QRCodeViewModel_QRCode_saved;
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
    private Task ScanScreenAsync() => ScanCaptureAsync(QRCodeScanMode.Screen);

    [RelayCommand]
    private Task ScanRegionAsync() => ScanCaptureAsync(QRCodeScanMode.Region);

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

    private async Task ScanCaptureAsync(QRCodeScanMode mode)
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

    private async Task ScanAsync(QRCodeScanMode mode, string? filePath)
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
            ? string.Format(results.Length == 1 ? Localization.Strings.QRCodeViewModel_Decoded_one : Localization.Strings.QRCodeViewModel_Decoded_many, results.Length)
            : Localization.Strings.QRCodeViewModel_No_QR_code;
        _services.PlayNotificationSound?.Invoke();
        return Task.CompletedTask;
    }

    public async Task<string[]?> ScanWithServiceAsync(QRCodeScanMode mode, string? filePath = null)
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
