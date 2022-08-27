#include <Assets/Blender Nodes Graph/Scripts/Editor/Includes/Importers.hlsl>

void Cracks_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_8110, out float3 VectorOut)
{
	
	float _VoronoiTexture_8104_dis; float4 _VoronoiTexture_8104_col; float3 _VoronoiTexture_8104_pos; float _VoronoiTexture_8104_w; float _VoronoiTexture_8104_rad; voronoi_tex_getValue(_POS, 0, 1, 1, 0.5, 1, 0, 2, 3, _VoronoiTexture_8104_dis, _VoronoiTexture_8104_col, _VoronoiTexture_8104_pos, _VoronoiTexture_8104_w, _VoronoiTexture_8104_rad);
	float4 _Cracks_8110 = color_ramp(gradient_8110, _VoronoiTexture_8104_dis);
	float4 _Mapping_8106 = float4(mapping_point(float4(_POS, 0), float3(0, 0, 0), float3(0, 0, 0), float3(1, 1, 1)), 0);
	float4 _VectorMath_8096 = float4(vector_math_scale(_Mapping_8106, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), 1), 1);
	float4 _VectorMath_8102 = float4(vector_math_scale(_Mapping_8106, float3(0.5, 0.5, 0.5), float3(0.5, 0.5, 0.5), 0.8), 1);
	float4 _MixRGB_8112 = mix_blend(_Cracks_8110, _VectorMath_8096, _VectorMath_8102);

	VectorOut = _MixRGB_8112;
}