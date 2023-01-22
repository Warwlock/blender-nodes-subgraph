using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomBlenderGradient
{
    public enum interpolationEnum { _Interpolation, Ease, Linear, Constant}
    public interpolationEnum Interpolation = interpolationEnum.Linear;

    public enum interpolationHSVEnum { _ColorInterpolation, Near, Far, Clockwise, CounterClockwise }
    public interpolationHSVEnum InterpolationHSV = interpolationHSVEnum.Near;

    public enum colorModeEnum { _ColorMode, RGB, HSV}
    public colorModeEnum ColorMode = colorModeEnum.RGB;

    [SerializeField]
    public List<ColorKey> keys = new List<ColorKey>();
    [SerializeField]
    public int selectedKeyIndex = 0;

    public CustomBlenderGradient()
    {
        AddKey(CustomBlenderColor.black, 0);
        AddKey(CustomBlenderColor.white, 1);
    }

    public CustomBlenderColor Evaluate(float time)
    {
        ColorKey keyLeft = keys[0];
        ColorKey keyRight = keys[keys.Count - 1];

        for(int i = 0; i < keys.Count; i++)
        {
            if(keys[i].Time < time)
            {
                keyLeft = keys[i];
            }
            if(keys[i].Time > time)
            {
                keyRight = keys[i];
                break;
            }
        }

        float blendTime = 0;

        if(ColorMode == colorModeEnum.HSV)
        {
            if(InterpolationHSV == interpolationHSVEnum.Near)
            {
                float h1, s1, v1, h2, s2, v2;
                CustomBlenderColor.RGBToHSV(keyLeft.Color, out h1, out s1, out v1);
                CustomBlenderColor.RGBToHSV(keyRight.Color, out h2, out s2, out v2);

                blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
                float h = 0;
                if (h1 > h2)
                {
                    float maxCCW = h1 - h2;
                    float maxCW = (h2 + 1) - h1;
                    h = Mathf.Lerp(h1, maxCCW > maxCW ? h2 + 1 : h2, blendTime);
                }
                else
                {
                    float maxCCW = h2 - h1;
                    float maxCW = (h1 + 1) - h2;
                    h = Mathf.Lerp(maxCCW > maxCW ? h1 + 1 : h1, h2, blendTime);
                }

                if (h > 1)
                {
                    h = h - 1;
                }

                return CustomBlenderColor.Lerp(CustomBlenderColor.HSVToRGB(h, s1, v1), CustomBlenderColor.HSVToRGB(h, s2, v2), blendTime);
            }
            if (InterpolationHSV == interpolationHSVEnum.Far)
            {
                float h1, s1, v1, h2, s2, v2;
                CustomBlenderColor.RGBToHSV(keyLeft.Color, out h1, out s1, out v1);
                CustomBlenderColor.RGBToHSV(keyRight.Color, out h2, out s2, out v2);

                blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
                float h = 0;
                if(h1 > h2)
                {
                    float maxCCW = h1 - h2;
                    float maxCW = (h2 + 1) - h1;
                    h = Mathf.Lerp(h1, maxCCW > maxCW ? h2 : (h2 + 1), blendTime);
                }
                else
                {
                    float maxCCW = h2 - h1;
                    float maxCW = (h1 + 1) - h2;
                    h = Mathf.Lerp(maxCCW > maxCW ? h1 : (h1 + 1), h2, blendTime);
                }

                if(h > 1)
                {
                    h = h - 1;
                }

                return CustomBlenderColor.Lerp(CustomBlenderColor.HSVToRGB(h, s1, v1), CustomBlenderColor.HSVToRGB(h, s2, v2), blendTime);
            }
            if (InterpolationHSV == interpolationHSVEnum.Clockwise)
            {
                float h1, s1, v1, h2, s2, v2;
                CustomBlenderColor.RGBToHSV(keyLeft.Color, out h1, out s1, out v1);
                CustomBlenderColor.RGBToHSV(keyRight.Color, out h2, out s2, out v2);

                blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
                float h = 0;
                if (h1 > h2)
                {
                    //maxCCW = h1 - h2;
                    //maxCW = (h2 + 1) - h1;
                    h = Mathf.Lerp(h1, (h2 + 1), blendTime);
                }
                else
                {
                    //maxCCW = h2 - h1;
                    //maxCW = (h1 + 1) - h2;
                    h = Mathf.Lerp(h1, h2, blendTime);
                }

                if (h > 1)
                {
                    h = h - 1;
                }

                return CustomBlenderColor.Lerp(CustomBlenderColor.HSVToRGB(h, s1, v1), CustomBlenderColor.HSVToRGB(h, s2, v2), blendTime);
            }
            if (InterpolationHSV == interpolationHSVEnum.CounterClockwise)
            {
                float h1, s1, v1, h2, s2, v2;
                CustomBlenderColor.RGBToHSV(keyLeft.Color, out h1, out s1, out v1);
                CustomBlenderColor.RGBToHSV(keyRight.Color, out h2, out s2, out v2);

                blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
                float h = 0;
                if (h1 > h2)
                {
                    //maxCCW = h1 - h2;
                    //maxCW = (h2 + 1) - h1;
                    h = Mathf.Lerp(h1, h2, blendTime);
                }
                else
                {
                    //maxCCW = h2 - h1;
                    //maxCW = (h1 + 1) - h2;
                    h = Mathf.Lerp(h1 + 1, h2, blendTime);
                }

                if (h > 1)
                {
                    h = h - 1;
                }

                return CustomBlenderColor.Lerp(CustomBlenderColor.HSVToRGB(h, s1, v1), CustomBlenderColor.HSVToRGB(h, s2, v2), blendTime);
            }
        }

        if(Interpolation == interpolationEnum.Ease)
        {
            blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);

            blendTime = -(Mathf.Cos(Mathf.PI * blendTime) - 1) / 2;

            return CustomBlenderColor.Lerp(keyLeft.Color, keyRight.Color, blendTime);
        }
        if (Interpolation == interpolationEnum.Linear)
        {
            blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
            return CustomBlenderColor.Lerp(keyLeft.Color, keyRight.Color, blendTime);
        }
        return keyLeft.Color;
    }

    public void ClearAllKeys()
    {
        keys.Clear();
        AddKey(CustomBlenderColor.black, 0);
        AddKey(CustomBlenderColor.white, 1);
        Interpolation = interpolationEnum.Linear;
        InterpolationHSV = interpolationHSVEnum.Near;
        ColorMode = colorModeEnum.RGB;
        selectedKeyIndex = 0;
    }

    public void FlipGradient()
    {
        ColorKey[] oldKeys = keys.ToArray();
        keys.Clear();
        for (int i = 0; i < oldKeys.Length; i++)
        {
            AddKey(oldKeys[i].Color, 1 - oldKeys[i].Time);
            /*ColorKey newKey = new ColorKey(oldKeys[i].Color, 1 - oldKeys[i].Time);
            keys.Add(newKey);*/
        }
        selectedKeyIndex = 0;
    }

    public void DistributeFromLeft()
    {
        if (keys.Count < 2)
            return;
        float distance = (float)1 / keys.Count;
        ColorKey[] oldKeys = keys.ToArray();
        keys.Clear();
        for(int i = 0; i < oldKeys.Length; i++)
        {
            ColorKey newKey = new ColorKey(oldKeys[i].Color, distance * i);
            keys.Add(newKey);
        }
        selectedKeyIndex = 0;
    }

    public void DistributeEvenly()
    {
        if (keys.Count < 2)
            return;
        float distance = (float)1 / (keys.Count - 1);
        ColorKey[] oldKeys = keys.ToArray();
        keys.Clear();
        for (int i = 0; i < oldKeys.Length; i++)
        {
            ColorKey newKey = new ColorKey(oldKeys[i].Color, distance * i);
            keys.Add(newKey);
        }
        selectedKeyIndex = 0;
    }

    public int AddKey(CustomBlenderColor color, float time)
    {
        ColorKey newKey = new ColorKey(color, time);
        for(int i = 0; i < keys.Count; i++)
        {
            if(newKey.Time < keys[i].Time)
            {
                keys.Insert(i, newKey);
                return i;
            }
        }

        keys.Add(newKey);
        return keys.Count - 1;
    }

    public void RemoveKey(int index)
    {
        if (keys.Count >= 2)
            keys.RemoveAt(index);
    }

    public int UpdateKeyTime(int index, float time)
    {
        CustomBlenderColor col = keys[index].Color;
        keys.RemoveAt(index);
        return AddKey(col, time);
    }

    public void UpdateKeyColor(int index, CustomBlenderColor col)
    {
        keys[index] = new ColorKey(col, keys[index].Time);
    }

    public int NumKeys
    {
        get
        {
            return keys.Count;
        }
    }

    public ColorKey GetKey(int i)
    {
        return keys[i];
    }

    public Texture2D GetTexture(int width)
    {
        Texture2D texture = new Texture2D(width, 1);
        Color[] colors = new Color[width];
        for(int i = 0; i < width; i++)
        {
            colors[i] = Evaluate((float)i / (width - 1)).gamma;
        }
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }


    [System.Serializable]
    public struct ColorKey
    {
        [SerializeField]
        CustomBlenderColor color;
        [SerializeField]
        float time;

        public ColorKey(CustomBlenderColor color, float time)
        {
            this.color = color;
            this.time = time;
        }

        public CustomBlenderColor Color
        {
            get
            {
                return color;
            }
        }

        public float Time
        {
            get
            {
                return time;
            }
        }
    }
}
