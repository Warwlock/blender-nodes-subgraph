/* 1D Musgrave fBm
 *
 * H: fractal increment parameter
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 *
 * from "Texturing and Modelling: A procedural approach"
 */

#ifndef __NoiseUtil_
#include <NoiseUtil.hlsl>
#endif

void node_tex_musgrave_fBm_1d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float p = w * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float value = 0.0;
    float pwr = 1.0;
    float pwHL = pow(lacunarity, -H);

    for (int i = 0; i < int(octaves); i++) {
        value += snoise(p) * pwr;
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value += rmd * snoise(p) * pwr;
    }

    fac = value;
}

/* 1D Musgrave Multifractal
 *
 * H: highest fractal dimension
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 */

void node_tex_musgrave_multi_fractal_1d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float p = w * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float value = 1.0;
    float pwr = 1.0;
    float pwHL = pow(lacunarity, -H);

    for (int i = 0; i < int(octaves); i++) {
        value *= (pwr * snoise(p) + 1.0);
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value *= (rmd * pwr * snoise(p) + 1.0); /* correct? */
    }

    fac = value;
}

/* 1D Musgrave Heterogeneous Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_hetero_terrain_1d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float p = w * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    /* first unscaled octave of function; later octaves are scaled */
    float value = offset + snoise(p);
    p *= lacunarity;

    for (int i = 1; i < int(octaves); i++) {
        float increment = (snoise(p) + offset) * pwr * value;
        value += increment;
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float increment = (snoise(p) + offset) * pwr * value;
        value += rmd * increment;
    }

    fac = value;
}

/* 1D Hybrid Additive/Multiplicative Multifractal Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_hybrid_multi_fractal_1d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float p = w * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    float value = snoise(p) + offset;
    float weight = gain * value;
    p *= lacunarity;

    for (int i = 1; (weight > 0.001f) && (i < int(octaves)); i++) {
        if (weight > 1.0) {
            weight = 1.0;
        }

        float signal = (snoise(p) + offset) * pwr;
        pwr *= pwHL;
        value += weight * signal;
        weight *= gain * signal;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value += rmd * ((snoise(p) + offset) * pwr);
    }

    fac = value;
}

/* 1D Ridged Multifractal Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_ridged_multi_fractal_1d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float p = w * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    float signal = offset - abs(snoise(p));
    signal *= signal;
    float value = signal;
    float weight = 1.0;

    for (int i = 1; i < int(octaves); i++) {
        p *= lacunarity;
        weight = clamp(signal * gain, 0.0, 1.0);
        signal = offset - abs(snoise(p));
        signal *= signal;
        signal *= weight;
        value += signal * pwr;
        pwr *= pwHL;
    }

    fac = value;
}

/* 2D Musgrave fBm
 *
 * H: fractal increment parameter
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 *
 * from "Texturing and Modelling: A procedural approach"
 */

void node_tex_musgrave_fBm_2d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float2 p = co.xy * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float value = 0.0;
    float pwr = 1.0;
    float pwHL = pow(lacunarity, -H);

    for (int i = 0; i < int(octaves); i++) {
        value += snoise(p) * pwr;
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value += rmd * snoise(p) * pwr;
    }

    fac = value;
}

/* 2D Musgrave Multifractal
 *
 * H: highest fractal dimension
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 */

void node_tex_musgrave_multi_fractal_2d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float2 p = co.xy * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float value = 1.0;
    float pwr = 1.0;
    float pwHL = pow(lacunarity, -H);

    for (int i = 0; i < int(octaves); i++) {
        value *= (pwr * snoise(p) + 1.0);
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value *= (rmd * pwr * snoise(p) + 1.0); /* correct? */
    }

    fac = value;
}

/* 2D Musgrave Heterogeneous Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_hetero_terrain_2d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float2 p = co.xy * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    /* first unscaled octave of function; later octaves are scaled */
    float value = offset + snoise(p);
    p *= lacunarity;

    for (int i = 1; i < int(octaves); i++) {
        float increment = (snoise(p) + offset) * pwr * value;
        value += increment;
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float increment = (snoise(p) + offset) * pwr * value;
        value += rmd * increment;
    }

    fac = value;
}

