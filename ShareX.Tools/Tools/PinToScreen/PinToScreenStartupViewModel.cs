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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.Platform;

namespace ShareX.Tools;

public sealed partial class PinToScreenStartupViewModel : ViewModelBase
{
    private readonly PinToScreenServices _services;
    private readonly Func<FeatureSupport> _getCaptureSupport;
    private readonly Func<Task> _hideDelay;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    [NotifyPropertyChangedFor(nameof(CanCaptureRegion))]
    [NotifyCanExecuteChangedFor(nameof(CaptureRegionCommand))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    public bool IsIdle => !IsBusy;
    public bool CanCaptureRegion => IsIdle && _getCaptureSupport().IsSupported;
    public string? CaptureUnavailableReason => _getCaptureSupport().IsSupported ? null :
        Localization.Strings.PinToScreenStartupViewModel_CaptureUnavailable;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public Action<PinToScreenSource>? SourceSelected { get; set; }
    public Action? RegionCaptureStarted { get; set; }
    public Action? RegionCaptureFinished { get; set; }

    public PinToScreenStartupViewModel(PinToScreenServices services, Func<FeatureSupport>? getCaptureSupport = null,
        Func<Task>? hideDelay = null)
    {
        _services = services;
        _getCaptureSupport = getCaptureSupport ?? (() => PlatformServices.IsInitialized
            ? PlatformServices.Current.ScreenCapture.Support
            : FeatureSupport.NotSupported(Localization.Strings.PinToScreenStartupViewModel_CaptureUnavailable));
        _hideDelay = hideDelay ?? (() => Task.Delay(200));
    }

    [RelayCommand(CanExecute = nameof(CanCaptureRegion))]
    private Task CaptureRegionAsync() => SelectAsync(_services.CaptureRegionAsync,
        Localization.Strings.PinToScreenStartupViewModel_No_region_selected, captureRegion: true);

    [RelayCommand]
    private Task FromClipboardAsync() => SelectAsync(_services.GetClipboardImageAsync, Localization.Strings.PinToScreenStartupViewModel_Clipboard_no_image);

    [RelayCommand]
    private Task FromFileAsync() => SelectAsync(_services.SelectImageFileAsync, Localization.Strings.PinToScreenStartupViewModel_No_image_selected);

    private async Task SelectAsync(Func<Task<PinToScreenSource?>> selector, string emptyMessage, bool captureRegion = false)
    {
        if (IsBusy)
        {
            return;
        }

        if (captureRegion && !_getCaptureSupport().IsSupported)
        {
            ErrorMessage = Localization.Strings.PinToScreenStartupViewModel_CaptureUnavailable;
            NotifyCaptureAvailability();
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        bool regionCaptureStarted = false;

        try
        {
            if (captureRegion)
            {
                regionCaptureStarted = true;
                RegionCaptureStarted?.Invoke();
                await _hideDelay();
                if (!_getCaptureSupport().IsSupported)
                {
                    ErrorMessage = Localization.Strings.PinToScreenStartupViewModel_CaptureUnavailable;
                    return;
                }
            }

            PinToScreenSource? source = await selector();
            if (source == null || source.ImageData.Length == 0)
            {
                ErrorMessage = emptyMessage;
                return;
            }

            SourceSelected?.Invoke(source);
        }
        catch (Exception ex)
        {
            ErrorMessage = string.Format(Localization.Strings.PinToScreenStartupViewModel_Unable_load_image, ex.Message);
            ToolsDiagnostics.ReportWarning(nameof(PinToScreenStartupViewModel), "Unable to select an image to pin.", ex);
        }
        finally
        {
            IsBusy = false;
            if (regionCaptureStarted)
            {
                RegionCaptureFinished?.Invoke();
            }
            NotifyCaptureAvailability();
        }
    }

    private void NotifyCaptureAvailability()
    {
        OnPropertyChanged(nameof(CanCaptureRegion));
        OnPropertyChanged(nameof(CaptureUnavailableReason));
        CaptureRegionCommand.NotifyCanExecuteChanged();
    }
}
