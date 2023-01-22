float4 brightness_contrast(float4 col, float brightness, float contrast)
{
	float a = 1.0 + contrast;
	float b = brightness - contrast * 0.5;

	float4 outcol;
	outcol.r = max(a * col.r + b, 0.0);
	outcol.g = max(a * col.g + b, 0.0);
	outcol.b = max(a * col.b + b, 0.0);
	outcol.a = col.a;
	return outcol;
}