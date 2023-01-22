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
    [CreateNodeMenu("Texture/Magic Texture")]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures")]
    public class MagicTexture : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures");
        }

        [Range(0, 10)] public int depth = 2;
        public Vector3 vector = Vector3.zero;
        [BlenderSingle] public float scale = 5;
        [BlenderSingle] public float distortion = 1;

        [Input] public string sVector;
        [Input] public string sScale;
        [Input] public string sDistortion;

        [Output] public string out_factor;
        [Output] public string out_color;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sVector = GetInputValue<string>("sVector", "_POS").Split('?').Last();
            string sScale = GetInputValue<string>("sScale", scale.ToString()).Split('?').Last();
            string sDistortion = GetInputValue<string>("sDistortion", distortion.ToString()).Split('?').Last();

            string sVector_f = GetInputValue<string>("sVector", "").Split('?').First();
            string sScale_f = GetInputValue<string>("sScale", "").Split('?').First();
            string sDistortion_f = GetInputValue<string>("sDistortion", "").Split('?').First();

            this.sVector = string.Format("float3({0}, {1}, {2})", vector.x, vector.y, vector.z);

            string ValueID_fac = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_fac";
            string ValueID_col = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_col";

            if (port.fieldName == "out_factor")
            {
                return sVector_f + sScale_f + sDistortion_f +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_tex_magic({0}, {1}, {2}, {3}, {4}, {5})", 
                    sVector, sScale, sDistortion, depth, ValueID_fac, ValueID_col) + ";?" + ValueID_fac;
            }
            else if (port.fieldName == "out_color")
            {
                return sVector_f + sScale_f + sDistortion_f +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_tex_magic({0}, {1}, {2}, {3}, {4}, {5})",
                    sVector, sScale, sDistortion, depth, ValueID_fac, ValueID_col) + ";?" + ValueID_col;
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


    [CustomNodeEditor(typeof(MagicTexture))]
    public class MagicTextureEditor : NodeEditor
    {
        MagicTexture serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as MagicTexture;
            serializedObject.Update();
            serializedNode.GetPort("out_color").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_color"), new GUIContent("Color", ""));
            serializedNode.GetPort("out_factor").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_factor"), new GUIContent("Fac", ""));
            GUILayout.Space(10);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("depth"), new GUIContent("Depth", ""), null);
            NodeEditorGUILayout.PortField(new GUIContent("Vector"), serializedNode.GetInputPort("sVector"));
            serializedNode.GetInputPort("sVector").connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort("sVector").nodePortType = "vector3";
            SetPortBehaviour("scale", "sScale", "Scale");
            SetPortBehaviour("distortion", "sDistortion", "Distortion");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, bool usePort = true)
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as MagicTexture;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = "float";
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            if (usePort)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
            else
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), null);
        }
    }
}