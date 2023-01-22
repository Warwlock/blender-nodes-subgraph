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
    [CreateNodeMenu("Texture/Voronoi Texture")]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures")]
    public class VoronoiTexture : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures");
        }

        [BlenderSingle] public float w = 0;
        [BlenderSingle] public float scale = 5;
        [BlenderRange(0, 1)] public float smoothness = 1;
        [BlenderSingle] public float exponent = 0.5f;
        [BlenderRange(0, 1)] public float randomness = 1;

        [Input] public string sW;
        [Input] public string sVector;
        [Input] public string sScale;
        [Input] public string sSmoothness;
        [Input] public string sExponent;
        [Input] public string sRandomness;

        [Output] public string out_distance;
        [Output] public string out_color;
        [Output] public string out_position;
        [Output] public string out_w;
        [Output] public string out_radius;

        [NodeEnum]
        public dimensionType dimenType = dimensionType._3D;
        public enum dimensionType { _1D, _2D, _3D, _4D }

        [NodeEnum]
        public featureType fType = featureType.F1;
        public enum featureType { F1, F2, SmoothF1, DistanceToEdge, NSphereRadius }

        [NodeEnum]
        public metricType mType = metricType.Euclidean;
        public enum metricType { Euclidean, Manhattan, Chebychev, Minkowski }

        public override object GetValue(NodePort port)
        {
            string sW = GetInputValue<string>("sW", w.ToString()).Split('?').Last();
            string sVector = GetInputValue<string>("sVector", "_POS").Split('?').Last();
            string sScale = GetInputValue<string>("sScale", scale.ToString()).Split('?').Last();
            string sSmoothness = GetInputValue<string>("sSmoothness", smoothness.ToString()).Split('?').Last();
            string sExponent = GetInputValue<string>("sExponent", exponent.ToString()).Split('?').Last();
            string sRandomness = GetInputValue<string>("sRandomness", randomness.ToString()).Split('?').Last();

            string sW_f = GetInputValue<string>("sW", "").Split('?').First();
            string sVector_f = GetInputValue<string>("sVector", "").Split('?').First();
            string sScale_f = GetInputValue<string>("sScale", "").Split('?').First();
            string sSmoothness_f = GetInputValue<string>("sSmoothness", "").Split('?').First();
            string sExponent_f = GetInputValue<string>("sExponent", "").Split('?').First();
            string sRandomness_f = GetInputValue<string>("sRandomness", "").Split('?').First();

            this.sVector = string.Format("float3({0}, {1}, {2})", 0, 0, 0);

            string ValueID_dis = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_dis";
            string ValueID_col = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_col";
            string ValueID_pos = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_pos";
            string ValueID_w = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_w";
            string ValueID_rad = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_rad";

            string valueConnector = sW_f + sVector_f + sScale_f + sSmoothness_f + sExponent_f + sRandomness_f +
                    "|float " + ValueID_dis + "; " + "float4 " + ValueID_col + "; " +
                    "float3 " + ValueID_pos + "; " + "float " + ValueID_w + "; " + "float " + ValueID_rad + "; " +
                    string.Format("voronoi_tex_getValue({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13})",
                    sVector, sW, sScale, sSmoothness, sExponent, sRandomness, (float)mType, (float)dimenType, (float)fType,
                    ValueID_dis, ValueID_col, ValueID_pos, ValueID_w, ValueID_rad) + ";?";

            if (port.fieldName == "out_distance")
            {
                return valueConnector + ValueID_dis;
            }
            else if (port.fieldName == "out_color")
            {
                return valueConnector + ValueID_col;
            }
            else if (port.fieldName == "out_position")
            {
                return valueConnector + "float4(" + ValueID_pos + ", 0)";
            }
            else if (port.fieldName == "out_w")
            {
                return valueConnector + ValueID_w;
            }
            else if (port.fieldName == "out_radius")
            {
                return valueConnector + ValueID_rad;
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


    [CustomNodeEditor(typeof(VoronoiTexture))]
    public class VoronoiTextureEditor : NodeEditor
    {
        VoronoiTexture serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as VoronoiTexture;
            serializedObject.Update();
            if (serializedNode.fType != VoronoiTexture.featureType.NSphereRadius)
            {
                serializedNode.GetPort("out_distance").nodePortType = "float";
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_distance"), new GUIContent("Distance", ""));
                if (serializedNode.fType != VoronoiTexture.featureType.DistanceToEdge)
                {
                    serializedNode.GetPort("out_color").nodePortType = "vector4";
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_color"), new GUIContent("Color", ""));
                    if (serializedNode.dimenType != VoronoiTexture.dimensionType._1D)
                    {
                        serializedNode.GetPort("out_position").nodePortType = "vector3";
                        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_position"), new GUIContent("Position", ""));
                    }
                    if (serializedNode.dimenType == VoronoiTexture.dimensionType._1D || serializedNode.dimenType == VoronoiTexture.dimensionType._4D)
                    {
                        serializedNode.GetPort("out_w").nodePortType = "float";
                        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_w"), new GUIContent("W", ""));
                    }
                }
            }
            if (serializedNode.fType == VoronoiTexture.featureType.NSphereRadius)
            {
                serializedNode.GetPort("out_radius").nodePortType = "float";
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_radius"), new GUIContent("Radius", ""));
            }

            GUILayout.Space(10);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("dimenType"), new GUIContent("", ""));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("fType"), new GUIContent("", ""));
            if (!(serializedNode.fType == VoronoiTexture.featureType.DistanceToEdge || serializedNode.fType == VoronoiTexture.featureType.NSphereRadius))
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("mType"), new GUIContent("", ""));

            if (serializedNode.dimenType != VoronoiTexture.dimensionType._1D)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Vector"), serializedNode.GetInputPort("sVector"));
                serializedNode.GetInputPort("sVector").connectionType = Node.ConnectionType.Override;
                serializedNode.GetInputPort("sVector").nodePortType = "vector3";
            }
            if (serializedNode.dimenType == VoronoiTexture.dimensionType._1D || serializedNode.dimenType == VoronoiTexture.dimensionType._4D)
                SetPortBehaviour("w", "sW", "W");

            SetPortBehaviour("scale", "sScale", "Scale");
            if (serializedNode.fType == VoronoiTexture.featureType.SmoothF1)
                SetPortBehaviour("smoothness", "sSmoothness", "Smoothness");
            if (serializedNode.fType != VoronoiTexture.featureType.DistanceToEdge && serializedNode.fType != VoronoiTexture.featureType.NSphereRadius)
                if (serializedNode.mType == VoronoiTexture.metricType.Minkowski)
                    SetPortBehaviour("exponent", "sExponent", "Exponent");
            SetPortBehaviour("randomness", "sRandomness", "Randomness");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as VoronoiTexture;
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