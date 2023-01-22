/*
 * Original code is under the MIT License, Copyright (c) 2013 Inigo Quilez.
 *
 * Smooth Voronoi:
 *
 * - https://wiki.blender.org/wiki/User:OmarSquircleArt/GSoC2019/Documentation/Smooth_Voronoi
 *
 * Distance To Edge based on:
 *
 * - https://www.iquilezles.org/www/articles/voronoilines/voronoilines.htm
 * - https://www.shadertoy.com/view/ldl3W8
 *
 * With optimization to change -2..2 scan window to -1..1 for better performance,
 * as explained in https://www.shadertoy.com/view/llG3zy.
 *
 */

 /* **** 1D Voronoi **** */

#ifndef __HashUtil_
#include <HashUtil.hlsl>
#endif

#ifndef __MathUtil_
#include <MathUtil.hlsl>
#endif

float voronoi_distance(float a, float b, float metric, float exponent)
{
    return distance(a, b);
}

void node_tex_voronoi_f1_1d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float scaledCoord = w * scale;
    float cellPosition = floor(scaledCoord);
    float localPosition = scaledCoord - cellPosition;

    float minDistance = 8.0;
    float targetOffset, targetPosition;
    for (int i = -1; i <= 1; i++) {
        float cellOffset = float(i);
        float pointPosition = cellOffset + hash_float_to_float(cellPosition + cellOffset) * randomness;
        float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
        if (distanceToPoint < minDistance) {
            targetOffset = cellOffset;
            minDistance = distanceToPoint;
            targetPosition = pointPosition;
        }
    }
    outDistance = minDistance;
    outColor.xyz = hash_float_to_float3(cellPosition + targetOffset);
    outW = safe_divide(targetPosition + cellPosition, scale);
}

void node_tex_voronoi_smooth_f1_1d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    randomness = clamp(randomness, 0.0, 1.0);
    smoothness = clamp(smoothness / 2.0, 0, 0.5);

    float scaledCoord = w * scale;
    float cellPosition = floor(scaledCoord);
    float localPosition = scaledCoord - cellPosition;

    float smoothDistance = 8.0;
    float smoothPosition = 0.0;
    float3 smoothColor = float3(0.0, 0.0, 0.0);
    for (int i = -2; i <= 2; i++) {
        float cellOffset = float(i);
        float pointPosition = cellOffset + hash_float_to_float(cellPosition + cellOffset) * randomness;
        float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
        float h = smoothstep(0.0, 1.0, 0.5 + 0.5 * (smoothDistance - distanceToPoint) / smoothness);
        float correctionFactor = smoothness * h * (1.0 - h);
        smoothDistance = lerp(smoothDistance, distanceToPoint, h) - correctionFactor;
        correctionFactor /= 1.0 + 3.0 * smoothness;
        float3 cellColor = hash_float_to_float3(cellPosition + cellOffset);
        smoothColor = lerp(smoothColor, cellColor, h) - correctionFactor;
        smoothPosition = lerp(smoothPosition, pointPosition, h) - correctionFactor;
    }
    outDistance = smoothDistance;
    outColor.xyz = smoothColor;
    outW = safe_divide(cellPosition + smoothPosition, scale);
}

void node_tex_voronoi_f2_1d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float scaledCoord = w * scale;
    float cellPosition = floor(scaledCoord);
    float localPosition = scaledCoord - cellPosition;

    float distanceF1 = 8.0;
    float distanceF2 = 8.0;
    float offsetF1 = 0.0;
    float positionF1 = 0.0;
    float offsetF2, positionF2;
    for (int i = -1; i <= 1; i++) {
        float cellOffset = float(i);
        float pointPosition = cellOffset + hash_float_to_float(cellPosition + cellOffset) * randomness;
        float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
        if (distanceToPoint < distanceF1) {
            distanceF2 = distanceF1;
            distanceF1 = distanceToPoint;
            offsetF2 = offsetF1;
            offsetF1 = cellOffset;
            positionF2 = positionF1;
            positionF1 = pointPosition;
        }
        else if (distanceToPoint < distanceF2) {
            distanceF2 = distanceToPoint;
            offsetF2 = cellOffset;
            positionF2 = pointPosition;
        }
    }
    outDistance = distanceF2;
    outColor.xyz = hash_float_to_float3(cellPosition + offsetF2);
    outW = safe_divide(positionF2 + cellPosition, scale);
}