/* 2D Hybrid Additive/Multiplicative Multifractal Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_hybrid_multi_fractal_2d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float2 p = co.xy * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    float value = snoise(p) + offset;
    float weight = gain * value;
    p *= lacunarity;

    for (int i = 1; (weight > 0.001f) && (i < int(octaves)); i++) {
        if (weight > 1.0) {
            weight = 1.0;
        }

        float signal = (snoise(p) + offset) * pwr;
        pwr *= pwHL;
        value += weight * signal;
        weight *= gain * signal;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value += rmd * ((snoise(p) + offset) * pwr);
    }

    fac = value;
}

/* 2D Ridged Multifractal Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_ridged_multi_fractal_2d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float2 p = co.xy * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    float signal = offset - abs(snoise(p));
    signal *= signal;
    float value = signal;
    float weight = 1.0;

    for (int i = 1; i < int(octaves); i++) {
        p *= lacunarity;
        weight = clamp(signal * gain, 0.0, 1.0);
        signal = offset - abs(snoise(p));
        signal *= signal;
        signal *= weight;
        value += signal * pwr;
        pwr *= pwHL;
    }

    fac = value;
}

/* 3D Musgrave fBm
 *
 * H: fractal increment parameter
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 *
 * from "Texturing and Modelling: A procedural approach"
 */

void node_tex_musgrave_fBm_3d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float3 p = co * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float value = 0.0;
    float pwr = 1.0;
    float pwHL = pow(lacunarity, -H);

    for (int i = 0; i < int(octaves); i++) {
        value += snoise(p) * pwr;
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value += rmd * snoise(p) * pwr;
    }

    fac = value;
}

/* 3D Musgrave Multifractal
 *
 * H: highest fractal dimension
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 */

void node_tex_musgrave_multi_fractal_3d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float3 p = co * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float value = 1.0;
    float pwr = 1.0;
    float pwHL = pow(lacunarity, -H);

    for (int i = 0; i < int(octaves); i++) {
        value *= (pwr * snoise(p) + 1.0);
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value *= (rmd * pwr * snoise(p) + 1.0); /* correct? */
    }

    fac = value;
}

/* 3D Musgrave Heterogeneous Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_hetero_terrain_3d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float3 p = co * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    /* first unscaled octave of function; later octaves are scaled */
    float value = offset + snoise(p);
    p *= lacunarity;

    for (int i = 1; i < int(octaves); i++) {
        float increment = (snoise(p) + offset) * pwr * value;
        value += increment;
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float increment = (snoise(p) + offset) * pwr * value;
        value += rmd * increment;
    }

    fac = value;
}

/* 3D Hybrid Additive/Multiplicative Multifractal Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_hybrid_multi_fractal_3d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float3 p = co * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    float value = snoise(p) + offset;
    float weight = gain * value;
    p *= lacunarity;

    for (int i = 1; (weight > 0.001f) && (i < int(octaves)); i++) {
        if (weight > 1.0) {
            weight = 1.0;
        }

        float signal = (snoise(p) + offset) * pwr;
        pwr *= pwHL;
        value += weight * signal;
        weight *= gain * signal;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value += rmd * ((snoise(p) + offset) * pwr);
    }

    fac = value;
}

/* 3D Ridged Multifractal Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_ridged_multi_fractal_3d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float3 p = co * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    float signal = offset - abs(snoise(p));
    signal *= signal;
    float value = signal;
    float weight = 1.0;

    for (int i = 1; i < int(octaves); i++) {
        p *= lacunarity;
        weight = clamp(signal * gain, 0.0, 1.0);
        signal = offset - abs(snoise(p));
        signal *= signal;
        signal *= weight;
        value += signal * pwr;
        pwr *= pwHL;
    }

    fac = value;
}

/* 4D Musgrave fBm
 *
 * H: fractal increment parameter
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 *
 * from "Texturing and Modelling: A procedural approach"
 */

void node_tex_musgrave_fBm_4d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float4 p = float4(co, w) * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float value = 0.0;
    float pwr = 1.0;
    float pwHL = pow(lacunarity, -H);

    for (int i = 0; i < int(octaves); i++) {
        value += snoise(p) * pwr;
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value += rmd * snoise(p) * pwr;
    }

    fac = value;
}

/* 4D Musgrave Multifractal
 *
 * H: highest fractal dimension
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 */

void node_tex_musgrave_multi_fractal_4d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float4 p = float4(co, w) * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float value = 1.0;
    float pwr = 1.0;
    float pwHL = pow(lacunarity, -H);

    for (int i = 0; i < int(octaves); i++) {
        value *= (pwr * snoise(p) + 1.0);
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value *= (rmd * pwr * snoise(p) + 1.0); /* correct? */
    }

    fac = value;
}

