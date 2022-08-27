#include <Assets/Blender Nodes Graph/Scripts/Editor/Includes/Importers.hlsl>

void BrickParty_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float4 ColorOut)
{
	
	float4 _Mapping_8054 = float4(mapping_point(float4(_UV, 0), float3(0, 0, 0), float3(0, 0, 0), float3(1, 1, 1)), 0);
	float _Math_8056 = math_multiply(_Time, 10, 0.5);
	float _Math_8064 = math_fraction(_Math_8056, 1, 0.5);
	float4 _CombineHSV_8058 = combine_hsv(_Math_8064, 1, 1);
	float _Math_8066 = math_add(_Math_8056, 0.5, 0.5);
	float _Math_8062 = math_fraction(_Math_8066, 1, 0.5);
	float4 _CombineHSV_8046 = combine_hsv(_Math_8062, 1, 1);
	float _BrickParty_8060_fac; float4 _BrickParty_8060_col; node_tex_brick(_Mapping_8054, _CombineHSV_8058, _CombineHSV_8046, float4(0, 0, 0, 1), 8, 0.02, 0.1, 0, 0.5, 0.25, 0.5, 2, 1, 2, _BrickParty_8060_fac, _BrickParty_8060_col);

	ColorOut = _BrickParty_8060_col;
}