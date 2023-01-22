float4 node_blackbody(float Temperature)
{
    float4 color = float4(255.0, 255.0, 255.0, 0.0);
    color.x = 56100000. * pow(Temperature, (-3.0 / 2.0)) + 148.0;
    color.y = 100.04 * log(Temperature) - 623.6;
    if (Temperature > 6500.0) color.y = 35200000.0 * pow(Temperature, (-3.0 / 2.0)) + 184.0;
    color.z = 194.18 * log(Temperature) - 1448.6;
    color = clamp(color, 0.0, 255.0) / 255.0;
    if (Temperature < 1000.0) color *= Temperature / 1000.0;
    return color;
}