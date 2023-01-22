using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;
using System.Linq;
using System.Text.RegularExpressions;

namespace MaterialNodesGraph
{
    [NodeTint("#4885AB")]
    [CreateNodeMenu("Converter/MapRange")]
    public class MapRange : Node
    {
        public bool clamp = true;
        [BlenderSingle] public float value = 1;
        [BlenderSingle] public float fromMin = 0;
        [BlenderSingle] public float fromMax = 1;
        [BlenderSingle] public float toMin = 0;
        [BlenderSingle] public float toMax = 1;
        [BlenderSingle] public float steps = 4;

        [Input] public string sValue;
        [Input] public string sFromMin;
        [Input] public string sFromMax;
        [Input] public string sToMin;
        [Input] public string sToMax;
        [Input] public string sSteps;

        [Output] public string Result;

        [NodeEnum]
        public mapRangeType mmType = mapRangeType.Linear;
        public enum mapRangeType { Linear, SteppedLinear, SmoothStep, SmootherStep }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sValue = GetInputValue<string>("sValue", value.ToString()).Split('?').Last();
            string sFromMin = GetInputValue<string>("sFromMin", fromMin.ToString()).Split('?').Last();
            string sFromMax = GetInputValue<string>("sFromMax", fromMax.ToString()).Split('?').Last();
            string sToMin = GetInputValue<string>("sToMin", toMin.ToString()).Split('?').Last();
            string sToMax = GetInputValue<string>("sToMax", toMax.ToString()).Split('?').Last();
            string sSteps = GetInputValue<string>("sSteps", steps.ToString()).Split('?').Last();

            string sValue_f = GetInputValue<string>("sValue", "").Split('?').First();
            string sFromMin_f = GetInputValue<string>("sFromMin", "").Split('?').First();
            string sFromMax_f = GetInputValue<string>("sFromMax", "").Split('?').First();
            string sToMin_f = GetInputValue<string>("sToMin", "").Split('?').First();
            string sToMax_f = GetInputValue<string>("sToMax", "").Split('?').First();
            string sSteps_f = GetInputValue<string>("sSteps", "").Split('?').First();

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                if (mmType == mapRangeType.Linear)
                {
                    if (!clamp)
                        return sValue_f + sFromMin_f + sFromMax_f + sToMin_f + sToMax_f + sSteps_f +
                            "|float " + ValueID + " = " +
                            string.Format("map_range_linear({0}, {1}, {2}, {3}, {4}, {5})", 
                            sValue, sFromMin, sFromMax, sToMin, sToMax, sSteps) + ";?" + ValueID;
                    else
                        return sValue_f + sFromMin_f + sFromMax_f + sToMin_f + sToMax_f + sSteps_f +
                            "|float " + ValueID + " = " +
                            "clamp_value(" + string.Format("map_range_linear({0}, {1}, {2}, {3}, {4}, {5})", 
                            sValue, sFromMin, sFromMax, sToMin, sToMax, sSteps) + ", " + sToMin + ", " + sToMax + ")" + ";?" + ValueID;
                }
                else if (mmType == mapRangeType.SteppedLinear)
                {
                    if (!clamp)
                        return sValue_f + sFromMin_f + sFromMax_f + sToMin_f + sToMax_f + sSteps_f +
                            "|float " + ValueID + " = " +
                            string.Format("map_range_stepped({0}, {1}, {2}, {3}, {4}, {5})", 
                            sValue, sFromMin, sFromMax, sToMin, sToMax, sSteps) + ";?" + ValueID;
                    else
                        return sValue_f + sFromMin_f + sFromMax_f + sToMin_f + sToMax_f + sSteps_f +
                            "|float " + ValueID + " = " +
                            "clamp_value(" + string.Format("map_range_stepped({0}, {1}, {2}, {3}, {4}, {5})", 
                            sValue, sFromMin, sFromMax, sToMin, sToMax, sSteps) + ", " + sToMin + ", " + sToMax + ")" + ";?" + ValueID;
                }
                else if (mmType == mapRangeType.SmoothStep)
                    return sValue_f + sFromMin_f + sFromMax_f + sToMin_f + sToMax_f + sSteps_f +
                        "|float " + ValueID + " = " +
                        "clamp_value(" + string.Format("map_range_smoothstep({0}, {1}, {2}, {3}, {4}, {5})", 
                        sValue, sFromMin, sFromMax, sToMin, sToMax, sSteps) + ", " + sToMin + ", " + sToMax + ")" + ";?" + ValueID;
                else
                    return sValue_f + sFromMin_f + sFromMax_f + sToMin_f + sToMax_f + sSteps_f +
                        "|float " + ValueID + " = " +
                        "clamp_value(" + string.Format("map_range_smootherstep({0}, {1}, {2}, {3}, {4}, {5})", 
                        sValue, sFromMin, sFromMax, sToMin, sToMax, sSteps) + ", " + sToMin + ", " + sToMax + ")" + ";?" + ValueID;
            }
            else
                return 0f;
        }


        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            if (from.node == to.node)
            {
                from.Disconnect(to);
            }
        }
    }


    [CustomNodeEditor(typeof(MapRange))]
    public class MapRangeEditor : NodeEditor
    {
        MapRange serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as MapRange;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Result", ""));
            GUILayout.Space(10);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("mmType"), new GUIContent("", ""));
            if (serializedNode.mmType == MapRange.mapRangeType.Linear || serializedNode.mmType == MapRange.mapRangeType.SteppedLinear)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("clamp"), new GUIContent("Clamp", "Limits the output to the range (To Min, To Max)."), null);
            SetPortBehaviour("value", "sValue", "Value");
            SetPortBehaviour("fromMin", "sFromMin", "From Min");
            SetPortBehaviour("fromMax", "sFromMax", "From Max");
            SetPortBehaviour("toMin", "sToMin", "To Min");
            SetPortBehaviour("toMax", "sToMax", "To Max");
            if (serializedNode.mmType == MapRange.mapRangeType.SteppedLinear)
                SetPortBehaviour("steps", "sSteps", "Steps");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer)
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as MapRange;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = "float";
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
        }
    }
}