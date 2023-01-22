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
    [CreateNodeMenu("Texture/Noise Texture")]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures")]
    public class NoiseTexture : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures");
        }

        [BlenderSingle] public float fac = 5;
        [BlenderSingle] public float w = 0;
        [BlenderRange(0, 16, false)] public float detail = 2;
        [BlenderRange(0, 1)] public float rough = 0.5f;
        [BlenderSingle] public float distort = 0;

        [Input] public string sFac;
        [Input] public string sW;
        [Input] public string sDetail;
        [Input] public string sRough;
        [Input] public string sDistort;
        [Input] public string sVector;

        [Output] public string Result;
        [Output] public string Result_Col;

        [NodeEnum]
        public dimensionType dimenType = dimensionType._3D;
        public enum dimensionType { _1D, _2D, _3D, _4D }

        public override object GetValue(NodePort port)
        {
            string sFac = GetInputValue<string>("sFac", fac.ToString()).Split('?').Last();
            string sW = GetInputValue<string>("sW", w.ToString()).Split('?').Last();
            string sDetail = GetInputValue<string>("sDetail", detail.ToString()).Split('?').Last();
            string sRough = GetInputValue<string>("sRough", rough.ToString()).Split('?').Last();
            string sDistort = GetInputValue<string>("sDistort", distort.ToString()).Split('?').Last();
            string sVector = GetInputValue<string>("sVector", "_POS").Split('?').Last();

            string sFac_first = GetInputValue<string>("sFac", "").Split('?').First();
            string sW_first = GetInputValue<string>("sW", "").Split('?').First();
            string sDetail_first = GetInputValue<string>("sDetail", "").Split('?').First();
            string sRough_first = GetInputValue<string>("sRough", "").Split('?').First();
            string sDistort_first = GetInputValue<string>("sDistort", "").Split('?').First();
            string sVector_first = GetInputValue<string>("sVector", "").Split('?').First();

            this.sVector = string.Format("float3({0}, {1}, {2})", 0, 0, 0);

            string ValueID_fac = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_fac";
            string ValueID_col = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_col";

            if (port.fieldName == "Result")
            {
                return sFac_first + sW_first + sDetail_first + sRough_first + sDistort_first + sVector_first +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_noise_texture_full({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                    sVector, sW, sFac, sDetail, sRough, sDistort, (float)dimenType, ValueID_fac, ValueID_col) + ";?" + ValueID_fac;
            }
            else if (port.fieldName == "Result_Col")
            {
                return sFac_first + sW_first + sDetail_first + sRough_first + sDistort_first + sVector_first +
                    "|float " + ValueID_fac + "; " + "float4 " + ValueID_col + "; " +
                    string.Format("node_noise_texture_full({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                    sVector, sW, sFac, sDetail, sRough, sDistort, (float)dimenType, ValueID_fac, ValueID_col) + ";?" + ValueID_col;
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


    [CustomNodeEditor(typeof(NoiseTexture))]
    public class NoiseTextureEditor : NodeEditor
    {
        NoiseTexture serializedNode;
        public override void OnBodyGUI()
        {
            //serializedNode.hideFlags = HideFlags.HideInHierarchy;
            if (serializedNode == null) serializedNode = target as NoiseTexture;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Fac", ""));
            serializedNode.GetPort("Result_Col").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result_Col"), new GUIContent("Color", ""));
            GUILayout.Space(10);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("dimenType"), new GUIContent("", ""));
            if (serializedNode.dimenType != NoiseTexture.dimensionType._1D)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Vector"), serializedNode.GetInputPort("sVector"));
                serializedNode.GetInputPort("sVector").connectionType = Node.ConnectionType.Override;
                serializedNode.GetInputPort("sVector").nodePortType = "vector3";
            }
            if (serializedNode.dimenType == NoiseTexture.dimensionType._1D || serializedNode.dimenType == NoiseTexture.dimensionType._4D)
                SetPortBehaviour("w", "sW", "W");

            SetPortBehaviour("fac", "sFac", "Scale");
            SetPortBehaviour("detail", "sDetail", "Detail");
            SetPortBehaviour("rough", "sRough", "Roughness");
            SetPortBehaviour("distort", "sDistort", "Distortion");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as NoiseTexture;
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