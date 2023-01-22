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
    [CreateNodeMenu("Texture/Image Texture")]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures")]
    public class ImageTextureNode : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures");
        }

        public bool UseLOD;
        [BlenderSingle] public float LOD_Num;
        public Texture2D ImageTexture;

        [Input] public string sUvInput;
        [Input] public string sImageTexture = "";
        [Input] public string sLOD_Num;

        [Output] public string Result;

        [NodeEnum]
        public FilterType fType = FilterType.Linear;
        public enum FilterType { Linear, Point, Trilinear }

        [NodeEnum]
        public WrapType wType = WrapType.Repeat;
        public enum WrapType { Repeat, Clamp, Mirror, MirrorOnce }

        public override object GetValue(NodePort port)
        {
            string sUvInput = GetInputValue<string>("sUvInput", "_UV").Split('?').Last();
            string sImageTexture = GetInputValue<string>("sImageTexture", this.sImageTexture).Split('?').Last();
            string sLOD_Num = GetInputValue<string>("sLOD_Num", LOD_Num.ToString()).Split('?').Last();

            string sUvInput_f = GetInputValue<string>("sUvInput", "").Split('?').First();
            string sImageTexture_f = GetInputValue<string>("sImageTexture", "").Split('?').First();
            string sLOD_Num_f = GetInputValue<string>("sLOD_Num", "").Split('?').First();

            this.sUvInput = string.Format("float2({0}, {1})", 0, 0);
            if (ImageTexture != null)
                this.sImageTexture = string.Format("image_" + Mathf.Abs(GetInstanceID()).ToString());
            else
                this.sImageTexture = string.Format("_Empty_Texture");

            float samplerNumber = 0;

            if (fType == FilterType.Linear && wType == WrapType.Repeat) samplerNumber = 0;
            if (fType == FilterType.Point && wType == WrapType.Repeat) samplerNumber = 1;
            if (fType == FilterType.Trilinear && wType == WrapType.Repeat) samplerNumber = 2;

            if (fType == FilterType.Linear && wType == WrapType.Clamp) samplerNumber = 3;
            if (fType == FilterType.Point && wType == WrapType.Clamp) samplerNumber = 4;
            if (fType == FilterType.Trilinear && wType == WrapType.Clamp) samplerNumber = 5;

            if (fType == FilterType.Linear && wType == WrapType.Mirror) samplerNumber = 6;
            if (fType == FilterType.Point && wType == WrapType.Mirror) samplerNumber = 7;
            if (fType == FilterType.Trilinear && wType == WrapType.Mirror) samplerNumber = 8;

            if (fType == FilterType.Linear && wType == WrapType.MirrorOnce) samplerNumber = 9;
            if (fType == FilterType.Point && wType == WrapType.MirrorOnce) samplerNumber = 10;
            if (fType == FilterType.Trilinear && wType == WrapType.MirrorOnce) samplerNumber = 11;

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                if (!UseLOD)
                    return sUvInput_f + sImageTexture_f + sLOD_Num_f +
                        "|float4 " + ValueID + " = " +
                        string.Format("node_image_texture({0}, {1}, {2})", 
                        sImageTexture, sUvInput, samplerNumber) + ";?" + ValueID;
                else
                    return sUvInput_f + sImageTexture_f + sLOD_Num_f +
                        "|float4 " + ValueID + " = " +
                        string.Format("node_image_texture_LOD({0}, {1}, {2}, {3})", 
                        sImageTexture, sUvInput, samplerNumber, sLOD_Num) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(ImageTextureNode))]
    public class ImageTextureNodeEditor : NodeEditor
    {
        ImageTextureNode serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as ImageTextureNode;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Color", ""));
            GUILayout.Space(10);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("fType"), new GUIContent("", ""));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("wType"), new GUIContent("", ""));

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("UseLOD"), new GUIContent("Use LOD", ""), null);

            GUILayout.BeginHorizontal();
            //NodeEditorGUILayout.PortField(new GUIContent(""), serializedNode.GetInputPort("sImageTexture"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ImageTexture"), new GUIContent("", ""), serializedNode.GetInputPort("sImageTexture"));
            GUILayout.EndHorizontal();
            serializedNode.GetInputPort("sImageTexture").connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort("sImageTexture").nodePortType = "Texture2D";
            NodeEditorGUILayout.PortField(new GUIContent("UV"), serializedNode.GetInputPort("sUvInput"));
            serializedNode.GetInputPort("sUvInput").connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort("sUvInput").nodePortType = "vector2";

            if (serializedNode.UseLOD)
            {
                SetPortBehaviour("LOD_Num", "sLOD_Num", "LOD");
            }

            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as ImageTextureNode;
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