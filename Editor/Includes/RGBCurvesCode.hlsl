SAMPLER(my_sampler_linear_clamp);

float4 curveArray(float value, Texture2D myTexture)
{
	/*if (value >= 1)
		value = 0.99;
	if (value <= 0)
		value = 0.01;*/
	return pow(SAMPLE_TEXTURE2D(myTexture, my_sampler_linear_clamp, float2(value, 0.5)), 1.000000);
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