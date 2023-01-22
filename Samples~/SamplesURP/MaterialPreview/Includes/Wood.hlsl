#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void Wood_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_8268, Texture2D gradient_8284, Texture2D gradient_8288, out float4 ColorOut, out float3 Normal)
{
	
	float4 _Mapping_8280 = float4(mapping_point(float4(_POS, 0), float3(0, 0, 0), float3(0, 0, 0), float3(5, 1, 1)), 0);
	float _SimpleNoiseTexture_8282_fac; float4 _SimpleNoiseTexture_8282_col; node_simple_noise_texture_full(_Mapping_8280, 0, 0.5, 6, 0.5, 0, 1, _SimpleNoiseTexture_8282_fac, _SimpleNoiseTexture_8282_col);
	float _NoiseTexture_8272_fac; float4 _NoiseTexture_8272_col; node_noise_texture_full(_SimpleNoiseTexture_8282_fac, 0, 5, 0, 0, 2, 2, _NoiseTexture_8272_fac, _NoiseTexture_8272_col);
	float4 _ColorRamp_8268 = color_ramp(gradient_8268, _NoiseTexture_8272_fac);
	float4 _MixRGB_8278 = mix_blend(0.4, _ColorRamp_8268, _SimpleNoiseTexture_8282_fac);
	float4 _Wood_8284 = color_ramp(gradient_8284, _MixRGB_8278);
	float4 _ColorRamp_8288 = color_ramp(gradient_8288, _MixRGB_8278);
	float4 _Bump_8286; node_bump(_POS, 1, 0.5, 0.15, _ColorRamp_8288, _NOS, _Bump_8286);

	ColorOut = _Wood_8284;
	Normal = _Bump_8286;
}