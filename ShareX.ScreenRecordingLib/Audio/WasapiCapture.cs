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
using ShareX.ScreenRecordingLib.Native;
using SharpGen.Runtime;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib.Audio;

/// <summary>WASAPI shared-mode loopback and microphone capture, on a dedicated event-driven MTA thread.</summary>
internal sealed unsafe class WasapiCapture : IDisposable
{
    private sealed class Endpoint : IDisposable
    {
        public required ComPtr<IAudioClient3> Client;
        public required ComPtr<IAudioCaptureClient> Capture;
        public required AutoResetEvent Ready;
        public required int Source;
        public required float Gain;
        public bool Started;
        public long NextPacketTime;
        public void Dispose()
        {
            if (Started) Client.Pointer->Stop();
            Capture.Dispose(); Client.Dispose(); Ready.Dispose();
        }
    }

    private readonly RecordingOptions options;
    private readonly AudioMixer mixer;
    private readonly Thread thread;
    private readonly ManualResetEvent stop = new(false);
    private readonly ManualResetEvent start = new(false);
    private readonly TaskCompletionSource prepared = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private Exception? failure;
    private int disposed;
    public Exception? Failure => Volatile.Read(ref failure);

    public WasapiCapture(RecordingOptions options, AudioMixer mixer)
    {
        this.options = options;
        this.mixer = mixer;
        thread = new Thread(Run) { IsBackground = true, Name = "ShareX WASAPI capture" };
        thread.Start();
        try { prepared.Task.GetAwaiter().GetResult(); }
        catch { Dispose(); throw; }
    }

    public void Start() => start.Set();

