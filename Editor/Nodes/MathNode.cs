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
using System.Globalization;

namespace MaterialNodesGraph
{
    [NodeTint("#4885AB")]
    [CreateNodeMenu("Converter/Math", 0)]
    public class MathNode : Node
    {
        [BlenderSingle] public float floatA = 0.5f;
        [BlenderSingle] public float floatB = 0.5f;
        [BlenderSingle] public float floatC = 0.5f;
        [Input] public string a = "0.5";
        [Input] public string b = "0.5";
        [Input] public string c = "0.5";
        public bool clamp = false;

        [Output] public string Result;

        [NodeEnum]
        public MathType mathType = MathType.Add;
        public enum MathType
        {
            _Functions, Add, Subtract, Multiply, Divide, MultiplyAdd, _,
            Power, Logarithm, SquareRoot, InverseSquareRoot, Absolute, Exponent, _a,
            _Comparison, Minimum, Maximum, LessThan, GreaterThan, Sign, Compare, SmoothMinimum, SmoothMaximum, _b,
            _Rounding, Round, Floor, Ceil, Truncate, __, Fraction, Modulo, Wrap, Snap, PingPong, _c,
            _Trigonometric, Sine, Cosine, Tangent, ___, Arcsine, Arccosine, Arctangent, Arctan2, ____, HyperbolicSine, HyperbolicCosine, HyperbolicTangent, _d,
            _Conversion, ToRadians, ToDegrees
        }

        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", this.a);
            string b = GetInputValue<string>("b", this.b);
            string c = GetInputValue<string>("c", this.c);

