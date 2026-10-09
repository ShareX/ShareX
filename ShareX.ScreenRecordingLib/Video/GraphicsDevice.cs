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
using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;

namespace ShareX.ScreenRecordingLib.Video;

internal sealed unsafe class GraphicsDevice : IDisposable
{
    public ID3D11Device Device { get; private set; } = null!;
    public ID3D11DeviceContext Context { get; private set; } = null!;
    public IDirect3DDevice WinRTDevice { get; private set; } = null!;

    public GraphicsDevice(nint monitor)
    {
        try
        {
            using IDXGIAdapter1? adapter = FindAdapter(monitor);
            D3D11.D3D11CreateDevice(adapter, adapter == null ? DriverType.Hardware : DriverType.Unknown,
                DeviceCreationFlags.BgraSupport | DeviceCreationFlags.VideoSupport,
                [FeatureLevel.Level_11_1, FeatureLevel.Level_11_0], out ID3D11Device device, out ID3D11DeviceContext context).CheckError();
            Device = device;
            Context = context;
            using ID3D11Multithread multithread = Context.QueryInterface<ID3D11Multithread>();
            multithread.SetMultithreadProtected(true);
            using IDXGIDevice dxgiDevice = Device.QueryInterface<IDXGIDevice>();
            nint inspectable;
            NativeMethods.CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.NativePointer, &inspectable).ThrowOnFailure();
            try { WinRTDevice = MarshalInterface<IDirect3DDevice>.FromAbi(inspectable); }
            finally { Marshal.Release(inspectable); }
        }
        catch { Dispose(); throw; }
    }

    private static IDXGIAdapter1? FindAdapter(nint monitor)
    {
        using IDXGIFactory1 factory = Vortice.DXGI.DXGI.CreateDXGIFactory1<IDXGIFactory1>();
        for (uint i = 0; factory.EnumAdapters1(i, out IDXGIAdapter1 adapter).Success; i++)
        {
            bool selected = false;
            try
            {
                for (uint j = 0; adapter.EnumOutputs(j, out IDXGIOutput output).Success; j++)
                {
                    using (output)
                    {
                        if (output.Description.Monitor == monitor) { selected = true; return adapter; }
                    }
                }
            }
            finally { if (!selected) adapter.Dispose(); }
        }
        return null;
    }

    public ID3D11Texture2D CreateTexture(int width, int height, Format format, BindFlags bindFlags) =>
        Device.CreateTexture2D(new Texture2DDescription
        {
            Width = (uint)width,
            Height = (uint)height,
            MipLevels = 1,
            ArraySize = 1,
            Format = format,
            SampleDescription = new(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = bindFlags
        });

    public void Dispose()
    {
        WinRTDevice?.Dispose();
        Context?.Dispose();
        Device?.Dispose();
    }
}
