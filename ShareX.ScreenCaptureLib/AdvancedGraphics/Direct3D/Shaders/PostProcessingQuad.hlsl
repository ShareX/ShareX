#include "Structures.hlsl"

VS_OUTPUT VxQuadEntry(VS_INPUT v)
{
    VS_OUTPUT vout;

    vout.TexCoord = v.TexCoord;
    vout.Position = v.Position;

    return vout;
}

VS_OUTPUT main(VS_INPUT v)
{
    return VxQuadEntry(v);
}
