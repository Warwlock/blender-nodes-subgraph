#include <Assets/Blender Nodes Graph/Scripts/Editor/Includes/Importers.hlsl>

void Wood_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_8302, Texture2D gradient_8318, Texture2D gradient_8322, out float4 ColorOut, out float3 Normal)
{
	
	float4 _Mapping_8314 = float4(mapping_point(float4(_POS, 0), float3(0, 0, 0), float3(0, 0, 0), float3(5, 1, 1)), 0);
	float _SimpleNoiseTexture_8316_fac; float4 _SimpleNoiseTexture_8316_col; node_simple_noise_texture_full(_Mapping_8314, 0, 0.5, 6, 0.5, 0, 1, _SimpleNoiseTexture_8316_fac, _SimpleNoiseTexture_8316_col);
	float _NoiseTexture_8306_fac; float4 _NoiseTexture_8306_col; node_noise_texture_full(_SimpleNoiseTexture_8316_fac, 0, 5, 0, 0, 2, 2, _NoiseTexture_8306_fac, _NoiseTexture_8306_col);
	float4 _ColorRamp_8302 = color_ramp(gradient_8302, _NoiseTexture_8306_fac);
	float4 _MixRGB_8312 = mix_blend(0.4, _ColorRamp_8302, _SimpleNoiseTexture_8316_fac);
	float4 _Wood_8318 = color_ramp(gradient_8318, _MixRGB_8312);
	float4 _ColorRamp_8322 = color_ramp(gradient_8322, _MixRGB_8312);
	float4 _Bump_8320; node_bump(_POS, 1, 0.5, 0.15, _ColorRamp_8322, _NOS, _Bump_8320);

	ColorOut = _Wood_8318;
	Normal = _Bump_8320;
}