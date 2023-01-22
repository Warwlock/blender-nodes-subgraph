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
    [CreateNodeMenu("Converter/Separate HSV")]
    public class SeparateHSV : Node
    {
        public CustomBlenderColor vector3A = CustomBlenderColor.white;
        [Input] public string a = "";

        [Output] public string ResultH;
        [Output] public string ResultS;
        [Output] public string ResultV;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", this.a).Split('?').Last();

            string a_first = GetInputValue<string>("a", "").Split('?').First();

            this.a = string.Format("float4({0}, {1}, {2}, {3})", vector3A.r, vector3A.g, vector3A.b, vector3A.a);

            string ValueID_h = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_h";
            string ValueID_s = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_s";
            string ValueID_v = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_v";

            if (port.fieldName == "ResultH")
            {
                return a_first +
                    "|float " + ValueID_h + " = " +
                    string.Format("separate_h({0})", a) + ";?" + ValueID_h;
            }
            else if (port.fieldName == "ResultS")
            {
                return a_first +
                    "|float " + ValueID_s + " = " +
                    string.Format("separate_s({0})", a) + ";?" + ValueID_s;
            }
            else if (port.fieldName == "ResultV")
            {
                return a_first +
                    "|float " + ValueID_v + " = " +
                    string.Format("separate_v({0})", a) + ";?" + ValueID_v;
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


    [CustomNodeEditor(typeof(SeparateHSV))]
    public class SeparateHSVEditor : NodeEditor
    {
        SeparateHSV serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as SeparateHSV;
            serializedObject.Update();
            serializedNode.GetInputPort("a").connectionType = Node.ConnectionType.Override;
            serializedNode.GetPort("ResultH").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultH"), new GUIContent("H", ""));
            serializedNode.GetPort("ResultS").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultS"), new GUIContent("S", ""));
            serializedNode.GetPort("ResultV").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultV"), new GUIContent("V", ""));
            serializedNode.GetPort("a").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("vector3A"), new GUIContent("Color", "Input value used for unconnected sockets."), serializedNode.GetInputPort("a"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}