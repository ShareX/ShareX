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

using ShareX.Platform;
using ShareX.Tools.Localization;

namespace ShareX.Tools;

public sealed class MouseHighlighterWindowViewModel
{
    private readonly Func<FeatureSupport> _getSupport;
    private readonly Func<bool> _isManuallyActive;
    private readonly Func<bool> _isRecordingActive;

    public MouseHighlighterWindowViewModel(Func<FeatureSupport>? getSupport = null,
        Func<bool>? isManuallyActive = null, Func<bool>? isRecordingActive = null)
    {
        _getSupport = getSupport ?? GetPlatformSupport;
        _isManuallyActive = isManuallyActive ?? (() => MouseHighlighterManager.IsManuallyActive);
        _isRecordingActive = isRecordingActive ?? (() => MouseHighlighterManager.IsRecordingActive);
    }

    public static FeatureSupport CurrentSettingsSupport => ForSettings(GetPlatformSupport());
    public static FeatureSupport CurrentToggleSupport => MouseHighlighterManager.IsManuallyActive
        ? FeatureSupport.Supported : CurrentSettingsSupport;

    public FeatureSupport SettingsSupport => ForSettings(_getSupport());
    public FeatureSupport ToggleSupport => _isManuallyActive() ? FeatureSupport.Supported : SettingsSupport;

    public string ToggleText => _isRecordingActive()
        ? (_isManuallyActive() ? Strings.MouseHighlighter_StopAfterRecording : Strings.MouseHighlighter_KeepAfterRecording)
        : (_isManuallyActive() ? Strings.MouseHighlighter_Stop : Strings.MouseHighlighter_Start);

    public bool TryToggle(Action<bool> setManuallyActive)
    {
        bool wasActive = _isManuallyActive();
        // An existing highlighter must remain stoppable after its desktop capability disappears.
        if (!wasActive && !SettingsSupport.IsSupported) return false;
        setManuallyActive(!wasActive);
        return true;
    }

    public bool TryChangeSettings(Action change)
    {
        if (!SettingsSupport.IsSupported) return false;
        change();
        return true;
    }

    private static FeatureSupport GetPlatformSupport() => PlatformServices.IsInitialized
        ? MouseHighlighterManager.Support : FeatureSupport.NotSupported(Strings.MouseHighlighter_Unavailable);

    private static FeatureSupport ForSettings(FeatureSupport support) => support.IsSupported
        ? FeatureSupport.Supported : FeatureSupport.NotSupported(Strings.MouseHighlighter_Unavailable);
}
