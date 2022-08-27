/* ***** Jenkins Lookup3 Hash Functions ***** */

/* Source: http://burtleburtle.net/bob/c/lookup3.c */

#define __HashUtil_

#define rot(x, k) (((x) << (k)) | ((x) >> (32 - (k))))

#define lerp(a, b, c) \
  { \
    a -= c; \
    a ^= rot(c, 4); \
    c += b; \
    b -= a; \
    b ^= rot(a, 6); \
    a += c; \
    c -= b; \
    c ^= rot(b, 8); \
    b += a; \
    a -= c; \
    a ^= rot(c, 16); \
    c += b; \
    b -= a; \
    b ^= rot(a, 19); \
    a += c; \
    c -= b; \
    c ^= rot(b, 4); \
    b += a; \
  }

#define final(a, b, c) \
  { \
    c ^= b; \
    c -= rot(b, 14); \
    a ^= c; \
    a -= rot(c, 11); \
    b ^= a; \
    b -= rot(a, 25); \
    c ^= b; \
    c -= rot(b, 16); \
    a ^= c; \
    a -= rot(c, 4); \
    b ^= a; \
    b -= rot(a, 14); \
    c ^= b; \
    c -= rot(b, 24); \
  }

uint hash_uint(uint kx)
{
    uint a, b, c;
    a = b = c = 0xdeadbeefu + (1u << 2u) + 13u;

    a += kx;
    final(a, b, c);

    return c;
}

uint hash_uint2(uint kx, uint ky)
{
    uint a, b, c;
    a = b = c = 0xdeadbeefu + (2u << 2u) + 13u;

    b += ky;
    a += kx;
    final(a, b, c);

    return c;
}

uint hash_uint3(uint kx, uint ky, uint kz)
{
    uint a, b, c;
    a = b = c = 0xdeadbeefu + (3u << 2u) + 13u;

    c += kz;
    b += ky;
    a += kx;
    final(a, b, c);

    return c;
}

uint hash_uint4(uint kx, uint ky, uint kz, uint kw)
{
    uint a, b, c;
    a = b = c = 0xdeadbeefu + (4u << 2u) + 13u;

    a += kx;
    b += ky;
    c += kz;
    lerp(a, b, c);

    a += kw;
    final(a, b, c);

    return c;
}

#undef rot
#undef final
#undef lerp

uint hash_int(int kx)
{
    return hash_uint(uint(kx));
}

uint hash_int2(int kx, int ky)
{
    return hash_uint2(uint(kx), uint(ky));
}

uint hash_int3(int kx, int ky, int kz)
{
    return hash_uint3(uint(kx), uint(ky), uint(kz));
}

uint hash_int4(int kx, int ky, int kz, int kw)
{
    return hash_uint4(uint(kx), uint(ky), uint(kz), uint(kw));
}

/* Hashing uint or uint[234] into a float in the range [0, 1]. */

float hash_uint_to_float(uint kx)
{
    return float(hash_uint(kx)) / float(0xFFFFFFFFu);
}

float hash_uint2_to_float(uint kx, uint ky)
{
    return float(hash_uint2(kx, ky)) / float(0xFFFFFFFFu);
}

float hash_uint3_to_float(uint kx, uint ky, uint kz)
{
    return float(hash_uint3(kx, ky, kz)) / float(0xFFFFFFFFu);
}

float hash_uint4_to_float(uint kx, uint ky, uint kz, uint kw)
{
    return float(hash_uint4(kx, ky, kz, kw)) / float(0xFFFFFFFFu);
}

/* Hashing float or vec[234] into a float in the range [0, 1]. */

float hash_float_to_float(float k)
{
    return hash_uint_to_float(asuint(k));
}

float hash_float2_to_float(float2 k)
{
    return hash_uint2_to_float(asuint(k.x), asuint(k.y));
}

float hash_float3_to_float(float3 k)
{
    return hash_uint3_to_float(asuint(k.x), asuint(k.y), asuint(k.z));
}

float hash_float4_to_float(float4 k)
{
    return hash_uint4_to_float(
        asuint(k.x), asuint(k.y), asuint(k.z), asuint(k.w));
}

/* Hashing vec[234] into vec[234] of components in the range [0, 1]. */

float2 hash_float2_to_float2(float2 k)
{
    return float2(hash_float2_to_float(k), hash_float3_to_float(float3(k, 1.0)));
}

float3 hash_float3_to_float3(float3 k)
{
    return float3(
        hash_float3_to_float(k), hash_float4_to_float(float4(k, 1.0)), hash_float4_to_float(float4(k, 2.0)));
}

float4 hash_float4_to_float4(float4 k)
{
    return float4(hash_float4_to_float(k.xyzw),
        hash_float4_to_float(k.wxyz),
        hash_float4_to_float(k.zwxy),
        hash_float4_to_float(k.yzwx));
}

/* Hashing float or vec[234] into float3 of components in range [0, 1]. */

float3 hash_float_to_float3(float k)
{
    return float3(
        hash_float_to_float(k), hash_float2_to_float(float2(k, 1.0)), hash_float2_to_float(float2(k, 2.0)));
}

float3 hash_float2_to_float3(float2 k)
{
    return float3(
        hash_float2_to_float(k), hash_float3_to_float(float3(k, 1.0)), hash_float3_to_float(float3(k, 2.0)));
}

float3 hash_float4_to_float3(float4 k)
{
    return float3(hash_float4_to_float(k.xyzw), hash_float4_to_float(k.zxwy), hash_float4_to_float(k.wzyx));
}

/* Other Hash Functions */

float integer_noise(int n)
{
    int nn;
    n = (n + 1013) & 0x7fffffff;
    n = (n >> 13) ^ n;
    nn = (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
    return 0.5 * (float(nn) / 1073741824.0);
}
