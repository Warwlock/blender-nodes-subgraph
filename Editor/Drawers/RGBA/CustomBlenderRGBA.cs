using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CustomBlenderRGBA
{
    [SerializeField]
    public CustomBlenderColor col = CustomBlenderColor.gray;

    public CustomBlenderRGBA()
    {
        col = CustomBlenderColor.gray;
    }

    public CustomBlenderRGBA(CustomBlenderColor Col)
    {
        col = Col;
    }

    public CustomBlenderRGBA(float r, float g, float b, float a)
    {
        col = new CustomBlenderColor(r, g, b, a);
    }

    public Color gamma
    {
        get
        {
            return col.gamma;
        }
    }

    public float r
    {
        get
        {
            return col.r;
        }
    }

    public float g
    {
        get
        {
            return col.g;
        }
    }

    public float b
    {
        get
        {
            return col.b;
        }
    }

    public float a
    {
        get
        {
            return col.a;
        }
    }
}
