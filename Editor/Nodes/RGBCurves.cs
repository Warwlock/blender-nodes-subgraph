using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MaterialNodesGraph
{
    [NodeTint("#8E8131")]
    [CreateNodeMenu("Color/RGB Curves")]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-RGB-Curves")]
    public class RGBCurves : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-RGB-Curves");
        }

        public AnimationCurve curveC = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveR = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveG = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveB = AnimationCurve.Linear(0, 0, 1, 1);
        public float currentCurve = 1;
        [BlenderRange(0, 1)] public float fac = 1;
        public CustomBlenderColor color = CustomBlenderColor.black;

        [Input] public string sFac;
        [Input] public string sColor = "";
        public string sCurveTexture = "";

        [Output] public string out_color;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sFac = GetInputValue<string>("sFac", fac.ToString()).Split('?').Last();
            string sColor = GetInputValue<string>("sColor", this.sColor).Split('?').Last();
            string sCurveTexture = GetInputValue<string>("sCurveTexture", this.sCurveTexture).Split('?').Last();

            string sFac_f = GetInputValue<string>("sFac", "").Split('?').First();
            string sColor_f = GetInputValue<string>("sColor", "").Split('?').First();
            string sCurveTexture_f = GetInputValue<string>("sCurveTexture", "").Split('?').First();

            this.sColor = string.Format("float4({0}, {1}, {2}, {3})", color.r, color.g, color.b, color.a);
            this.sCurveTexture = string.Format("curve_" + Mathf.Abs(GetInstanceID()).ToString());

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();            

            if (port.fieldName == "out_color")
            {
                return sFac_f + sColor_f + sCurveTexture_f +
                    "|float4 " + ValueID + " = " +
                    string.Format("node_rgb_curves({0}, {1}, {2})", sFac, sColor, sCurveTexture) + ";?" + ValueID;
            }
            else
                return 0f;
        }

        private void OnValidate()
        {
            //Debug.Log(curveC.keys[0].outWeight);
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


    [CustomNodeEditor(typeof(RGBCurves))]
    public class RGBCurvesEditor : NodeEditor
    {
        string path;
        RGBCurves serializedNode;
        bool t1 = true, t2 = false, t3 = false, t4 = false;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as RGBCurves;
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(t1, "C", EditorStyles.toolbarButton))
                SetToggle(1);
            if (GUILayout.Toggle(t2, "R", EditorStyles.toolbarButton))
                SetToggle(2);
            if (GUILayout.Toggle(t3, "G", EditorStyles.toolbarButton))
                SetToggle(3);
            if (GUILayout.Toggle(t4, "B", EditorStyles.toolbarButton))
                SetToggle(4);
            GUILayout.EndHorizontal();

            if (serializedNode.currentCurve == 1)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("curveC"), new GUIContent("Combined", ""), null);
            if (serializedNode.currentCurve == 2)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("curveR"), new GUIContent("R Channel", ""), null);
            if (serializedNode.currentCurve == 3)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("curveG"), new GUIContent("G Channel", ""), null);
            if (serializedNode.currentCurve == 4)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("curveB"), new GUIContent("B Channel", ""), null);

            /*if (GUILayout.Button("Generate Curve"))
            {
                Debug.Log(serializedNode.GetInstanceID());
                Texture2D curveTexture = new Texture2D(512, 1, TextureFormat.RGBA32, false, true);
                Color color;
                for (int i = 0; i < 512; i++)
                {
                    float t = i / 512f;
                    color = new Color(serializedNode.curveR.Evaluate(t), serializedNode.curveG.Evaluate(t), serializedNode.curveB.Evaluate(t), serializedNode.curveC.Evaluate(t));
                    curveTexture.SetPixel(i, 0, color);
                    //Debug.Log(color);
                }
                curveTexture.Apply();
                path = EditorUtility.SaveFilePanel(
                    "Save PNG File",
                    "Assets/",
                    "MyCurveTexture",
                    "png");
                if (path != "" && path != null)
                {
                    File.WriteAllBytes(path, curveTexture.EncodeToPNG());
                    string newPath = "Assets" + path.Substring(Application.dataPath.Length);
                    AssetDatabase.ImportAsset(newPath);

                    TextureImporter txtImp = (TextureImporter)AssetImporter.GetAtPath(newPath);
                    txtImp.sRGBTexture = false;
                    txtImp.SetPlatformTextureSettings(new TextureImporterPlatformSettings() { resizeAlgorithm = TextureResizeAlgorithm.Bilinear });
                    txtImp.filterMode = FilterMode.Point;
                    txtImp.textureCompression = TextureImporterCompression.Uncompressed;
                    txtImp.maxTextureSize = 64;
                    txtImp.mipmapEnabled = false;
                    AssetDatabase.ImportAsset(newPath);
                }
            }

            GUILayout.Space(5);
            GUIStyle horizontalLine;
            horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            Colorize(horizontalLine.normal.background, Color.gray);
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
            GUILayout.Box(GUIContent.none, horizontalLine);
            Colorize(horizontalLine.normal.background, Color.white);
            GUILayout.Space(5);*/

            serializedNode.GetPort("out_color").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_color"), new GUIContent("Result", ""));
            //NodeEditorGUILayout.PortField(new GUIContent("Generated Curve Texture", "Connect this port directly to Texture2D port of Material Input Node."), serializedNode.GetInputPort("sCurveTexture"));
            SetPortBehaviour("fac", "sFac", "Fac");
            SetPortBehaviour("color", "sColor", "Color", "vector4");
            serializedObject.ApplyModifiedProperties();
        }

        void SetToggle(int toggle)
        {
            if (serializedNode == null) serializedNode = target as RGBCurves;
            serializedNode.currentCurve = toggle;
            t1 = false; t2 = false; t3 = false; t4 = false;
            if (toggle == 1)
                t1 = true;
            if (toggle == 2)
                t2 = true;
            if (toggle == 3)
                t3 = true;
            if (toggle == 4)
                t4 = true;
        }


        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as RGBCurves;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = portType;
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
        }

        void Colorize(Texture2D tex, Color color)
        {
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();
        }
    }
}