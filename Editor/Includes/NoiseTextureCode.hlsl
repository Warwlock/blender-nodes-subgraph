/* The following offset functions generate random offsets to be added to texture
 * coordinates to act as a seed since the noise functions don't have seed values.
 * A seed value is needed for generating distortion textures and color outputs.
 * The offset's components are in the range [100, 200], not too high to cause
 * bad precision and not too small to be noticeable. We use float seed because
 * OSL only support float hashes.
 */

#ifndef __NoiseUtil_
#include <NoiseUtil.hlsl>
#endif

#ifndef __FractalNoiseUtil_
#include <FractalNoiseUtil.hlsl>
#endif

float random_float_offset(float seed)
{
    return 100.0 + hash_float_to_float(seed) * 100.0;
}

float2 random_float2_offset(float seed)
{
    return float2(100.0 + hash_float2_to_float(float2(seed, 0.0)) * 100.0,
        100.0 + hash_float2_to_float(float2(seed, 1.0)) * 100.0);
}

float3 random_float3_offset(float seed)
{
    return float3(100.0 + hash_float2_to_float(float2(seed, 0.0)) * 100.0,
        100.0 + hash_float2_to_float(float2(seed, 1.0)) * 100.0,
        100.0 + hash_float2_to_float(float2(seed, 2.0)) * 100.0);
}

float4 random_float4_offset(float seed)
{
    return float4(100.0 + hash_float2_to_float(float2(seed, 0.0)) * 100.0,
        100.0 + hash_float2_to_float(float2(seed, 1.0)) * 100.0,
        100.0 + hash_float2_to_float(float2(seed, 2.0)) * 100.0,
        100.0 + hash_float2_to_float(float2(seed, 3.0)) * 100.0);
}

void node_noise_texture_1d_float(float3 co,
    float w,
    float scale,
    float detail,
    float roughness,
    float distortion,
    out float value,
    out float4 color)
{
    float p = w * scale;
    if (distortion != 0.0) {
        p += snoise(p + random_float_offset(0.0)) * distortion;
    }

    value = fractal_noise(p, detail, roughness);
    color = float4(value,
        fractal_noise(p + random_float_offset(1.0), detail, roughness),
        fractal_noise(p + random_float_offset(2.0), detail, roughness),
        1.0);
}

void node_noise_texture_2d_float(float3 co,
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
        p += float2(snoise(p + random_float2_offset(0.0)) * distortion,
            snoise(p + random_float2_offset(1.0)) * distortion);
    }

    value = fractal_noise(p, detail, roughness);
    color = float4(value,
        fractal_noise(p + random_float2_offset(2.0), detail, roughness),
        fractal_noise(p + random_float2_offset(3.0), detail, roughness),
        1.0);
}

void node_noise_texture_3d_float(float3 co,
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
        p += float3(snoise(p + random_float3_offset(0.0)) * distortion,
            snoise(p + random_float3_offset(1.0)) * distortion,
            snoise(p + random_float3_offset(2.0)) * distortion);
    }

    value = fractal_noise(p, detail, roughness);
    color = float4(value,
        fractal_noise(p + random_float3_offset(3.0), detail, roughness),
        fractal_noise(p + random_float3_offset(4.0), detail, roughness),
        1.0);
}

void node_noise_texture_4d_float(float3 co,
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
        p += float4(snoise(p + random_float4_offset(0.0)) * distortion,
            snoise(p + random_float4_offset(1.0)) * distortion,
            snoise(p + random_float4_offset(2.0)) * distortion,
            snoise(p + random_float4_offset(3.0)) * distortion);
    }

    value = fractal_noise(p, detail, roughness);
    color = float4(value,
        fractal_noise(p + random_float4_offset(4.0), detail, roughness),
        fractal_noise(p + random_float4_offset(5.0), detail, roughness),
        1.0);
}

void node_noise_texture_full(float3 co,
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
        node_noise_texture_1d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 1)
    {
        node_noise_texture_2d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 2)
    {
        node_noise_texture_3d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 3)
    {
        node_noise_texture_4d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
}

float node_noise_texture_fac(float3 co,
    float w,
    float scale,
    float detail,
    float roughness,
    float distortion,
    float dimensions) {

    float factor_value;
    float4 color_value;

    if (dimensions == 0) 
    {
        node_noise_texture_1d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 1)
    {
        node_noise_texture_2d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 2)
    {
        node_noise_texture_3d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 3)
    {
        node_noise_texture_4d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    return factor_value;
}

float4 node_noise_texture_col(float3 co,
    float w,
    float scale,
    float detail,
    float roughness,
    float distortion,
    float dimensions) {

    float factor_value;
    float4 color_value;

    if (dimensions == 0)
    {
        node_noise_texture_1d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 1)
    {
        node_noise_texture_2d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 2)
    {
        node_noise_texture_3d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    if (dimensions == 3)
    {
        node_noise_texture_4d_float(co, w, scale, detail, roughness, distortion, factor_value, color_value);
    }
    return color_value;
}