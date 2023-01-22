using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomBlenderGradient))]
public class BlenderGradientDrawer : PropertyDrawer
{
    float keyWidth = 7;
    float keyHeight = 12;

    Texture2D plus = Resources.Load<Texture2D>("xnode_plus");
    Texture2D minus = Resources.Load<Texture2D>("xnode_minus");
    Texture2D arrow = Resources.Load<Texture2D>("xnode_down_arrow");
    Texture2D pin = Resources.Load<Texture2D>("xnode_pin_arrow");
    float sizes = 10;
    float offs = 8;

    float dropDownSize = 55;

    Rect[] keyRects;
    bool isMouseDownOverKey;
    int selectedKeyIndex;

    arrowEnum ArrowEnum = arrowEnum.FlipColorRamp;
    enum arrowEnum { FlipColorRamp, DistributeKeysFromLeft, DistributeKeysEvenly, ResetColorRamp}
    GeneralNodePopup nodePopup = new GeneralNodePopup(Vector2.zero, new string[0], "Point");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //return base.GetPropertyHeight(property, label);
        return 20;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //draw
        CustomBlenderGradient gradient = (CustomBlenderGradient)fieldInfo.GetValue(property.serializedObject.targetObject);

        /*GUIStyle gradientStyle = new GUIStyle();
        gradientStyle.normal.background = gradient.GetTexture((int)position.width);
        GUI.Label(position, "", gradientStyle);*/

        Rect plusRect = new Rect(position.x, position.y, sizes, sizes);
        Rect minusRect = new Rect(position.x + sizes + offs, position.y, sizes, sizes);
        Rect arrowRect = new Rect(position.x + (sizes + offs) * 2, position.y, sizes, sizes);

        GUI.DrawTexture(plusRect, plus);
        GUI.DrawTexture(minusRect, minus);
        GUI.DrawTexture(arrowRect, arrow);

        Rect interpolationRect = new Rect(position.xMax - dropDownSize, position.y - 3, dropDownSize, position.height);
        Rect colorModeRect = new Rect(position.xMax - dropDownSize * 2 - 2, position.y - 3, dropDownSize, position.height);

