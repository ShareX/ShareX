// SPDX-License-Identifier: GPL-3.0-or-later
using ShareX.ScreenRecordingLib.Native;
using SharpGen.Runtime;
using System.Runtime.InteropServices;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib;

/// <summary>A Windows audio endpoint. Id can be passed to RecordingOptions.MicrophoneDeviceId or SystemAudioDeviceId.</summary>
public sealed record AudioCaptureDevice(string Id, string Name);

public static unsafe class AudioCaptureDevices
{
    /// <summary>Lists active microphones and other audio input endpoints using the Windows MMDevice API.</summary>
    public static IReadOnlyList<AudioCaptureDevice> GetMicrophones() => GetDevices(DataFlow.Capture);

    /// <summary>Lists active audio output endpoints that can be recorded using WASAPI loopback.</summary>
    public static IReadOnlyList<AudioCaptureDevice> GetSystemAudioDevices() => GetDevices(DataFlow.Render);

    /// <summary>Resolves the default endpoint using the same roles as WASAPI recording, without opening a stream.</summary>
    public static AudioCaptureDevice? GetDefaultSystemAudioDevice() => GetDefaultDevice(DataFlow.Render);

    public static AudioCaptureDevice? GetDefaultMicrophone() => GetDefaultDevice(DataFlow.Capture);

    private static AudioCaptureDevice? GetDefaultDevice(DataFlow flow)
    {
        HRESULT initialized = NativeMethods.CoInitializeEx(null, COINIT.COINIT_MULTITHREADED);
        if (initialized.Value != unchecked((int)0x80010106)) initialized.ThrowOnFailure();
        try
        {
            using IMMDeviceEnumerator enumerator = new();
            IMMDevice endpoint;
            try { endpoint = enumerator.GetDefaultAudioEndpoint(flow, flow == DataFlow.Render ? Role.Multimedia : Role.Communications); }
            catch (SharpGenException) when (flow == DataFlow.Capture)
            {
                endpoint = enumerator.GetDefaultAudioEndpoint(flow, Role.Multimedia);
            }
            using (endpoint) return new(endpoint.Id, endpoint.FriendlyName);
        }
        catch (Exception ex) when (ex is COMException or SharpGenException) { return null; }
        finally { if (initialized.Succeeded) NativeMethods.CoUninitialize(); }
    }

    private static IReadOnlyList<AudioCaptureDevice> GetDevices(DataFlow flow)
    {
        HRESULT initialized = NativeMethods.CoInitializeEx(null, COINIT.COINIT_MULTITHREADED);
        // A UI thread may already be initialized as STA. Enumeration supports either apartment.
        if (initialized.Value != unchecked((int)0x80010106)) initialized.ThrowOnFailure(); // RPC_E_CHANGED_MODE
        try
        {
            using IMMDeviceEnumerator enumerator = new();
            IReadOnlyList<IMMDevice> endpoints = enumerator.EnumAudioEndpoints(flow, DeviceStates.Active);
            try
            {
                List<AudioCaptureDevice> devices = new();
                foreach (IMMDevice endpoint in endpoints)
                {
                    try { devices.Add(new(endpoint.Id, endpoint.FriendlyName)); }
                    catch (Exception ex) when (ex is COMException or SharpGenException)
                    {
                        // An endpoint can disappear between enumeration and reading its properties.
                    }
                }
                return devices;
            }
            finally { foreach (IMMDevice endpoint in endpoints) endpoint.Dispose(); }
        }
        finally { if (initialized.Succeeded) NativeMethods.CoUninitialize(); }
    }
}
