#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void StoneBricks_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_8208, Texture2D gradient_8210, out float4 ColorOut, out float3 Normal)
{
	
	float4 _Mapping_8198 = float4(mapping_point(float4(_UV, 0), float3(0, 0, 0), float3(0, 0, 0), float3(1, 1, 1)), 0);
	float _SimpleNoiseTexture_8202_fac; float4 _SimpleNoiseTexture_8202_col; node_simple_noise_texture_full(_Mapping_8198, 0, 10, 5, 0.7, 0, 1, _SimpleNoiseTexture_8202_fac, _SimpleNoiseTexture_8202_col);
	float4 _ColorRamp_8210 = color_ramp(gradient_8210, _SimpleNoiseTexture_8202_fac);
	float4 _ColorRamp_8208 = color_ramp(gradient_8208, _SimpleNoiseTexture_8202_fac);
	float _SimpleNoiseTexture_8200_fac; float4 _SimpleNoiseTexture_8200_col; node_simple_noise_texture_full(_Mapping_8198, 0, 40, 2, 0.5, 0, 1, _SimpleNoiseTexture_8200_fac, _SimpleNoiseTexture_8200_col);
	float _StoneBricks_8196_fac; float4 _StoneBricks_8196_col; node_tex_brick(_Mapping_8198, float4(0.4793202, 0.08021983, 0.06662595, 1), float4(0.2158605, 0.09305898, 0.07036011, 1), float4(0.08865561, 0.07421358, 0.05951124, 1), 10, 0.02, _SimpleNoiseTexture_8200_fac, 0, 0.5, 0.25, 0.5, 2, 1, 2, _StoneBricks_8196_fac, _StoneBricks_8196_col);
	float4 _MixRGB_8204 = mix_dark(_ColorRamp_8208, _StoneBricks_8196_col, float4(0.04806327, 0.01846059, 0.01225591, 1));
	float4 _MixRGB_8206 = mix_blend(_ColorRamp_8210, _MixRGB_8204, float4(0.3277781, 0.2422812, 0.1470273, 1));
	float4 _Bump_8216; node_bump(_POS, -1, 0.3, 0.1, _StoneBricks_8196_fac, _NOS, _Bump_8216);
	float4 _Bump_8218; node_bump(_POS, 1, 0.1, 0.1, _ColorRamp_8208, _Bump_8216, _Bump_8218);
	float4 _Bump_8220; node_bump(_POS, -1, 0.3, 0.2, _ColorRamp_8210, _Bump_8218, _Bump_8220);

	ColorOut = _MixRGB_8206;
	Normal = _Bump_8220;
}