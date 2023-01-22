using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;

namespace MaterialNodesGraph
{
    [NodeTint("#9C333E")]
    [CreateNodeMenu("Input/RGBA Input")]
    public class RGBAInput : Node
    {
        public CustomBlenderRGBA rgbaValue = new CustomBlenderRGBA(CustomBlenderColor.gray);
        public string a = "";

        [Output] public string Result;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", this.a);
            this.a = string.Format("float4({0}, {1}, {2}, {3})", rgbaValue.r, rgbaValue.g, rgbaValue.b, rgbaValue.a);
            if (port.fieldName == "Result")
            {
                return "?" + a;
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

    [CustomNodeEditor(typeof(RGBAInput))]
    public class RGBAInputEditor : NodeEditor
    {
        RGBAInput serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as RGBAInput;
            serializedObject.Update();
            NodePort myPort = serializedNode.GetPort("Result");
            myPort.nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Value", ""), myPort);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("rgbaValue"), new GUIContent("Color"));
            serializedObject.ApplyModifiedProperties();
        }

        string SetPortBehaviour(string portNamer)
        {
            if (serializedNode == null) serializedNode = target as RGBAInput;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = "Vector4";
            if (serializedNode.GetPort(portNamer).IsConnected)
                return "";
            else
                return "Input value used for unconnected sockets.";
        }
    }
}