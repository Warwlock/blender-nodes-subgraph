#ifndef __ColorUtil_
#include <ColorUtil.hlsl>
#endif

float4 hue_sat(float hue, float sat, float value, float fac, float4 col)
{
	float4 hsv;
	float4 outcol;

	rgb_to_hsv(col, hsv);

	hsv[0] = frac(hsv[0] + hue + 0.5);
	hsv[1] = clamp(hsv[1] * sat, 0.0, 1.0);
	hsv[2] = hsv[2] * value;

	hsv_to_rgb(hsv, outcol);

	return lerp(col, outcol, fac);
}