/* 4D Musgrave Heterogeneous Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_hetero_terrain_4d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float4 p = float4(co, w) * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    /* first unscaled octave of function; later octaves are scaled */
    float value = offset + snoise(p);
    p *= lacunarity;

    for (int i = 1; i < int(octaves); i++) {
        float increment = (snoise(p) + offset) * pwr * value;
        value += increment;
        pwr *= pwHL;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float increment = (snoise(p) + offset) * pwr * value;
        value += rmd * increment;
    }

    fac = value;
}

/* 4D Hybrid Additive/Multiplicative Multifractal Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_hybrid_multi_fractal_4d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float4 p = float4(co, w) * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    float value = snoise(p) + offset;
    float weight = gain * value;
    p *= lacunarity;

    for (int i = 1; (weight > 0.001f) && (i < int(octaves)); i++) {
        if (weight > 1.0) {
            weight = 1.0;
        }

        float signal = (snoise(p) + offset) * pwr;
        pwr *= pwHL;
        value += weight * signal;
        weight *= gain * signal;
        p *= lacunarity;
    }

    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        value += rmd * ((snoise(p) + offset) * pwr);
    }

    fac = value;
}

/* 4D Ridged Multifractal Terrain
 *
 * H: fractal dimension of the roughest area
 * lacunarity: gap between successive frequencies
 * octaves: number of frequencies in the fBm
 * offset: raises the terrain from `sea level'
 */

void node_tex_musgrave_ridged_multi_fractal_4d_float(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    out float fac)
{
    float4 p = float4(co, w) * scale;
    float H = max(dimension, 1e-5);
    float octaves = clamp(detail, 0.0, 16.0);
    float lacunarity = max(lac, 1e-5);

    float pwHL = pow(lacunarity, -H);
    float pwr = pwHL;

    float signal = offset - abs(snoise(p));
    signal *= signal;
    float value = signal;
    float weight = 1.0;

    for (int i = 1; i < int(octaves); i++) {
        p *= lacunarity;
        weight = clamp(signal * gain, 0.0, 1.0);
        signal = offset - abs(snoise(p));
        signal *= signal;
        signal *= weight;
        value += signal * pwr;
        pwr *= pwHL;
    }

    fac = value;
}

float texture_musgrave_factor(float3 co,
    float w,
    float scale,
    float detail,
    float dimension,
    float lac,
    float offset,
    float gain,
    float ddDimensions,
    float ddTypes)
{
    float outfactor;

    if (ddDimensions == 0)
    {
        if (ddTypes == 0)
        {
            node_tex_musgrave_multi_fractal_1d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 1)
        {
            node_tex_musgrave_ridged_multi_fractal_1d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 2)
        {
            node_tex_musgrave_hybrid_multi_fractal_1d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 3)
        {
            node_tex_musgrave_fBm_1d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 4)
        {
            node_tex_musgrave_hetero_terrain_1d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
    }

    if (ddDimensions == 1)
    {
        if (ddTypes == 0)
        {
            node_tex_musgrave_multi_fractal_2d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 1)
        {
            node_tex_musgrave_ridged_multi_fractal_2d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 2)
        {
            node_tex_musgrave_hybrid_multi_fractal_2d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 3)
        {
            node_tex_musgrave_fBm_2d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 4)
        {
            node_tex_musgrave_hetero_terrain_2d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
    }

    if (ddDimensions == 2)
    {
        if (ddTypes == 0)
        {
            node_tex_musgrave_multi_fractal_3d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 1)
        {
            node_tex_musgrave_ridged_multi_fractal_3d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 2)
        {
            node_tex_musgrave_hybrid_multi_fractal_3d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 3)
        {
            node_tex_musgrave_fBm_3d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 4)
        {
            node_tex_musgrave_hetero_terrain_3d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
    }

    if (ddDimensions == 3)
    {
        if (ddTypes == 0)
        {
            node_tex_musgrave_multi_fractal_4d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 1)
        {
            node_tex_musgrave_ridged_multi_fractal_4d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 2)
        {
            node_tex_musgrave_hybrid_multi_fractal_4d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 3)
        {
            node_tex_musgrave_fBm_4d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
        if (ddTypes == 4)
        {
            node_tex_musgrave_hetero_terrain_4d_float(co, w, scale, detail, dimension, lac, offset, gain, outfactor);
        }
    }

    return outfactor;
}