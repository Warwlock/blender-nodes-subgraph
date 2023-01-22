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
    [CreateNodeMenu("Color/Bright Contrast")]
    public class BrightContrast : Node
    {
        public CustomBlenderColor colorA = CustomBlenderColor.white;
        [BlenderSingle] public float floatB = 0f;
        [BlenderSingle] public float floatC = 0f;
        [Input] public string a = "";
        [Input] public string b = "";
        [Input] public string c = "";

        [Output] public string Result;

        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", this.a).Split('?').Last();
            string b = GetInputValue<string>("b", this.b).Split('?').Last();
            string c = GetInputValue<string>("c", this.c).Split('?').Last();

            string a_f = GetInputValue<string>("a", "").Split('?').First();
            string b_f = GetInputValue<string>("b", "").Split('?').First();
            string c_f = GetInputValue<string>("c", "").Split('?').First();

            this.a = string.Format("float4({0}, {1}, {2}, {3})", colorA.r, colorA.g, colorA.b, colorA.a);
            this.b = floatB.ToString();
            this.c = floatC.ToString();

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                return a_f + b_f + c_f + 
                    "|float4 " + ValueID + " = " +
                    string.Format("brightness_contrast({0}, {1}, {2})", a, b, c) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(BrightContrast))]
    public class BrightContrastEditor : NodeEditor
    {
        BrightContrast serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as BrightContrast;
            serializedObject.Update();
            NodePort myPort = serializedNode.GetPort("Result");
            myPort.nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Color", ""), myPort);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("colorA"), new GUIContent("Color", SetPortBehaviour("a", "vector4")), serializedNode.GetInputPort("a"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("floatB"), new GUIContent("Bright", SetPortBehaviour("b")), serializedNode.GetInputPort("b"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("floatC"), new GUIContent("Contrast", SetPortBehaviour("c")), serializedNode.GetInputPort("c"));
            serializedObject.ApplyModifiedProperties();
        }

        string SetPortBehaviour(string portNamer, string portType = "float")
        {
            if (serializedNode == null) serializedNode = target as BrightContrast;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = portType;
            if (serializedNode.GetPort(portNamer).IsConnected)
                return "";
            else
                return "Input value used for unconnected sockets.";
        }
    }
}