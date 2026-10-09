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

// SPDX-License-Identifier: GPL-3.0-or-later
using ShareX.HelpersLib;
using ShareX.Tools;

namespace ShareX;

/// <summary>Setup edits are saved together when the bar closes, including when recording is canceled.</summary>
internal sealed class ScreenRecorderBarSettings
{
    public TaskSettingsCapture Capture { get; }
    public MouseHighlighterOptions MouseHighlighter { get; }

    public ScreenRecorderBarSettings(TaskSettings settings)
    {
        Capture = settings.CaptureSettings.Copy();
        MouseHighlighter = settings.ToolsSettings.MouseHighlighterOptions.Copy();
    }

    public void Commit(TaskSettings recording)
    {
        ApplyCapture(recording.CaptureSettings);
        recording.ToolsSettings.MouseHighlighterOptions = MouseHighlighter.Copy();

        // Safe task settings are snapshots. Resolve the original task's overrides rather than
        // the snapshot's flags, which can have been inherited from the default task.
        TaskSettings origin = recording.TaskSettingsReference ?? recording;
        TaskSettingsCapture persistedCapture = origin.UseDefaultCaptureSettings
            ? ApplicationState.DefaultTaskSettings.CaptureSettings : origin.CaptureSettings;
        TaskSettingsTools persistedTools = origin.UseDefaultToolsSettings
            ? ApplicationState.DefaultTaskSettings.ToolsSettings : origin.ToolsSettings;
        ApplyCapture(persistedCapture);
        persistedTools.MouseHighlighterOptions = MouseHighlighter.Copy();
    }

    private void ApplyCapture(TaskSettingsCapture target)
    {
        target.ScreenRecordShowBar = Capture.ScreenRecordShowBar;
        target.ScreenRecordSystemAudio = Capture.ScreenRecordSystemAudio;
        target.ScreenRecordSystemAudioDeviceId = Capture.ScreenRecordSystemAudioDeviceId;
        target.ScreenRecordSystemAudioGain = Capture.ScreenRecordSystemAudioGain;
        target.ScreenRecordMicrophone = Capture.ScreenRecordMicrophone;
        target.ScreenRecordMicrophoneDeviceId = Capture.ScreenRecordMicrophoneDeviceId;
        target.ScreenRecordMicrophoneGain = Capture.ScreenRecordMicrophoneGain;
        target.ScreenRecordCamera = Capture.ScreenRecordCamera;
        target.ScreenRecordCameraDeviceId = Capture.ScreenRecordCameraDeviceId;
        target.ScreenRecordCameraResolution = Capture.ScreenRecordCameraResolution;
        target.ScreenRecordCameraFPS = Capture.ScreenRecordCameraFPS;
        target.ScreenRecordCameraPosition = Capture.ScreenRecordCameraPosition;
        target.ScreenRecordCameraShape = Capture.ScreenRecordCameraShape;
        target.ScreenRecordCameraWidthPercent = Capture.ScreenRecordCameraWidthPercent;
        target.ScreenRecordCameraMargin = Capture.ScreenRecordCameraMargin;
        target.ScreenRecordRequireHardwareEncoder = Capture.ScreenRecordRequireHardwareEncoder;
        target.ScreenRecordVideoBitrate = Capture.ScreenRecordVideoBitrate;
        target.ScreenRecordFPS = Capture.ScreenRecordFPS;
        target.ScreenRecordShowCursor = Capture.ScreenRecordShowCursor;
        target.ScreenRecordMouseHighlighter = Capture.ScreenRecordMouseHighlighter;
        target.ScreenRecordShowTimer = Capture.ScreenRecordShowTimer;
        target.ScreenRecordShowButtonLabels = Capture.ScreenRecordShowButtonLabels;
        target.ScreenRecordStartDelayEnabled = Capture.ScreenRecordStartDelayEnabled;
        target.ScreenRecordStartDelay = Capture.ScreenRecordStartDelay;
        target.ScreenRecordFixedDuration = Capture.ScreenRecordFixedDuration;
        target.ScreenRecordDuration = Capture.ScreenRecordDuration;
    }
}
