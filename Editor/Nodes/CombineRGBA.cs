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
    [CreateNodeMenu("Converter/Combine RGBA")]
    public class CombineRGBA : Node
    {
        [BlenderSingle] public float inA = 0f;
        [BlenderSingle] public float inB = 0f;
        [BlenderSingle] public float inC = 0f;
        [BlenderSingle] public float inD = 0f;
        [Input] public string a = "";
        [Input] public string b = "";
        [Input] public string c = "";
        [Input] public string d = "";

        [Output] public string Result;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", inA.ToString()).Split('?').Last();
            string b = GetInputValue<string>("b", inB.ToString()).Split('?').Last();
            string c = GetInputValue<string>("c", inC.ToString()).Split('?').Last();
            string d = GetInputValue<string>("d", inD.ToString()).Split('?').Last();

            string a_f = GetInputValue<string>("a", "").Split('?').First();
            string b_f = GetInputValue<string>("b", "").Split('?').First();
            string c_f = GetInputValue<string>("c", "").Split('?').First();
            string d_f = GetInputValue<string>("d", "").Split('?').First();

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                return a_f + b_f + c_f + d_f +
                    "|float4 " + ValueID + " = " +
                    string.Format("combine_rgba({0}, {1}, {2}, {3})", a, b, c, d) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(CombineRGBA))]
    public class CombineRGBAEditor : NodeEditor
    {
        CombineRGBA serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as CombineRGBA;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Color", ""));
            GUILayout.Space(10);
            SetPortBehaviour("inA", "a", "R");
            SetPortBehaviour("inB", "b", "G");
            SetPortBehaviour("inC", "c", "B");
            SetPortBehaviour("inD", "d", "A");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer)
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as CombineRGBA;
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