#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void Cloth_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float4 ColorOut, out float3 Normal)
{
	
	float _MagicTexture_8056_fac; float4 _MagicTexture_8056_col; node_tex_magic(_POS, 70, 0.5, 5, _MagicTexture_8056_fac, _MagicTexture_8056_col);
	float4 _Bump_8058; node_bump(_POS, -1, 0.3, 0.1, _MagicTexture_8056_col, _NOS, _Bump_8058);

	ColorOut = float4(0.8217627, 0.224549, 0.06443264, 1);
	Normal = _Bump_8058;
}