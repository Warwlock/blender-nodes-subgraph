float d0; float4 c0; float3 p0; float w0; float r0;

float filterwidth(float2 v)
{
	float2 fw = max(abs(ddx(v)), abs(ddy(v)));
	return max(fw.x, fw.y);
}

void mipmap_code(float3 coord, float resolution, out float3 OutVec)
{
	float width = filterwidth(float2(coord.x, coord.y));
	width = map_range_linear(width, 0, 1, 1, 0, 0);

	float multp = resolution;

	if (width > 0.98)
		voronoi_tex_getValue(uv, 0, multp * 2, 1, 0.5, 0, 0, 2, 0, d0, c0, p0, w0, r0);
	else if (width > 0.96)
		voronoi_tex_getValue(uv, 0, multp * 1, 1, 0.5, 0, 0, 2, 0, d0, c0, p0, w0, r0);
	else if (width > 0.9)
		voronoi_tex_getValue(uv, 0, multp * 0.5, 1, 0.5, 0, 0, 2, 0, d0, c0, p0, w0, r0);
	else if (width > 0.85)
		voronoi_tex_getValue(uv, 0, multp * 0.25, 1, 0.5, 0, 0, 2, 0, d0, c0, p0, w0, r0);
	else if (width > 0.65)
		voronoi_tex_getValue(uv, 0, multp * 0.125, 1, 0.5, 0, 0, 2, 0, d0, c0, p0, w0, r0);
	else if (width > 0.5)
		voronoi_tex_getValue(uv, 0, multp * 0.0625, 1, 0.5, 0, 0, 2, 0, d0, c0, p0, w0, r0);
	else if (width > 0.2)
		voronoi_tex_getValue(uv, 0, multp * 0.03125, 1, 0.5, 0, 0, 2, 0, d0, c0, p0, w0, r0);
	else
		voronoi_tex_getValue(uv, 0, multp * 0.015625, 1, 0.5, 0, 0, 2, 0, d0, c0, p0, w0, r0);

	OutVec = p0;
}