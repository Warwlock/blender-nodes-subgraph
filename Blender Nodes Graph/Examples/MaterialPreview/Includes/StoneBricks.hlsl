#include <Assets/Blender Nodes Graph/Scripts/Editor/Includes/Importers.hlsl>

void StoneBricks_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_8242, Texture2D gradient_8244, out float4 ColorOut, out float3 Normal)
{
	
	float4 _Mapping_8232 = float4(mapping_point(float4(_UV, 0), float3(0, 0, 0), float3(0, 0, 0), float3(1, 1, 1)), 0);
	float _SimpleNoiseTexture_8236_fac; float4 _SimpleNoiseTexture_8236_col; node_simple_noise_texture_full(_Mapping_8232, 0, 10, 5, 0.7, 0, 1, _SimpleNoiseTexture_8236_fac, _SimpleNoiseTexture_8236_col);
	float4 _ColorRamp_8244 = color_ramp(gradient_8244, _SimpleNoiseTexture_8236_fac);
	float4 _ColorRamp_8242 = color_ramp(gradient_8242, _SimpleNoiseTexture_8236_fac);
	float _SimpleNoiseTexture_8234_fac; float4 _SimpleNoiseTexture_8234_col; node_simple_noise_texture_full(_Mapping_8232, 0, 40, 2, 0.5, 0, 1, _SimpleNoiseTexture_8234_fac, _SimpleNoiseTexture_8234_col);
	float _StoneBricks_8230_fac; float4 _StoneBricks_8230_col; node_tex_brick(_Mapping_8232, float4(0.4793202, 0.08021983, 0.06662595, 1), float4(0.2158605, 0.09305898, 0.07036011, 1), float4(0.08865561, 0.07421358, 0.05951124, 1), 10, 0.02, _SimpleNoiseTexture_8234_fac, 0, 0.5, 0.25, 0.5, 2, 1, 2, _StoneBricks_8230_fac, _StoneBricks_8230_col);
	float4 _MixRGB_8238 = mix_dark(_ColorRamp_8242, _StoneBricks_8230_col, float4(0.04806327, 0.01846059, 0.01225591, 1));
	float4 _MixRGB_8240 = mix_blend(_ColorRamp_8244, _MixRGB_8238, float4(0.3277781, 0.2422812, 0.1470273, 1));
	float4 _Bump_8250; node_bump(_POS, -1, 0.3, 0.1, _StoneBricks_8230_fac, _NOS, _Bump_8250);
	float4 _Bump_8252; node_bump(_POS, 1, 0.1, 0.1, _ColorRamp_8242, _Bump_8250, _Bump_8252);
	float4 _Bump_8254; node_bump(_POS, -1, 0.3, 0.2, _ColorRamp_8244, _Bump_8252, _Bump_8254);

	ColorOut = _MixRGB_8240;
	Normal = _Bump_8254;
}