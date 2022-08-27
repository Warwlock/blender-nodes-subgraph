#include <Assets/Blender Nodes Graph/Scripts/Editor/Includes/Importers.hlsl>

void SnowyMaterial_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float4 ColorOut, out float3 NormalOut)
{
	
	float _SeparateXYZ_8212_z = separate_z(float4(_POS, 0));
	float _Math_8202 = clamp_value(math_divide(_SeparateXYZ_8212_z, 0.8, 0.5), 0, 1);
	float _NoiseTexture_8214_fac; float4 _NoiseTexture_8214_col; node_noise_texture_full(_POS, 0, 50, 0, 0.5, 0.24, 2, _NoiseTexture_8214_fac, _NoiseTexture_8214_col);
	float _Math_8200 = math_multiply(_NoiseTexture_8214_fac, 0.5, 0.5);
	float _Math_8206 = clamp_value(math_multiply(_Math_8202, _Math_8200, 0.5), 0, 1);
	float _Math_8218 = clamp_value(math_multiply(_Math_8206, 2, 0.5), 0, 1);
	float4 _Invert_8208 = node_invert(1, _Math_8218);
	float4 _MixRGB_8210 = mix_blend(_Invert_8208, float4(1, 1, 1, 1), float4(0.0754717, 0.04266897, 0.02954788, 1));
	float _NoiseTexture_8222_fac; float4 _NoiseTexture_8222_col; node_noise_texture_full(_POS, 0, 6, 2, 0.5, 0, 2, _NoiseTexture_8222_fac, _NoiseTexture_8222_col);
	float4 _Bump_8224; node_bump(_POS, 1, 0.1, 1, _NoiseTexture_8222_fac, _NTS, _Bump_8224);

	ColorOut = _MixRGB_8210;
	NormalOut = _Bump_8224;
}