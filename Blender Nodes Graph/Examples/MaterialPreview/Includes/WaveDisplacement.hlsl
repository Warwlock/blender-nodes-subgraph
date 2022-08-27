#include <Assets/Blender Nodes Graph/Scripts/Editor/Includes/Importers.hlsl>

void WaveDisplacement_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float3 VectorOut)
{
	
	float _Math_8276 = math_multiply(_Time, -5, 0.5);
	float4 _CombineXYZ_8264 = float4(combine_xyz(_Math_8276, 0, 0), 0);
	float4 _Mapping_8262 = float4(mapping_point(float4(_POS, 0), _CombineXYZ_8264, float3(0, 0, 0), float3(1, 1, 1)), 0);
	float _WaveTexture_8258_fac; float4 _WaveTexture_8258_col; node_tex_wave(_Mapping_8262, 1.5, 0, 2, 1, 0.5, 0, 0, 0, 0, 0, _WaveTexture_8258_col, _WaveTexture_8258_fac);
	float4 _VectorMath_8260 = float4(vector_math_scale(float4(_POS, 0), float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), 1.1), 1);
	float4 _VectorMath_8268 = float4(vector_math_scale(float4(_POS, 0), float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), 0.9), 1);
	float4 _MixRGB_8266 = mix_blend(_WaveTexture_8258_fac, _VectorMath_8260, _VectorMath_8268);

	VectorOut = _MixRGB_8266;
}