void node_tex_voronoi_distance_to_edge_1d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float scaledCoord = w * scale;
    float cellPosition = floor(scaledCoord);
    float localPosition = scaledCoord - cellPosition;

    float midPointPosition = hash_float_to_float(cellPosition) * randomness;
    float leftPointPosition = -1.0 + hash_float_to_float(cellPosition - 1.0) * randomness;
    float rightPointPosition = 1.0 + hash_float_to_float(cellPosition + 1.0) * randomness;
    float distanceToMidLeft = distance((midPointPosition + leftPointPosition) / 2.0, localPosition);
    float distanceToMidRight = distance((midPointPosition + rightPointPosition) / 2.0,
        localPosition);

    outDistance = min(distanceToMidLeft, distanceToMidRight);
}

void node_tex_voronoi_n_sphere_radius_1d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float scaledCoord = w * scale;
    float cellPosition = floor(scaledCoord);
    float localPosition = scaledCoord - cellPosition;

    float closestPoint;
    float closestPointOffset;
    float minDistance = 8.0;
    for (int i = -1; i <= 1; i++) {
        float cellOffset = float(i);
        float pointPosition = cellOffset + hash_float_to_float(cellPosition + cellOffset) * randomness;
        float distanceToPoint = distance(pointPosition, localPosition);
        if (distanceToPoint < minDistance) {
            minDistance = distanceToPoint;
            closestPoint = pointPosition;
            closestPointOffset = cellOffset;
        }
    }

    minDistance = 8.0;
    float closestPointToClosestPoint;
    for (int i = -1; i <= 1; i++) {
        if (i == 0) {
            continue;
        }
        float cellOffset = float(i) + closestPointOffset;
        float pointPosition = cellOffset + hash_float_to_float(cellPosition + cellOffset) * randomness;
        float distanceToPoint = distance(closestPoint, pointPosition);
        if (distanceToPoint < minDistance) {
            minDistance = distanceToPoint;
            closestPointToClosestPoint = pointPosition;
        }
    }
    outRadius = distance(closestPointToClosestPoint, closestPoint) / 2.0;
}

/* **** 2D Voronoi **** */

float voronoi_distance(float2 a, float2 b, float metric, float exponent)
{
    if (metric == 0.0)  // SHD_VORONOI_EUCLIDEAN
    {
        return distance(a, b);
    }
    else if (metric == 1.0)  // SHD_VORONOI_MANHATTAN
    {
        return abs(a.x - b.x) + abs(a.y - b.y);
    }
    else if (metric == 2.0)  // SHD_VORONOI_CHEBYCHEV
    {
        return max(abs(a.x - b.x), abs(a.y - b.y));
    }
    else if (metric == 3.0)  // SHD_VORONOI_MINKOWSKI
    {
        return pow(pow(abs(a.x - b.x), exponent) + pow(abs(a.y - b.y), exponent), 1.0 / exponent);
    }
    else {
        return 0.0;
    }
}

void node_tex_voronoi_f1_2d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float2 scaledCoord = coord.xy * scale;
    float2 cellPosition = floor(scaledCoord);
    float2 localPosition = scaledCoord - cellPosition;

    float minDistance = 8.0;
    float2 targetOffset, targetPosition;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            float2 cellOffset = float2(i, j);
            float2 pointPosition = cellOffset + hash_float2_to_float2(cellPosition + cellOffset) * randomness;
            float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
            if (distanceToPoint < minDistance) {
                targetOffset = cellOffset;
                minDistance = distanceToPoint;
                targetPosition = pointPosition;
            }
        }
    }
    outDistance = minDistance;
    outColor.xyz = hash_float2_to_float3(cellPosition + targetOffset);
    outPosition = float3(safe_divide(targetPosition + cellPosition, scale), 0.0);
}

void node_tex_voronoi_smooth_f1_2d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);
    smoothness = clamp(smoothness / 2.0, 0, 0.5);

    float2 scaledCoord = coord.xy * scale;
    float2 cellPosition = floor(scaledCoord);
    float2 localPosition = scaledCoord - cellPosition;

    float smoothDistance = 8.0;
    float3 smoothColor = float3(0.0, 0.0, 0.0);
    float2 smoothPosition = float2(0.0, 0.0);
    for (int j = -2; j <= 2; j++) {
        for (int i = -2; i <= 2; i++) {
            float2 cellOffset = float2(i, j);
            float2 pointPosition = cellOffset + hash_float2_to_float2(cellPosition + cellOffset) * randomness;
            float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
            float h = smoothstep(0.0, 1.0, 0.5 + 0.5 * (smoothDistance - distanceToPoint) / smoothness);
            float correctionFactor = smoothness * h * (1.0 - h);
            smoothDistance = lerp(smoothDistance, distanceToPoint, h) - correctionFactor;
            correctionFactor /= 1.0 + 3.0 * smoothness;
            float3 cellColor = hash_float2_to_float3(cellPosition + cellOffset);
            smoothColor = lerp(smoothColor, cellColor, h) - correctionFactor;
            smoothPosition = lerp(smoothPosition, pointPosition, h) - correctionFactor;
        }
    }
    outDistance = smoothDistance;
    outColor.xyz = smoothColor;
    outPosition = float3(safe_divide(cellPosition + smoothPosition, scale), 0.0);
}