            this.a = floatA.ToString(CultureInfo.InvariantCulture);
            this.b = floatB.ToString(CultureInfo.InvariantCulture);
            this.c = floatC.ToString(CultureInfo.InvariantCulture);

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                if (!clamp)
                    return GetInputValue("a", "").Split('?').First() + GetInputValue("b", "").Split('?').First() + GetInputValue("c", "").Split('?').First() + 
                        "|float " + ValueID + " = " + 
                        MathCalculationFormuls(a.Split('?').Last(), b.Split('?').Last(), c.Split('?').Last()) + ";?" + ValueID;
                else
                    return GetInputValue("a", "").Split('?').First() + GetInputValue("b", "").Split('?').First() + GetInputValue("c", "").Split('?').First() +
                        "|float " + ValueID + " = " +
                        "clamp_value(" + MathCalculationFormuls(a.Split('?').Last(), b.Split('?').Last(), c.Split('?').Last()) + ", 0, 1)" + ";?" + ValueID;
            }
            else
                return 0f;
        }

        string FuncBlockCreator()
        {
            return "";
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            if (from.node == to.node)
            {
                from.Disconnect(to);
            }
        }

        string MathCalculationFormuls(string a, string b, string c)
        {
            string valueInA = a;
            string valueInB = b;
            string valueInC = c;
            switch (mathType)
            {
                case MathType.Add: default: return stringFormatterNode("math_add", valueInA, valueInB, valueInC);
                case MathType.Subtract: return stringFormatterNode("math_subtract", valueInA, valueInB, valueInC);
                case MathType.Multiply: return stringFormatterNode("math_multiply", valueInA, valueInB, valueInC); ;
                case MathType.Divide: return stringFormatterNode("math_divide", valueInA, valueInB, valueInC);
                case MathType.Power: return stringFormatterNode("math_power", valueInA, valueInB, valueInC);
                case MathType.Logarithm: return stringFormatterNode("math_logarithm", valueInA, valueInB, valueInC);
                case MathType.SquareRoot: return stringFormatterNode("math_sqrt", valueInA, valueInB, valueInC);
                case MathType.InverseSquareRoot: return stringFormatterNode("math_inversesqrt", valueInA, valueInB, valueInC);
                case MathType.Absolute: return stringFormatterNode("math_absolute", valueInA, valueInB, valueInC);
                case MathType.ToRadians: return stringFormatterNode("math_radians", valueInA, valueInB, valueInC);
                case MathType.ToDegrees: return stringFormatterNode("math_degrees", valueInA, valueInB, valueInC);
                case MathType.Minimum: return stringFormatterNode("math_minimum", valueInA, valueInB, valueInC);
                case MathType.Maximum: return stringFormatterNode("math_maximum", valueInA, valueInB, valueInC);
                case MathType.LessThan: return stringFormatterNode("math_less_than", valueInA, valueInB, valueInC);
                case MathType.GreaterThan: return stringFormatterNode("math_greater_than", valueInA, valueInB, valueInC);
                case MathType.Round: return stringFormatterNode("math_round", valueInA, valueInB, valueInC);
                case MathType.Floor: return stringFormatterNode("math_floor", valueInA, valueInB, valueInC);
                case MathType.Ceil: return stringFormatterNode("math_ceil", valueInA, valueInB, valueInC);
                case MathType.Fraction: return stringFormatterNode("math_fraction", valueInA, valueInB, valueInC);
                case MathType.Modulo: return stringFormatterNode("math_modulo", valueInA, valueInB, valueInC);
                case MathType.Truncate: return stringFormatterNode("math_trunc", valueInA, valueInB, valueInC);
                case MathType.Snap: return stringFormatterNode("math_snap", valueInA, valueInB, valueInC);
                case MathType.PingPong: return stringFormatterNode("math_pingpong", valueInA, valueInB, valueInC);
                case MathType.Wrap: return stringFormatterNode("math_wrap", valueInA, valueInB, valueInC);
                case MathType.Sine: return stringFormatterNode("math_sine", valueInA, valueInB, valueInC);
                case MathType.Cosine: return stringFormatterNode("math_cosine", valueInA, valueInB, valueInC);
                case MathType.Tangent: return stringFormatterNode("math_tangent", valueInA, valueInB, valueInC);
                case MathType.HyperbolicSine: return stringFormatterNode("math_sinh", valueInA, valueInB, valueInC);
                case MathType.HyperbolicCosine: return stringFormatterNode("math_cosh", valueInA, valueInB, valueInC);
                case MathType.HyperbolicTangent: return stringFormatterNode("math_tanh", valueInA, valueInB, valueInC);
                case MathType.Arcsine: return stringFormatterNode("math_arcsine", valueInA, valueInB, valueInC);
                case MathType.Arccosine: return stringFormatterNode("math_arccosine", valueInA, valueInB, valueInC);
                case MathType.Arctangent: return stringFormatterNode("math_arctangent", valueInA, valueInB, valueInC);
                case MathType.Arctan2: return stringFormatterNode("math_arctan2", valueInA, valueInB, valueInC);
                case MathType.Sign: return stringFormatterNode("math_sign", valueInA, valueInB, valueInC);
                case MathType.Exponent: return stringFormatterNode("math_exponent", valueInA, valueInB, valueInC);
                case MathType.Compare: return stringFormatterNode("math_compare", valueInA, valueInB, valueInC);
                case MathType.MultiplyAdd: return stringFormatterNode("math_multiply_add", valueInA, valueInB, valueInC);
                case MathType.SmoothMinimum: return stringFormatterNode("math_smoothmin", valueInA, valueInB, valueInC);
                case MathType.SmoothMaximum: return stringFormatterNode("math_smoothmax", valueInA, valueInB, valueInC);
            }
        }

        string stringFormatterNode(string function, string inA, string inB, string inC)
        {
            return string.Format("{0}({1}, {2}, {3})",
                function, inA, inB, inC);
        }
    }

    [CustomNodeEditor(typeof(MathNode))]
    public class MathNodeEditor : NodeEditor
    {
        MathNode m;
        GeneralNodePopup nodePopup = new GeneralNodePopup(Vector2.zero, new string[0], "Add");
        Rect buttonRect;
        public override void OnBodyGUI()
        {
            if (m == null) m = target as MathNode;
            serializedObject.Update();
            m.GetInputPort("a").connectionType = Node.ConnectionType.Override;
            m.GetInputPort("b").connectionType = Node.ConnectionType.Override;
            m.GetInputPort("c").connectionType = Node.ConnectionType.Override;
            NodePort myPort = m.GetPort("Result");
            myPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Value", ""), myPort);
            GUILayout.Space(10);
            
            //Undo.RecordObject(m, "Enum Change");
            if (EditorGUILayout.DropdownButton(new GUIContent(AddSpacesToSentence(m.mathType.ToString())), FocusType.Keyboard))
            {
                string[] enumNames = Enum.GetNames(typeof(MathNode.MathType));
                nodePopup = new GeneralNodePopup(new Vector2(680, 260), enumNames, m.mathType.ToString());
                nodePopup.OnCloseEvent += () => { Undo.RecordObject(m, "Enum Change");
                    m.mathType = (MathNode.MathType)Enum.Parse(typeof(MathNode.MathType), nodePopup.EnumValue);
                    NodeEditorWindow.current.Save();
                };
                PopupWindow.Show(buttonRect, nodePopup);
            }
            //Debug.Log(nodePopup.EnumValue);

            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("mathType"), new GUIContent("", ""));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("clamp"), new GUIContent("Clamp", "Limits the output to the range (0.0 to 1.0)."), null);
            switch (m.mathType)
            {
                case MathNode.MathType.Add:
                case MathNode.MathType.Subtract:
                case MathNode.MathType.Multiply:
                case MathNode.MathType.Divide:
                case MathNode.MathType.MultiplyAdd:
                case MathNode.MathType.Logarithm:
                case MathNode.MathType.SquareRoot:
                case MathNode.MathType.InverseSquareRoot:
                case MathNode.MathType.Absolute:
                case MathNode.MathType.Exponent:
                case MathNode.MathType.Minimum:
                case MathNode.MathType.Maximum:
                case MathNode.MathType.LessThan:
                case MathNode.MathType.GreaterThan:
                case MathNode.MathType.Sign:
                case MathNode.MathType.Compare:
                case MathNode.MathType.SmoothMinimum:
                case MathNode.MathType.SmoothMaximum:
                case MathNode.MathType.Round:
                case MathNode.MathType.Floor:
                case MathNode.MathType.Ceil:
                case MathNode.MathType.Truncate:
                case MathNode.MathType.Fraction:
                case MathNode.MathType.Modulo:
                case MathNode.MathType.Wrap:
                case MathNode.MathType.Snap:
                case MathNode.MathType.PingPong:
                case MathNode.MathType.Sine:
                case MathNode.MathType.Cosine:
                case MathNode.MathType.Tangent:
                case MathNode.MathType.Arcsine:
                case MathNode.MathType.Arccosine:
                case MathNode.MathType.Arctangent:
                case MathNode.MathType.Arctan2:
                case MathNode.MathType.HyperbolicSine:
                case MathNode.MathType.HyperbolicCosine:
                case MathNode.MathType.HyperbolicTangent:
                    AutoPropertyField("a", "Value", "floatA");
                    break;
                case MathNode.MathType.Power:
                    AutoPropertyField("a", "Base", "floatA");
                    break;
                case MathNode.MathType.ToRadians:
                    AutoPropertyField("a", "Degrees", "floatA");
                    break;
                case MathNode.MathType.ToDegrees:
                    AutoPropertyField("a", "Radians", "floatA");
                    break;
            }
            switch (m.mathType)
            {
                case MathNode.MathType.Add:
                case MathNode.MathType.Subtract:
                case MathNode.MathType.Multiply:
                case MathNode.MathType.Divide:
                case MathNode.MathType.Minimum:
                case MathNode.MathType.Maximum:
                case MathNode.MathType.Compare:
                case MathNode.MathType.SmoothMinimum:
                case MathNode.MathType.SmoothMaximum:
                case MathNode.MathType.Modulo:
                case MathNode.MathType.Arctan2:
                    AutoPropertyField("b", "Value", "floatB");
                    break;
                case MathNode.MathType.PingPong:
                    AutoPropertyField("b", "Scale", "floatB");
                    break;
                case MathNode.MathType.Snap:
                    AutoPropertyField("b", "Increment", "floatB");
                    break;
                case MathNode.MathType.Wrap:
                    AutoPropertyField("b", "Max", "floatB");
                    break;
                case MathNode.MathType.LessThan:
                case MathNode.MathType.GreaterThan:
                    AutoPropertyField("b", "Threshold", "floatB");
                    break;
                case MathNode.MathType.MultiplyAdd:
                    AutoPropertyField("b", "Multiplier", "floatB");
                    break;
                case MathNode.MathType.Logarithm:
                    AutoPropertyField("b", "Base", "floatB");
                    break;
                case MathNode.MathType.Power:
                    AutoPropertyField("b", "Exponent", "floatB");
                    break;
            }
            switch (m.mathType)
            {
                case MathNode.MathType.Wrap:
                    AutoPropertyField("c", "Min", "floatC");
                    break;
                case MathNode.MathType.SmoothMinimum:
                case MathNode.MathType.SmoothMaximum:
                    AutoPropertyField("c", "Distance", "floatC");
                    break;
                case MathNode.MathType.MultiplyAdd:
                    AutoPropertyField("c", "Addened", "floatC");
                    break;
                case MathNode.MathType.Compare:
                    AutoPropertyField("c", "Epsilon", "floatC");
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }

        void AutoPropertyField(string portNamer, string propertyNamer, string variableNamer)
        {
            if (m == null) m = target as MathNode;
            string propertyDescription = "Input value used for unconnected sockets.";
            if (m.GetPort(portNamer).IsConnected)
                propertyDescription = "";
            NodePort mPort = m.GetInputPort(portNamer);
            mPort.nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(variableNamer), new GUIContent(propertyNamer, propertyDescription), mPort);
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