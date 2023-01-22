#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void CheckerView_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, out float4 ColorOut)
{
	
	float4 _Mapping_8040 = float4(mapping_point(float4(_SP, 0), float3(0, 0, 0), float3(0, 0, 0), float3(1, 1, 1)), 0);
	float _CheckerView_8042_fac; float4 _CheckerView_8042_col; node_tex_checker(_Mapping_8040, float4(0.4395247, 0.7830189, 0.6881176, 1), float4(0.8396226, 0.8063828, 0.6534799, 1), 30, _CheckerView_8042_fac, _CheckerView_8042_col);

	ColorOut = _CheckerView_8042_col;
}