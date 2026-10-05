// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

[Guid("770aae78-f26f-4dba-a829-253c83d1b387")]
internal unsafe struct IDXGIFactory1
{
    public void** Vtable;
    public HRESULT EnumAdapters1(uint index, IDXGIAdapter1** adapter)
    {
        fixed (IDXGIFactory1* self = &this)
            return ((delegate* unmanaged[Stdcall]<IDXGIFactory1*, uint, IDXGIAdapter1**, HRESULT>)Vtable[12])(self, index, adapter);
    }
}

[Guid("29038f61-3839-4626-91fd-086879011a05")]
internal unsafe struct IDXGIAdapter1
{
    public void** Vtable;
    public HRESULT EnumOutputs(uint index, IDXGIOutput** output)
    {
        fixed (IDXGIAdapter1* self = &this)
            return ((delegate* unmanaged[Stdcall]<IDXGIAdapter1*, uint, IDXGIOutput**, HRESULT>)Vtable[7])(self, index, output);
    }
}

[Guid("ae02eedb-c735-4690-8d52-5a8dc20213aa")]
internal unsafe struct IDXGIOutput
{
    public void** Vtable;
    public HRESULT GetDesc(DXGI_OUTPUT_DESC* description)
    {
        fixed (IDXGIOutput* self = &this)
            return ((delegate* unmanaged[Stdcall]<IDXGIOutput*, DXGI_OUTPUT_DESC*, HRESULT>)Vtable[7])(self, description);
    }
}

[Guid("54ec77fa-1377-44e6-8c32-88fd5f44c84c")]
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct IDXGIDevice { public void** Vtable; }

[Guid("db6f6ddb-ac77-4e88-8253-819df9bbf140")]
internal unsafe struct ID3D11Device
{
    public void** Vtable;
    public HRESULT CreateTexture2D(D3D11_TEXTURE2D_DESC* description, void* initialData, ID3D11Texture2D** texture)
    {
        fixed (ID3D11Device* self = &this)
            return ((delegate* unmanaged[Stdcall]<ID3D11Device*, D3D11_TEXTURE2D_DESC*, void*, ID3D11Texture2D**, HRESULT>)Vtable[5])(self, description, initialData, texture);
    }
    public HRESULT CreateRenderTargetView(ID3D11Resource* resource, void* description, ID3D11RenderTargetView** view)
    {
        fixed (ID3D11Device* self = &this)
            return ((delegate* unmanaged[Stdcall]<ID3D11Device*, ID3D11Resource*, void*, ID3D11RenderTargetView**, HRESULT>)Vtable[9])(self, resource, description, view);
    }
}

[Guid("c0bfa96c-e089-44fb-8eaf-26f8796190da")]
internal unsafe struct ID3D11DeviceContext
{
    public void** Vtable;
    public void CopySubresourceRegion(ID3D11Resource* destination, uint destinationSubresource, uint x, uint y, uint z,
        ID3D11Resource* source, uint sourceSubresource, D3D11_BOX* box)
    {
        fixed (ID3D11DeviceContext* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11Resource*, uint, uint, uint, uint, ID3D11Resource*, uint, D3D11_BOX*, void>)Vtable[46])
                (self, destination, destinationSubresource, x, y, z, source, sourceSubresource, box);
    }
    public void ClearRenderTargetView(ID3D11RenderTargetView* view, float* rgba)
    {
        fixed (ID3D11DeviceContext* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11DeviceContext*, ID3D11RenderTargetView*, float*, void>)Vtable[50])(self, view, rgba);
    }
}

[Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c")]
internal unsafe struct ID3D11Texture2D
{
    public void** Vtable;
    public void GetDesc(D3D11_TEXTURE2D_DESC* description)
    {
        fixed (ID3D11Texture2D* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11Texture2D*, D3D11_TEXTURE2D_DESC*, void>)Vtable[10])(self, description);
    }
}

