// Float Math 
#define __MathUtil_

float mod(float x, float y)
{
    return x - y * floor(x / y);
}

float safe_divide(float a, float b)
{
    return (b != 0.0) ? a / b : 0.0;
}

// mod function compatible with OSL using nvidia reference example. 
float compatible_mod(float a, float b)
{
    float c = (b != 0.0) ? frac(abs(a / b)) * abs(b) : 0.0;
    return (a < 0.0) ? -c : c;
}

float compatible_pow(float x, float y)
{
    if (y == 0.0) { // x^0 -> 1, including 0^0 
        return 1.0;
    }

    // glsl pow doesn't accept negative x 
    if (x < 0.0) {
        if (mod(-y, 2.0) == 0.0) {
            return pow(-x, y);
        }
        else {
            return -pow(-x, y);
        }
    }
    else if (x == 0.0) {
        return 0.0;
    }

    return pow(x, y);
}

float wrap(float a, float b, float c)
{
    float range = b - c;
    return (range != 0.0) ? a - (range * floor((a - c) / range)) : c;
}

float3 wrap(float3 a, float3 b, float3 c)
{
    return float3(wrap(a.x, b.x, c.x), wrap(a.y, b.y, c.y), wrap(a.z, b.z, c.z));
}

float hypot(float x, float y)
{
    return sqrt(x * x + y * y);
}

int floor_to_int(float x)
{
    return int(floor(x));
}

int quick_floor(float x)
{
    return int(x) - ((x < 0) ? 1 : 0);
}

// Vector Math 

float2 safe_divide(float2 a, float2 b)
{
    return float2(safe_divide(a.x, b.x), safe_divide(a.y, b.y));
}

float3 safe_divide(float3 a, float3 b)
{
    return float3(safe_divide(a.x, b.x), safe_divide(a.y, b.y), safe_divide(a.z, b.z));
}

float4 safe_divide(float4 a, float4 b)
{
    return float4(
        safe_divide(a.x, b.x), safe_divide(a.y, b.y), safe_divide(a.z, b.z), safe_divide(a.w, b.w));
}

float2 safe_divide(float2 a, float b)
{
    return (b != 0.0) ? a / b : float2(0.0, 0.0);
}

float3 safe_divide(float3 a, float b)
{
    return (b != 0.0) ? a / b : float3(0.0, 0.0, 0.0);
}

float4 safe_divide(float4 a, float b)
{
    return (b != 0.0) ? a / b : float4(0.0, 0.0, 0.0, 0.0);
}

float3 compatible_fmod(float3 a, float3 b)
{
    return float3(compatible_mod(a.x, b.x), compatible_mod(a.y, b.y), compatible_mod(a.z, b.z));
}

void invert_z(float3 v, out float3 outv)
{
    v.z = -v.z;
    outv = v;
}

void vector_normalize(float3 normal, out float3 outnormal)
{
    outnormal = normalize(normal);
}

// Matirx Math 

float3x3 euler_to_mat3(float3 euler)
{
    float cx = cos(euler.x);
    float cy = cos(euler.y);
    float cz = cos(euler.z);
    float sx = sin(euler.x);
    float sy = sin(euler.y);
    float sz = sin(euler.z);

    float3x3 mat;
    mat[0][0] = cy * cz;
    mat[0][1] = cy * sz;
    mat[0][2] = -sy;

    mat[1][0] = sy * sx * cz - cx * sz;
    mat[1][1] = sy * sx * sz + cx * cz;
    mat[1][2] = cy * sx;

    mat[2][0] = sy * cx * cz + sx * sz;
    mat[2][1] = sy * cx * sz - sx * cz;
    mat[2][2] = cy * cx;
    return mat;
}

/*void direction_transform_m4v3(float3 vin, float4x4 mat, out float3 vout)
{
    vout = (mat * float4(vin, 0.0)).xyz;
}

void normal_transform_transposed_m4v3(float3 vin, float4x4 mat, out float3 vout)
{
    vout = transpose(float3x3(mat)) * vin;
}

void point_transform_m4v3(float3 vin, float4x4 mat, out float3 vout)
{
    vout = (mat * float4(vin, 1.0)).xyz;
}*/