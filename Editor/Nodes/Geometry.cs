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
    [NodeTint("#9C333E")]
    [CreateNodeMenu("Input/Geometry")]
    public class Geometry : Node
    {
        [Output] public string oPosition;
        [Output] public string oNormal;
        [Output] public string oIncoming;
        [Output] public string oBackFacing;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "oPosition")
            {
                return "?float4(_PWS, 0)";
            }
            else if (port.fieldName == "oNormal")
            {
                return "?float4(_NWS, 0)";
            }
            else if (port.fieldName == "oIncoming")
            {
                return "?float4(_VWS, 0)";
            }
            else if (port.fieldName == "oBackFacing")
            {
                return "?GBackFacing(_POS, _NOS)";
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


    [CustomNodeEditor(typeof(Geometry))]
    public class GeometryEditor : NodeEditor
    {
        Geometry serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as Geometry;
            serializedObject.Update();
            NodePort myPort = serializedNode.GetPort("oPosition");
            myPort.nodePortType = "vector3";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oPosition"), new GUIContent("Position", ""));
            myPort = serializedNode.GetPort("oNormal");
            myPort.nodePortType = "vector3";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oNormal"), new GUIContent("Normal", ""));
            myPort = serializedNode.GetPort("oIncoming");
            myPort.nodePortType = "vector3";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oIncoming"), new GUIContent("Incoming", ""));
            myPort = serializedNode.GetPort("oBackFacing");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oBackFacing"), new GUIContent("Backfacing", ""));
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as Geometry;
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