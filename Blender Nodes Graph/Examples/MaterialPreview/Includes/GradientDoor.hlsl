#include <Assets/Blender Nodes Graph/Scripts/Editor/Includes/Importers.hlsl>

void GradientDoor_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_8124, out float4 ColorOut, out float alphaOut)
{
	
	float _GradientTexture_8126_fac; float4 _GradientTexture_8126_col; node_tex_gradient(_POS, 4, _GradientTexture_8126_fac, _GradientTexture_8126_col);
	float4 _GradientDoor_8124 = color_ramp(gradient_8124, _GradientTexture_8126_fac);
	float _Math_8118 = math_multiply(_Time, 10, 0.5);
	float _Math_8122 = math_pingpong(_Math_8118, 1, 0.5);
	float _MapRange_8130 = clamp_value(map_range_linear(_Math_8122, 0, 1, -0.6, 0.6, 4), -0.6, 0.6);
	float _Math_8132 = math_add(_GradientTexture_8126_fac, _MapRange_8130, 0.5);

	ColorOut = _GradientDoor_8124;
	alphaOut = _Math_8132;
}