void node_tex_voronoi_f2_2d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float2 scaledCoord = coord.xy * scale;
    float2 cellPosition = floor(scaledCoord);
    float2 localPosition = scaledCoord - cellPosition;

    float distanceF1 = 8.0;
    float distanceF2 = 8.0;
    float2 offsetF1 = float2(0.0, 0.0);
    float2 positionF1 = float2(0.0, 0.0);
    float2 offsetF2, positionF2;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            float2 cellOffset = float2(i, j);
            float2 pointPosition = cellOffset + hash_float2_to_float2(cellPosition + cellOffset) * randomness;
            float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
            if (distanceToPoint < distanceF1) {
                distanceF2 = distanceF1;
                distanceF1 = distanceToPoint;
                offsetF2 = offsetF1;
                offsetF1 = cellOffset;
                positionF2 = positionF1;
                positionF1 = pointPosition;
            }
            else if (distanceToPoint < distanceF2) {
                distanceF2 = distanceToPoint;
                offsetF2 = cellOffset;
                positionF2 = pointPosition;
            }
        }
    }
    outDistance = distanceF2;
    outColor.xyz = hash_float2_to_float3(cellPosition + offsetF2);
    outPosition = float3(safe_divide(positionF2 + cellPosition, scale), 0.0);
}

void node_tex_voronoi_distance_to_edge_2d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float2 scaledCoord = coord.xy * scale;
    float2 cellPosition = floor(scaledCoord);
    float2 localPosition = scaledCoord - cellPosition;

    float2 vectorToClosest;
    float minDistance = 8.0;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            float2 cellOffset = float2(i, j);
            float2 vectorToPoint = cellOffset + hash_float2_to_float2(cellPosition + cellOffset) * randomness -
                localPosition;
            float distanceToPoint = dot(vectorToPoint, vectorToPoint);
            if (distanceToPoint < minDistance) {
                minDistance = distanceToPoint;
                vectorToClosest = vectorToPoint;
            }
        }
    }

    minDistance = 8.0;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            float2 cellOffset = float2(i, j);
            float2 vectorToPoint = cellOffset + hash_float2_to_float2(cellPosition + cellOffset) * randomness -
                localPosition;
            float2 perpendicularToEdge = vectorToPoint - vectorToClosest;
            if (dot(perpendicularToEdge, perpendicularToEdge) > 0.0001) {
                float distanceToEdge = dot((vectorToClosest + vectorToPoint) / 2.0,
                    normalize(perpendicularToEdge));
                minDistance = min(minDistance, distanceToEdge);
            }
        }
    }
    outDistance = minDistance;
}

void node_tex_voronoi_n_sphere_radius_2d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float2 scaledCoord = coord.xy * scale;
    float2 cellPosition = floor(scaledCoord);
    float2 localPosition = scaledCoord - cellPosition;

    float2 closestPoint;
    float2 closestPointOffset;
    float minDistance = 8.0;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            float2 cellOffset = float2(i, j);
            float2 pointPosition = cellOffset + hash_float2_to_float2(cellPosition + cellOffset) * randomness;
            float distanceToPoint = distance(pointPosition, localPosition);
            if (distanceToPoint < minDistance) {
                minDistance = distanceToPoint;
                closestPoint = pointPosition;
                closestPointOffset = cellOffset;
            }
        }
    }

    minDistance = 8.0;
    float2 closestPointToClosestPoint;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            if (i == 0 && j == 0) {
                continue;
            }
            float2 cellOffset = float2(i, j) + closestPointOffset;
            float2 pointPosition = cellOffset + hash_float2_to_float2(cellPosition + cellOffset) * randomness;
            float distanceToPoint = distance(closestPoint, pointPosition);
            if (distanceToPoint < minDistance) {
                minDistance = distanceToPoint;
                closestPointToClosestPoint = pointPosition;
            }
        }
    }
    outRadius = distance(closestPointToClosestPoint, closestPoint) / 2.0;
}

