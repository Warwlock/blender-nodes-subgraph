#ifndef __ColorUtil_
#include <ColorUtil.hlsl>
#endif

float separate_x(float3 vec)
{
	return vec.r;
}

float separate_y(float3 vec)
{
	return vec.g;
}

float separate_z(float3 vec)
{
	return vec.b;
}

float separate_r(float4 col)
{
	return col.r;
}

float separate_g(float4 col)
{
	return col.g;
}

float separate_b(float4 col)
{
	return col.b;
}

float separate_a(float4 col)
{
	return col.a;
}

float separate_h(float4 col)
{
	float4 hsv;
	rgb_to_hsv(col, hsv);
	return hsv.r;
}

float separate_s(float4 col)
{
	float4 hsv;
	rgb_to_hsv(col, hsv);
	return hsv.g;
}

float separate_v(float4 col)
{
	float4 hsv;
	rgb_to_hsv(col, hsv);
	return hsv.b;
}