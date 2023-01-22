// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;
using System.IO;

public class LineRendererCurveEditor
{
    /*private class Styles
    {
        public static GUIContent widthMultiplier = EditorGUIUtility.TrTextContent("Width", "The multiplier applied to the curve, describing the width (in world space) along the line.");
    }

    private bool m_Refresh = false;
    private CurveEditor m_Editor = null;
    private CurveEditorSettings m_Settings = new CurveEditorSettings();

    private SerializedProperty m_WidthMultiplier;
    private SerializedProperty m_WidthCurve;

    public void OnEnable(SerializedObject serializedObject)
    {
        m_WidthMultiplier = serializedObject.FindProperty("m_Parameters.widthMultiplier");
        m_WidthCurve = serializedObject.FindProperty("m_Parameters.widthCurve");

        m_Settings.hRangeMin = 0.0f;
        m_Settings.vRangeMin = 0.0f;
        m_Settings.vRangeMax = 1.0f;
        m_Settings.hRangeMax = 1.0f;
        m_Settings.vSlider = false;
        m_Settings.hSlider = false;
        m_Settings.rectangleToolFlags = CurveEditorSettings.RectangleToolFlags.MiniRectangleTool;
        m_Settings.allowDeleteLastKeyInCurve = false;

        TickStyle hTS = new TickStyle();
        hTS.tickColor.color = new Color(0.0f, 0.0f, 0.0f, 0.15f);
        hTS.distLabel = 30;
        hTS.stubs = false;
        m_Settings.hTickStyle = hTS;
        TickStyle vTS = new TickStyle();
        vTS.tickColor.color = new Color(0.0f, 0.0f, 0.0f, 0.15f);
        vTS.distLabel = 20;
        vTS.stubs = false;
        m_Settings.vTickStyle = vTS;

        m_Settings.undoRedoSelection = true;

        m_Editor = new CurveEditor(new Rect(0, 0, 1000, 100), new CurveWrapper[0], false);
        m_Editor.settings = m_Settings;
        m_Editor.margin = 25;
        m_Editor.leftmargin = 35;
        m_Editor.SetShownHRangeInsideMargins(0.0f, 1.0f);
        m_Editor.SetShownVRangeInsideMargins(0.0f, 1.0f);
        m_Editor.ignoreScrollWheelUntilClicked = true;

        Undo.undoRedoPerformed += UndoRedoPerformed;
    }

    public void OnDisable()
    {
        m_Editor.OnDisable();
        Undo.undoRedoPerformed -= UndoRedoPerformed;
    }

    private CurveWrapper GetCurveWrapper(AnimationCurve curve)
    {
        float colorMultiplier = !EditorGUIUtility.isProSkin ? 0.9f : 1.0f;
        Color colorMult = new Color(colorMultiplier, colorMultiplier, colorMultiplier, 1);

        CurveWrapper wrapper = new CurveWrapper();
        wrapper.id = 0;
        wrapper.groupId = -1;
        wrapper.color = new Color(1.0f, 0.0f, 0.0f, 1.0f) * colorMult;
        wrapper.hidden = false;
        wrapper.readOnly = false;
        wrapper.renderer = new NormalCurveRenderer(curve);
        wrapper.renderer.SetCustomRange(0.0f, 1.0f);
        wrapper.getAxisUiScalarsCallback = GetAxisScalars;
        wrapper.setAxisUiScalarsCallback = SetAxisScalars;
        return wrapper;
    }

    public Vector2 GetAxisScalars()
    {
        return new Vector2(1.0f, m_WidthMultiplier.floatValue);
    }

    public void SetAxisScalars(Vector2 scalars)
    {
        m_WidthMultiplier.floatValue = scalars.y;
        m_Refresh = true;
    }

    private void UndoRedoPerformed()
    {
        m_Refresh = true;
    }

    public void CheckCurveChangedExternally()
    {
        if (Event.current.type != EventType.Repaint)
            return;

        CurveWrapper cw = m_Editor.GetCurveWrapperFromID(0);
        if (m_WidthCurve != null)
        {
            AnimationCurve propCurve = m_WidthCurve.animationCurveValue;
            if ((cw == null) != m_WidthCurve.hasMultipleDifferentValues)
            {
                m_Refresh = true;
            }
            else if (cw != null)
            {
                if (cw.curve.length == 0 || cw.curve.length != propCurve.length)
                {
                    m_Refresh = true;
                }
                else
                {
                    var curveKeys = cw.curve.keys;
                    var propKeys = propCurve.keys;

                    for (int i = 0; i < curveKeys.Length; i++)
                    {
                        if (!propKeys[i].Equals(curveKeys[i]))
                        {
                            m_Refresh = true;
                            break;
                        }
                    }
                }
            }
        }
        else if (cw != null)
        {
            m_Refresh = true;
        }
    }

    public void OnInspectorGUI()
    {
        Rect r = GUILayoutUtility.GetAspectRect(2, GUI.skin.textField);
        r.xMin += EditorGUI.indent;
        if (Event.current.type != EventType.Layout && Event.current.type != EventType.Used)
        {
            m_Editor.rect = new Rect(r.x, r.y, r.width, r.height);
        }

        // Draw Curve Editor
        if (m_Refresh)
        {
            m_Editor.animationCurves = new CurveWrapper[] { GetCurveWrapper(m_WidthCurve.animationCurveValue) };
            m_Refresh = false;
        }

        GUI.Label(m_Editor.drawRect, GUIContent.none, "TextField");

        m_Editor.hRangeLocked = Event.current.shift;
        m_Editor.vRangeLocked = EditorGUI.actionKey;

        m_Editor.OnGUI();

        var labelRect = new Rect(r.x + 4, r.y, r.width, EditorGUI.kSingleLineHeight);
        EditorGUI.LabelField(labelRect, Styles.widthMultiplier);

        // Apply curve changes
        if ((m_Editor.GetCurveWrapperFromID(0) != null) && (m_Editor.GetCurveWrapperFromID(0).changed))
        {
            AnimationCurve changedCurve = m_Editor.GetCurveWrapperFromID(0).curve;

            // Never save a curve with no keys
            if (changedCurve.length > 0)
            {
                m_WidthCurve.animationCurveValue = changedCurve;
                m_Editor.GetCurveWrapperFromID(0).changed = false;
            }
        }
    }*/
}