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
    [CreateNodeMenu("Texture/Wave Texture")]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures")]
    public class WaveTexture : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures");
        }

        [BlenderSingle] public float fac = 5;
        [BlenderSingle] public float dist = 0;
        [BlenderRange(0, 16, false)] public float detail = 2;
        [BlenderSingle] public float detailScale = 1;
        [BlenderRange(0, 1)] public float detailRough = 0.5f;
        [BlenderSingle] public float phaseOffset = 0;

        [Input] public string sFac;
        [Input] public string sDist;
        [Input] public string sDetail;
        [Input] public string sDetailScale;
        [Input] public string sDetailRough;
        [Input] public string sPhaseOffset;
        [Input] public string sVector;

        [Output] public string Result;
        [Output] public string Result_Col;

        [NodeEnum]
        public WaveType waveType = WaveType.Bands;
        public enum WaveType { Bands, Rings }

        [NodeEnum]
        public BandsDirection bandsDirection = BandsDirection.X;
        public enum BandsDirection { X, Y, Z, Diagonal }

        [NodeEnum]
        public RingsDirection ringsDirection = RingsDirection.X;
        public enum RingsDirection { X, Y, Z, Spherical }

        [NodeEnum]
        public WaveProfile waveProfile = WaveProfile.Sine;
        public enum WaveProfile { Sine, Saw, Triangle }

        public override object GetValue(NodePort port)
        {
            string sFac = GetInputValue<string>("sFac", fac.ToString()).Split('?').Last();
            string sDist = GetInputValue<string>("sDist", dist.ToString()).Split('?').Last();
            string sDetail = GetInputValue<string>("sDetail", detail.ToString()).Split('?').Last();
            string sDetailScale = GetInputValue<string>("sDetailScale", detailScale.ToString()).Split('?').Last();
            string sDetailRough = GetInputValue<string>("sDetailRough", detailRough.ToString()).Split('?').Last();
            string sPhaseOffset = GetInputValue<string>("sPhaseOffset", phaseOffset.ToString()).Split('?').Last();
            string sVector = GetInputValue<string>("sVector", "_POS").Split('?').Last();

            string sFac_f = GetInputValue<string>("sFac", "").Split('?').First();
            string sDist_f = GetInputValue<string>("sDist", "").Split('?').First();
            string sDetail_f = GetInputValue<string>("sDetail", "").Split('?').First();
            string sDetailScale_f = GetInputValue<string>("sDetailScale", "").Split('?').First();
            string sDetailRough_f = GetInputValue<string>("sDetailRough", "").Split('?').First();
            string sPhaseOffset_f = GetInputValue<string>("sPhaseOffset", "").Split('?').First();
            string sVector_f = GetInputValue<string>("sVector", "").Split('?').First();

            this.sVector = string.Format("float3({0}, {1}, {2})", 0, 0, 0);

            string ValueID_fac = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_fac";
            string ValueID_col = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_col";

            if (port.fieldName == "Result")
            {
                return sFac_f + sDist_f + sDetail_f + sDetailScale_f + sDetailRough_f + sPhaseOffset_f + sVector_f +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_tex_wave({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12})",
                    sVector, sFac, sDist, sDetail, sDetailScale, sDetailRough, sPhaseOffset, (float)waveType, 
                    (float)bandsDirection, (float)ringsDirection, (float)waveProfile, ValueID_col, ValueID_fac) + ";?" + ValueID_fac;
            }
            else if (port.fieldName == "Result_Col")
            {
                return sFac_f + sDist_f + sDetail_f + sDetailScale_f + sDetailRough_f + sPhaseOffset_f + sVector_f +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_tex_wave({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12})",
                    sVector, sFac, sDist, sDetail, sDetailScale, sDetailRough, sPhaseOffset, (float)waveType,
                    (float)bandsDirection, (float)ringsDirection, (float)waveProfile, ValueID_col, ValueID_fac) + ";?" + ValueID_col;
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


    [CustomNodeEditor(typeof(WaveTexture))]
    public class WaveTextureEditor : NodeEditor
    {
        WaveTexture serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as WaveTexture;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Fac", ""));
            serializedNode.GetPort("Result_Col").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result_Col"), new GUIContent("Color", ""));
            GUILayout.Space(10);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("waveType"), new GUIContent("", ""));
            if (serializedNode.waveType == WaveTexture.WaveType.Bands)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("bandsDirection"), new GUIContent("", ""));
            if (serializedNode.waveType == WaveTexture.WaveType.Rings)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ringsDirection"), new GUIContent("", ""));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("waveProfile"), new GUIContent("", ""));

            NodeEditorGUILayout.PortField(new GUIContent("Vector"), serializedNode.GetInputPort("sVector"));
            serializedNode.GetInputPort("sVector").connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort("sVector").nodePortType = "vector3";

            SetPortBehaviour("fac", "sFac", "Scale");
            SetPortBehaviour("dist", "sDist", "Distortion");
            SetPortBehaviour("detail", "sDetail", "Detail");
            SetPortBehaviour("detailScale", "sDetailScale", "Detail Scale");
            SetPortBehaviour("detailRough", "sDetailRough", "Detail Roughness");
            SetPortBehaviour("phaseOffset", "sPhaseOffset", "Phase Offset");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as WaveTexture;
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