/* **** 3D Voronoi **** */

float voronoi_distance(float3 a, float3 b, float metric, float exponent)
{
    if (metric == 0.0)  // SHD_VORONOI_EUCLIDEAN
    {
        return distance(a, b);
    }
    else if (metric == 1.0)  // SHD_VORONOI_MANHATTAN
    {
        return abs(a.x - b.x) + abs(a.y - b.y) + abs(a.z - b.z);
    }
    else if (metric == 2.0)  // SHD_VORONOI_CHEBYCHEV
    {
        return max(abs(a.x - b.x), max(abs(a.y - b.y), abs(a.z - b.z)));
    }
    else if (metric == 3.0)  // SHD_VORONOI_MINKOWSKI
    {
        return pow(pow(abs(a.x - b.x), exponent) + pow(abs(a.y - b.y), exponent) +
            pow(abs(a.z - b.z), exponent),
            1.0 / exponent);
    }
    else {
        return 0.0;
    }
}

void node_tex_voronoi_f1_3d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float3 scaledCoord = coord * scale;
    float3 cellPosition = floor(scaledCoord);
    float3 localPosition = scaledCoord - cellPosition;

    float minDistance = 8.0;
    float3 targetOffset, targetPosition;
    for (int k = -1; k <= 1; k++) {
        for (int j = -1; j <= 1; j++) {
            for (int i = -1; i <= 1; i++) {
                float3 cellOffset = float3(i, j, k);
                float3 pointPosition = cellOffset +
                    hash_float3_to_float3(cellPosition + cellOffset) * randomness;
                float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
                if (distanceToPoint < minDistance) {
                    targetOffset = cellOffset;
                    minDistance = distanceToPoint;
                    targetPosition = pointPosition;
                }
            }
        }
    }
    outDistance = minDistance;
    outColor.xyz = hash_float3_to_float3(cellPosition + targetOffset);
    outPosition = safe_divide(targetPosition + cellPosition, scale);
}

void node_tex_voronoi_smooth_f1_3d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);
    smoothness = clamp(smoothness / 2.0, 0, 0.5);

    float3 scaledCoord = coord * scale;
    float3 cellPosition = floor(scaledCoord);
    float3 localPosition = scaledCoord - cellPosition;

    float smoothDistance = 8.0;
    float3 smoothColor = float3(0.0, 0.0, 0.0);
    float3 smoothPosition = float3(0.0, 0.0, 0.0);
    for (int k = -2; k <= 2; k++) {
        for (int j = -2; j <= 2; j++) {
            for (int i = -2; i <= 2; i++) {
                float3 cellOffset = float3(i, j, k);
                float3 pointPosition = cellOffset +
                    hash_float3_to_float3(cellPosition + cellOffset) * randomness;
                float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
                float h = smoothstep(
                    0.0, 1.0, 0.5 + 0.5 * (smoothDistance - distanceToPoint) / smoothness);
                float correctionFactor = smoothness * h * (1.0 - h);
                smoothDistance = lerp(smoothDistance, distanceToPoint, h) - correctionFactor;
                correctionFactor /= 1.0 + 3.0 * smoothness;
                float3 cellColor = hash_float3_to_float3(cellPosition + cellOffset);
                smoothColor = lerp(smoothColor, cellColor, h) - correctionFactor;
                smoothPosition = lerp(smoothPosition, pointPosition, h) - correctionFactor;
            }
        }
    }
    outDistance = smoothDistance;
    outColor.xyz = smoothColor;
    outPosition = safe_divide(cellPosition + smoothPosition, scale);
}

void node_tex_voronoi_f2_3d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float3 scaledCoord = coord * scale;
    float3 cellPosition = floor(scaledCoord);
    float3 localPosition = scaledCoord - cellPosition;

    float distanceF1 = 8.0;
    float distanceF2 = 8.0;
    float3 offsetF1 = float3(0.0, 0.0, 0.0);
    float3 positionF1 = float3(0.0, 0.0, 0.0);
    float3 offsetF2, positionF2;
    for (int k = -1; k <= 1; k++) {
        for (int j = -1; j <= 1; j++) {
            for (int i = -1; i <= 1; i++) {
                float3 cellOffset = float3(i, j, k);
                float3 pointPosition = cellOffset +
                    hash_float3_to_float3(cellPosition + cellOffset) * randomness;
                float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
                if (distanceToPoint < distanceF1) {
                    distanceF2 = distanceF1;
                    distanceF1 = distanceToPoint;
                    offsetF2 = offsetF1;
                    offsetF1 = cellOffset;
                    positionF2 = positionF1;
                    positionF1 = pointPosition;
                }
                else if (distanceToPoint < distanceF2) {
                    distanceF2 = distanceToPoint;
                    offsetF2 = cellOffset;
                    positionF2 = pointPosition;
                }
            }
        }
    }
    outDistance = distanceF2;
    outColor.xyz = hash_float3_to_float3(cellPosition + offsetF2);
    outPosition = safe_divide(positionF2 + cellPosition, scale);
}

