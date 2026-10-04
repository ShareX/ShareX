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

#nullable enable

using ShareX.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib;

public sealed class FFmpegOptionsWindowViewModel
{
    private readonly bool _isRecording;
    private readonly Func<FeatureSupport> _getSupport;
    private readonly Func<RecordingDeviceAction, FeatureSupport> _getDeviceActionSupport;

    public bool IsClosed { get; private set; }
    public bool IsReadingDevices { get; private set; }

    public FFmpegOptionsWindowViewModel(bool isRecording, Func<FeatureSupport>? getSupport = null,
        Func<RecordingDeviceAction, FeatureSupport>? getDeviceActionSupport = null)
    {
        _isRecording = isRecording;
        _getSupport = getSupport ?? GetRecordingSupport;
        _getDeviceActionSupport = getDeviceActionSupport ?? GetCurrentDeviceActionSupport;
    }

    public static FeatureSupport CurrentRecordingSupport => ForRecording(GetRecordingSupport());

    // File conversion does not need the desktop recording or capture services.
    public FeatureSupport Support => _isRecording ? ForRecording(_getSupport()) : FeatureSupport.Supported;

    public bool TryChange(Action change)
    {
        if (IsClosed || !Support.IsSupported) return false;
        change();
        return true;
    }

    public async Task<bool> TryReadAsync<T>(Func<Task<T>> read, Action<T> apply)
    {
        if (IsClosed || !Support.IsSupported) return false;
        T result = await read();
        if (IsClosed || !Support.IsSupported) return false;
        apply(result);
        return true;
    }

    public void Close() => IsClosed = true;

    public FeatureSupport GetDeviceActionSupport(RecordingDeviceAction action) => Support.IsSupported
        ? _getDeviceActionSupport(action) : Support;

    public bool TryDeviceAction(RecordingDeviceAction action, Action run)
    {
        if (IsClosed || !GetDeviceActionSupport(action).IsSupported) return false;
        run();
        return true;
    }

    public async Task<bool> TryReadDevicesAsync<T>(Func<Task<T>> read, Action<T> apply)
    {
        if (IsClosed || IsReadingDevices || !GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices).IsSupported) return false;
        IsReadingDevices = true;
        try
        {
            T result = await read();
            if (IsClosed || !GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices).IsSupported) return false;
            apply(result);
            return true;
        }
        finally
        {
            IsReadingDevices = false;
        }
    }

    public FFmpegCaptureDevice ResolveSelectedSource(List<FFmpegCaptureDevice> sources, string savedSource, string defaultSource)
    {
        FFmpegCaptureDevice? source = sources.FirstOrDefault(x => string.Equals(x.Value, savedSource, StringComparison.OrdinalIgnoreCase));
        if (source != null) return source;
        // An unavailable enumeration cannot establish whether the saved device exists.
        if (!GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices).IsSupported)
        {
            source = new FFmpegCaptureDevice(savedSource, savedSource);
            sources.Add(source);
            return source;
        }
        return sources.First(x => string.Equals(x.Value, defaultSource, StringComparison.OrdinalIgnoreCase));
    }

    private static FeatureSupport GetCurrentDeviceActionSupport(RecordingDeviceAction action) => PlatformServices.IsInitialized
        ? PlatformServices.Current.ScreenRecording.GetDeviceActionSupport(action)
        : FeatureSupport.NotSupported(Localization.Strings.FFmpegOptionsWindow_RecordingUnavailable);

    private static FeatureSupport GetRecordingSupport()
    {
        if (!PlatformServices.IsInitialized) return FeatureSupport.NotSupported(Localization.Strings.FFmpegOptionsWindow_RecordingUnavailable);
        IPlatformServices platform = PlatformServices.Current;
        return platform.ScreenRecording.Support.IsSupported && platform.ScreenCapture.Support.IsSupported
            ? FeatureSupport.Supported : FeatureSupport.NotSupported(Localization.Strings.FFmpegOptionsWindow_RecordingUnavailable);
    }

    private static FeatureSupport ForRecording(FeatureSupport support) => support.IsSupported
        ? FeatureSupport.Supported : FeatureSupport.NotSupported(Localization.Strings.FFmpegOptionsWindow_RecordingUnavailable);
}
