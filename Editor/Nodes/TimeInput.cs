using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;

namespace MaterialNodesGraph
{
    [NodeTint("#9C333E")]
    [CreateNodeMenu("Input/Time")]
    public class TimeInput : Node
    {

        [Output] public string oTime;
        [Output] public string oSinTime;
        [Output] public string oCosTime;
        [Output] public string oDeltaTime;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "oTime")
            {
                return "?_Time";
            }
            else if (port.fieldName == "oSinTime")
            {
                return "?_SinTime";
            }
            else if (port.fieldName == "oCosTime")
            {
                return "?_CosTime";
            }
            else if (port.fieldName == "oDeltaTime")
            {
                return "?unity_DeltaTime";
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


    [CustomNodeEditor(typeof(TimeInput))]
    public class NodeTimeEditor : NodeEditor
    {
        TimeInput serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as TimeInput;
            serializedObject.Update();
            NodePort myPort = serializedNode.GetPort("oTime");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oTime"), new GUIContent("Time", ""), myPort);
            myPort = serializedNode.GetPort("oSinTime");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oSinTime"), new GUIContent("Sine Time", ""), myPort);
            myPort = serializedNode.GetPort("oCosTime");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oCosTime"), new GUIContent("Cosine Time", ""), myPort);
            myPort = serializedNode.GetPort("oDeltaTime");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oDeltaTime"), new GUIContent("Delta Time", ""), myPort);
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer)
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as TimeInput;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
        }
    }
}