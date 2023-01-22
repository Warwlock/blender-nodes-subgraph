using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BlenderRangeAttribute))]
public class BlenderSingleRangeDrawer : PropertyDrawer
{
    float arrowSpace = 15;
    float colMul = 1.2f;
    float colHold = 0.7f;
    Color colRange = new Color(0.8f, 1.1f, 1.4f);

    bool showArrows = false;

    bool buttonClicked = false;
    bool isButtonHeldDown = false;
    bool isEditingArea = false;

    bool isMovable = false;
    Vector2 mouseFirstPos;
    float dragDistance;
    bool isShifting;
    bool isDraggedWhileMoving = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //float value = 0;// (float)fieldInfo.GetValue(property.serializedObject.targetObject);
        BlenderRangeAttribute range = attribute as BlenderRangeAttribute;

        Rect posSpaced = new Rect(position.x + arrowSpace, position.y, position.width - arrowSpace * 2, position.height);

        Rect leftArrow = new Rect(position.x, position.y, arrowSpace, position.height);
        Rect rightArrow = new Rect(posSpaced.xMax, position.y, arrowSpace, position.height);

        Rect labelPos = new Rect(posSpaced.x, posSpaced.y, posSpaced.width, posSpaced.height);
        Rect valuePos = new Rect(posSpaced.x, posSpaced.y, posSpaced.width, posSpaced.height);

        GUIStyle rightAlign = EditorStyles.label;
        rightAlign.alignment = TextAnchor.MiddleRight;

        if (!isEditingArea)
        {
            /*if (GUI.Button(position, GUIContent.none))
                buttonClicked = true;*/
            Color oldCol = GUI.color;
            if (!isButtonHeldDown)
            {                
                GUI.color = new Color(colMul, colMul, colMul);
                GUI.Box(position, GUIContent.none, EditorStyles.miniButton);
            }
            else
            {
                GUI.color = new Color(colHold, colHold, colHold);
                GUI.Box(position, GUIContent.none, EditorStyles.miniButton);
            }

            if (range.useSlider)
            {
                GUI.color = colRange;
                float xPos = (property.floatValue + dragDistance).Remap(range.min, range.max, position.x, position.xMax);

                if (xPos < position.x)
                    xPos = position.x;
                if (xPos > position.xMax)
                    xPos = position.xMax;

                Rect RangeRect = new Rect(position.x, position.y, xPos - position.x, position.height);
                GUI.Box(RangeRect, GUIContent.none, EditorStyles.miniButton);
            }

            if (showArrows)
            {
                if (!isButtonHeldDown)
                {
                    GUI.color = new Color(colMul, colMul, colMul);
                    GUI.Box(leftArrow, GUIContent.none, EditorStyles.miniButton);
                    GUI.Box(rightArrow, GUIContent.none, EditorStyles.miniButton);
                }
                else
                {
                    GUI.color = new Color(colHold, colHold, colHold);
                    GUI.Box(leftArrow, GUIContent.none, EditorStyles.miniButton);
                    GUI.Box(rightArrow, GUIContent.none, EditorStyles.miniButton);
                    
                }
            }
            GUI.color = oldCol;

            GUI.Label(labelPos, label);

            if (!isMovable)
                GUI.Label(valuePos,
                    range.asInt ? ((int)(property.floatValue + dragDistance)).ToString() : (property.floatValue + dragDistance).ToString(),
                    rightAlign);
            else
                GUI.Label(valuePos, 
                    range.asInt ? ((int)(property.floatValue + dragDistance)).ToString() : (property.floatValue + dragDistance).ToString(), 
                    rightAlign);

        }
        else
        {
            GUI.SetNextControlName("ValueChange" + label.text);
            EditorGUI.PropertyField(position, property, GUIContent.none);
            EditorGUI.FocusTextInControl("ValueChange" + label.text);
        }

        if (range.forcedRange)
        {
            if(property.floatValue > range.max)
            {
                property.floatValue = range.max;
            }
            if (property.floatValue < range.min)
            {
                property.floatValue = range.min;
            }
        }

