using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class BlenderSingleAttribute : PropertyAttribute
{
    public BlenderSingleAttribute()
    {
    }
}

[CustomPropertyDrawer(typeof(BlenderSingleAttribute))]
public class BlenderSingleDrawer : PropertyDrawer
{
    float arrowSpace = 15;
    float colMul = 1.2f;
    float colHold = 0.7f;

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

        Rect posSpaced = new Rect(position.x + arrowSpace, position.y, position.width - arrowSpace * 2, position.height);

        Rect leftArrow = new Rect(position.x, position.y, arrowSpace, position.height);
        Rect rightArrow = new Rect(posSpaced.xMax, position.y, arrowSpace, position.height);

        Rect labelPos = new Rect(posSpaced.x, posSpaced.y, posSpaced.width, posSpaced.height);
        Rect valuePos = new Rect(posSpaced.x, posSpaced.y, posSpaced.width, posSpaced.height);

        GUIStyle rightAlign = EditorStyles.label;
        rightAlign.alignment = TextAnchor.MiddleRight;
        if (!isEditingArea)
        {
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
                GUI.Label(valuePos, property.floatValue.ToString(), rightAlign);
            else
                GUI.Label(valuePos, (property.floatValue + dragDistance).ToString(), rightAlign);

        }
        else
        {
            EditorGUI.BeginProperty(position, label, property);
            GUI.SetNextControlName("ValueChange" + label.text);
            EditorGUI.PropertyField(position, property, GUIContent.none);
            EditorGUI.FocusTextInControl("ValueChange" + label.text);
            EditorGUI.EndProperty();
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
            if (!isShifting)
            {
                dragDistance = (guiEvent.mousePosition.x - mouseFirstPos.x) / (10f * BNGNodeEditor.NodeEditorWindow.current.zoom);
            }
            else
            {
                dragDistance = (guiEvent.mousePosition.x - mouseFirstPos.x) / (100f * BNGNodeEditor.NodeEditorWindow.current.zoom);
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
            if (rightArrow.Contains(Event.current.mousePosition))
            {
                property.floatValue += 0.1f;
                property.serializedObject.ApplyModifiedProperties();
            }
            else if (leftArrow.Contains(Event.current.mousePosition))
            {
                property.floatValue -= 0.1f;
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                isEditingArea = true;
            }
            buttonClicked = false;
        }
    }
}
