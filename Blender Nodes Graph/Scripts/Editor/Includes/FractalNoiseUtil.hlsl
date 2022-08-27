/* The fractal_noise functions are all exactly the same except for the input type. */
#ifndef __NoiseUtil_
#include <NoiseUtil.hlsl>
#endif

#define __FractalNoiseUtil_

float fractal_noise(float p, float octaves, float roughness)
{
    float fscale = 1.0;
    float amp = 1.0;
    float maxamp = 0.0;
    float sum = 0.0;
    octaves = clamp(octaves, 0.0, 16.0);
    int n = int(octaves);
    for (int i = 0; i <= n; i++) {
        float t = noise(fscale * p);
        sum += t * amp;
        maxamp += amp;
        amp *= clamp(roughness, 0.0, 1.0);
        fscale *= 2.0;
    }
    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float t = noise(fscale * p);
        float sum2 = sum + t * amp;
        sum /= maxamp;
        sum2 /= maxamp + amp;
        return (1.0 - rmd) * sum + rmd * sum2;
    }
    else {
        return sum / maxamp;
    }
}

/* The fractal_noise functions are all exactly the same except for the input type. */
float fractal_noise(float2 p, float octaves, float roughness)
{
    float fscale = 1.0;
    float amp = 1.0;
    float maxamp = 0.0;
    float sum = 0.0;
    octaves = clamp(octaves, 0.0, 16.0);
    int n = int(octaves);
    for (int i = 0; i <= n; i++) {
        float t = noise(fscale * p);
        sum += t * amp;
        maxamp += amp;
        amp *= clamp(roughness, 0.0, 1.0);
        fscale *= 2.0;
    }
    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float t = noise(fscale * p);
        float sum2 = sum + t * amp;
        sum /= maxamp;
        sum2 /= maxamp + amp;
        return (1.0 - rmd) * sum + rmd * sum2;
    }
    else {
        return sum / maxamp;
    }
}

/* The fractal_noise functions are all exactly the same except for the input type. */
float fractal_noise(float3 p, float octaves, float roughness)
{
    float fscale = 1.0;
    float amp = 1.0;
    float maxamp = 0.0;
    float sum = 0.0;
    octaves = clamp(octaves, 0.0, 16.0);
    int n = int(octaves);
    for (int i = 0; i <= n; i++) {
        float t = noise(fscale * p);
        sum += t * amp;
        maxamp += amp;
        amp *= clamp(roughness, 0.0, 1.0);
        fscale *= 2.0;
    }
    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float t = noise(fscale * p);
        float sum2 = sum + t * amp;
        sum /= maxamp;
        sum2 /= maxamp + amp;
        return (1.0 - rmd) * sum + rmd * sum2;
    }
    else {
        return sum / maxamp;
    }
}

/* The fractal_noise functions are all exactly the same except for the input type. */
float fractal_noise(float4 p, float octaves, float roughness)
{
    float fscale = 1.0;
    float amp = 1.0;
    float maxamp = 0.0;
    float sum = 0.0;
    octaves = clamp(octaves, 0.0, 16.0);
    int n = int(octaves);
    for (int i = 0; i <= n; i++) {
        float t = noise(fscale * p);
        sum += t * amp;
        maxamp += amp;
        amp *= clamp(roughness, 0.0, 1.0);
        fscale *= 2.0;
    }
    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float t = noise(fscale * p);
        float sum2 = sum + t * amp;
        sum /= maxamp;
        sum2 /= maxamp + amp;
        return (1.0 - rmd) * sum + rmd * sum2;
    }
    else {
        return sum / maxamp;
    }
}