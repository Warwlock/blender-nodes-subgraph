float rgbtobw(float4 color)
{
	float3 factors = float3(0.2126, 0.7152, 0.0722);
	return dot(color.rgb, factors);
}