void node_tex_voronoi_distance_to_edge_3d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float3 scaledCoord = coord * scale;
    float3 cellPosition = floor(scaledCoord);
    float3 localPosition = scaledCoord - cellPosition;

    float3 vectorToClosest;
    float minDistance = 8.0;
    for (int k = -1; k <= 1; k++) {
        for (int j = -1; j <= 1; j++) {
            for (int i = -1; i <= 1; i++) {
                float3 cellOffset = float3(i, j, k);
                float3 vectorToPoint = cellOffset +
                    hash_float3_to_float3(cellPosition + cellOffset) * randomness -
                    localPosition;
                float distanceToPoint = dot(vectorToPoint, vectorToPoint);
                if (distanceToPoint < minDistance) {
                    minDistance = distanceToPoint;
                    vectorToClosest = vectorToPoint;
                }
            }
        }
    }

    minDistance = 8.0;
    for (int k = -1; k <= 1; k++) {
        for (int j = -1; j <= 1; j++) {
            for (int i = -1; i <= 1; i++) {
                float3 cellOffset = float3(i, j, k);
                float3 vectorToPoint = cellOffset +
                    hash_float3_to_float3(cellPosition + cellOffset) * randomness -
                    localPosition;
                float3 perpendicularToEdge = vectorToPoint - vectorToClosest;
                if (dot(perpendicularToEdge, perpendicularToEdge) > 0.0001) {
                    float distanceToEdge = dot((vectorToClosest + vectorToPoint) / 2.0,
                        normalize(perpendicularToEdge));
                    minDistance = min(minDistance, distanceToEdge);
                }
            }
        }
    }
    outDistance = minDistance;
}

void node_tex_voronoi_n_sphere_radius_3d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float3 scaledCoord = coord * scale;
    float3 cellPosition = floor(scaledCoord);
    float3 localPosition = scaledCoord - cellPosition;

    float3 closestPoint;
    float3 closestPointOffset;
    float minDistance = 8.0;
    for (int k = -1; k <= 1; k++) {
        for (int j = -1; j <= 1; j++) {
            for (int i = -1; i <= 1; i++) {
                float3 cellOffset = float3(i, j, k);
                float3 pointPosition = cellOffset +
                    hash_float3_to_float3(cellPosition + cellOffset) * randomness;
                float distanceToPoint = distance(pointPosition, localPosition);
                if (distanceToPoint < minDistance) {
                    minDistance = distanceToPoint;
                    closestPoint = pointPosition;
                    closestPointOffset = cellOffset;
                }
            }
        }
    }

    minDistance = 8.0;
    float3 closestPointToClosestPoint;
    for (int k = -1; k <= 1; k++) {
        for (int j = -1; j <= 1; j++) {
            for (int i = -1; i <= 1; i++) {
                if (i == 0 && j == 0 && k == 0) {
                    continue;
                }
                float3 cellOffset = float3(i, j, k) + closestPointOffset;
                float3 pointPosition = cellOffset +
                    hash_float3_to_float3(cellPosition + cellOffset) * randomness;
                float distanceToPoint = distance(closestPoint, pointPosition);
                if (distanceToPoint < minDistance) {
                    minDistance = distanceToPoint;
                    closestPointToClosestPoint = pointPosition;
                }
            }
        }
    }
    outRadius = distance(closestPointToClosestPoint, closestPoint) / 2.0;
}

/* **** 4D Voronoi **** */

float voronoi_distance(float4 a, float4 b, float metric, float exponent)
{
    if (metric == 0.0)  // SHD_VORONOI_EUCLIDEAN
    {
        return distance(a, b);
    }
    else if (metric == 1.0)  // SHD_VORONOI_MANHATTAN
    {
        return abs(a.x - b.x) + abs(a.y - b.y) + abs(a.z - b.z) + abs(a.w - b.w);
    }
    else if (metric == 2.0)  // SHD_VORONOI_CHEBYCHEV
    {
        return max(abs(a.x - b.x), max(abs(a.y - b.y), max(abs(a.z - b.z), abs(a.w - b.w))));
    }
    else if (metric == 3.0)  // SHD_VORONOI_MINKOWSKI
    {
        return pow(pow(abs(a.x - b.x), exponent) + pow(abs(a.y - b.y), exponent) +
            pow(abs(a.z - b.z), exponent) + pow(abs(a.w - b.w), exponent),
            1.0 / exponent);
    }
    else {
        return 0.0;
    }
}