[Guid("dc8e63f3-d12b-4952-b47b-5e45026a862d")]
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ID3D11Resource { public void** Vtable; }

[Guid("dfdba067-0b8d-4865-875b-d7b4516cc164")]
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ID3D11RenderTargetView { public void** Vtable; }

[Guid("9b7e4e00-342c-4106-a19f-4f2704f689f0")]
internal unsafe struct ID3D11Multithread
{
    public void** Vtable;
    public BOOL SetMultithreadProtected(BOOL protect)
    {
        fixed (ID3D11Multithread* self = &this)
            return ((delegate* unmanaged[Stdcall]<ID3D11Multithread*, BOOL, BOOL>)Vtable[5])(self, protect);
    }
}

[Guid("10ec4d5b-975a-4689-b9e4-d0aac30fe333")]
internal unsafe struct ID3D11VideoDevice
{
    public void** Vtable;
    public HRESULT CreateVideoProcessor(ID3D11VideoProcessorEnumerator* enumerator, uint rateIndex, ID3D11VideoProcessor** processor)
    {
        fixed (ID3D11VideoDevice* self = &this)
            return ((delegate* unmanaged[Stdcall]<ID3D11VideoDevice*, ID3D11VideoProcessorEnumerator*, uint, ID3D11VideoProcessor**, HRESULT>)Vtable[4])(self, enumerator, rateIndex, processor);
    }
    public HRESULT CreateVideoProcessorInputView(ID3D11Resource* resource, ID3D11VideoProcessorEnumerator* enumerator,
        D3D11_VIDEO_PROCESSOR_INPUT_VIEW_DESC* description, ID3D11VideoProcessorInputView** view)
    {
        fixed (ID3D11VideoDevice* self = &this)
            return ((delegate* unmanaged[Stdcall]<ID3D11VideoDevice*, ID3D11Resource*, ID3D11VideoProcessorEnumerator*, D3D11_VIDEO_PROCESSOR_INPUT_VIEW_DESC*, ID3D11VideoProcessorInputView**, HRESULT>)Vtable[8])(self, resource, enumerator, description, view);
    }
    public HRESULT CreateVideoProcessorOutputView(ID3D11Resource* resource, ID3D11VideoProcessorEnumerator* enumerator,
        D3D11_VIDEO_PROCESSOR_OUTPUT_VIEW_DESC* description, ID3D11VideoProcessorOutputView** view)
    {
        fixed (ID3D11VideoDevice* self = &this)
            return ((delegate* unmanaged[Stdcall]<ID3D11VideoDevice*, ID3D11Resource*, ID3D11VideoProcessorEnumerator*, D3D11_VIDEO_PROCESSOR_OUTPUT_VIEW_DESC*, ID3D11VideoProcessorOutputView**, HRESULT>)Vtable[9])(self, resource, enumerator, description, view);
    }
    public HRESULT CreateVideoProcessorEnumerator(D3D11_VIDEO_PROCESSOR_CONTENT_DESC* description, ID3D11VideoProcessorEnumerator** enumerator)
    {
        fixed (ID3D11VideoDevice* self = &this)
            return ((delegate* unmanaged[Stdcall]<ID3D11VideoDevice*, D3D11_VIDEO_PROCESSOR_CONTENT_DESC*, ID3D11VideoProcessorEnumerator**, HRESULT>)Vtable[10])(self, description, enumerator);
    }
}

[Guid("31627037-53ab-4200-9061-05faa9ab45f9")]
internal unsafe struct ID3D11VideoProcessorEnumerator
{
    public void** Vtable;
    public HRESULT CheckVideoProcessorFormat(DXGI_FORMAT format, uint* flags)
    {
        fixed (ID3D11VideoProcessorEnumerator* self = &this)
            return ((delegate* unmanaged[Stdcall]<ID3D11VideoProcessorEnumerator*, DXGI_FORMAT, uint*, HRESULT>)Vtable[8])(self, format, flags);
    }
}

