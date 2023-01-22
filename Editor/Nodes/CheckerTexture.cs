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
    [NodeTint("#9F5E38")]
    [CreateNodeMenu("Texture/Checker Texture")]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures")]
    public class CheckerTexture : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures");
        }

        public Vector3 vector = Vector3.zero;
        public CustomBlenderColor color1 = CustomBlenderColor.white;
        public CustomBlenderColor color2 = CustomBlenderColor.gray;
        [BlenderSingle] public float scale = 5;

        [Input] public string sVector = "";
        [Input] public string sColor1 = "";
        [Input] public string sColor2 = "";
        [Input] public string sScale = "";

        [Output] public string out_factor;
        [Output] public string out_color;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sVector = GetInputValue<string>("sVector", "_POS").Split('?').Last();
            string sColor1 = GetInputValue<string>("sColor1", this.sColor1).Split('?').Last();
            string sColor2 = GetInputValue<string>("sColor2", this.sColor2).Split('?').Last();
            string sScale = GetInputValue<string>("sScale", scale.ToString()).Split('?').Last();

            string sVector_f = GetInputValue<string>("sVector", "").Split('?').First();
            string sColor1_f = GetInputValue<string>("sColor1", "").Split('?').First();
            string sColor2_f = GetInputValue<string>("sColor2", "").Split('?').First();
            string sScale_f = GetInputValue<string>("sScale", "").Split('?').First();

            this.sVector = string.Format("float3({0}, {1}, {2})", vector.x, vector.y, vector.z);
            this.sColor1 = string.Format("float4({0}, {1}, {2}, {3})", color1.r, color1.g, color1.b, color1.a);
            this.sColor2 = string.Format("float4({0}, {1}, {2}, {3})", color2.r, color2.g, color2.b, color2.a);

            string ValueID_fac = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_fac";
            string ValueID_col = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_col";

            if (port.fieldName == "out_factor")
            {
                return sVector_f + sColor1_f + sColor2_f + sScale_f +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_tex_checker({0}, {1}, {2}, {3}, {4}, {5})",
                    sVector, sColor1, sColor2, sScale, ValueID_fac, ValueID_col) + ";?" + ValueID_fac;
            }
            else if (port.fieldName == "out_color")
            {
                return sVector_f + sColor1_f + sColor2_f + sScale_f +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_tex_checker({0}, {1}, {2}, {3}, {4}, {5})", 
                    sVector, sColor1, sColor2, sScale, ValueID_fac, ValueID_col) + ";?" + ValueID_col;
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


    [CustomNodeEditor(typeof(CheckerTexture))]
    public class CheckerTextureEditor : NodeEditor
    {
        CheckerTexture serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as CheckerTexture;
            serializedObject.Update();
            serializedNode.GetPort("out_color").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_color"), new GUIContent("Color", ""));
            serializedNode.GetPort("out_factor").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_factor"), new GUIContent("Fac", ""));
            GUILayout.Space(10);
            NodeEditorGUILayout.PortField(new GUIContent("Vector"), serializedNode.GetInputPort("sVector"));
            serializedNode.GetInputPort("sVector").connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort("sVector").nodePortType = "vector3";
            SetPortBehaviour("color1", "sColor1", "Color1", "vector4");
            SetPortBehaviour("color2", "sColor2", "Color2", "vector4");
            SetPortBehaviour("scale", "sScale", "Scale");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as CheckerTexture;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodePort nPort = serializedNode.GetInputPort(portNamer);
            nPort.nodePortType = portType;
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), nPort);
        }
    }
}