#include "UnityCG.cginc"

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
#define PCX_COPY_FOG(o, i) o.fogCoord = i.fogCoord;
#else
#define PCX_COPY_FOG(o, i)
#endif

half4 UnpackColor32(uint c)
{
    return (uint4(c, c >> 8, c >> 16, c >> 24) & 0xff) / 255.0;
}

half3 UnpackColor(float p)
{
    uint i = asuint(p);
    return half3(
        ((i      ) & 0x7ff) / 256.0,
        ((i >> 11) & 0x7ff) / 256.0,
        ((i >> 22) & 0x3ff) / 128.0
    );

}

float PackColor(half3 rgb)
{
    uint r = rgb.r * 256;
    uint g = rgb.r * 256;
    uint b = rgb.r * 256;
    return r | (g << 11) | (b << 22);
}

#define PCX_MAX_BRIGHTNESS 16

uint PcxEncodeColor(half3 rgb)
{
    half y = max(max(rgb.r, rgb.g), rgb.b);
    y = clamp(ceil(y * 255 / PCX_MAX_BRIGHTNESS), 1, 255);
    rgb *= 255 * 255 / (y * PCX_MAX_BRIGHTNESS);
    uint4 i = half4(rgb, y);
    return i.x | (i.y << 8) | (i.z << 16) | (i.w << 24);
}

half3 PcxDecodeColor(uint data)
{
    half r = (data      ) & 0xff;
    half g = (data >>  8) & 0xff;
    half b = (data >> 16) & 0xff;
    half a = (data >> 24) & 0xff;
    return half3(r, g, b) * a * PCX_MAX_BRIGHTNESS / (255 * 255);
}