using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CustomBlenderColor
{
    public float r;
    public float g;
    public float b;
    public float a;

    public CustomBlenderColor(Color col)
    {
        this.r = col.r;
        this.g = col.g;
        this.b = col.b;
        this.a = col.a;
    }

    public CustomBlenderColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public CustomBlenderColor(float r, float g, float b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = 1;
    }

    public CustomBlenderColor(float all)
    {
        this.r = all;
        this.g = all;
        this.b = all;
        this.a = 1;
    }

    public static implicit operator CustomBlenderColor(Vector4 v)
    {
        return new CustomBlenderColor(v.x, v.y, v.z, v.w);
    }

    public static implicit operator Vector4(CustomBlenderColor c)
    {
        return new Vector4(c.r, c.g, c.b, c.a);
    }

    public static implicit operator Color(CustomBlenderColor c)
    {
        return new Color(c.r, c.g, c.b, c.a);
    }

    public static CustomBlenderColor operator +(CustomBlenderColor a, CustomBlenderColor b) { return new CustomBlenderColor(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a); }
    public static CustomBlenderColor operator -(CustomBlenderColor a, CustomBlenderColor b) { return new CustomBlenderColor(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a); }
    public static CustomBlenderColor operator *(CustomBlenderColor a, CustomBlenderColor b) { return new CustomBlenderColor(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a); }
    public static CustomBlenderColor operator *(CustomBlenderColor a, float b) { return new CustomBlenderColor(a.r * b, a.g * b, a.b * b, a.a * b); }
    public static CustomBlenderColor operator *(float b, CustomBlenderColor a) { return new CustomBlenderColor(a.r * b, a.g * b, a.b * b, a.a * b); }
    public static CustomBlenderColor operator /(CustomBlenderColor a, float b) { return new CustomBlenderColor(a.r / b, a.g / b, a.b / b, a.a / b); }
    /*public static bool operator ==(CustomBlenderColor lc, CustomBlenderColor rc) { return (Vector4)lc == (Vector4)rc;}
    public static bool operator !=(CustomBlenderColor lc, CustomBlenderColor rc) { return !(lc == rc);}*/

    public static CustomBlenderColor Lerp(CustomBlenderColor a, CustomBlenderColor b, float t)
    {
        t = Mathf.Clamp01(t);
        return new CustomBlenderColor(
            a.r + (b.r - a.r) * t,
            a.g + (b.g - a.g) * t,
            a.b + (b.b - a.b) * t,
            a.a + (b.a - a.a) * t
        );
    }

    public static CustomBlenderColor red { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(1, 0, 0, 1); } }
    public static CustomBlenderColor yellow { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(1, 0.92f, 0.016f, 1); } }
    public static CustomBlenderColor clear { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(0, 0, 0, 0); } }
    public static CustomBlenderColor gray { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(0.5f, 0.5f, 0.5f, 1); } }
    public static CustomBlenderColor magenta { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(1, 0, 1, 1); } }
    public static CustomBlenderColor cyan { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(0, 1, 1, 1); } }
    public static CustomBlenderColor black { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(0, 0, 0, 1); } }
    public static CustomBlenderColor white { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(1, 1, 1, 1); } }
    public static CustomBlenderColor blue { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(0, 0, 0, 1); } }
    public static CustomBlenderColor green { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new CustomBlenderColor(0, 1, 0, 1); } }

    public CustomBlenderColor linear
    {
        get
        {
            return new CustomBlenderColor(Mathf.GammaToLinearSpace(r), Mathf.GammaToLinearSpace(g), Mathf.GammaToLinearSpace(b), a);
        }
    }

    public CustomBlenderColor gamma
    {
        get
        {
            return new CustomBlenderColor(Mathf.LinearToGammaSpace(r), Mathf.LinearToGammaSpace(g), Mathf.LinearToGammaSpace(b), a);
        }
    }

    public static void RGBToHSV(CustomBlenderColor rgbColor, out float H, out float S, out float V)
    {
        // when blue is highest valued
        if ((rgbColor.b > rgbColor.g) && (rgbColor.b > rgbColor.r))
            RGBToHSVConverter((float)4, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
        //when green is highest valued
        else if (rgbColor.g > rgbColor.r)
            RGBToHSVConverter((float)2, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
        //when red is highest valued
        else
            RGBToHSVConverter((float)0, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
    }

    static void RGBToHSVConverter(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
    {
        V = dominantcolor;
        if (V != 0)
        {
            float small = 0;
            if (colorone > colortwo) 
                small = colortwo;
            else 
                small = colorone;

            float diff = V - small;

            if (diff != 0)
            {
                S = diff / V;
                H = offset + ((colorone - colortwo) / diff);
            }
            else
            {
                S = 0;
                H = offset + (colorone - colortwo);
            }

            H /= 6;

            if (H < 0)
            {
                H += 1.0f;
            }
        }
        else
        {
            S = 0;
            H = 0;
        }
    }

    public static CustomBlenderColor HSVToRGB(float H, float S, float V)
    {
        return HSVToRGB(H, S, V, true);
    }

    public static CustomBlenderColor HSVToRGB(float H, float S, float V, bool hdr)
    {
        CustomBlenderColor retval = white;
        if (S == 0)
        {
            retval = new CustomBlenderColor(V, V, V);
        }
        else if (V == 0)
        {
            retval = black;
        }
        else
        {
            retval = black;

            //crazy hsv conversion
            H *= 6.0f;

            int temp = (int)Mathf.Floor(H);
            float t = H - temp;
            float a = (V) * (1 - S);
            float b = V * (1 - S * t);
            float c = V * (1 - S * (1 - t));

            if (temp == 0)
                retval = new CustomBlenderColor(V, c, a);
            else if(temp == 1)
                retval = new CustomBlenderColor(b, V, a);
            else if (temp == 2)
                retval = new CustomBlenderColor(a, V, c);
            else if (temp == 3)
                retval = new CustomBlenderColor(a, b, V);
            else if (temp == 4)
                retval = new CustomBlenderColor(c, a, V);
            else if (temp == 5)
                retval = new CustomBlenderColor(V, a, b);
            else if (temp == 6)
                retval = new CustomBlenderColor(V, c, a);
            else
                retval = new CustomBlenderColor(V, a, b);

            if (!hdr)
            {
                retval.r = Mathf.Clamp(retval.r, 0.0f, 1.0f);
                retval.g = Mathf.Clamp(retval.g, 0.0f, 1.0f);
                retval.b = Mathf.Clamp(retval.b, 0.0f, 1.0f);
            }
        }
        return retval;
    }

}