void node_tex_voronoi_f1_4d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float4 scaledCoord = float4(coord, w) * scale;
    float4 cellPosition = floor(scaledCoord);
    float4 localPosition = scaledCoord - cellPosition;

    float minDistance = 8.0;
    float4 targetOffset, targetPosition;
    for (int u = -1; u <= 1; u++) {
        for (int k = -1; k <= 1; k++) {
            for (int j = -1; j <= 1; j++) {
                for (int i = -1; i <= 1; i++) {
                    float4 cellOffset = float4(i, j, k, u);
                    float4 pointPosition = cellOffset +
                        hash_float4_to_float4(cellPosition + cellOffset) * randomness;
                    float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
                    if (distanceToPoint < minDistance) {
                        targetOffset = cellOffset;
                        minDistance = distanceToPoint;
                        targetPosition = pointPosition;
                    }
                }
            }
        }
    }
    outDistance = minDistance;
    outColor.xyz = hash_float4_to_float3(cellPosition + targetOffset);
    float4 p = safe_divide(targetPosition + cellPosition, scale);
    outPosition = p.xyz;
    outW = p.w;
}

void node_tex_voronoi_smooth_f1_4d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);
    smoothness = clamp(smoothness / 2.0, 0, 0.5);

    float4 scaledCoord = float4(coord, w) * scale;
    float4 cellPosition = floor(scaledCoord);
    float4 localPosition = scaledCoord - cellPosition;

    float smoothDistance = 8.0;
    float3 smoothColor = float3(0.0, 0.0, 0.0);
    float4 smoothPosition = float4(0.0, 0.0, 0.0, 0.0);
    for (int u = -2; u <= 2; u++) {
        for (int k = -2; k <= 2; k++) {
            for (int j = -2; j <= 2; j++) {
                for (int i = -2; i <= 2; i++) {
                    float4 cellOffset = float4(i, j, k, u);
                    float4 pointPosition = cellOffset +
                        hash_float4_to_float4(cellPosition + cellOffset) * randomness;
                    float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
                    float h = smoothstep(
                        0.0, 1.0, 0.5 + 0.5 * (smoothDistance - distanceToPoint) / smoothness);
                    float correctionFactor = smoothness * h * (1.0 - h);
                    smoothDistance = lerp(smoothDistance, distanceToPoint, h) - correctionFactor;
                    correctionFactor /= 1.0 + 3.0 * smoothness;
                    float3 cellColor = hash_float4_to_float3(cellPosition + cellOffset);
                    smoothColor = lerp(smoothColor, cellColor, h) - correctionFactor;
                    smoothPosition = lerp(smoothPosition, pointPosition, h) - correctionFactor;
                }
            }
        }
    }
    outDistance = smoothDistance;
    outColor.xyz = smoothColor;
    float4 p = safe_divide(cellPosition + smoothPosition, scale);
    outPosition = p.xyz;
    outW = p.w;
}

void node_tex_voronoi_f2_4d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float4 scaledCoord = float4(coord, w) * scale;
    float4 cellPosition = floor(scaledCoord);
    float4 localPosition = scaledCoord - cellPosition;

    float distanceF1 = 8.0;
    float distanceF2 = 8.0;
    float4 offsetF1 = float4(0.0, 0.0, 0.0, 0.0);
    float4 positionF1 = float4(0.0, 0.0, 0.0, 0.0);
    float4 offsetF2, positionF2;
    for (int u = -1; u <= 1; u++) {
        for (int k = -1; k <= 1; k++) {
            for (int j = -1; j <= 1; j++) {
                for (int i = -1; i <= 1; i++) {
                    float4 cellOffset = float4(i, j, k, u);
                    float4 pointPosition = cellOffset +
                        hash_float4_to_float4(cellPosition + cellOffset) * randomness;
                    float distanceToPoint = voronoi_distance(pointPosition, localPosition, metric, exponent);
                    if (distanceToPoint < distanceF1) {
                        distanceF2 = distanceF1;
                        distanceF1 = distanceToPoint;
                        offsetF2 = offsetF1;
                        offsetF1 = cellOffset;
                        positionF2 = positionF1;
                        positionF1 = pointPosition;
                    }
                    else if (distanceToPoint < distanceF2) {
                        distanceF2 = distanceToPoint;
                        offsetF2 = cellOffset;
                        positionF2 = pointPosition;
                    }
                }
            }
        }
    }
    outDistance = distanceF2;
    outColor.xyz = hash_float4_to_float3(cellPosition + offsetF2);
    float4 p = safe_divide(positionF2 + cellPosition, scale);
    outPosition = p.xyz;
    outW = p.w;
}

