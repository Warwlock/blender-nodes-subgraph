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

namespace MaterialNodesGraph
{
    [NodeTint("#6B5AAC")]
    [CreateNodeMenu("Converter/Vector Math")]
    public class VectorMath : Node
    {
        [BlenderVector] public Vector3 floatA = new Vector3(0.5f, 0.5f, 0.5f);
        [BlenderVector] public Vector3 floatB = new Vector3(0.5f, 0.5f, 0.5f);
        [BlenderVector] public Vector3 floatC = new Vector3(0.5f, 0.5f, 0.5f);
        [BlenderSingle] public float scale = 0.5f;

        [Input] public string a = "0.5";
        [Input] public string b = "0.5";
        [Input] public string c = "0.5";
        [Input] public string s = "0.5";

        [Output] public string Result;

        [NodeEnum]
        public MathType mathType = MathType.Add;
        public enum MathType
        {
            _Operation, Add, Subtract, Multiply, Divide, MultiplyAdd, _,
            CrossProduct, Project, Reflect, Refract, Faceforward, DotProduct, __,
            Distance, Length, Scale, _a,
            Absolute, Minimum, Maximum, Floor, Ceil, Fraction, Modulo, Wrap, Snap, ____,
            Sine, Cosine, Tangent, _____,
            Normalize
        }

