float4 node_invert(float fac, float4 col) 
{
	float4 outcol;
	outcol = lerp(col, float4(1.0, 1.0, 1.0, 1.0) - col, fac);
	outcol.w = col.w;
	return outcol;
}