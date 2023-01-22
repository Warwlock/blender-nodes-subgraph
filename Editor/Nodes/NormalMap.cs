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
    [NodeTint("#6B5AAC")]
    [CreateNodeMenu("Vector/Normal Map", 50)]
    public class NormalMap : Node
    {
        [BlenderSingle] public float fac = 1;
        public CustomBlenderColor color = new CustomBlenderColor(0.5f, 0.5f, 1, 1);

        [Input] public string sFac;
        [Input] public string sColor = "";

        [Output] public string Result;

        public override object GetValue(NodePort port)
        {
            string sFac = GetInputValue<string>("sFac", fac.ToString()).Split('?').Last();
            string sColor = GetInputValue<string>("sColor", this.sColor).Split('?').Last();

            string sFac_f = GetInputValue<string>("sFac", "").Split('?').First();
            string sColor_f = GetInputValue<string>("sColor", "").Split('?').First();

            this.sColor = string.Format("float4({0}, {1}, {2}, {3})", color.r, color.g, color.b, color.a);

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                return sFac_f + sColor_f +
                    "|float4 " + ValueID + "; " +
                    string.Format("node_normal_map({0}, {1}, {2})", sColor, sFac, ValueID) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(NormalMap))]
    public class NormalMapEditor : NodeEditor
    {
        NormalMap serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as NormalMap;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "vector3";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Normal", ""));
            GUILayout.Space(10);

            SetPortBehaviour("fac", "sFac", "Strength");
            SetPortBehaviour("color", "sColor", "Color", "vector4");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as NormalMap;
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