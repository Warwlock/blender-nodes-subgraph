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
    [NodeTint("#4885AB")]
    [CreateNodeMenu("Converter/SeparateXYZ")]
    public class SeparateXYZ : Node
    {
        [BlenderVector] public Vector3 vector3A = Vector3.zero;
        [Input] public string a = "";

        [Output] public string ResultX;
        [Output] public string ResultY;
        [Output] public string ResultZ;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", this.a).Split('?').Last();

            string a_first = GetInputValue<string>("a", "").Split('?').First();

            string ValueID_x = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_x";
            string ValueID_y = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_y";
            string ValueID_z = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_z";

            if (port.fieldName == "ResultX")
            {
                this.a = vector3A.x.ToString();
                return a_first +
                    "|float " + ValueID_x + " = " +
                    string.Format("separate_x({0})", a) + ";?" + ValueID_x;
            }
            else if (port.fieldName == "ResultY")
            {
                this.a = vector3A.y.ToString();
                return a_first +
                    "|float " + ValueID_y + " = " +
                    string.Format("separate_y({0})", a) + ";?" + ValueID_y;
            }
            else if (port.fieldName == "ResultZ")
            {
                this.a = vector3A.z.ToString();
                return a_first +
                    "|float " + ValueID_z + " = " +
                    string.Format("separate_z({0})", a) + ";?" + ValueID_z;
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


    [CustomNodeEditor(typeof(SeparateXYZ))]
    public class SeparateXYZEditor : NodeEditor
    {
        SeparateXYZ serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as SeparateXYZ;
            serializedObject.Update();
            serializedNode.GetInputPort("a").connectionType = Node.ConnectionType.Override;
            serializedNode.GetPort("ResultX").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultX"), new GUIContent("X", ""));
            serializedNode.GetPort("ResultY").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultY"), new GUIContent("Y", ""));
            serializedNode.GetPort("ResultZ").nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ResultZ"), new GUIContent("Z", ""));
            serializedNode.GetPort("a").nodePortType = "vector3";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("vector3A"), new GUIContent("Vector", "Input value used for unconnected sockets."), serializedNode.GetInputPort("a"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}