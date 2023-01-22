using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class BlenderVectorAttribute : PropertyAttribute
{
    public BlenderVectorAttribute()
    {
    }
}

[CustomPropertyDrawer(typeof(BlenderVectorAttribute))]
public class BlenderVectorDrawer : PropertyDrawer
{
    float arrowSpace = 15;
    float colMul = 1.2f;
    float colHold = 0.7f;

    bool showArrows = false;

    bool buttonClicked = false;
    bool isButtonHeldDown = false;
    bool isEditingArea = false;

    int vectorValue = 0;
    int vectorArrowValue = 0;

    bool isMovable = false;
    Vector2 mouseFirstPos;
    float dragDistance;
    bool isShifting;
    bool isDraggedWhileMoving = false;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //return base.GetPropertyHeight(property, label);
        return 72;
    }

    void AddPropertyField(Rect position, Rect leftArrow, Rect rightArrow, float vecVal)
    {
        Color oldCol = GUI.color;
        if (isButtonHeldDown && vectorValue == vecVal)
        {
            GUI.color = new Color(colHold, colHold, colHold);
            GUI.Box(position, GUIContent.none, EditorStyles.miniButton);
        }
        else
        {
            GUI.color = new Color(colMul, colMul, colMul);
            GUI.Box(position, GUIContent.none, EditorStyles.miniButton);
        }

        if (showArrows && vectorArrowValue == vecVal)
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
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //float value = 0;// (float)fieldInfo.GetValue(property.serializedObject.targetObject);

        Vector3 a = (Vector3)fieldInfo.GetValue(property.serializedObject.targetObject);

        Rect posSpaced = new Rect(position.x + arrowSpace, position.y, position.width - arrowSpace * 2, position.height);

        Rect posX = new Rect(position.x, position.y + 18, position.width, position.height / 4);
        Rect posY = new Rect(position.x, position.y + 36, position.width, position.height / 4);
        Rect posZ = new Rect(position.x, position.y + 54, position.width, position.height / 4);

        Rect leftArrowX = new Rect(position.x, position.y + 18, arrowSpace, position.height / 4);
        Rect leftArrowY = new Rect(position.x, position.y + 36, arrowSpace, position.height / 4);
        Rect leftArrowZ = new Rect(position.x, position.y + 54, arrowSpace, position.height / 4);

        Rect rightArrowX = new Rect(posSpaced.xMax, position.y + 18, arrowSpace, position.height / 4);
        Rect rightArrowY = new Rect(posSpaced.xMax, position.y + 36, arrowSpace, position.height / 4);
        Rect rightArrowZ = new Rect(posSpaced.xMax, position.y + 54, arrowSpace, position.height / 4);

        Rect labelPos = new Rect(position.x, position.y, position.width, position.height / 4);
        Rect valuePosX = new Rect(posSpaced.x, posSpaced.y + 18, posSpaced.width, posSpaced.height / 4);
        Rect valuePosY = new Rect(posSpaced.x, posSpaced.y + 36, posSpaced.width, posSpaced.height / 4);
        Rect valuePosZ = new Rect(posSpaced.x, posSpaced.y + 54, posSpaced.width, posSpaced.height / 4);

        GUIStyle rightAlign = EditorStyles.label;
        rightAlign.alignment = TextAnchor.MiddleRight;

        if (!isEditingArea)
        {
            /*if (GUI.Button(position, GUIContent.none))
                buttonClicked = true;*/

            AddPropertyField(posX, leftArrowX, rightArrowX, 0);
            AddPropertyField(posY, leftArrowY, rightArrowY, 1);
            AddPropertyField(posZ, leftArrowZ, rightArrowZ, 2);

            GUI.Label(labelPos, label);

            GUI.Label(valuePosX, "X");
            GUI.Label(valuePosY, "Y");
            GUI.Label(valuePosZ, "Z");

            if (!isMovable)
            {
                GUI.Label(valuePosX, property.vector3Value.x.ToString(), rightAlign);
                GUI.Label(valuePosY, property.vector3Value.y.ToString(), rightAlign);
                GUI.Label(valuePosZ, property.vector3Value.z.ToString(), rightAlign);
            }
            else if (vectorValue == 0)
            {
                GUI.Label(valuePosX, (property.vector3Value.x + dragDistance).ToString(), rightAlign);
                GUI.Label(valuePosY, property.vector3Value.y.ToString(), rightAlign);
                GUI.Label(valuePosZ, property.vector3Value.z.ToString(), rightAlign);
            }
            else if (vectorValue == 1)
            {
                GUI.Label(valuePosY, (property.vector3Value.y + dragDistance).ToString(), rightAlign);
                GUI.Label(valuePosX, property.vector3Value.x.ToString(), rightAlign);
                GUI.Label(valuePosZ, property.vector3Value.z.ToString(), rightAlign);
            }
            else if (vectorValue == 2)
            {
                GUI.Label(valuePosZ, (property.vector3Value.z + dragDistance).ToString(), rightAlign);
                GUI.Label(valuePosY, property.vector3Value.y.ToString(), rightAlign);
                GUI.Label(valuePosX, property.vector3Value.x.ToString(), rightAlign);
            }

        }
        else if (vectorValue == 0)
        {
            Vector3 newVec = property.vector3Value;

            GUI.Label(labelPos, label);

            GUI.SetNextControlName("ValueChangeX" + label.text);
            newVec.x = EditorGUI.FloatField(posX, newVec.x);
            EditorGUI.FocusTextInControl("ValueChangeX" + label.text);

            AddPropertyField(posY, leftArrowY, rightArrowY, 1);
            AddPropertyField(posZ, leftArrowZ, rightArrowZ, 2);

            GUI.Label(valuePosY, "Y");
            GUI.Label(valuePosZ, "Z");

            GUI.Label(valuePosY, property.vector3Value.y.ToString(), rightAlign);
            GUI.Label(valuePosZ, property.vector3Value.z.ToString(), rightAlign);

            property.vector3Value = newVec;
            property.serializedObject.ApplyModifiedProperties();
        }
        else if (vectorValue == 1)
        {
            Vector3 newVec = property.vector3Value;

            GUI.Label(labelPos, label);

            GUI.SetNextControlName("ValueChangeY" + label.text);
            newVec.y = EditorGUI.FloatField(posY, newVec.y);
            EditorGUI.FocusTextInControl("ValueChangeY" + label.text);

            AddPropertyField(posX, leftArrowX, rightArrowX, 0);
            AddPropertyField(posZ, leftArrowZ, rightArrowZ, 2);

            GUI.Label(valuePosX, "X");
            GUI.Label(valuePosZ, "Z");

            GUI.Label(valuePosX, property.vector3Value.x.ToString(), rightAlign);
            GUI.Label(valuePosZ, property.vector3Value.z.ToString(), rightAlign);

            property.vector3Value = newVec;
            property.serializedObject.ApplyModifiedProperties();
        }
        else if (vectorValue == 2)
        {
            Vector3 newVec = property.vector3Value;

            GUI.Label(labelPos, label);

            GUI.SetNextControlName("ValueChangeZ" + label.text);
            newVec.z = EditorGUI.FloatField(posZ, newVec.z);
            EditorGUI.FocusTextInControl("ValueChangeZ" + label.text);

            AddPropertyField(posY, leftArrowY, rightArrowY, 1);
            AddPropertyField(posX, leftArrowX, rightArrowX, 0);

            GUI.Label(valuePosY, "Y");
            GUI.Label(valuePosX, "X");

            GUI.Label(valuePosY, property.vector3Value.y.ToString(), rightAlign);
            GUI.Label(valuePosX, property.vector3Value.x.ToString(), rightAlign);

            property.vector3Value = newVec;
            property.serializedObject.ApplyModifiedProperties();
        }

        rightAlign.alignment = TextAnchor.MiddleLeft;

        //input

        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (posX.Contains(Event.current.mousePosition))
            {
                vectorValue = 0;
                isMovable = true;
                mouseFirstPos = guiEvent.mousePosition;
            }
            else if (posY.Contains(Event.current.mousePosition))
            {
                vectorValue = 1;
                isMovable = true;
                mouseFirstPos = guiEvent.mousePosition;
            }
            else if (posZ.Contains(Event.current.mousePosition))
            {
                vectorValue = 2;
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
            if (posX.Contains(Event.current.mousePosition))
            {
                vectorArrowValue = 0;
                showArrows = true;
            }
            else if (posY.Contains(Event.current.mousePosition))
            {
                vectorArrowValue = 1;
                showArrows = true;
            }
            else if (posZ.Contains(Event.current.mousePosition))
            {
                vectorArrowValue = 2;
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
                dragDistance = (guiEvent.mousePosition.x - mouseFirstPos.x) / (10 * BNGNodeEditor.NodeEditorWindow.current.zoom);
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
                    Vector3 newVec = property.vector3Value;
                    if (vectorValue == 0)
                        newVec.x += dragDistance;
                    if (vectorValue == 1)
                        newVec.y += dragDistance;
                    if (vectorValue == 2)
                        newVec.z += dragDistance;
                    property.vector3Value = newVec;
                    property.serializedObject.ApplyModifiedProperties();
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
            Vector3 newVec = property.vector3Value;
            if (vectorValue == 0)
                newVec.x += dragDistance;
            if (vectorValue == 1)
                newVec.y += dragDistance;
            if (vectorValue == 2)
                newVec.z += dragDistance;
            property.vector3Value = newVec;
            property.serializedObject.ApplyModifiedProperties();
            dragDistance = 0;

        }
        if (buttonClicked)
        {
            Vector3 newVec = property.vector3Value;
            if (rightArrowX.Contains(Event.current.mousePosition))
            {
                newVec.x += 0.1f;
            }
            else if (leftArrowX.Contains(Event.current.mousePosition))
            {
                newVec.x -= 0.1f;
            }
            else if (rightArrowY.Contains(Event.current.mousePosition))
            {
                newVec.y += 0.1f;
            }
            else if (leftArrowY.Contains(Event.current.mousePosition))
            {
                newVec.y -= 0.1f;
            }
            else if (rightArrowZ.Contains(Event.current.mousePosition))
            {
                newVec.z += 0.1f;
            }
            else if (leftArrowZ.Contains(Event.current.mousePosition))
            {
                newVec.z -= 0.1f;
            }
            else
            {
                isEditingArea = true;
            }
            buttonClicked = false;
            property.vector3Value = newVec;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
