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
    [CreateNodeMenu("Converter/Clamp")]
    public class Clamp : Node
    {
        [BlenderSingle] public float value = 1;
        [BlenderSingle] public float min = 0;
        [BlenderSingle] public float max = 1;

        [Input] public string sValue;
        [Input] public string sMin;
        [Input] public string sMax;

        [Output] public string Result;

        [NodeEnum]
        public ClampType clampType = ClampType.MinMax;
        public enum ClampType { MinMax, Range }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sValue = GetInputValue<string>("sValue", value.ToString()).Split('?').Last();
            string sMin = GetInputValue<string>("sMin", min.ToString()).Split('?').Last();
            string sMax = GetInputValue<string>("sMax", max.ToString()).Split('?').Last();

            string sValue_f = GetInputValue<string>("sValue", "").Split('?').First();
            string sMin_f = GetInputValue<string>("sMin", "").Split('?').First();
            string sMax_f = GetInputValue<string>("sMax", "").Split('?').First();

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                if (clampType == ClampType.MinMax)
                    return sValue_f + sMin_f + sMax_f +
                        "|float " + ValueID + " = " +
                        string.Format("clamp_minmax({0}, {1}, {2})", sValue, sMin, sMax) + ";?" + ValueID;
                else
                    return sValue_f + sMin_f + sMax_f +
                        "|float " + ValueID + " = " +
                        string.Format("clamp_range({0}, {1}, {2})", sValue, sMin, sMax) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(Clamp))]
    public class ClampEditor : NodeEditor
    {
        Clamp serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as Clamp;
            serializedObject.Update();
            NodePort myPort = serializedNode.GetPort("Result");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Result", ""));
            GUILayout.Space(10);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("clampType"), new GUIContent("", ""));
            SetPortBehaviour("value", "sValue", "Value");
            SetPortBehaviour("min", "sMin", "Min");
            SetPortBehaviour("max", "sMax", "Max");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer)
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as Clamp;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = serializedObject.FindProperty(propertyNamer).type;
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
        }
    }
}