#ifndef __MathUtil_
#include <MathUtil.hlsl>
#endif

float3 vector_math_add(float3 a, float3 b, float3 c, float scale)
{
    return a + b;
}

float3 vector_math_subtract(
    float3 a, float3 b, float3 c, float scale)
{
    return a - b;
}

float3 vector_math_multiply(
    float3 a, float3 b, float3 c, float scale)
{
    return a * b;
}

float3 vector_math_divide(
    float3 a, float3 b, float3 c, float scale)
{
    return safe_divide(a, b);
}

float3 vector_math_cross(float3 a, float3 b, float3 c, float scale)
{
    return cross(a, b);
}

float3 vector_math_project(
    float3 a, float3 b, float3 c, float scale)
{
    float lenSquared = dot(b, b);
    return (lenSquared != 0.0) ? (dot(a, b) / lenSquared) * b : float3(0.0, 0.0, 0.0);
}

float3 vector_math_reflect(
    float3 a, float3 b, float3 c, float scale)
{
    return reflect(a, normalize(b));
}

//VALUES
float vector_math_dot(float3 a, float3 b, float3 c, float scale)
{
    return dot(a, b);
}

float vector_math_distance(
    float3 a, float3 b, float3 c, float scale)
{
    return distance(a, b);
}

float vector_math_length(
    float3 a, float3 b, float3 c, float scale)
{
    return length(a);
}
//END VALUES

float3 vector_math_scale(float3 a, float3 b, float3 c, float scale)
{
    return a * scale;
}

float3 vector_math_normalize(
    float3 a, float3 b, float3 c, float scale)
{
    float3 outV = a;
    /* Safe version of normalize(a). */
    float lenSquared = dot(a, a);
    if (lenSquared > 0.0) {
        return outV * rsqrt(lenSquared);
    }
    else 
    {
        return 0;
    }
}

float3 vector_math_snap(float3 a, float3 b, float3 c, float scale)
{
    return floor(safe_divide(a, b)) * b;
}

float3 vector_math_floor(float3 a, float3 b, float3 c, float scale)
{
    return floor(a);
}

float3 vector_math_ceil(float3 a, float3 b, float3 c, float scale)
{
    return ceil(a);
}

float3 vector_math_modulo(
    float3 a, float3 b, float3 c, float scale)
{
    return compatible_mod(a, b);
}

float3 vector_math_wrap(float3 a, float3 b, float3 c, float scale)
{
    return wrap(a, b, c);
}

float3 vector_math_fraction(
    float3 a, float3 b, float3 c, float scale)
{
    return frac(a);
}

float3 vector_math_absolute(
    float3 a, float3 b, float3 c, float scale)
{
    return abs(a);
}

float3 vector_math_minimum(
    float3 a, float3 b, float3 c, float scale)
{
    return min(a, b);
}

float3 vector_math_maximum(
    float3 a, float3 b, float3 c, float scale)
{
    return max(a, b);
}

float3 vector_math_sine(float3 a, float3 b, float3 c, float scale)
{
    return sin(a);
}

float3 vector_math_cosine(
    float3 a, float3 b, float3 c, float scale)
{
    return cos(a);
}

float3 vector_math_tangent(
    float3 a, float3 b, float3 c, float scale)
{
    return tan(a);
}

float3 vector_math_refract(
    float3 a, float3 b, float3 c, float scale)
{
    return refract(a, normalize(b), scale);
}

float3 vector_math_faceforward(
    float3 a, float3 b, float3 c, float scale)
{
    return faceforward(a, b, c);
}

float3 vector_math_multiply_add(
    float3 a, float3 b, float3 c, float scale)
{
    return a * b + c;
}