#include <Assets/Blender Nodes Graph/Scripts/Editor/Includes/Importers.hlsl>

void Asphalt_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, Texture2D gradient_8012, Texture2D gradient_8018, out float4 ColorOut, out float3 NormalOut)
{
	
	float _NoiseTexture_8022_fac; float4 _NoiseTexture_8022_col; node_noise_texture_full(_POS, 0, 8, 0, 0, 0, 2, _NoiseTexture_8022_fac, _NoiseTexture_8022_col);
	float4 _MixRGB_7998 = mix_blend(0.9, _NoiseTexture_8022_fac, float4(_POS, 0));
	float _VoronoiTexture_8020_dis; float4 _VoronoiTexture_8020_col; float3 _VoronoiTexture_8020_pos; float _VoronoiTexture_8020_w; float _VoronoiTexture_8020_rad; voronoi_tex_getValue(_MixRGB_7998, 0, 2, 1, 0.5, 1, 0, 1, 3, _VoronoiTexture_8020_dis, _VoronoiTexture_8020_col, _VoronoiTexture_8020_pos, _VoronoiTexture_8020_w, _VoronoiTexture_8020_rad);
	float4 _Asphalt_8018 = color_ramp(gradient_8018, _VoronoiTexture_8020_dis);
	float _NoiseTexture_8006_fac; float4 _NoiseTexture_8006_col; node_noise_texture_full(float4(_POS, 0), 0, 4, 0, 0, 0, 2, _NoiseTexture_8006_fac, _NoiseTexture_8006_col);
	float4 _MixRGB_8002 = mix_blend(_NoiseTexture_8006_fac, float4(0.009721218, 0.009721218, 0.009721218, 1), float4(0.05951124, 0.05951124, 0.05951124, 1));
	float4 _MixRGB_8016 = mix_blend(_Asphalt_8018, float4(0.009134057, 0.009134057, 0.009134057, 1), _MixRGB_8002);
	float _NoiseTexture_8000_fac; float4 _NoiseTexture_8000_col; node_noise_texture_full(_POS, 0, 250, 0, 0, 0, 2, _NoiseTexture_8000_fac, _NoiseTexture_8000_col);
	float4 _Asphalt_8012 = color_ramp(gradient_8012, _VoronoiTexture_8020_dis);
	float4 _Bump_8024; node_bump(_POS, 1, 0.56, 1, _Asphalt_8012, _NOS, _Bump_8024);
	float4 _Bump_8014; node_bump(_POS, 1, 0.24, 1, _NoiseTexture_8000_fac, _Bump_8024, _Bump_8014);

	ColorOut = _MixRGB_8016;
	NormalOut = _Bump_8014;
}