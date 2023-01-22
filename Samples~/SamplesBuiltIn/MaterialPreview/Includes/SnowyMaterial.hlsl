#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void SnowyMaterial_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float4 ColorOut, out float3 NormalOut)
{
	
	float _SeparateXYZ_8184_z = separate_z(float4(_POS, 0));
	float _Math_8174 = clamp_value(math_divide(_SeparateXYZ_8184_z, 0.8, 0.5), 0, 1);
	float _NoiseTexture_8186_fac; float4 _NoiseTexture_8186_col; node_noise_texture_full(_POS, 0, 50, 0, 0.5, 0.24, 2, _NoiseTexture_8186_fac, _NoiseTexture_8186_col);
	float _Math_8172 = math_multiply(_NoiseTexture_8186_fac, 0.5, 0.5);
	float _Math_8178 = clamp_value(math_multiply(_Math_8174, _Math_8172, 0.5), 0, 1);
	float _Math_8190 = clamp_value(math_multiply(_Math_8178, 2, 0.5), 0, 1);
	float4 _Invert_8180 = node_invert(1, _Math_8190);
	float4 _MixRGB_8182 = mix_blend(_Invert_8180, float4(1, 1, 1, 1), float4(0.0754717, 0.04266897, 0.02954788, 1));
	float _NoiseTexture_8194_fac; float4 _NoiseTexture_8194_col; node_noise_texture_full(_POS, 0, 6, 2, 0.5, 0, 2, _NoiseTexture_8194_fac, _NoiseTexture_8194_col);
	float4 _Bump_8196; node_bump(_POS, 1, 0.1, 1, _NoiseTexture_8194_fac, _NTS, _Bump_8196);

	ColorOut = _MixRGB_8182;
	NormalOut = _Bump_8196;
}