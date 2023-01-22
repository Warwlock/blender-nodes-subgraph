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
    [CreateNodeMenu("Texture/BrickTexture")]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures")]
    public class BrickTexture : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures");
        }

        [BlenderRange(0f, 1f, true, true)] public float offset = 0.5f;
        [BlenderRange(1f, 99f, false, true, true)] public float offset_freq = 2;
        [BlenderRange(0f, 99f, false, true)] public float squash = 1;
        [BlenderRange(1f, 99f, false, true, true)] public float squash_freq = 2;
        public Vector3 vector = Vector3.zero;
        public CustomBlenderColor color1 = new CustomBlenderColor(0.8f);
        public CustomBlenderColor color2 = new CustomBlenderColor(0.2f);
        public CustomBlenderColor mortar = CustomBlenderColor.black;
        [BlenderSingle] public float scale = 5;
        [BlenderRange(0f, 0.125f, false)] public float mortarSize = 0.02f;
        [BlenderRange(0f, 1f, false)] public float mortarSmooth = 0.1f;
        [BlenderRange(-1f, 1f, false)] public float bias = 0f;
        [BlenderRange(0.01f, 100f, false)] public float brickWidth = 0.5f;
        [BlenderRange(0.01f, 100f, false)] public float brickHeight = 0.25f;

        [Input] public string sOffset = "";
        [Input] public string sOffset_freq;
        [Input] public string sSquash;
        [Input] public string sSquash_freq;
        [Input] public string sVector;
        [Input] public string sColor1 = "";
        [Input] public string sColor2 = "";
        [Input] public string sMortar = "";
        [Input] public string sScale;
        [Input] public string sMortarSize;
        [Input] public string sMortarSmooth;
        [Input] public string sBias;
        [Input] public string sBrickWidth;
        [Input] public string sBrickHeight;

        [Output] public string out_factor;
        [Output] public string out_color;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sOffset = GetInputValue<string>("sOffset", offset.ToString()).Split('?').Last();
            string sOffset_freq = GetInputValue<string>("sOffset_freq", offset_freq.ToString()).Split('?').Last();
            string sSquash = GetInputValue<string>("sSquash", squash.ToString()).Split('?').Last();
            string sSquash_freq = GetInputValue<string>("sSquash_freq", squash_freq.ToString()).Split('?').Last();
            string sVector = GetInputValue<string>("sVector", "_POS").Split('?').Last();
            string sColor1 = GetInputValue<string>("sColor1", this.sColor1).Split('?').Last();
            string sColor2 = GetInputValue<string>("sColor2", this.sColor2).Split('?').Last();
            string sMortar = GetInputValue<string>("sMortar", this.sMortar).Split('?').Last();
            string sScale = GetInputValue<string>("sScale", scale.ToString()).Split('?').Last();
            string sMortarSize = GetInputValue<string>("sMortarSize", mortarSize.ToString()).Split('?').Last();
            string sMortarSmooth = GetInputValue<string>("sMortarSmooth", mortarSmooth.ToString()).Split('?').Last();
            string sBias = GetInputValue<string>("sBias", bias.ToString()).Split('?').Last();
            string sBrickWidth = GetInputValue<string>("sBrickWidth", brickWidth.ToString()).Split('?').Last();
            string sBrickHeight = GetInputValue<string>("sBrickHeight", brickHeight.ToString()).Split('?').Last();

            string sOffset_f = GetInputValue<string>("sOffset", "").Split('?').First();
            string sOffset_freq_f = GetInputValue<string>("sOffset_freq","").Split('?').First();
            string sSquash_f = GetInputValue<string>("sSquash", "").Split('?').First();
            string sSquash_freq_f = GetInputValue<string>("sSquash_freq", "").Split('?').First();
            string sVector_f = GetInputValue<string>("sVector", "").Split('?').First();
            string sColor1_f = GetInputValue<string>("sColor1", "").Split('?').First();
            string sColor2_f = GetInputValue<string>("sColor2", "").Split('?').First();
            string sMortar_f = GetInputValue<string>("sMortar", "").Split('?').First();
            string sScale_f = GetInputValue<string>("sScale", "").Split('?').First();
            string sMortarSize_f = GetInputValue<string>("sMortarSize", "").Split('?').First();
            string sMortarSmooth_f = GetInputValue<string>("sMortarSmooth", "").Split('?').First();
            string sBias_f = GetInputValue<string>("sBias", "").Split('?').First();
            string sBrickWidth_f = GetInputValue<string>("sBrickWidth", "").Split('?').First();
            string sBrickHeight_f = GetInputValue<string>("sBrickHeight", "").Split('?').First();

            this.sVector = string.Format("float3({0}, {1}, {2})", vector.x, vector.y, vector.z);
            this.sColor1 = string.Format("float4({0}, {1}, {2}, {3})", color1.r, color1.g, color1.b, color1.a);
            this.sColor2 = string.Format("float4({0}, {1}, {2}, {3})", color2.r, color2.g, color2.b, color2.a);
            this.sMortar = string.Format("float4({0}, {1}, {2}, {3})", mortar.r, mortar.g, mortar.b, mortar.a);

            string ValueID_fac = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_fac";
            string ValueID_col = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_col";

            if (port.fieldName == "out_factor")
            {
                return sVector_f + sColor1_f + sColor2_f + sMortar_f + sScale_f + sMortarSize_f + sMortarSmooth_f + sBias_f + sBrickWidth_f + 
                    sBrickHeight_f + sOffset_f + sOffset_freq_f + sSquash_f + sSquash_freq_f +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_tex_brick({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15})",
                    sVector, sColor1, sColor2, sMortar, sScale, sMortarSize, sMortarSmooth, sBias, sBrickWidth,
                    sBrickHeight, sOffset, sOffset_freq, sSquash, sSquash_freq, ValueID_fac, ValueID_col) + ";?" + ValueID_fac;
            }
            else if (port.fieldName == "out_color")
            {
                return sVector_f + sColor1_f + sColor2_f + sMortar_f + sScale_f + sMortarSize_f + sMortarSmooth_f + sBias_f + sBrickWidth_f + 
                    sBrickHeight_f + sOffset_f + sOffset_freq_f + sSquash_f + sSquash_freq_f +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_tex_brick({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15})",
                    sVector, sColor1, sColor2, sMortar, sScale, sMortarSize, sMortarSmooth, sBias, sBrickWidth,
                    sBrickHeight, sOffset, sOffset_freq, sSquash, sSquash_freq, ValueID_fac, ValueID_col) + ";?" + ValueID_col;
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


    [CustomNodeEditor(typeof(BrickTexture))]
    public class BrickTextureEditor : NodeEditor
    {
        BrickTexture serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as BrickTexture;
            serializedObject.Update();
            NodePort myPort = serializedNode.GetPort("out_color");
            myPort.nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_color"), new GUIContent("Color", ""), myPort);
            myPort = serializedNode.GetPort("out_factor");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_factor"), new GUIContent("Fac", ""), myPort);
            GUILayout.Space(10);
            SetPortBehaviour("offset", "sOffset", "Offset", false);
            SetPortBehaviour("offset_freq", "sOffset_freq", "Frequency", false);
            GUILayout.Space(10);
            SetPortBehaviour("squash", "sSquash", "Squash", false);
            SetPortBehaviour("squash_freq", "sSquash_freq", "Frequency", false);
            GUILayout.Space(10);
            NodeEditorGUILayout.PortField(new GUIContent("Vector"), serializedNode.GetInputPort("sVector"));
            serializedNode.GetInputPort("sVector").connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort("sVector").nodePortType = "vector3";
            SetPortBehaviour("color1", "sColor1", "Color1", portType: "vector4");
            SetPortBehaviour("color2", "sColor2", "Color2", portType: "vector4");
            SetPortBehaviour("mortar", "sMortar", "Mortar", portType: "vector4");
            SetPortBehaviour("scale", "sScale", "Scale");
            SetPortBehaviour("mortarSize", "sMortarSize", "Mortar Size");
            SetPortBehaviour("mortarSmooth", "sMortarSmooth", "Mortar Smooth");
            SetPortBehaviour("bias", "sBias", "Bias");
            SetPortBehaviour("brickWidth", "sBrickWidth", "Brick Width");
            SetPortBehaviour("brickHeight", "sBrickHeight", "Brick Height");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, bool usePort = true, string portType = "float")
        {
            string fieldName = "";
            if (serializedNode == null) serializedNode = target as BrickTexture;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodePort nPort = serializedNode.GetInputPort(portNamer);
            nPort.nodePortType = portType;
            if (usePort)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), nPort);
            else
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), null);
        }
    }
}