    private Endpoint Open(IMMDeviceEnumerator enumerator, bool loopback, string? id, float gain)
    {
        IMMDevice device;
        if (string.IsNullOrWhiteSpace(id))
        {
            try
            {
                try { device = enumerator.GetDefaultAudioEndpoint(loopback ? DataFlow.Render : DataFlow.Capture, loopback ? Role.Multimedia : Role.Communications); }
                catch (SharpGenException) when (!loopback) { device = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia); }
            }
            catch (SharpGenException ex)
            {
                throw new InvalidOperationException(loopback ? "Windows has no available default system audio output device." : "Windows has no available default microphone. Connect or enable a microphone, or turn off microphone recording.", ex);
            }
        }
        else device = enumerator.GetDevice(id);
        using IMMDevice ownedDevice = device;
        ComPtr<IAudioClient3> client = ActivateClient(ownedDevice);
        AutoResetEvent ready = new(false);
        try
        {
            // Ask Windows to perform channel mapping and high-quality resampling. No managed resampler or codec DLL.
            WAVEFORMATEX format = new()
            {
                wFormatTag = 3,
                nChannels = 2,
                nSamplesPerSec = 48_000,
                nAvgBytesPerSec = 384_000,
                nBlockAlign = 8,
                wBitsPerSample = 32,
                cbSize = 0
            };
            const uint eventCallback = 0x00040000, loopbackFlag = 0x00020000, autoConvert = 0x80000000, srcQuality = 0x08000000;
            uint flags = eventCallback | autoConvert | srcQuality | (loopback ? loopbackFlag : 0);
            bool initialized = false;
            if (!loopback)
            {
                uint defaultPeriod, fundamentalPeriod, minimumPeriod, maximumPeriod;
                if (client.Pointer->GetSharedModeEnginePeriod(&format, &defaultPeriod, &fundamentalPeriod, &minimumPeriod, &maximumPeriod).Succeeded)
                {
                    // IAudioClient3 accepts EVENTCALLBACK only, not AUTOCONVERTPCM or SRC_DEFAULT_QUALITY.
                    // Try the low period when this format is supported directly by the shared engine.
                    initialized = client.Pointer->InitializeSharedAudioStream(eventCallback, minimumPeriod, &format, null).Succeeded;
                    if (!initialized)
                    {
                        // Failed initialization can leave a client partially initialized. Retry on a
                        // fresh client so default-period Windows resampling also works for mono/44.1 kHz microphones.
                        client.Dispose();
                        client = ActivateClient(ownedDevice);
                    }
                }
            }
            // Loopback and devices with a locked engine period use the supported default period (typically 10 ms).
            if (!initialized) client.Pointer->Initialize(AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_SHARED, flags, 0, 0, &format, null).ThrowOnFailure();
            client.Pointer->SetEventHandle(ready.SafeWaitHandle.DangerousGetHandle()).ThrowOnFailure();
            Guid iid = typeof(IAudioCaptureClient).GUID;
            void* rawCapture;
            client.Pointer->GetService(&iid, &rawCapture).ThrowOnFailure();
            return new() { Client = client, Capture = new((IAudioCaptureClient*)rawCapture), Ready = ready, Source = loopback ? 0 : 1, Gain = gain };
        }
        catch { ready.Dispose(); client.Dispose(); throw; }
    }

    private static ComPtr<IAudioClient3> ActivateClient(IMMDevice device)
    {
        Guid iid = typeof(IAudioClient3).GUID;
        device.Activate(iid, 1, null, out nint rawClient).CheckError(); // CLSCTX_INPROC_SERVER
        return new((IAudioClient3*)rawClient);
    }

    private void Run()
    {
        List<Endpoint> endpoints = new();
        bool com = false;
        nint mmcss = 0;
        try
        {
            NativeMethods.CoInitializeEx(null, COINIT.COINIT_MULTITHREADED).ThrowOnFailure();
            com = true;
            uint taskIndex = 0;
            mmcss = NativeMethods.AvSetMmThreadCharacteristics("Audio", ref taskIndex);
            using IMMDeviceEnumerator enumerator = new();
            if (options.CaptureSystemAudio) endpoints.Add(Open(enumerator, true, options.SystemAudioDeviceId, options.SystemAudioGain));
            if (options.CaptureMicrophone) endpoints.Add(Open(enumerator, false, options.MicrophoneDeviceId, options.MicrophoneGain));
            prepared.TrySetResult();
            if (WaitHandle.WaitAny([stop, start]) == 0) return;
            foreach (Endpoint endpoint in endpoints) { endpoint.Client.Pointer->Start().ThrowOnFailure(); endpoint.Started = true; }
            WaitHandle[] events = [stop, .. endpoints.Select(x => x.Ready)];
            while (WaitHandle.WaitAny(events, 100) != 0)
                foreach (Endpoint endpoint in endpoints) Drain(endpoint);
            foreach (Endpoint endpoint in endpoints) Drain(endpoint);
        }
        catch (Exception ex) { Volatile.Write(ref failure, ex); prepared.TrySetException(ex); }
        finally
        {
            foreach (Endpoint endpoint in endpoints) endpoint.Dispose();
            if (mmcss != 0) NativeMethods.AvRevertMmThreadCharacteristics(mmcss);
            if (com) NativeMethods.CoUninitialize();
        }
    }

    private void Drain(Endpoint endpoint)
    {
        while (true)
        {
            uint available;
            endpoint.Capture.Pointer->GetNextPacketSize(&available).ThrowOnFailure();
            if (available == 0) return;
            byte* data;
            uint frames, flags;
            ulong position, timestamp;
            endpoint.Capture.Pointer->GetBuffer(&data, &frames, &flags, &position, &timestamp).ThrowOnFailure();
            if (frames == 0) return;
            try
            {
                if ((flags & 1) != 0) mixer.MarkDiscontinuity();
                long packetTime = (flags & 4) == 0 ? (long)timestamp : endpoint.NextPacketTime != 0 ? endpoint.NextPacketTime : RecordingClock.Now - frames * TimeSpan.TicksPerSecond / AudioMixer.SampleRate;
                if ((flags & 4) != 0) mixer.MarkDiscontinuity();
                endpoint.NextPacketTime = packetTime + frames * TimeSpan.TicksPerSecond / AudioMixer.SampleRate;
                // Silent packets still advance the sample clock; their buffer pointer must not be read.
                ReadOnlySpan<float> packet = (flags & 2) == 0 ? new(data, checked((int)frames * 2)) : ReadOnlySpan<float>.Empty;
                mixer.Write(endpoint.Source, packetTime, checked((int)frames), packet, endpoint.Gain, (flags & 1) != 0);
            }
            finally { endpoint.Capture.Pointer->ReleaseBuffer(frames).ThrowOnFailure(); }
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0) return;
        stop.Set();
        thread.Join();
        start.Dispose(); stop.Dispose();
    }
}
