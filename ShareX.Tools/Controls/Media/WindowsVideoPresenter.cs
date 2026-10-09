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

using Avalonia;
using SharpGen.Runtime;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using Vortice.MediaFoundation;

namespace ShareX.Tools;

/// <summary>Media Foundation frame-server output, presented entirely on the GPU.</summary>
internal sealed class WindowsVideoPresenter : IDisposable
{
    private readonly nint _window;
    private ID3D11Device? _device;
    private ID3D11DeviceContext? _context;
    private IDXGISwapChain1? _swapChain;
    private PixelSize _size;
    public IMFDXGIDeviceManager DeviceManager { get; private set; } = null!;

    public WindowsVideoPresenter(nint window)
    {
        _window = window;
        try
        {
            var result = D3D11.D3D11CreateDevice(null, DriverType.Hardware,
                DeviceCreationFlags.BgraSupport | DeviceCreationFlags.VideoSupport,
                [FeatureLevel.Level_11_1, FeatureLevel.Level_11_0], out _device, out _context);
            if (result.Failure)
                D3D11.D3D11CreateDevice(null, DriverType.Warp, DeviceCreationFlags.BgraSupport,
                    [FeatureLevel.Level_11_1, FeatureLevel.Level_11_0], out _device, out _context).CheckError();
            using ID3D11Multithread multithread = _context!.QueryInterface<ID3D11Multithread>();
            multithread.SetMultithreadProtected(true);
            using IDXGIDevice1 dxgiDevice = _device!.QueryInterface<IDXGIDevice1>();
            dxgiDevice.MaximumFrameLatency = 1;
            DeviceManager = MediaFactory.MFCreateDXGIDeviceManager();
            DeviceManager.ResetDevice(_device).CheckError();
        }
        catch { Dispose(); throw; }
    }

    public void Present(IMFMediaEngineEx engine, RawRect rectangle, bool repaint)
    {
        int width = rectangle.Right, height = rectangle.Bottom;
        if (width <= 0 || height <= 0) return;
        if (!repaint)
        {
            // The bool overload also treats S_FALSE as success. Only S_OK means a new frame is ready.
            Result result = engine.OnVideoStreamTick_(out _);
            result.CheckError();
            if (result.Code != 0) return;
        }

        PixelSize size = new(width, height);
        if (_swapChain == null)
        {
            using IDXGIDevice dxgiDevice = _device!.QueryInterface<IDXGIDevice>();
            using IDXGIAdapter adapter = dxgiDevice.GetAdapter();
            using IDXGIFactory2 factory = adapter.GetParent<IDXGIFactory2>();
            _swapChain = factory.CreateSwapChainForHwnd(_device, _window, new SwapChainDescription1
            {
                Width = (uint)width,
                Height = (uint)height,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new(1, 0),
                BufferUsage = Usage.RenderTargetOutput,
                BufferCount = 2,
                SwapEffect = SwapEffect.FlipSequential,
                Scaling = Scaling.Stretch,
                AlphaMode = AlphaMode.Ignore
            });
            factory.MakeWindowAssociation(_window, WindowAssociationFlags.IgnoreAll).CheckError();
            _size = size;
        }
        else if (_size != size)
        {
            // No back-buffer references survive a presentation, so resize can release them safely.
            _swapChain.ResizeBuffers(0, (uint)width, (uint)height, Format.Unknown, SwapChainFlags.None).CheckError();
            _size = size;
        }
        using ID3D11Texture2D buffer = _swapChain.GetBuffer<ID3D11Texture2D>(0);
        engine.TransferVideoFrame(buffer, null, rectangle, new ColorBgra(0, 0, 0, 255));
        _swapChain.Present(0, PresentFlags.None).CheckError();
    }

    public void Dispose()
    {
        _swapChain?.Dispose();
        _swapChain = null;
        DeviceManager?.Dispose();
        _context?.Dispose();
        _device?.Dispose();
    }
}