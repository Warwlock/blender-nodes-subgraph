using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;
using MaterialNodesGraph;
using System;
using System.IO;
using System.Linq;


public static class NodeJSONImporter
{
    public static void ImportJson(NodeGraphEditor editor, bool isDragDrop = false, string mainPath = "", Vector2 pos = new Vector2())
    {
        bool useSimpleNoise = true;
        bool showUnsupported = true;
        NodesData format = new NodesData();
        string path = "";
        if (isDragDrop)
        {
            path = mainPath;
        }
        else
        {
            path = EditorUtility.OpenFilePanel(
            "Open JSON File",
            "Assets/",
            "json");
        }
        if (path != null && path != "")
        {
            format = JsonUtility.FromJson<NodesData>(File.ReadAllText(path));
            List<Node> jsonNodes = new List<Node>();
            List<string> jsonNodesName = new List<string>();

            for (int i = 0; i < format.nodeDatas.Length; i++)
            {
                if (format.nodeDatas[i].name == "EXTRA")
                {
                    if (format.nodeDatas[i].properties[0] == "True")
                        useSimpleNoise = true;
                    else
                        useSimpleNoise = false;

                    if (format.nodeDatas[i].properties[1] == "True")
                        showUnsupported = true;
                    else
                        showUnsupported = false;
                }
            }

            for (int i = 0; i < format.nodeDatas.Length; i++)
            {
                if (format.nodeDatas[i].name == "EXTRA")
                {
                    jsonNodesName.Add("NULL");
                    jsonNodes.Add(null);
                    continue;
                }

                Vector2 location = new Vector2(format.nodeDatas[i].location[0] * 1.2f, -format.nodeDatas[i].location[1]);
                location += pos;

                if (format.nodeDatas[i].name.Split('.')[0] == "NULL")
                {
                    if (!showUnsupported)
                    {
                        jsonNodesName.Add("NULL");
                        jsonNodes.Add(null);
                        continue;
                    }

                    Node nnode = editor.CreateNode(typeof(StickyNote), location);
                    (nnode as StickyNote).StickyTitle = format.nodeDatas[i].name.Split('.')[2];
                    (nnode as StickyNote).StickyDetails = "Unsupported Type:\n";
                    (nnode as StickyNote).StickyDetails += format.nodeDatas[i].name.Split('.')[1];

                    jsonNodesName.Add("NULL");
                    jsonNodes.Add(nnode);
                    continue;
                }

                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeMath")
                {
                    Node nnode = editor.CreateNode(typeof(MathNode), location);
                    (nnode as MathNode).mathType = GetMathType(format.nodeDatas[i].properties[0]);

                    if (format.nodeDatas[i].properties[1] == "True")
                        (nnode as MathNode).clamp = true;
                    if (format.nodeDatas[i].properties[1] == "False")
                        (nnode as MathNode).clamp = false;

                    (nnode as MathNode).floatA = float.Parse(format.nodeDatas[i].properties[2]);
                    (nnode as MathNode).floatB = float.Parse(format.nodeDatas[i].properties[3]);
                    (nnode as MathNode).floatC = float.Parse(format.nodeDatas[i].properties[4]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexNoise")
                {
                    if (!useSimpleNoise)
                    {
                        Node nnode = editor.CreateNode(typeof(NoiseTexture), location);

                        if (format.nodeDatas[i].properties[0] == "1D")
                            (nnode as NoiseTexture).dimenType = NoiseTexture.dimensionType._1D;
                        if (format.nodeDatas[i].properties[0] == "2D")
                            (nnode as NoiseTexture).dimenType = NoiseTexture.dimensionType._2D;
                        if (format.nodeDatas[i].properties[0] == "3D")
                            (nnode as NoiseTexture).dimenType = NoiseTexture.dimensionType._3D;
                        if (format.nodeDatas[i].properties[0] == "4D")
                            (nnode as NoiseTexture).dimenType = NoiseTexture.dimensionType._4D;

                        (nnode as NoiseTexture).w = float.Parse(format.nodeDatas[i].properties[1]);
                        (nnode as NoiseTexture).fac = float.Parse(format.nodeDatas[i].properties[2]);
                        (nnode as NoiseTexture).detail = float.Parse(format.nodeDatas[i].properties[3]);
                        (nnode as NoiseTexture).rough = float.Parse(format.nodeDatas[i].properties[4]);
                        (nnode as NoiseTexture).distort = float.Parse(format.nodeDatas[i].properties[5]);

                        jsonNodesName.Add(format.nodeDatas[i].name);
                        jsonNodes.Add(nnode);
                    }
                    else
                    {
                        Node nnode = editor.CreateNode(typeof(SimpleNoiseTexture), location);

                        if (format.nodeDatas[i].properties[0] == "1D")
                            (nnode as SimpleNoiseTexture).dimenType = SimpleNoiseTexture.dimensionType._2D;
                        if (format.nodeDatas[i].properties[0] == "2D")
                            (nnode as SimpleNoiseTexture).dimenType = SimpleNoiseTexture.dimensionType._2D;
                        if (format.nodeDatas[i].properties[0] == "3D")
                            (nnode as SimpleNoiseTexture).dimenType = SimpleNoiseTexture.dimensionType._3D;
                        if (format.nodeDatas[i].properties[0] == "4D")
                            (nnode as SimpleNoiseTexture).dimenType = SimpleNoiseTexture.dimensionType._4D;

                        (nnode as SimpleNoiseTexture).w = float.Parse(format.nodeDatas[i].properties[1]);
                        (nnode as SimpleNoiseTexture).fac = float.Parse(format.nodeDatas[i].properties[2]);
                        (nnode as SimpleNoiseTexture).detail = float.Parse(format.nodeDatas[i].properties[3]);
                        (nnode as SimpleNoiseTexture).rough = float.Parse(format.nodeDatas[i].properties[4]);
                        (nnode as SimpleNoiseTexture).distort = float.Parse(format.nodeDatas[i].properties[5]);

                        jsonNodesName.Add(format.nodeDatas[i].name);
                        jsonNodes.Add(nnode);
                    }
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexChecker")
                {
                    Node nnode = editor.CreateNode(typeof(CheckerTexture), location);

                    CustomBlenderColor col1 = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[0]), float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]), float.Parse(format.nodeDatas[i].properties[3]));
                    CustomBlenderColor col2 = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[4]), float.Parse(format.nodeDatas[i].properties[5]),
                        float.Parse(format.nodeDatas[i].properties[6]), float.Parse(format.nodeDatas[i].properties[7]));

                    (nnode as CheckerTexture).color1 = col1;
                    (nnode as CheckerTexture).color2 = col2;
                    (nnode as CheckerTexture).scale = float.Parse(format.nodeDatas[i].properties[8]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexGradient")
                {
                    Node nnode = editor.CreateNode(typeof(GradientTexture), location);

                    if (format.nodeDatas[i].properties[0] == "LINEAR")
                        (nnode as GradientTexture).gradType = GradientTexture.gradientType.Linear;
                    if (format.nodeDatas[i].properties[0] == "QUADRATIC")
                        (nnode as GradientTexture).gradType = GradientTexture.gradientType.Quadratic;
                    if (format.nodeDatas[i].properties[0] == "EASING")
                        (nnode as GradientTexture).gradType = GradientTexture.gradientType.Easing;
                    if (format.nodeDatas[i].properties[0] == "DIAGONAL")
                        (nnode as GradientTexture).gradType = GradientTexture.gradientType.Diagonal;
                    if (format.nodeDatas[i].properties[0] == "SPHERICAL")
                        (nnode as GradientTexture).gradType = GradientTexture.gradientType.Spherical;
                    if (format.nodeDatas[i].properties[0] == "QUADRATIC_SPHERE")
                        (nnode as GradientTexture).gradType = GradientTexture.gradientType.Linear;
                    if (format.nodeDatas[i].properties[0] == "RADIAL")
                        (nnode as GradientTexture).gradType = GradientTexture.gradientType.Radial;

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexImage")
                {
                    Node nnode = editor.CreateNode(typeof(ImageTextureNode), location);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexMagic")
                {
                    Node nnode = editor.CreateNode(typeof(MagicTexture), location);

                    (nnode as MagicTexture).depth = int.Parse(format.nodeDatas[i].properties[0]);
                    (nnode as MagicTexture).scale = float.Parse(format.nodeDatas[i].properties[1]);
                    (nnode as MagicTexture).distortion = float.Parse(format.nodeDatas[i].properties[2]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexMusgrave")
                {
                    Node nnode = editor.CreateNode(typeof(MusgraveTexture), location);

                    if (format.nodeDatas[i].properties[0] == "1D")
                        (nnode as MusgraveTexture).dimenType = MusgraveTexture.dimensionType._1D;
                    if (format.nodeDatas[i].properties[0] == "2D")
                        (nnode as MusgraveTexture).dimenType = MusgraveTexture.dimensionType._2D;
                    if (format.nodeDatas[i].properties[0] == "3D")
                        (nnode as MusgraveTexture).dimenType = MusgraveTexture.dimensionType._3D;
                    if (format.nodeDatas[i].properties[0] == "4D")
                        (nnode as MusgraveTexture).dimenType = MusgraveTexture.dimensionType._4D;

                    if (format.nodeDatas[i].properties[1] == "MULTIFRACTAL")
                        (nnode as MusgraveTexture).typeType = MusgraveTexture.tType.Multifractal;
                    if (format.nodeDatas[i].properties[1] == "RIDGED_MULTIFRACTAL")
                        (nnode as MusgraveTexture).typeType = MusgraveTexture.tType.RidgedMultifractal;
                    if (format.nodeDatas[i].properties[1] == "HYBRID_MULTIFRACTAL")
                        (nnode as MusgraveTexture).typeType = MusgraveTexture.tType.HybridMultifractal;
                    if (format.nodeDatas[i].properties[1] == "FBM")
                        (nnode as MusgraveTexture).typeType = MusgraveTexture.tType.FBM;
                    if (format.nodeDatas[i].properties[1] == "HETERO_TERRAIN")
                        (nnode as MusgraveTexture).typeType = MusgraveTexture.tType.HeteroTerrain;

                    (nnode as MusgraveTexture).w = float.Parse(format.nodeDatas[i].properties[2]);
                    (nnode as MusgraveTexture).scale = float.Parse(format.nodeDatas[i].properties[3]);
                    (nnode as MusgraveTexture).detail = float.Parse(format.nodeDatas[i].properties[4]);
                    (nnode as MusgraveTexture).dimension = float.Parse(format.nodeDatas[i].properties[5]);
                    (nnode as MusgraveTexture).lac = float.Parse(format.nodeDatas[i].properties[6]);
                    (nnode as MusgraveTexture).offset = float.Parse(format.nodeDatas[i].properties[7]);
                    (nnode as MusgraveTexture).gain = float.Parse(format.nodeDatas[i].properties[8]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexVoronoi")
                {
                    Node nnode = editor.CreateNode(typeof(VoronoiTexture), location);

                    if (format.nodeDatas[i].properties[0] == "1D")
                        (nnode as VoronoiTexture).dimenType = VoronoiTexture.dimensionType._1D;
                    if (format.nodeDatas[i].properties[0] == "2D")
                        (nnode as VoronoiTexture).dimenType = VoronoiTexture.dimensionType._2D;
                    if (format.nodeDatas[i].properties[0] == "3D")
                        (nnode as VoronoiTexture).dimenType = VoronoiTexture.dimensionType._3D;
                    if (format.nodeDatas[i].properties[0] == "4D")
                        (nnode as VoronoiTexture).dimenType = VoronoiTexture.dimensionType._4D;

                    if (format.nodeDatas[i].properties[1] == "F1")
                        (nnode as VoronoiTexture).fType = VoronoiTexture.featureType.F1;
                    if (format.nodeDatas[i].properties[1] == "F2")
                        (nnode as VoronoiTexture).fType = VoronoiTexture.featureType.F2;
                    if (format.nodeDatas[i].properties[1] == "SMOOTH_F1")
                        (nnode as VoronoiTexture).fType = VoronoiTexture.featureType.SmoothF1;
                    if (format.nodeDatas[i].properties[1] == "DISTANCE_TO_EDGE")
                        (nnode as VoronoiTexture).fType = VoronoiTexture.featureType.DistanceToEdge;
                    if (format.nodeDatas[i].properties[1] == "N_SPHERE_RADIUS")
                        (nnode as VoronoiTexture).fType = VoronoiTexture.featureType.NSphereRadius;

                    if (format.nodeDatas[i].properties[2] == "EUCLIDEAN")
                        (nnode as VoronoiTexture).mType = VoronoiTexture.metricType.Euclidean;
                    if (format.nodeDatas[i].properties[2] == "MANHATTAN")
                        (nnode as VoronoiTexture).mType = VoronoiTexture.metricType.Manhattan;
                    if (format.nodeDatas[i].properties[2] == "CHEBYCHEV")
                        (nnode as VoronoiTexture).mType = VoronoiTexture.metricType.Chebychev;
                    if (format.nodeDatas[i].properties[2] == "MINKOWSKI")
                        (nnode as VoronoiTexture).mType = VoronoiTexture.metricType.Minkowski;

                    (nnode as VoronoiTexture).w = float.Parse(format.nodeDatas[i].properties[3]);
                    (nnode as VoronoiTexture).scale = float.Parse(format.nodeDatas[i].properties[4]);
                    (nnode as VoronoiTexture).smoothness = float.Parse(format.nodeDatas[i].properties[5]);
                    (nnode as VoronoiTexture).exponent = float.Parse(format.nodeDatas[i].properties[6]);
                    (nnode as VoronoiTexture).randomness = float.Parse(format.nodeDatas[i].properties[7]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexWave")
                {
                    Node nnode = editor.CreateNode(typeof(WaveTexture), location);

                    if (format.nodeDatas[i].properties[0] == "BANDS")
                        (nnode as WaveTexture).waveType = WaveTexture.WaveType.Bands;
                    if (format.nodeDatas[i].properties[0] == "RINGS")
                        (nnode as WaveTexture).waveType = WaveTexture.WaveType.Rings;

                    if (format.nodeDatas[i].properties[1] == "X")
                        (nnode as WaveTexture).bandsDirection = WaveTexture.BandsDirection.X;
                    if (format.nodeDatas[i].properties[1] == "Y")
                        (nnode as WaveTexture).bandsDirection = WaveTexture.BandsDirection.Y;
                    if (format.nodeDatas[i].properties[1] == "Z")
                        (nnode as WaveTexture).bandsDirection = WaveTexture.BandsDirection.Z;
                    if (format.nodeDatas[i].properties[1] == "DIAGONAL")
                        (nnode as WaveTexture).bandsDirection = WaveTexture.BandsDirection.Diagonal;

                    if (format.nodeDatas[i].properties[2] == "X")
                        (nnode as WaveTexture).ringsDirection = WaveTexture.RingsDirection.X;
                    if (format.nodeDatas[i].properties[2] == "Y")
                        (nnode as WaveTexture).ringsDirection = WaveTexture.RingsDirection.Y;
                    if (format.nodeDatas[i].properties[2] == "Z")
                        (nnode as WaveTexture).ringsDirection = WaveTexture.RingsDirection.Z;
                    if (format.nodeDatas[i].properties[2] == "SPHERICAL")
                        (nnode as WaveTexture).ringsDirection = WaveTexture.RingsDirection.Spherical;

                    if (format.nodeDatas[i].properties[3] == "SINE")
                        (nnode as WaveTexture).waveProfile = WaveTexture.WaveProfile.Sine;
                    if (format.nodeDatas[i].properties[3] == "SAW")
                        (nnode as WaveTexture).waveProfile = WaveTexture.WaveProfile.Saw;
                    if (format.nodeDatas[i].properties[3] == "TRIANGLE")
                        (nnode as WaveTexture).waveProfile = WaveTexture.WaveProfile.Triangle;

                    (nnode as WaveTexture).fac = float.Parse(format.nodeDatas[i].properties[4]);
                    (nnode as WaveTexture).dist = float.Parse(format.nodeDatas[i].properties[5]);
                    (nnode as WaveTexture).detail = float.Parse(format.nodeDatas[i].properties[6]);
                    (nnode as WaveTexture).detailScale = float.Parse(format.nodeDatas[i].properties[7]);
                    (nnode as WaveTexture).detailRough = float.Parse(format.nodeDatas[i].properties[8]);
                    (nnode as WaveTexture).phaseOffset = float.Parse(format.nodeDatas[i].properties[9]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeBrightContrast")
                {
                    Node nnode = editor.CreateNode(typeof(BrightContrast), location);

                    CustomBlenderColor col = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[0]), float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]), float.Parse(format.nodeDatas[i].properties[3]));

                    (nnode as BrightContrast).colorA = col;
                    (nnode as BrightContrast).floatB = float.Parse(format.nodeDatas[i].properties[4]);
                    (nnode as BrightContrast).floatC = float.Parse(format.nodeDatas[i].properties[5]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeGamma")
                {
                    Node nnode = editor.CreateNode(typeof(Gamma), location);

                    CustomBlenderColor col = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[0]), float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]), float.Parse(format.nodeDatas[i].properties[3]));

                    (nnode as Gamma).color = col;
                    (nnode as Gamma).gamma = float.Parse(format.nodeDatas[i].properties[4]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeHueSaturation")
                {
                    Node nnode = editor.CreateNode(typeof(HueSaturationValue), location);

                    CustomBlenderColor col = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[4]), float.Parse(format.nodeDatas[i].properties[5]),
                        float.Parse(format.nodeDatas[i].properties[6]), float.Parse(format.nodeDatas[i].properties[7]));

                    (nnode as HueSaturationValue).floatHue = float.Parse(format.nodeDatas[i].properties[0]);
                    (nnode as HueSaturationValue).floatSat = float.Parse(format.nodeDatas[i].properties[1]);
                    (nnode as HueSaturationValue).floatVal = float.Parse(format.nodeDatas[i].properties[2]);
                    (nnode as HueSaturationValue).floatFac = float.Parse(format.nodeDatas[i].properties[3]);
                    (nnode as HueSaturationValue).colorIn = col;

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeInvert")
                {
                    Node nnode = editor.CreateNode(typeof(Invert), location);

                    CustomBlenderColor col = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[1]), float.Parse(format.nodeDatas[i].properties[2]),
                        float.Parse(format.nodeDatas[i].properties[3]), float.Parse(format.nodeDatas[i].properties[4]));

                    (nnode as Invert).fac = float.Parse(format.nodeDatas[i].properties[0]);
                    (nnode as Invert).color = col;

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeMixRGB")
                {
                    Node nnode = editor.CreateNode(typeof(MixRGB), location);
                    (nnode as MixRGB).blendType = GetBlendType(format.nodeDatas[i].properties[0]);

                    if (format.nodeDatas[i].properties[1] == "True")
                        (nnode as MixRGB).clamp = true;
                    if (format.nodeDatas[i].properties[1] == "False")
                        (nnode as MixRGB).clamp = false;

                    CustomBlenderColor col1 = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[3]), float.Parse(format.nodeDatas[i].properties[4]),
                        float.Parse(format.nodeDatas[i].properties[5]), float.Parse(format.nodeDatas[i].properties[6]));

                    CustomBlenderColor col2 = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[7]), float.Parse(format.nodeDatas[i].properties[8]),
                        float.Parse(format.nodeDatas[i].properties[9]), float.Parse(format.nodeDatas[i].properties[10]));

                    (nnode as MixRGB).fac = float.Parse(format.nodeDatas[i].properties[2]);
                    (nnode as MixRGB).color1 = col1;
                    (nnode as MixRGB).color2 = col2;

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeRGBCurve")
                {
                    Node nnode = editor.CreateNode(typeof(RGBCurves), location);

                    int cLength = format.nodeDatas[i].properties[0].Split('|').Length - 1;
                    int rLength = format.nodeDatas[i].properties[1].Split('|').Length - 1;
                    int gLength = format.nodeDatas[i].properties[2].Split('|').Length - 1;
                    int bLength = format.nodeDatas[i].properties[3].Split('|').Length - 1;

                    Keyframe[] cKeys = new Keyframe[cLength];
                    Keyframe[] rKeys = new Keyframe[rLength];
                    Keyframe[] gKeys = new Keyframe[gLength];
                    Keyframe[] bKeys = new Keyframe[bLength];

                    for(int setPoints = 0; setPoints < cLength; setPoints++)
                    {
                        cKeys[setPoints] = new Keyframe(
                            float.Parse(format.nodeDatas[i].properties[0].Split('|')[setPoints].Split('-')[0]),
                            float.Parse(format.nodeDatas[i].properties[0].Split('|')[setPoints].Split('-')[1])
                            );
                    }
                    for (int setPoints = 0; setPoints < rLength; setPoints++)
                    {
                        rKeys[setPoints] = new Keyframe(
                            float.Parse(format.nodeDatas[i].properties[1].Split('|')[setPoints].Split('-')[0]),
                            float.Parse(format.nodeDatas[i].properties[1].Split('|')[setPoints].Split('-')[1])
                            );
                    }
                    for (int setPoints = 0; setPoints < gLength; setPoints++)
                    {
                        gKeys[setPoints] = new Keyframe(
                            float.Parse(format.nodeDatas[i].properties[2].Split('|')[setPoints].Split('-')[0]),
                            float.Parse(format.nodeDatas[i].properties[2].Split('|')[setPoints].Split('-')[1])
                            );
                    }
                    for (int setPoints = 0; setPoints < bLength; setPoints++)
                    {
                        bKeys[setPoints] = new Keyframe(
                            float.Parse(format.nodeDatas[i].properties[3].Split('|')[setPoints].Split('-')[0]),
                            float.Parse(format.nodeDatas[i].properties[3].Split('|')[setPoints].Split('-')[1])
                            );
                    }

                    (nnode as RGBCurves).curveC = new AnimationCurve(cKeys);
                    (nnode as RGBCurves).curveR = new AnimationCurve(rKeys);
                    (nnode as RGBCurves).curveG = new AnimationCurve(gKeys);
                    (nnode as RGBCurves).curveB = new AnimationCurve(bKeys);

                    for (int index = 0; index < cLength; index++)
                    {
                        AnimationUtility.SetKeyLeftTangentMode((nnode as RGBCurves).curveC, index, AnimationUtility.TangentMode.ClampedAuto);
                        AnimationUtility.SetKeyRightTangentMode((nnode as RGBCurves).curveC, index, AnimationUtility.TangentMode.ClampedAuto);
                        AnimationUtility.SetKeyBroken((nnode as RGBCurves).curveC, index, false);
                    }
                    for (int index = 0; index < rLength; index++)
                    {
                        AnimationUtility.SetKeyLeftTangentMode((nnode as RGBCurves).curveR, index, AnimationUtility.TangentMode.ClampedAuto);
                        AnimationUtility.SetKeyRightTangentMode((nnode as RGBCurves).curveR, index, AnimationUtility.TangentMode.ClampedAuto);
                        AnimationUtility.SetKeyBroken((nnode as RGBCurves).curveR, index, false);
                    }
                    for (int index = 0; index < gLength; index++)
                    {
                        AnimationUtility.SetKeyLeftTangentMode((nnode as RGBCurves).curveG, index, AnimationUtility.TangentMode.ClampedAuto);
                        AnimationUtility.SetKeyRightTangentMode((nnode as RGBCurves).curveG, index, AnimationUtility.TangentMode.ClampedAuto);
                        AnimationUtility.SetKeyBroken((nnode as RGBCurves).curveG, index, false);
                    }
                    for (int index = 0; index < bLength; index++)
                    {
                        AnimationUtility.SetKeyLeftTangentMode((nnode as RGBCurves).curveB, index, AnimationUtility.TangentMode.ClampedAuto);
                        AnimationUtility.SetKeyRightTangentMode((nnode as RGBCurves).curveB, index, AnimationUtility.TangentMode.ClampedAuto);
                        AnimationUtility.SetKeyBroken((nnode as RGBCurves).curveB, index, false);
                    }

                    Color col = new Color(float.Parse(format.nodeDatas[i].properties[5]), float.Parse(format.nodeDatas[i].properties[6]),
                        float.Parse(format.nodeDatas[i].properties[7]), float.Parse(format.nodeDatas[i].properties[8]));

                    (nnode as RGBCurves).fac = float.Parse(format.nodeDatas[i].properties[4]);
                    (nnode as RGBCurves).color = new CustomBlenderColor(col);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeBump")
                {
                    Node nnode = editor.CreateNode(typeof(Bump), location);

                    if (format.nodeDatas[i].properties[0] == "True")
                        (nnode as Bump).invert = true;
                    if (format.nodeDatas[i].properties[0] == "False")
                        (nnode as Bump).invert = false;

                    (nnode as Bump).strength = float.Parse(format.nodeDatas[i].properties[1]);
                    (nnode as Bump).dist = float.Parse(format.nodeDatas[i].properties[2]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeMapping")
                {
                    Node nnode = editor.CreateNode(typeof(Mapping), location);

                    if (format.nodeDatas[i].properties[0] == "POINT")
                        (nnode as Mapping).vecType = Mapping.VectorType.Point;
                    if (format.nodeDatas[i].properties[0] == "TEXTURE")
                        (nnode as Mapping).vecType = Mapping.VectorType.Texture;
                    if (format.nodeDatas[i].properties[0] == "VECTOR")
                        (nnode as Mapping).vecType = Mapping.VectorType.Vector;
                    if (format.nodeDatas[i].properties[0] == "NORMAL")
                        (nnode as Mapping).vecType = Mapping.VectorType.Normal;

                    Vector3 vec = new Vector3(float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]),
                        float.Parse(format.nodeDatas[i].properties[3]));

                    Vector3 loc = new Vector3(float.Parse(format.nodeDatas[i].properties[4]), 
                        float.Parse(format.nodeDatas[i].properties[5]),
                        float.Parse(format.nodeDatas[i].properties[6]));

                    Vector3 rot = new Vector3(Mathf.Rad2Deg * float.Parse(format.nodeDatas[i].properties[7]),
                        Mathf.Rad2Deg * float.Parse(format.nodeDatas[i].properties[8]),
                        Mathf.Rad2Deg * float.Parse(format.nodeDatas[i].properties[9]));

                    Vector3 scl = new Vector3(float.Parse(format.nodeDatas[i].properties[10]),
                        float.Parse(format.nodeDatas[i].properties[11]),
                        float.Parse(format.nodeDatas[i].properties[12]));

                    (nnode as Mapping).vector = vec;
                    (nnode as Mapping).location = loc;
                    (nnode as Mapping).rotation = rot;
                    (nnode as Mapping).scale = scl;

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeNormalMap")
                {
                    Node nnode = editor.CreateNode(typeof(NormalMap), location);

                    (nnode as NormalMap).fac = float.Parse(format.nodeDatas[i].properties[0]);
                    (nnode as NormalMap).color = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]),
                        float.Parse(format.nodeDatas[i].properties[3]),
                        float.Parse(format.nodeDatas[i].properties[4]));

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeValToRGB")
                {
                    Node nnode = editor.CreateNode(typeof(ColorRamp), location);

                    (nnode as ColorRamp).gradientInColor.ClearAllKeys();

                    (nnode as ColorRamp).floatFac = float.Parse(format.nodeDatas[i].properties[4]);

                    if (format.nodeDatas[i].properties[1] == "RGB")
                        (nnode as ColorRamp).gradientInColor.ColorMode = CustomBlenderGradient.colorModeEnum.RGB;
                    if (format.nodeDatas[i].properties[1] == "HSV" || format.nodeDatas[i].properties[1] == "HSL")
                        (nnode as ColorRamp).gradientInColor.ColorMode = CustomBlenderGradient.colorModeEnum.HSV;

                    if (format.nodeDatas[i].properties[2] == "EASE")
                        (nnode as ColorRamp).gradientInColor.Interpolation = CustomBlenderGradient.interpolationEnum.Ease;
                    else if (format.nodeDatas[i].properties[2] == "LINEAR")
                        (nnode as ColorRamp).gradientInColor.Interpolation = CustomBlenderGradient.interpolationEnum.Linear;
                    else if (format.nodeDatas[i].properties[2] == "CONSTANT")
                        (nnode as ColorRamp).gradientInColor.Interpolation = CustomBlenderGradient.interpolationEnum.Constant;
                    else
                        (nnode as ColorRamp).gradientInColor.Interpolation = CustomBlenderGradient.interpolationEnum.Linear;

                    if (format.nodeDatas[i].properties[3] == "FAR")
                        (nnode as ColorRamp).gradientInColor.InterpolationHSV = CustomBlenderGradient.interpolationHSVEnum.Far;
                    if (format.nodeDatas[i].properties[3] == "NEAR")
                        (nnode as ColorRamp).gradientInColor.InterpolationHSV = CustomBlenderGradient.interpolationHSVEnum.Near;
                    if (format.nodeDatas[i].properties[3] == "CW")
                        (nnode as ColorRamp).gradientInColor.InterpolationHSV = CustomBlenderGradient.interpolationHSVEnum.Clockwise;
                    if (format.nodeDatas[i].properties[3] == "CCW")
                        (nnode as ColorRamp).gradientInColor.InterpolationHSV = CustomBlenderGradient.interpolationHSVEnum.CounterClockwise;

                    (nnode as ColorRamp).gradientInColor.RemoveKey(1);

                    int keyLength = format.nodeDatas[i].properties[0].Split('|').Length - 1;
                    Color[] colorKeys = new Color[keyLength];
                    float[] posKeys = new float[keyLength];

                    float r, g, b, a, p;

                    for (int ss = 0; ss < keyLength; ss++)
                    {
                        r = float.Parse(format.nodeDatas[i].properties[0].Split('|')[ss].Split('-')[0]);
                        g = float.Parse(format.nodeDatas[i].properties[0].Split('|')[ss].Split('-')[1]);
                        b = float.Parse(format.nodeDatas[i].properties[0].Split('|')[ss].Split('-')[2]);
                        a = float.Parse(format.nodeDatas[i].properties[0].Split('|')[ss].Split('-')[3]);
                        p = float.Parse(format.nodeDatas[i].properties[0].Split('|')[ss].Split('-')[4]);
                        colorKeys[ss] = new Color(r, g, b, a);
                        posKeys[ss] = p;
                    }

                    for (int ss = 0; ss < keyLength; ss++)
                    {
                        if(ss == 0)
                        {
                            (nnode as ColorRamp).gradientInColor.UpdateKeyColor(0, new CustomBlenderColor(colorKeys[0]));
                            (nnode as ColorRamp).gradientInColor.UpdateKeyTime(0, posKeys[0]);
                        }
                        else
                        {
                            (nnode as ColorRamp).gradientInColor.AddKey(new CustomBlenderColor(colorKeys[ss]), posKeys[ss]);
                        }
                    }

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeCombineHSV")
                {
                    Node nnode = editor.CreateNode(typeof(CombineHSV), location);
                    (nnode as CombineHSV).inA = float.Parse(format.nodeDatas[i].properties[0]);
                    (nnode as CombineHSV).inB = float.Parse(format.nodeDatas[i].properties[1]);
                    (nnode as CombineHSV).inC = float.Parse(format.nodeDatas[i].properties[2]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeCombineRGB")
                {
                    Node nnode = editor.CreateNode(typeof(CombineRGBA), location);
                    (nnode as CombineRGBA).inA = float.Parse(format.nodeDatas[i].properties[0]);
                    (nnode as CombineRGBA).inB = float.Parse(format.nodeDatas[i].properties[1]);
                    (nnode as CombineRGBA).inC = float.Parse(format.nodeDatas[i].properties[2]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeCombineXYZ")
                {
                    Node nnode = editor.CreateNode(typeof(CombineXYZ), location);
                    (nnode as CombineXYZ).inA = float.Parse(format.nodeDatas[i].properties[0]);
                    (nnode as CombineXYZ).inB = float.Parse(format.nodeDatas[i].properties[1]);
                    (nnode as CombineXYZ).inC = float.Parse(format.nodeDatas[i].properties[2]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeMapRange")
                {
                    Node nnode = editor.CreateNode(typeof(MapRange), location);

                    if (format.nodeDatas[i].properties[0] == "LINEAR")
                        (nnode as MapRange).mmType = MapRange.mapRangeType.Linear;
                    if (format.nodeDatas[i].properties[0] == "STEPPED")
                        (nnode as MapRange).mmType = MapRange.mapRangeType.SteppedLinear;
                    if (format.nodeDatas[i].properties[0] == "SMOOTHSTEP")
                        (nnode as MapRange).mmType = MapRange.mapRangeType.SmoothStep;
                    if (format.nodeDatas[i].properties[0] == "SMOOTHERSTEP")
                        (nnode as MapRange).mmType = MapRange.mapRangeType.SmootherStep;

                    if (format.nodeDatas[i].properties[1] == "True")
                        (nnode as MapRange).clamp = true;
                    if (format.nodeDatas[i].properties[1] == "False")
                        (nnode as MapRange).clamp = false;

                    (nnode as MapRange).value = float.Parse(format.nodeDatas[i].properties[2]);
                    (nnode as MapRange).fromMin = float.Parse(format.nodeDatas[i].properties[3]);
                    (nnode as MapRange).fromMax = float.Parse(format.nodeDatas[i].properties[4]);
                    (nnode as MapRange).toMin = float.Parse(format.nodeDatas[i].properties[5]);
                    (nnode as MapRange).toMax = float.Parse(format.nodeDatas[i].properties[6]);
                    (nnode as MapRange).steps = float.Parse(format.nodeDatas[i].properties[7]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeRGBToBW")
                {
                    Node nnode = editor.CreateNode(typeof(RGBToBW), location);

                    CustomBlenderColor col = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[0]),
                        float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]),
                        float.Parse(format.nodeDatas[i].properties[3]));

                    (nnode as RGBToBW).color = col;

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeSeparateHSV")
                {
                    Node nnode = editor.CreateNode(typeof(SeparateHSV), location);

                    CustomBlenderColor col = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[0]),
                        float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]),
                        float.Parse(format.nodeDatas[i].properties[3]));

                    (nnode as SeparateHSV).vector3A = col;

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeSeparateRGB")
                {
                    Node nnode = editor.CreateNode(typeof(SeparateRGBA), location);

                    CustomBlenderColor col = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[0]),
                        float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]),
                        float.Parse(format.nodeDatas[i].properties[3]));

                    (nnode as SeparateRGBA).vector4A = col;

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeSeparateXYZ")
                {
                    Node nnode = editor.CreateNode(typeof(SeparateXYZ), location);

                    Vector3 vec = new Vector3(float.Parse(format.nodeDatas[i].properties[0]),
                        float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]));

                    (nnode as SeparateXYZ).vector3A = vec;

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeVectorMath")
                {
                    Node nnode = editor.CreateNode(typeof(VectorMath), location);
                    (nnode as VectorMath).mathType = GetVectorMathType(format.nodeDatas[i].properties[0]);

                    Vector3 vec1 = new Vector3(float.Parse(format.nodeDatas[i].properties[1]),
                        float.Parse(format.nodeDatas[i].properties[2]),
                        float.Parse(format.nodeDatas[i].properties[3]));

                    Vector3 vec2 = new Vector3(float.Parse(format.nodeDatas[i].properties[4]),
                        float.Parse(format.nodeDatas[i].properties[5]),
                        float.Parse(format.nodeDatas[i].properties[6]));

                    Vector3 vec3 = new Vector3(float.Parse(format.nodeDatas[i].properties[7]),
                        float.Parse(format.nodeDatas[i].properties[8]),
                        float.Parse(format.nodeDatas[i].properties[9]));

                    (nnode as VectorMath).floatA = vec1;
                    (nnode as VectorMath).floatB = vec2;
                    (nnode as VectorMath).floatC = vec3;
                    (nnode as VectorMath).scale = float.Parse(format.nodeDatas[i].properties[10]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeValue")
                {
                    Node nnode = editor.CreateNode(typeof(ValueInput), location);
                    (nnode as ValueInput).floatValue = float.Parse(format.nodeDatas[i].properties[0]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeRGB")
                {
                    Node nnode = editor.CreateNode(typeof(RGBAInput), location);
                    (nnode as RGBAInput).rgbaValue = new CustomBlenderRGBA(float.Parse(format.nodeDatas[i].properties[0]),
                        float.Parse(format.nodeDatas[i].properties[1]), float.Parse(format.nodeDatas[i].properties[2]), 
                        float.Parse(format.nodeDatas[i].properties[3]));

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexBrick")
                {
                    Node nnode = editor.CreateNode(typeof(BrickTexture), location);

                    (nnode as BrickTexture).offset = float.Parse(format.nodeDatas[i].properties[0]);
                    (nnode as BrickTexture).offset_freq = int.Parse(format.nodeDatas[i].properties[1]);
                    (nnode as BrickTexture).squash = float.Parse(format.nodeDatas[i].properties[2]);
                    (nnode as BrickTexture).squash_freq = int.Parse(format.nodeDatas[i].properties[3]);

                    CustomBlenderColor col1 = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[4]), float.Parse(format.nodeDatas[i].properties[5]), 
                        float.Parse(format.nodeDatas[i].properties[6]), float.Parse(format.nodeDatas[i].properties[7]));
                    CustomBlenderColor col2 = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[8]), float.Parse(format.nodeDatas[i].properties[9]),
                        float.Parse(format.nodeDatas[i].properties[10]), float.Parse(format.nodeDatas[i].properties[11]));
                    CustomBlenderColor mortar = new CustomBlenderColor(float.Parse(format.nodeDatas[i].properties[12]), float.Parse(format.nodeDatas[i].properties[13]),
                        float.Parse(format.nodeDatas[i].properties[14]), float.Parse(format.nodeDatas[i].properties[15]));

                    (nnode as BrickTexture).color1 = col1;
                    (nnode as BrickTexture).color2 = col2;
                    (nnode as BrickTexture).mortar = mortar;

                    (nnode as BrickTexture).scale = float.Parse(format.nodeDatas[i].properties[16]);
                    (nnode as BrickTexture).mortarSize = float.Parse(format.nodeDatas[i].properties[17]);
                    (nnode as BrickTexture).mortarSmooth = float.Parse(format.nodeDatas[i].properties[18]);
                    (nnode as BrickTexture).bias = float.Parse(format.nodeDatas[i].properties[19]);
                    (nnode as BrickTexture).brickWidth = float.Parse(format.nodeDatas[i].properties[20]);
                    (nnode as BrickTexture).brickHeight = float.Parse(format.nodeDatas[i].properties[21]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeTexCoord")
                {
                    Node nnode = editor.CreateNode(typeof(TextureCoordinate), location);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeNewGeometry")
                {
                    Node nnode = editor.CreateNode(typeof(Geometry), location);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeBlackbody")
                {
                    Node nnode = editor.CreateNode(typeof(Blackbody), location);
                    (nnode as Blackbody).floatA = float.Parse(format.nodeDatas[i].properties[0]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
                //_______________________________________________________________________
                if (format.nodeDatas[i].name.Split('.')[0] == "ShaderNodeClamp")
                {
                    Node nnode = editor.CreateNode(typeof(Clamp), location);
                    (nnode as Clamp).clampType = format.nodeDatas[i].properties[0] == "MINMAX" ? Clamp.ClampType.MinMax : Clamp.ClampType.Range;
                    (nnode as Clamp).value = float.Parse(format.nodeDatas[i].properties[1]);
                    (nnode as Clamp).min = float.Parse(format.nodeDatas[i].properties[2]);
                    (nnode as Clamp).max = float.Parse(format.nodeDatas[i].properties[3]);

                    jsonNodesName.Add(format.nodeDatas[i].name);
                    jsonNodes.Add(nnode);
                }
            }

            string[] jsonNodesNameArray = jsonNodesName.ToArray();
            for (int i = 0; i < format.nodeDatas.Length; i++)
            {
                if (jsonNodes[i] == null)
                    continue;

                if (format.nodeDatas[i].connections0Name == null)
                    continue;

                NodePort[] ports;

                //0NodeName

                ports = GetPorts(format.nodeDatas[i].connections0Name, format.nodeDatas[i].connections0NodeName, jsonNodesNameArray, jsonNodes);

                for (int t = 0; t < format.nodeDatas[i].connections0Name.Length; t++)
                {
                    NodePort port = ports[t];

                    if (port == null)
                        continue;
                    if (jsonNodes[i].GetType() == typeof(BrickTexture) ||
                        jsonNodes[i].GetType() == typeof(GradientTexture) ||
                        jsonNodes[i].GetType() == typeof(CheckerTexture) ||
                        jsonNodes[i].GetType() == typeof(MagicTexture) ||
                        jsonNodes[i].GetType() == typeof(MixRGB) ||
                        jsonNodes[i].GetType() == typeof(RGBCurves))
                        jsonNodes[i].GetPort("out_color").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(VoronoiTexture))
                        jsonNodes[i].GetPort("out_distance").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(SeparateHSV))
                        jsonNodes[i].GetPort("ResultH").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(SeparateRGBA))
                        jsonNodes[i].GetPort("ResultR").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(SeparateXYZ))
                        jsonNodes[i].GetPort("ResultX").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(TextureCoordinate))
                        jsonNodes[i].GetPort("oNormal").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(Geometry))
                        jsonNodes[i].GetPort("oPosition").Connect(port);
                    else
                        jsonNodes[i].GetPort("Result").Connect(port);
                }

                //1NodeName

                if (format.nodeDatas[i].connections1Name == null)
                    continue;

                ports = GetPorts(format.nodeDatas[i].connections1Name, format.nodeDatas[i].connections1NodeName, jsonNodesNameArray, jsonNodes);

                for (int t = 0; t < format.nodeDatas[i].connections1Name.Length; t++)
                {
                    NodePort port = ports[t];

                    if (port == null)
                        continue;
                    if (jsonNodes[i].GetType() == typeof(BrickTexture) ||
                        jsonNodes[i].GetType() == typeof(GradientTexture) ||
                        jsonNodes[i].GetType() == typeof(CheckerTexture) ||
                        jsonNodes[i].GetType() == typeof(MagicTexture))
                        jsonNodes[i].GetPort("out_factor").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(VoronoiTexture))
                        jsonNodes[i].GetPort("out_color").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(SeparateHSV))
                        jsonNodes[i].GetPort("ResultS").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(SeparateRGBA))
                        jsonNodes[i].GetPort("ResultG").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(SeparateXYZ))
                        jsonNodes[i].GetPort("ResultY").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(TextureCoordinate))
                        jsonNodes[i].GetPort("oUV").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(Geometry))
                        jsonNodes[i].GetPort("oNormal").Connect(port);
                    else
                        jsonNodes[i].GetPort("Result_Col").Connect(port);
                }

                //2NodeName

                if (format.nodeDatas[i].connections2Name == null)
                    continue;

                ports = GetPorts(format.nodeDatas[i].connections2Name, format.nodeDatas[i].connections2NodeName, jsonNodesNameArray, jsonNodes);

                for (int t = 0; t < format.nodeDatas[i].connections2Name.Length; t++)
                {
                    NodePort port = ports[t];

                    if (port == null)
                        continue;
                    if (jsonNodes[i].GetType() == typeof(VoronoiTexture))
                        jsonNodes[i].GetPort("out_position").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(SeparateHSV))
                        jsonNodes[i].GetPort("ResultV").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(SeparateRGBA))
                        jsonNodes[i].GetPort("ResultB").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(SeparateXYZ))
                        jsonNodes[i].GetPort("ResultZ").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(TextureCoordinate))
                        jsonNodes[i].GetPort("oObject").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(Geometry))
                        jsonNodes[i].GetPort("oIncoming").Connect(port);
                    else
                        jsonNodes[i].GetPort("Result").Connect(port);
                }

                //3NodeName

                if (format.nodeDatas[i].connections3Name == null)
                    continue;

                ports = GetPorts(format.nodeDatas[i].connections3Name, format.nodeDatas[i].connections3NodeName, jsonNodesNameArray, jsonNodes);

                for (int t = 0; t < format.nodeDatas[i].connections3Name.Length; t++)
                {
                    NodePort port = ports[t];

                    if (port == null)
                        continue;
                    if (jsonNodes[i].GetType() == typeof(VoronoiTexture))
                        jsonNodes[i].GetPort("out_w").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(TextureCoordinate))
                        jsonNodes[i].GetPort("oCamera").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(Geometry))
                        jsonNodes[i].GetPort("oBackFacing").Connect(port);
                    else
                        jsonNodes[i].GetPort("Result").Connect(port);
                }

                //4NodeName

                if (format.nodeDatas[i].connections4Name == null)
                    continue;

                ports = GetPorts(format.nodeDatas[i].connections4Name, format.nodeDatas[i].connections4NodeName, jsonNodesNameArray, jsonNodes);

                for (int t = 0; t < format.nodeDatas[i].connections4Name.Length; t++)
                {
                    NodePort port = ports[t];

                    if (port == null)
                        continue;
                    if (jsonNodes[i].GetType() == typeof(VoronoiTexture))
                        jsonNodes[i].GetPort("out_radius").Connect(port);
                    else if (jsonNodes[i].GetType() == typeof(TextureCoordinate))
                        jsonNodes[i].GetPort("oWindow").Connect(port);
                    else
                        jsonNodes[i].GetPort("Result").Connect(port);
                }
            }
            (NodeEditorWindow.current.graph as MaterialNodeGraph).SetSubAssetHideFlags(HideFlags.HideInHierarchy);
            Selection.objects = jsonNodes.ToArray();
            NodeEditorWindow.current.Home();
            NodeEditorWindow.current.Save();
        }
        /*string[] lines = File.ReadAllLines(path);
        string fulltext = "";
        for(int i = 0; i < lines.Length; i++)
        {
            fulltext += "if (opType == \"" + lines[i] + "\")\n";
            fulltext += "return MathNode.MathType.;\n";
        }
        Debug.Log(fulltext);*/
    }

    static NodePort[] GetPorts(string[] name, string[] nodeName, string[] jsonNodesNameArray, List<Node> jsonNodes)
    {
        List<NodePort> ports = new List<NodePort>();
        for (int i = 0; i < name.Length; i++)
        {
            NodePort port = null;
            if (nodeName[i].Split('.')[0] == "ShaderNodeMath")
            {
                if (name[i] == "Value")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
                if (name[i] == "Value_001")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("b");
                if (name[i] == "Value_002")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("c");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeTexNoise")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
                if (name[i] == "W")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sW");
                if (name[i] == "Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFac");
                if (name[i] == "Detail")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDetail");
                if (name[i] == "Roughness")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sRough");
                if (name[i] == "Distortion")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDistort");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeTexChecker")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
                if (name[i] == "Color1")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor1");
                if (name[i] == "Color2")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor2");
                if (name[i] == "Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sScale");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeTexGradient")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeTexImage")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sUvInput");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeTexMagic")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
                if (name[i] == "Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sScale");
                if (name[i] == "Distortion")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDistortion");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeTexMusgrave")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
                if (name[i] == "W")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sW");
                if (name[i] == "Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sScale");
                if (name[i] == "Detail")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDetail");
                if (name[i] == "Dimension")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDimension");
                if (name[i] == "Lacunarity")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sLac");
                if (name[i] == "Offset")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sOffset");
                if (name[i] == "Gain")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sGain");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeTexVoronoi")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
                if (name[i] == "W")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sW");
                if (name[i] == "Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sScale");
                if (name[i] == "Smoothness")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sSmoothness");
                if (name[i] == "Exponent")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sExponent");
                if (name[i] == "Randomness")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sRandomness");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeTexWave")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
                if (name[i] == "Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFac");
                if (name[i] == "Distortion")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDist");
                if (name[i] == "Detail")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDetail");
                if (name[i] == "Detail Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDetailScale");
                if (name[i] == "Detail Roughness")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDetailRough");
                if (name[i] == "Phase Offset")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sPhaseOffset");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeBrightContrast")
            {
                if (name[i] == "Color")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
                if (name[i] == "Bright")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("b");
                if (name[i] == "Contrast")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("c");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeHueSaturation")
            {
                if (name[i] == "Color")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColorIn");
                if (name[i] == "Hue")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFloatHue");
                if (name[i] == "Saturation")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFloatSat");
                if (name[i] == "Value")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFloatVal");
                if (name[i] == "Fac")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFloatFac");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeGamma")
            {
                if (name[i] == "Color")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor");
                if (name[i] == "Gamma")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sGamma");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeInvert")
            {
                if (name[i] == "Color")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor");
                if (name[i] == "Fac")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFac");
            }
            if (nodeName[i].Split('.')[0] == "ShaderNodeMixRGB")
            {
                if (name[i] == "Color1")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor1");
                if (name[i] == "Color2")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor2");
                if (name[i] == "Fac")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFac");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeRGBCurve")
            {
                if (name[i] == "Color")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor");
                if (name[i] == "Fac")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFac");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeBump")
            {
                if (name[i] == "Strength")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sStrength");
                if (name[i] == "Distance")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sDist");
                if (name[i] == "Height")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sHeight");
                if (name[i] == "Normal")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeMapping")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
                if (name[i] == "Location")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sLocation");
                if (name[i] == "Rotation")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sRotation");
                if (name[i] == "Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sScale");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeNormalMap")
            {
                if (name[i] == "Strength")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFac");
                if (name[i] == "Color")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeValToRGB")
            {
                if (name[i] == "Fac")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFloatFac");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeCombineHSV")
            {
                if (name[i] == "H")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
                if (name[i] == "S")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("b");
                if (name[i] == "V")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("c");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeCombineRGB")
            {
                if (name[i] == "R")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
                if (name[i] == "G")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("b");
                if (name[i] == "B")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("c");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeCombineXYZ")
            {
                if (name[i] == "X")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
                if (name[i] == "Y")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("b");
                if (name[i] == "Z")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("c");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeMapRange")
            {
                if (name[i] == "Value")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sValue");
                if (name[i] == "From Min")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFromMin");
                if (name[i] == "From Max")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sFromMax");
                if (name[i] == "To Min")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sToMin");
                if (name[i] == "To Max")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sToMax");
                if (name[i] == "Steps")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sSteps");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeRGBToBW")
            {
                if (name[i] == "Color")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeSeparateHSV")
            {
                if (name[i] == "Color")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeSeparateRGB")
            {
                if (name[i] == "Image")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeSeparateXYZ")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeVectorMath")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
                if (name[i] == "Vector_001")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("b");
                if (name[i] == "Vector_002")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("c");
                if (name[i] == "Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("s");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeTexBrick")
            {
                if (name[i] == "Vector")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sVector");
                if (name[i] == "Color1")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor1");
                if (name[i] == "Color2")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sColor2");
                if (name[i] == "Mortar")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sMortar");
                if (name[i] == "Scale")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sScale");
                if (name[i] == "Mortar Size")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sMortarSize");
                if (name[i] == "Mortar Smooth")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sMortarSmooth");
                if (name[i] == "Bias")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sBias");
                if (name[i] == "Brick Width")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sBrickWidth");
                if (name[i] == "Row Height")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sBrickHeight");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeBlackbody")
            {
                if (name[i] == "Temperature")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("a");
            }
            //_______________________________________________________________________
            if (nodeName[i].Split('.')[0] == "ShaderNodeClamp")
            {
                if (name[i] == "Value")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sValue");
                if (name[i] == "Min")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sMin");
                if (name[i] == "Max")
                    port = jsonNodes[Array.IndexOf(jsonNodesNameArray, nodeName[i])].GetPort("sMax");
            }

            ports.Add(port);
        }

        return ports.ToArray();
    }

    static MixRGB.BlendType GetBlendType(string opType)
    {
        if (opType == "MIX")
            return MixRGB.BlendType.Mix;
        if (opType == "DARKEN")
            return MixRGB.BlendType.Darken;
        if (opType == "MULTIPLY")
            return MixRGB.BlendType.Multiply;
        if (opType == "BURN")
            return MixRGB.BlendType.ColorBurn;
        if (opType == "LIGHTEN")
            return MixRGB.BlendType.Lighten;
        if (opType == "SCREEN")
            return MixRGB.BlendType.Screen;
        if (opType == "DODGE")
            return MixRGB.BlendType.ColorDodge;
        if (opType == "ADD")
            return MixRGB.BlendType.Add;
        if (opType == "OVERLAY")
            return MixRGB.BlendType.Overlay;
        if (opType == "SOFT_LIGHT")
            return MixRGB.BlendType.SoftLight;
        if (opType == "LINEAR_LIGHT")
            return MixRGB.BlendType.LinearLight;
        if (opType == "DIFFERENCE")
            return MixRGB.BlendType.Difference;
        if (opType == "SUBTRACT")
            return MixRGB.BlendType.Subtract;
        if (opType == "DIVIDE")
            return MixRGB.BlendType.Divide;
        if (opType == "HUE")
            return MixRGB.BlendType.Hue;
        if (opType == "SATURATION")
            return MixRGB.BlendType.Saturation;
        if (opType == "COLOR")
            return MixRGB.BlendType.Color;
        if (opType == "VALUE")
            return MixRGB.BlendType.Value;
        else
            return MixRGB.BlendType.Mix;
    }

    static VectorMath.MathType GetVectorMathType(string opType)
    {
        if (opType == "ADD")
            return VectorMath.MathType.Add;
        else if (opType == "SUBTRACT")
            return VectorMath.MathType.Subtract;
        else if (opType == "MULTIPLY")
            return VectorMath.MathType.Multiply;
        else if (opType == "DIVIDE")
            return VectorMath.MathType.Divide;
        else if (opType == "MULTIPLY_ADD")
            return VectorMath.MathType.MultiplyAdd;
        else if (opType == "CROSS_PRODUCT")
            return VectorMath.MathType.CrossProduct;
        else if (opType == "PROJECT")
            return VectorMath.MathType.Project;
        else if (opType == "REFLECT")
            return VectorMath.MathType.Reflect;
        else if (opType == "REFRACT")
            return VectorMath.MathType.Refract;
        else if (opType == "FACEFORWARD")
            return VectorMath.MathType.Faceforward;
        else if (opType == "DOT_PRODUCT")
            return VectorMath.MathType.DotProduct;
        else if (opType == "DISTANCE")
            return VectorMath.MathType.Distance;
        else if (opType == "LENGTH")
            return VectorMath.MathType.Length;
        else if (opType == "SCALE")
            return VectorMath.MathType.Scale;
        else if (opType == "NORMALIZE")
            return VectorMath.MathType.Normalize;
        else if (opType == "ABSOLUTE")
            return VectorMath.MathType.Absolute;
        else if (opType == "MINIMUM")
            return VectorMath.MathType.Minimum;
        else if (opType == "MAXIMUM")
            return VectorMath.MathType.Maximum;
        else if (opType == "FLOOR")
            return VectorMath.MathType.Floor;
        else if (opType == "CEIL")
            return VectorMath.MathType.Ceil;
        else if (opType == "FRACTION")
            return VectorMath.MathType.Fraction;
        else if (opType == "MODULO")
            return VectorMath.MathType.Modulo;
        else if (opType == "WRAP")
            return VectorMath.MathType.Wrap;
        else if (opType == "SNAP")
            return VectorMath.MathType.Snap;
        else if (opType == "SINE")
            return VectorMath.MathType.Sine;
        else if (opType == "COSINE")
            return VectorMath.MathType.Cosine;
        else if (opType == "TANGENT")
            return VectorMath.MathType.Tangent;
        else
            return VectorMath.MathType.Add;
    }

    static MathNode.MathType GetMathType(string opType)
    {
        if (opType == "ADD")
            return MathNode.MathType.Add;
        else if (opType == "SUBTRACT")
            return MathNode.MathType.Subtract;
        else if (opType == "MULTIPLY")
            return MathNode.MathType.Multiply;
        else if (opType == "DIVIDE")
            return MathNode.MathType.Divide;
        else if (opType == "MULTIPLY_ADD")
            return MathNode.MathType.MultiplyAdd;
        else if (opType == "POWER")
            return MathNode.MathType.Power;
        else if (opType == "LOGARITHM")
            return MathNode.MathType.Logarithm;
        else if (opType == "SQRT")
            return MathNode.MathType.SquareRoot;
        else if (opType == "INVERSE_SQRT")
            return MathNode.MathType.InverseSquareRoot;
        else if (opType == "ABSOLUTE")
            return MathNode.MathType.Absolute;
        else if (opType == "EXPONENT")
            return MathNode.MathType.Exponent;
        else if (opType == "MINIMUM")
            return MathNode.MathType.Minimum;
        else if (opType == "MAXIMUM")
            return MathNode.MathType.Maximum;
        else if (opType == "LESS_THAN")
            return MathNode.MathType.LessThan;
        else if (opType == "GREATER_THAN")
            return MathNode.MathType.GreaterThan;
        else if (opType == "SIGN")
            return MathNode.MathType.Sign;
        else if (opType == "COMPARE")
            return MathNode.MathType.Compare;
        else if (opType == "SMOOTH_MIN")
            return MathNode.MathType.SmoothMinimum;
        else if (opType == "SMOOTH_MAX")
            return MathNode.MathType.SmoothMaximum;
        else if (opType == "ROUND")
            return MathNode.MathType.Round;
        else if (opType == "FLOOR")
            return MathNode.MathType.Floor;
        else if (opType == "CEIL")
            return MathNode.MathType.Ceil;
        else if (opType == "TRUNC")
            return MathNode.MathType.Truncate;
        else if (opType == "FRACT")
            return MathNode.MathType.Fraction;
        else if (opType == "MODULO")
            return MathNode.MathType.Modulo;
        else if (opType == "WRAP")
            return MathNode.MathType.Wrap;
        else if (opType == "SNAP")
            return MathNode.MathType.Snap;
        else if (opType == "PINGPONG")
            return MathNode.MathType.PingPong;
        else if (opType == "SINE")
            return MathNode.MathType.Sine;
        else if (opType == "COSINE")
            return MathNode.MathType.Cosine;
        else if (opType == "TANGENT")
            return MathNode.MathType.Tangent;
        else if (opType == "ARCSINE")
            return MathNode.MathType.Arcsine;
        else if (opType == "ARCCOSINE")
            return MathNode.MathType.Arccosine;
        else if (opType == "ARCTANGENT")
            return MathNode.MathType.Arctangent;
        else if (opType == "ARCTAN2")
            return MathNode.MathType.Arctan2;
        else if (opType == "SINH")
            return MathNode.MathType.HyperbolicSine;
        else if (opType == "COSH")
            return MathNode.MathType.HyperbolicCosine;
        else if (opType == "TANH")
            return MathNode.MathType.HyperbolicTangent;
        else if (opType == "RADIANS")
            return MathNode.MathType.ToRadians;
        else if (opType == "DEGREES")
            return MathNode.MathType.ToDegrees;
        else
            return MathNode.MathType.Add;
    }
}
