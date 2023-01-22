using UnityEditor;
using UnityEngine;
using BNGNode;
using System.Reflection;
using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
#endif

namespace BNGNodeEditor {
    /// <summary> Override graph inspector to show an 'Open Graph' button at the top </summary>
    [CustomEditor(typeof(BNGNode.NodeGraph), true)]
    [CanEditMultipleObjects]
#if ODIN_INSPECTOR
    public class GlobalGraphEditor : OdinEditor {
        public override void OnInspectorGUI() {
            if (GUILayout.Button("Edit Graph", GUILayout.Height(20))) {
                NodeEditorWindow.Open(serializedObject.targetObject as XNode.NodeGraph);
            }
            base.OnInspectorGUI();
        }
    }
#else
    public class GlobalGraphEditor : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            if (targets.Length == 1)
            {
                if (GUILayout.Button("Edit Graph", GUILayout.Height(20)))
                {
                    BNGNode.NodeGraph graphObj = serializedObject.targetObject as BNGNode.NodeGraph;
                    NodeEditorWindow.Open(graphObj, graphObj.name);
                }
            }

            GUILayout.Space(2);

            if (GUILayout.Button("Generate Subgraph", GUILayout.Height(20)))
            {
                BNGNode.NodeGraph[] graphObj = new BNGNode.NodeGraph[targets.Length];
                for(int i = 0; i < targets.Length; i++)
                {
                    graphObj[i] = serializedObject.targetObjects[i] as BNGNode.NodeGraph;
                }
                for (int i = 0; i < graphObj.Length; i++)
                {
                    (graphObj[i] as MaterialNodesGraph.MaterialNodeGraph).newFilePath = Application.dataPath + AssetDatabase.GetAssetPath(graphObj[i]).Substring("Assets".Length);
                    if ((graphObj[i] as MaterialNodesGraph.MaterialNodeGraph).newFilePath != "")
                    {
                        (graphObj[i] as MaterialNodesGraph.MaterialNodeGraph).hlslFileName = System.IO.Path.ChangeExtension(name, ".hlsl");
                        //Debug.Log(hlslFileName);
                        (graphObj[i] as MaterialNodesGraph.MaterialNodeGraph).newFilePath = Application.dataPath + AssetDatabase.GetAssetPath(graphObj[i]).Substring("Assets".Length);
                        (graphObj[i] as MaterialNodesGraph.MaterialNodeGraph).GenerateCode((graphObj[i] as MaterialNodesGraph.MaterialNodeGraph).newFilePath);
                    }
                }
            }

            if (targets.Length == 1)
            {
                BNGNode.NodeGraph graphObj = serializedObject.targetObject as BNGNode.NodeGraph;
                GUILayout.Space(10);
                GUILayout.Label("Subgraph Menu Name");
                EditorGUI.BeginChangeCheck();
                (graphObj as MaterialNodesGraph.MaterialNodeGraph).graphMenuPath = 
                    EditorGUILayout.TextField((graphObj as MaterialNodesGraph.MaterialNodeGraph).graphMenuPath);
                if (EditorGUI.EndChangeCheck())
                {
                    if (AssetDatabase.Contains(graphObj))
                    {
                        EditorUtility.SetDirty(graphObj);
                        if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
                    }
                }
            }

