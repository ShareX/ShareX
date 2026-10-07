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
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.EasterEggs;

/// <summary>
/// A full-window color CRT with a curved glass face, finite electron-beam width and
/// phosphor mask. Display geometry is shared with input mapping; the image itself is untouched.
/// </summary>
internal sealed class CrtMonitorEffect : IShaderEasterEggEffect
{
    private const float Curvature = 0.085f;

    public void UpdateUniforms(SKRuntimeEffectUniforms uniforms, Size bounds)
    {
        uniforms["curvature"] = Curvature;
    }

    public Point MapToSource(Point position, Size bounds)
    {
        double halfWidth = Math.Max(bounds.Width * 0.5, 1);
        double halfHeight = Math.Max(bounds.Height * 0.5, 1);
        double px = position.X - bounds.Width * 0.5;
        double py = position.Y - bounds.Height * 0.5;

        double x = px / halfWidth;
        double y = py / halfHeight;
        double bulge = 1 - Curvature * Math.Max(1 - x * x, 0) * Math.Max(1 - y * y, 0);
        return new Point((x * bulge * 0.5 + 0.5) * bounds.Width,
            (y * bulge * 0.5 + 0.5) * bounds.Height);
    }

    public string ShaderSource => """
        uniform shader source;
        uniform float2 resolution;
        uniform float2 pixelSize;
        uniform float time;
        uniform float powerOn;
        uniform float curvature;

        const float PI = 3.14159265359;

        float noise(float2 p)
        {
            float3 q = fract(float3(p.xyx) * 0.1031);
            q += dot(q, q.yzx + 33.33);
            return fract((q.x + q.y) * q.z);
        }

        float3 phosphorMask(float2 position)
        {
            // Fine RGB stripes, with at least one physical pixel per phosphor. Pixel-sized
            // edge filtering keeps the smaller mask stable at different display scales.
            float2 pitch = max(float2(3.0, 4.0), pixelSize * float2(3.0, 4.0));
            float2 cell = position / pitch;
            float3 distance = abs(fract(cell.x - float3(1.0 / 6.0, 0.5, 5.0 / 6.0) + 0.5) - 0.5);
            float edge = pixelSize.x / pitch.x * 0.5;
            float3 phosphors = 1.0 - smoothstep(float3(0.13 - edge), float3(0.13 + edge), distance);

            // Align the separators across columns so the cells stay rectangular. Narrow,
            // softer gaps avoid the coarse honeycomb appearance of staggered slot masks.
            float slotY = abs(fract(cell.y) - 0.5);
            float slotEdge = pixelSize.y / pitch.y * 0.5;
            float slot = 1.0 - smoothstep(0.40 - slotEdge, 0.40 + slotEdge, slotY);
            float vertical = dot(phosphors, float3(1.0));
            return (0.72 + phosphors * 0.55) * (0.70 + vertical * 0.30) * (0.70 + slot * 0.30);
        }

        float3 linearSample(float2 p)
        {
            half4 sample = source.eval(clamp(p, pixelSize * 0.5, resolution - pixelSize * 0.5));
            // Source shaders return premultiplied colors. Transparent image regions sit on
            // the editor background, but unpremultiply here for embedded transparent hosts.
            float3 rgb = float3(sample.rgb) / max(float(sample.a), 0.0001);
            return pow(clamp(rgb, 0.0, 1.0), float3(2.2));
        }

        half4 main(float2 position)
        {
            float2 p = position - resolution * 0.5;
            float2 halfScreen = max(resolution * 0.5, float2(1.0));
            float aa = max(pixelSize.x, pixelSize.y);

            float2 q = p / halfScreen;
            // Edge-preserving barrel distortion keeps controls accessible along window edges.
            // This is the same mapping used for pointer input in MapToSource.
            float bulge = 1.0 - curvature * (1.0 - q.x * q.x) * (1.0 - q.y * q.y);
            float2 uv = q * bulge * 0.5 + 0.5;
            float2 samplePosition = uv * resolution;
            float frame = floor(time * 60.0);
            float jitter = sin(uv.y * 143.0 + time * 17.0) *
                sin(time * 31.0) * 0.20;
            samplePosition.x += jitter * pixelSize.x;

            // Subpixel convergence error increases gently towards the edges of the tube.
            // Three central samples and a narrow horizontal beam preserve UI text detail.
            float convergence = (0.16 + 0.24 * dot(q, q)) * pixelSize.x;
            float3 center = linearSample(samplePosition);
            float3 color = float3(
                linearSample(samplePosition - float2(convergence, 0.0)).r,
                center.g,
                linearSample(samplePosition + float2(convergence, 0.0)).b);
            float2 beamOffset = float2(pixelSize.x * 0.65, 0.0);
            color = color * 0.84 + (linearSample(samplePosition - beamOffset) +
                linearSample(samplePosition + beamOffset)) * 0.08;

            // A small, energy-limited halo in linear light: bright phosphors spill into dark
            // neighbours without washing out the entire editor or crushing its black levels.
            float2 haloX = float2(pixelSize.x * 3.5, 0.0);
            float2 haloY = float2(0.0, pixelSize.y * 2.5);
            float3 halo = (linearSample(samplePosition - haloX) + linearSample(samplePosition + haloX) +
                linearSample(samplePosition - haloY) + linearSample(samplePosition + haloY)) * 0.25;
            color = color * 0.96 + halo * 0.04 + max(halo - 0.35, 0.0) * 0.035;

            // Integrate the scanline harmonic over a display pixel instead of sampling an
            // infinitely thin dark stripe. This prevents moire at different sizes and DPIs.
            float tubePixels = halfScreen.y * 2.0 / pixelSize.y;
            float lines = min(600.0, tubePixels * 0.5);
            float footprint = lines * pixelSize.y / (halfScreen.y * 2.0);
            float scanAA = sin(PI * footprint) / max(PI * footprint, 0.0001);
            float luminance = dot(color, float3(0.2126, 0.7152, 0.0722));
            float scanStrength = mix(0.52, 0.34, sqrt(clamp(luminance, 0.0, 1.0)));
            float scanline = 1.0 - scanStrength * 0.5 *
                (1.0 - cos(uv.y * lines * 2.0 * PI) * scanAA);
            color *= scanline / (1.0 - scanStrength * 0.5);

            // The mask belongs to the tube, so it stays fixed as the image pans and zooms.
            float2 devicePixel = position / pixelSize;

            // A slow beat between refresh and mains produces a rolling dim band, followed
            // by a brighter beam and phosphor decay. Keep the band continuous at wraparound.
            float refreshDistance = fract(uv.y - time * 0.30 + 0.5) - 0.5;
            float refresh = 1.0 - 0.36 * exp(-refreshDistance * refreshDistance * 180.0);
            float beamDistance = fract(refreshDistance + 0.09 + 0.5) - 0.5;
            float refreshBeam = exp(-beamDistance * beamDistance * 1800.0);
            float hum = 1.0 + 0.003 * sin(time * 2.0 * PI * 59.94);
            color *= (refresh + refreshBeam * 0.24) * hum;
            color += refreshBeam * float3(0.010, 0.011, 0.009);
            color += (noise(floor(devicePixel) + float2(frame * 13.0, frame * 7.0)) - 0.5) * 0.0038;
            color *= phosphorMask(position) * 1.14;
            color *= 1.0 - 0.12 * dot(q * q, q * q);

            // One power-on sweep, then continuous operation until the window is closed.
            float opening = smoothstep(0.0, 0.82, powerOn);
            float powered = (1.0 - smoothstep(-aa, aa,
                abs(p.y) - max(aa, halfScreen.y * opening))) * smoothstep(0.0, 0.08, powerOn);
            float startupBeam = exp(-pow(p.y / max(pixelSize.y * 1.5, 0.01), 2.0)) *
                (1.0 - smoothstep(0.02, 0.22, powerOn)) * smoothstep(0.0, 0.03, powerOn);
            color = max(color, 0.0) * powered + startupBeam * 0.35;

            // Slightly lifted, cool glass black and a broad soft room reflection convey
            // convex glass without painting a glossy white UI overlay.
            color += float3(0.0008, 0.0011, 0.0015);
            float reflection = exp(-pow((q.x + q.y * 0.32 + 0.70) * 3.4, 2.0) -
                pow((q.y + 0.80) * 3.0, 2.0)) * 0.005;
            color += reflection * float3(0.8, 0.9, 1.0);
            float3 glass = pow(clamp(color, 0.0, 1.0), float3(1.0 / 2.2));
            return half4(half3(glass), 1.0);
        }
        """;
}