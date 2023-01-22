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
    [CreateNodeMenu("Color/Gamma")]
    public class Gamma : Node
    {
        public CustomBlenderColor color = CustomBlenderColor.white;
        [BlenderSingle] public float gamma = 1;

        [Input] public string sColor = "";
        [Input] public string sGamma;

        [Output] public string Result;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sColor = GetInputValue<string>("sColor", this.sColor).Split('?').Last();
            string sGamma = GetInputValue<string>("sGamma", gamma.ToString()).Split('?').Last();

            string sColor_f = GetInputValue<string>("sColor", "").Split('?').First();
            string sGamma_f = GetInputValue<string>("sGamma", "").Split('?').First();

            this.sColor = string.Format("float4({0}, {1}, {2}, {3})", color.r, color.g, color.b, color.a);

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                return sColor_f + sGamma_f +
                    "|float4 " + ValueID + " = " +
                    string.Format("node_gamma({0}, {1})", sColor, sGamma) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(Gamma))]
    public class GammaEditor : NodeEditor
    {
        Gamma serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as Gamma;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Color", ""));
            GUILayout.Space(10);
            SetPortBehaviour("color", "sColor", "Color", "vector4");
            SetPortBehaviour("gamma", "sGamma", "Gamma");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as Gamma;
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