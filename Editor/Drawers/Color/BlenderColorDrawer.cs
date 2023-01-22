using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomBlenderColor))]
public class BlenderColorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        //CustomBlenderColor propertyColor = (CustomBlenderColor)fieldInfo.GetValue(property.serializedObject.targetObject);
        float r = property.FindPropertyRelative("r").floatValue;
        float g = property.FindPropertyRelative("g").floatValue;
        float b = property.FindPropertyRelative("b").floatValue;
        float a = property.FindPropertyRelative("a").floatValue;
        Color propertyColor = new Color(r, g, b, a);

        Rect rectHolder = new Rect(position.x, position.y, position.width, position.height);
        if(label == null || label == GUIContent.none)
        {
            rectHolder = new Rect(position.x, position.y, position.width, position.height);
        }
        else
        {
            GUI.Label(position, label);
            rectHolder = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);
        }
        Rect left = new Rect(rectHolder.x, rectHolder.y, rectHolder.width / 2, position.height);
        Rect right = new Rect(rectHolder.x + rectHolder.width / 2, rectHolder.y, rectHolder.width / 2, position.height);
        Color noAlpha = new Color(propertyColor.gamma.r, propertyColor.gamma.g, propertyColor.gamma.b, 1);

        Vector2 center = right.size / 2f;
        float zoom = BNGNodeEditor.NodeEditorWindow.current.zoom;
        Vector2 panOffset = BNGNodeEditor.NodeEditorWindow.current.panOffset;

        Texture2D alphaTex = BNGNodeEditor.NodeEditorResources.GenerateAlphaTexture(new CustomBlenderColor(0.45f), new CustomBlenderColor(0.65f));

        float xOffset = -(center.x * zoom + panOffset.x) / alphaTex.width;
        float yOffset = ((center.y - right.size.y) * zoom + panOffset.y) / alphaTex.height;
        Vector2 tileOffset = new Vector2(xOffset, yOffset) * 2;

        float tileAmountX = Mathf.Round(right.size.x / zoom * 6) / alphaTex.width;
        float tileAmountY = Mathf.Round(right.size.y / zoom * 6) / alphaTex.height;
        Vector2 tileAmount = new Vector2(tileAmountX, tileAmountY);

        GUI.DrawTextureWithTexCoords(right, alphaTex, new Rect(tileOffset, tileAmount));

        EditorGUI.DrawRect(left, noAlpha);
        EditorGUI.DrawRect(right, propertyColor.gamma);

        //input

        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (position.Contains(guiEvent.mousePosition))
            {
                BlenderColorEditor colorPopup = new BlenderColorEditor(propertyColor);
                colorPopup.OnUpdateEvent += () =>
                {
                    property.FindPropertyRelative("r").floatValue = colorPopup.rgba.r;
                    property.FindPropertyRelative("g").floatValue = colorPopup.rgba.g;
                    property.FindPropertyRelative("b").floatValue = colorPopup.rgba.b;
                    property.FindPropertyRelative("a").floatValue = colorPopup.rgba.a;
                    property.serializedObject.ApplyModifiedProperties();
                    BNGNodeEditor.NodeEditorWindow.current.Repaint();
                };
                colorPopup.OnCloseEvent += () =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Color Change");
                    property.FindPropertyRelative("r").floatValue = colorPopup.rgba.r;
                    property.FindPropertyRelative("g").floatValue = colorPopup.rgba.g;
                    property.FindPropertyRelative("b").floatValue = colorPopup.rgba.b;
                    property.FindPropertyRelative("a").floatValue = colorPopup.rgba.a;
                    property.serializedObject.ApplyModifiedProperties();
                    BNGNodeEditor.NodeEditorWindow.current.Save();
                };
                PopupWindow.Show(position, colorPopup);
            }
        }
        property.serializedObject.ApplyModifiedProperties();
    }
}