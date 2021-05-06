#include "ShaderInputStructure.hlsl"
#include "ColorSpace.hlsl"

Texture2D screenTexture : register(t0);
SamplerState simpleSampler : register(s0);

// Pass through from C# to configure the tone mapping process
cbuffer HdrMetadata : register(b0)
{
    bool  EnableHdrProcessing;
    float MonHdrDispNits;
    float MonSdrDispNits;
    float ExposureLevel;
};

// Reinhard tonemap operator
// Reinhard et al. "Photographic tone reproduction for digital images." ACM Transactions on Graphics. 21. 2002.
// http://www.cs.utah.edu/~reinhard/cdrom/tonemap.pdf
float3 ToneMapReinhard(float3 color)
{
    return color / (1.0f + color);
}

// ACES Filmic tonemap operator
// https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/
float3 ToneMapACESFilmic(float3 x)
{
    float a = 2.51f;
    float b = 0.03f;
    float c = 2.43f;
    float d = 0.59f;
    float e = 0.14f;
    return saturate((x * (a * x + b)) / (x * (c * x + d) + e));
}

// Adjust input luminance based on monitor information.
float3 TuneInputLuminance(float3 scRgbColor)
{
    return scRgbColor / (MonHdrDispNits / SRGB_D65_WHITE_POINT_NITS);
}

// Adjust output luminance based on monitor information.
float3 TuneOutputLuminance(float3 scRgbColor)
{
    return scRgbColor * (MonSdrDispNits / SRGB_D65_WHITE_POINT_NITS);
}

// Perform transform processing for Windows HDR content.
float4 PsWindowsHDR2SDR(VS_OUTPUT p) : SV_TARGET
{
    // Source -> EOTF -> Color Management -> InputLuminance ->
    // Tonemap -> OutputLuminance -> Whitescale -> Destination
    //
    // EOTF is _possibly_ not required on every Windows HDR/WCG monitors.
    // Some might just use DXGI_COLOR_SPACE_RGB_FULL_G10_NONE_P709.
    // Right now we assume DXGI_COLOR_SPACE_RGB_STUDIO_G2084_NONE_P2020
    // until we have a way to determine DXGI colorspace in desktop apps.
    //
    // Luminance information is provided by system infra.

    float4 hdr = screenTexture.Sample(simpleSampler, p.TexCoord);

    // Convert to pure scRGB (DXGI_COLOR_SPACE_RGB_FULL_G10_NONE_P709)
    float3 eotf = ST2084ToLinear(hdr.xyz);
    float3 scrgb = mul(from2020to709, eotf);

    // Tone mapping
    float3 sdr = TuneInputLuminance(hdr.xyz * ExposureLevel);
    sdr = ToneMapReinhard(sdr);
    sdr = TuneOutputLuminance(sdr);

    // White scale fix-up
    float scale = SRGB_D65_WHITE_POINT_NITS / MonSdrDispNits;
    float3x3 whiteFixupMatrix =
    {
        { scale, 0, 0 },
        { 0, scale, 0 },
        { 0, 0, scale }
    };

    float3 sdrFixup = mul(whiteFixupMatrix, sdr);

    // Return image
    return float4(sdr, hdr.a);
}

// Passthrough pixel content
float4 PsPassthrough(VS_OUTPUT p) : SV_TARGET
{
    return screenTexture.Sample(simpleSampler, p.TexCoord);
}

// For fxc
float4 main(VS_OUTPUT p) : SV_TARGET
{
    if (EnableHdrProcessing) {
        return PsWindowsHDR2SDR(p);
    }

    return PsPassthrough(p);
}