            if (NodeEditorPreferences.GetSettings().dMode)
            {
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                GUILayout.Label("Raw data - Debug mode", "BoldLabel");

                DrawDefaultInspector();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    [CustomEditor(typeof(BNGNode.Node), true)]
#if ODIN_INSPECTOR
    public class GlobalNodeEditor : OdinEditor {
        public override void OnInspectorGUI() {
            if (GUILayout.Button("Edit Graph", GUILayout.Height(20))) {
                SerializedProperty graphProp = serializedObject.FindProperty("graph");
                NodeEditorWindow w = NodeEditorWindow.Open(graphProp.objectReferenceValue as XNode.NodeGraph);
                w.Home(); // Focus selected node
            }
            base.OnInspectorGUI();
        }
    }
#else
    public class GlobalNodeEditor : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty graphProp = serializedObject.FindProperty("graph");
            BNGNode.NodeGraph graphObj = graphProp.objectReferenceValue as BNGNode.NodeGraph;

            BNGNode.Node nodeObj = serializedObject.targetObject as BNGNode.Node;

            Color oldCol = GUI.color;

            if (GUILayout.Button("Focus to Node", GUILayout.Height(20))) {                
                NodeEditorWindow w = NodeEditorWindow.Open(graphObj, graphObj.name);
                w.Home(); // Focus selected node
            }

            if(GUILayout.Button("Show In Project", GUILayout.Height(20)))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(nodeObj)));
            }

            for (int i = 0; i < graphObj.nodes.Count; i++)
            {
                if (nodeObj == graphObj.nodes[i])
                {
                    NodeEditor nodeEditor = NodeEditor.GetEditor(nodeObj, NodeEditorWindow.current);
                    GUILayout.Space(EditorGUIUtility.singleLineHeight);
                    Color nodeCol;
                    NodeEditorReflection.TryGetAttributeTint(nodeObj.GetType(), out nodeCol);
                    nodeCol *= 2;
                    GUI.color = nodeCol;
                    GUILayout.Label(nodeObj.name, "BoldLabel");
                    GUI.color = oldCol;

                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Label:");
                    EditorGUI.BeginChangeCheck();
                    nodeObj.nodeLabel = EditorGUILayout.TextField(nodeObj.nodeLabel);
                    if (EditorGUI.EndChangeCheck())
                    {
                        NodeEditorWindow.RepaintAll();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);
                    nodeObj.useCustomColor = GUILayout.Toggle(nodeObj.useCustomColor, " Color");

                    if (nodeObj.useCustomColor)
                    {
                        GUILayout.Space(5);
                        nodeObj.nodeColor = EditorGUILayout.ColorField(nodeObj.nodeColor);
                        NodeEditorWindow.RepaintAll();
                    }
                }                
            }

            /*GUILayout.Space(10);
            oldCol = GUI.color;
            GUI.color = Color.yellow;
            GUILayout.Label("Values Status");
            GUI.color = oldCol;
            FieldInfo[] objectFields = nodeObj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < objectFields.Length; i++)
            {
                CustomVariables attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(CustomVariables)) as CustomVariables;
                if (attribute != null)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(objectFields[i].Name), new GUIContent(attribute.variableName));
                    //Debug.Log(attribute.variableName);
                }
            }
            GUILayout.Space(10);
            GUI.color = Color.yellow;
            GUILayout.Label("Ports Status");
            GUI.color = oldCol;
            for (int i = 0; i < objectFields.Length; i++)
            {
                CustomPortsName attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(CustomPortsName)) as CustomPortsName;
                if (attribute != null)
                {
                    NodePort myPort = nodeObj.GetPort(objectFields[i].Name);
                    string connectedNodes = "";

                    for(int p = 0; p < myPort.ConnectionCount; p++)
                    {
                        connectedNodes += myPort.GetConnections()[p].node.name + "\n\t   ";
                    }
                    if (myPort.IsConnected)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(attribute.variableName + " Port:");
                        GUI.color = Color.green;
                        GUILayout.Label("Connected | " + myPort.ConnectionCount + 
                            " connection" + (myPort.ConnectionCount > 1 ? "s" : ""));
                        GUI.color = oldCol;
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(attribute.variableName + " Port:");
                        GUI.color = Color.red;
                        GUILayout.Label("Disconnected");
                        GUI.color = oldCol;
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    //Debug.Log(attribute.variableName);
                }
            }
            NodeEditorWindow.RepaintAll();*/
            if (NodeEditorPreferences.GetSettings().dMode)
            {
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                GUILayout.Label("Raw data - Debug mode", "BoldLabel");

                // Now draw the node itself.
                DrawDefaultInspector();
            }
            nodeObj.TriggerOnValidate();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}