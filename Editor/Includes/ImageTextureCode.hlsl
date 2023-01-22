SAMPLER(sampler_linear_repeat);
SAMPLER(sampler_point_repeat);
SAMPLER(sampler_trilinear_repeat);

SAMPLER(sampler_linear_clamp);
SAMPLER(sampler_point_clamp);
SAMPLER(sampler_trilinear_clamp);

SAMPLER(sampler_linear_mirror);
SAMPLER(sampler_point_mirror);
SAMPLER(sampler_trilinear_mirror);

SAMPLER(sampler_linear_mirroronce);
SAMPLER(sampler_point_mirroronce);
SAMPLER(sampler_trilinear_mirroronce);

float4 node_image_texture(Texture2D textureInput, float2 uv, float sNumber) 
{
	float4 color;
	if (sNumber == 0)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_linear_repeat, uv);
	if (sNumber == 1)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_point_repeat, uv);
	if (sNumber == 2)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_trilinear_repeat, uv);

	if (sNumber == 3)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_linear_clamp, uv);
	if (sNumber == 4)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_point_clamp, uv);
	if (sNumber == 5)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_trilinear_clamp, uv);

	if (sNumber == 6)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_linear_mirror, uv);
	if (sNumber == 7)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_point_mirror, uv);
	if (sNumber == 8)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_trilinear_mirror, uv);

	if (sNumber == 9)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_linear_mirroronce, uv);
	if (sNumber == 10)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_point_mirroronce, uv);
	if (sNumber == 11)
		color = SAMPLE_TEXTURE2D(textureInput, sampler_trilinear_mirroronce, uv);

	return color;
}

float4 node_image_texture_LOD(Texture2D textureInput, float2 uv, float sNumber, float LOD_Num)
{
	float4 color;
	if (sNumber == 0)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_linear_repeat, uv, LOD_Num);
	if (sNumber == 1)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_point_repeat, uv, LOD_Num);
	if (sNumber == 2)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_trilinear_repeat, uv, LOD_Num);

	if (sNumber == 3)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_linear_clamp, uv, LOD_Num);
	if (sNumber == 4)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_point_clamp, uv, LOD_Num);
	if (sNumber == 5)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_trilinear_clamp, uv, LOD_Num);

	if (sNumber == 6)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_linear_mirror, uv, LOD_Num);
	if (sNumber == 7)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_point_mirror, uv, LOD_Num);
	if (sNumber == 8)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_trilinear_mirror, uv, LOD_Num);

	if (sNumber == 9)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_linear_mirroronce, uv, LOD_Num);
	if (sNumber == 10)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_point_mirroronce, uv, LOD_Num);
	if (sNumber == 11)
		color = SAMPLE_TEXTURE2D_LOD(textureInput, sampler_trilinear_mirroronce, uv, LOD_Num);

	return color;
}