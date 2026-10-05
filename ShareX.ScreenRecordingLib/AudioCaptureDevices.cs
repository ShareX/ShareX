// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;
using ShareX.ScreenRecordingLib.Native;

namespace ShareX.ScreenRecordingLib;

/// <summary>A Windows audio capture endpoint. Id can be passed to RecordingOptions.MicrophoneDeviceId.</summary>
public sealed record AudioCaptureDevice(string Id, string Name);

public static unsafe class AudioCaptureDevices
{
    /// <summary>Lists active microphones and other audio input endpoints using the Windows MMDevice API.</summary>
    public static IReadOnlyList<AudioCaptureDevice> GetMicrophones()
    {
        HRESULT initialized = NativeMethods.CoInitializeEx(null, COINIT.COINIT_MULTITHREADED);
        // A UI thread may already be initialized as STA. Enumeration supports either apartment.
        if (initialized.Value != unchecked((int)0x80010106)) initialized.ThrowOnFailure(); // RPC_E_CHANGED_MODE
        try
        {
            Guid clsid = new("bcde0395-e52f-467c-8e3d-c4579291692e"), iid = typeof(IMMDeviceEnumerator).GUID;
            void* rawEnumerator;
            NativeMethods.CoCreateInstance(&clsid, null, CLSCTX.CLSCTX_INPROC_SERVER, &iid, &rawEnumerator).ThrowOnFailure();
            using ComPtr<IMMDeviceEnumerator> enumerator = new((IMMDeviceEnumerator*)rawEnumerator);
            IMMDeviceCollection* rawCollection;
            enumerator.Pointer->EnumAudioEndpoints(EDataFlow.eCapture, 1, &rawCollection).ThrowOnFailure(); // DEVICE_STATE_ACTIVE
            using ComPtr<IMMDeviceCollection> collection = new(rawCollection);
            uint count;
            collection.Pointer->GetCount(&count).ThrowOnFailure();
            List<AudioCaptureDevice> devices = new();
            for (uint index = 0; index < count; index++)
            {
                try
                {
                    IMMDevice* rawDevice;
                    collection.Pointer->Item(index, &rawDevice).ThrowOnFailure();
                    using ComPtr<IMMDevice> device = new(rawDevice);
                    char* rawId = null;
                    PROPVARIANT name = default;
                    try
                    {
                        device.Pointer->GetId(&rawId).ThrowOnFailure();
                        string id = new(rawId);
                        IPropertyStore* rawProperties;
                        device.Pointer->OpenPropertyStore(0, &rawProperties).ThrowOnFailure(); // STGM_READ
                        using ComPtr<IPropertyStore> properties = new(rawProperties);
                        PROPERTYKEY key = new(new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), 14); // PKEY_Device_FriendlyName
                        properties.Pointer->GetValue(&key, &name).ThrowOnFailure();
                        string title = name.vt == 31 && name.pwszVal != null ? new(name.pwszVal) : id; // VT_LPWSTR
                        devices.Add(new(id, title));
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem((nint)rawId);
                        NativeMethods.PropVariantClear(&name);
                    }
                }
                catch (COMException)
                {
                    // An endpoint can disappear between enumeration and reading its properties.
                }
            }
            return devices;
        }
        finally
        {
            if (initialized.Succeeded) NativeMethods.CoUninitialize();
        }
    }
}
