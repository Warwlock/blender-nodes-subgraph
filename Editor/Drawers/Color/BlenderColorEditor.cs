using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNodeEditor;

public class BlenderColorEditor : PopupWindowContent
{
    Texture2D colorWheel = Resources.Load<Texture2D>("xnode_color_wheel");
    Texture2D colorLine = Resources.Load<Texture2D>("xnode_color_line");
    Texture2D dot = Resources.Load<Texture2D>("xnode_dot");
    int index = 0;

    string editorPrefStr = "blenderGraph.ColorMode";

    float h, s, v;
    public Color rgba;

    [BlenderRange(0, 1)] float newR, newG, newB, newA;

    Color GetCol(float x)
    {
        return new Color(x, x, x, 1);
    }

    public BlenderColorEditor(Color rgba)
    {
        this.rgba = rgba;
    }
    public override Vector2 GetWindowSize()
    {
        return new Vector2(185, 270);
    }
    public override void OnGUI(Rect rect)
    {
        Color.RGBToHSV(rgba.gamma, out h, out s, out v);

        Rect position = new Rect(rect.x + 5, rect.y + 5, 185, 160);

        Rect colorLineRect = new Rect(position.x + 153, position.y + 5, 20, 140);
        GUI.DrawTexture(colorLineRect, colorLine);

        Rect colorWheelRect = new Rect(position.x, position.y, 150, 150);
        GUI.DrawTexture(colorWheelRect, colorWheel, ScaleMode.ScaleToFit, true, 0, GetCol(v), 0, 0);

        Rect colorLinePointRect = new Rect(position.x + 158, Mathf.Lerp(position.y + 140, position.y, v), 10, 10);
        GUI.DrawTexture(new Rect(colorLinePointRect.x - 1, colorLinePointRect.y - 1, colorLinePointRect.width + 2, colorLinePointRect.height + 2)
            , dot, ScaleMode.StretchToFill, true, 0, Color.black, 0, 0);
        GUI.DrawTexture(colorLinePointRect, dot);

        float radius = Mathf.Lerp(0, 75, s);
        float degH = Mathf.Lerp(360, 0, h) * Mathf.Deg2Rad;
        float sinDegH = Mathf.Sin(degH);
        float cosDegH = Mathf.Cos(degH);

        Rect colorWheelPointRect = new Rect((position.x + 70) + radius * sinDegH, (position.y + 70) + radius * cosDegH, 10, 10);
        GUI.DrawTexture(new Rect(colorWheelPointRect.x - 1, colorWheelPointRect.y - 1, colorWheelPointRect.width + 2, colorWheelPointRect.height + 2)
            , dot, ScaleMode.StretchToFill, true, 0, Color.black, 0, 0);
        GUI.DrawTexture(colorWheelPointRect, dot);

        Rect areaBegin = new Rect(position.x, position.yMax, rect.width - 10, rect.height - 160);
        Rect buttonOutline = new Rect(areaBegin.x - 1, areaBegin.y - 1, areaBegin.width + 2, 22);
        EditorGUI.DrawRect(buttonOutline, Color.black);
        GUILayout.BeginArea(areaBegin);
        GUILayout.BeginHorizontal();
        if(GUILayout.Toggle(EditorPrefs.GetInt(editorPrefStr, 0) == 0 ? true : false, "RGB", EditorStyles.toolbarButton))
        {
            EditorPrefs.SetInt(editorPrefStr, 0);
        }
        if(GUILayout.Toggle(EditorPrefs.GetInt(editorPrefStr, 0) == 1 ? true : false, "HSV", EditorStyles.toolbarButton))
        {
            EditorPrefs.SetInt(editorPrefStr, 1);
        }
       if(GUILayout.Toggle(EditorPrefs.GetInt(editorPrefStr, 0) == 2 ? true : false, "Hex", EditorStyles.toolbarButton))
        {
            EditorPrefs.SetInt(editorPrefStr, 2);
        }
        GUILayout.EndHorizontal();

        if(EditorPrefs.GetInt(editorPrefStr, 0) == 0)
        {
            newR = EditorGUILayout.Slider(rgba.r, 0, 1);
            newG = EditorGUILayout.Slider(rgba.g, 0, 1);
            newB = EditorGUILayout.Slider(rgba.b, 0, 1);
            newA = EditorGUILayout.Slider(rgba.a, 0, 1);
            rgba = new Color(newR, newG, newB, newA);
        }
        if (EditorPrefs.GetInt(editorPrefStr, 0) == 1)
        {
            float rgbaH, rgbaS, rgbaV;
            Color.RGBToHSV(rgba, out rgbaH, out rgbaS, out rgbaV);
            float newH = EditorGUILayout.Slider(rgbaH, 0, 1);
            float newS = EditorGUILayout.Slider(rgbaS, 0, 1);
            float newV = EditorGUILayout.Slider(rgbaV, 0, 1);
            float newA = EditorGUILayout.Slider(rgba.a, 0, 1);
            Color hsvCol = Color.HSVToRGB(newH, newS, newV);
            rgba = new Color(hsvCol.r, hsvCol.g, hsvCol.b, newA);
        }
        if (EditorPrefs.GetInt(editorPrefStr, 0) == 2)
        {
            string hexString = ColorUtility.ToHtmlStringRGB(rgba.gamma);
            Color hexCol;
            
            hexString = EditorGUILayout.TextField(hexString);
            GUILayout.Label("(Gamma corrected)");
            GUILayout.Space(21);
            if (ColorUtility.TryParseHtmlString("#" + hexString, out hexCol))
            {
                hexCol = hexCol.linear;
                float newA = EditorGUILayout.Slider(rgba.a, 0, 1);
                rgba = new Color(hexCol.r, hexCol.g, hexCol.b, newA);
            }
        }
        GUILayout.EndArea();

        //input

        Event guiEvent = Event.current;

        if ((guiEvent.type == EventType.MouseDown || guiEvent.type == EventType.MouseDrag) && guiEvent.button == 0)
        {
            if (colorLineRect.Contains(guiEvent.mousePosition))
            {
                v = Mathf.InverseLerp(colorLineRect.yMax, colorLineRect.y, guiEvent.mousePosition.y);
                float a = rgba.a;
                rgba = Color.HSVToRGB(h, s, v).linear;
                rgba = new Color(rgba.r, rgba.g, rgba.b, a);
                editorWindow.Repaint();
            }
            if (colorWheelRect.Contains(guiEvent.mousePosition))
            {
                s = Mathf.InverseLerp(0, 75, Mathf.Abs(Vector2.Distance(guiEvent.mousePosition, new Vector2(position.x + 75, position.y + 75))));

                float sign = (position.x + 75 < guiEvent.mousePosition.x) ? -1.0f : 1.0f;
                float angle = Vector2.Angle(guiEvent.mousePosition - new Vector2(position.x + 75, position.y + 75), Vector2.down) * -sign;
                h = Mathf.InverseLerp(0, 360, angle + 180);

                float a = rgba.a;
                rgba = Color.HSVToRGB(h, s, v).linear;
                rgba = new Color(rgba.r, rgba.g, rgba.b, a);
                editorWindow.Repaint();
            }
        }
        //Debug.Log(rgba);
        OnUpdateEvent();
        OneTimePosition();
    }

    void OneTimePosition()
    {
        if (index < 3)
        {
            NodeEditorWindow.current.onLateGUI += () => editorWindow.position = new Rect(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), new Vector2(0, 0));
            index++;
        }
    }

    public event System.Action OnCloseEvent;
    public event System.Action OnUpdateEvent;
    public override void OnClose()
    {
        if(OnUpdateEvent != null)
        {
            OnUpdateEvent = null;
        }
        if (OnCloseEvent != null)
        {
            OnCloseEvent();
            OnCloseEvent = null;
        }
    }
}
