SAMPLER(my_sampler_linear_clamp);

float4 curveArray(float value, Texture2D myTexture)
{
	/*if (value >= 1)
		value = 0.99;
	if (value <= 0)
		value = 0.01;*/
	value = value * 16;
	if(value < 1)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value, 0.03125)), 1.000000);
	else if (value < 2)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 1, 0.09375)), 1.000000);
	else if (value < 3)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 2, 0.15625)), 1.000000);
	else if (value < 4)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 3, 0.21875)), 1.000000);
	else if (value < 5)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 4, 0.28125)), 1.000000);
	else if (value < 6)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 5, 0.34375)), 1.000000);
	else if (value < 7)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 6, 0.40625)), 1.000000);
	else if (value < 8)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 7, 0.46875)), 1.000000);
	else if (value < 9)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 8, 0.53125)), 1.000000);
	else if (value < 10)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 9, 0.59375)), 1.000000);
	else if (value < 11)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 10, 0.65625)), 1.000000);
	else if (value < 12)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 11, 0.71875)), 1.000000);
	else if (value < 13)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 12, 0.78125)), 1.000000);
	else if (value < 14)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 13, 0.84375)), 1.000000);
	else if (value < 15)
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 14, 0.90625)), 1.000000);
	else
		return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value - 15, 0.96875)), 1.000000);
}

float4 node_rgb_curves(float fac, float4 col, Texture2D _curveTexture)
{	
	fac = clamp(fac, 0, 1);
	float4 col2 = col;
	float4 outcol = float4(curveArray(col2.r, _curveTexture).a, curveArray(col2.g, _curveTexture).a, curveArray(col2.b, _curveTexture).a, 0);
	float4 outcol2 = float4(curveArray(outcol.r, _curveTexture).r, curveArray(outcol.g, _curveTexture).g, curveArray(outcol.b, _curveTexture).b, 0);
	outcol2 = outcol2;
	outcol2.a = col.a;
	return lerp(col, outcol2, fac);
}