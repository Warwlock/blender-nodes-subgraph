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
    [CreateNodeMenu("Converter/Combine XYZ")]
    public class CombineXYZ : Node
    {
        [BlenderSingle] public float inA = 0f;
        [BlenderSingle] public float inB = 0f;
        [BlenderSingle] public float inC = 0f;
        [Input] public string a = "";
        [Input] public string b = "";
        [Input] public string c = "";

        [Output] public string Result;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", inA.ToString()).Split('?').Last();
            string b = GetInputValue<string>("b", inB.ToString()).Split('?').Last();
            string c = GetInputValue<string>("c", inC.ToString()).Split('?').Last();

            string a_f = GetInputValue<string>("a", "").Split('?').First();
            string b_f = GetInputValue<string>("b", "").Split('?').First();
            string c_f = GetInputValue<string>("c", "").Split('?').First();

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                return a_f + b_f + c_f +
                    "|float4 " + ValueID + " = " +
                    "float4(" + string.Format("combine_xyz({0}, {1}, {2})", a, b, c) + ", 0);?" + ValueID;
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


    [CustomNodeEditor(typeof(CombineXYZ))]
    public class CombineXYZEditor : NodeEditor
    {
        CombineXYZ serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as CombineXYZ;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "vector3";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Vector", ""));
            GUILayout.Space(10);
            SetPortBehaviour("inA", "a", "X");
            SetPortBehaviour("inB", "b", "Y");
            SetPortBehaviour("inC", "c", "Z");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer)
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as CombineXYZ;
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