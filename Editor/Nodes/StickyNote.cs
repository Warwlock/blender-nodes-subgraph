using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;

namespace MaterialNodesGraph
{
    [NodeTint("#fff626")]
    [CreateNodeMenu("Layout/Sticky Note", 10000)]
    public class StickyNote : Node
    {
        public string StickyTitle;
        [TextArea]
        public string StickyDetails;
        public override object GetValue(NodePort port)
        {
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


    [CustomNodeEditor(typeof(StickyNote))]
    public class StickyNodeEditor : NodeEditor
    {
        StickyNote serializedNode;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as StickyNote;
            serializedObject.Update();
            GUI.backgroundColor = new Color(1, 0.92f, 0.016f, 0.5f);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("StickyTitle"), new GUIContent());
            GUILayout.Space(-15);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("StickyDetails"), new GUIContent());
            GUI.backgroundColor = Color.white;
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer)
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as StickyNote;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
        }
    }
}