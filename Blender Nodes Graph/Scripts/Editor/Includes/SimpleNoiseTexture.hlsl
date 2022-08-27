#include "SimpleNoiseFunctions.hlsl"
#include "SimpleNoiseUtil.hlsl"
#ifndef __HashUtil_
#include <HashUtil.hlsl>
#endif

float Srandom_float_offset(float seed)
{
    return 100.0 + rand2dTo1d(seed) * 100.0;
}

float2 Srandom_float2_offset(float seed)
{
    return float2(100.0 + rand2dTo1d(float2(seed, 0.0)) * 100.0,
        100.0 + rand2dTo1d(float2(seed, 1.0)) * 100.0);
}

float3 Srandom_float3_offset(float seed)
{
    return float3(100.0 + rand2dTo1d(float2(seed, 0.0)) * 100.0,
        100.0 + rand2dTo1d(float2(seed, 1.0)) * 100.0,
        100.0 + rand2dTo1d(float2(seed, 2.0)) * 100.0);
}

float4 Srandom_float4_offset(float seed)
{
    return float4(100.0 + rand2dTo1d(float2(seed, 0.0)) * 100.0,
        100.0 + rand2dTo1d(float2(seed, 1.0)) * 100.0,
        100.0 + rand2dTo1d(float2(seed, 2.0)) * 100.0,
        100.0 + rand2dTo1d(float2(seed, 3.0)) * 100.0);
}

void node_simple_noise_texture_2d_float(float3 co,
    float w,
    float scale,
    float detail,
    float roughness,
    float distortion,
    out float value,
    out float4 color)
{
    float2 p = co.xy * scale;

    if (distortion != 0.0) {
        p += float2(Ssnoise(p + Srandom_float2_offset(0.0)) * distortion,
            Ssnoise(p + Srandom_float2_offset(1.0)) * distortion);
    }

    value = Sfractal_noise(p, detail, roughness);
    color = float4(value,
        Sfractal_noise(p + Srandom_float2_offset(2.0), detail, roughness),
        Sfractal_noise(p + Srandom_float2_offset(3.0), detail, roughness),
        1.0);
}

void node_simple_noise_texture_3d_float(float3 co,
    float w,
    float scale,
    float detail,
    float roughness,
    float distortion,
    out float value,
    out float4 color)
{
    float3 p = co * scale;
    if (distortion != 0.0) {
        p += float3(Ssnoise(p + Srandom_float3_offset(0.0)) * distortion,
            Ssnoise(p + Srandom_float3_offset(1.0)) * distortion,
            Ssnoise(p + Srandom_float3_offset(2.0)) * distortion);
    }

    value = Sfractal_noise(p, detail, roughness);
    color = float4(value,
        Sfractal_noise(p + Srandom_float3_offset(3.0), detail, roughness),
        Sfractal_noise(p + Srandom_float3_offset(4.0), detail, roughness),
        1.0);
}

void node_simple_noise_texture_4d_float(float3 co,
    float w,
    float scale,
    float detail,
    float roughness,
    float distortion,
    out float value,
    out float4 color)
{
    float4 p = float4(co, w) * scale;
    if (distortion != 0.0) {
        p += float4(Ssnoise(p + Srandom_float4_offset(0.0)) * distortion,
            Ssnoise(p + Srandom_float4_offset(1.0)) * distortion,
            Ssnoise(p + Srandom_float4_offset(2.0)) * distortion,
            Ssnoise(p + Srandom_float4_offset(3.0))* distortion);
    }

    value = Sfractal_noise(p, detail, roughness);
    color = float4(value,
        Sfractal_noise(p + Srandom_float4_offset(4.0), detail, roughness),
        Sfractal_noise(p + Srandom_float4_offset(5.0), detail, roughness),
        1.0);
}

void node_simple_noise_texture_full(float3 co,
    float w,
    float scale,
    float detail,
    float roughness,
    float distortion,
    float dimensions,
    out float factor_value,
    out float4 color_value) {

    if (dimensions == 0)
    {
        node_simple_noise_texture_2d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 1)
    {
        node_simple_noise_texture_3d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 2)
    {
        node_simple_noise_texture_4d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
}