void node_tex_checker(float3 co, float4 color1, float4 color2, float scale, out float out_fac, out float4 out_col)
{
    float3 p = co * scale;

    /* Prevent precision issues on unit coordinates. */
    p = (p + 0.000001) * 0.999999;

    int xi = int(abs(floor(p.x)));
    int yi = int(abs(floor(p.y)));
    int zi = int(abs(floor(p.z)));

    bool check = ((fmod(xi, 2) == fmod(yi, 2)) == bool(fmod(zi, 2)));

    out_fac = check ? 1.0 : 0.0;
    out_col = check ? color1 : color2;
}