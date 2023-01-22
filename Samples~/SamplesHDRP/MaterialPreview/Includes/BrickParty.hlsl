#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void BrickParty_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float4 ColorOut)
{
	
	float4 _Mapping_8040 = float4(mapping_point(float4(_UV, 0), float3(0, 0, 0), float3(0, 0, 0), float3(1, 1, 1)), 0);
	float _Math_8026 = math_multiply(_Time, 10, 0.5);
	float _Math_8034 = math_fraction(_Math_8026, 1, 0.5);
	float4 _CombineHSV_8028 = combine_hsv(_Math_8034, 1, 1);
	float _Math_8036 = math_add(_Math_8026, 0.5, 0.5);
	float _Math_8032 = math_fraction(_Math_8036, 1, 0.5);
	float4 _CombineHSV_8018 = combine_hsv(_Math_8032, 1, 1);
	float _BrickParty_8030_fac; float4 _BrickParty_8030_col; node_tex_brick(_Mapping_8040, _CombineHSV_8028, _CombineHSV_8018, float4(0, 0, 0, 1), 8, 0.02, 0.1, 0, 0.5, 0.25, 0.5, 2, 1, 2, _BrickParty_8030_fac, _BrickParty_8030_col);

	ColorOut = _BrickParty_8030_col;
}