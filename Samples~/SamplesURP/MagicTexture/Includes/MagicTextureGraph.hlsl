#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void MagicTextureGraph_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float4 ColorOut)
{
	
	float _Math_7964 = math_multiply(_Time, 2, 0.5);
	float4 _CombineXYZ_7960 = float4(combine_xyz(0, _Math_7964, 0), 0);
	float _Math_7962 = math_multiply(_Time, 60, 0.5);
	float4 _CombineXYZ_7958 = float4(combine_xyz(0, _Math_7962, 0), 0);
	float4 _Mapping_7948 = float4(mapping_point(float4(_POS, 0), _CombineXYZ_7960, _CombineXYZ_7958, float3(1, 1, 1)), 0);
	float _MagicTexture_7952_fac; float4 _MagicTexture_7952_col; node_tex_magic(_Mapping_7948, 2, 1, 8, _MagicTexture_7952_fac, _MagicTexture_7952_col);

	ColorOut = _MagicTexture_7952_col;
}