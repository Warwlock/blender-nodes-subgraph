using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace MaterialNodesGraph
{
    [NodeTint("#6B5AAC")]
    [CreateNodeMenu("Vector/Bump", 50)]
    public class Bump : Node
    {
        public bool invert = false;
        [BlenderRange(0, 1)]public float strength = 1;
        [BlenderSingle] public float dist = 1;

        [Input] public string sStrength;
        [Input] public string sDist;
        [Input] public string sHeight;
        [Input] public string sVector;

        [Output] public string Result;

        public SpaceType spaceType = SpaceType.Tangent;

        public enum SpaceType
        {
            _OutputSpace,
            Object, Tangent, World
        }

        public override object GetValue(NodePort port)
        {
            string sStrength = GetInputValue<string>("sStrength", strength.ToString()).Split('?').Last();
            string sDist = GetInputValue<string>("sDist", dist.ToString()).Split('?').Last();
            string sHeight = GetInputValue<string>("sHeight", "0").Split('?').Last();
            string sVector;
            if (spaceType == SpaceType.World)
                sVector = GetInputValue<string>("sVector", "_NWS").Split('?').Last();
            else if (spaceType == SpaceType.Tangent)
                sVector = GetInputValue<string>("sVector", "_NTS").Split('?').Last();
            else
                sVector = GetInputValue<string>("sVector", "_NOS").Split('?').Last();

            string sStrength_f = GetInputValue<string>("sStrength", "").Split('?').First();
            string sDist_f = GetInputValue<string>("sDist", "").Split('?').First();
            string sHeight_f = GetInputValue<string>("sHeight", "").Split('?').First();
            string sVector_f = GetInputValue<string>("sVector", "").Split('?').First();

            this.sVector = string.Format("float3({0}, {1}, {2})", 0, 0, 0);

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                return sStrength_f + sDist_f + sHeight_f + sVector_f +
                    "|float4 " + ValueID + "; " +
                    string.Format("node_bump({0}, {1}, {2}, {3}, {4}, {5}, {6})", "_POS", invert ? "-1" : "1",
                        sStrength, sDist, sHeight, sVector, ValueID) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(Bump))]
    public class BumpEditor : NodeEditor
    {
        Bump serializedNode;
        GeneralNodePopup nodePopup = new GeneralNodePopup(Vector2.zero, new string[0], "Tangent");
        Rect buttonRect;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as Bump;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "vector3";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Normal", ""));
            GUILayout.Space(10);

            if (EditorGUILayout.DropdownButton(new GUIContent(AddSpacesToSentence(serializedNode.spaceType.ToString())), FocusType.Keyboard))
            {
                string[] enumNames = Enum.GetNames(typeof(Bump.SpaceType));
                nodePopup = new GeneralNodePopup(new Vector2(100, 90), enumNames, serializedNode.spaceType.ToString());
                nodePopup.OnCloseEvent += () => {
                    Undo.RecordObject(serializedNode, "Enum Change");
                    serializedNode.spaceType = (Bump.SpaceType)Enum.Parse(typeof(Bump.SpaceType), nodePopup.EnumValue);
                    NodeEditorWindow.current.Save();
                };
                PopupWindow.Show(buttonRect, nodePopup);
            }

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("invert"), new GUIContent("Invert"));

            SetPortBehaviour("strength", "sStrength", "Strength");
            SetPortBehaviour("dist", "sDist", "Distance");
            serializedObject.ApplyModifiedProperties();

            NodeEditorGUILayout.PortField(new GUIContent("Height"), serializedNode.GetInputPort("sHeight"));
            serializedNode.GetInputPort("sHeight").connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort("sHeight").nodePortType = "float";

            NodeEditorGUILayout.PortField(new GUIContent("Normal"), serializedNode.GetInputPort("sVector"));
            serializedNode.GetInputPort("sVector").connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort("sVector").nodePortType = "vector3";
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as Bump;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = portType;
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
        }

        string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            System.Text.StringBuilder newText = new System.Text.StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}