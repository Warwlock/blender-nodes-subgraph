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
    [CreateNodeMenu("Converter/SeparateRGBA")]
    public class SeparateRGBA : Node
    {
        public CustomBlenderColor vector4A = CustomBlenderColor.white;
        [Input] public string a = "";

        [Output] public string ResultR;
        [Output] public string ResultG;
        [Output] public string ResultB;
        [Output] public string ResultA;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", this.a).Split('?').Last();

            string a_first = GetInputValue<string>("a", "").Split('?').First();

            string ValueID_r = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_r";
            string ValueID_g = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_g";
            string ValueID_b = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_b";
            string ValueID_a = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_a";

            if (port.fieldName == "ResultR")
            {
                this.a = vector4A.r.ToString();
                return a_first +
                    "|float " + ValueID_r + " = " +
                    string.Format("separate_r({0})", a) + ";?" + ValueID_r;
            }
            else if (port.fieldName == "ResultG")
            {
                this.a = vector4A.g.ToString();
                return a_first +
                    "|float " + ValueID_g + " = " +
                    string.Format("separate_g({0})", a) + ";?" + ValueID_g;
            }
            else if (port.fieldName == "ResultB")
            {
                this.a = vector4A.b.ToString();
                return a_first +
                    "|float " + ValueID_b + " = " +
                    string.Format("separate_b({0})", a) + ";?" + ValueID_b;
            }
            else if (port.fieldName == "ResultA")
            {
                this.a = vector4A.a.ToString();
                return a_first +
                    "|float " + ValueID_a + " = " +
                    string.Format("separate_a({0})", a) + ";?" + ValueID_a;
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


    [CustomNodeEditor(typeof(SeparateRGBA))]
    public class SeparateRGBAEditor : NodeEditor
    {
        SeparateRGBA serializedNode;
        public override void OnBodyGUI()
        {
            string propertyDescription = "Input value used for unconnected sockets.";
            if (serializedNode == null) serializedNode = target as SeparateRGBA;
            serializedObject.Update();
            serializedNode.GetInputPort("a").connectionType = Node.ConnectionType.Override;
            if (serializedNode.GetPort("a").IsConnected)
                propertyDescription = "";
            serializedNode.GetPort("ResultR").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultR"), new GUIContent("R", ""));
            serializedNode.GetPort("ResultG").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultG"), new GUIContent("G", ""));
            serializedNode.GetPort("ResultB").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultB"), new GUIContent("B", ""));
            serializedNode.GetPort("ResultA").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultA"), new GUIContent("A", ""));
            serializedNode.GetPort("a").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("vector4A"), new GUIContent("Color", propertyDescription), serializedNode.GetInputPort("a"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}