[Guid("1d7b0652-185f-41c6-85ce-0c5be3d4ae6c")]
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ID3D11VideoProcessor { public void** Vtable; }

[Guid("11ec5a5f-51dc-4945-ab34-6e8c21300ea5")]
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ID3D11VideoProcessorInputView { public void** Vtable; }

[Guid("a048285e-25a9-4527-bd93-d68b68c44254")]
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ID3D11VideoProcessorOutputView { public void** Vtable; }

[Guid("61f21c45-3c0e-4a74-9cea-67100d9ad5e4")]
internal unsafe struct ID3D11VideoContext
{
    public void** Vtable;
    public void VideoProcessorSetOutputTargetRect(ID3D11VideoProcessor* processor, BOOL enable, RECT* rectangle)
    {
        fixed (ID3D11VideoContext* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11VideoContext*, ID3D11VideoProcessor*, BOOL, RECT*, void>)Vtable[13])(self, processor, enable, rectangle);
    }
    public void VideoProcessorSetOutputColorSpace(ID3D11VideoProcessor* processor, D3D11_VIDEO_PROCESSOR_COLOR_SPACE* colorSpace)
    {
        fixed (ID3D11VideoContext* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11VideoContext*, ID3D11VideoProcessor*, D3D11_VIDEO_PROCESSOR_COLOR_SPACE*, void>)Vtable[15])(self, processor, colorSpace);
    }
    public void VideoProcessorSetStreamFrameFormat(ID3D11VideoProcessor* processor, uint index, D3D11_VIDEO_FRAME_FORMAT format)
    {
        fixed (ID3D11VideoContext* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11VideoContext*, ID3D11VideoProcessor*, uint, D3D11_VIDEO_FRAME_FORMAT, void>)Vtable[27])(self, processor, index, format);
    }
    public void VideoProcessorSetStreamColorSpace(ID3D11VideoProcessor* processor, uint index, D3D11_VIDEO_PROCESSOR_COLOR_SPACE* colorSpace)
    {
        fixed (ID3D11VideoContext* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11VideoContext*, ID3D11VideoProcessor*, uint, D3D11_VIDEO_PROCESSOR_COLOR_SPACE*, void>)Vtable[28])(self, processor, index, colorSpace);
    }
    public void VideoProcessorSetStreamSourceRect(ID3D11VideoProcessor* processor, uint index, BOOL enable, RECT* rectangle)
    {
        fixed (ID3D11VideoContext* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11VideoContext*, ID3D11VideoProcessor*, uint, BOOL, RECT*, void>)Vtable[30])(self, processor, index, enable, rectangle);
    }
    public void VideoProcessorSetStreamDestRect(ID3D11VideoProcessor* processor, uint index, BOOL enable, RECT* rectangle)
    {
        fixed (ID3D11VideoContext* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11VideoContext*, ID3D11VideoProcessor*, uint, BOOL, RECT*, void>)Vtable[31])(self, processor, index, enable, rectangle);
    }
    public void VideoProcessorSetStreamAutoProcessingMode(ID3D11VideoProcessor* processor, uint index, BOOL enable)
    {
        fixed (ID3D11VideoContext* self = &this)
            ((delegate* unmanaged[Stdcall]<ID3D11VideoContext*, ID3D11VideoProcessor*, uint, BOOL, void>)Vtable[37])(self, processor, index, enable);
    }
    public HRESULT VideoProcessorBlt(ID3D11VideoProcessor* processor, ID3D11VideoProcessorOutputView* output,
        uint frame, uint count, D3D11_VIDEO_PROCESSOR_STREAM* streams)
    {
        fixed (ID3D11VideoContext* self = &this)
            return ((delegate* unmanaged[Stdcall]<ID3D11VideoContext*, ID3D11VideoProcessor*, ID3D11VideoProcessorOutputView*, uint, uint, D3D11_VIDEO_PROCESSOR_STREAM*, HRESULT>)Vtable[53])(self, processor, output, frame, count, streams);
    }
}