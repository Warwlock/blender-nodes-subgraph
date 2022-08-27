#ifndef __MathUtil_
#include <MathUtil.hlsl>
#endif

//All Math Functions
float math_add(float a, float b, float c)
{
    return a + b;
}

float math_subtract(float a, float b, float c)
{
    return a - b;
}

float math_multiply(float a, float b, float c)
{
    return a * b;
}

float math_divide(float a, float b, float c)
{
    return safe_divide(a, b);
}

float math_power(float a, float b, float c)
{
    if (a >= 0.0) {
        return compatible_pow(a, b);
    }
    else {
        float fraction = mod(abs(b), 1.0);
        if (fraction > 0.999 || fraction < 0.001) {
            return compatible_pow(a, floor(b + 0.5));
        }
        else {
            return 0.0;
        }
    }
}

float math_logarithm(float a, float b, float c)
{
    return (a > 0.0 && b > 0.0) ? log2(a) / log2(b) : 0.0;
}

float math_sqrt(float a, float b, float c)
{
    return (a > 0.0) ? sqrt(a) : 0.0;
}

float math_inversesqrt(float a, float b, float c)
{
    return rsqrt(a);
}

float math_absolute(float a, float b, float c)
{
    return abs(a);
}

float math_radians(float a, float b, float c)
{
    return radians(a);
}

float math_degrees(float a, float b, float c)
{
    return degrees(a);
}

float math_minimum(float a, float b, float c)
{
    return min(a, b);
}

float math_maximum(float a, float b, float c)
{
    return max(a, b);
}

float math_less_than(float a, float b, float c)
{
    return (a < b) ? 1.0 : 0.0;
}

float math_greater_than(float a, float b, float c)
{
    return (a > b) ? 1.0 : 0.0;
}

float math_round(float a, float b, float c)
{
    return floor(a + 0.5);
}

float math_floor(float a, float b, float c)
{
    return floor(a);
}

float math_ceil(float a, float b, float c)
{
    return ceil(a);
}

float math_fraction(float a, float b, float c)
{
    return a - floor(a);
}

float math_modulo(float a, float b, float c)
{
    return compatible_mod(a, b);
}

float math_trunc(float a, float b, float c)
{
    return trunc(a);
}

float math_snap(float a, float b, float c)
{
    return floor(safe_divide(a, b)) * b;
}

float math_pingpong(float a, float b, float c)
{
    return (b != 0.0) ? abs(frac((a - b) / (b * 2.0)) * b * 2.0 - b) : 0.0;
}

/* Adapted from godotengine math_funcs.h. */
float math_wrap(float a, float b, float c)
{
    return wrap(a, b, c);
}

float math_sine(float a, float b, float c)
{
    return sin(a);
}

float math_cosine(float a, float b, float c)
{
    return cos(a);
}

float math_tangent(float a, float b, float c)
{
    return tan(a);
}

float math_sinh(float a, float b, float c)
{
    return sinh(a);
}

float math_cosh(float a, float b, float c)
{
    return cosh(a);
}

float math_tanh(float a, float b, float c)
{
    return tanh(a);
}

float math_arcsine(float a, float b, float c)
{
    return (a <= 1.0 && a >= -1.0) ? asin(a) : 0.0;
}

float math_arccosine(float a, float b, float c)
{
    return (a <= 1.0 && a >= -1.0) ? acos(a) : 0.0;
}

float math_arctangent(float a, float b, float c)
{
    return atan(a);
}

float math_arctan2(float a, float b, float c)
{
    return atan2(b, a);
}

float math_sign(float a, float b, float c)
{
    return sign(a);
}

float math_exponent(float a, float b, float c)
{
    return exp(a);
}

float math_compare(float a, float b, float c)
{
    return (abs(a - b) <= max(c, 1e-5)) ? 1.0 : 0.0;
}

float math_multiply_add(float a, float b, float c)
{
    return a * b + c;
}

/* See: https://www.iquilezles.org/www/articles/smin/smin.htm. */
float math_smoothmin(float a, float b, float c)
{
    if (c != 0.0) {
        float h = max(c - abs(a - b), 0.0) / c;
        return min(a, b) - h * h * h * c * (1.0 / 6.0);
    }
    else {
        return min(a, b);
    }
}

float math_smoothmax(float a, float b, float c)
{
    return -math_smoothmin(-a, -b, c);
}

float clamp_value(float value, float min, float max)
{
    return clamp(value, min, max);
}

float clamp_minmax(float value, float min_allowed, float max_allowed)
{
    return min(max(value, min_allowed), max_allowed);
}

float clamp_range(float value, float min, float max)
{
    return (max > min) ? clamp(value, min, max) : clamp(value, max, min);
}