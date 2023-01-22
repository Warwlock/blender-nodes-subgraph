#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void Asphalt_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_7976, Texture2D gradient_7980, out float4 ColorOut, out float3 NormalOut)
{
	
	float _NoiseTexture_7984_fac; float4 _NoiseTexture_7984_col; node_noise_texture_full(_POS, 0, 8, 0, 0, 0, 2, _NoiseTexture_7984_fac, _NoiseTexture_7984_col);
	float4 _MixRGB_7964 = mix_blend(0.9, _NoiseTexture_7984_fac, float4(_POS, 0));
	float _VoronoiTexture_7982_dis; float4 _VoronoiTexture_7982_col; float3 _VoronoiTexture_7982_pos; float _VoronoiTexture_7982_w; float _VoronoiTexture_7982_rad; voronoi_tex_getValue(_MixRGB_7964, 0, 2, 1, 0.5, 1, 0, 1, 3, _VoronoiTexture_7982_dis, _VoronoiTexture_7982_col, _VoronoiTexture_7982_pos, _VoronoiTexture_7982_w, _VoronoiTexture_7982_rad);
	float4 _Asphalt_7980 = color_ramp(gradient_7980, _VoronoiTexture_7982_dis);
	float _NoiseTexture_7970_fac; float4 _NoiseTexture_7970_col; node_noise_texture_full(float4(_POS, 0), 0, 4, 0, 0, 0, 2, _NoiseTexture_7970_fac, _NoiseTexture_7970_col);
	float4 _MixRGB_7986 = mix_blend(_NoiseTexture_7970_fac, float4(0.009721218, 0.009721218, 0.009721218, 1), float4(0.05951124, 0.05951124, 0.05951124, 1));
	float4 _MixRGB_7988 = mix_blend(_Asphalt_7980, float4(0.009134057, 0.009134057, 0.009134057, 1), _MixRGB_7986);
	float _NoiseTexture_7966_fac; float4 _NoiseTexture_7966_col; node_noise_texture_full(_POS, 0, 250, 0, 0, 0, 2, _NoiseTexture_7966_fac, _NoiseTexture_7966_col);
	float4 _Asphalt_7976 = color_ramp(gradient_7976, _VoronoiTexture_7982_dis);
	float4 _Bump_7990; node_bump(_POS, 1, 0.56, 1, _Asphalt_7976, _NTS, _Bump_7990);
	float4 _Bump_7978; node_bump(_POS, 1, 0.24, 1, _NoiseTexture_7966_fac, _Bump_7990, _Bump_7978);

	ColorOut = _MixRGB_7988;
	NormalOut = _Bump_7978;
}