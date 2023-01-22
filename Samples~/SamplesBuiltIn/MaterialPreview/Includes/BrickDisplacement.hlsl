#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void BrickDisplacement_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float3 VectorOut)
{
	
	float4 _Mapping_8004 = float4(mapping_point(float4(_POS, 0), float3(0, 0, 0), float3(0, 0, 0), float3(1, 1, 1)), 0);
	float _BrickDisplacement_8008_fac; float4 _BrickDisplacement_8008_col; node_tex_brick(_Mapping_8004, float4(1, 1, 1, 1), float4(0.5, 0.5, 0.5, 1), float4(0, 0, 0, 1), 1, 0.02, 0.1, 0, 0.5, 0.25, 0.5, 2, 1, 2, _BrickDisplacement_8008_fac, _BrickDisplacement_8008_col);
	float4 _VectorMath_8010 = float4(vector_math_scale(_Mapping_8004, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), 1.24), 1);
	float4 _MixRGB_8012 = mix_blend(_BrickDisplacement_8008_fac, _VectorMath_8010, float4(0, 0, 0, 1));

	VectorOut = _MixRGB_8012;
}