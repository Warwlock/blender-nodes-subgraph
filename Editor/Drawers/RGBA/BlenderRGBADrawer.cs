using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomBlenderRGBA))]
public class BlenderRGBADrawer : PropertyDrawer
{
    Texture2D colorWheel = Resources.Load<Texture2D>("xnode_color_wheel");
    Texture2D colorLine = Resources.Load<Texture2D>("xnode_color_line");
    Texture2D dot = Resources.Load<Texture2D>("xnode_dot");

    float h, s, v;
    Color GetCol(float x)
    {
        return new Color(x, x, x, 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        CustomBlenderRGBA rgba = (CustomBlenderRGBA)fieldInfo.GetValue(property.serializedObject.targetObject);

        /*Rect area = new Rect(position.x, position.y, position.width, 150);
        EditorGUI.DrawRect(area, Color.white);*/

        Color.RGBToHSV(rgba.gamma, out h, out s, out v);

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

        Rect colorButtonPos = new Rect(position.x, position.y + 160, position.width, position.height);
        //EditorGUI.BeginChangeCheck();
        //Debug.Log(gradient);
        SerializedProperty colorField = property.FindPropertyRelative(nameof(CustomBlenderRGBA.col));
        //Debug.Log(colorField.GetArrayElementAtIndex(selectedKeyIndex).FindPropertyRelative("color"));
        EditorGUI.PropertyField(colorButtonPos, colorField, GUIContent.none);

        GUILayout.Space(160);

        //input

        Event guiEvent = Event.current;

        if ((guiEvent.type == EventType.MouseDown || guiEvent.type == EventType.MouseDrag) && guiEvent.button == 0)
        {
            if (colorLineRect.Contains(guiEvent.mousePosition))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "RGBA Change");
                v = Mathf.InverseLerp(colorLineRect.yMax, colorLineRect.y, guiEvent.mousePosition.y);
                float a = rgba.a;
                rgba.col = CustomBlenderColor.HSVToRGB(h, s, v).linear;
                rgba.col = new CustomBlenderColor(rgba.r, rgba.g, rgba.b, a);
                BNGNodeEditor.NodeEditorWindow.current.Repaint();
            }
            if (colorWheelRect.Contains(guiEvent.mousePosition))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "RGBA Change");
                s = Mathf.InverseLerp(0, 75, Mathf.Abs(Vector2.Distance(guiEvent.mousePosition, new Vector2(position.x + 75, position.y + 75))));

                float sign = (position.x + 75 < guiEvent.mousePosition.x) ? -1.0f : 1.0f;
                float angle = Vector2.Angle(guiEvent.mousePosition - new Vector2(position.x + 75, position.y + 75), Vector2.down) * -sign;
                h = Mathf.InverseLerp(0, 360, angle + 180);

                float a = rgba.a;
                rgba.col = CustomBlenderColor.HSVToRGB(h, s, v).linear;
                rgba.col = new CustomBlenderColor(rgba.r, rgba.g, rgba.b, a);
                BNGNodeEditor.NodeEditorWindow.current.Repaint();
            }
        }

        if(guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            BNGNodeEditor.NodeEditorWindow.current.Save();
        }
    }
}
