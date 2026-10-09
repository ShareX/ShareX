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
using System.Collections.Concurrent;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib.Video;

/// <summary>Textures are returned by IMFTrackedSample only after the encoder releases the sample.</summary>
internal sealed class VideoTexturePool : IDisposable
{
    internal sealed class Slot : IDisposable
    {
        public required ID3D11Texture2D Texture;
        public required ID3D11VideoProcessorOutputView View;
        public required SampleReleaseCallback Callback;
        public void Dispose() { Callback.Dispose(); View.Dispose(); Texture.Dispose(); }
    }

    private readonly List<Slot> slots = new();
    private readonly ConcurrentQueue<Slot> available = new();
    private int disposed;

    public VideoTexturePool(GraphicsDevice graphics, GpuVideoProcessor processor, BindFlags encoderBindFlags)
    {
        try
        {
            for (int i = 0; i < 8; i++)
            {
                ID3D11Texture2D texture = graphics.CreateTexture(processor.Width, processor.Height,
                    Format.NV12, encoderBindFlags | BindFlags.RenderTarget);
                ID3D11VideoProcessorOutputView view;
                try { view = processor.CreateOutputView(texture); }
                catch { texture.Dispose(); throw; }
                Slot slot = new() { Texture = texture, View = view, Callback = null! };
                slot.Callback = new(() => { if (Volatile.Read(ref disposed) == 0) available.Enqueue(slot); });
                slots.Add(slot); available.Enqueue(slot);
            }
        }
        catch { Dispose(); throw; }
    }

    public bool TryRent(out Slot? slot) => available.TryDequeue(out slot);
    public void Return(Slot slot) { if (Volatile.Read(ref disposed) == 0) available.Enqueue(slot); }

    public IMFSample CreateSample(Slot slot)
    {
        MediaFactory.MFCreateVideoSampleFromSurface(null!, out IMFSample sample).CheckError();
        bool allocatorSet = false;
        try
        {
            using IMFMediaBuffer buffer = MediaFactory.MFCreateDXGISurfaceBuffer(typeof(ID3D11Texture2D).GUID, slot.Texture, 0, false);
            Texture2DDescription desc = slot.Texture.Description;
            buffer.CurrentLength = checked((int)(desc.Width * desc.Height * 3 / 2));
            sample.AddBuffer(buffer);
            using IMFTrackedSample tracked = sample.QueryInterface<IMFTrackedSample>();
            tracked.SetAllocator(slot.Callback, null!);
            allocatorSet = true;
            return sample;
        }
        catch { sample.Dispose(); if (!allocatorSet) Return(slot); throw; }
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref disposed, 1);
        foreach (Slot slot in slots) slot.Dispose();
        slots.Clear(); available.Clear();
    }
}
