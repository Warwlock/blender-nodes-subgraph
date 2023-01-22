#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void NoiseFade_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float FadeValue)
{
	
	float _NoiseTexture_8150_fac; float4 _NoiseTexture_8150_col; node_noise_texture_full(_POS, 0, 15, 5, 0.5, 0, 2, _NoiseTexture_8150_fac, _NoiseTexture_8150_col);
	float _Math_8156 = math_multiply(_Time, 30, 0.5);
	float _Math_8152 = math_pingpong(_Math_8156, 2, 0.5);
	float _NoiseFade_8154 = clamp_value(map_range_linear(_Math_8152, 0, 2, 0.6, 2, 4), 0.6, 2);
	float _Math_8160 = math_multiply(_NoiseTexture_8150_fac, _NoiseFade_8154, 0.5);

	FadeValue = _Math_8160;
}