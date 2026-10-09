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
        target.ScreenRecordStartDelay = Capture.ScreenRecordStartDelay;
        target.ScreenRecordLastStartDelay = Capture.ScreenRecordLastStartDelay;
        target.ScreenRecordFixedDuration = Capture.ScreenRecordFixedDuration;
        target.ScreenRecordDuration = Capture.ScreenRecordDuration;
    }
}