void node_tex_voronoi_distance_to_edge_4d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float4 scaledCoord = float4(coord, w) * scale;
    float4 cellPosition = floor(scaledCoord);
    float4 localPosition = scaledCoord - cellPosition;

    float4 vectorToClosest;
    float minDistance = 8.0;
    for (int u = -1; u <= 1; u++) {
        for (int k = -1; k <= 1; k++) {
            for (int j = -1; j <= 1; j++) {
                for (int i = -1; i <= 1; i++) {
                    float4 cellOffset = float4(i, j, k, u);
                    float4 vectorToPoint = cellOffset +
                        hash_float4_to_float4(cellPosition + cellOffset) * randomness -
                        localPosition;
                    float distanceToPoint = dot(vectorToPoint, vectorToPoint);
                    if (distanceToPoint < minDistance) {
                        minDistance = distanceToPoint;
                        vectorToClosest = vectorToPoint;
                    }
                }
            }
        }
    }

    minDistance = 8.0;
    for (int u = -1; u <= 1; u++) {
        for (int k = -1; k <= 1; k++) {
            for (int j = -1; j <= 1; j++) {
                for (int i = -1; i <= 1; i++) {
                    float4 cellOffset = float4(i, j, k, u);
                    float4 vectorToPoint = cellOffset +
                        hash_float4_to_float4(cellPosition + cellOffset) * randomness -
                        localPosition;
                    float4 perpendicularToEdge = vectorToPoint - vectorToClosest;
                    if (dot(perpendicularToEdge, perpendicularToEdge) > 0.0001) {
                        float distanceToEdge = dot((vectorToClosest + vectorToPoint) / 2.0,
                            normalize(perpendicularToEdge));
                        minDistance = min(minDistance, distanceToEdge);
                    }
                }
            }
        }
    }
    outDistance = minDistance;
}

void node_tex_voronoi_n_sphere_radius_4d(float3 coord,
    float w,
    float scale,
    float smoothness,
    float exponent,
    float randomness,
    float metric,
    out float outDistance,
    out float4 outColor,
    out float3 outPosition,
    out float outW,
    out float outRadius)
{
    outPosition = float3(0.0, 0.0, 0.0);
    outColor = float4(0.0, 0.0, 0.0, 0.0);
    outRadius = 0;
    outW = 0;
    outDistance = 0;
    randomness = clamp(randomness, 0.0, 1.0);

    float4 scaledCoord = float4(coord, w) * scale;
    float4 cellPosition = floor(scaledCoord);
    float4 localPosition = scaledCoord - cellPosition;

    float4 closestPoint;
    float4 closestPointOffset;
    float minDistance = 8.0;
    for (int u = -1; u <= 1; u++) {
        for (int k = -1; k <= 1; k++) {
            for (int j = -1; j <= 1; j++) {
                for (int i = -1; i <= 1; i++) {
                    float4 cellOffset = float4(i, j, k, u);
                    float4 pointPosition = cellOffset +
                        hash_float4_to_float4(cellPosition + cellOffset) * randomness;
                    float distanceToPoint = distance(pointPosition, localPosition);
                    if (distanceToPoint < minDistance) {
                        minDistance = distanceToPoint;
                        closestPoint = pointPosition;
                        closestPointOffset = cellOffset;
                    }
                }
            }
        }
    }

    minDistance = 8.0;
    float4 closestPointToClosestPoint;
    for (int u = -1; u <= 1; u++) {
        for (int k = -1; k <= 1; k++) {
            for (int j = -1; j <= 1; j++) {
                for (int i = -1; i <= 1; i++) {
                    if (i == 0 && j == 0 && k == 0 && u == 0) {
                        continue;
                    }
                    float4 cellOffset = float4(i, j, k, u) + closestPointOffset;
                    float4 pointPosition = cellOffset +
                        hash_float4_to_float4(cellPosition + cellOffset) * randomness;
                    float distanceToPoint = distance(closestPoint, pointPosition);
                    if (distanceToPoint < minDistance) {
                        minDistance = distanceToPoint;
                        closestPointToClosestPoint = pointPosition;
                    }
                }
            }
        }
    }
    outRadius = distance(closestPointToClosestPoint, closestPoint) / 2.0;
}

