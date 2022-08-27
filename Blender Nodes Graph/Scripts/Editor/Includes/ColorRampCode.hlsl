SAMPLER(gradient_sampler_linear_clamp);

float4 color_ramp(Texture2D gradientTexture, float time)
{
    return SAMPLE_TEXTURE2D_LOD(gradientTexture, gradient_sampler_linear_clamp, float2(time, 0), 0);
}