using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;

namespace MaterialNodesGraph
{
    [NodeTint("#9C333E")]
    [CreateNodeMenu("Input/Value")]
    public class ValueInput : Node
    {

        [BlenderSingle] public float floatValue = 0.5f;
        public string a = "";

        [Output] public string Result;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", this.a);
            this.a = floatValue.ToString();
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

    [CustomNodeEditor(typeof(ValueInput))]
    public class ValueInputEditor : NodeEditor
    {
        ValueInput serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as ValueInput;
            serializedObject.Update();
            NodePort myPort = serializedNode.GetPort("Result");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Value", ""));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("floatValue"), new GUIContent("Value"));
            serializedObject.ApplyModifiedProperties();
        }

        string SetPortBehaviour(string portNamer)
        {
            if (serializedNode == null) serializedNode = target as ValueInput;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            if (serializedNode.GetPort(portNamer).IsConnected)
                return "";
            else
                return "Input value used for unconnected sockets.";
        }
    }
}