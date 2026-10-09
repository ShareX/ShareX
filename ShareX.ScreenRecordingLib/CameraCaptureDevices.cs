// SPDX-License-Identifier: GPL-3.0-or-later
using ShareX.ScreenRecordingLib.Native;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib;

/// <summary>A Windows camera. Id is its persistent Media Foundation symbolic link.</summary>
public sealed record CameraCaptureDevice(string Id, string Name);

public static unsafe class CameraCaptureDevices
{
    public static IReadOnlyList<CameraCaptureDevice> GetCameras()
    {
        HRESULT initialized = NativeMethods.CoInitializeEx(null, COINIT.COINIT_MULTITHREADED);
        if (initialized.Value != unchecked((int)0x80010106)) initialized.ThrowOnFailure(); // UI may already be STA.
        bool started = false;
        try
        {
            MediaFactory.MFStartup(true).CheckError();
            started = true;
            using var devices = MediaFactory.MFEnumVideoDeviceSources();
            List<CameraCaptureDevice> result = new();
            foreach (IMFActivate device in devices)
                result.Add(new(device.GetString(CaptureDeviceAttributeKeys.SourceTypeVidcapSymbolicLink),
                    device.GetString(CaptureDeviceAttributeKeys.FriendlyName)));
            return result;
        }
        finally
        {
            if (started) MediaFactory.MFShutdown();
            if (initialized.Succeeded) NativeMethods.CoUninitialize();
        }
    }
}

public enum CameraOverlayPosition { TopLeft, TopRight, BottomLeft, BottomRight }
public enum CameraOverlayShape { Rectangle, Circle }
public enum CameraCaptureResolution { Size640x480, Size1280x720, Size1920x1080 }

/// <summary>The camera's negotiated mode; actual delivery depends on the device.</summary>
public sealed record CameraCaptureInfo(string Name, int Width, int Height, double FramesPerSecond, string PixelFormat);
