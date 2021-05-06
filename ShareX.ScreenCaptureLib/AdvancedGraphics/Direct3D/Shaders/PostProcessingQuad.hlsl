#include "Structures.hlsl"

VS_OUTPUT VxQuadEntry(VS_INPUT v)
{
    VS_OUTPUT vout;

    vout.TexCoord = v.TexCoord;
    vout.Position = v.Position;

    return vout;
}
