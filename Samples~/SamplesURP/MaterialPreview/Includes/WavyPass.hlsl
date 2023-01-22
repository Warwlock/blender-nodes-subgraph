#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void WavyPass_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_8264, out float4 ColorOut)
{
	
	float _Math_8254 = math_subtract(_Time, 2, 0.5);
	float4 _CombineXYZ_8252 = float4(combine_xyz(0, _Math_8254, 0), 0);
	float4 _Mapping_8250 = float4(mapping_point(float4(_POS, 0), _CombineXYZ_8252, float3(0, 0, 0), float3(1, 1, 1)), 0);
	float _WaveTexture_8258_fac; float4 _WaveTexture_8258_col; node_tex_wave(_Mapping_8250, 3, 0, 2, 1, 0.5, 0, 0, 1, 0, 0, _WaveTexture_8258_col, _WaveTexture_8258_fac);
	float4 _WavyPass_8264 = color_ramp(gradient_8264, _WaveTexture_8258_fac);

	ColorOut = _WavyPass_8264;
}