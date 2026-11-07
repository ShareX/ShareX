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
using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace ShareX.ScreenRecordingLib.Video;

/// <summary>
/// Converts an HDR desktop capture (FP16 scRGB, 1.0 = 80 nits) to SDR sRGB on the GPU. Windows' own 8-bit capture
/// clips HDR-aware content (browsers, video players, games) at 80 nits instead of at the SDR white level, which
/// blows it out. Dividing by the SDR white level instead puts SDR content back at its original value, and only
/// highlights brighter than SDR white are compressed, using the curve of the HDR screenshot fallback.
/// </summary>
internal sealed class HdrToneMapShader : IDisposable
{
    private const string Source = """
        Texture2D<float4> source : register(t0);

        cbuffer Constants : register(b0)
        {
            float inversePaperWhite;
            float relativePeak;
            int2 offset;
        };

        struct Vertex { float4 position : SV_Position; };

        Vertex VS(uint id : SV_VertexID)
        {
            float2 uv = float2((id << 1) & 2, id & 2);
            Vertex output;
            output.position = float4(uv.x * 2 - 1, 1 - uv.y * 2, 0, 1);
            return output;
        }

        static const float knee = 0.75;

        float Shoulder(float value)
        {
            if (value <= knee) return value;
            float peak = max(relativePeak, knee + 0.001);
            float normalized = saturate((value - knee) / (peak - knee));
            float strength = (peak - knee) / (1 - knee);
            return knee + (1 - knee) * strength * normalized / (1 + (strength - 1) * normalized);
        }

        float3 ToneMap(float3 color)
        {
            // Everything within SDR white is SDR content, or HDR content that needs no compression.
            float maximum = max(color.r, max(color.g, color.b));
            if (maximum <= 1) return color;
            float luminance = dot(color, float3(0.2126, 0.7152, 0.0722));
            float mapped = Shoulder(luminance);
            if (luminance > 0.00001) color *= mapped / luminance;
            maximum = max(color.r, max(color.g, color.b));
            if (maximum > 1)
            {
                // Bring remaining out-of-range channels in toward gray, keeping luminance and hue direction.
                float gray = saturate(mapped);
                float chroma = maximum > gray ? (1 - gray) / (maximum - gray) : 0;
                color = gray + (color - gray) * chroma;
            }
            return color;
        }

        float3 EncodeSrgb(float3 color)
        {
            float3 low = color * 12.92;
            float3 high = 1.055 * pow(color, 1 / 2.4) - 0.055;
            return lerp(high, low, step(color, 0.0031308));
        }

        float4 PS(Vertex input) : SV_Target
        {
            float3 color = source.Load(int3(int2(input.position.xy) - offset, 0)).rgb * inversePaperWhite;
            // scRGB stores colors outside the sRGB gamut as negative values; NaN also becomes 0 here.
            color = ToneMap(max(color, 0));
            return float4(EncodeSrgb(saturate(color)), 1);
        }
        """;

    [StructLayout(LayoutKind.Sequential)]
    private struct Constants
    {
        public float InversePaperWhite;
        public float RelativePeak;
        public int OffsetX;
        public int OffsetY;
    }

    private static readonly Lazy<byte[]> vertexCode = new(() => ShaderCompiler.Compile(Source, "HDR tone mapping", "VS", "vs_5_0"));
    private static readonly Lazy<byte[]> pixelCode = new(() => ShaderCompiler.Compile(Source, "HDR tone mapping", "PS", "ps_5_0"));
    private readonly GraphicsDevice graphics;
    private ID3D11Multithread multithread = null!;
    private ID3D11VertexShader vertexShader = null!;
    private ID3D11PixelShader pixelShader = null!;
    private ID3D11Buffer constants = null!;
    private ID3D11RasterizerState rasterizer = null!;

    public HdrToneMapShader(GraphicsDevice graphics)
    {
        this.graphics = graphics;
        try
        {
            multithread = graphics.Context.QueryInterface<ID3D11Multithread>();
            vertexShader = graphics.Device.CreateVertexShader(vertexCode.Value);
            pixelShader = graphics.Device.CreatePixelShader(pixelCode.Value);
            constants = graphics.Device.CreateBuffer(new BufferDescription((uint)Marshal.SizeOf<Constants>(), BindFlags.ConstantBuffer));
            rasterizer = graphics.Device.CreateRasterizerState(new RasterizerDescription(CullMode.None, FillMode.Solid));
        }
        catch { Dispose(); throw; }
    }

    /// <summary>Writes <paramref name="source"/> (sized to <paramref name="destination"/>) into the BGRA target.</summary>
    public void Draw(ID3D11RenderTargetView target, ID3D11ShaderResourceView source, Rectangle destination, MonitorColorInfo color)
    {
        ID3D11DeviceContext context = graphics.Context;
        Constants values = new()
        {
            InversePaperWhite = 1f / color.SdrWhiteScale,
            RelativePeak = color.RelativePeak,
            OffsetX = destination.X,
            OffsetY = destination.Y
        };
        // Media Foundation shares this device; keep the whole binding/draw sequence together.
        multithread.Enter();
        try
        {
            context.UpdateSubresource(values, constants);
            context.IASetInputLayout(null);
            context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
            context.VSSetShader(vertexShader);
            context.PSSetShader(pixelShader);
            context.PSSetConstantBuffer(0, constants);
            context.PSSetShaderResource(0, source);
            context.RSSetState(rasterizer);
            context.RSSetViewport(new Viewport(destination.X, destination.Y, destination.Width, destination.Height));
            context.OMSetBlendState(null);
            context.OMSetRenderTargets(target);
            context.Draw(3, 0);
        }
        finally
        {
            // Release bindings before the video processor or encoder reads the canvas.
            try
            {
                context.PSSetShaderResource(0, null!);
                context.OMSetRenderTargets((ID3D11RenderTargetView)null!);
            }
            finally { multithread.Leave(); }
        }
    }

    public void Dispose()
    {
        vertexShader?.Dispose(); pixelShader?.Dispose(); constants?.Dispose(); rasterizer?.Dispose();
        multithread?.Dispose();
    }
}
