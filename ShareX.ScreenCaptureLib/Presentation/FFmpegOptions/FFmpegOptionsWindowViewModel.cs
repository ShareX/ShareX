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
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib;

public sealed class FFmpegOptionsWindowViewModel
{
    private readonly bool _isRecording;
    private readonly Func<FeatureSupport> _getSupport;

    public FFmpegOptionsWindowViewModel(bool isRecording, Func<FeatureSupport>? getSupport = null)
    {
        _isRecording = isRecording;
        _getSupport = getSupport ?? GetRecordingSupport;
    }

    public static FeatureSupport CurrentRecordingSupport => ForRecording(GetRecordingSupport());

    // File conversion does not need the desktop recording or capture services.
    public FeatureSupport Support => _isRecording ? ForRecording(_getSupport()) : FeatureSupport.Supported;

    public bool TryChange(Action change)
    {
        if (!Support.IsSupported) return false;
        change();
        return true;
    }

    public async Task<bool> TryReadAsync<T>(Func<Task<T>> read, Action<T> apply)
    {
        if (!Support.IsSupported) return false;
        T result = await read();
        if (!Support.IsSupported) return false;
        apply(result);
        return true;
    }

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
