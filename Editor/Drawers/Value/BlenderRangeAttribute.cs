using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class BlenderRangeAttribute : PropertyAttribute
{
    public readonly float min;
    public readonly float max;
    public readonly bool useSlider;
    public readonly bool forcedRange;
    public readonly bool asInt;

    public BlenderRangeAttribute(float min, float max, bool useSlider = true, bool forcedRange = false, bool asInt = false)
    {
        this.min = min;
        this.max = max;
        this.useSlider = useSlider;
        this.forcedRange = forcedRange;
        this.asInt = asInt;
    }
}
