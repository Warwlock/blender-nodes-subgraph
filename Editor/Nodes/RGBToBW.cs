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
    [CreateNodeMenu("Converter/RGB To BW")]
    public class RGBToBW : Node
    {
        public CustomBlenderColor color = CustomBlenderColor.white;

        [Input] public string sColor = "";

        [Output] public string Result;

        public override object GetValue(NodePort port)
        {
            string sColor = GetInputValue<string>("sColor", this.sColor).Split('?').Last();

            string sColor_first = GetInputValue<string>("sColor", "").Split('?').First();

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            this.sColor = string.Format("float4({0}, {1}, {2}, {3})", color.r, color.g, color.b, color.a);

            if (port.fieldName == "Result")
            {
                return sColor_first + 
                    "|float " + ValueID + " = " +
                    "rgbtobw(" + sColor + ")" + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(RGBToBW))]
    public class RGBToBWEditor : NodeEditor
    {
        RGBToBW serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as RGBToBW;
            serializedObject.Update();
            serializedNode.GetOutputPort("Result").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Result", ""));
            GUILayout.Space(10);
            SetPortBehaviour("color", "sColor", "Color", "vector4");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as RGBToBW;
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