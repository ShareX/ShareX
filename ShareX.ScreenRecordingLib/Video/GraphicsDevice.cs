// SPDX-License-Identifier: GPL-3.0-or-later
using ShareX.ScreenRecordingLib.Native;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;

namespace ShareX.ScreenRecordingLib.Video;

internal sealed unsafe class GraphicsDevice : IDisposable
{
    public ComPtr<ID3D11Device> Device { get; private set; } = null!;
    public ComPtr<ID3D11DeviceContext> Context { get; private set; } = null!;
    public IDirect3DDevice WinRTDevice { get; private set; } = null!;

    public GraphicsDevice(nint monitor)
    {
        try
        {
            using ComPtr<IDXGIAdapter1>? adapter = FindAdapter(monitor);
            ID3D11Device* device;
            ID3D11DeviceContext* context;
            D3D_FEATURE_LEVEL level;
            D3D_FEATURE_LEVEL* levels = stackalloc D3D_FEATURE_LEVEL[] { D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0 };
            NativeMethods.D3D11CreateDevice(adapter == null ? null : adapter.Pointer,
                adapter == null ? D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE : D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_UNKNOWN,
                default, D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT | D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_VIDEO_SUPPORT,
                levels, 2, 7, &device, &level, &context).ThrowOnFailure();
            Device = new(device);
            Context = new(context);
            using ComPtr<ID3D11Multithread> multithread = Context.Query<ID3D11Multithread>();
            multithread.Pointer->SetMultithreadProtected(true);
            using ComPtr<IDXGIDevice> dxgiDevice = Device.Query<IDXGIDevice>();
            IUnknown* inspectable;
            NativeMethods.CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.Pointer, &inspectable).ThrowOnFailure();
            try { WinRTDevice = MarshalInterface<IDirect3DDevice>.FromAbi((nint)inspectable); }
            finally { inspectable->Release(); }
        }
        catch { Dispose(); throw; }
    }

    private static ComPtr<IDXGIAdapter1>? FindAdapter(nint monitor)
    {
        Guid iid = typeof(IDXGIFactory1).GUID;
        void* rawFactory;
        NativeMethods.CreateDXGIFactory1(&iid, &rawFactory).ThrowOnFailure();
        using ComPtr<IDXGIFactory1> factory = new((IDXGIFactory1*)rawFactory);
        for (uint i = 0; ; i++)
        {
            IDXGIAdapter1* rawAdapter;
            if (factory.Pointer->EnumAdapters1(i, &rawAdapter).Failed) break;
            ComPtr<IDXGIAdapter1> adapter = new(rawAdapter);
            bool selected = false;
            try
            {
                for (uint j = 0; ; j++)
                {
                    IDXGIOutput* rawOutput;
                    if (adapter.Pointer->EnumOutputs(j, &rawOutput).Failed) break;
                    using ComPtr<IDXGIOutput> output = new(rawOutput);
                    DXGI_OUTPUT_DESC description;
                    output.Pointer->GetDesc(&description).ThrowOnFailure();
                    if (description.Monitor == monitor) { selected = true; return adapter; }
                }
            }
            finally { if (!selected) adapter.Dispose(); }
        }
        return null;
    }

    public ComPtr<ID3D11Texture2D> CreateTexture(int width, int height, DXGI_FORMAT format, D3D11_BIND_FLAG bindFlags)
    {
        D3D11_TEXTURE2D_DESC description = new()
        {
            Width = (uint)width, Height = (uint)height, MipLevels = 1, ArraySize = 1,
            Format = format, SampleDesc = new() { Count = 1 }, Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
            BindFlags = bindFlags
        };
        ID3D11Texture2D* texture;
        Device.Pointer->CreateTexture2D(&description, null, &texture).ThrowOnFailure();
        return new(texture);
    }

    public void Dispose()
    {
        WinRTDevice?.Dispose();
        Context?.Dispose();
        Device?.Dispose();
    }
}