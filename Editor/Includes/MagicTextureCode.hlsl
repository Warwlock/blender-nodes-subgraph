void node_tex_magic(
    float3 co, float scale, float distortion, float depth, out float fac, out float4 color)
{
    float3 p = co * scale;
    float x = sin((p.x + p.y + p.z) * 5.0);
    float y = cos((-p.x + p.y - p.z) * 5.0);
    float z = -cos((-p.x - p.y + p.z) * 5.0);

    if (depth > 0) {
        x *= distortion;
        y *= distortion;
        z *= distortion;
        y = -cos(x - y + z);
        y *= distortion;
        if (depth > 1) {
            x = cos(x - y - z);
            x *= distortion;
            if (depth > 2) {
                z = sin(-x - y - z);
                z *= distortion;
                if (depth > 3) {
                    x = -cos(-x + y - z);
                    x *= distortion;
                    if (depth > 4) {
                        y = -sin(-x + y + z);
                        y *= distortion;
                        if (depth > 5) {
                            y = -cos(-x + y + z);
                            y *= distortion;
                            if (depth > 6) {
                                x = cos(x + y + z);
                                x *= distortion;
                                if (depth > 7) {
                                    z = sin(x + y - z);
                                    z *= distortion;
                                    if (depth > 8) {
                                        x = -cos(-x - y + z);
                                        x *= distortion;
                                        if (depth > 9) {
                                            y = -sin(x - y + z);
                                            y *= distortion;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    if (distortion != 0.0) {
        distortion *= 2.0;
        x /= distortion;
        y /= distortion;
        z /= distortion;
    }

    color = float4(0.5 - x, 0.5 - y, 0.5 - z, 1.0);
    fac = (color.x + color.y + color.z) / 3.0;
}