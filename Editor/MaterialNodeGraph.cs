using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using System.Linq;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BNGNodeEditor;

namespace MaterialNodesGraph
{
    [HelpURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki")]
    [CreateAssetMenu(fileName = "New Node Graph", menuName = "Material Nodes Graph", order = 82)]
    public class MaterialNodeGraph : NodeGraph
    {
        public Material test;
        public Node outNode;
        public Node inNode;
        public string hlslFileName;
        public string newFilePath;
        public string graphMenuPath = "Material Nodes Graph";
        [HideInInspector] public bool isEnable = false;
        [HideInInspector] public Vector2 mousePos;
        [HideInInspector] public List<string> TextureCurvePaths = new List<string>();

        public string[] OldOutputNodeSlotNames;
        public string[] OldInputNodeSlotNames;

        int GUIDLength = 32;
        string oldgraphMenuPath = "";

        //[ContextMenu("Generate Subgraph - Save")]
        public void contextMenuSaveFilePanel()
        {
            openGenerateHLSLSaveMenu(false);
        }
        public void openGenerateHLSLSaveMenu(bool isSaveAs)
        {
            //Debug.Log(Application.dataPath + newFilePath.Substring(Application.dataPath.Length));
            //Debug.Log(Application.dataPath + AssetDatabase.GetAssetPath(this).Substring("Assets".Length));
            newFilePath = Application.dataPath + AssetDatabase.GetAssetPath(this).Substring("Assets".Length);
            //Debug.Log(Path.ChangeExtension(name, ".hlsl"));
            /*if (!isSaveAs && (newFilePath == "" || newFilePath == null))
            {
                var path = EditorUtility.SaveFilePanel(
                    "Save Shader Subgraph File",
                    "Assets/",
                    hlslFileName == "" ? "MyFile.shadersubgraph" : hlslFileName,
                    "shadersubgraph");
                if (path != "")
                    newFilePath = path;
                else
                    return;
            }
            else if (isSaveAs)
            {
                var path = EditorUtility.SaveFilePanel(
                    "Save Shader Subgraph File",
                    "Assets/",
                    hlslFileName == "" ? "MyFile.shadersubgraph" : hlslFileName,
                    "shadersubgraph");
                if (path != "")
                    newFilePath = path;
                else
                    return;
            }*/
            if (newFilePath != "")
            {
                hlslFileName = Path.ChangeExtension(name, ".hlsl");
                //Debug.Log(hlslFileName);
                newFilePath = Application.dataPath + AssetDatabase.GetAssetPath(this).Substring("Assets".Length);
                GenerateCode(newFilePath);
            }
        }


        public void GenerateCode(string savePath = "")
        {
            outNode = null;
            inNode = null;

            string[] includeStrings = {"BrickTextureCode.hlsl", "CheckerTextureCode.hlsl",
            "GammaCode.hlsl", "GradientTextureCode.hlsl", "HueSatValCode.hlsl", "MagicTextureCode.hlsl",
            "MappingCode.hlsl", "MapRangeCode.hlsl", "MathCode.hlsl", "MixRGBCode.hlsl",
            "MusgraveTextureCode.hlsl", "NoiseTextureCode.hlsl", "VectorMathCode.hlsl",
            "VoronoiTextureCode.hlsl", "WaveTextureCode.hlsl", "BrightContrastCode.hlsl",
            "SeparateCode.hlsl", "BlackbodyCode.hlsl", "CombineCode.hlsl", "ColorRampCode.hlsl",
            "RGBCurvesCode.hlsl", "RGBtoBWCode.hlsl", "ImageTextureCode.hlsl", "InvertCode.hlsl",
            "SimpleNoiseTexture.hlsl"};

            string includeParagraph;
            string functionStartString;
            string inputVariablesString;
            string outputVariablesString;
            string insideFunctionString;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].GetOutputPort("outputvariable_1") != null)
                {
                    //Debug.Log(nodes[i].GetOutputPort("outputvariable_1").GetOutputValue());
                    outNode = nodes[i].GetOutputPort("outputvariable_1").node;
                }
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].GetOutputPort("inputvariable_1") != null)
                {
                    //Debug.Log(nodes[i].GetOutputPort("inputvariable_1").GetOutputValue());
                    inNode = nodes[i].GetOutputPort("inputvariable_1").node;
                }
            }
            List<NodePort> dynamicMaterialOutputList = outNode.DynamicInputs.ToList();
            List<NodePort> dynamicMaterialInputList = inNode.DynamicOutputs.ToList();
            //Debug.Log(dynamicMaterialOutputList[0].GetInputValue());
            outputVariablesString = "";
            for (int i = 0; i < dynamicMaterialOutputList.Count; i++)
            {
                string hlslValueType = "";
                if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "float")
                    hlslValueType = "float";
                if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "vector2")
                    hlslValueType = "float2";
                if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "vector3")
                    hlslValueType = "float3";
                if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "vector4")
                    hlslValueType = "float4";
                if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "bool")
                    hlslValueType = "bool";
                if (i != dynamicMaterialOutputList.Count - 1)
                    outputVariablesString += "out " + hlslValueType + " " + dynamicMaterialOutputList[i].fieldName.Split('_').First() + ", ";
                else
                    outputVariablesString += "out " + hlslValueType + " " + dynamicMaterialOutputList[i].fieldName.Split('_').First();
            }

            inputVariablesString = "";
            for (int i = 0; i < dynamicMaterialInputList.Count; i++)
            {
                string hlslValueType = "";
                if (dynamicMaterialInputList[i].fieldName.Split('_').Last() == "float")
                    hlslValueType = "float";
                if (dynamicMaterialInputList[i].fieldName.Split('_').Last() == "vector2")
                    hlslValueType = "float2";
                if (dynamicMaterialInputList[i].fieldName.Split('_').Last() == "uv")
                    hlslValueType = "float2";
                if (dynamicMaterialInputList[i].fieldName.Split('_').Last() == "vector3")
                    hlslValueType = "float3";
                if (dynamicMaterialInputList[i].fieldName.Split('_').Last() == "pos")
                    hlslValueType = "float3";
                if (dynamicMaterialInputList[i].fieldName.Split('_').Last() == "vector4")
                    hlslValueType = "float4";
                if (dynamicMaterialInputList[i].fieldName.Split('_').Last() == "bool")
                    hlslValueType = "bool";
                if (dynamicMaterialInputList[i].fieldName.Split('_').Last() == "Texture2D")
                    hlslValueType = "Texture2D";
                if (dynamicMaterialOutputList.Count > 0)
                {
                    inputVariablesString += hlslValueType + " " + dynamicMaterialInputList[i].fieldName.Split('_').First() + ", ";
                }
                else
                {
                    if (i != dynamicMaterialInputList.Count - 1)
                        inputVariablesString += hlslValueType + " " + dynamicMaterialInputList[i].fieldName.Split('_').First() + ", ";
                    else
                        inputVariablesString += hlslValueType + " " + dynamicMaterialInputList[i].fieldName.Split('_').First();
                }
            }

            string dynamicPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            dynamicPath = dynamicPath.Remove(dynamicPath.Length - "BlenderNodeGraph.cs".Length) + "Includes/";

            hlslFileName = newFilePath.Split('/').Last();
            hlslFileName = hlslFileName.Remove(hlslFileName.Length - ".asset".Length);
            hlslFileName = "0" + hlslFileName;
            hlslFileName = hlslFileName.Replace(" ", "_");
            hlslFileName = Regex.Replace(hlslFileName, @"^[^a-zA-Z]|[^a-zA-Z0-9_]", "");
            for (int i = 0; i < 10; i++)
            {
                if (hlslFileName[0] == i.ToString()[0])
                {
                    hlslFileName = "_" + hlslFileName;
                    break;
                }
            }

            /*includeParagraph = "#ifndef __IncludeFiles_\n #define __IncludeFiles_ \n";
            for (int i = 0; i < includeStrings.Length; i++)
            {
                includeParagraph += "\t#include <" + dynamicPath + includeStrings[i] + ">\n";
            }
            includeParagraph += "\nTEXTURE2D(_Empty_Texture);\n";
            includeParagraph += "#endif \n\n";*/

            includeParagraph = "#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>\n";

            for(int i = 0; i < nodes.Count(); i++)
            {
                if(nodes[i].GetType() == typeof(CustomFunctionNode))
                {
                    if((nodes[i] as CustomFunctionNode).DynamicOutputs.Count() > 0)
                    includeParagraph += "#include <" + AssetDatabase.GetAssetPath((nodes[i] as CustomFunctionNode).SourceFile) + ">\n";
                }
            }

            includeParagraph += "\n";

            string InputSpaces = "float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, " +
                "float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, ";
            string RGBCurveTextures = "";
            for (int i = 0; i < nodes.Count(); i++)
            {
                if (nodes[i].GetType() == typeof(RGBCurves))
                {
                    RGBCurveTextures += "Texture2D curve_" + Mathf.Abs(nodes[i].GetInstanceID()).ToString() + ", ";
                }
                if (nodes[i].GetType() == typeof(ColorRamp))
                {
                    RGBCurveTextures += "Texture2D gradient_" + Mathf.Abs(nodes[i].GetInstanceID()).ToString() + ", ";
                }
                if (nodes[i].GetType() == typeof(ImageTextureNode))
                {
                    if ((nodes[i] as ImageTextureNode).ImageTexture != null)
                        RGBCurveTextures += "Texture2D image_" + Mathf.Abs(nodes[i].GetInstanceID()).ToString() + ", ";
                }
            }
            functionStartString = "void " + hlslFileName.Split('.').First() + "_float(" + InputSpaces + inputVariablesString + RGBCurveTextures + outputVariablesString + ")\n{\n";


            insideFunctionString = "";
            string[] functionArraysRaw;
            List<string> functionLists;
            string[] functionArrays = new string[0];
            if (dynamicMaterialOutputList[0].GetInputValue() != null)
            {
                dynamicMaterialOutputList[0].GetInputValue();
                functionArraysRaw = dynamicMaterialOutputList[0].GetInputValue().ToString().Split('|');
                functionLists = functionArraysRaw.ToList();
                functionLists.Remove(functionLists.Last());
                functionLists.Add(functionArraysRaw.Last().Split('?').First());
                functionArrays = functionLists.ToArray();
            }
            for (int i = 0; i < dynamicMaterialOutputList.Count; i++)
            {
                dynamicMaterialOutputList[i].GetInputValue();
                if (dynamicMaterialOutputList[i].GetInputValue() != null)
                {
                    functionLists = dynamicMaterialOutputList[i].GetInputValue().ToString().Split('|').ToList();
                    functionLists.Remove(functionLists.Last());
                    functionLists.Add(dynamicMaterialOutputList[i].GetInputValue().ToString().Split('|').Last().Split('?').First());
                    functionArrays = functionArrays.Concat(functionLists.ToArray()).ToArray();
                }
            }
            /*for (int i = 0; i < functionArrays.Length; i++)
            {
                Debug.Log(functionArrays[i]);
            }*/
            functionArrays = functionArrays.Distinct().ToArray();
            /*Debug.Log("");
            for (int i = 0; i < functionArrays.Length; i++)
            {
                Debug.Log(functionArrays[i]);
            }*/

            for (int i = 0; i < functionArrays.Length; i++)
            {
                insideFunctionString += "\t" + functionArrays[i].Split('?').First() + "\n";
            }
            insideFunctionString += "\n";
            for (int i = 0; i < dynamicMaterialOutputList.Count; i++)
            {
                if (dynamicMaterialOutputList[i].GetInputValue() != null)
                {
                    insideFunctionString += "\t" + dynamicMaterialOutputList[i].fieldName.Split('_').First() + " = " + dynamicMaterialOutputList[i].GetInputValue().ToString().Split('|').Last().Split('?').Last() + ";\n";
                }
                else
                {
                    string nullStringOut = "";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "float")
                        nullStringOut = "0.0";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "vector2")
                        nullStringOut = "float2(0.0, 0.0)";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "vector3")
                        nullStringOut = "float3(0.0, 0.0, 0.0)";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "vector4")
                        nullStringOut = "float4(0.0, 0.0, 0.0, 0.0)";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "bool")
                        nullStringOut = "false";
                    insideFunctionString += "\t" + dynamicMaterialOutputList[i].fieldName.Split('_').First() + " = " + nullStringOut + ";\n";
                }
            }
            /*for (int i = 0; i < dynamicMaterialOutputList.Count; i++)
            {
                if (dynamicMaterialOutputList[i].GetInputValue() != null)
                {
                    //Debug.Log(dynamicMaterialOutputList[i].GetInputValue().ToString().Split('|').Last());
                    insideFunctionString += "\t" + dynamicMaterialOutputList[i].fieldName.Split('_').First() + " = " + dynamicMaterialOutputList[i].GetInputValue() + ";\n";
                }
                else
                {
                    string nullStringOut = "";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "float")
                        nullStringOut = "0.0";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "vector2")
                        nullStringOut = "float2(0.0, 0.0)";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "vector3")
                        nullStringOut = "float3(0.0, 0.0, 0.0)";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "vector4")
                        nullStringOut = "float4(0.0, 0.0, 0.0, 0.0)";
                    if (dynamicMaterialOutputList[i].fieldName.Split('_').Last() == "bool")
                        nullStringOut = "false";
                    insideFunctionString += "\t" + dynamicMaterialOutputList[i].fieldName.Split('_').First() + " = " + nullStringOut + ";\n";
                }
            }*/

            hlslFileName = newFilePath.Split('/').Last();
            dynamicPath = newFilePath.Remove(newFilePath.Length - hlslFileName.Length) + "Includes/";
            string subgraphPath = newFilePath.Remove(newFilePath.Length - hlslFileName.Length) + "Subgraph/";
            Directory.CreateDirectory(dynamicPath);
            Directory.CreateDirectory(subgraphPath);
            hlslFileName = hlslFileName.Remove(hlslFileName.Length - ".asset".Length);
            hlslFileName = "0" + hlslFileName;
            hlslFileName = hlslFileName.Replace(" ", "_");
            hlslFileName = Regex.Replace(hlslFileName, @"^[^a-zA-Z]|[^a-zA-Z0-9_]", "");
            for (int i = 0; i < 10; i++)
            {
                if (hlslFileName[0] == i.ToString()[0])
                {
                    hlslFileName = "_" + hlslFileName;
                    break;
                }
            }
            //Debug.Log(hlslFileName);
            dynamicPath += Path.ChangeExtension(hlslFileName, ".hlsl");
            subgraphPath += Path.ChangeExtension(hlslFileName, ".shadersubgraph");
            File.WriteAllText(dynamicPath, includeParagraph + functionStartString + insideFunctionString + "}");
            //newFilePath = newFilePath.Remove(newFilePath.Length - hlslFileName.Length) + "Subgraphs/" + hlslFileName;
            string newPath = "Assets" + subgraphPath.Substring(Application.dataPath.Length);
            //Debug.Log(newPath);
            string hlslNewPath = "Assets" + dynamicPath.Substring(Application.dataPath.Length);
            AssetDatabase.ImportAsset(Path.ChangeExtension(hlslNewPath, ".hlsl"));
            //EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(newPath));
            if (NodeEditorPreferences.GetSettings().dMode)
                Debug.Log("File creation is completed.");

            //bool propertyChanged = false;
            string[] OutputNodeSlotNames = new string[dynamicMaterialOutputList.Count];
            for (int i = 0; i < dynamicMaterialOutputList.Count; i++)
            {
                OutputNodeSlotNames[i] = dynamicMaterialOutputList[i].fieldName;
            }

            string[] InputNodeSlotNames = new string[dynamicMaterialInputList.Count];
            for (int i = 0; i < dynamicMaterialInputList.Count; i++)
            {
                InputNodeSlotNames[i] = dynamicMaterialInputList[i].fieldName;
            }

            /*if (OldInputNodeSlotNames != null || OldOutputNodeSlotNames != null)
            {
                if (OldInputNodeSlotNames.Length == InputNodeSlotNames.Length)
                {
                    for (int i = 0; i < InputNodeSlotNames.Length; i++)
                    {
                        if (InputNodeSlotNames[i] != OldInputNodeSlotNames[i])
                        {
                            propertyChanged = true;
                            break;
                        }
                    }
                    if (OldOutputNodeSlotNames.Length == OutputNodeSlotNames.Length)
                    {
                        for (int i = 0; i < OutputNodeSlotNames.Length; i++)
                        {
                            if (OutputNodeSlotNames[i] != OldOutputNodeSlotNames[i])
                            {
                                propertyChanged = true;
                                break;
                            }
                        }
                    }
                    else
                        propertyChanged = true;
                }
                else
                    propertyChanged = true;
            }
            else
                propertyChanged = true;
            */
            //if (propertyChanged || graphMenuPath != oldgraphMenuPath)
            // {
            OldInputNodeSlotNames = InputNodeSlotNames;
            OldOutputNodeSlotNames = OutputNodeSlotNames;
            File.WriteAllText(subgraphPath, GenerateSubgraph(AssetDatabase.AssetPathToGUID(hlslNewPath), Path.GetFileNameWithoutExtension(newPath), graphMenuPath, InputNodeSlotNames, OutputNodeSlotNames));
            AssetDatabase.ImportAsset(newPath);
            //}
            oldgraphMenuPath = graphMenuPath;
        }

        string graphObjectID;
        string categoryObjectID;
        string[] nodeObjectID;
        string[] specifiedPropertyGUIDs;

        string GenerateSubgraph(string customFunctionGUID, string customFunctionName, string graphPath = "Blender Nodes Graph", string[] InputSlots = null, string[] OutputSlots = null)
        {
            string graphJson = "";
            string json = "";
            GraphDataClass myClass = new GraphDataClass();
            List<DataIDs> allNodes = new List<DataIDs>();

            string[] InputSpaces = new string[] { "POS_vector3", "PVS_vector3", "PWS_vector3",
                "NOS_vector3" , "NVS_vector3", "NWS_vector3", "NTS_vector3",
                "TWS_vector3", "BTWS_vector3",
                "UV_vector3", "SP_vector3", "VVS_vector3", "VWS_vector3"};

            if (InputSlots == null) InputSlots = new string[] { };
            if (OutputSlots == null) OutputSlots = new string[] { };

            DataIDs[] PropertyIDs = new DataIDs[InputSlots.Length];
            DataIDs[] PropertyNodeIDs = new DataIDs[InputSlots.Length];
            for (int i = 0; i < InputSlots.Length; i++)
            {
                PropertyIDs[i] = new DataIDs() { m_Id = GetRandomGUID(GUIDLength) };
                PropertyNodeIDs[i] = new DataIDs() { m_Id = GetRandomGUID(GUIDLength) };

                //Debug.Log(GetSpecifiedGUID(InputSlots[i]));
                json += PropertyClassData(InputSlots[i].Split('_').Last(), PropertyIDs[i].m_Id, GetSpecifiedGUID(InputSlots[i]), InputSlots[i].Split('_').First());
                json += "\n\n";
                json += CreateNodesData("PropertyNode", null, new string[1] { InputSlots[i] }, PropertyIDs[i], nodeID: PropertyNodeIDs[i].m_Id, size: new Rect(-600, i * 30 + 230, 0, 0));
                json += "\n\n";
            }
            json += CreateCategoryData(PropertyIDs);
            json += "\n\n";
            myClass.m_ObjectId = GetRandomGUID(GUIDLength);
            myClass.m_Properties = PropertyIDs;
            myClass.m_CategoryData = new DataIDs[1] { new DataIDs() { m_Id = categoryObjectID } };

            List<string> RGBCurveTextures = new List<string>();
            List<string> RGBCurveTexturesGUID = new List<string>();
            for (int i = 0; i < TextureCurvePaths.Count(); i++)
            {
                AssetDatabase.DeleteAsset(TextureCurvePaths[i]);
            }

            TextureCurvePaths.Clear();
            for (int i = 0; i < nodes.Count(); i++)
            {
                if (nodes[i].GetType() == typeof(RGBCurves))
                {
                    string RGBCurvePath = newFilePath.Remove(newFilePath.Length - (name + ".asset").Length) + "Includes/";
                    string RGBCurvePathName = hlslFileName + "_" + Mathf.Abs(nodes[i].GetInstanceID()).ToString() + "_Curve";
                    RGBCurvePath += Path.ChangeExtension(RGBCurvePathName, ".png");
                    //Debug.Log(RGBCurvePath);
                    //Debug.Log((nodes[i] as RGBCurves).curveC.Evaluate(0));
                    Texture2D curveTexture = new Texture2D(1024, 1, TextureFormat.RGBA32, false, true);
                    Color color;
                    for (int index = 0; index < 1024; index++)
                    {
                        float t = index / 1023f;
                        color = new Color((nodes[i] as RGBCurves).curveR.Evaluate(t),
                            (nodes[i] as RGBCurves).curveG.Evaluate(t),
                            (nodes[i] as RGBCurves).curveB.Evaluate(t),
                            (nodes[i] as RGBCurves).curveC.Evaluate(t));
                        curveTexture.SetPixel(index, 1, color);
                    }
                    curveTexture.Apply();

                    File.WriteAllBytes(RGBCurvePath, curveTexture.EncodeToPNG());
                    string newPath = "Assets" + RGBCurvePath.Substring(Application.dataPath.Length);
                    AssetDatabase.ImportAsset(newPath);

                    TextureImporter txtImp = (TextureImporter)AssetImporter.GetAtPath(newPath);
                    txtImp.sRGBTexture = false;
                    txtImp.SetPlatformTextureSettings(new TextureImporterPlatformSettings() { resizeAlgorithm = TextureResizeAlgorithm.Bilinear });
                    txtImp.filterMode = FilterMode.Point;
                    txtImp.textureCompression = TextureImporterCompression.Uncompressed;
                    txtImp.maxTextureSize = 1024;
                    txtImp.mipmapEnabled = false;
                    AssetDatabase.ImportAsset(newPath);
                    //Debug.Log(AssetDatabase.AssetPathToGUID(newPath));
                    RGBCurveTextures.Add("Curve" + Mathf.Abs(nodes[i].GetInstanceID()).ToString() + "_Texture2D");
                    RGBCurveTexturesGUID.Add(AssetDatabase.AssetPathToGUID(newPath));
                    TextureCurvePaths.Add(newPath);
                }
                if (nodes[i].GetType() == typeof(ColorRamp))
                {
                    string RGBCurvePath = newFilePath.Remove(newFilePath.Length - (name + ".asset").Length) + "Includes/";
                    string RGBCurvePathName = hlslFileName + "_" + Mathf.Abs(nodes[i].GetInstanceID()).ToString() + "_Gradient";
                    RGBCurvePath += Path.ChangeExtension(RGBCurvePathName, ".png");
                    //Debug.Log(RGBCurvePath);
                    //Debug.Log((nodes[i] as RGBCurves).curveC.Evaluate(0));
                    Texture2D curveTexture = new Texture2D(1024, 1, TextureFormat.RGBA32, false, true);
                    curveTexture = (nodes[i] as ColorRamp).gradientInColor.GetTexture(1024);
                    curveTexture.Apply();

                    File.WriteAllBytes(RGBCurvePath, curveTexture.EncodeToPNG());
                    string newPath = "Assets" + RGBCurvePath.Substring(Application.dataPath.Length);
                    AssetDatabase.ImportAsset(newPath);

                    TextureImporter txtImp = (TextureImporter)AssetImporter.GetAtPath(newPath);
                    txtImp.sRGBTexture = true;
                    txtImp.SetPlatformTextureSettings(new TextureImporterPlatformSettings() { resizeAlgorithm = TextureResizeAlgorithm.Bilinear });
                    txtImp.filterMode = FilterMode.Point;
                    txtImp.textureCompression = TextureImporterCompression.Uncompressed;
                    txtImp.maxTextureSize = 1024;
                    txtImp.mipmapEnabled = false;
                    AssetDatabase.ImportAsset(newPath);
                    //Debug.Log(AssetDatabase.AssetPathToGUID(newPath));
                    RGBCurveTextures.Add("Gradient" + Mathf.Abs(nodes[i].GetInstanceID()).ToString() + "_Texture2D");
                    RGBCurveTexturesGUID.Add(AssetDatabase.AssetPathToGUID(newPath));
                    TextureCurvePaths.Add(newPath);
                }
                if (nodes[i].GetType() == typeof(ImageTextureNode))
                {
                    ImageTextureNode myNode = nodes[i] as ImageTextureNode;
                    if (myNode.ImageTexture != null)
                    {
                        string newPath = AssetDatabase.GetAssetPath(myNode.ImageTexture);
                        RGBCurveTextures.Add("Image" + Mathf.Abs(nodes[i].GetInstanceID()).ToString() + "_Texture2D");
                        RGBCurveTexturesGUID.Add(AssetDatabase.AssetPathToGUID(newPath));
                    }
                }
            }

            for (int i = 0; i < PropertyNodeIDs.Length; i++)
            {
                allNodes.Add(PropertyNodeIDs[i]);
            }

            string OutputNodeID = GetRandomGUID(GUIDLength);
            json += CreateNodesData("SubGraphOutputNode", InputSlots: OutputSlots, nodeID: OutputNodeID);
            json += "\n\n";
            allNodes.Add(new DataIDs() { m_Id = OutputNodeID });

            //InputSlots = InputSpaces.Concat(InputSlots).ToArray();
            string CustomFunctionID = GetRandomGUID(GUIDLength);
            json += CreateNodesData("CustomFunctionNode", InputSlots: InputSpaces.Concat(InputSlots).ToArray().Concat(RGBCurveTextures).ToArray(), OutputSlots: OutputSlots, nodeID: CustomFunctionID,
                funcName: customFunctionName, funcSource: customFunctionGUID, size: new Rect(-350, 0, 0, 0), synonyms: new string[] { "code", "HLSL" }, textureGUID: RGBCurveTexturesGUID.ToArray());
            json += "\n\n";
            allNodes.Add(new DataIDs() { m_Id = CustomFunctionID });

            string[] InputSpaceIDs = new string[InputSpaces.Length];
            for (int i = 0; i < InputSpaces.Length; i++)
            {
                InputSpaceIDs[i] = GetRandomGUID(GUIDLength);
                if (i < 3)
                {
                    json += CreateNodesData("PositionNode", OutputSlots: new string[1] { "Out" }, nodeID: InputSpaceIDs[i], size: new Rect(-850, i * 130 - 200, 0, 0), space: i);
                    json += "\n\n";
                }
                else if (i < 7)
                {
                    json += CreateNodesData("NormalVectorNode", OutputSlots: new string[1] { "Out" }, nodeID: InputSpaceIDs[i], size: new Rect(-850, i * 130 - 200, 0, 0), space: i - 3);
                    json += "\n\n";
                }
                else if (i == 7)
                {
                    json += CreateNodesData("TangentVectorNode", OutputSlots: new string[1] { "Out" }, nodeID: InputSpaceIDs[i], size: new Rect(-850, i * 130 - 200, 0, 0), space: 2);
                    json += "\n\n";
                }
                else if (i == 8)
                {
                    json += CreateNodesData("BitangentVectorNode", OutputSlots: new string[1] { "Out" }, nodeID: InputSpaceIDs[i], size: new Rect(-850, i * 130 - 200, 0, 0), space: 2);
                    json += "\n\n";
                }
                else if (i == 9)
                {
                    json += CreateNodesData("UVNode", OutputSlots: new string[1] { "Out" }, nodeID: InputSpaceIDs[i], size: new Rect(-850, i * 130 - 200, 0, 0), space: 0);
                    json += "\n\n";
                }
                else if (i == 10)
                {
                    json += CreateNodesData("ScreenPositionNode", OutputSlots: new string[1] { "Out" }, nodeID: InputSpaceIDs[i], size: new Rect(-850, i * 130 - 200, 0, 0), space: 0);
                    json += "\n\n";
                }
                else if (i == 11)
                {
                    json += CreateNodesData("ViewDirectionNode", OutputSlots: new string[1] { "Out" }, nodeID: InputSpaceIDs[i], size: new Rect(-850, i * 130 - 200, 0, 0), space: 1);
                    json += "\n\n";
                }
                else if (i == 12)
                {
                    json += CreateNodesData("ViewDirectionNode", OutputSlots: new string[1] { "Out" }, nodeID: InputSpaceIDs[i], size: new Rect(-850, i * 130 - 200, 0, 0), space: 2);
                    json += "\n\n";
                }
                allNodes.Add(new DataIDs() { m_Id = InputSpaceIDs[i] });
            }

            EdgesClass[] connectionClass = new EdgesClass[InputSpaces.Length + InputSlots.Length + OutputSlots.Length];
            for (int i = 0; i < InputSpaces.Length; i++)
            {
                connectionClass[i] = new EdgesClass()
                {
                    m_OutputSlot = new EdgesSlotsClass() { m_Node = new DataIDs() { m_Id = InputSpaceIDs[i] }, m_SlotId = 0 },
                    m_InputSlot = new EdgesSlotsClass() { m_Node = new DataIDs() { m_Id = CustomFunctionID }, m_SlotId = i }
                };
            }
            for (int i = 0; i < InputSlots.Length; i++)
            {
                connectionClass[InputSpaces.Length + i] = new EdgesClass()
                {
                    m_OutputSlot = new EdgesSlotsClass() { m_Node = PropertyNodeIDs[i], m_SlotId = 0 },
                    m_InputSlot = new EdgesSlotsClass() { m_Node = new DataIDs() { m_Id = CustomFunctionID }, m_SlotId = InputSpaces.Length + i }
                };
            }

            for (int i = 0; i < OutputSlots.Length; i++)
            {
                connectionClass[InputSlots.Length + InputSpaces.Length + i] = new EdgesClass()
                {
                    m_InputSlot = new EdgesSlotsClass() { m_Node = new DataIDs() { m_Id = OutputNodeID }, m_SlotId = i },
                    m_OutputSlot = new EdgesSlotsClass() { m_Node = new DataIDs() { m_Id = CustomFunctionID }, m_SlotId = InputSlots.Length + InputSpaces.Length + RGBCurveTextures.Count() + i }
                };
            }

            myClass.m_OutputNode = new DataIDs() { m_Id = OutputNodeID };
            myClass.m_Nodes = allNodes.ToArray();
            myClass.m_Edges = connectionClass;
            myClass.m_Path = graphPath;
#if UNITY_2021_1_OR_NEWER
            myClass.m_SGVersion = 3;
#elif UNITY_2020_1_OR_NEWER
            myClass.m_SGVersion = 2;
#endif

            graphJson = JsonUtility.ToJson(myClass, true);
            graphJson += "\n\n" + json;
            return graphJson;
        }

        string CreateCategoryData(DataIDs[] propertyList = null)
        {
            if (propertyList == null) propertyList = new DataIDs[] { };
            categoryObjectID = GetRandomGUID(GUIDLength);
            CategoryClass myClass = new CategoryClass();
            myClass.m_ObjectId = categoryObjectID;
            myClass.m_ChildObjectList = propertyList;

            string json = JsonUtility.ToJson(myClass, true);
            return json;
            /*return string.Format("\n\t{0},\n\t{1},\n\t{2},\n\t{3},\n\t{4}\n", GetNonStringValue("m_SGVersion", "0"),
                GetStringValue("m_Type", "UnityEditor.ShaderGraph.CategoryData"), GetStringValue("m_ObjectId", objectID),
                GetStringValue("m_Name", ""), GetNonStringValue("m_ChildObjectList", "[]"));*/
        }

        string CreateNodesData(string nodeType, string[] InputSlots = null, string[] OutputSlots = null, DataIDs propertyName = null, string funcName = "", string funcSource = "", string nodeID = "", Rect size = new Rect(), string[] synonyms = null, int space = 0, string[] textureGUID = null)
        {
            NodeClass myClass = new NodeClass();
            string json = "";

            if (InputSlots == null) InputSlots = new string[] { };
            if (OutputSlots == null) OutputSlots = new string[] { };
            if (textureGUID == null) textureGUID = new string[] { };

            DataIDs[] SlotsIDs = new DataIDs[InputSlots.Length + OutputSlots.Length];
            string[] InputSlotsGUID = new string[InputSlots.Length];
            for (int i = 0; i < InputSlots.Length; i++)
            {
                string SlotsTextureGUID = "";
                InputSlotsGUID[i] = GetRandomGUID(GUIDLength);
                SlotsIDs[i] = new DataIDs() { m_Id = InputSlotsGUID[i] };
                if (i >= InputSlots.Length - textureGUID.Length)
                {
                    SlotsTextureGUID = textureGUID[i - (InputSlots.Length - textureGUID.Length)];
                }
                json += CreateSlotData(InputSlots[i].Split('_').Last(), InputSlotsGUID[i], i, 0, InputSlots[i].Split('_').First(), SlotsTextureGUID);
                json += "\n\n";
            }

            string[] OutputSlotsGUID = new string[OutputSlots.Length];
            for (int i = 0; i < OutputSlots.Length; i++)
            {
                OutputSlotsGUID[i] = GetRandomGUID(GUIDLength);
                SlotsIDs[InputSlots.Length + i] = new DataIDs() { m_Id = OutputSlotsGUID[i] };
                json += CreateSlotData(OutputSlots[i].Split('_').Last(), OutputSlotsGUID[i], InputSlots.Length + i, 1, OutputSlots[i].Split('_').First());
                json += "\n\n";
            }

            myClass.m_ObjectId = nodeID;
            myClass.m_Type = "UnityEditor.ShaderGraph." + nodeType;
            myClass.m_Name = nodeType;
            myClass.m_DrawState = new DrawState() { m_Position = new PositionClass() { x = size.x, y = size.y, height = size.height, width = size.width } };
            myClass.m_Slots = SlotsIDs;

            if (propertyName == null) propertyName = new DataIDs() { };
            myClass.m_Property = propertyName;
            myClass.m_FunctionName = funcName;
            myClass.m_FunctionSource = funcSource;

            if (synonyms == null) synonyms = new string[] { };
            myClass.synonyms = synonyms;
            myClass.m_Space = space;
            myClass.m_ScreenSpaceType = space;
            myClass.m_OutputChannel = space;

            json += JsonUtility.ToJson(myClass, true);
            return json;
        }

        string CreateSlotData(string myType, string objectID, int id, int slotType, string slotName, string textureGUID = "")
        {
            SlotClass myClass = new SlotClass();
            myClass.m_ObjectId = objectID;
            myClass.m_Id = id;
            myClass.m_SlotType = slotType;
            myClass.m_DisplayName = slotName;
            myClass.m_ShaderOutputName = slotName;

            if (textureGUID == "")
                myClass.m_Texture = new SerializedTextureGUID { m_SerializedTexture = "" };
            else
                myClass.m_Texture = new SerializedTextureGUID { m_SerializedTexture = "{\"texture\":{\"fileID\":2800000,\"guid\":\"" + textureGUID + "\",\"type\":3}}" };

            switch (myType)
            {
                case "vector4":
                default:
                    myClass.m_Type = "UnityEditor.ShaderGraph.Vector4MaterialSlot";
                    break;
                case "float":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Vector1MaterialSlot";
                    break;
                case "uv":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Vector2MaterialSlot";
                    break;
                case "vector2":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Vector2MaterialSlot";
                    break;
                case "vector3":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Vector3MaterialSlot";
                    break;
                case "pos":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Vector3MaterialSlot";
                    break;
                case "Texture2D":
                    if (textureGUID == "")
                        myClass.m_Type = "UnityEditor.ShaderGraph.Texture2DMaterialSlot";
                    else
                        myClass.m_Type = "UnityEditor.ShaderGraph.Texture2DInputMaterialSlot";
                    break;
            }
            return JsonUtility.ToJson(myClass, true);
        }

        string PropertyClassData(string myType, string objectID, string guid, string propertyName)
        {
            PropertyClass myClass = new PropertyClass();
            myClass.m_ObjectId = objectID;
            myClass.m_Guid = new GUIDs() { m_GuidSerialized = guid };
            myClass.m_Name = propertyName;
            myClass.m_RefNameGeneratedByDisplayName = propertyName;
            myClass.m_DefaultReferenceName = "_" + propertyName;
            switch (myType)
            {
                case "vector4":
                default:
                    myClass.m_Type = "UnityEditor.ShaderGraph.Internal.Vector4ShaderProperty";
                    break;
                case "float":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Internal.Vector1ShaderProperty";
                    break;
                case "vector2":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Internal.Vector2ShaderProperty";
                    break;
                case "uv":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Internal.Vector2ShaderProperty";
                    myClass.m_UseCustomSlotLabel = true;
                    myClass.m_CustomSlotLabel = "UV";
                    break;
                case "vector3":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Internal.Vector3ShaderProperty";
                    break;
                case "pos":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Internal.Vector3ShaderProperty";
                    myClass.m_UseCustomSlotLabel = true;
                    myClass.m_CustomSlotLabel = "Object Space";
                    break;
                case "Texture2D":
                    myClass.m_Type = "UnityEditor.ShaderGraph.Internal.Texture2DShaderProperty";
                    break;
            }
            return JsonUtility.ToJson(myClass, true);
        }

        string GetRandomGUID(int charLenght)
        {
            const string glyphs = "abcdef0123456789";
            string myString = "";
            for (int i = 0; i < charLenght; i++)
            {
                myString += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
            }
            return myString;
        }

        string GetSpecifiedGUID(string s)
        {
            int value = 0;
            for (int i = 0; i < s.Length; i++)
            {
                value += (byte)s[i];
            }
            string fi = value.ToString()[value.ToString().Length - 2].ToString();
            string lt = value.ToString()[1].ToString();
            string le = (((byte)value.ToString().Last()) + ((byte)value.ToString()[0])).ToString()[0].ToString();
            return string.Format("{0}-{1}-{2}-{3}-{4}", "8" + fi + le + lt + "8" + fi + fi + lt, fi + lt + fi + le,
                lt + lt + lt + fi, "4" + "a" + fi + lt,
                "4a2b" + fi + "4c5d" + lt + le + "1");
        }

        private void OnEnable()
        {
            /*List<NodePort> dynamicMaterialOutputList = outNode.DynamicInputs.ToList();
            List<NodePort> dynamicMaterialInputList = inNode.DynamicOutputs.ToList();

            string[] OutputNodeSlotNames = new string[dynamicMaterialOutputList.Count];
            for (int i = 0; i < dynamicMaterialOutputList.Count; i++)
            {
                OutputNodeSlotNames[i] = dynamicMaterialOutputList[i].fieldName;
            }

            string[] InputNodeSlotNames = new string[dynamicMaterialInputList.Count];
            for (int i = 0; i < dynamicMaterialInputList.Count; i++)
            {
                InputNodeSlotNames[i] = dynamicMaterialInputList[i].fieldName;
            }

            OldInputNodeSlotNames = InputNodeSlotNames;
            OldOutputNodeSlotNames = OutputNodeSlotNames;*/
            isEnable = true;
            //SetSubAssetHideFlags(HideFlags.HideInHierarchy);
        }

        private void OnValidate()
        {
            //Debug.Log("ReturnValue");
            isEnable = true;
        }

        public void SetSubAssetHideFlags(HideFlags flag)
        {
            //Debug.Log(this);
            UnityEngine.Object[] os = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            foreach (UnityEngine.Object o in os)
            {
                //Debug.Log(o.GetType());
                if (o.GetType() != typeof(MaterialNodeGraph))
                {
                    o.hideFlags = flag;
                    EditorUtility.SetDirty(o);
                }
            }
            AssetDatabase.SaveAssets();
        }
    }

    [CustomNodeGraphEditor(typeof(MaterialNodeGraph))]
    public class BlenderNodeGraphEditor : NodeGraphEditor
    {
        MaterialNodeGraph bng;

        bool option = false;
        public override void OnGUI()
        {
            if (bng == null) bng = target as MaterialNodeGraph;
            if (bng.isEnable)
            {
                bng.isEnable = false;
                RealtimeUpdate();
            }
            bng.mousePos = Event.current.mousePosition;
            string[] toolbarSettings = { "Test" };
            serializedObject.Update();
            base.OnGUI();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawToolStrip();
            // buttons and stuff go here.
            EditorGUILayout.EndHorizontal();
            if (Selection.objects.Length == 1 && Selection.activeObject is Node)
            {
                Node node = Selection.activeObject as Node;
                NodeEditorWindow.current.MoveNodeToTop(node);
            }
            serializedObject.ApplyModifiedProperties();
        }
        void DrawToolStrip()
        {
            if (bng == null) bng = target as MaterialNodeGraph;
            if (GUILayout.Button("Generate Subgraph", EditorStyles.toolbarButton))
            {
                OnMenu_Create();
                EditorGUIUtility.ExitGUI();
            }

            GUILayout.Space(5);

            /*if (NodeEditorPreferences.GetSettings().dMode)
                option = GUILayout.Toggle(option, "Realtime Update (Beta)", EditorStyles.toolbarButton);*/

            if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(bng)));
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Import Node", EditorStyles.toolbarButton))
            {
                NodeJSONImporter.ImportJson(this);
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Preferences", EditorStyles.toolbarButton))
            {
                NodeEditorReflection.OpenPreferences();
            }
        }

        void RealtimeUpdate()
        {
            if (bng == null) bng = target as MaterialNodeGraph;
            //if (EditorWindow.mouseOverWindow.GetType() == typeof(NodeEditorWindow))
            //{            
            if (option && !NodeEditorWindow.isDraggingValue)
            {
                if (bng.newFilePath == "" || bng.newFilePath == null)
                {

                }
                else
                {
                    if (!EditorGUIUtility.editingTextField)
                        bng.GenerateCode(bng.newFilePath);
                }
            }
            //}
        }

        void OnMenu_Create()
        {
            if (bng == null) bng = target as MaterialNodeGraph;
            bng.openGenerateHLSLSaveMenu(false);
            // Do something!
        }
        void OnTools_Help()
        {
            Help.BrowseURL("https://github.com/Warwlock/blender-nodes-subgraph/wiki/");
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            if (bng == null) bng = target as MaterialNodeGraph;
            Vector2 mousePos = Event.current.mousePosition;
            Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);

            menu.AddItem(new GUIContent("Add Node"), false, () => { OpenNewNodeMenu(mousePos); });
            menu.AddSeparator("");

            /*var nodeTypes = NodeEditorReflection.nodeTypes.OrderBy(type => GetNodeMenuOrder(type)).ToArray();
            bool addNode = true;

            bool isInput = false;
            bool isOutput = false;
            Node[] gNodes = bng.nodes.ToArray();
            for (int t = 0; t < gNodes.Length; t++)
            {
                if (gNodes[t].GetType() == typeof(MaterialInput))
                    isInput = true;
                if (gNodes[t].GetType() == typeof(MaterialOutput))
                    isOutput = true;
            }

            for (int i = 0; i < nodeTypes.Length; i++)
            {
                Type type = nodeTypes[i];
                addNode = true;

                //Get node context menu path
                string path = GetNodeMenuName(type);
                if (path == "Blender Nodes Graph/Material Input" && isInput) addNode = false;
                if (path == "Blender Nodes Graph/Material Output" && isOutput) addNode = false;
                if (!NodeEditorPreferences.GetSettings().dMode)
                    if (path == "Color/RGBCurves (Beta)") addNode = false;
                if (string.IsNullOrEmpty(path)) continue;

                // Check if user is allowed to add more of given node type
                XNode.Node.DisallowMultipleNodesAttribute disallowAttrib;
                bool disallowed = false;
                if (NodeEditorUtilities.GetAttrib(type, out disallowAttrib))
                {
                    int typeCount = target.nodes.Count(x => x.GetType() == type);
                    if (typeCount >= disallowAttrib.max) disallowed = true;
                }
                // Add node entry to context menu
                if (path == "Blender Nodes Graph/Material Input") path = "Material Input";
                if (path == "Blender Nodes Graph/Material Output") path = "Material Output";
                if (disallowed) menu.AddItem(new GUIContent(path), false, null);
                else if (addNode) menu.AddItem(new GUIContent(path), false, () =>
                {
                    XNode.Node node = CreateNode(type, pos);
                    NodeEditorWindow.current.AutoConnect(node);
                });
            }
            menu.AddSeparator("");*/
            if (NodeEditorWindow.copyBuffer != null && NodeEditorWindow.copyBuffer.Length > 0) menu.AddItem(new GUIContent("Paste"), false, () => NodeEditorWindow.current.PasteNodes(pos));
            else menu.AddDisabledItem(new GUIContent("Paste"));
            menu.AddItem(new GUIContent("Preferences"), false, () => NodeEditorReflection.OpenPreferences());
            menu.AddCustomContextMenuItems(target);
        }
    }
}

[Serializable]
public class NodesData
{
    public NodeFormat[] nodeDatas;
}

[Serializable]
public class NodeFormat
{
    public string[] connections0Name;
    public string[] connections0NodeName;
    public string[] connections1Name;
    public string[] connections1NodeName;
    public string[] connections2Name;
    public string[] connections2NodeName;
    public string[] connections3Name;
    public string[] connections3NodeName;
    public string[] connections4Name;
    public string[] connections4NodeName;
    public float[] location;
    public string name;
    public string[] properties;
}