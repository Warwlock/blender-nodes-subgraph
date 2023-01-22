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
    [CreateNodeMenu("Converter/Blackbody")]
    public class Blackbody : Node
    {
        [BlenderSingle] public float floatA = 1000f;
        [Input] public string a = "";

        [Output] public string Result;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", this.a).Split('?').Last();

            string a_first = GetInputValue<string>("a", "").Split('?').First();

            this.a = floatA.ToString();

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                return a_first +
                    "|float4 " + ValueID + " = " +
                    "node_blackbody(" + a + ")" + ";?" + ValueID;
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

        string portConnectedSplit(string portName, string value)
        {
            if (GetPort(portName).IsConnected)
            {
                return value.Split('|').First();
            }
            else
                return "";
        }
    }


    [CustomNodeEditor(typeof(Blackbody))]
    public class BlackbodyEditor : NodeEditor
    {
        Blackbody serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as Blackbody;
            serializedObject.Update();
            NodePort myPort = serializedNode.GetPort("Result");
            myPort.nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Color", ""), myPort);
            myPort = serializedNode.GetInputPort("a");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("floatA"), new GUIContent("Temperature", SetPortBehaviour("a")), myPort);
            serializedObject.ApplyModifiedProperties();
        }

        string SetPortBehaviour(string portNamer)
        {
            if (serializedNode == null) serializedNode = target as Blackbody;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            if (serializedNode.GetPort(portNamer).IsConnected)
                return "";
            else
                return "Input value used for unconnected sockets.";
        }
    }
}