        protected override void Init()
        {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            string a = GetInputValue<string>("a", this.a).Split('?').Last();
            string b = GetInputValue<string>("b", this.b).Split('?').Last();
            string c = GetInputValue<string>("c", this.c).Split('?').Last();
            string s = GetInputValue<string>("s", this.s).Split('?').Last();

            string fa = GetInputValue<string>("a", "").Split('?').First();
            string fb = GetInputValue<string>("b", "").Split('?').First();
            string fc = GetInputValue<string>("c", "").Split('?').First();
            string fs = GetInputValue<string>("s", "").Split('?').First();

            this.a = string.Format("float3({0}, {1}, {2})", floatA.x, floatA.y, floatA.z);
            this.b = string.Format("float3({0}, {1}, {2})", floatB.x, floatB.y, floatB.z);
            this.c = string.Format("float3({0}, {1}, {2})", floatC.x, floatC.y, floatC.z);
            this.s = scale.ToString();

            string ValueID = "_" + Regex.Replace(name, @"[^a-zA-Z0-9]", "") + "_" + Mathf.Abs(GetInstanceID()).ToString();

            if (port.fieldName == "Result")
            {
                if (mathType == VectorMath.MathType.DotProduct || mathType == VectorMath.MathType.Distance || mathType == VectorMath.MathType.Length)
                    return fa + fb + fc + fs +
                    "|float " + ValueID + " = " +
                    MathCalculationFormuls(a, b, c, s) + ";?" + ValueID;
                else
                    return fa + fb + fc + fs +
                    "|float4 " + ValueID + " = " +
                    "float4(" + MathCalculationFormuls(a, b, c, s) + ", 1)" + ";?" + ValueID;
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

        string MathCalculationFormuls(string a, string b, string c, string s)
        {
            string valueInA = a;
            string valueInB = b;
            string valueInC = c;
            switch (mathType)
            {
                case MathType.Add: default: return stringFormatterNode("vector_math_add", valueInA, valueInB, valueInC, s);
                case MathType.Subtract: return stringFormatterNode("vector_math_subtract", valueInA, valueInB, valueInC, s);
                case MathType.Multiply: return stringFormatterNode("vector_math_multiply", valueInA, valueInB, valueInC, s); ;
                case MathType.Divide: return stringFormatterNode("vector_math_divide", valueInA, valueInB, valueInC, s);
                case MathType.CrossProduct: return stringFormatterNode("vector_math_cross", valueInA, valueInB, valueInC, s);
                case MathType.Project: return stringFormatterNode("vector_math_project", valueInA, valueInB, valueInC, s);
                case MathType.Reflect: return stringFormatterNode("vector_math_reflect", valueInA, valueInB, valueInC, s);
                case MathType.DotProduct: return stringFormatterNode("vector_math_dot", valueInA, valueInB, valueInC, s);
                case MathType.Distance: return stringFormatterNode("vector_math_distance", valueInA, valueInB, valueInC, s);
                case MathType.Length: return stringFormatterNode("vector_math_length", valueInA, valueInB, valueInC, s);
                case MathType.Scale: return stringFormatterNode("vector_math_scale", valueInA, valueInB, valueInC, s);
                case MathType.Normalize: return stringFormatterNode("vector_math_normalize", valueInA, valueInB, valueInC, s);
                case MathType.Snap: return stringFormatterNode("vector_math_snap", valueInA, valueInB, valueInC, s);
                case MathType.Floor: return stringFormatterNode("vector_math_floor", valueInA, valueInB, valueInC, s);
                case MathType.Ceil: return stringFormatterNode("vector_math_ceil", valueInA, valueInB, valueInC, s);
                case MathType.Modulo: return stringFormatterNode("vector_math_modulo", valueInA, valueInB, valueInC, s);
                case MathType.Wrap: return stringFormatterNode("vector_math_wrap", valueInA, valueInB, valueInC, s);
                case MathType.Fraction: return stringFormatterNode("vector_math_fraction", valueInA, valueInB, valueInC, s);
                case MathType.Absolute: return stringFormatterNode("vector_math_absolute", valueInA, valueInB, valueInC, s);
                case MathType.Minimum: return stringFormatterNode("vector_math_minimum", valueInA, valueInB, valueInC, s);
                case MathType.Maximum: return stringFormatterNode("vector_math_maximum", valueInA, valueInB, valueInC, s);
                case MathType.Sine: return stringFormatterNode("vector_math_sine", valueInA, valueInB, valueInC, s);
                case MathType.Cosine: return stringFormatterNode("vector_math_cosine", valueInA, valueInB, valueInC, s);
                case MathType.Tangent: return stringFormatterNode("vector_math_tangent", valueInA, valueInB, valueInC, s);
                case MathType.Refract: return stringFormatterNode("vector_math_refract", valueInA, valueInB, valueInC, s);
                case MathType.Faceforward: return stringFormatterNode("vector_math_faceforward", valueInA, valueInB, valueInC, s);
                case MathType.MultiplyAdd: return stringFormatterNode("vector_math_multiply_add", valueInA, valueInB, valueInC, s);
            }
        }

        string stringFormatterNode(string function, string inA, string inB, string inC, string inS)
        {
            return string.Format("{0}({1}, {2}, {3}, {4})",
                function, inA, inB, inC, inS);
        }
    }

    [CustomNodeEditor(typeof(VectorMath))]
    public class VectorMathEditor : NodeEditor
    {
        VectorMath m;
        GeneralNodePopup nodePopup = new GeneralNodePopup(Vector2.zero, new string[0], "Add");
        Rect buttonRect;
        public override void OnBodyGUI()
        {
            if (m == null) m = target as VectorMath;
            serializedObject.Update();
            m.GetInputPort("a").connectionType = Node.ConnectionType.Override;
            m.GetInputPort("b").connectionType = Node.ConnectionType.Override;
            m.GetInputPort("c").connectionType = Node.ConnectionType.Override;
            if (m.mathType == VectorMath.MathType.DotProduct || m.mathType == VectorMath.MathType.Distance || m.mathType == VectorMath.MathType.Length)
            {
                m.GetPort("Result").nodePortType = "float";
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Value", ""));
            }
            else
            {
                m.GetPort("Result").nodePortType = "vector3";
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Result"), new GUIContent("Vector", ""));
            }
            GUILayout.Space(10);

            //Undo.RecordObject(m, "Enum Change");
            if (EditorGUILayout.DropdownButton(new GUIContent(m.mathType.ToString()), FocusType.Keyboard))
            {
                string[] enumNames = Enum.GetNames(typeof(VectorMath.MathType));
                nodePopup = new GeneralNodePopup(new Vector2(190, 330), enumNames, m.mathType.ToString());
                nodePopup.OnCloseEvent += () => {
                    Undo.RecordObject(m, "Enum Change");
                    m.mathType = (VectorMath.MathType)Enum.Parse(typeof(VectorMath.MathType), nodePopup.EnumValue);
                    NodeEditorWindow.current.Save();
                };
                PopupWindow.Show(buttonRect, nodePopup);
            }

            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("mathType"), new GUIContent("", ""));
            switch (m.mathType)
            {
                case VectorMath.MathType.Add:
                case VectorMath.MathType.Subtract:
                case VectorMath.MathType.Multiply:
                case VectorMath.MathType.Divide:
                case VectorMath.MathType.MultiplyAdd:
                case VectorMath.MathType.CrossProduct:
                case VectorMath.MathType.Project:
                case VectorMath.MathType.Reflect:
                case VectorMath.MathType.Refract:
                case VectorMath.MathType.Faceforward:
                case VectorMath.MathType.DotProduct:
                case VectorMath.MathType.Distance:
                case VectorMath.MathType.Length:
                case VectorMath.MathType.Scale:
                case VectorMath.MathType.Normalize:
                case VectorMath.MathType.Absolute:
                case VectorMath.MathType.Minimum:
                case VectorMath.MathType.Maximum:
                case VectorMath.MathType.Floor:
                case VectorMath.MathType.Ceil:
                case VectorMath.MathType.Fraction:
                case VectorMath.MathType.Modulo:
                case VectorMath.MathType.Wrap:
                case VectorMath.MathType.Snap:
                case VectorMath.MathType.Sine:
                case VectorMath.MathType.Cosine:
                case VectorMath.MathType.Tangent:
                    AutoPropertyField("a", "Vector", "floatA");
                    break;
            }
            switch (m.mathType)
            {
                case VectorMath.MathType.Add:
                case VectorMath.MathType.Subtract:
                case VectorMath.MathType.Multiply:
                case VectorMath.MathType.Divide:
                case VectorMath.MathType.CrossProduct:
                case VectorMath.MathType.Project:
                case VectorMath.MathType.Reflect:
                case VectorMath.MathType.Refract:
                case VectorMath.MathType.DotProduct:
                case VectorMath.MathType.Distance:
                case VectorMath.MathType.Minimum:
                case VectorMath.MathType.Maximum:
                case VectorMath.MathType.Modulo:
                    AutoPropertyField("b", "Vector", "floatB");
                    break;
                case VectorMath.MathType.MultiplyAdd:
                    AutoPropertyField("b", "Multiplier", "floatB");
                    break;
                case VectorMath.MathType.Faceforward:
                    AutoPropertyField("b", "Incident", "floatB");
                    break;
                case VectorMath.MathType.Wrap:
                    AutoPropertyField("b", "Max", "floatB");
                    break;
                case VectorMath.MathType.Snap:
                    AutoPropertyField("b", "Increment", "floatB");
                    break;
            }
            switch (m.mathType)
            {
                case VectorMath.MathType.MultiplyAdd:
                    AutoPropertyField("c", "Addened", "floatC");
                    break;
                case VectorMath.MathType.Faceforward:
                    AutoPropertyField("c", "Referance", "floatC");
                    break;
                case VectorMath.MathType.Wrap:
                    AutoPropertyField("c", "Min", "floatC");
                    break;
            }
            switch (m.mathType)
            {
                case VectorMath.MathType.Refract:
                    AutoPropertyField("s", "Ior", "scale");
                    break;
                case VectorMath.MathType.Scale:
                    AutoPropertyField("s", "Scale", "scale");
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }

        void AutoPropertyField(string portNamer, string propertyNamer, string variableNamer)
        {
            if (m == null) m = target as VectorMath;
            string propertyDescription = "Input value used for unconnected sockets.";
            if (m.GetPort(portNamer).IsConnected)
                propertyDescription = "";
            m.GetPort(portNamer).nodePortType = "vector3";
            if(variableNamer == "scale")
                m.GetPort(portNamer).nodePortType = "float";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(variableNamer), new GUIContent(propertyNamer, propertyDescription), m.GetInputPort(portNamer));
        }

        void MathTypeGenericMenu(GenericMenu menu)
        {
            if (m == null) m = target as VectorMath;
            string[] enumNames = Enum.GetNames(typeof(VectorMath.MathType));
            for (int i = 0; i < enumNames.Length; i++)
            {
                if (enumNames[i] == "_" || enumNames[i] == "__" || enumNames[i] == "___" || enumNames[i] == "____" || enumNames[i] == "_____")
                    menu.AddSeparator("");
                else
                    menu.AddItem(new GUIContent(enumNames[i]), false, enumReturnMenu, enumNames[i]);
            }
        }

        void enumReturnMenu(object enumName)
        {
            if (m == null) m = target as VectorMath;
            m.mathType = (VectorMath.MathType)Enum.Parse(typeof(VectorMath.MathType), (string)enumName);
        }

        public void ShowContextMenuAtMouse(GenericMenu menu)
        {
            // Display at cursor position
            Rect r = new Rect(Event.current.mousePosition, new Vector2(0, 0));
            menu.DropDown(r);
            m.graph.TriggerOnValidate();
        }
    }
}