        if (EditorGUI.DropdownButton(colorModeRect, new GUIContent(gradient.ColorMode.ToString()), FocusType.Keyboard))
        {
            string[] enumNames = System.Enum.GetNames(typeof(CustomBlenderGradient.colorModeEnum));
            nodePopup = new GeneralNodePopup(new Vector2(150, 70), enumNames, gradient.ColorMode.ToString());
            nodePopup.OnCloseEvent += () =>
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
                gradient.ColorMode = (CustomBlenderGradient.colorModeEnum)System.Enum.Parse(typeof(CustomBlenderGradient.colorModeEnum), nodePopup.EnumValue);
                BNGNodeEditor.NodeEditorWindow.current.Save();
            };
            PopupWindow.Show(new Rect(), nodePopup);
        }

        if (gradient.ColorMode == CustomBlenderGradient.colorModeEnum.RGB)
        {
            if (EditorGUI.DropdownButton(interpolationRect, new GUIContent(gradient.Interpolation.ToString()), FocusType.Keyboard))
            {
                string[] enumNames = System.Enum.GetNames(typeof(CustomBlenderGradient.interpolationEnum));
                nodePopup = new GeneralNodePopup(new Vector2(150, 90), enumNames, gradient.Interpolation.ToString());
                nodePopup.OnCloseEvent += () =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
                    gradient.Interpolation = (CustomBlenderGradient.interpolationEnum)System.Enum.Parse(typeof(CustomBlenderGradient.interpolationEnum), nodePopup.EnumValue);
                    BNGNodeEditor.NodeEditorWindow.current.Save();
                };
                PopupWindow.Show(new Rect(), nodePopup);
            }
        }
        else
        {
            if (EditorGUI.DropdownButton(interpolationRect, new GUIContent(gradient.InterpolationHSV.ToString()), FocusType.Keyboard))
            {
                string[] enumNames = System.Enum.GetNames(typeof(CustomBlenderGradient.interpolationHSVEnum));
                nodePopup = new GeneralNodePopup(new Vector2(150, 115), enumNames, gradient.InterpolationHSV.ToString());
                nodePopup.OnCloseEvent += () =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
                    gradient.InterpolationHSV = (CustomBlenderGradient.interpolationHSVEnum)System.Enum.Parse(typeof(CustomBlenderGradient.interpolationHSVEnum), nodePopup.EnumValue);
                    BNGNodeEditor.NodeEditorWindow.current.Save();
                };
                PopupWindow.Show(new Rect(), nodePopup);
            }
        }

        position = new Rect(position.x, position.y + 20, position.width, position.height);

        Vector2 center = position.size / 2f;
        float zoom = BNGNodeEditor.NodeEditorWindow.current.zoom;
        Vector2 panOffset = BNGNodeEditor.NodeEditorWindow.current.panOffset;

        Texture2D alphaTex = BNGNodeEditor.NodeEditorResources.GenerateAlphaTexture(new CustomBlenderColor(0.45f), new CustomBlenderColor(0.65f));

        float xOffset = -(center.x * zoom + panOffset.x) / alphaTex.width;
        float yOffset = ((center.y - position.size.y) * zoom + panOffset.y) / alphaTex.height;
        Vector2 tileOffset = new Vector2(xOffset, yOffset) * 2;

        float tileAmountX = Mathf.Round(position.size.x / zoom * 6) / alphaTex.width;
        float tileAmountY = Mathf.Round(position.size.y / zoom * 6) / alphaTex.height;
        Vector2 tileAmount = new Vector2(tileAmountX, tileAmountY);

        GUI.DrawTextureWithTexCoords(position, alphaTex, new Rect(tileOffset, tileAmount));

        //EditorGUI.DrawRect(new Rect(position.x - 1, position.y - 1, position.width + 2, position.height + 2), Color.black);
        GUI.DrawTexture(position, gradient.GetTexture((int)position.width));
        Rect bottomTex = new Rect(position.x, position.y + 16, position.width, 4);
        GUI.DrawTexture(bottomTex, gradient.GetTexture((int)position.width), ScaleMode.StretchToFill, false);

        keyRects = new Rect[gradient.NumKeys];

        for (int i = 0; i < gradient.NumKeys; i++)
        {
            if (i == selectedKeyIndex)
                continue;
            CustomBlenderGradient.ColorKey key = gradient.GetKey(i);
            Rect keyRectf = new Rect(position.x + position.width * key.Time - keyWidth / 2f, position.yMax - 5, keyWidth, keyHeight);
            //EditorGUI.DrawRect(keyRectf, key.Color);

            GUI.DrawTexture(new Rect(keyRectf.x - 1, keyRectf.y - 1, keyRectf.width + 2, keyRectf.height + 2), 
                pin, ScaleMode.StretchToFill, true, 0, Color.black, 0, 0);
            GUI.DrawTexture(keyRectf, pin, ScaleMode.StretchToFill, true, 0, 
                new Color(key.Color.gamma.r, key.Color.gamma.g, key.Color.gamma.b, 1), 0, 0);

            keyRects[i] = keyRectf;
        }

        Rect keyRect = new Rect(position.x + position.width * gradient.GetKey(selectedKeyIndex).Time - keyWidth / 2f, position.yMax - keyHeight/2f, keyWidth, keyHeight);
        Color selCol = gradient.GetKey(selectedKeyIndex).Color.gamma;
        GUI.DrawTexture(new Rect(keyRect.x - 1, keyRect.y - 1, keyRect.width + 2, keyRect.height + 2),
                pin, ScaleMode.StretchToFill, true, 0, Color.white, 0, 0);
        GUI.DrawTexture(keyRect, pin, ScaleMode.StretchToFill, true, 0, new Color(selCol.r, selCol.g, selCol.b, 1), 0, 0);
        keyRects[selectedKeyIndex] = keyRect;

        GUILayout.Space(30);
        GUILayout.Space(55);

        Rect keyButtonPos = new Rect(position.x, position.y + 30, position.width / 4 - 5, position.height);
        SerializedProperty keysLocation = property.FindPropertyRelative(nameof(CustomBlenderGradient.selectedKeyIndex));        
        EditorGUI.PropertyField(keyButtonPos, keysLocation, new GUIContent(""));

        Rect timeButtonPos = new Rect(position.x + position.width / 4, position.y + 30, position.width / 4 * 3, position.height);
        EditorGUI.BeginChangeCheck();
        float newTime = EditorGUI.FloatField(timeButtonPos, gradient.GetKey(selectedKeyIndex).Time);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
            gradient.selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, newTime);
            BNGNodeEditor.NodeEditorWindow.current.Save();
        }

        Rect colorButtonPos = new Rect(position.x, position.y + 55, position.width, position.height);
        //EditorGUI.BeginChangeCheck();
        //Debug.Log(gradient);
        SerializedProperty colorField = property.FindPropertyRelative(nameof(CustomBlenderGradient.keys));
        //Debug.Log(colorField.GetArrayElementAtIndex(selectedKeyIndex).FindPropertyRelative("color"));
        EditorGUI.PropertyField(colorButtonPos, colorField.GetArrayElementAtIndex(selectedKeyIndex).FindPropertyRelative("color"), GUIContent.none);


        if (property.serializedObject.ApplyModifiedProperties())
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
            if (gradient.selectedKeyIndex > gradient.NumKeys - 1)
            {
                gradient.selectedKeyIndex = gradient.NumKeys - 1;
            }
            if (gradient.selectedKeyIndex < 0)
            {
                gradient.selectedKeyIndex = 0;
            }
            BNGNodeEditor.NodeEditorWindow.current.Save();
        }

        //input

        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (plusRect.Contains(guiEvent.mousePosition) && !isMouseDownOverKey)
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
                float keyTime = 0;
                if(gradient.NumKeys > 2)
                {
                    if (gradient.NumKeys > selectedKeyIndex + 1)
                        keyTime = Mathf.Lerp(gradient.GetKey(selectedKeyIndex).Time, gradient.GetKey(selectedKeyIndex + 1).Time, 0.5f);
                    else
                        keyTime = Mathf.Lerp(gradient.GetKey(selectedKeyIndex).Time, gradient.GetKey(selectedKeyIndex - 1).Time, 0.5f);
                }
                else
                {
                    if(gradient.GetKey(selectedKeyIndex).Time > 0.5f)
                    {
                        keyTime = Mathf.Lerp(gradient.GetKey(selectedKeyIndex).Time, 0, 0.5f);
                    }
                    else
                    {
                        keyTime = Mathf.Lerp(gradient.GetKey(selectedKeyIndex).Time, 1, 0.5f);
                    }
                }
                CustomBlenderColor interpolatedColor = gradient.Evaluate(keyTime);
                selectedKeyIndex = gradient.AddKey(interpolatedColor, keyTime);
                gradient.selectedKeyIndex = selectedKeyIndex;
                BNGNodeEditor.NodeEditorWindow.current.Save();
            }

            if(minusRect.Contains(guiEvent.mousePosition) && !isMouseDownOverKey)
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
                gradient.RemoveKey(selectedKeyIndex);
                if (selectedKeyIndex >= gradient.NumKeys)
                {
                    selectedKeyIndex--;
                    gradient.selectedKeyIndex = selectedKeyIndex;
                }
                BNGNodeEditor.NodeEditorWindow.current.Repaint();
                BNGNodeEditor.NodeEditorWindow.current.Save();
            }

            if (arrowRect.Contains(guiEvent.mousePosition) && !isMouseDownOverKey)
            {
                string[] enumNames = System.Enum.GetNames(typeof(arrowEnum));
                nodePopup = new GeneralNodePopup(new Vector2(150, 80), enumNames, ArrowEnum.ToString());
                nodePopup.OnCloseEvent += () => 
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
                    if (!nodePopup.isButtonPressed)
                        return;

                    if((arrowEnum)System.Enum.Parse(typeof(arrowEnum), nodePopup.EnumValue) == arrowEnum.FlipColorRamp)
                        gradient.FlipGradient();

                    if ((arrowEnum)System.Enum.Parse(typeof(arrowEnum), nodePopup.EnumValue) == arrowEnum.DistributeKeysFromLeft)
                        gradient.DistributeFromLeft();

                    if ((arrowEnum)System.Enum.Parse(typeof(arrowEnum), nodePopup.EnumValue) == arrowEnum.DistributeKeysEvenly)
                        gradient.DistributeEvenly();

                    if ((arrowEnum)System.Enum.Parse(typeof(arrowEnum), nodePopup.EnumValue) == arrowEnum.ResetColorRamp)
                    {
                        gradient.ClearAllKeys();
                        selectedKeyIndex = 0;
                    }

                    BNGNodeEditor.NodeEditorWindow.current.Save();
                };
                PopupWindow.Show(new Rect(), nodePopup);
            }

            for (int i = 0; i < keyRects.Length; i++)
            {
                if (keyRects[i].Contains(guiEvent.mousePosition))
                {
                    isMouseDownOverKey = true;
                    selectedKeyIndex = i;
                    gradient.selectedKeyIndex = selectedKeyIndex;
                    break;
                }
            }
            selectedKeyIndex = gradient.selectedKeyIndex;
        }

        if(isMouseDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
        {
            float keyTime = Mathf.InverseLerp(position.x, position.xMax, guiEvent.mousePosition.x);
            Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
            selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
            gradient.selectedKeyIndex = selectedKeyIndex;
            BNGNodeEditor.NodeEditorWindow.current.Repaint();
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Color Ramp Change");
            selectedKeyIndex = gradient.selectedKeyIndex;
            isMouseDownOverKey = false;
            BNGNodeEditor.NodeEditorWindow.current.Save();
        }

        selectedKeyIndex = gradient.selectedKeyIndex;

        /*if(guiEvent.keyCode == KeyCode.Backspace && guiEvent.type == EventType.KeyDown)
        {
            gradient.RemoveKey(selectedKeyIndex);
            if(selectedKeyIndex >= gradient.NumKeys)
            {
                selectedKeyIndex--;
            }
            BNGNodeEditor.NodeEditorWindow.current.Repaint();
        }*/
    }
}
