#ifndef __ColorUtil_
#include <ColorUtil.hlsl>
#endif

float4 combine_rgba(float r, float g, float b, float a)
{
	return float4(r, g, b, a);
}

float4 combine_hsv(float h, float s, float v)
{
	float4 col;
	hsv_to_rgb(float4(h, s, v, 1.0), col);
	return col;
}

float3 combine_xyz(float x, float y, float z)
{
	return float3(x, y, z);
}