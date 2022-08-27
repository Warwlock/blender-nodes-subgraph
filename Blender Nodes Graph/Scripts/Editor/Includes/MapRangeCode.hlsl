#ifndef __MathUtil_
#include <MathUtil.hlsl>
#endif

float smootherstep(float edge0, float edge1, float x)
{
    x = clamp(safe_divide((x - edge0), (edge1 - edge0)), 0.0, 1.0);
    return x * x * x * (x * (x * 6.0 - 15.0) + 10.0);
}

float map_range_linear(float value,
    float fromMin,
    float fromMax,
    float toMin,
    float toMax,
    float steps)
{
    if (fromMax != fromMin) {
        return toMin + ((value - fromMin) / (fromMax - fromMin)) * (toMax - toMin);
    }
    else {
        return 0.0;
    }
}

float map_range_stepped(float value,
    float fromMin,
    float fromMax,
    float toMin,
    float toMax,
    float steps)
{
    if (fromMax != fromMin) {
        float factor = (value - fromMin) / (fromMax - fromMin);
        factor = (steps > 0.0) ? floor(factor * (steps + 1.0)) / steps : 0.0;
        return toMin + factor * (toMax - toMin);
    }
    else {
        return 0.0;
    }
}

float map_range_smoothstep(float value,
    float fromMin,
    float fromMax,
    float toMin,
    float toMax,
    float steps)
{
    if (fromMax != fromMin) {
        float factor = (fromMin > fromMax) ? 1.0 - smoothstep(fromMax, fromMin, value) :
            smoothstep(fromMin, fromMax, value);
        return toMin + factor * (toMax - toMin);
    }
    else {
        return 0.0;
    }
}

float map_range_smootherstep(float value,
    float fromMin,
    float fromMax,
    float toMin,
    float toMax,
    float steps)
{
    if (fromMax != fromMin) {
        float factor = (fromMin > fromMax) ? 1.0 - smootherstep(fromMax, fromMin, value) :
            smootherstep(fromMin, fromMax, value);
        return toMin + factor * (toMax - toMin);
    }
    else {
        return 0.0;
    }
}