void voronoi_tex_getValue(float3 coord, float w, float scale, float smoothness, float exponent, float randomness, float metric,
    float dimen, float feature, out float outDistance, out float4 outColor, out float3 outPosition, out float outW, out float outRadius)
{
    if (dimen == 0) 
    {
        if (feature == 0) 
        {
            node_tex_voronoi_f1_1d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 1)
        {
            node_tex_voronoi_f2_1d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 2)
        {
            node_tex_voronoi_smooth_f1_1d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 3)
        {
            node_tex_voronoi_distance_to_edge_1d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 4)
        {
            node_tex_voronoi_n_sphere_radius_1d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
    }
    if (dimen == 1)
    {
        if (feature == 0)
        {
            node_tex_voronoi_f1_2d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 1)
        {
            node_tex_voronoi_f2_2d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 2)
        {
            node_tex_voronoi_smooth_f1_2d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 3)
        {
            node_tex_voronoi_distance_to_edge_2d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 4)
        {
            node_tex_voronoi_n_sphere_radius_2d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
    }
    if (dimen == 2)
    {
        if (feature == 0)
        {
            node_tex_voronoi_f1_3d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 1)
        {
            node_tex_voronoi_f2_3d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 2)
        {
            node_tex_voronoi_smooth_f1_3d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 3)
        {
            node_tex_voronoi_distance_to_edge_3d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 4)
        {
            node_tex_voronoi_n_sphere_radius_3d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
    }
    if (dimen == 3)
    {
        if (feature == 0)
        {
            node_tex_voronoi_f1_4d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 1)
        {
            node_tex_voronoi_f2_4d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 2)
        {
            node_tex_voronoi_smooth_f1_4d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 3)
        {
            node_tex_voronoi_distance_to_edge_4d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
        if (feature == 4)
        {
            node_tex_voronoi_n_sphere_radius_4d(coord, w, scale, smoothness, exponent, randomness, metric,
                outDistance, outColor, outPosition, outW, outRadius);
        }
    }
}

float voronoi_tex_out_distance(float3 coord, float w, float scale, float smoothness, float exponent, float randomness, float metric,
    float dimen, float feature) 
{
    float outDistance;
    float4 outColor;
    float3 outPosition;
    float outW;
    float outRadius;
    voronoi_tex_getValue(coord, w, scale, smoothness, exponent, randomness, metric,
        dimen, feature, outDistance, outColor, outPosition, outW, outRadius);
    return outDistance;
}

float4 voronoi_tex_out_color(float3 coord, float w, float scale, float smoothness, float exponent, float randomness, float metric,
    float dimen, float feature)
{
    float outDistance;
    float4 outColor;
    float3 outPosition;
    float outW;
    float outRadius;
    voronoi_tex_getValue(coord, w, scale, smoothness, exponent, randomness, metric,
        dimen, feature, outDistance, outColor, outPosition, outW, outRadius);
    return outColor;
}

float3 voronoi_tex_out_position(float3 coord, float w, float scale, float smoothness, float exponent, float randomness, float metric,
    float dimen, float feature)
{
    float outDistance;
    float4 outColor;
    float3 outPosition;
    float outW;
    float outRadius;
    voronoi_tex_getValue(coord, w, scale, smoothness, exponent, randomness, metric,
        dimen, feature, outDistance, outColor, outPosition, outW, outRadius);
    return outPosition;
}

float voronoi_tex_out_w(float3 coord, float w, float scale, float smoothness, float exponent, float randomness, float metric,
    float dimen, float feature)
{
    float outDistance;
    float4 outColor;
    float3 outPosition;
    float outW;
    float outRadius;
    voronoi_tex_getValue(coord, w, scale, smoothness, exponent, randomness, metric,
        dimen, feature, outDistance, outColor, outPosition, outW, outRadius);
    return outW;
}

float voronoi_tex_out_radius(float3 coord, float w, float scale, float smoothness, float exponent, float randomness, float metric,
    float dimen, float feature)
{
    float outDistance;
    float4 outColor;
    float3 outPosition;
    float outW;
    float outRadius;
    voronoi_tex_getValue(coord, w, scale, smoothness, exponent, randomness, metric,
        dimen, feature, outDistance, outColor, outPosition, outW, outRadius);
    return outRadius;
}