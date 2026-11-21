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

using System.Drawing;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace ShareX.ScreenRecordingLib.Video;

/// <summary>Draws an <see cref="IRecordingOverlay"/> image over the screen with per-pixel alpha on the GPU.</summary>
internal sealed unsafe class PixelOverlay : IVideoFrameOverlay, IDisposable
{
    private const string Source = """
        Texture2D overlay : register(t0);
        SamplerState pointClamp : register(s0);

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
            return overlay.Sample(pointClamp, input.uv);
        }
        """;

    private static readonly Lazy<byte[]> vertexCode = new(() => ShaderCompiler.Compile(Source, "recording overlay", "VS", "vs_5_0"));
    private static readonly Lazy<byte[]> pixelCode = new(() => ShaderCompiler.Compile(Source, "recording overlay", "PS", "ps_5_0"));
    private readonly GraphicsDevice graphics;
    private readonly IRecordingOverlay source;
    private readonly int frameWidth, frameHeight;
    private ID3D11Multithread? multithread;
    private ID3D11VertexShader? vertexShader;
    private ID3D11PixelShader? pixelShader;
    private ID3D11SamplerState? sampler;
    private ID3D11BlendState? blend;
    private ID3D11RasterizerState? rasterizer;
    private ID3D11Texture2D? image;
    private ID3D11ShaderResourceView? imageView;
    private ID3D11Texture2D? tile;
    private ID3D11RenderTargetView? tileView;
    private int imageWidth, imageHeight;
    private int version = -1;

    public bool HasFrame { get; private set; }
    public Rectangle Bounds { get; private set; }

    public PixelOverlay(GraphicsDevice graphics, IRecordingOverlay source, int frameWidth, int frameHeight)
    {
        this.graphics = graphics;
        this.source = source;
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
        try
        {
            multithread = graphics.Context.QueryInterface<ID3D11Multithread>();
            vertexShader = graphics.Device.CreateVertexShader(vertexCode.Value);
            pixelShader = graphics.Device.CreatePixelShader(pixelCode.Value);
            sampler = graphics.Device.CreateSamplerState(new SamplerDescription(Filter.MinMagMipPoint, TextureAddressMode.Clamp));
            // Premultiplied alpha: result = overlay + screen * (1 - overlay alpha).
            blend = graphics.Device.CreateBlendState(new BlendDescription(Blend.One, Blend.InverseSourceAlpha,
                Blend.One, Blend.InverseSourceAlpha));
            rasterizer = graphics.Device.CreateRasterizerState(new RasterizerDescription(CullMode.None, FillMode.Solid));
        }
        catch { Dispose(); throw; }
    }

    /// <summary>Gets the current image from the source. Call once per frame before compositing.</summary>
    public void Update()
    {
        RecordingOverlayImage? next = source.GetImage(frameWidth, frameHeight);
        if (next == null || next.Width < 1 || next.Height < 1 || next.Width > frameWidth || next.Height > frameHeight ||
            next.Pixels.Length < next.Width * next.Height * 4)
        {
            HasFrame = false;
            return;
        }
        if (next.Version != version)
        {
            if (image == null || imageWidth != next.Width || imageHeight != next.Height) CreateTextures(next.Width, next.Height);
            fixed (byte* pixels = next.Pixels)
            {
                graphics.Context.UpdateSubresource(image!, 0, null, (nint)pixels, (uint)(next.Width * 4), 0);
            }
            version = next.Version;
        }
        int x = Math.Clamp(next.X, 0, frameWidth - next.Width);
        int y = Math.Clamp(next.Y, 0, frameHeight - next.Height);
        Bounds = new Rectangle(x, y, next.Width, next.Height);
        HasFrame = true;
    }

    private void CreateTextures(int width, int height)
    {
        imageView?.Dispose(); image?.Dispose(); tileView?.Dispose(); tile?.Dispose();
        image = graphics.CreateTexture(width, height, Format.B8G8R8A8_UNorm, BindFlags.ShaderResource);
        imageView = graphics.Device.CreateShaderResourceView(image);
        tile = graphics.CreateTexture(width, height, Format.B8G8R8A8_UNorm, BindFlags.RenderTarget);
        tileView = graphics.Device.CreateRenderTargetView(tile);
        imageWidth = width;
        imageHeight = height;
    }

    public ID3D11Texture2D GetCompositionTexture(ID3D11Texture2D canvas)
    {
        ID3D11DeviceContext context = graphics.Context;
        Rectangle bounds = Bounds;
        multithread!.Enter();
        try
        {
            context.CopySubresourceRegion(tile!, 0, 0, 0, 0, canvas, 0,
                new Box(bounds.Left, bounds.Top, 0, bounds.Right, bounds.Bottom, 1));
            context.IASetInputLayout(null);
            context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
            context.VSSetShader(vertexShader);
            context.PSSetShader(pixelShader);
            context.PSSetSampler(0, sampler);
            context.RSSetState(rasterizer);
            context.RSSetViewport(new Viewport(0, 0, bounds.Width, bounds.Height));
            context.OMSetBlendState(blend);
            context.OMSetRenderTargets(tileView!);
            context.PSSetShaderResource(0, imageView!);
            context.Draw(3, 0);
        }
        finally
        {
            try
            {
                context.PSSetShaderResource(0, null!);
                context.OMSetRenderTargets((ID3D11RenderTargetView)null!);
            }
            finally { multithread.Leave(); }
        }
        return tile!;
    }

    public void Dispose()
    {
        imageView?.Dispose(); image?.Dispose(); tileView?.Dispose(); tile?.Dispose();
        vertexShader?.Dispose(); pixelShader?.Dispose(); sampler?.Dispose(); blend?.Dispose(); rasterizer?.Dispose();
        multithread?.Dispose();
    }
}
