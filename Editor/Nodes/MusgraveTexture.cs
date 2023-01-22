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
    [CreateNodeMenu("Texture/Musgrave Texture")]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures")]
    public class MusgraveTexture : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures");
        }

        [BlenderSingle] public float scale = 5;
        [BlenderSingle] public float w = 0;
        [BlenderRange(0, 16, false)] public float detail = 2;
        [BlenderSingle] public float dimension = 2;
        [BlenderSingle] public float lac = 2;
        [BlenderSingle] public float offset = 0;
        [BlenderSingle] public float gain = 1;

        [Input] public string sVector = "";
        [Input] public string sScale;
        [Input] public string sW;
        [Input] public string sDetail;
        [Input] public string sDimension;
        [Input] public string sLac;
        [Input] public string sOffset;
        [Input] public string sGain;

        [Output] public string Result;

        [NodeEnum]
        public dimensionType dimenType = dimensionType._3D;
        public enum dimensionType { _1D, _2D, _3D, _4D }

        [NodeEnum]
        public tType typeType = tType.FBM;
        public enum tType { Multifractal, RidgedMultifractal, HybridMultifractal, FBM, HeteroTerrain }

        public override object GetValue(NodePort port)
        {
            string sVector = GetInputValue<string>("sVector", "_POS").Split('?').Last();
            string sScale = GetInputValue<string>("sScale", scale.ToString()).Split('?').Last();
            string sW = GetInputValue<string>("sW", w.ToString()).Split('?').Last();
            string sDetail = GetInputValue<string>("sDetail", detail.ToString()).Split('?').Last();
            string sDimension = GetInputValue<string>("sDimension", dimension.ToString()).Split('?').Last();
            string sLac = GetInputValue<string>("sLac", lac.ToString().Split('?').Last());
            string sOffset = GetInputValue<string>("sOffset", offset.ToString()).Split('?').Last();
            string sGain = GetInputValue<string>("sGain", gain.ToString()).Split('?').Last();

            string sVector_first = GetInputValue<string>("sVector", "").Split('?').First();
            string sScale_first = GetInputValue<string>("sScale", "").Split('?').First();
            string sW_first = GetInputValue<string>("sW", "").Split('?').First();
            string sDetail_first = GetInputValue<string>("sDetail", "").Split('?').First();
            string sDimension_first = GetInputValue<string>("sDimension", "").Split('?').First();
            string sLac_first = GetInputValue<string>("sLac", "").Split('?').First();
            string sOffset_first = GetInputValue<string>("sOffset", "").Split('?').First();
            string sGain_first = GetInputValue<string>("sGain", "").Split('?').First();

            this.sVector = string.Format("float3({0}, {1}, {2})", 0, 0, 0);

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_fac";

            if (port.fieldName == "Result")
            {
                return sVector_first + sScale_first + sW_first + sDetail_first + sDimension_first + sLac_first + sOffset_first + sGain_first +
                    "|float " + ValueID + " = " +
                    string.Format("texture_musgrave_factor({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                    sVector, sW, sScale, sDetail, sDimension, sLac, sOffset, sGain, (float)dimenType, (float)typeType) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(MusgraveTexture))]
    public class MusgraveTextureEditor : NodeEditor
    {
        MusgraveTexture serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as MusgraveTexture;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Height", ""));
            GUILayout.Space(10);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("dimenType"), new GUIContent("", ""));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("typeType"), new GUIContent("", ""));
            if (serializedNode.dimenType != MusgraveTexture.dimensionType._1D)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Vector"), serializedNode.GetInputPort("sVector"));
                serializedNode.GetInputPort("sVector").connectionType = Node.ConnectionType.Override;
                serializedNode.GetInputPort("sVector").nodePortType = "vector3";
            }
            if (serializedNode.dimenType == MusgraveTexture.dimensionType._1D || serializedNode.dimenType == MusgraveTexture.dimensionType._4D)
                SetPortBehaviour("w", "sW", "W");
            SetPortBehaviour("scale", "sScale", "Scale");
            SetPortBehaviour("detail", "sDetail", "Detail");
            SetPortBehaviour("dimension", "sDimension", "Dimension");
            SetPortBehaviour("lac", "sLac", "Lacunarity");

            if (serializedNode.typeType == MusgraveTexture.tType.RidgedMultifractal || serializedNode.typeType == MusgraveTexture.tType.HybridMultifractal || serializedNode.typeType == MusgraveTexture.tType.HeteroTerrain)
                SetPortBehaviour("offset", "sOffset", "Offset");
            if (serializedNode.typeType == MusgraveTexture.tType.RidgedMultifractal || serializedNode.typeType == MusgraveTexture.tType.HybridMultifractal)
                SetPortBehaviour("gain", "sGain", "Gain");

            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as MusgraveTexture;
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