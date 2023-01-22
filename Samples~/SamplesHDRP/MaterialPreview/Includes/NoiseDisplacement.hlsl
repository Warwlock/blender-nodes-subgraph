#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void NoiseDisplacement_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float3 VectorOut)
{
	
	float _Math_8136 = math_subtract(_Time, 4, 0.5);
	float _NoiseTexture_8140_fac; float4 _NoiseTexture_8140_col; node_noise_texture_full(_POS, _Math_8136, 5, 2, 0.7, 0, 3, _NoiseTexture_8140_fac, _NoiseTexture_8140_col);
	float4 _Mapping_8144 = float4(mapping_point(float4(_POS, 0), float3(0, 0, 0), float3(0, 0, 0), float3(1, 1, 1)), 0);
	float4 _VectorMath_8150 = float4(vector_math_scale(_Mapping_8144, float3(1, 1, 1), float3(0.5, 0.5, 0.5), 1.5), 1);
	float4 _VectorMath_8138 = float4(vector_math_scale(_Mapping_8144, float3(1, 1, 1), float3(0.5, 0.5, 0.5), 0.5), 1);
	float4 _MixRGB_8132 = mix_blend(_NoiseTexture_8140_fac, _VectorMath_8150, _VectorMath_8138);

	VectorOut = _MixRGB_8132;
}