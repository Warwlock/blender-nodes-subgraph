using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BNGNode;
using BNGNodeEditor;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MaterialNodesGraph
{
    [NodeTint("#8E8131")]
    [CreateNodeMenu("Color/MixRGB")]
    public class MixRGB : Node
    {
        public bool clamp = false;
        [BlenderRange(0, 1)] public float fac = 0.5f;
        public CustomBlenderColor color1 = CustomBlenderColor.white;
        public CustomBlenderColor color2 = CustomBlenderColor.white;

        [Input] public string sFac;
        [Input] public string sColor1 = "";
        [Input] public string sColor2 = "";

        [Output] public string out_color;

        [NodeEnum]
        public BlendType blendType = BlendType.Mix;
        public enum BlendType
        {
            _BlendingMode, Mix, _,
            Darken, Multiply, ColorBurn, __,
            Lighten, Screen, ColorDodge, Add, _a,
            Overlay, SoftLight, LinearLight, ____,
            Difference, Subtract, Divide, _____,
            Hue, Saturation, Color, Value
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string sFac = GetInputValue<string>("sFac", fac.ToString()).Split('?').Last();
            string sColor1 = GetInputValue<string>("sColor1", this.sColor1).Split('?').Last();
            string sColor2 = GetInputValue<string>("sColor2", this.sColor2).Split('?').Last();

            string sFac_f = GetInputValue<string>("sFac", "").Split('?').First();
            string sColor1_f = GetInputValue<string>("sColor1", "").Split('?').First();
            string sColor2_f = GetInputValue<string>("sColor2", "").Split('?').First();

            this.sColor1 = string.Format("float4({0}, {1}, {2}, {3})", color1.r, color1.g, color1.b, color1.a);
            this.sColor2 = string.Format("float4({0}, {1}, {2}, {3})", color2.r, color2.g, color2.b, color2.a);

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "out_color")
            {
                if (!clamp)
                    return sFac_f + sColor1_f + sColor2_f +
                        "|float4 " + ValueID + " = " +
                        MixRGBCalculationFunctions(sFac, sColor1, sColor2) + ";?" + ValueID;
                else
                    return sFac_f + sColor1_f + sColor2_f +
                        "|float4 " + ValueID + " = " +
                        string.Format("clamp_color({0}, {1}, {2})", MixRGBCalculationFunctions(sFac, sColor1, sColor2), 0, 1) + ";?" + ValueID;
            }
            else
                return 0f;
        }

        string MixRGBCalculationFunctions(string fac, string col1, string col2)
        {
            switch (blendType)
            {
                case BlendType.Mix: default: return string.Format("mix_blend({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Add: return string.Format("mix_add({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Multiply: return string.Format("mix_mult({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Screen: return string.Format("mix_screen({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Overlay: return string.Format("mix_overlay({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Subtract: return string.Format("mix_sub({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Divide: return string.Format("mix_div({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Difference: return string.Format("mix_diff({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Darken: return string.Format("mix_dark({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Lighten: return string.Format("mix_light({0}, {1}, {2})", fac, col1, col2);
                case BlendType.ColorDodge: return string.Format("mix_dodge({0}, {1}, {2})", fac, col1, col2);
                case BlendType.ColorBurn: return string.Format("mix_burn({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Hue: return string.Format("mix_hue({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Saturation: return string.Format("mix_sat({0}, {1}, {2})", fac, col1, col2);
                case BlendType.Value: return string.Format("mix_val({0}, {1}, {2})", fac, col1, col2);
                case BlendType.SoftLight: return string.Format("mix_soft({0}, {1}, {2})", fac, col1, col2);
                case BlendType.LinearLight: return string.Format("mix_linear({0}, {1}, {2})", fac, col1, col2);
            }
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


    [CustomNodeEditor(typeof(MixRGB))]
    public class MixRGBEditor : NodeEditor
    {
        MixRGB serializedNode;
        GeneralNodePopup nodePopup = new GeneralNodePopup(Vector2.zero, new string[0], "Mix");
        Rect buttonRect;
        public override void OnBodyGUI()
        {
            if (serializedNode == null) serializedNode = target as MixRGB;
            serializedObject.Update();
            serializedNode.GetPort("out_color").nodePortType = "vector4";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("out_color"), new GUIContent("Result", ""));
            GUILayout.Space(10);
            buttonRect = new Rect(Event.current.mousePosition, Vector2.zero);

            //Undo.RecordObject(serializedNode, "Enum Change");
            if (EditorGUILayout.DropdownButton(new GUIContent(AddSpacesToSentence(serializedNode.blendType.ToString())), FocusType.Keyboard))
            {
                string[] enumNames = Enum.GetNames(typeof(MixRGB.BlendType));
                nodePopup = new GeneralNodePopup(new Vector2(190, 220), enumNames, serializedNode.blendType.ToString());
                nodePopup.OnCloseEvent += () => {
                    Undo.RecordObject(serializedNode, "Enum Change");
                    serializedNode.blendType = (MixRGB.BlendType)Enum.Parse(typeof(MixRGB.BlendType), nodePopup.EnumValue);
                    NodeEditorWindow.current.Save();
                };
                PopupWindow.Show(buttonRect, nodePopup);
            }            

            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("blendType"), new GUIContent("", ""));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("clamp"), new GUIContent("Clamp", "Limits the output to the range (0.0 to 1.0)."), null);
            SetPortBehaviour("fac", "sFac", "Fac");
            SetPortBehaviour("color1", "sColor1", "Color1", "vector4");
            SetPortBehaviour("color2", "sColor2", "Color2", "vector4");
            serializedObject.ApplyModifiedProperties();
        }

        void SetPortBehaviour(string propertyNamer, string portNamer, string guiNamer, string portType = "float")
        {
            string fieldName;
            if (serializedNode == null) serializedNode = target as MixRGB;
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