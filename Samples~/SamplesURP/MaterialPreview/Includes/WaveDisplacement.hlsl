#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void WaveDisplacement_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float3 VectorOut)
{
	
	float _Math_8242 = math_multiply(_Time, -5, 0.5);
	float4 _CombineXYZ_8230 = float4(combine_xyz(_Math_8242, 0, 0), 0);
	float4 _Mapping_8228 = float4(mapping_point(float4(_POS, 0), _CombineXYZ_8230, float3(0, 0, 0), float3(1, 1, 1)), 0);
	float _WaveTexture_8224_fac; float4 _WaveTexture_8224_col; node_tex_wave(_Mapping_8228, 1.5, 0, 2, 1, 0.5, 0, 0, 0, 0, 0, _WaveTexture_8224_col, _WaveTexture_8224_fac);
	float4 _VectorMath_8226 = float4(vector_math_scale(float4(_POS, 0), float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), 1.1), 1);
	float4 _VectorMath_8234 = float4(vector_math_scale(float4(_POS, 0), float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), 0.9), 1);
	float4 _MixRGB_8232 = mix_blend(_WaveTexture_8224_fac, _VectorMath_8226, _VectorMath_8234);

	VectorOut = _MixRGB_8232;
}