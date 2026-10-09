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
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

// Vortice provides MMDevice wrappers, but does not currently wrap these WASAPI capture interfaces.
[Guid("7ed4ee07-8e67-4cd4-8c1a-2b7a5987ad42")]
internal unsafe struct IAudioClient3
{
    public void** Vtable;
    public HRESULT Initialize(AUDCLNT_SHAREMODE mode, uint flags, long bufferDuration, long periodicity, WAVEFORMATEX* format, Guid* session)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, AUDCLNT_SHAREMODE, uint, long, long, WAVEFORMATEX*, Guid*, HRESULT>)Vtable[3])(self, mode, flags, bufferDuration, periodicity, format, session);
    }
    public HRESULT Start()
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, HRESULT>)Vtable[10])(self);
    }
    public HRESULT Stop()
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, HRESULT>)Vtable[11])(self);
    }
    public HRESULT SetEventHandle(nint ready)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, nint, HRESULT>)Vtable[13])(self, ready);
    }
    public HRESULT GetService(Guid* iid, void** result)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, Guid*, void**, HRESULT>)Vtable[14])(self, iid, result);
    }
    public HRESULT GetSharedModeEnginePeriod(WAVEFORMATEX* format, uint* defaultPeriod, uint* fundamentalPeriod, uint* minimumPeriod, uint* maximumPeriod)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, WAVEFORMATEX*, uint*, uint*, uint*, uint*, HRESULT>)Vtable[18])(self, format, defaultPeriod, fundamentalPeriod, minimumPeriod, maximumPeriod);
    }
    public HRESULT InitializeSharedAudioStream(uint flags, uint period, WAVEFORMATEX* format, Guid* session)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, uint, uint, WAVEFORMATEX*, Guid*, HRESULT>)Vtable[20])(self, flags, period, format, session);
    }
}

[Guid("c8adbd64-e71e-48a0-a4de-185c395cd317")]
internal unsafe struct IAudioCaptureClient
{
    public void** Vtable;
    public HRESULT GetBuffer(byte** data, uint* frames, uint* flags, ulong* devicePosition, ulong* qpcPosition)
    {
        fixed (IAudioCaptureClient* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioCaptureClient*, byte**, uint*, uint*, ulong*, ulong*, HRESULT>)Vtable[3])(self, data, frames, flags, devicePosition, qpcPosition);
    }
    public HRESULT ReleaseBuffer(uint frames)
    {
        fixed (IAudioCaptureClient* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioCaptureClient*, uint, HRESULT>)Vtable[4])(self, frames);
    }
    public HRESULT GetNextPacketSize(uint* frames)
    {
        fixed (IAudioCaptureClient* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioCaptureClient*, uint*, HRESULT>)Vtable[5])(self, frames);
    }
}
