using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace MaterialNodesGraph
{
    [NodeTint("#62B05A")]
    [CreateNodeMenu("Utility/Custom Function", 9999)]
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures")]
    public class CustomFunctionNode : Node
    {
        [ContextMenu("Help")]
        public void GetHelp()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/Using-Textures");
        }

        public TextAsset SourceFile;
        public string FunctionName;
        public string[] parameterList;

        public List<float> flValues;
        public List<Vector2> v2Values;
        public List<Vector3> v3Values;
        public List<CustomBlenderColor> v4Values;

        public bool isNameSame = false;

        public override object GetValue(NodePort port)
        {
            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString() + "_";

            string[] ValueID_outName = new string[DynamicOutputs.Count()];
            string[] ValueID_outType = new string[DynamicOutputs.Count()];

            string[] inputsType = new string[DynamicInputs.Count()];
            string[] inputsName = new string[DynamicInputs.Count()];
            string[] inputValues_first = new string[DynamicInputs.Count()];
            string[] inputValues_last = new string[DynamicInputs.Count()];

            for (int i = 0; i < DynamicOutputs.Count(); i++)
            {
                int x = DynamicOutputs.Count() - 1 - i;
                ValueID_outName[i] = ValueID + DynamicOutputs.ElementAt(x).fieldName;

                if (DynamicOutputs.ElementAt(x).nodePortType == "float")
                {
                    ValueID_outType[i] = "float";
                }
                if (DynamicOutputs.ElementAt(x).nodePortType == "vector2")
                {
                    ValueID_outType[i] = "float2";
                }
                if (DynamicOutputs.ElementAt(x).nodePortType == "vector3")
                {
                    ValueID_outType[i] = "float3";
                }
                if (DynamicOutputs.ElementAt(x).nodePortType == "vector4")
                {
                    ValueID_outType[i] = "float4";
                }
                if (DynamicOutputs.ElementAt(x).nodePortType == "Texture2D")
                {
                    ValueID_outType[i] = "Texture2D";
                }
            }

            for(int i = 0; i < DynamicInputs.Count(); i++)
            {
                int x = DynamicInputs.Count() - 1 - i;
                inputsName[i] = DynamicInputs.ElementAt(x).fieldName;

                if(DynamicInputs.ElementAt(x).GetInputValue() != null)
                {
                    inputValues_first[i] = DynamicInputs.ElementAt(x).GetInputValue<string>().Split('?').First();
                    inputValues_last[i] = DynamicInputs.ElementAt(x).GetInputValue<string>().Split('?').Last();
                }
                else
                {
                    if (DynamicInputs.ElementAt(x).nodePortType == "float")
                    {
                        inputValues_first[i] = "";
                        inputValues_last[i] = flValues[i].ToString();
                    }
                    if (DynamicInputs.ElementAt(x).nodePortType == "vector2")
                    {
                        string vec = string.Format("float2({0}, {1})", v2Values[i].x, v2Values[i].y);
                        inputValues_first[i] = "";
                        inputValues_last[i] = vec;
                    }
                    if (DynamicInputs.ElementAt(x).nodePortType == "vector3")
                    {
                        string vec = string.Format("float3({0}, {1}, {2})", v3Values[i].x, v3Values[i].y, v3Values[i].z);
                        inputValues_first[i] = "";
                        inputValues_last[i] = vec;
                    }
                    if (DynamicInputs.ElementAt(x).nodePortType == "vector4")
                    {
                        string vec = string.Format("float4({0}, {1}, {2}, {3})", v4Values[i].r, v4Values[i].g, v4Values[i].b, v4Values[i].a);
                        inputValues_first[i] = "";
                        inputValues_last[i] = vec;
                    }
                    if (DynamicInputs.ElementAt(x).nodePortType == "Texture2D")
                    {
                        inputValues_first[i] = "";
                        inputValues_last[i] = "_Empty_Texture";
                    }
                }
            }

            for (int i = 0; i < DynamicOutputs.Count(); i++)
            {
                int x = DynamicOutputs.Count() - 1 - i;
                if (DynamicOutputs.ElementAt(x) == port)
                {
                    string fullOut = "";

                    for (int t = 0; t < inputValues_first.Length; t++)
                    {
                        fullOut += inputValues_first[t];
                    }

                    fullOut += "|";

                    for (int t = 0; t < ValueID_outName.Length; t++)
                    {
                        fullOut += ValueID_outType[t] + " ";
                        fullOut += ValueID_outName[t] + "; ";
                    }

                    fullOut += FunctionName + "_float(";

                    for(int t = 0; t < inputValues_last.Length; t++)
                    {
                        fullOut += inputValues_last[t] + ", ";
                    }

                    for (int t = 0; t < ValueID_outName.Length; t++)
                    {
                        fullOut += ValueID_outName[t] + (t != ValueID_outName.Length - 1 ? ", " : ")");
                    }

                    fullOut += ";?";
                    fullOut += ValueID_outName[i];
                    return fullOut;
                }
            }
            return 0;
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


    [CustomNodeEditor(typeof(CustomFunctionNode))]
    public class CustomFunctionNodeEditor : NodeEditor
    {
        CustomFunctionNode serializedNode;
        string funcName;
        string sourceFileText;
        string[] parameterList;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as CustomFunctionNode;
            serializedObject.Update();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("SourceFile"), new GUIContent("Source File", ""));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("FunctionName"), new GUIContent("Function Name", ""));

            if (serializedNode.SourceFile != null)
            {
                if (AssetDatabase.GetAssetPath(serializedNode.SourceFile).Split('.').Last() != "hlsl")
                {
                    serializedNode.SourceFile = null;
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Generate Ports"))
            {
                bool isSec = false;
                secondOne:

                funcName = "";
                sourceFileText = "";
                parameterList = new string[0];

                serializedNode.ClearDynamicPorts();
                serializedNode.flValues = new List<float>();
                serializedNode.v2Values = new List<Vector2>();
                serializedNode.v3Values = new List<Vector3>();
                serializedNode.v4Values = new List<CustomBlenderColor>();

                string graphName = serializedNode.graph.name;
                graphName = "0" + graphName;
                graphName = graphName.Replace(" ", "_");
                graphName = Regex.Replace(graphName, @"^[^a-zA-Z]|[^a-zA-Z0-9_]", "");
                for (int i = 0; i < 10; i++)
                {
                    if (graphName[0] == i.ToString()[0])
                    {
                        graphName = "_" + graphName;
                        break;
                    }
                }

                if (graphName != serializedNode.FunctionName)
                {
                    serializedNode.isNameSame = false;

                    if (serializedNode.FunctionName != null && serializedNode.FunctionName != "")
                    {
                        funcName = serializedNode.FunctionName + "_float";

                        sourceFileText = File.ReadAllText(Application.dataPath + AssetDatabase.GetAssetPath(serializedNode.SourceFile).Substring("Assets".Length));


                        int pfIndex = sourceFileText.IndexOf(funcName);
                        if (pfIndex != -1)
                        {
                            pfIndex += funcName.Length;

                            for (int i = 0; i < 10; i++)
                            {
                                if (sourceFileText[pfIndex + i] == '(')
                                {
                                    pfIndex = pfIndex + i;
                                    break;
                                }
                            }

                            int plIndex = 0;

                            for (int i = 0; i < 1000; i++)
                            {
                                if (sourceFileText[pfIndex + i] == ')')
                                {
                                    plIndex = pfIndex + i;
                                    break;
                                }
                            }

                            string parameterString = sourceFileText.Substring(pfIndex + 1, plIndex - pfIndex - 1);
                            string[] parameteres = parameterString.Split(',');
                            for (int i = 0; i < parameteres.Length; i++)
                            {
                                string singleText = "";
                                string[] parameterValues = parameteres[i].Split(' ');
                                for (int t = 0; t < parameterValues.Length; t++)
                                {
                                    if (parameterValues[t].Trim() != "" && parameterValues[t].Trim() != " " && parameterValues[t] != null)
                                    {
                                        singleText += parameterValues[t];
                                        singleText += "|";
                                    }
                                }
                                parameteres[i] = singleText;
                            }
                            parameterList = parameteres;
                            serializedNode.parameterList = parameterList;
                        }
                    }

                    for (int i = 0; i < parameterList.Length; i++)
                    {
                        string[] parameter = parameterList[i].Split('|');
                        if (parameter[0] == "out")
                        {
                            string portType = "float";
                            if (parameter[1] == "float")
                                portType = "float";
                            if (parameter[1] == "float2")
                                portType = "vector2";
                            if (parameter[1] == "float3")
                                portType = "vector3";
                            if (parameter[1] == "float4")
                                portType = "vector4";
                            if (parameter[1] == "Texture2D")
                                portType = "Texture2D";
                            serializedNode.AddDynamicOutput(typeof(string), fieldName: parameter[2], portType: portType);
                        }
                        else
                        {
                            string portType = "float";
                            if (parameter[0] == "float")
                                portType = "float";
                            if (parameter[0] == "float2")
                                portType = "vector2";
                            if (parameter[0] == "float3")
                                portType = "vector3";
                            if (parameter[0] == "float4")
                                portType = "vector4";
                            if (parameter[0] == "Texture2D")
                                portType = "Texture2D";
                            serializedNode.AddDynamicInput(typeof(string), fieldName: parameter[1], portType: portType);
                            serializedNode.flValues.Add(0);
                            serializedNode.v2Values.Add(Vector2.zero);
                            serializedNode.v3Values.Add(Vector3.zero);
                            serializedNode.v4Values.Add(new CustomBlenderColor(0.5f));
                        }
                    }
                    if (!isSec)
                    {
                        isSec = true;
                        goto secondOne;
                    }
                }
                else
                {
                    serializedNode.isNameSame = true;
                }
            }

            if(serializedNode.isNameSame)
                EditorGUILayout.HelpBox("Same Name with Graph", MessageType.Error);

            for (int i = 0; i < serializedNode.DynamicOutputs.Count(); i++)
            {
                NodeEditorGUILayout.PortField(serializedNode.DynamicOutputs.ElementAt(serializedNode.DynamicOutputs.Count() - i - 1));
            }

            for (int i = 0; i < serializedNode.DynamicInputs.Count(); i++)
            {
                NodePort port = serializedNode.DynamicInputs.ElementAt(serializedNode.DynamicInputs.Count() - i - 1);

                EditorGUILayout.BeginHorizontal();
                if (port.IsConnected)
                {
                    EditorGUILayout.LabelField(port.fieldName);
                }
                else if (port.nodePortType == "float")
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(CustomFunctionNode.flValues)).GetArrayElementAtIndex(i),
                        new GUIContent(port.fieldName));
                }
                else if (port.nodePortType == "vector2")
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(CustomFunctionNode.v2Values)).GetArrayElementAtIndex(i),
                        new GUIContent(port.fieldName));
                }
                else if (port.nodePortType == "vector3")
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(CustomFunctionNode.v3Values)).GetArrayElementAtIndex(i),
                        new GUIContent(port.fieldName));
                }
                else if (port.nodePortType == "vector4")
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(CustomFunctionNode.v4Values)).GetArrayElementAtIndex(i),
                        new GUIContent(port.fieldName));
                }
                else
                {
                    EditorGUILayout.LabelField(port.fieldName);
                }
                
                Rect rect = GUILayoutUtility.GetLastRect();
                rect.x -= 15;
                NodeEditorGUILayout.PortField(rect.position, port);

                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as CustomFunctionNode;
            serializedNode.GetInputPort(portNamer).connectionType = Node.ConnectionType.Override;
            serializedNode.GetInputPort(portNamer).nodePortType = portType;
            if (serializedNode.GetPort(portNamer).IsConnected)
                fieldName = "";
            else
                fieldName = "Input value used for unconnected sockets.";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(propertyNamer), new GUIContent(guiNamer, fieldName), serializedNode.GetInputPort(portNamer));
        }

        public string Between(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }
    }
}