        rightAlign.alignment = TextAnchor.MiddleLeft;

        //input

        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (position.Contains(Event.current.mousePosition))
            {
                isMovable = true;
                mouseFirstPos = guiEvent.mousePosition;
            }
            else
            {
                if (isEditingArea)
                {
                    EditorGUI.FocusTextInControl(null);
                    EditorGUIUtility.editingTextField = false;
                    isEditingArea = false;
                }
            }
        }

        if (guiEvent.type == EventType.MouseMove)
        {
            if (position.Contains(Event.current.mousePosition))
            {
                if (!range.useSlider)
                    showArrows = true;
            }
            else
            {
                showArrows = false;
                if (isEditingArea)
                {
                    EditorGUI.FocusTextInControl(null);
                    EditorGUIUtility.editingTextField = false;
                    isEditingArea = false;
                }
            }
        }

        if(isMovable)
        {
            if (range.asInt)
            {
                dragDistance = (guiEvent.mousePosition.x - mouseFirstPos.x);
            }
            else if (!range.useSlider)
            {
                if (!isShifting)
                {
                    dragDistance = (guiEvent.mousePosition.x - mouseFirstPos.x) / (10f * BNGNodeEditor.NodeEditorWindow.current.zoom);
                }
                else
                {
                    dragDistance = (guiEvent.mousePosition.x - mouseFirstPos.x) / (100f * BNGNodeEditor.NodeEditorWindow.current.zoom);
                }
            }
            else if(range.useSlider)
            {
                if (!isShifting)
                {
                    dragDistance = (guiEvent.mousePosition.x - mouseFirstPos.x) / (150f * BNGNodeEditor.NodeEditorWindow.current.zoom);
                }
                else
                {
                    dragDistance = (guiEvent.mousePosition.x - mouseFirstPos.x) / (1200f * BNGNodeEditor.NodeEditorWindow.current.zoom);
                }
            }
            isButtonHeldDown = true;
            if (guiEvent.isMouse)
            {
                if (!position.Contains(Event.current.mousePosition))
                {
                    isDraggedWhileMoving = false;
                    isMovable = false;
                    isButtonHeldDown = false;
                    property.floatValue += dragDistance;
                    dragDistance = 0;
                }
            }
            if (property.floatValue + dragDistance > range.max)
            {
                property.floatValue = range.max;
                dragDistance = 0;
            }
            if (property.floatValue + dragDistance < range.min)
            {
                property.floatValue = range.min;
                dragDistance = 0;
            }
        }

        if (guiEvent.isKey)
        {

            isShifting = guiEvent.shift;

            if(guiEvent.keyCode == KeyCode.Escape)
            {
                isMovable = false;
                isButtonHeldDown = false;
                isDraggedWhileMoving = true;
                dragDistance = 0;
                BNGNodeEditor.NodeEditorWindow.RepaintAll();
            }
        }

        if(guiEvent.type == EventType.MouseDrag)
        {
            BNGNodeEditor.NodeEditorWindow.RepaintAll();
            if (isMovable)
            {
                isDraggedWhileMoving = true;
            }
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            if (position.Contains(Event.current.mousePosition))
            {
                if (!isDraggedWhileMoving)
                {
                    buttonClicked = true;
                }
            }
            isDraggedWhileMoving = false;
            isMovable = false;
            isButtonHeldDown = false;
            property.floatValue += dragDistance;
            dragDistance = 0;
        }
        if (buttonClicked)
        {
            if (rightArrow.Contains(Event.current.mousePosition) && showArrows)
            {
                if (property.floatValue + dragDistance < range.max)
                {
                    property.floatValue += range.asInt ? 1 : 0.1f;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else if (leftArrow.Contains(Event.current.mousePosition) && showArrows)
            {
                if (property.floatValue + dragDistance > range.min)
                {
                    property.floatValue -= range.asInt ? 1 : 0.1f;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                isEditingArea = true;
            }
            buttonClicked = false;
        }
    }
}

public static class ExtensionMethods
{

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}
