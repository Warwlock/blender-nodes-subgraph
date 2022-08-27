#include <Assets/Blender Nodes Graph/Scripts/Editor/Includes/Importers.hlsl>

void MagicTextureGraph_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float4 ColorOut)
{
	
	float _Math_31902 = math_multiply(_Time, 60, 0.5);
	float4 _CombineXYZ_32282 = float4(combine_xyz(0, _Math_31902, 0), 0);
	float4 _Mapping_32084 = float4(mapping_point(float4(_POS, 0), float3(0, 0, 0), _CombineXYZ_32282, float3(1, 1, 1)), 0);
	float _MagicTexture_32030_fac; float4 _MagicTexture_32030_col; node_tex_magic(_Mapping_32084, 2, 1, 8, _MagicTexture_32030_fac, _MagicTexture_32030_col);

	ColorOut = _MagicTexture_32030_col;
}