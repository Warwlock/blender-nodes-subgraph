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
    [CreateNodeMenu("Vector/Mapping", 50)]
    public class Mapping : Node
    {
        [BlenderVector] public Vector3 vector = Vector3.zero;
        [BlenderVector] public Vector3 location = Vector3.zero;
        [BlenderVector] public Vector3 rotation = Vector3.zero;
        [BlenderVector] public Vector3 scale = Vector3.one;

        [Input] public string sVector;
        [Input] public string sLocation;
        [Input] public string sRotation;
        [Input] public string sScale;

        [Output] public string Result;

        [NodeEnum]
        public VectorType vecType = VectorType.Point;
        public enum VectorType {_Type, Point, Texture, Vector, Normal }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            this.sVector = string.Format("float3({0}, {1}, {2})", vector.x, vector.y, vector.z);
            this.sLocation = string.Format("float3({0}, {1}, {2})", location.x, location.y, location.z);
            this.sRotation = string.Format("float3({0}, {1}, {2})", rotation.x, rotation.y, rotation.z);
            this.sScale = string.Format("float3({0}, {1}, {2})", scale.x, scale.y, scale.z);

            string sVector = GetInputValue<string>("sVector", this.sVector).Split('?').Last();
            string sLocation = GetInputValue<string>("sLocation", this.sLocation).Split('?').Last();
            string sRotation = GetInputValue<string>("sRotation", this.sRotation).Split('?').Last();
            string sScale = GetInputValue<string>("sScale", this.sScale).Split('?').Last();

            string sVector_f = GetInputValue<string>("sVector", "").Split('?').First();
            string sLocation_f = GetInputValue<string>("sLocation", "").Split('?').First();
            string sRotation_f = GetInputValue<string>("sRotation", "").Split('?').First();
            string sScale_f = GetInputValue<string>("sScale", "").Split('?').First();           

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                if (vecType == VectorType.Point)
                    return sVector_f + sLocation_f + sRotation_f + sScale_f +
                        "|float4 " + ValueID + " = " +
                        string.Format("float4(mapping_point({0}, {1}, {2}, {3}), 0)", sVector, sLocation, sRotation, sScale) + ";?" + ValueID;
                else if (vecType == VectorType.Texture)
                    return sVector_f + sLocation_f + sRotation_f + sScale_f +
                        "|float4 " + ValueID + " = " +
                        string.Format("float4(mapping_texture({0}, {1}, {2}, {3}), 0)", sVector, sLocation, sRotation, sScale) + ";?" + ValueID;
                else if (vecType == VectorType.Vector)
                    return sVector_f + sLocation_f + sRotation_f + sScale_f +
                        "|float4 " + ValueID + " = " +
                        string.Format("float4(mapping_vector({0}, {1}, {2}, {3}), 0)", sVector, sLocation, sRotation, sScale) + ";?" + ValueID;
                else
                    return sVector_f + sLocation_f + sRotation_f + sScale_f +
                        "|float4 " + ValueID + " = " +
                        string.Format("float4(mapping_normal({0}, {1}, {2}, {3}), 0)", sVector, sLocation, sRotation, sScale) + ";?" + ValueID;
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


    [CustomNodeEditor(typeof(Mapping))]
    public class MappingEditor : NodeEditor
    {
        Mapping serializedNode;
        GeneralNodePopup nodePopup = new GeneralNodePopup(Vector2.zero, new string[0], "Point");
        Rect buttonRect;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as Mapping;
            serializedObject.Update();
            serializedNode.GetPort("Result").nodePortType = "vector3";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Vector", ""));
            GUILayout.Space(10);

            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("vecType"), new GUIContent("", ""));
            if (EditorGUILayout.DropdownButton(new GUIContent(AddSpacesToSentence(serializedNode.vecType.ToString())), FocusType.Keyboard))
            {
                string[] enumNames = Enum.GetNames(typeof(Mapping.VectorType));
                nodePopup = new GeneralNodePopup(new Vector2(150, 110), enumNames, serializedNode.vecType.ToString());
                nodePopup.OnCloseEvent += () => {
                    Undo.RecordObject(serializedNode, "Enum Change");
                    serializedNode.vecType = (Mapping.VectorType)Enum.Parse(typeof(Mapping.VectorType), nodePopup.EnumValue);
                    NodeEditorWindow.current.Save();
                };
                PopupWindow.Show(buttonRect, nodePopup);
            }

            /*NodeEditorGUILayout.PortField(new GUIContent("Vector"), serializedNode.GetInputPort("sVector"));
            serializedNode.GetInputPort("sVector").connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort("sVector").nodePortType = "vector3";*/

            SetPortBehaviour("vector", "sVector", "Vector");

            if (serializedNode.vecType == Mapping.VectorType.Point || serializedNode.vecType == Mapping.VectorType.Texture) 
                SetPortBehaviour("location", "sLocation", "Location");

            SetPortBehaviour("rotation", "sRotation", "Rotation");
            SetPortBehaviour("scale", "sScale", "Scale");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer)
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as Mapping;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = "vector3";
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