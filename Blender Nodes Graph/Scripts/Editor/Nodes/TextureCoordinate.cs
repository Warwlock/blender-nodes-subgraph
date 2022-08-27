using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;

[NodeTint("#9C333E")]
[CreateNodeMenu("Input/Texture Coordinate")]
public class TextureCoordinate : Node 
{
    [Output] public string oNormal;
    [Output] public string oUV;
    [Output] public string oObject;
    [Output] public string oCamera;
    [Output] public string oWindow;

	public override object GetValue(NodePort port)
    {
        if (port.fieldName == "oNormal")
        {
            return "?float4(_NOS, 0)";
        }
        else if (port.fieldName == "oUV")
        {
            return "?float4(_UV, 0)";
        }
        else if (port.fieldName == "oObject")
        {
            return "?float4(_POS, 0)";
        }
        else if (port.fieldName == "oCamera")
        {
            return "?float4(_VVS, 0)";
        }
        else if (port.fieldName == "oWindow")
        {
            return "?float4(_SP, 0)";
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


[CustomNodeEditor(typeof(TextureCoordinate))]
public class TextureCoordinateEditor : NodeEditor
{
    TextureCoordinate serializedNode;
    public override void OnBodyGUI()
    {
        if (serializedNode == null) serializedNode = target as TextureCoordinate;
        serializedObject.Update();
        NodePort myPort = serializedNode.GetPort("oNormal");
        myPort.nodePortType = "vector3";
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oNormal"), new GUIContent("Normal", ""));
        myPort = serializedNode.GetPort("oUV");
        myPort.nodePortType = "vector3";
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oUV"), new GUIContent("UV", ""));
        myPort = serializedNode.GetPort("oObject");
        myPort.nodePortType = "vector3";
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oObject"), new GUIContent("Object", ""));
        myPort = serializedNode.GetPort("oCamera");
        myPort.nodePortType = "vector3";
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oCamera"), new GUIContent("Camera", ""));
        myPort = serializedNode.GetPort("oWindow");
        myPort.nodePortType = "vector3";
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("oWindow"), new GUIContent("Window", ""));
        serializedObject.ApplyModifiedProperties();
    }

    void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer)
    {
        string fieldName;
        if (serializedNode == null) serializedNode = target as TextureCoordinate;
        serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
        if (serializedNode.GetPort(portNamer).IsConnected)
            fieldName = "";
        else
            fieldName = "Input value used for unconnected sockets.";
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
    }
 }