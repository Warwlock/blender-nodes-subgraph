void node_normal_map(float4 colorIn, float strength, out float4 NormalOut)
{
	float3 unpacked = UnpackNormal(colorIn);
	NormalOut = float4(unpacked.rg * strength, lerp(1, unpacked.b, saturate(strength)), 0);
}