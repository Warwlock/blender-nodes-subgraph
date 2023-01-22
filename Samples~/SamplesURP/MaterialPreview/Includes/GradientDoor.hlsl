#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void GradientDoor_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_8090, out float4 ColorOut, out float alphaOut)
{
	
	float _GradientTexture_8092_fac; float4 _GradientTexture_8092_col; node_tex_gradient(_POS, 4, _GradientTexture_8092_fac, _GradientTexture_8092_col);
	float4 _GradientDoor_8090 = color_ramp(gradient_8090, _GradientTexture_8092_fac);
	float _Math_8084 = math_multiply(_Time, 10, 0.5);
	float _Math_8088 = math_pingpong(_Math_8084, 1, 0.5);
	float _MapRange_8096 = clamp_value(map_range_linear(_Math_8088, 0, 1, -0.6, 0.6, 4), -0.6, 0.6);
	float _Math_8098 = math_add(_GradientTexture_8092_fac, _MapRange_8096, 0.5);

	ColorOut = _GradientDoor_8090;
	alphaOut = _Math_8098;
}