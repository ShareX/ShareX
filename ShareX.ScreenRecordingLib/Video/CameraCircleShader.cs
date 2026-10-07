// SPDX-License-Identifier: GPL-3.0-or-later
using System.Drawing;
using System.Runtime.InteropServices;
using ShareX.ScreenRecordingLib.Native;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace ShareX.ScreenRecordingLib.Video;

/// <summary>Blends a circular camera image into a small screen tile without CPU pixel processing.</summary>
internal sealed class CameraCircleShader : IDisposable
{
    private const string Source = """
        Texture2D camera : register(t0);
        SamplerState linearClamp : register(s0);

        struct Vertex { float4 position : SV_Position; float2 uv : TEXCOORD0; };

        Vertex VS(uint id : SV_VertexID)
        {
            Vertex output;
            output.uv = float2((id << 1) & 2, id & 2);
            output.position = float4(output.uv.x * 2 - 1, 1 - output.uv.y * 2, 0, 1);
            return output;
        }

        float4 PS(Vertex input) : SV_Target
        {
            float distance = length(input.uv - 0.5);
            float coverage = saturate((0.5 - distance) / max(fwidth(distance), 0.000001) + 0.5);
            clip(coverage - 0.000001);
            return float4(camera.Sample(linearClamp, input.uv).rgb, coverage);
        }
        """;

    // Compile with Windows' built-in compiler once per process, only when a circle is requested.
    private static readonly Lazy<byte[]> vertexCode = new(() => Compile("VS", "vs_5_0"));
    private static readonly Lazy<byte[]> pixelCode = new(() => Compile("PS", "ps_5_0"));
    private readonly GraphicsDevice graphics;
    private ID3D11Multithread multithread = null!;
    private ID3D11VertexShader vertexShader = null!;
    private ID3D11PixelShader pixelShader = null!;
    private ID3D11ShaderResourceView cameraView = null!;
    private ID3D11RenderTargetView targetView = null!;
    private ID3D11SamplerState sampler = null!;
    private ID3D11BlendState blend = null!;
    private ID3D11RasterizerState rasterizer = null!;
    public ID3D11Texture2D Texture { get; private set; } = null!;

    public CameraCircleShader(GraphicsDevice graphics, ID3D11Texture2D camera)
    {
        this.graphics = graphics;
        try
        {
            multithread = graphics.Context.QueryInterface<ID3D11Multithread>();
            vertexShader = graphics.Device.CreateVertexShader(vertexCode.Value);
            pixelShader = graphics.Device.CreatePixelShader(pixelCode.Value);
            cameraView = graphics.Device.CreateShaderResourceView(camera);
            Texture2DDescription desc = camera.Description;
            Texture = graphics.CreateTexture((int)desc.Width, (int)desc.Height, Format.B8G8R8A8_UNorm, BindFlags.RenderTarget);
            targetView = graphics.Device.CreateRenderTargetView(Texture);
            sampler = graphics.Device.CreateSamplerState(new SamplerDescription(Filter.MinMagMipLinear, TextureAddressMode.Clamp));
            blend = graphics.Device.CreateBlendState(new BlendDescription(Blend.SourceAlpha, Blend.InverseSourceAlpha,
                Blend.One, Blend.InverseSourceAlpha));
            rasterizer = graphics.Device.CreateRasterizerState(new RasterizerDescription(CullMode.None, FillMode.Solid));
        }
        catch { Dispose(); throw; }
    }

    public ID3D11Texture2D Compose(ID3D11Texture2D canvas, Rectangle bounds)
    {
        ID3D11DeviceContext context = graphics.Context;
        // Refresh the background even when the camera frame is unchanged. The screen canvas
        // stays pristine, and this adds only a tile-sized GPU copy on the multi-stream path.
        // Media Foundation shares this device. Protect the whole binding/draw sequence,
        // beyond the per-call protection enabled by GraphicsDevice.
        multithread.Enter();
        try
        {
            context.CopySubresourceRegion(Texture, 0, 0, 0, 0, canvas, 0,
                new Box(bounds.Left, bounds.Top, 0, bounds.Right, bounds.Bottom, 1));
            context.IASetInputLayout(null);
            context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
            context.VSSetShader(vertexShader);
            context.PSSetShader(pixelShader);
            context.PSSetSampler(0, sampler);
            context.RSSetState(rasterizer);
            context.RSSetViewport(new Viewport(0, 0, bounds.Width, bounds.Height));
            context.OMSetBlendState(blend);
            context.OMSetRenderTargets(targetView);
            context.PSSetShaderResource(0, cameraView);
            context.Draw(3, 0);
        }
        finally
        {
            // Release bindings before the next camera conversion or video-processor read.
            try
            {
                context.PSSetShaderResource(0, null!);
                context.OMSetRenderTargets((ID3D11RenderTargetView)null!);
            }
            finally { multithread.Leave(); }
        }
        return Texture;
    }

    private static unsafe byte[] Compile(string entryPoint, string target)
    {
        byte[] source = System.Text.Encoding.ASCII.GetBytes(Source);
        fixed (byte* data = source)
        {
            HRESULT result = NativeMethods.D3DCompile(data, (nuint)source.Length, "CameraCircleShader", 0, 0,
                entryPoint, target, (1u << 11) | (1u << 15), 0, out nint code, out nint errors); // Strictness, optimization level 3.
            using Blob? bytecode = code == 0 ? null : new(code);
            using Blob? messages = errors == 0 ? null : new(errors);
            if (result.Failed)
                throw new InvalidOperationException($"Could not compile the camera overlay shader. {Marshal.PtrToStringAnsi(messages?.BufferPointer ?? 0)}",
                    Marshal.GetExceptionForHR(result.Value));
            if (bytecode == null) throw new InvalidOperationException("Windows did not return camera overlay shader bytecode.");
            return new ReadOnlySpan<byte>((void*)bytecode.BufferPointer, checked((int)(nuint)bytecode.BufferSize)).ToArray();
        }
    }

    public void Dispose()
    {
        cameraView?.Dispose(); targetView?.Dispose(); Texture?.Dispose();
        vertexShader?.Dispose(); pixelShader?.Dispose(); sampler?.Dispose(); blend?.Dispose(); rasterizer?.Dispose();
        multithread?.Dispose();
    }
}
