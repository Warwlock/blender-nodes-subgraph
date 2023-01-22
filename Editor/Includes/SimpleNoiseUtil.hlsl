#ifndef _INCLUDE_NOISEUTILS_
#define _INCLUDE_NOISEUTILS_

float Snoise_scale1(float result)
{
    return 0.2500 * result;
}

float Snoise_scale2(float result)
{
    return 0.6616 * result;
}

float Snoise_scale3(float result)
{
    return 0.9820 * result;
}

float Snoise_scale4(float result)
{
    return 0.8344 * result;
}

/* Safe Signed And Unsigned Noise */

float Ssnoise(float p)
{
    float r = 0;// noise_perlin(p);
    return (isinf(r)) ? 0.0 : Snoise_scale1(r);
}

float Snoise(float p)
{
    return 0.5 * Ssnoise(p) + 0.5;
}

float Ssnoise(float2 p)
{
    float r = snoise2D(p);
    return (isinf(r)) ? 0.0 : Snoise_scale2(r);
}

float Snoise(float2 p)
{
    return 0.5 * Ssnoise(p) + 0.5;
}

float Ssnoise(float3 p)
{
    float r = snoise3D(p);
    return (isinf(r)) ? 0.0 : Snoise_scale3(r);
}

float Snoise(float3 p)
{
    return 0.5 * Ssnoise(p) + 0.5;
}

float Ssnoise(float4 p)
{
    float r = snoise4D(p);
    return (isinf(r)) ? 0.0 : Snoise_scale4(r);
}

float Snoise(float4 p)
{
    return 0.5 * Ssnoise(p) + 0.5;
}

/* The fractal_noise functions are all exactly the same except for the input type. */
float Sfractal_noise(float2 p, float octaves, float roughness)
{
    float fscale = 1.0;
    float amp = 1.0;
    float maxamp = 0.0;
    float sum = 0.0;
    octaves = clamp(octaves, 0.0, 16.0);
    int n = int(octaves);
    for (int i = 0; i <= n; i++) {
        float t = Snoise(fscale * p);
        sum += t * amp;
        maxamp += amp;
        amp *= clamp(roughness, 0.0, 1.0);
        fscale *= 2.0;
    }
    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float t = Snoise(fscale * p);
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
float Sfractal_noise(float3 p, float octaves, float roughness)
{
    float fscale = 1.0;
    float amp = 1.0;
    float maxamp = 0.0;
    float sum = 0.0;
    octaves = clamp(octaves, 0.0, 16.0);
    int n = int(octaves);
    for (int i = 0; i <= n; i++) {
        float t = Snoise(fscale * p);
        sum += t * amp;
        maxamp += amp;
        amp *= clamp(roughness, 0.0, 1.0);
        fscale *= 2.0;
    }
    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float t = Snoise(fscale * p);
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
float Sfractal_noise(float4 p, float octaves, float roughness)
{
    float fscale = 1.0;
    float amp = 1.0;
    float maxamp = 0.0;
    float sum = 0.0;
    octaves = clamp(octaves, 0.0, 16.0);
    int n = int(octaves);
    for (int i = 0; i <= n; i++) {
        float t = Snoise(fscale * p);
        sum += t * amp;
        maxamp += amp;
        amp *= clamp(roughness, 0.0, 1.0);
        fscale *= 2.0;
    }
    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float t = Snoise(fscale * p);
        float sum2 = sum + t * amp;
        sum /= maxamp;
        sum2 /= maxamp + amp;
        return (1.0 - rmd) * sum + rmd * sum2;
    }
    else {
        return sum / maxamp;
    }
}

#endif