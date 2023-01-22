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
    [NodeTint("#8E8131")]
    [CreateNodeMenu("Color/Hue Saturation Value")]
    public class HueSaturationValue : Node
    {
        public CustomBlenderColor colorIn = CustomBlenderColor.white;
        [BlenderRange(0f, 1f, false)] public float floatHue = 0.5f;
        [BlenderRange(0f, 2f, false)] public float floatSat = 1f;
        [BlenderRange(0f, 2f, false)] public float floatVal = 1f;
        [BlenderRange(0f, 1f)] public float floatFac = 1f;

        [Input] public string sColorIn;
        [Input] public string sFloatHue;
        [Input] public string sFloatSat;
        [Input] public string sFloatVal;
        [Input] public string sFloatFac;

        [Output] public string Result;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sColorIn = GetInputValue<string>("sColorIn", this.sColorIn).Split('?').Last();
            string sFloatHue = GetInputValue<string>("sFloatHue", floatHue.ToString()).Split('?').Last();
            string sFloatSat = GetInputValue<string>("sFloatSat", floatSat.ToString()).Split('?').Last();
            string sFloatVal = GetInputValue<string>("sFloatVal", floatVal.ToString()).Split('?').Last();
            string sFloatFac = GetInputValue<string>("sFloatFac", floatFac.ToString()).Split('?').Last();

            string sColorIn_f = GetInputValue<string>("sColorIn", "").Split('?').First();
            string sFloatHue_f = GetInputValue<string>("sFloatHue", "").Split('?').First();
            string sFloatSat_f = GetInputValue<string>("sFloatSat", "").Split('?').First();
            string sFloatVal_f = GetInputValue<string>("sFloatVal", "").Split('?').First();
            string sFloatFac_f = GetInputValue<string>("sFloatFac", "").Split('?').First();

            this.sColorIn = string.Format("float4({0}, {1}, {2}, {3})", colorIn.r, colorIn.g, colorIn.b, colorIn.a);

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                return sColorIn_f + sFloatHue_f + sFloatSat_f + sFloatVal_f + sFloatFac_f +
                    "|float4 " + ValueID + " = " +
                    string.Format("hue_sat({0}, {1}, {2}, {3}, {4})", 
                    sFloatHue, sFloatSat, sFloatVal, sFloatFac, sColorIn) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(HueSaturationValue))]
    public class HueSaturationValueEditor : NodeEditor
    {
        HueSaturationValue serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as HueSaturationValue;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Color", ""));
            SetPortBehaviour("floatHue", "sFloatHue", "Hue");
            SetPortBehaviour("floatSat", "sFloatSat", "Saturation");
            SetPortBehaviour("floatVal", "sFloatVal", "Value");
            SetPortBehaviour("floatFac", "sFloatFac", "Fac");
            SetPortBehaviour("colorIn", "sColorIn", "Color", "vector4");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as HueSaturationValue;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = portType;
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
        }
    }
}