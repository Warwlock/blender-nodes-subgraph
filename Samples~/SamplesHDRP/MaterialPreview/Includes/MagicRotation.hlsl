#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void MagicRotation_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float4 ColorOut)
{
	
	float _Math_8110 = math_multiply(_Time, 200, 0.5);
	float4 _MagicRotation_8112 = float4(combine_xyz(0, _Math_8110, 0), 0);
	float4 _Mapping_8128 = float4(mapping_point(float4(_UV, 0), float3(0, 0, 0), _MagicRotation_8112, float3(1, 1, 1)), 0);
	float _Math_8114 = math_multiply(_Time, 100, 0.5);
	float _Math_8116 = math_snap(_Math_8114, 0.5, 0.5);
	float _MapRange_8118 = clamp_value(map_range_linear(_Math_8116, -1, 1, 0.8, 2.5, 4), 0.8, 2.5);
	float _MagicTexture_8108_fac; float4 _MagicTexture_8108_col; node_tex_magic(_Mapping_8128, 3, _MapRange_8118, 3, _MagicTexture_8108_fac, _MagicTexture_8108_col);

	ColorOut = _MagicTexture_8108_col;
}