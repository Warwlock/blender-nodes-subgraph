#ifndef __MathUtil_
#include <MathUtil.hlsl>
#endif

/*Main Code
void mapping_mat4(
    float3 vec, float4 m0, float4 m1, float4 m2, float4 m3, float3 minvec, float3 maxvec, out float3 outvec)
{
    float4x4 mat = float4x4(m0, m1, m2, m3);
    outvec = (mat * float4(vec, 1.0)).xyz;
    outvec = clamp(outvec, minvec, maxvec);
}*/

float3 mapping_point(float3 vecto, float3 location, float3 rotation, float3 scale)
{
    return (mul(euler_to_mat3(radians(rotation)), (vecto * scale))) + location;
}

float3 mapping_texture(float3 vecto, float3 location, float3 rotation, float3 scale)
{
    return safe_divide(mul(transpose(euler_to_mat3(radians(rotation))), (vecto - location)), scale);
}

float3 mapping_vector(float3 vecto, float3 location, float3 rotation, float3 scale)
{
    return mul(euler_to_mat3(radians(rotation)), (vecto * scale));
}

float3 mapping_normal(float3 vecto, float3 location, float3 rotation, float3 scale)
{
    return normalize(mul(euler_to_mat3(radians(rotation)), safe_divide(vecto, scale)));
}