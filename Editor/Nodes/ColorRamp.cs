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
    [CreateNodeMenu("Converter/Color Ramp")]
    public class ColorRamp : Node
    {
        public CustomBlenderGradient gradientInColor = new CustomBlenderGradient();
        public string sGradientIn = "";

        [BlenderRange(0f, 1f)] public float floatFac = 0.5f;
        [Input] public string sFloatFac = "";

        [Output] public string Result;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sGradientIn = GetInputValue<string>("sGradientIn", this.sGradientIn).Split('?').Last();
            string sFloatFac = GetInputValue<string>("sFloatFac", floatFac.ToString()).Split('?').Last();

            string sGradientIn_f = GetInputValue<string>("sGradientIn", "").Split('?').First();
            string sFloatFac_f = GetInputValue<string>("sFloatFac", "").Split('?').First();

            this.sGradientIn = string.Format("gradient_" + Mathf.Abs(GetInstanceID()).ToString());

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                return sGradientIn_f + sFloatFac_f +
                    "|float4 " + ValueID + " = " +
                    string.Format("color_ramp({0}, {1})", sGradientIn, sFloatFac) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(ColorRamp))]
    public class ColorRampEditor : NodeEditor
    {
        ColorRamp serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as ColorRamp;
            serializedObject.Update();

            serializedNode.GetOutputPort("Result").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Color", ""));
            GUILayout.Space(10);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("gradientInColor"));

            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("gradientInColor"), new GUIContent("Gradient Color", "Color selector for gradient map."));
            SetPortBehaviour("floatFac", "sFloatFac", "Fac");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer)
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as ColorRamp;
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