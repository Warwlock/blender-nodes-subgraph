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
    public class MaterialOutput : Node
    {

        public string portAddName = "name";
        [Output] public float outputvariable_1;

        
        public override object GetValue(NodePort port)
        {
            float a = 151;
            //value = a;
            return a;
        }
    }

    [CustomNodeEditor(typeof(MaterialOutput))]
    public class MaterialOutputEditor : NodeEditor
    {
        MaterialOutput mo;
        List<NodePort> dynamicPortList;
        Rect buttonRect;
        //int portName = 0;
        public override void OnBodyGUI()
        {
            if (mo == null) mo = target as MaterialOutput;
            dynamicPortList = mo.DynamicPorts.ToList();
            serializedObject.Update();
            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("outputvariable_1"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("portAddName"), new GUIContent("Port Name"));
            mo.portAddName = Regex.Replace(mo.portAddName, @"^[^a-zA-Z]|[^a-zA-Z0-9]", "");
            GUILayout.BeginHorizontal();
            Undo.RecordObject(mo, "Enum Change");
            if (GUILayout.Button("   Add Port   "))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Select Port Type"), false, null);
                menu.AddSeparator("");
                AddItemForDynamicPortAdder(menu, "Float", "float");
                AddItemForDynamicPortAdder(menu, "Vector 2", "vector2");
                AddItemForDynamicPortAdder(menu, "Vector 3", "vector3");
                AddItemForDynamicPortAdder(menu, "Vector 4", "vector4");
                //AddItemForDynamicPortAdder(menu, "Color", typeof(Color));
                //AddItemForDynamicPortAdder(menu, "Boolean", "bool");
                //AddItemForDynamicPortAdder(menu, "String", typeof(string));
                NodeEditorWindow.current.onLateGUI += () => ShowContextMenuAtMouse(menu);
            }
            if (GUILayout.Button("Remove Port"))
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < dynamicPortList.Count; i++)
                {
                    AddItemForDynamicPortRemover(menu, dynamicPortList[i].fieldName.Split('_').First(), dynamicPortList[i].fieldName);
                }
                if (dynamicPortList.Count == 0)
                    menu.AddItem(new GUIContent("No Port Available"), false, null);
                else
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Remove All"), false, () =>
                    {
                        mo.ClearDynamicPorts();
                    });
                }
                NodeEditorWindow.current.onLateGUI += () => ShowContextMenuAtMouse(menu);
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Create Lit Shader Outputs"))
            {
                mo.ClearDynamicPorts();

                string oldName = mo.portAddName;

                mo.portAddName = "Color";
                AddDynamicPort("vector4");
                mo.portAddName = "Normal";
                AddDynamicPort("vector3");
                mo.portAddName = "Smoothness";
                AddDynamicPort("float");
                mo.portAddName = "Emission";
                AddDynamicPort("vector4");
                mo.portAddName = "AmbientOcculusion";
                AddDynamicPort("float");
                mo.portAddName = "Metallic";
                AddDynamicPort("float");
                mo.portAddName = "Specular";
                AddDynamicPort("vector4");

                mo.portAddName = oldName;
                dynamicPortList = mo.DynamicPorts.ToList();
            }
            
            for (int i = 0; i < dynamicPortList.Count; i++)
            {
                NodeEditorGUILayout.PortField(new GUIContent(dynamicPortList[i].fieldName.Split('_').First() + " (" + dynamicPortList[i].fieldName.Split('_').Last() + ")"), dynamicPortList[i]);
            }
            serializedObject.ApplyModifiedProperties();
        }

        void AddItemForDynamicPortAdder(GenericMenu menu, string menuPath, string portType)
        {
            menu.AddItem(new GUIContent(menuPath), false, AddDynamicPort, portType);
        }
        void AddDynamicPort(object portType)
        {
            if (mo == null) mo = target as MaterialOutput;
            dynamicPortList = mo.DynamicPorts.ToList();
            bool isContainSameName = false;
            for (int i = 0; i < dynamicPortList.Count; i++)
            {
                if (mo.portAddName.Split('_').First() == dynamicPortList[i].fieldName.Split('_').First())
                {
                    PopupWindow.Show(buttonRect, new WarningPopup("Port " + mo.portAddName.Split('_').First() + " already exists in Material Output."));
                    isContainSameName = true;
                    break;
                }
                else
                    isContainSameName = false;
            }
            if (!isContainSameName)
            {
                if (mo.portAddName.Split('_').First() == "")
                {
                    PopupWindow.Show(buttonRect, new WarningPopup("No port name assigned."));
                }
                else if (mo.portAddName.Split('_').First().Length < 3)
                {
                    PopupWindow.Show(buttonRect, new WarningPopup("Too short port name. Make it at least 3 character."));
                }
                else
                {
                    mo.AddDynamicInput(typeof(string), fieldName: mo.portAddName + "_" + portType, connectionType: Node.ConnectionType.Override, portType: (string)portType);
                }
            }
        }

        void AddItemForDynamicPortRemover(GenericMenu menu, string menuPath, string portName)
        {
            menu.AddItem(new GUIContent(menuPath), false, RemoveDynamicPort, portName);
        }
        void RemoveDynamicPort(object portName)
        {
            if (mo == null) mo = target as MaterialOutput;
            mo.RemoveDynamicPort((string)portName);
        }

        public static void ShowContextMenuAtMouse(GenericMenu menu)
        {
            // Display at cursor position
            Rect r = new Rect(Event.current.mousePosition, new Vector2(0, 0));
            menu.DropDown(r);
        }
    }
}